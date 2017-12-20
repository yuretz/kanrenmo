namespace Kanrenmo
{
    public abstract class ValueVar: Var
    {
        public abstract object UntypedValue { get; }
    }

    /// <summary>
    /// Kanren variable bound to a value
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <seealso cref="Kanrenmo.Var" />
    public class ValueVar<T> : ValueVar 
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="T"/> to <see cref="ValueVar{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ValueVar<T>(T value) => new ValueVar<T>(value);

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueVar{T}"/> class.
        /// </summary>
        /// <param name="value">The value to bind the variable to.</param>
        public ValueVar(T value) => Value = value;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var" /> is bound to a value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bound; otherwise, <c>false</c>.
        /// </value>
        public override bool Bound => true;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }

        public override object UntypedValue => Value;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as ValueVar<T>;
            return !Equals(other, null) && Equals(other.Value, Value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    }

}
