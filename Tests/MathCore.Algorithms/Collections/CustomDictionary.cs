using System.Security.Cryptography;

namespace MathCore.Algorithms.Collections;

public class CustomDictionary<TKey, TValue>
{
    private struct Element()
    {
        public TKey Key;

        public TValue Value;

        public int? Next;

        public override string ToString() => $"[{Key}]({Next}){Value}";
    }

    private Element[] _Items;

    private readonly IEqualityComparer<TKey> _KeyComparer;

    public int Count { get; private set; }

    public TValue this[TKey key]
    {
        get
        {
            var hash = Math.Abs(_KeyComparer.GetHashCode(key));

            int? index = hash % _Items.Length;

            do
            {
                var item = _Items[(int)index!];

                if (item.Next == null)
                    throw new KeyNotFoundException();

                if (Equals(item.Key, key))
                    return item.Value;

                if (item.Next == index)
                    throw new KeyNotFoundException();

                index = item.Next;
            } while (true);
        }
    }

    public CustomDictionary()
    {
        _Items = Array.Empty<Element>();
        _KeyComparer = EqualityComparer<TKey>.Default;
    }

    public CustomDictionary(int Capacity)
    {
        _Items = new Element[Capacity];
        _KeyComparer = EqualityComparer<TKey>.Default;
    }

    public CustomDictionary(IEqualityComparer<TKey> KeyComparer)
    {
        _Items = Array.Empty<Element>();
        _KeyComparer = KeyComparer;
    }

    public CustomDictionary(int Capacity, IEqualityComparer<TKey> KeyComparer)
    {
        _Items = new Element[Capacity];
        _KeyComparer = KeyComparer;
    }

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

        if (items[index].Next is null) // попали в пустую ячейку
        {
            items[index].Key = key;
            items[index].Value = value;
            items[index].Next = -1;
            return;
        }

        while (items[index].Next is { } i and >= 0) 
            index = i;

        var next_index = (index + 1) % items.Length;

        while (items[next_index].Next is not null) 
            next_index = (next_index + 1) % items.Length;

        items[index].Next = next_index;
        items[next_index].Key = key;
        items[next_index].Value = value;
        items[next_index].Next = -1;
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
}
