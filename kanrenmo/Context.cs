﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Kanrenmo
{
    public class Context
    {

        public static IEnumerable<IReadOnlyDictionary<Var, Var>> Run(Relation relation, params Var[] variables) => 
            new Context()
                .With(variables)
                .Apply(relation)
                .SelectMany(context => context.QueryAll(variables));


        public static Relation Fresh(Relation relation, params Var[] variables) =>
            new Relation(parent =>
                Enumerable.Repeat(
                    parent.With(new Relation(context =>
                        new Context(context._bindings).With(variables).Apply(relation))), 1));


        internal IEnumerable<Context> Unify(Var left, Var right) =>
            UnifyImpl((dynamic)Get(left), (dynamic)Get(right));


        private Context(ImmutableDictionary<Var, Var> bindings = null, Relation children = null)
        {
            _bindings = bindings ?? ImmutableDictionary<Var, Var>.Empty;
            _children = children ?? Relation.Empty;
        }

        private Context With(Relation children) => new Context(_bindings, _children | children);

        private Context With(params Var[] variables) => new Context(_bindings.SetItems(variables.Select(v => new KeyValuePair<Var, Var>(v, new Var()))), _children);

        private IEnumerable<Context> Apply(Relation relation) => relation.Exec(this);

        private IEnumerable<Context> UnifyImpl<T>(Var left, BoundVar<T> right)
        {
            yield return new Context(_bindings.Add(left, right), _children);
        }

        private IEnumerable<Context> UnifyImpl<T>(BoundVar<T> left, Var right) => UnifyImpl(right, left);

        private IEnumerable<Context> UnifyImpl(Var left, Var right)
        {
            var unified = new Var();
            yield return new Context(
                _bindings
                    .Add(left, unified)
                    .Add(right, unified),
                _children);
        }

        private IEnumerable<Context> UnifyImpl<TLeft, TRight>(BoundVar<TLeft> left, BoundVar<TRight> right)
        {
            if (Equals(left, right))
            {
                yield return this;
            }
        }


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

        private IEnumerable<ImmutableDictionary<Var, Var>> QueryAll(Var[] variables)
        {

            // TODO: replace it with recursive LINQ Aggregate() expression, with the help of Head-Tail split function 
            if (variables.Length == 0)
            {
                yield return ImmutableDictionary<Var, Var>.Empty;
                yield break;
            }

            foreach (var result in QueryOne(variables[0]))
            {
                foreach (var goal in QueryAll(variables.Skip(1).ToArray()))
                {
                    yield return goal.Add(variables[0], result);
                }
            }
        }

        private IEnumerable<Var> QueryOne(Var variable)
        {
            if (!_bindings.TryGetValue(variable, out var result))
            {
                return _children?.Exec(this).SelectMany(c => c.QueryOne(variable));
            }

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
