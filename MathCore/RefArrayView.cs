#nullable enable
using System.Collections;

namespace MathCore;

public class RefArrayView<T> : IReadOnlyList<T>, ICollection
{
    public class IndexInfo
    {
        private readonly int[] _Indexes;

        public int this[int index]
        {
            get => _Indexes[index];
            set
            {
                if (index < 0 || index >= _Indexes.Length)
                    throw new ArgumentOutOfRangeException(nameof(index), index, "Индекс выходит за рамки массива");
                if (value < 0 || value >= _Indexes.Length)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Устанавливаемый индекс выходит за рамки массива");
                _Indexes[index] = value;
            }
        }

        public IndexInfo(int[] Indexes) => _Indexes = Indexes;
    }

    private readonly T[] _Array;

    private readonly int[] _Indexes;

    private readonly IndexInfo _IndexInfo;

    public IndexInfo Index => _IndexInfo;

    public int Length => _Array.Length;

    public T[] Source => _Array;

    /// <inheritdoc />
    int IReadOnlyCollection<T>.Count => _Array.Length;

    public ref T this[int index] => ref _Array[_Indexes[index]];

    T IReadOnlyList<T>.this[int index] => this[index];

    public RefArrayView(int Length) : this(new T[Length]) { }

    public RefArrayView(T[] array)
    {
        _Array = array.NotNull();
        var length = _Array.Length;
        _Indexes   = new int[length];
        _IndexInfo = new IndexInfo(_Indexes);

        for (var i = 0; i < length; i++)
            _Indexes[i] = i;
    }

    public RefArrayView(T[] array, bool Inverted)
    {
        _Array = array.NotNull();
        var length = _Array.Length;
        _Indexes   = new int[length];
        _IndexInfo = new IndexInfo(_Indexes);

        if (Inverted)
            for (var i = 0; i < length; i++)
                _Indexes[i] = length - i - 1;
        else
            for (var i = 0; i < length; i++)
                _Indexes[i] = i;
    }

    #region ICollection

    /// <inheritdoc />
    bool ICollection.IsSynchronized => false;

    /// <inheritdoc />
    object ICollection.SyncRoot => this;

    /// <inheritdoc />
    int ICollection.Count => _Array.Length;

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
    {
        var length = _Array.Length;
        for (var i = 0; i < length; i++)
            array.SetValue(this[i], i + index);
    } 

    #endregion

    #region IEnumerable<T>

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        var length = _Indexes.Length;
        for (var i = 0; i < length; i++)
            yield return _Array[_Indexes[i]];
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    public static implicit operator RefArrayView<T>(T[] array) => new(array);

    public static implicit operator T[](RefArrayView<T> view)
    {
        var length = view.Length;
        var result = new T[length];
        for (var i = 0; i < length; i++)
            result[i] = view[i];
        return result;
    }
}