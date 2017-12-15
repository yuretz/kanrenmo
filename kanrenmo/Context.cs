using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren execution context (variable scope)
    /// </summary>
    public class Context
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
                .SelectMany(context => context.QueryAll(variables));


        /// <summary>
        /// Adds scoped variables and relation to this context
        /// </summary>
        /// <param name="relation">The scoped relation.</param>
        /// <param name="variables">The scoped variables.</param>
        /// <returns>Resulting relation</returns>
        public static Relation Fresh(Relation relation, params Var[] variables) =>
            new Relation(parent =>
                Enumerable.Repeat(
                    parent.With(new Relation(context =>
                        new Context(context._bindings).With(variables).Apply(relation))), 1));


        /// <summary>
        /// Unifies two variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>Resulting context enumeration</returns>
        internal IEnumerable<Context> Unify(Var left, Var right) =>
            // use dynamic binding to dispatch to the proper method
            UnifyImpl((dynamic)Get(left), (dynamic)Get(right));


        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="bindings">The bindings tree.</param>
        /// <param name="children">The child context relation.</param>
        private Context(ImmutableDictionary<Var, Var> bindings = null, Relation children = null)
        {
            _bindings = bindings ?? ImmutableDictionary<Var, Var>.Empty;
            _children = children ?? Relation.Empty;
        }

        /// <summary>
        /// Adds the specified child context relation
        /// </summary>
        /// <param name="children">The child context relation to add.</param>
        /// <returns>The new context instance</returns>
        private Context With(Relation children) => new Context(_bindings, _children | children);

        /// <summary>
        /// Adds the specified variables to the context
        /// </summary>
        /// <param name="variables">The variables to add.</param>
        /// <returns>The new context instance</returns>
        private Context With(params Var[] variables) => new Context(_bindings.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))), _children);

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
        private IEnumerable<Context> UnifyImpl<T>(Var left, BoundVar<T> right)
        {
            yield return new Context(_bindings.Add(left, right), _children);
        }

        /// <summary>
        /// Unifies a bound and an unbound variables.
        /// </summary>
        /// <typeparam name="T">Bound variable value type</typeparam>
        /// <param name="left">The bound variable.</param>
        /// <param name="right">The unbound variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl<T>(BoundVar<T> left, Var right) => UnifyImpl(right, left);

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
                _bindings
                    .Add(left, unified)
                    .Add(right, unified),
                _children);
        }

        /// <summary>
        /// Unifies two bound variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>The resulting context enumeration</returns>
        private IEnumerable<Context> UnifyImpl<TLeft, TRight>(BoundVar<TLeft> left, BoundVar<TRight> right)
        {
            if (Equals(left, right))
            {
                yield return this;
            }
        }


        /// <summary>
        /// Gets the specified variable binding from a tree.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>The corresponding variable binding</returns>
        private Var Get(Var variable)
        {
            while (true)
            {
                if (!_bindings.TryGetValue(variable, out var result))
                {
                    return variable;
                }

                variable = result;
            }
        }

        /// <summary>
        /// Queries all the provided variables.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns></returns>
        private IEnumerable<ImmutableDictionary<Var, Var>> QueryAll(Var[] variables)
        {

            // TODO: replace it with recursive LINQ Aggregate() expression, with the help of Head-Tail split function 
            if (variables.Length == 0)
            {
                // no variables to query, recursion stops
                yield return ImmutableDictionary<Var, Var>.Empty;
                yield break;
            }

            // for each resulting variable returned by querying the first one
            foreach (var result in QueryOne(variables[0]))
            {
                // for each result set from querying the rest of the variables
                foreach (var goal in QueryAll(variables.Skip(1).ToArray()))
                {
                    // combine the results
                    yield return goal.Add(variables[0], result);
                }
            }
        }

        /// <summary>
        /// Queries one variable.
        /// </summary>
        /// <param name="variable">The variable to query.</param>
        /// <returns>the resulting variable</returns>
        private IEnumerable<Var> QueryOne(Var variable)
        {
            // check if variable is present in bindings tree
            if (!_bindings.TryGetValue(variable, out var result))
            {
                // no, it's not, so query the child contexts
                return _children.Execute(this).SelectMany(c => c.QueryOne(variable));
            }

            // walk through the bindings tree and search for the root node
            while (true)
            {
                variable = result;

                if (!_bindings.TryGetValue(variable, out result))
                {
                    return Enumerable.Repeat(variable, 1);
                }
            }
        }

        private readonly ImmutableDictionary<Var, Var> _bindings;
        private readonly Relation _children;
    }
}
