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
            new Relation(context => Union(context, left, right));
        /// <summary>
        /// Conjunction operator between two relations
        /// </summary>
        /// <param name="left">left relation</param>
        /// <param name="right">right relation</param>
        /// <returns>operation result</returns>
        public static Relation operator &(Relation left, Relation right) =>
            new Relation(context => Product(context, left, right));

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

        private static IEnumerable<Context> Union(Context context, Relation left, Relation right)
        {
            IEnumerator<Context> leftEnum = null;
            IEnumerator<Context> rightEnum = null;
            bool any;
            do
            {
                any = false;

                leftEnum = leftEnum ?? left.Execute(context).GetEnumerator();
                if (leftEnum.MoveNext())
                {
                    any = true;
                    yield return leftEnum.Current;
                }

                rightEnum = rightEnum ?? right.Execute(context).GetEnumerator();
                if (rightEnum.MoveNext())
                {
                    any = true;
                    yield return rightEnum.Current;
                }

            } while (any);
        }

        private static IEnumerable<Context> Product(Context context, Relation left, Relation right) => 
            left.Execute(context).SelectMany(c => right.Execute(c));
    }
}
