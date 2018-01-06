using Kanrenmo.Annotations;
using static Kanrenmo.Context;

namespace Kanrenmo
{
    public static class VarExt
    {
        [NotNull]
        public static Relation Heado(this Var sequence, Var head) => Fresh(tail => head.Cons(tail) == sequence);

        [NotNull]
        public static Relation Tailo(this Var sequence, Var tail) => Fresh(head => head.Cons(tail) == sequence);

        [NotNull]
        public static Relation Conso(this Var sequence, Var head, Var tail) => Invoke((s, h, t) => h.Cons(t) == s, sequence, head, tail);
    }
}