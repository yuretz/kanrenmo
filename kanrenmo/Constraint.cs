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

        [CanBeNull]
        public abstract Context Satisfy([NotNull] Context context);

        [NotNull]
        public override IEnumerable<Context> Execute([NotNull] Context context) =>
            Context.Maybe(Satisfy(context));
    }
}
