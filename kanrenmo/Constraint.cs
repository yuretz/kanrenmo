using System;
using System.Collections.Generic;
using Kanrenmo.Annotations;


namespace Kanrenmo
{
    public abstract class Constraint: Relation
    {
        protected Constraint():base((Func<Context, IEnumerable<Context>>)null)
        {
        }

        [NotNull]
        public abstract override IEnumerable<Context> Execute([NotNull] Context context);

        [NotNull]
        public abstract string ToSExpression(SortedList<int, Var> unbound);
    }
}
