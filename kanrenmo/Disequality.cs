using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    public class Disequality: Constraint
    {
        public Disequality([NotNull] Var left, [NotNull] Var right)
        {
            _pairs = ImmutableList<PairVar>.Empty.Add(new PairVar(left, right));
        }

        public override IEnumerable<Context> Execute(Context context)
        {

            var pairs = ImmutableList<PairVar>.Empty.AddRange(
                _pairs.SelectMany(pair => Expand(context, pair))
                    .Distinct(Comparer));
            var valid = ImmutableList<PairVar>.Empty.ToBuilder();

            foreach (var pair in pairs)
            {
                var unified = context.Unify(pair.Head(), pair.Tail()).ToList();

                if (!unified.Any())
                {
                    return Context.Just(context);
                }

                var current = context;
                if (unified.Any(c => c != current))
                {
                    valid.Add(pair);
                }        
            }

            return valid.Any() ? context.Enforce(new Disequality(valid.ToImmutable())) : Context.Nothing;
        }

        public override string ToSExpression(SortedList<int, Var> unbound) =>
            "(!= ("
            + string.Join(
                " ",
                _pairs.Select(
                    pair => "("
                            + pair.Head().ToSExpression(unbound)
                            + " "
                            + pair.Tail().ToSExpression(unbound) + ")"))
            + "))";

        private class DisequalityPairComparer : IEqualityComparer<PairVar>
        {
            public bool Equals([NotNull] PairVar x, [NotNull] PairVar y) =>
                Equals(x.Head(), y.Head()) && Equals(x.Tail(), y.Tail())
                || Equals(x.Head(), y.Tail()) && Equals(x.Tail(), y.Head());


            public int GetHashCode([NotNull] PairVar pair) => pair.Head().GetHashCode() ^ pair.Tail().GetHashCode();
        }
        private static readonly DisequalityPairComparer Comparer = new DisequalityPairComparer();

        private Disequality(ImmutableList<PairVar> pairs) => _pairs = pairs;

        private IEnumerable<PairVar> Expand([NotNull] Context context, [NotNull] PairVar pair)
        {
            var left = context.Reify(pair.Head());
            var right = context.Reify(pair.Tail());

            if (left is PairVar leftPair
                && right is PairVar rightPair)
            {
                return Expand(context, new PairVar(leftPair.Head(), rightPair.Head()))
                    .Union(Expand(context, new PairVar(leftPair.Tail(), rightPair.Tail())), Comparer);
            }

            return Enumerable.Repeat(new PairVar(left, right), 1);
        }

        private readonly ImmutableList<PairVar> _pairs;
    }
}
