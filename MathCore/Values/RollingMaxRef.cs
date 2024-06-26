#if NET8_0_OR_GREATER
#nullable enable
using System.Runtime.CompilerServices;
using System.Text;

namespace MathCore.Values;

public ref struct RollingMaxRef<T>(Span<T> Buffer, IComparer<T>? Comparer = null, bool Inverted = false)
    where T : IComparable<T>
{
    private readonly Span<T> _Buffer = Buffer;

    private int _Index;
    private int _Count;
    private readonly IComparer<T>? _Comparer = Comparer ?? Comparer<T>.Default;

    public int Count => _Count;
    public int MaxCount => _Buffer.Length;

    public bool Inverted { get; } = Inverted;

    public T this[int index]
    {
        get => _Buffer[GetIndex(index, _Index, _Buffer.Length)];
        set => _Buffer[GetIndex(index, _Index, _Buffer.Length)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), DST]
    private static int GetIndex(int Index, int BaseIndex, int MaxCount) => (BaseIndex - Index) % MaxCount;

    public T Add(T value)
    {
        if (_Count == 0)
        {
            _Buffer[0] = value;
            _Count = 1;
            return value;
        }

        var max = this[0];

        if (Inverted)
        {
            if (_Comparer.Compare(value, max) >= 0)
                return max;
        }
        else
        if (_Comparer.Compare(value, max) <= 0)
            return max;

        if (_Count < MaxCount)
            _Count++;

        _Index++;

        this[0] = value;

        return value;
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
        _Buffer.Clear();
    }

    public override string ToString()
    {
        var result = new StringBuilder(2 + _Count * 4).Append('[');

        var count1 = _Count - 1;
        for (var i = 0; i < _Count; i++)
            result.Append(_Buffer[i]).Append(i < count1, ',');

        return result.Append(']').ToString();
    }

    public static RollingMaxRef<T> operator +(RollingMaxRef<T> max, T value)
    {
        max.Add(value);
        return max;
    }

    public static RollingMaxRef<T> operator +(RollingMaxRef<T> max, T[] value)
    {
        max.Add((IEnumerable<T>)value);
        return max;
    }

    public static RollingMaxRef<T> operator +(RollingMaxRef<T> max, IEnumerable<T> value)
    {
        max.Add(value);
        return max;
    }

    public static RollingMaxRef<T> operator +(RollingMaxRef<T> max, ReadOnlySpan<T> value)
    {
        max.Add(value);
        return max;
    }

    public static RollingMaxRef<T> operator +(RollingMaxRef<T> max, ReadOnlyMemory<T> value)
    {
        max.Add(value);
        return max;
    }
}
#endif