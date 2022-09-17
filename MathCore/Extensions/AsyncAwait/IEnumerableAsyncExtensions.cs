using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

// ReSharper disable once InconsistentNaming
public static class IEnumerableAsyncExtensions
{
    public static IEnumerable<Task<TValue>> SelectAsync<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Selector, Cancel);
    }

    public static IEnumerable<Task<TValue>> SelectAsync<TItem, TParameter, TValue>(this IEnumerable<TItem> items, TParameter Parameter, Func<TItem, TParameter, TValue> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter, Selector, Cancel);
    }

    public static IEnumerable<Task<TValue>> SelectAsync<TItem, TParameter1, TParameter2, TValue>(this IEnumerable<TItem> items, TParameter1 Parameter1, TParameter2 Parameter2, Func<TItem, TParameter1, TParameter2, TValue> Selector, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter1, Parameter2, Selector, Cancel);
    }

    public static IEnumerable<Task> SelectAsync<T>(this IEnumerable<T> items, Action<T> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(action, Cancel);
    }

    public static IEnumerable<Task> SelectAsync<T>(this IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(i => { }, Cancel);
    }

    public static IEnumerable<Task> SelectAsync<T, TParameter>(this IEnumerable<T> items, TParameter Parameter, Action<T, TParameter> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter, action, Cancel);
    }

    public static IEnumerable<Task> SelectAsync<T, TParameter1, TParameter2>(this IEnumerable<T> items, TParameter1 Parameter1, TParameter2 Parameter2, Action<T, TParameter1, TParameter2> action, CancellationToken Cancel = default)
    {
        foreach (var item in items)
            yield return item.Async(Parameter1, Parameter2, action, Cancel);
    }

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