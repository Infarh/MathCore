#nullable enable
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Values;

/// <summary>Очередь с линейным доступом</summary>
/// <typeparam name="T">Тип элементов очереди</typeparam>
public class LinearQueue<T>
{
    private readonly T[] _Buffer;
    private int _Offset;
    private int _AddedCount;

    public int Length { get; }

    public ref T this[int i] => ref _Buffer[(i + _Offset) % Length];

    public LinearQueue(T[] buffer) => (_Buffer, Length) = (buffer, buffer.Length);
    public LinearQueue(int Length) => _Buffer = new T[this.Length = Length];

    public T Add(T t)
    {
        var offset = _Offset;
        var last   = _Buffer[offset];
        _Buffer[offset] = t;
        _Offset++;
        _Offset %= Length;
        _AddedCount++;
        return last;
    }

    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(_Buffer, _Offset, result, 0, Length - _Offset);
        Array.Copy(_Buffer, 0, result, Length - _Offset, _Offset);
        return result;
    }

    public void CopyTo(T[] array, int Index)
    {
        if (_Offset == 0)
            Array.Copy(_Buffer, 0, array, Index, Length);
        else
        {
            Array.Copy(_Buffer, _Offset, array, Index, Length - _Offset);
            Array.Copy(_Buffer, 0, array, Index + Length - _Offset, _Offset);
        }
    }

    public override string ToString()
    {
        if (_Offset == 0 && _AddedCount > 0)
            return new StringBuilder()
               .Append('[')
               .Append(_AddedCount > 0 ? string.Join(", ", _Buffer) : " ")
               .Append(']')
               .ToString();

        var result = new StringBuilder();
        result.Append('[');

        if (_AddedCount == 0)
            result.Append(' ');
        else if (_AddedCount <= Length)
            result.Append(string.Join(", ", _Buffer.Take(_AddedCount)));
        else
        {
            var items1 = _Buffer.Skip(_Offset);
            var items2 = _Buffer.Take(_Offset);
            var items  = items1.Concat(items2);
            result.Append(string.Join(", ", items));
        }

        return result.Append(']').ToString();
    }
}