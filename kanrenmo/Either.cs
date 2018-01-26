namespace Kanrenmo
{
    public class Either: Constraint
    {
        public Either(Constraint left, Constraint right)
        {
            _left = left;
            _right = right;
        }

        public override Context Satisfy(Context context)
        {
            var leftContext = _left.Satisfy(context);
            if (leftContext == context)
            {
                // left constraint satisfied
                return context;
            }

            var rightContext = _right.Satisfy(context);
            if (rightContext == context)
            {
                // right constraint satisfied
                return context;
            }

            if (leftContext != null && rightContext != null)
            {
                // both constraints remain undecided yet
                return context.Enforce(this);
            }

            // one or both are not satisfied
            return leftContext ?? rightContext;
        }

        private readonly Constraint _left;
        private readonly Constraint _right;
    }
}
