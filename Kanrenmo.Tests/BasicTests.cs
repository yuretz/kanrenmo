using System.Linq;
using JetBrains.Annotations;
using Xunit;
using static Kanrenmo.Context;


namespace Kanrenmo.Tests
{
    public class BasicTests
    {
        [Theory]
        [InlineData(5)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData("foo")]
        public void VarCanBeBound<T>(T value)
        {
            var results = Solve(new ValueVar<T>(value) == _q, _q).ToList();
            Assert.Single(results);
            Assert.Single(results[0]);
            Assert.True(results[0].TryGetValue(_q, out var x));
            Assert.IsType<ValueVar<T>>(x);
            Assert.Equal(value, ((ValueVar<T>)x).Value);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(5, 6, 7)]
        public void OrReturnsMultipleResults([NotNull] params int[] values)
        {
            
            var relation = values.Aggregate(Relation.Failure, (r, i) => r | _q == i);
            var results = Solve(relation, _q).ToList();
            Assert.Equal(values.Length, results.Count);
            Assert.All(results, r => Assert.Single(r));
            foreach (var value in values)
            {
                Assert.Contains(results, d => d.TryGetValue(_q, out var x) && (x as ValueVar<int>)?.Value == value);
            }
        }

        [Fact]
        public void OccursCheckIsDoneForPairHeads()
        {
            Assert.Empty(Solve(q => q == Pair(q, 1)));
        }
        
        [Fact]
        public void OccursCheckIsDoneForPairTails()
        {
            Assert.Empty(Solve(q => q == Pair(1, q)));
        }

        [Fact]
        public void OccursCheckIsDoneForSequences()
        {
            Assert.Empty(Solve(q => q == Seq(1, 2, q, 3, 4)));
        }

        [Fact]
        public void OccursCheckIsDoneWeirdStructs()
        {
            Assert.Empty(
                Solve(q =>
                    Declare(r => q == Seq(1, 2, Pair(Seq(5, 7, 8), Seq(Pair(Pair(2, Seq(2, r, 3)), 4))), 3, 4)
                                 & r == q)));
        }

        private readonly Var _q = new Var();
    }
}
