#nullable enable
using MathCore.Collections.Interfaces;

namespace MathCore.Collections;

/// <summary>Страница элементов</summary>
/// <typeparam name="T">Тип элементов</typeparam>
public class Page<T> : IPage<T>
{
    /// <summary>Пустая страница</summary>
    /// <param name="TotalCount">Полное число элементов в выборке</param>
    /// <param name="Index">Индекс страницы</param>
    /// <param name="Size">Размер страницы</param>
    /// <returns>Страница с пустым перечислением элементов</returns>
    public static Page<T> Empty(int TotalCount, int Index, int Size) =>
        new([], 0, TotalCount, Index, Size);

    /// <summary>Новая страница элементов</summary>
    /// <param name="Items">Перечисление элементов страницы</param>
    /// <param name="Count">Количество элементов на странице</param>
    /// <param name="TotalCount">Полное количество элементов в выборке</param>
    /// <param name="Index">Индекс страницы</param>
    /// <param name="Size">Размер страницы</param>
    public Page(IEnumerable<T> Items, int Count, int TotalCount, int Index, int Size)
    {
        this.Items = Items;
        this.Count = Count;
        this.TotalCount = TotalCount;
        this.Index = Index;
        this.Size = Size;
    }

    /// <summary>Индекс страницы</summary>
    public int Index { get; }

    /// <summary>Перечисление элементов страницы</summary>
    public IEnumerable<T> Items { get; }

    /// <summary>Количество элементов на странице</summary>
    public int Count { get; }

    /// <summary>Полное количество элементов в выборке</summary>
    public int TotalCount { get; }

    /// <summary>Размер страницы</summary>
    public int Size { get; }

    /// <summary>Число страниц в выборке</summary>
    public int PagesCount => (int)Math.Ceiling((double)TotalCount / Size);
}
