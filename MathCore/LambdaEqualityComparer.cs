#nullable enable
using System;
using System.Collections.Generic;

// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore;

public class LambdaEqualityComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> _Comparer;

    private readonly Func<T, int> _HashFunction;

    public LambdaEqualityComparer(Func<T, T, bool> Comparer, Func<T, int>? HashFunction = null)
    {
        _Comparer = Comparer;
        _HashFunction = HashFunction ?? (o => o.GetHashCode());
    }

    /// <summary>Определяет, равны ли два указанных объекта.</summary>
    /// <returns>Значение true, если указанные объекты равны; в противном случае — значение false.</returns>
    /// <param name="x">Первый сравниваемый объект типа <typeparamref name="T"/>.</param>
    /// <param name="y">Второй сравниваемый объект типа <typeparamref name="T"/>.</param>
    public bool Equals(T? x, T? y) => _Comparer(x, y);

    /// <summary>Возвращает хэш-код указанного объекта.</summary>
    /// <returns>Хэш-код указанного объекта.</returns>
    /// <param name="obj">Объект <see cref="T:System.Object"/>, для которого должен быть возвращен хэш-код.</param>
    /// <exception cref="T:System.ArgumentNullException">Тип <paramref name="obj"/> является ссылочным типом, значением <paramref name="obj"/> является null.</exception>
    public int GetHashCode(T obj) => _HashFunction(obj);
}

public static class LambdaEqualityComparer
{
    public static LambdaEqualityComparer<T> Create<T>(
        this Func<T, T, bool> Comparer, 
        Func<T, int>? HashFunction = null)
        => new(Comparer, HashFunction);
}