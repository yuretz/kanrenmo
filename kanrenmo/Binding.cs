using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kanrenmo
{
    public class Binding: IReadOnlyDictionary<Var, Var>, IReadOnlyList<Var>
    {
        internal Binding(IEnumerable<KeyValuePair<Var, Var>> pairs)
        {
            var list = pairs.ToList();
            _bindings = list.ToDictionary(pair => pair.Key, pair => pair.Value);
            _variables = list.Select(pair => pair.Key).ToList();
        }


        Var IReadOnlyDictionary<Var, Var>.this[Var key] => _bindings[key];


        Var IReadOnlyList<Var>.this[int index] => _bindings[_variables[index]];


        public int Count => _variables.Count;
        public IEnumerable<Var> Keys => _variables.AsReadOnly();
        public IEnumerable<Var> Values => _variables.Select(variable => _bindings[variable]);
        public bool ContainsKey(Var key) => _bindings.ContainsKey(key);


        public bool TryGetValue(Var key, out Var value) => _bindings.TryGetValue(key, out value);


        IEnumerator<KeyValuePair<Var, Var>> IEnumerable<KeyValuePair<Var, Var>>.GetEnumerator() =>
            _variables
                .Select(variable => new KeyValuePair<Var, Var>(variable, _bindings[variable]))
                .GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => _variables.GetEnumerator();

        IEnumerator<Var> IEnumerable<Var>.GetEnumerator() => _variables.GetEnumerator();
        

        private readonly Dictionary<Var, Var> _bindings;
        private readonly List<Var> _variables;
    }
}
