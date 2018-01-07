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
        /// The empty sequence
        /// </summary>
        public static readonly PairVar Empty = new PairVar(null, null);

        /// <summary>
        /// Initializes a new instance of the <see cref="PairVar" /> class.
        /// </summary>
        /// <param name="head">Sequnce head.</param>
        /// <param name="tail">Sequence tail.</param>
        internal PairVar([CanBeNull] Var head, [CanBeNull] Var tail)
        {
            _head = head;
            _tail = Equals(head, null) ? null : (tail ?? Empty);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var" /> is bound to a value.
        /// </summary>
        public override bool Bound => true;

        /// <summary>
        /// Gets a value indicating whether this sequence is empty.
        /// </summary>
        public bool IsEmpty => Equals(_head, null);

        /// <summary>
        /// Gets the head element of the sequence.
        /// </summary>
        /// <returns>The head element</returns>
        public override Var Head() => _head;

        /// <summary>
        /// Gets the tail subsequence of the sequence.
        /// </summary>
        /// <returns>The tail element</returns>
        public override Var Tail() => _tail;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        public IEnumerator<Var> GetEnumerator() => new SequenceEnumerator(this);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => new SequenceEnumerator(this);


        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj is PairVar other) && this.SequenceEqual(other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() =>
            this.Aggregate(5381, (hash, variable) => hash * 33 ^ (variable?.GetHashCode() ?? 0));
        

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
                    case null:
                    case PairVar sequence when Equals(sequence._head, null):
                        return false;
                    case PairVar sequence:
                        Current = sequence._head;
                        _variable = sequence._tail;
                        return true;
                    default:
                        Current = _variable;
                        _variable = null;
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
