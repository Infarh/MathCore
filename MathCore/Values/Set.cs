#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

// ReSharper disable UnusedType.Global

namespace MathCore.Values;

/// <summary>Множество элементов</summary>
/// <typeparam name="T">Тип данных множества</typeparam>
public class Set<T> : AbstractSetOf<T>
{
    private readonly struct Slot
    {
        public readonly int HashCode;
        public readonly T Value;
        public readonly int Next;

        public Slot(int HashCode, T Value, int Next)
        {
            this.HashCode = HashCode;
            this.Value    = Value;
            this.Next     = Next;
        }
    }

    private int[] _Buckets;

    private Slot[] _Slots;

    private int _Count;

    private int _FreeList;

    private readonly IEqualityComparer<T> _Comparer;

    /// <inheritdoc />
    public override int Power => _Count;

    /// <summary>Инициализация нового экземпляра <see cref="Set{TElement}"/></summary>
    public Set() : this(null) { }

    /// <summary>Инициализация нового экземпляра <see cref="Set{TElement}"/></summary>
    /// <param name="comparer">Объект, осуществляющий сравнение элементов множества</param>
    public Set(IEqualityComparer<T>? comparer)
    {
        _Comparer = comparer ?? EqualityComparer<T>.Default;
        _Buckets  = new int[7];
        _Slots    = new Slot[7];
        _FreeList = -1;
    }

    /// <summary>Добавить элемент в множество</summary>
    /// <param name="Value">Добавляемый элемент</param>
    /// <returns>Истина, если элемент был успешно добавлен и ложь, если элемент уже присутствует в множестве</returns>
    public override bool Add(T Value) => !Find(Value, true);

    /// <summary>Проверка на вхождение элемента в множество</summary>
    /// <param name="Value">Проверяемый элемент</param>
    /// <returns>Истина, если элемент входит в множество</returns>
    public override bool Contains(T Value) => Find(Value, false);

    private bool Find(T Value, bool AddValue)
    {
        var hash = GetHashCodeOf(Value);
        for (var i = _Buckets[hash % _Buckets.Length] - 1; i >= 0; i = _Slots[i].Next)
            if (_Slots[i].HashCode == hash && _Comparer.Equals(_Slots[i].Value, Value))
                return true;
        if (!AddValue) return false;
        Add(Value, hash);
        return false;
    }

    /// <summary>Добавить элемент в множество</summary>
    /// <param name="Value">Добавляемый элемент</param>
    /// <param name="Hash">Хеш-код элемента</param>
    private void Add(T Value, int Hash)
    {
        int k;
        if (_FreeList >= 0)
        {
            k         = _FreeList;
            _FreeList = _Slots[k].Next;
        }
        else
        {
            if (_Count == _Slots.Length) Resize();
            k = _Count;
            _Count++;
        }

        var j = Hash % _Buckets.Length;
        _Slots[k]   = new Slot(Hash, Value, _Buckets[j] - 1);
        _Buckets[j] = k + 1;
    }

    /// <summary>Удаление элемента из множества</summary>
    /// <param name="Value">Удаляемый элемент</param>
    /// <returns>Истина, если элемент присутствовал в множестве и был оттуда удалён</returns>
    public bool Remove(T Value)
    {
        var hash = GetHashCodeOf(Value);
        var j    = hash % _Buckets.Length;
        var k    = -1;
        for (var i = _Buckets[j] - 1; i >= 0; i = _Slots[i].Next)
        {
            if (_Slots[i].HashCode == hash && _Comparer.Equals(_Slots[i].Value, Value))
            {
                if (k < 0)
                    _Buckets[j] = _Slots[i].Next + 1;
                else
                    _Slots[k] = new Slot(_Slots[k].HashCode, _Slots[k].Value, _Slots[i].Next);
                _Slots[i] = new Slot(-1, default, _FreeList);
                _FreeList = i;
                return true;
            }
            k = i;
        }
        return false;
    }

    /// <summary>Подгон размера хранилища</summary>
    private void Resize()
    {
        var length = checked(_Count * 2 + 1);
        var nn     = new int[length];
        var slots  = new Slot[length];
        Array.Copy(_Slots, 0, slots, 0, _Count);
        for (var i = 0; i < _Count; i++)
        {
            var j = slots[i].HashCode % length;
            slots[i] = new Slot(slots[i].HashCode, slots[i].Value, nn[j] - 1);
            nn[j]    = i + 1;
        }
        _Buckets = nn;
        _Slots   = slots;
    }

    /// <summary>Получить хещ-код элемента</summary>
    /// <param name="Value">Элемент, хеш-код которого требуется получить</param>
    /// <returns>Рассчитанный хеш-код элемента</returns>
    private int GetHashCodeOf(T? Value) => Value != null ? _Comparer.GetHashCode(Value) & int.MaxValue : 0;

    /// <inheritdoc />
    public override IEnumerator<T> GetEnumerator() => _Slots.Where(slot => slot.Value != null && slot.HashCode != 0).Select(slot => slot.Value).GetEnumerator();
}