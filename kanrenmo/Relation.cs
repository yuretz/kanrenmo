using System;
using System.Collections.Generic;
using System.Linq;

namespace Kanrenmo
{
    public class Relation
    {
        public static readonly Relation Empty = new Relation(context => Enumerable.Empty<Context>());

        public static Relation operator |(Relation left, Relation right) =>
            new Relation(context => left.Exec(context).Union(right.Exec(context)));

        public static Relation operator &(Relation left, Relation right) =>
            new Relation(context => left.Exec(context).Select(right.Exec).SelectMany(s => s));

        public Relation(Func<Context, IEnumerable<Context>> exec) => Exec = exec;

        public readonly Func<Context, IEnumerable<Context>> Exec;
    }
}
