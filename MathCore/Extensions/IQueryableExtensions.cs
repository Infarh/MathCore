using System.Linq.Expressions;

using MathCore.Extensions.Expressions;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System.Linq;

/// <summary>Класс методов-расширений для интерфейса <see cref="IQueryable{T}"/></summary>
// ReSharper disable once InconsistentNaming
public static class IQueryableExtensions
{
    /// <summary>Получить элементы перечисления для заданной страницы</summary>
    /// <typeparam name="T">Тип элемента перечисления</typeparam>
    /// <param name="items">Исходное перечисление элементов</param>
    /// <param name="PageNumber">Номер требуемой страницы</param>
    /// <param name="PageItemsCount">Количество элементов на одну страницу</param>
    /// <returns>Перечисление элементов из указанной страницы</returns>
    public static IQueryable<T> Page<T>(this IQueryable<T> items, int PageNumber, int PageItemsCount) =>
        items.Skip(PageItemsCount * PageNumber).Take(PageItemsCount);

    /// <summary>Вычисляет дисперсию элементов последовательности</summary>
    /// <param name="query">Последовательность чисел</param>
    /// <returns>Дисперсия</returns>
    public static double Dispersion(this IQueryable<double> query) => query.Average(x => x * x - query.Average() * query.Average());
        
    /// <summary>Выполняет левое внешнее соединение двух последовательностей</summary>
    /// <param name="OuterItems">Внешняя последовательность</param>
    /// <param name="InnerItems">Внутренняя последовательность</param>
    /// <param name="OuterKeySelector">Функция выбора ключа из внешней последовательности</param>
    /// <param name="InnerKeySelector">Функция выбора ключа из внутренней последовательности</param>
    /// <param name="ResultSelector">Функция формирования результата соединения</param>
    /// <typeparam name="T1">Тип элементов внешней последовательности</typeparam>
    /// <typeparam name="T2">Тип элементов внутренней последовательности</typeparam>
    /// <typeparam name="TKey">Тип ключа соединения</typeparam>
    /// <typeparam name="TResult">Тип результата соединения</typeparam>
    /// <returns>Результат левого внешнего соединения</returns>
    public static IQueryable<TResult> LeftOuterJoin<T1, T2, TKey, TResult>(
        this IQueryable<T1> OuterItems,
        IEnumerable<T2> InnerItems,
        Expression<Func<T1, TKey>> OuterKeySelector,
        Expression<Func<T2, TKey>> InnerKeySelector,
        Expression<Func<T1, T2, TResult>> ResultSelector
    )
    {
        var v  = Expression.Parameter(typeof(Tuple<T1, IEnumerable<T2>>), "v");
        var t2 = ResultSelector.Parameters.Last();

        var t1 = Expression.Property(v, nameof(Tuple<T1, IEnumerable<T2>>.Item1));

        var body            = ResultSelector.Body.Replace(ResultSelector.Parameters.First(), t1);
        var result_selector = body.CreateLambda<Func<Tuple<T1, IEnumerable<T2>>, T2, TResult>>(v, t2);

        return OuterItems
           .GroupJoin(
                InnerItems,
                OuterKeySelector,
                InnerKeySelector,
                (Primary, Items) => new Tuple<T1, IEnumerable<T2>>(Primary, Items))
           .SelectMany(
                value => value.Item2.DefaultIfEmpty()!,
                result_selector
            );
    }
}