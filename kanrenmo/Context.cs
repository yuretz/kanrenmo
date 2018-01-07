using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren execution context (variable scope)
    /// </summary>
    public partial class Context
    {
        /// <summary>
        /// The relation that always succeeds
        /// </summary>
        public static readonly Relation Succeed = (Var)false == false;

        /// <summary>
        /// The relation that always fails
        /// </summary>
        public static readonly Relation Fail = (Var)true == false;      

        /// <summary>
        /// Solve the specified relation and query the variables.
        /// </summary>
        /// <param name="relation">The relation to solve.</param>
        /// <param name="variables">The variables to query.</param>
        /// <returns>Variable bindings enumeration</returns>
        [NotNull, Pure]
        public static IEnumerable<Binding> Solve([NotNull] Relation relation, params Var[] variables) =>
            new Context()
                // add vars to context
                .With(variables)
                // run the relation in this context
                .Apply(relation)
                // query results from all the returned contexts
                .Select(context => context.QueryAll(variables));


        /// <summary>
        /// Adds scoped variables and relation to this context
        /// </summary>
        /// <param name="relation">The scoped relation.</param>
        /// <param name="variables">The scoped variables.</param>
        /// <returns>Resulting relation</returns>
        [NotNull, Pure]
        public static Relation Declare(Relation relation, params Var[] variables) =>
            new Relation(parent =>
                    // extend the existing scope with fresh vars
                    parent.With(variables)
                        // and apply the relation
                        .Apply(relation)
                        // restore the scope for each resulting context
                        .Select(child => new Context(parent._scope, child._environment)));

        /// <summary>
        /// Invokes the specified relation function.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <returns>Resulting relation</returns>
        [NotNull, Pure]
        public static Relation Invoke([NotNull] Func<Relation> function) =>
            new Relation(function);


        [NotNull, Pure]
        public static Var Var<T>(T value) => (ValueVar<T>) value;

        /// <summary>
        /// Converts variable enumeration to a <see cref="SequenceVar"/>
        /// </summary>
        /// <param name="variables">The variables enumeration.</param>
        /// <returns>new sequence variable instance</returns>
        [NotNull, Pure]
        public static SequenceVar Seq([CanBeNull] IEnumerable<Var> variables) => 
            variables == null ? SequenceVar.Empty : Seq(variables.GetEnumerator());

        /// <summary>
        /// Converts variables to a <see cref="SequenceVar"/>
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>new sequence variable instance</returns>
        [NotNull, Pure]
        public static SequenceVar Seq(params Var[] variables) => Seq(variables.AsEnumerable());

        /// <summary>
        /// Unifies two variables.
        /// </summary>
        /// <param name="left">The unbound variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>Resulting context enumeration</returns>
        [NotNull, Pure]
        internal IEnumerable<Context> Unify(Var left, Var right)
        {
            // use dynamic binding to dispatch to the proper method
            var result = UnifySingle(left, right);
            return result == null ? Enumerable.Empty<Context>() : Enumerable.Repeat(result, 1);
        }

        [NotNull]
        private static SequenceVar Seq([CanBeNull] IEnumerator<Var> variables) =>
            !(variables?.MoveNext() ?? false) ? SequenceVar.Empty : variables.Current.Combine(Seq(variables));

        /// <summary>
        /// Unifies two variables returning the new context.
        /// </summary>
        /// <param name="left">The unbound.</param>
        /// <param name="right">The right.</param>
        /// <returns>The new context or null if unification fails</returns>
        [CanBeNull]
        private Context UnifySingle(Var left, Var right)
        {
            left = Reify(left);
            right = Reify(right);

            if (!left.Bound)
            {
                return UnifyUnbound(left, right);
            }

            if (!right.Bound)
            {
                return UnifyUnbound(right, left);
            }

            if (left is ValueVar leftVal && right is ValueVar rightVal)
            {
                return UnifyValues(leftVal, rightVal);
            }
                
            if (left is SequenceVar leftSeq && right is SequenceVar rightSeq)
            {
                return UnifySequences(leftSeq, rightSeq);
            }

            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="scope">The scope variables.</param>
        /// <param name="environment">The environment containing existing variable bindings.</param>
        private Context(
            [CanBeNull] ImmutableDictionary<Var, Var> scope = null, 
            [CanBeNull] ImmutableDictionary<Var, Var> environment = null)
        {
            _scope = scope ?? ImmutableDictionary<Var, Var>.Empty;
            _environment = environment ?? ImmutableDictionary<Var, Var>.Empty;
        }

        /// <summary>
        /// Adds the specified variables to the context
        /// </summary>
        /// <param name="variables">The variables to add.</param>
        /// <returns>The new context instance</returns>
        [NotNull]
        private Context With(params Var[] variables) =>
            new Context(
                _scope.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))),
                _environment);

        /// <summary>
        /// Applies the specified relation to this context.
        /// </summary>
        /// <param name="relation">The relation to apply.</param>
        /// <returns>The resulting context enumeration</returns>
        [NotNull]
        private IEnumerable<Context> Apply([NotNull] Relation relation) => relation.Execute(this);

        /// <summary>
        /// Unifies two unbound variables.
        /// </summary>
        /// <param name="unbound">The unbound variable.</param>
        /// <param name="other">The other variable.</param>
        /// <returns>The resulting context</returns>
        [NotNull]
        private Context UnifyUnbound(Var unbound, Var other) => new Context(_scope, _environment.Add(unbound, other));

        /// <summary>
        /// Unifies two bound variables.
        /// </summary>
        /// <param name="left">The unbound variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>The resulting context</returns>
        [CanBeNull]
        private Context UnifyValues(ValueVar left, ValueVar right) => 
            Equals(left, right) ? this : null;


        /// <summary>
        /// Unifies two sequence variables.
        /// </summary>
        /// <param name="left">The first sequence variable.</param>
        /// <param name="right">The second sequence variable.</param>
        /// <returns>The resulting context</returns>
        [CanBeNull]
        private Context UnifySequences([NotNull] SequenceVar left, [NotNull] SequenceVar right)
        {
            if (left.IsEmpty || right.IsEmpty)
            {
                return left.IsEmpty && right.IsEmpty ? this : null;
            }
            
            return UnifySingle(left.Head(), right.Head())
                    ?.UnifySingle(left.Tail(), right.Tail());
        }
            

        /// <summary>
        /// Reifies the specified variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reification</returns>
        [NotNull]
        private Var Reify(Var variable)
        {
            switch (variable)
            {
                case ValueVar value:
                    return ReifyImpl(value);
                case SequenceVar sequence:
                    return ReifyImpl(sequence);
                default:
                    return ReifyImpl(variable);
            }
        }

        /// <summary>
        /// Reifies the <see cref="ValueVar"/>.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reified variable</returns>
        [NotNull]
        private Var ReifyImpl(ValueVar variable) => variable;

        /// <summary>
        /// Reifies the variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reified variable</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        private Var ReifyImpl(Var variable)
        {
            if (!_scope.TryGetValue(variable, out var bound))
            {
                bound = variable;
            }

            return !_environment.TryGetValue(bound, out var result) ? bound : Reify(result);
        }

        /// <summary>
        /// Reifies the sequence variable.
        /// </summary>
        /// <param name="sequence">The sequence variable to reify.</param>
        /// <returns>reified sequence</returns>
        [NotNull]
        private Var ReifyImpl([NotNull] SequenceVar sequence) => 
            sequence.IsEmpty ? SequenceVar.Empty : new SequenceVar(Reify(sequence.Head()), Reify(sequence.Tail()));

        /// <summary>
        /// Queries all the provided variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>A dictionary of variables with their bindings</returns>
        [NotNull]
        private Binding QueryAll(IEnumerable<Var> variables) =>
            new Binding(variables.Select(v => new KeyValuePair<Var, Var>(v, Reify(v))));

        private readonly ImmutableDictionary<Var, Var> _environment;
        private readonly ImmutableDictionary<Var, Var> _scope;
    }
}
