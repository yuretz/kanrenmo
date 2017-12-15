using System;

namespace Kanrenmo
{
    public class Var
    {

        public static implicit operator Var(int value) => new BoundVar<int>(value);
        public static implicit operator Var(bool value) => new BoundVar<bool>(value);
        public static implicit operator Var(double value) => new BoundVar<double>(value);
        public static implicit operator Var(string value) => new BoundVar<string>(value);
        public static Relation operator ==(Var left, Var right) => new Relation(context => context.Unify(left, right));
        public static Relation operator !=(Var left, Var right) => new Relation(context => throw new NotImplementedException());
        public readonly int Id = ++_id;
        public virtual bool Bound => false;
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
        public override int GetHashCode() => Id;
        private static int _id;
    }
}
