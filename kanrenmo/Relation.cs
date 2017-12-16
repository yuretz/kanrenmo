using System;
using System.Collections.Generic;
using System.Linq;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren relation function wrapper class
    /// </summary>
    public class Relation
    {
        /// <summary>
        /// Empty relation
        /// </summary>
        public static readonly Relation Empty = new Relation(context => Enumerable.Empty<Context>());

        /// <summary>
        /// Disjunction operator between two relations (same as "conde")
        /// </summary>
        /// <param name="left">left relation</param>
        /// <param name="right">right relation</param>
        /// <returns>operation result</returns>
        public static Relation operator |(Relation left, Relation right) => 
            new Relation(context => left.Execute(context).Union(right.Execute(context)));

        /// <summary>
        /// Conjunction operator between two relations
        /// </summary>
        /// <param name="left">left relation</param>
        /// <param name="right">right relation</param>
        /// <returns>operation result</returns>
        public static Relation operator &(Relation left, Relation right) =>
            new Relation(context => left.Execute(context).Select(right.Execute).SelectMany(s => s));

        /// <summary>
        /// Constructs and initializes the class instance
        /// </summary>
        /// <param name="execute">relation function to wrap</param>
        public Relation(Func<Context, IEnumerable<Context>> execute) => Execute = execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Relation"/> class.
        /// </summary>
        /// <param name="relation">The lazy relation constructor function.</param>
        public Relation(Func<Relation> relation) : this(context => relation().Execute(context))
        {
        }

        /// <summary>
        /// Executes the relation
        /// </summary>
        public readonly Func<Context, IEnumerable<Context>> Execute;
    }
}
