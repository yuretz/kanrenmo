using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    /// <summary>
    /// Sequence variable
    /// </summary>
    /// <seealso cref="Var" />
    public class PairVar : Var, IEnumerable<Var>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PairVar" /> class.
        /// </summary>
        /// <param name="head">Sequnce head.</param>
        /// <param name="tail">Sequence tail.</param>
        public PairVar([CanBeNull] Var head = null, [CanBeNull] Var tail = null)
        {
            _head = head ?? Empty;
            _tail = tail ?? Empty;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var" /> is bound to a value.
        /// </summary>
        public override bool Bound => true;

        /// <summary>
        /// Gets the head element of the sequence.
        /// </summary>
        /// <returns>The head element</returns>
        [NotNull]
        public override Var Head() => _head;

        /// <summary>
        /// Gets the tail subsequence of the sequence.
        /// </summary>
        /// <returns>The tail element</returns>
        [NotNull]
        public override Var Tail() => _tail;

        /// <summary>
        /// Check whether this variable includes another one
        /// </summary>
        /// <param name="variable">The variable to check.</param>
        /// <returns>
        /// true if it does, false otherwise
        /// </returns>
        public override bool Includes(Var variable) => _head.Includes(variable) || _tail.Includes(variable);

        /// <summary>
        /// Gets a value indicating whether this instance is a pair.
        /// </summary>
        public override bool IsPair => true;

        /// <summary>
        /// Gets a value indicating whether this instance is list.
        /// </summary>
        public override bool IsSequence => _tail.IsEmpty || _tail.IsSequence;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        public IEnumerator<Var> GetEnumerator() =>
            //AllLeafVariables().GetEnumerator();
            new SequenceEnumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => 
            //AllLeafVariables().GetEnumerator();
            new SequenceEnumerator(this);


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => 
            obj is PairVar other && Equals(_head, other._head) && Equals(_tail, other._tail);//this.SequenceEqual(other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() =>
            (((5381 * 33) ^ _head.GetHashCode()) * 33) ^ _tail.GetHashCode();

        /// <summary>
        /// Converts this variable instance to s-expression.
        /// </summary>
        /// <param name="unbound">The unbound variables list.</param>
        /// <returns>
        /// S-expression string
        /// </returns>
        internal override string ToSExpression(SortedList<int, Var> unbound) =>
            IsSequence
                ? "(" + string.Join(" ", this.Select(v => v.ToSExpression(unbound))) + ")"
                : "(" + _head.ToSExpression(unbound) + " . " + _tail.ToSExpression(unbound) + ")"; 
        

        private struct SequenceEnumerator : IEnumerator<Var>
        {
            public SequenceEnumerator(PairVar start)
            {
                _start = start;
                _started = false;
                Current = null;
                _variable = null;
            }
            

            public bool MoveNext()
            {
                if (!_started)
                {
                    _started = true;
                    _variable = _start;
                }

                
                switch (_variable)
                {
                    case PairVar sequence when (sequence._tail is PairVar || sequence._tail.IsEmpty):
                        Current = sequence._head;
                        _variable = sequence._tail;
                        return true;
                    case Var variable when variable.IsEmpty:
                        return false;
                    default:
                        Current = _variable;
                        _variable = Empty;
                        return true;
                }

            }

            public void Reset()
            {
                _started = false;
                Current = null;
            }

            public Var Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            
            }

            private readonly PairVar _start;
            private bool _started;
            private Var _variable;
        }

        /// <summary>
        /// The sequence head
        /// </summary>
        private readonly Var _head;

        /// <summary>
        /// The sequence tail
        /// </summary>
        private readonly Var _tail;
    }
}
