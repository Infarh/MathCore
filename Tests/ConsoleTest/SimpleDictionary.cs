using System;
using System.CodeDom;
using System.Collections.Generic;
using MathCore;

namespace ConsoleTest
{
    class SimpleDictionary<TKey, TValue>
    {
        private Item[] _Items;
        private struct Item
        {
            public int Hash;
            public int NextIndex;
            public TKey Key;
            public TValue Value;
        }

        private int[] _Indexes;
        private int _Version;

        private int _Count;
        private int _FreeCount = -1;
        private int _FreeItemIndex;

        private readonly IEqualityComparer<TKey> _Comparer = EqualityComparer<TKey>.Default;

        public void Add(TKey Key, TValue Value) => Insert(Key, Value);

        private void Initialize(int Capacity)
        {
            _FreeItemIndex = -1;

            var count = PrimeNumbers.GetClosestUp(Capacity);
            _Items = new Item[count];
            _Indexes = new int[count];
            for (var i = 0; i < count; i++)
                _Indexes[i] = -1;
        }

        private void Insert(TKey Key, TValue Value, bool Add = true)
        {
            if (Key is null) throw new ArgumentNullException(nameof(Key));

            if (_Indexes is null) Initialize(0);
            var hash = _Comparer.GetHashCode(Key) & int.MaxValue;

            var indexes_count = _Indexes.Length;
            var item_index = hash % indexes_count;
            var iteration = 0;
            for (var i = _Indexes[item_index]; i >= 0; i = _Items[i].NextIndex, iteration++)
                if (_Items[i].Hash == hash && _Comparer.Equals(_Items[i].Key, Key))
                {
                    if (Add) throw new InvalidOperationException("Рассаглосованное состояние словаря. Дублирование записей по ключу.");

                    _Items[i].Value = Value;
                    _Version++;
                    return;
                }

            int last_item_index;
            if (_FreeCount > 0)
            {
                last_item_index = _FreeItemIndex;
                _FreeItemIndex = _Items[_FreeCount].NextIndex;
                _FreeCount--;
            }
            else
            {
                if (_Count == _Items.Length)
                {
                    Resize();
                    item_index = hash % indexes_count;
                }

                last_item_index = _Count;
                _Count++;
            }

            _Items[last_item_index].Hash = hash;
            _Items[last_item_index].Key = Key;
            _Items[last_item_index].Value = Value;
            _Items[last_item_index].NextIndex = _Indexes[item_index];

            _Indexes[item_index] = last_item_index;

            _Version++;

            if (iteration <= 100 || IsWellKnownEqualityComparer(_Comparer))
                return;

            Resize(_Items.Length, true);
        }

        private void Resize() => Resize(PrimeNumbers.ExpandPrime(_Count), false);

        private void Resize(int NewSize, bool ForceNewHashCodes)
        {
            var new_indexes = new int[NewSize];
            for (var i = 0; i < new_indexes.Length; ++i)
                new_indexes[i] = -1;

            var new_items = new Item[NewSize];
            Array.Copy(_Items, 0, new_items, 0, _Count);

            if (ForceNewHashCodes)
                for(var i = 0; i < _Count; i++)
                    if (new_items[i].Hash != -1)
                        new_items[i].Hash = _Comparer.GetHashCode(new_items[i].Key) & int.MaxValue;

            for(var i = 0; i < _Count; i++)
                if (new_items[i].Hash >= 0)
                {
                    var j = new_items[i].Hash % NewSize;
                    new_items[i].NextIndex = new_indexes[j];
                    new_indexes[j] = i;
                }

            _Indexes = new_indexes;
            _Items = new_items;
        }

        static bool IsWellKnownEqualityComparer(object comparer) => comparer == null || comparer == EqualityComparer<string>.Default;
    }
}