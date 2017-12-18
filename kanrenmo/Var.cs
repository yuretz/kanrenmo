using System;

namespace Kanrenmo
{
    /// <summary>
    /// Kanren variable class
    /// </summary>
    public class Var
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Var(int value) => new ValueVar<int>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Var(bool value) => new ValueVar<bool>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Double"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Var(double value) => new ValueVar<double>(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Var"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Var(string value) => new ValueVar<string>(value);

        /// <summary>
        /// Equality (unification) operator == between two variables.
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static Relation operator ==(Var left, Var right) => new Relation(context => context.Unify(left, right));

        /// <summary>
        /// Disequality constraint != between two variables
        /// </summary>
        /// <param name="left">The left variable.</param>
        /// <param name="right">The right variable.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static Relation operator !=(Var left, Var right) => new Relation(context => throw new NotImplementedException());

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
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => ReferenceEquals(this, obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Id;

        private static int _id;
    }
}
