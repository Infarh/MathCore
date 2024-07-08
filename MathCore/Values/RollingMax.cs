#nullable enable
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace MathCore.Values;

public static class RollingMax
{
    public static RollingMax<T> New<T>(int Count, IEnumerable<T> items) where T : IComparable<T> => new RollingMax<T>(Count) + items;

    public static RollingMaxBuilder<T> Build<T>(int Count, IEnumerable<T> items) => new(Count, items);

    public readonly ref struct RollingMaxBuilder<T>(int Count, IEnumerable<T> items)
    {
        public RollingMax<T> New(Comparison<T> comparer) => new RollingMax<T>(Count, comparer) + items;

    }
}

public class RollingMax<T>(T[] Buffer, IComparer<T>? Comparer = null, bool Inverted = false) : IEnumerable<T>
{
    public RollingMax(int MaxCount, IComparer<T>? Comparer = null, bool Inverted = false)
        :this(new T[MaxCount > 0 ? MaxCount : throw new ArgumentOutOfRangeException(nameof(MaxCount), MaxCount, $"{nameof(MaxCount)} должно быть больше 0")], Comparer, Inverted)
    { }

    public RollingMax(int MaxCount, Comparison<T> comparison, bool Inverted = false)
        : this(MaxCount, Comparer<T>.Create(comparison), Inverted) { }

    private int _Index;
    private int _Count;
    private readonly IComparer<T>? _Comparer = Comparer ?? Comparer<T>.Default;

    private readonly T[] _Buffer = Buffer switch
    {
        null => throw new ArgumentNullException(nameof(Buffer)),
        { Length: 0 } => throw new ArgumentOutOfRangeException(nameof(Buffer), 0, $"Длина буфера {nameof(Buffer)} должна быть больше 0"),
        _ => Buffer
    };

    public int Count => _Count;

    public int MaxCount => _Buffer.Length;

    public bool Inverted { get; } = Inverted;

    public Interval<T> Interval => new(this[-1], this[0]);

    public T this[int index]
    {
        get => _Buffer[GetIndex(index < 0 ? _Count + index : index, _Index, _Buffer.Length)];
        set => _Buffer[GetIndex(index < 0 ? _Count + index : index, _Index, _Buffer.Length)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)/*, DST*/]
    private static int GetIndex(int Index, int BaseIndex, int MaxCount) => (MaxCount + (BaseIndex - Index) % MaxCount) % MaxCount;

    public T Add(T value)
    {
        if (_Count == 0)
        {
            _Buffer[0] = value;
            _Count = 1;
            return value;
        }

        var head = this[0];

        var comparer = _Comparer;
        if (Inverted)
        {
            if (comparer.Compare(value, head) > 0)
            {
                if (_Count == 1) return head;

                switch (comparer.Compare(value, this[-1]))
                {
                    case 0:
                        this[-1] = value;
                        break;
                    case <0:

                        break;
        }

                return head;
            }
        }
        else
        {
            // [6,1]
            // [6,2,1]
            // [6,3,2,1]
            // [6,4,3,2,1]
            // [6,5,4,3,2]
            // [6,6,5,4,3]

            // Если value < head - меньше ведущего элемента,
            // то, возможно, элемент надо поместить в хвост
            if (comparer.Compare(value, head) < 0)
            {
                if (_Count == 1)
                {
                    if(Grow())
                        this[-1] = value;
                }
                else switch (comparer.Compare(value, this[-1])) // Сравниваем элемент с последним элементом в хвосте
                {
                    #region Если добавляемый элемент равен последнему элементу в хвосте
                    // ... и в хвосте нет больше места
                    // .., то записываем элемент последним в хвосте
                    case 0:
                        Grow();
                        this[-1] = value; 
                        break; 
                    #endregion

                    // Если элемент больше чем последний элемент хвоста
                    case >0:
                        if (_Count == 2 & Grow()) // именно "&"! Местами операнды не менять!
                        {
                            (this[-1], this[-2]) = (this[-2], value);
                            break;
                        }

                        var set = false;
                        for (var i = 2; i < _Count; i++)
                        {
                            this[-(i - 1)] = this[-i];
                            if (comparer.Compare(value, this[-i]) >= 0) continue;

                            set = true;
                            this[-i] = value;
                            break;

                        }

                        if(!set)
                            this[1] = value;

                        break;
                }

                return head;
            }
        }

        // Тут мы оказываемся если у нас новый максимум,
        // и его надо добавить в голову.
        Grow();

        _Index++;

        this[0] = value;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool Grow()
    {
        if (_Count >= MaxCount) return false;
        _Count++;
        return true;
    }

    public void Add(params IEnumerable<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

#if NET8_0_OR_GREATER
    public void Add(ReadOnlySpan<T> span)
    {
        foreach (var item in span)
            Add(item);
    }

    public void Add(ReadOnlyMemory<T> memory) => Add(memory.Span);
#endif

    public void Clear()
    {
        _Count = 0;
        _Index = 0;
        Array.Clear(_Buffer, 0, _Buffer.Length);
    }

    public override string ToString() => _Count == 0 ? "[]" : this
        .Aggregate(
            new StringBuilder(2 + _Count * 4).Append('['),
            (S, v) => S.Append(v).Append(','),
            S => S.SetLength(-1).Append(']'))
        .ToString();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _Count; i++)
            yield return this[i];
    }

    public static RollingMax<T> operator +(RollingMax<T> max, T value)
    {
        max.Add(value);
        return max;
    }

    public static RollingMax<T> operator +(RollingMax<T> max, T[] value)
    {
#if NET8_0_OR_GREATER
        max.Add((IEnumerable<T>)value);
#else
        max.Add(value);
#endif
        return max;
    }

    public static RollingMax<T> operator +(RollingMax<T> max, IEnumerable<T> value)
    {
        max.Add(value);
        return max;
    }

#if NET8_0_OR_GREATER
    public static RollingMax<T> operator +(RollingMax<T> max, ReadOnlySpan<T> value)
    {
        max.Add(value);
        return max;
    }

    public static RollingMax<T> operator +(RollingMax<T> max, ReadOnlyMemory<T> value)
    {
        max.Add(value);
        return max;
    }
#endif
}
