using System.Collections;
using System.Collections.Generic;

using MathCore.Annotations;

namespace MathCore
{
    public class DictionaryKeySafe<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [NotNull]
        private readonly IDictionary<TKey, TValue> _Dictionary;

        public int Count => _Dictionary.Count;

        public ICollection<TKey> Keys => _Dictionary.Keys;

        public ICollection<TValue> Values => _Dictionary.Values;

        public TValue this[TKey key]
        {
            get => !_Dictionary.TryGetValue(key, out var obj) ? default : obj;
            set => _Dictionary[key] = value;
        }

        public bool IsReadOnly => _Dictionary.IsReadOnly;

        public DictionaryKeySafe([NotNull] IDictionary<TKey, TValue> dictionary) => _Dictionary = dictionary;

        public void Add(TKey key, TValue value) => _Dictionary.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item) => _Dictionary.Add(item);

        public bool Remove(KeyValuePair<TKey, TValue> item) => _Dictionary.Remove(item);

        public bool Remove(TKey key) => _Dictionary.Remove(key);

        public bool Contains(KeyValuePair<TKey, TValue> item) => _Dictionary.Contains(item);

        public bool ContainsKey(TKey key) => _Dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _Dictionary.TryGetValue(key, out value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) => _Dictionary.CopyTo(array, index);

        public void Clear() => _Dictionary.Clear();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Dictionary.GetEnumerator();

        public static implicit operator DictionaryKeySafe<TKey, TValue>(Dictionary<TKey, TValue> d) => new DictionaryKeySafe<TKey, TValue>(d);

        public static implicit operator Dictionary<TKey, TValue>(DictionaryKeySafe<TKey, TValue> d) => new Dictionary<TKey, TValue>(d._Dictionary);
    }
}
