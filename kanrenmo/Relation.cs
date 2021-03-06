﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren relation function wrapper class
    /// </summary>
    public class Relation
    {
        /// <summary>
        /// The failure relation
        /// </summary>
        public static readonly Relation Failure = new Relation(_ => Context.Nothing);

        /// <summary>
        /// The identity relation, 
        /// </summary>
        public static readonly Relation Identity = new Relation(Context.Just);

        /// <summary>
        /// Disjunction operator between two relations (same as "conde")
        /// </summary>
        /// <param name="left">left relation</param>
        /// <param name="right">right relation</param>
        /// <returns>operation result</returns>
        [NotNull]
        public static Relation operator |([NotNull] Relation left, [NotNull] Relation right) =>
            new Relation(context => Union(context, left, right));
        /// <summary>
        /// Conjunction operator between two relations
        /// </summary>
        /// <param name="left">left relation</param>
        /// <param name="right">right relation</param>
        /// <returns>operation result</returns>
        [NotNull]
        public static Relation operator &([NotNull] Relation left, [NotNull] Relation right) =>
            new Relation(context => Product(context, left, right));

        /// <summary>
        /// Constructs and initializes the class instance
        /// </summary>
        /// <param name="execute">relation function to wrap</param>
        public Relation(Func<Context, IEnumerable<Context>> execute) => _execute = execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Relation"/> class.
        /// </summary>
        /// <param name="relation">The lazy relation constructor function.</param>
        public Relation([NotNull] Func<Relation> relation) : this(context => relation().Execute(context))
        {
        }

        /// <summary>
        /// Executes the relation
        /// </summary>
        public virtual IEnumerable<Context> Execute(Context context) =>
            _execute?.Invoke(context) ?? Context.Nothing;  

        private static IEnumerable<Context> Union([NotNull] Context context, [NotNull] Relation left, [NotNull] Relation right)
        {
            IEnumerator<Context> leftEnum = null;
            IEnumerator<Context> rightEnum = null;
            var anyLeft = true;
            var anyRight = true;
            while (anyLeft || anyRight)
            {
                if (anyLeft 
                    && (anyLeft = (leftEnum ?? (leftEnum = left.Execute(context).GetEnumerator())).MoveNext()))
                {
                    yield return leftEnum.Current;
                }

                if (anyRight 
                    && (anyRight = (rightEnum ?? (rightEnum = right.Execute(context).GetEnumerator())).MoveNext()))
                {
                    yield return rightEnum.Current;
                }
            }
        }

        private static IEnumerable<Context> Product([NotNull] Context context, [NotNull] Relation left, [NotNull] Relation right) => 
            left.Execute(context).SelectMany(right.Execute);
           
        private readonly Func<Context, IEnumerable<Context>> _execute;
    }
}
