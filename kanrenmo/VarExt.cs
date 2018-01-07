using Kanrenmo.Annotations;
using static Kanrenmo.Context;

namespace Kanrenmo
{
    public static class VarExt
    {
        [NotNull, Pure]
        public static Relation HasNothing(this Var sequence) => sequence == SequenceVar.Empty;

        [NotNull, Pure]
        public static Relation HasHead(this Var sequence, Var head) => Declare(tail => head.Combine(tail) == sequence);

        [NotNull, Pure]
        public static Relation HasTail(this Var sequence, Var tail) => Declare(head => head.Combine(tail) == sequence);

        [NotNull, Pure]
        public static Relation Consists(this Var sequence, [NotNull] Var head, [NotNull] Var tail) => head.Combine(tail) == sequence;
    }
}