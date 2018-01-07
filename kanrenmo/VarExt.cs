using Kanrenmo.Annotations;
using static Kanrenmo.Context;

namespace Kanrenmo
{
    public static class VarExt
    {
        [NotNull]
        public static Relation HasHead(this Var sequence, Var head) => Declare(tail => head.Combine(tail) == sequence);

        [NotNull]
        public static Relation HasTail(this Var sequence, Var tail) => Declare(head => head.Combine(tail) == sequence);

        [NotNull]
        public static Relation Consists(this Var sequence, Var head, Var tail) => Invoke((s, h, t) => h.Combine(t) == s, sequence, head, tail);
    }
}