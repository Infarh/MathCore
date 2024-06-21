using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace MathCore.Values;

public class RollingMax<T>(int MaxCount, IComparer<T>? Comparer = null, bool Inverted = false)
    : IEnumerable<T>
    where T : IComparable<T>
{
    public RollingMax(int MaxCount, Comparison<T> comparison, bool Inverted = false)
        : this(MaxCount, LambdaComparer.New(comparison), Inverted) { }

    private int _Index;
    private int _Count;
    private readonly IComparer<T>? _Comparer = Comparer ?? Comparer<T>.Default;

    private readonly T[] _Values = new T[MaxCount > 0 ? MaxCount : throw new ArgumentOutOfRangeException(nameof(MaxCount), MaxCount, $"{nameof(MaxCount)} должно быть больше 0")];

    public int Count => _Count;
    public int MaxCount => _Values.Length;

    public bool Inverted { get; } = Inverted;

    public T this[int index]
    {
        get => _Values[GetIndex(index, _Index, _Values.Length)];
        set => _Values[GetIndex(index, _Index, _Values.Length)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), DST]
    private static int GetIndex(int Index, int BaseIndex, int MaxCount) => (BaseIndex - Index) % MaxCount;

    public T Add(T value)
    {
        if (_Count == 0)
        {
            _Values[0] = value;
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
}
