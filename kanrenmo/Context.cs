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
        /// Converts variable enumeration to a sequence of nested <see cref="PairVar"/>
        /// </summary>
        /// <param name="variables">The variables enumeration.</param>
        /// <returns>new sequence of nested pair variable instances</returns>
        [NotNull, Pure]
        public static PairVar Seq([CanBeNull] IEnumerable<Var> variables) => 
            variables == null ? PairVar.Empty : Seq(variables.GetEnumerator());

        /// <summary>
        /// Converts variables to a to a sequence of nested <see cref="PairVar"/>
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>new sequence of nested pair variable instances</returns>
        [NotNull, Pure]
        public static PairVar Seq(params Var[] variables) => Seq(variables.AsEnumerable());

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
        private static PairVar Seq([CanBeNull] IEnumerator<Var> variables) =>
            !(variables?.MoveNext() ?? false) ? PairVar.Empty : variables.Current.Combine(Seq(variables));

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

            if (Equals(left, right))
            {
                return this;
            }

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
                
            if (left is PairVar leftSeq && right is PairVar rightSeq)
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
        /// Unifies two pair variables.
        /// </summary>
        /// <param name="left">The first pair variable.</param>
        /// <param name="right">The second pair variable.</param>
        /// <returns>The resulting context</returns>
        [CanBeNull]
        private Context UnifySequences([NotNull] PairVar left, [NotNull] PairVar right)
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
                case PairVar sequence:
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
        /// Reifies the pair variable.
        /// </summary>
        /// <param name="pair">The pair variable to reify.</param>
        /// <returns>reified pair</returns>
        [NotNull]
        private Var ReifyImpl([NotNull] PairVar pair) => 
            pair.IsEmpty ? PairVar.Empty : new PairVar(Reify(pair.Head()), Reify(pair.Tail()));

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
