using System;
using System.Collections.Generic;

namespace Kanrenmo
{
    public abstract class Constraint: Relation
    {
        protected Constraint():base((Func<Context, IEnumerable<Context>>)null)
        {
        }
    }
}
