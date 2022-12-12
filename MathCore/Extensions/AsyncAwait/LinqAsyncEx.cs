#nullable enable

// ReSharper disable once CheckNamespace
namespace MathCore.Linq.Async;

public static class LinqAsyncEx
{
    public static IEnumerable<Task<TResult>> SelectAsync<T, TResult>(this IEnumerable<Task<T>> tasks, Func<T, TResult> Converter)
    {
        static async Task<TResult> Convert(Task<T> task, Func<T, TResult> converter) => converter(await task.ConfigureAwait(false));
        foreach (var task in tasks)
            yield return Convert(task, Converter);
    }

    public static IEnumerable<Task<TResult>> SelectAsync<T, TResult>(this IEnumerable<Task<T>> tasks, Func<T, TResult> Converter, TaskScheduler Scheduler)
    {
        static async Task<TResult> Convert(Task<T> task, Func<T, TResult> converter, TaskScheduler scheduler)
        {
            await scheduler.SwitchContext();
            return converter(await task.ConfigureAwait(false));
        }

        foreach (var task in tasks)
            yield return Convert(task, Converter, Scheduler);
    }

    public static Task<T[]> AggregateAsync<T>(this IEnumerable<Task<T>> tasks) => tasks.WhenAll();
}