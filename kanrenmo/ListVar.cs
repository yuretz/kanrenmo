using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Kanrenmo
{
    /// <summary>
    /// List variable
    /// </summary>
    /// <seealso cref="Var" />
    /// <seealso cref="Var" />
    public class ListVar : Var, IReadOnlyList<Var>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListVar"/> class.
        /// </summary>
        /// <param name="variables">The enumeration of variables to put in the list.</param>
        public ListVar(IEnumerable<Var> variables) => _variables = variables.ToImmutableList();

        /// <summary>
        /// Gets a value indicating whether this <see cref="Var" /> is bound to a value.
        /// </summary>
        public override bool Bound => true;


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Var> GetEnumerator() => _variables.GetEnumerator();


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _variables.GetEnumerator();

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _variables.Count;

        /// <summary>
        /// Gets the <see cref="Var"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Var"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>The variable at the specified index</returns>
        public Var this[int index] => _variables[index];

        private readonly IReadOnlyList<Var> _variables;
    }
}
