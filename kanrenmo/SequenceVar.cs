using System.Collections;
using System.Collections.Generic;

namespace Kanrenmo
{
    /// <summary>
    /// Sequence variable
    /// </summary>
    /// <seealso cref="Var" />
    public class SequenceVar : Var, IEnumerable<Var>
    {
        /// <summary>
        /// The empty sequence
        /// </summary>
        public static readonly SequenceVar Empty = new SequenceVar(null, null);

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceVar" /> class.
        /// </summary>
        /// <param name="head">Sequnce head.</param>
        /// <param name="tail">Sequence tail.</param>
        internal SequenceVar(Var head, Var tail)
        {
            Head = head;
            Tail = tail ?? Empty;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var" /> is bound to a value.
        /// </summary>
        public override bool Bound => true;

        /// <summary>
        /// Gets a value indicating whether this sequence is empty.
        /// </summary>
        public bool IsEmpty => Equals(Head, null);

        /// <summary>
        /// The sequence head
        /// </summary>
        public readonly Var Head;

        /// <summary>
        /// The sequence tail
        /// </summary>
        public readonly Var Tail;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Var> GetEnumerator() => new SequenceEnumerator(this);


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => new SequenceEnumerator(this);

        private class SequenceEnumerator : IEnumerator<Var>
        {
            public SequenceEnumerator(SequenceVar start) => _start = start;

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
                    case SequenceVar sequence when Equals(sequence.Head, null):
                        return false;
                    case SequenceVar sequence:
                        Current = sequence.Head;
                        _variable = sequence.Tail;
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

            private readonly SequenceVar _start;
            private bool _started;
            private Var _variable;
        }

    }
}
