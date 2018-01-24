using Kanrenmo.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Kanrenmo
{
    public class Disequality: Constraint
    {
        public Disequality([NotNull] Var left, [NotNull] Var right)
        {
            _left = left;
            _right = right;
        }

        public override IEnumerable<Context> Execute([NotNull] Context context) =>
            Disunify(context, context.Reify(_left), context.Reify(_right));
        
        private IEnumerable<Context> Disunify(Context context, Var left, Var right)
        {
            if(Equals(left, right))
            {
                return Enumerable.Empty<Context>();
            }

            if(!(left.Bound && right.Bound))
            {
                return Enumerable.Repeat(context.Enforce(this), 1);
            }

            if (left is ValueVar 
                && right is ValueVar)
            {
                return Enumerable.Repeat(context, 1);
            }

            if (left is PairVar leftPair 
                && right is PairVar rightPair)
            {
                return (leftPair.Head() != rightPair.Head() 
                        | leftPair.Tail() != rightPair.Tail()).Execute(context);
            }

            return Enumerable.Repeat(context, 1);
        }

        private readonly Var _left;
        private readonly Var _right;
    }
}
