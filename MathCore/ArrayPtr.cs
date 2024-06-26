using System.ComponentModel;

namespace MathCore;

public static class ArrayPtr
{
    public static ArrayPtr<T> Create<T>(T[] array) => new(array);

    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array) => new(array);
    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array, int Offset) => new(array, Offset);
    public static ArrayPtr<T> ToArrayPtr<T>(this T[] array, int Offset, int Length) => new(array, Offset, Length);
}

public readonly ref struct ArrayPtr<T>(T[] array, int Offset = 0, int Length = -1)
{
    public ArrayPtr(T[] array, (int Offset, int Length) Position) : this(array, Position.Offset, Position.Length) { }

    private readonly T[] _Array = array;

    private readonly int _Offset = Offset < 0 ? (Length < 0 ? array.Length : Math.Max(array.Length, Length)) + Offset : Offset;

    public int Length { get; } = Length < 0 ? array.Length : Math.Min(Length, array.Length);

    public ref T this[int index] => ref _Array[_Offset + (index < 0 ? Length + index : index)];

    private int GetIndex(int index) => _Offset + (index < 0 ? Length + index : index);

    public ArrayPtr<T> this[int Start, int End] => new(_Array, (GetIndex(Start), GetIndex(End)).MinMaxToMinLength());

    public ArrayPtr<T> Slice(int Offset) => new(_Array, _Offset + Offset, Length - Offset);

    public ArrayPtr<T> Slice(int Offset, int Length) => new(_Array, _Offset + Offset, Math.Min(this.Length - Offset, Length));

    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(_Array, _Offset, result, 0, Length);
        return result;
    }

    public override string ToString() => $"{typeof(T).Name}[off:{_Offset}, len{Length}]*";

    public override int GetHashCode() => HashBuilder.New(_Array).Append(_Offset).Append(Length);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => throw new NotSupportedException();

    public bool Equals(ArrayPtr<T> other) => 
        ReferenceEquals(_Array, other._Array) 
        && _Offset == other._Offset 
        && Length == other.Length;

    public static bool operator ==(ArrayPtr<T> a, ArrayPtr<T> b) => a.Equals(b);
    public static bool operator !=(ArrayPtr<T> a, ArrayPtr<T> b) => !a.Equals(b);

    public static implicit operator ArrayPtr<T>(T[] array) => new(array);

    public static explicit operator T[](ArrayPtr<T> ptr) => 
        ptr._Offset == 0 && ptr.Length == ptr._Array.Length 
            ? ptr._Array 
            : ptr.ToArray();
}
