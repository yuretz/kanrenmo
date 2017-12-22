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
        /// Runs the specified relation and query the variables.
        /// </summary>
        /// <param name="relation">The relation to run.</param>
        /// <param name="variables">The variables to query.</param>
        /// <returns>Variable bindings enumeration</returns>
        public static IEnumerable<IReadOnlyDictionary<Var, Var>> Run(Relation relation, params Var[] variables) =>
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
                        .Select(child => new Context(parent._scope, child._bindings)));

        /// <summary>
        /// Unifies two variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>Resulting context enumeration</returns>
        internal IEnumerable<Context> Unify(Var left, Var right) =>
            // use dynamic binding to dispatch to the proper method
            UnifyImpl((dynamic)Reify(left), (dynamic)Reify(right));


        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// </summary>
        /// <param name="scope">The scope variables.</param>
        /// <param name="bindings">The bindings tree.</param>
        private Context(
            ImmutableDictionary<Var, Var> scope = null, 
            ImmutableDictionary<Var, Var> bindings = null)
        {
            _scope = scope ?? ImmutableDictionary<Var, Var>.Empty;
            _bindings = bindings ?? ImmutableDictionary<Var, Var>.Empty;
        }

        /// <summary>
        /// Adds the specified variables to the context
        /// </summary>
        /// <param name="variables">The variables to add.</param>
        /// <returns>The new context instance</returns>
        private Context With(params Var[] variables) =>
            new Context(
                _scope.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))),
                _bindings);

        /// <summary>
        /// Applies the specified relation to this context.
        /// </summary>
        /// <param name="relation">The relation to apply.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> Apply(Relation relation) => relation.Execute(this);

        /// <summary>
        /// Unifies an unbound and a bound variables.
        /// </summary>
        /// <typeparam name="T">Bound variable value type</typeparam>
        /// <param name="left">The unbound variable.</param>
        /// <param name="right">The bound variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl<T>(Var left, ValueVar<T> right)
        {
            yield return new Context(_scope, _bindings.Add(left, right));
        }

        /// <summary>
        /// Unifies a bound and an unbound variables.
        /// </summary>
        /// <typeparam name="T">Bound variable value type</typeparam>
        /// <param name="left">The bound variable.</param>
        /// <param name="right">The unbound variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl<T>(ValueVar<T> left, Var right) => UnifyImpl(right, left);

        /// <summary>
        /// Unifies two unbound variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl(Var left, Var right)
        {
            var unified = new Var();
            yield return new Context(
                _scope,
                _bindings
                    .Add(left, unified)
                    .Add(right, unified));
        }

        /// <summary>
        /// Unifies two bound variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl<TLeft, TRight>(ValueVar<TLeft> left, ValueVar<TRight> right)
        {
            if (Equals(left, right))
            {
                yield return this;
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
                if (!_bindings.TryGetValue(bound, out var result))
                {
                    return bound;
                }

                bound = result;
            }
        }

        /// <summary>
        /// Queries all the provided variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>A dictionary of variables with their bindings</returns>
        private ImmutableDictionary<Var, Var> QueryAll(Var[] variables) =>
            ImmutableDictionary<Var, Var>.Empty.AddRange(variables.Select(v => new KeyValuePair<Var, Var>(v, Reify(v))));

        private readonly ImmutableDictionary<Var, Var> _bindings;
        private readonly ImmutableDictionary<Var, Var> _scope;
    }
}
