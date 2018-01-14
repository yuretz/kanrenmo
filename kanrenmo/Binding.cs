using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kanrenmo.Annotations;

namespace Kanrenmo
{
    public class Binding: IReadOnlyDictionary<Var, Var>, IReadOnlyList<Var>
    {
        internal Binding(IEnumerable<KeyValuePair<Var, Var>> pairs)
        {
            _pairs = pairs.ToList();
            _bindings = _pairs.ToDictionary(pair => pair.Key, pair => pair.Value);
        }


        Var IReadOnlyDictionary<Var, Var>.this[Var key] => _bindings[key];


        Var IReadOnlyList<Var>.this[int index] => _pairs[index].Value;


        public int Count => _pairs.Count;

        [NotNull]
        public IEnumerable<Var> Keys => _pairs.Select(pair => pair.Key);

        [NotNull]
        public IEnumerable<Var> Values => _pairs.Select(pair => pair.Value);
        public bool ContainsKey(Var key) => _bindings.ContainsKey(key);


        public bool TryGetValue(Var key, out Var value) => _bindings.TryGetValue(key, out value);

        [NotNull]
        IEnumerator<KeyValuePair<Var, Var>> IEnumerable<KeyValuePair<Var, Var>>.GetEnumerator() =>
            _pairs.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();

        IEnumerator<Var> IEnumerable<Var>.GetEnumerator() => Values.GetEnumerator();
        

        private readonly Dictionary<Var, Var> _bindings;

        private readonly List<KeyValuePair<Var, Var>> _pairs;
    }
}
