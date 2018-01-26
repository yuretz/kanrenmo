using Kanrenmo.Annotations;

namespace Kanrenmo
{
    public class Disequality: Constraint
    {
        public Disequality([NotNull] Var left, [NotNull] Var right)
        {
            _left = left;
            _right = right;
        }

        public override Context Satisfy(Context context) =>
            Disunify(context, context.Reify(_left), context.Reify(_right));
        
        private Context Disunify(Context context, Var left, Var right)
        {
            if(Equals(left, right))
            {
                return null;
            }

            if (!left.Bound)
            {
                return right.Includes(left) ? context : context.Enforce(this);
            }

            if (!right.Bound)
            {
                return left.Includes(right) ? context : context.Enforce(this);
            }

            if (left is PairVar leftPair 
                && right is PairVar rightPair)
            {
                return new Either(
                    new Disequality(leftPair.Head(), rightPair.Head()), 
                    new Disequality(leftPair.Tail(), rightPair.Tail())).Satisfy(context);
            }

            return context;
        }

        private readonly Var _left;
        private readonly Var _right;
    }
}
