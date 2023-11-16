namespace MathCore.Collections.Interfaces;

/// <summary>Страница элементов</summary>
/// <typeparam name="T">Тип элементов</typeparam>
public interface IPage<out T>
{
    /// <summary>Индекс страницы</summary>
    int Index { get; }

    /// <summary>Перечисление элементов страницы</summary>
    IEnumerable<T> Items { get; }

    /// <summary>Количество элементов на странице</summary>
    int Count { get; }

    /// <summary>Полное количество элементов в выборке</summary>
    int TotalCount { get; }

    /// <summary>Размер страницы</summary>
    int Size { get; }

    /// <summary>Число страниц в выборке</summary>
    int PagesCount { get; }
}