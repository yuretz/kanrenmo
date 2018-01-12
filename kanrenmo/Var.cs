using System;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren variable class
    /// </summary>
    public class Var
    {
        /// <summary>
        /// The empty variable
        /// </summary>
        public static readonly Var Empty = Context.Var<object>(null);

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(int value) => new ValueVar<int>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="bool"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(bool value) => new ValueVar<bool>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="double"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(double value) => new ValueVar<double>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="string"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(string value) => new ValueVar<string>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="char"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(char value) => new ValueVar<char>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Var"/>[] to <see cref="Var"/>.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [NotNull]
        public static implicit operator Var(Var[] variables) => Context.Seq(variables); 

        /// <summary>
        /// Equality (unification) operator == between two variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        [NotNull]
        public static Relation operator ==([NotNull] Var left, [NotNull] Var right) => new Relation(context => context.Unify(left, right));

        /// <summary>
        /// Disequality constraint != between two variables
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        [NotNull]
        public static Relation operator !=([NotNull] Var left, [NotNull] Var right) => new Relation(context => throw new NotImplementedException());

        /// <summary>
        /// The unique variable identifier
        /// </summary>
        public readonly int Id = ++_id;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var"/> is bound to a value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bound; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Bound => false;

        /// <summary>
        /// Determines whether this instance is an empty sequence variable.
        /// </summary>
        public bool IsEmpty => Equals(this, Empty);

        /// <summary>
        /// Gets a value indicating whether this instance is a pair.
        /// </summary>
        public bool IsPair => this is PairVar;

        /// <summary>
        /// Gets a helper relation wrapping this variable.
        /// </summary>
        [NotNull]
        public VarRelation Must => new VarRelation(this);

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => ReferenceEquals(this, obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Id;

        /// <summary>
        /// Constructs the sequence variable by combining this variable (head) with another one (tail).
        /// </summary>
        /// <param name="tail">The tail variable.</param>
        /// <returns>The constructed sequence</returns>
        [NotNull]
        public PairVar Combine(Var tail) => new PairVar(this, tail);

        /// <summary>
        /// Gets the head element of the sequence.
        /// </summary>
        /// <exception cref="InvalidOperationException">when this variable is not a <see cref="PairVar"/></exception>
        public virtual Var Head() => throw new InvalidOperationException($"Variable {Id} is not a sequence");

        /// <summary>
        /// Gets the tail subsequence of the sequence.
        /// </summary>
        /// <exception cref="InvalidOperationException">when this variable is not a <see cref="PairVar"/></exception>
        public virtual Var Tail() => throw new InvalidOperationException($"Variable {Id} is not a sequence");

        private static int _id;
    }
}
