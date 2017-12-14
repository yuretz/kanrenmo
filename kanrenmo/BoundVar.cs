namespace Kanrenmo
{
    public class BoundVar<T> : Var
    {
        public static implicit operator BoundVar<T>(T value) => new BoundVar<T>(value);

        public BoundVar(T value)
        {
            Value = value;
        }

        public override bool Bound => true;

        public T Value { get; }

        public override bool Equals(object obj)
        {
            var other = obj as BoundVar<T>;
            return !Equals(other, null) && Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }

}
