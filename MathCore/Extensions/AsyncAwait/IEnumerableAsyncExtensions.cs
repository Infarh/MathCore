// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

// ReSharper disable once InconsistentNaming
/// <summary>Расширения для асинхронной обработки последовательностей</summary>
public static class IEnumerableAsyncExtensions
{
    /// <summary>Проецирует элементы последовательности в задачи с использованием селектора</summary>
    /// <typeparam name="TItem">Тип элемента последовательности</typeparam>
    /// <typeparam name="TValue">Тип результата</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Selector">Селектор результата</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task<TValue?>> SelectAsync<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue?> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Selector, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи с параметром</summary>
    /// <typeparam name="TItem">Тип элемента последовательности</typeparam>
    /// <typeparam name="TParameter">Тип параметра</typeparam>
    /// <typeparam name="TValue">Тип результата</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Parameter">Параметр селектора</param>
    /// <param name="Selector">Селектор результата</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task<TValue?>> SelectAsync<TItem, TParameter, TValue>(this IEnumerable<TItem> items, TParameter Parameter, Func<TItem, TParameter, TValue?> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter, Selector, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи с двумя параметрами</summary>
    /// <typeparam name="TItem">Тип элемента последовательности</typeparam>
    /// <typeparam name="TParameter1">Тип первого параметра</typeparam>
    /// <typeparam name="TParameter2">Тип второго параметра</typeparam>
    /// <typeparam name="TValue">Тип результата</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Parameter1">Первый параметр селектора</param>
    /// <param name="Parameter2">Второй параметр селектора</param>
    /// <param name="Selector">Селектор результата</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task<TValue?>> SelectAsync<TItem, TParameter1, TParameter2, TValue>(this IEnumerable<TItem> items, TParameter1 Parameter1, TParameter2 Parameter2, Func<TItem, TParameter1, TParameter2, TValue?> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter1, Parameter2, Selector, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи действия</summary>
    /// <typeparam name="T">Тип элемента последовательности</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="action">Действие над элементом</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task> SelectAsync<T>(this IEnumerable<T> items, Action<T> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(action, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи действия с токеном отмены</summary>
    /// <typeparam name="T">Тип элемента последовательности</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="action">Действие над элементом</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task> SelectAsync<T>(this IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(action, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи действия с параметром</summary>
    /// <typeparam name="T">Тип элемента последовательности</typeparam>
    /// <typeparam name="TParameter">Тип параметра</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Parameter">Параметр действия</param>
    /// <param name="action">Действие над элементом</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task> SelectAsync<T, TParameter>(this IEnumerable<T> items, TParameter Parameter, Action<T, TParameter> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter, action, Cancel);
    }

    /// <summary>Проецирует элементы последовательности в задачи действия с двумя параметрами</summary>
    /// <typeparam name="T">Тип элемента последовательности</typeparam>
    /// <typeparam name="TParameter1">Тип первого параметра</typeparam>
    /// <typeparam name="TParameter2">Тип второго параметра</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="Parameter1">Первый параметр действия</param>
    /// <param name="Parameter2">Второй параметр действия</param>
    /// <param name="action">Действие над элементом</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Последовательность задач</returns>
    public static IEnumerable<Task> SelectAsync<T, TParameter1, TParameter2>(this IEnumerable<T> items, TParameter1 Parameter1, TParameter2 Parameter2, Action<T, TParameter1, TParameter2> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter1, Parameter2, action, Cancel);
    }

    /// <summary>Выполняет действие для элементов последовательности параллельно</summary>
    /// <typeparam name="T">Тип элемента последовательности</typeparam>
    /// <param name="items">Последовательность элементов</param>
    /// <param name="action">Действие над элементом</param>
    /// <param name="cancel">Токен отмены</param>
    /// <returns>Задача выполнения</returns>
    /// <exception cref="ArgumentNullException">Если действие не задано</exception>
    public static async Task ForeachAsync<T>(this IEnumerable<T> items, Action<T> action, CancellationToken cancel = default)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));
        if (items is null) return;

        await Task.Yield().ConfigureAwait(false);
        items
           .AsParallel()
           .WithCancellation(cancel)
           .ForAll(action);
    }
}