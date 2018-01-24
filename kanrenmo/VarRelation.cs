using System;
using System.Collections.Generic;
using Kanrenmo.Annotations;
using System.Linq;
using static Kanrenmo.Context;

namespace Kanrenmo
{
    /// <summary>
    /// Variable relation class. Provides fluent API to compose more relations on top of the same variable
    /// </summary>
    /// <seealso cref="Kanrenmo.Relation" />
    public class VarRelation: Relation
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VarRelation"/> class.
        /// </summary>
        /// <param name="variable">The variable to wrap.</param>
        /// <param name="relation">The relation.</param>
        internal VarRelation(Var variable, [CanBeNull] Relation relation = null): 
            base((relation??Unit).Execute)
        {
            _variable = variable;
        }

        /// <summary>
        /// Variable must be empty
        /// </summary>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'emptyo'</remarks>
        [NotNull, Pure]
        public VarRelation BeEmpty() => new VarRelation(_variable, this & _variable == Var.Empty);

        /// <summary>
        /// Variable must be equal to another variable
        /// </summary>
        /// <param name="other">The other variable.</param>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'eqo'</remarks>
        [NotNull, Pure]
        public VarRelation BeEqual([NotNull] Var other) => new VarRelation(_variable, this & _variable == other);

        /// <summary>
        /// Pair or sequence variable must have a certain head part
        /// </summary>
        /// <param name="head">The head variable.</param>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'caro'</remarks>
        [NotNull, Pure]
        public VarRelation HaveHead([NotNull] Var head) => new VarRelation(_variable, this & Declare(tail => head.Combine(tail) == _variable));

        /// <summary>
        /// Pair or sequence variable must have a certain tail part
        /// </summary>
        /// <param name="tail">The tail variable.</param>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'cdro'</remarks>
        [NotNull, Pure]
        public VarRelation HaveTail([NotNull] Var tail) => new VarRelation(_variable, this & Declare(head => head.Combine(tail) == _variable));

        /// <summary>
        /// Pair or sequence variable must have consist of certain head and tail parts
        /// </summary>
        /// <param name="head">The head variable.</param>
        /// <param name="tail">The tail variable.</param>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'conso' the magnificento</remarks>
        [NotNull, Pure]
        public VarRelation Consist([NotNull] Var head, [NotNull] Var tail) => new VarRelation(_variable, this & head.Combine(tail) == _variable);

        /// <summary>
        /// Variable must be a pair variable
        /// </summary>
        /// <returns>the new <see cref="VarRelation"/> instance</returns>
        /// <remarks>Similar to Reasoned Schemer's 'pairo'</remarks>
        [NotNull, Pure]
        public VarRelation BePair() => new VarRelation(_variable, this & Declare((head, tail) => _variable.Must.Consist(head, tail)));

        private readonly Var _variable;
    }
}
