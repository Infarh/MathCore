﻿#nullable enable
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Values;

/// <summary>Очередь с линейным доступом</summary>
/// <typeparam name="T">Тип элементов очереди</typeparam>
public class LinearQueue<T>(T[] Buffer)
{
    public LinearQueue(int Length) : this(new T[Length]) { }

    private int _Offset;
    private int _AddedCount;

    public int Length => Buffer.Length;

    public ref T this[int i] => ref Buffer[(i + _Offset) % Length];
    
    /// <summary>Добавляет элемент в очередь</summary>
    /// <param name="t">Добавляемый элемент</param>
    /// <returns>Последний элемент, который был в очереди на месте добавленного элемента</returns>
    /// <remarks>Если очередь заполнена, то последний добавленный элемент будет заменен</remarks>
    public T Add(T t)
    {
        var offset = _Offset;
        var last   = Buffer[offset];
        Buffer[offset] = t;
        _Offset++;
        _Offset %= Length;
        _AddedCount++;
        return last;
    }


    public T[] ToArray()
    {
        var result = new T[Length];
        Array.Copy(Buffer, _Offset, result, 0, Length - _Offset);
        Array.Copy(Buffer, 0, result, Length - _Offset, _Offset);
        return result;
    }

    public void CopyTo(T[] array, int Index)
    {
        if (_Offset == 0)
            Array.Copy(Buffer, 0, array, Index, Length);
        else
        {
            Array.Copy(Buffer, _Offset, array, Index, Length - _Offset);
            Array.Copy(Buffer, 0, array, Index + Length - _Offset, _Offset);
        }
    }

    public override string ToString()
    {
        if (_Offset == 0 && _AddedCount > 0)
            return new StringBuilder()
               .Append('[')
               .Append(_AddedCount > 0 ? string.Join(", ", Buffer) : " ")
               .Append(']')
               .ToString();

        var result = new StringBuilder();
        result.Append('[');

        if (_AddedCount == 0)
            result.Append(' ');
        else if (_AddedCount <= Length)
            result.Append(string.Join(", ", Buffer.Take(_AddedCount)));
        else
        {
            var items1 = Buffer.Skip(_Offset);
            var items2 = Buffer.Take(_Offset);
            var items  = items1.Concat(items2);
            result.Append(string.Join(", ", items));
        }

        return result.Append(']').ToString();
    }
}