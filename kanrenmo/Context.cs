using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
        /// Runs the specified relation and query the variables.
        /// </summary>
        /// <param name="relation">The relation to run.</param>
        /// <param name="variables">The variables to query.</param>
        /// <returns>Variable bindings enumeration</returns>
        public static IEnumerable<Binding> Run(Relation relation, params Var[] variables) =>
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
        public static Relation Fresh(Relation relation, params Var[] variables) =>
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
        public static Relation Invoke(Func<Relation> function) =>
            new Relation(function);

        /// <summary>
        /// Unifies two variables.
        /// </summary>
        /// <param name="left">The unbound variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>Resulting context enumeration</returns>
        internal IEnumerable<Context> Unify(Var left, Var right)
        {
            // use dynamic binding to dispatch to the proper method
            var result = UnifySingle(left, right);
            return result == null ? Enumerable.Empty<Context>() : Enumerable.Repeat(result, 1);
        }


        /// <summary>
        /// Unifies two variables returning the new context.
        /// </summary>
        /// <param name="left">The unbound.</param>
        /// <param name="right">The right.</param>
        /// <returns>The new context or null if unification fails</returns>
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
            ImmutableDictionary<Var, Var> scope = null, 
            ImmutableDictionary<Var, Var> environment = null)
        {
            _scope = scope ?? ImmutableDictionary<Var, Var>.Empty;
            _environment = environment ?? ImmutableDictionary<Var, Var>.Empty;
        }

        /// <summary>
        /// Adds the specified variables to the context
        /// </summary>
        /// <param name="variables">The variables to add.</param>
        /// <returns>The new context instance</returns>
        private Context With(params Var[] variables) =>
            new Context(
                _scope.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))),
                _environment);

        /// <summary>
        /// Applies the specified relation to this context.
        /// </summary>
        /// <param name="relation">The relation to apply.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> Apply(Relation relation) => relation.Execute(this);

        /// <summary>
        /// Unifies two unbound variables.
        /// </summary>
        /// <param name="unbound">The unbound variable.</param>
        /// <param name="other">The other variable.</param>
        /// <returns>The resulting context</returns>
        private Context UnifyUnbound(Var unbound, Var other) => new Context(_scope, _environment.Add(unbound, other));

        /// <summary>
        /// Unifies two bound variables.
        /// </summary>
        /// <param name="left">The unbound variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>The resulting context</returns>
        private Context UnifyValues(ValueVar left, ValueVar right) => 
            Equals(left, right) ? this : null;

        /// <summary>
        /// Unifies two sequence variables.
        /// </summary>
        /// <param name="left">The first sequence variable.</param>
        /// <param name="right">The second sequence variable.</param>
        /// <returns>The resulting context</returns>
        private Context UnifySequences(SequenceVar left, SequenceVar right)
        {
            var result = this;

            using (var leftEnumerator = left.GetEnumerator())
            {
                using (var rightEnumerator = right.GetEnumerator())
                {
                    while (true)
                    {
                        var moreLeft = leftEnumerator.MoveNext();
                        var moreRight = rightEnumerator.MoveNext();

                        if (!(moreLeft || moreRight))
                        {
                            return result;
                        }

                        if (moreLeft ^ moreRight)
                        {
                            return null;
                        }

                        var leftItem = leftEnumerator.Current;
                        var rightItem = rightEnumerator.Current;
                        result = result.UnifySingle(leftItem, rightItem);
                    }
                }
            }


        }

        /// <summary>
        /// Reifies the specified variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reification</returns>
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
        private Var ReifyImpl(ValueVar variable) => variable;

        /// <summary>
        /// Reifies the variable.
        /// </summary>
        /// <param name="variable">The variable to reify.</param>
        /// <returns>Reified variable</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private Var ReifyImpl(Var variable)
        {
            if (!_scope.TryGetValue(variable, out var bound))
            {
                throw new InvalidOperationException($"Variable Id ${variable.Id} is not in scope");
            }

            while (true)
            {
                if (!_environment.TryGetValue(bound, out var result))
                {
                    return bound;
                }

                bound = result;
            }
        }

        /// <summary>
        /// Reifies the sequence variable.
        /// </summary>
        /// <param name="sequence">The sequence variable to reify.</param>
        /// <returns>reified sequence</returns>
        private Var ReifyImpl(SequenceVar sequence) => 
            sequence.IsEmpty ? SequenceVar.Empty : new SequenceVar(Reify(sequence.Head), Reify(sequence.Tail));

        /// <summary>
        /// Queries all the provided variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>A dictionary of variables with their bindings</returns>
        private Binding QueryAll(IEnumerable<Var> variables) =>
            new Binding(variables.Select(v => new KeyValuePair<Var, Var>(v, Reify(v))));

        private readonly ImmutableDictionary<Var, Var> _environment;
        private readonly ImmutableDictionary<Var, Var> _scope;
    }
}
