#nullable enable
using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore;

/// <summary>Индексатор массива элементов</summary>
/// <typeparam name="T">Тип элемента массива</typeparam>
public class ArrayIndexer<T>
{
    /// <summary>Индексируемый массив</summary>
    private T[] _Array;

    /// <summary>Индекс текущего элемента</summary>
    private int _Index;

    /// <summary>Длина массива</summary>
    public int Length => _Array.Length;

    /// <summary>Индекс текущего элемента</summary>
    public int Index { get => _Index; set => _Index = value; }

    /// <summary>Ссылка на текущий элемент массива</summary>
    public ref T Value => ref _Array[_Index];

    /// <summary>Элемент массива с указанным индексом</summary>
    /// <param name="index">Индекс элемента массива</param>
    /// <returns>Ссылка на элемент с указанным индексом</returns>
    public ref T this[int index] => ref _Array[index];

    /// <summary>Индексируемый массив</summary>
    public T[] Array { get => _Array; set => _Array = value ?? throw new ArgumentNullException(nameof(value)); }

    /// <summary>Инициализация нового <see cref="ArrayIndexer{T}"/></summary>
    public ArrayIndexer() => _Array = System.Array.Empty<T>();

    /// <summary>Инициализация нового <see cref="ArrayIndexer{T}"/></summary>
    /// <param name="Array">Индексируемый массив</param>
    /// <param name="Index">Индекс текущего элемента</param>
    public ArrayIndexer(T[] Array, int Index = 0)
    {
        _Array = Array ?? throw new ArgumentNullException(nameof(Array));
        _Index = Index;
    }

    /// <summary>Оператор неявного приведения типа <see cref="ArrayIndexer{T}"/> к <see cref="System.Array{T}"/></summary>
    /// <param name="Indexer">Индексатор массива <see cref="ArrayIndexer{T}"/></param>
    /// <returns>Индексируемый массив элементов</returns>
    public static implicit operator T[](ArrayIndexer<T> Indexer) => Indexer._Array;
}