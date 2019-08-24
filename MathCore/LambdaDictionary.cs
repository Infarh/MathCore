using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    public class LambdaDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [NotNull]
        private readonly Func<IEnumerable<KeyValuePair<TKey, TValue>>> _ElementsGetter;
        [CanBeNull]
        private readonly Action<TKey, TValue> _ElementSetter;
        [CanBeNull]
        private readonly Action _Clear;
        [CanBeNull]
        private readonly Func<TKey, bool> _Remove;

        /// <inheritdoc />
        public bool IsReadOnly => _ElementSetter == null;

        /// <inheritdoc />
        public int Count => _ElementsGetter().Count();

        /// <inheritdoc />
        public ICollection<TKey> Keys => new Collection<TKey>(_ElementsGetter().Select(v => v.Key).ToList());

        /// <inheritdoc />
        public ICollection<TValue> Values => new Collection<TValue>(_ElementsGetter().Select(v => v.Value).ToList());

        /// <inheritdoc />
        public TValue this[TKey key]
        {
            get => _ElementsGetter().Where(v => Equals(v.Key, key)).Select(v => v.Value).FirstOrDefault();
            set => Add(key, value);
        }

        public LambdaDictionary
        (
            [NotNull]
            Func<IEnumerable<KeyValuePair<TKey, TValue>>> ElementsGetter,
            [CanBeNull]
            Action<TKey, TValue> ElementSetter = null,
            [CanBeNull]
            Action Clear = null,
            [CanBeNull]
            Func<TKey, bool> Remove = null
        )
        {
            _ElementsGetter = ElementsGetter ?? throw new ArgumentNullException(nameof(ElementsGetter), "Не задан метод получения значения");
            _ElementSetter = ElementSetter;
            _Clear = Clear;
            _Remove = Remove;
        } 

        // ReSharper disable once UnusedParameter.Local
        private void CheckSupported(Delegate action, string message)
        {
            if(action == null)
                throw new NotSupportedException(message);
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        /// <inheritdoc />
        public void Add(TKey key, TValue value)
        {
            CheckSupported(_ElementsGetter, "Словарь не поддерживает операции записи");
            _ElementSetter(key, value);
        }

        /// <inheritdoc />
        public void Clear()
        {
            CheckSupported(_Clear, "Словарь не поддерживает операцию очистки");
            _Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TValue> item) => _ElementsGetter().Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var item in _ElementsGetter())
                array[arrayIndex++] = item;
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        /// <inheritdoc />
        public bool Remove(TKey key)
        {
            CheckSupported(_Remove, "Словарь не поддерживает операцию удаления");
            return _Remove(key);
        }

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => _ElementsGetter().Contains(v => Equals(v.Key, key));

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var v in _ElementsGetter().Where(v => Equals(v.Key, key)))
            {
                value = v.Value;
                return true;
            }
            value = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _ElementsGetter().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}