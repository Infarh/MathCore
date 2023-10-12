using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MathCore.Algorithms.Collections;

public class CustomDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private struct Element
    {
        public TKey Key;

        public TValue Value;

        public int Next;

        public override string ToString() => $"[{Key}]({Next}){Value}";
    }

    private Element[] _Items;

    private readonly IEqualityComparer<TKey> _KeyComparer;

    public int Count { get; private set; }

    private KeysCollection _KeysCollection;

    public ICollection<TKey> Keys => _KeysCollection ??= new(this);

    private class KeysCollection(CustomDictionary<TKey, TValue> Dict) : ICollection<TKey>
    {
        public IEnumerator<TKey> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TKey item) => throw new NotSupportedException();

        public void Clear() => Dict.Clear();

        public bool Contains(TKey item) => throw new NotImplementedException();

        public void CopyTo(TKey[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(TKey item) => Dict.Remove(item!);

        public int Count => Dict.Count;

        public bool IsReadOnly => true;
    }

    private ValuesCollection _ValuesCollection;

    public ICollection<TValue> Values => _ValuesCollection ??= new(this);

    private class ValuesCollection(CustomDictionary<TKey, TValue> Dict) : ICollection<TValue>
    {
        public IEnumerator<TValue> GetEnumerator() => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValue item) => throw new NotSupportedException();

        public void Clear() => Dict.Clear();

        public bool Contains(TValue item) => throw new NotImplementedException();

        public void CopyTo(TValue[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(TValue item) => throw new NotSupportedException();

        public int Count => Dict.Count;

        public bool IsReadOnly => true;
    }

    public bool IsReadOnly => false;

    public TValue this[TKey key]
    {
        get
        {
            // Определяем место положения записи в словаре - индекс как
            // хеш ключа по модулю длины массива записей
            var index = Math.Abs(_KeyComparer.GetHashCode(key)) % _Items.Length;

            // Пока не надоест...
            while (true)
            {
                // Извлекаем запись из словаря по её индексу
                var item = _Items[index];

                // Если у извлечённой записи указанный следующий индекс 0,
                // то эта ячейка не была заполнена значением.
                // Значит ко указанному ключу запись в словаре отсутствует
                if (item.Next == 0)
                    throw new KeyNotFoundException();

                // Если в ячейке есть запись, то проверяем равенство ключей.
                // Если ключи равны, то мы нашли то, что искали.
                if (_KeyComparer.Equals(item.Key, key))
                    return item.Value;

                // Если ключи не совпали, то мы наткнулись на коллизию, и надо смотреть в следующей ячейке.
                // При этом, если индекс следующей ячейки указывает на текущую, то в ячейке при записи
                // коллизий не было. Следовательно, в словаре значение по искомому ключу отсутствует.
                if (item.Next - 1 == index)
                    throw new KeyNotFoundException();

                // Если индекс следующей ячейки есть, то переходим к рассмотрению его.
                index = item.Next - 1;
            }
        }
        set => throw new NotImplementedException();
    }

    public CustomDictionary() => (_Items, _KeyComparer) = (Array.Empty<Element>(), EqualityComparer<TKey>.Default);

    public CustomDictionary(int Capacity) => (_Items, _KeyComparer) = (new Element[Capacity], EqualityComparer<TKey>.Default);

    public CustomDictionary(IEqualityComparer<TKey> KeyComparer) => (_Items, _KeyComparer) = (Array.Empty<Element>(), KeyComparer);

    public CustomDictionary(int Capacity, IEqualityComparer<TKey> KeyComparer) => (_Items, _KeyComparer) = (new Element[Capacity], KeyComparer);

    public void Add(TKey key, TValue value)
    {
        if (Count == _Items.Length)
            Expanse();

        AddItem(_Items, key, value, _KeyComparer);
        Count++;
    }

    private static void AddItem(Element[] items, TKey key, TValue value, IEqualityComparer<TKey> comparer)
    {
        var index = Math.Abs(comparer.GetHashCode(key)) % items.Length;

        if (items[index].Next == 0) // попали в пустую ячейку
        {
            items[index].Key = key;
            items[index].Value = value;
            items[index].Next = index + 1;
            return;
        }

        while (items[index].Next is { } i and > 0 && i - 1 != index) 
            index = i - 1;

        var next_index = (index + 1) % items.Length;

        while (items[next_index].Next > 0) 
            next_index = (next_index + 1) % items.Length;

        items[index].Next = next_index + 1;
        items[next_index].Key = key;
        items[next_index].Value = value;
        items[next_index].Next = next_index + 1;
    }

    private void Expanse()
    {
        if (_Items.Length == 0)
        {
            _Items = new Element[4];
            return;
        }

        var new_items = new Element[_Items.Length * 2];

        foreach(var item in _Items)
            AddItem(new_items, item.Key, item.Value, _KeyComparer);

        _Items = new_items;
    }

    public bool ContainsKey(TKey key)
    {
        var index = Math.Abs(_KeyComparer.GetHashCode(key)) % _Items.Length;
        while (true)
        {
            var item = _Items[index];
            if (item.Next == 0) return false;
            if (_KeyComparer.Equals(item.Key, key)) return true;
            if (item.Next - 1 == index) return false;
            index = item.Next - 1;
        }
    }

    public bool Remove(TKey key)
    {
        var index = Math.Abs(_KeyComparer.GetHashCode(key)) % _Items.Length;
        while (true)
        {
            var item = _Items[index];
            if (item.Next == 0) return false;
            if (_KeyComparer.Equals(item.Key, key))
            {
                throw new NotImplementedException();
                return true;
            }
            if (item.Next - 1 == index) return false;
            index = item.Next - 1;
        }
    }
    
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => throw new NotImplementedException();
    
    public void Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

    public void Clear()
    {
        for(var i = 0; i < _Items.Length; i++)
            _Items[i] = default;
        Count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => TryGetValue(item.Key, out var value) && Equals(item.Value, value);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int Index) => throw new NotImplementedException();

    public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) && Remove(item.Key);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new KeyValueEnumerator(this);

    private struct KeyValueEnumerator(CustomDictionary<TKey, TValue> Dict) : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        public bool MoveNext() => throw new NotImplementedException();

        public void Reset() => throw new NotImplementedException();

        public KeyValuePair<TKey, TValue> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
