using System;
using System.Collections.Generic;

namespace MathCore.Values
{
    public class Set<TElement>
    {
        private struct Slot
        {
            internal int HashCode;
            internal TElement Value;
            internal int Next;
        }

        private int[] _Buckets;

        private Slot[] _Slots;

        private int _Count;

        private int _FreeList;

        private readonly IEqualityComparer<TElement> _Comparer;

        public Set() : this(null) { }

        public Set(IEqualityComparer<TElement> comparer)
        {
            if(comparer is null) comparer = EqualityComparer<TElement>.Default;
            _Comparer = comparer;
            _Buckets = new int[7];
            _Slots = new Slot[7];
            _FreeList = -1;
        }

        public bool Add(TElement value) => !Find(value, true);

        public bool Contains(TElement value) => Find(value, false);

        private bool Find(TElement value, bool add)
        {
            var hash = GetHashCodeOf(value);
            for(var i = _Buckets[hash % _Buckets.Length] - 1; i >= 0; i = _Slots[i].Next)
            {
                if(_Slots[i].HashCode == hash && _Comparer.Equals(_Slots[i].Value, value))
                    return true;
            }
            if(!add) return false;
            int k;
            if(_FreeList >= 0)
            {
                k = _FreeList;
                _FreeList = _Slots[k].Next;
            }
            else
            {
                if(_Count == _Slots.Length) Resize();
                k = _Count;
                _Count++;
            }
            var j = hash % _Buckets.Length;
            _Slots[k].HashCode = hash;
            _Slots[k].Value = value;
            _Slots[k].Next = _Buckets[j] - 1;
            _Buckets[j] = k + 1;
            return false;
        }

        public bool Remove(TElement value)
        {
            var hash = GetHashCodeOf(value);
            var j = hash % _Buckets.Length;
            var k = -1;
            for(var i = _Buckets[j] - 1; i >= 0; i = _Slots[i].Next)
            {
                if(_Slots[i].HashCode == hash && _Comparer.Equals(_Slots[i].Value, value))
                {
                    if(k < 0)
                        _Buckets[j] = _Slots[i].Next + 1;
                    else
                        _Slots[k].Next = _Slots[i].Next;
                    _Slots[i].HashCode = -1;
                    _Slots[i].Value = default;
                    _Slots[i].Next = _FreeList;
                    _FreeList = i;
                    return true;
                }
                k = i;
            }
            return false;
        }

        private void Resize()
        {
            var length = checked(_Count * 2 + 1);
            var nums = new int[length];
            var slots = new Slot[length];
            Array.Copy(_Slots, 0, slots, 0, _Count);
            for(var i = 0; i < _Count; i++)
            {
                var j = slots[i].HashCode % length;
                slots[i].Next = nums[j] - 1;
                nums[j] = i + 1;
            }
            _Buckets = nums;
            _Slots = slots;
        }

        private int GetHashCodeOf(TElement value) => value != null ? _Comparer.GetHashCode(value) & int.MaxValue : 0;
    }
}