#nullable enable
using System.Collections;

// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Объект, представляющий метод сравнения двух объектов типа <typeparamref name="T"/>, задаваемый lambda-выражением</summary>
/// <typeparam name="T">Тип сравниваемых объектов</typeparam>
public class LambdaComparer<T> : IComparer<T>, IComparer
{
    /* ------------------------------------------------------------------------------------------ */

    private readonly Func<T, T, int> _Comparer;

    /* ------------------------------------------------------------------------------------------ */

    public LambdaComparer(Func<T, T, int> Comparer) => _Comparer = Comparer;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Сравнивает два объекта и возвращает значение, показывающее, что один объект меньше или больше другого или равен ему</summary>
    /// <param name="x">Первый сравниваемый объект.</param>
    /// <param name="y">Второй сравниваемый объект.</param>
    /// <returns>
    /// Значение Условие Меньше нуля<paramref name="x"/> меньше, чем <paramref name="y"/>.Нуль<paramref name="x"/> равно <paramref name="y"/>.Больше нуля<paramref name="x"/> больше, чем <paramref name="y"/>.
    /// </returns>
    public int Compare(T x, T y) => _Comparer(x, y);

    /// <inheritdoc />
    int IComparer.Compare(object x, object y) => Compare((T)x, (T)y);

    /* ------------------------------------------------------------------------------------------ */
}