using Kanrenmo.Annotations;
using System.Linq;
using static Kanrenmo.Context;

namespace Kanrenmo
{
    public class VarRelation: Relation
    {

        internal VarRelation(Var variable, [CanBeNull] Relation relation = null): 
            base(relation?.Execute ?? (context => Enumerable.Repeat(context, 1)))
        {
            _variable = variable;
        }

        [NotNull, Pure]
        public VarRelation BeEmpty() => new VarRelation(_variable, this & _variable == PairVar.Empty);

        [NotNull, Pure]
        public VarRelation BeEqual([NotNull] Var other) => new VarRelation(_variable, this & _variable == other);

        [NotNull, Pure]
        public VarRelation HaveHead([NotNull] Var head) => new VarRelation(_variable, this & Declare(tail => head.Combine(tail) == _variable));

        [NotNull, Pure]
        public VarRelation HaveTail([NotNull] Var tail) => new VarRelation(_variable, this & Declare(head => head.Combine(tail) == _variable));

        [NotNull, Pure]
        public VarRelation Consist([NotNull] Var head, [NotNull] Var tail) => new VarRelation(_variable, this & head.Combine(tail) == _variable);

        private readonly Var _variable;
    }
}
