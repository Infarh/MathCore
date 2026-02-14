using System.Diagnostics.CodeAnalysis;

using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

public static class AsyncExtensions
{
    /// <summary>Асинхронно обрабатывает элементы, получаемые от Producer, с помощью Consumer</summary>
    /// <param name="source">Источник данных</param>
    /// <param name="Producer">Функция получения элемента и признака завершения</param>
    /// <param name="Consumer">Функция обработки элемента</param>
    public static async Task Process<TSource, TValue>(
        [DisallowNull] this TSource source,
        Func<TSource, Task<(TValue Value, bool IsComplete)>> Producer,
        Func<TValue, Task> Consumer)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Producer is null) throw new ArgumentNullException(nameof(Producer));
        if (Consumer is null) throw new ArgumentNullException(nameof(Consumer));

        var reading = Producer(source);
        do
        {
            var (value, complete) = await reading.ConfigureAwait(false);
            reading = complete ? null : Producer(source);
            await Consumer(value).ConfigureAwait(false);
        } while (reading != null);
    }

    /// <summary>Асинхронно обрабатывает элементы с дополнительным параметром, получаемые от Producer, с помощью Consumer</summary>
    /// <param name="source">Источник данных</param>
    /// <param name="p">Дополнительный параметр</param>
    /// <param name="Producer">Функция получения элемента и признака завершения</param>
    /// <param name="Consumer">Функция обработки элемента</param>
    public static async Task Process<TSource, TValue, TP>(
        [DisallowNull] this TSource source,
        TP p,
        Func<TSource, TP, Task<(TValue Value, bool IsComplete)>> Producer,
        Func<TValue, TP, Task> Consumer)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Producer is null) throw new ArgumentNullException(nameof(Producer));
        if (Consumer is null) throw new ArgumentNullException(nameof(Consumer));

        var reading = Producer(source, p);
        do
        {
            var (value, complete) = await reading.ConfigureAwait(false);
            reading = complete ? null : Producer(source, p);
            await Consumer(value, p).ConfigureAwait(false);
        } while (reading != null);
    }

    /// <summary>Асинхронно обрабатывает элементы с двумя дополнительными параметрами, получаемые от Producer, с помощью Consumer</summary>
    /// <param name="source">Источник данных</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="Producer">Функция получения элемента и признака завершения</param>
    /// <param name="Consumer">Функция обработки элемента</param>
    public static async Task Process<TSource, TValue, TP1, TP2>(
        [DisallowNull] this TSource source,
        TP1 p1,
        TP2 p2,
        Func<TSource, TP1, TP2, Task<(TValue Value, bool IsComplete)>> Producer,
        Func<TValue, TP1, TP2, Task> Consumer)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Producer is null) throw new ArgumentNullException(nameof(Producer));
        if (Consumer is null) throw new ArgumentNullException(nameof(Consumer));

        var reading = Producer(source, p1, p2);
        do
        {
            var (value, complete) = await reading.ConfigureAwait(false);
            reading = complete ? null : Producer(source, p1, p2);
            await Consumer(value, p1, p2).ConfigureAwait(false);
        } while (reading != null);
    }

    /// <summary>Асинхронно обрабатывает элементы с тремя дополнительными параметрами, получаемые от Producer, с помощью Consumer</summary>
    /// <param name="source">Источник данных</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="p3">Третий дополнительный параметр</param>
    /// <param name="Producer">Функция получения элемента и признака завершения</param>
    /// <param name="Consumer">Функция обработки элемента</param>
    public static async Task Process<TSource, TValue, TP1, TP2, TP3>(
        [DisallowNull] this TSource source,
        TP1 p1,
        TP2 p2,
        TP3 p3,
        Func<TSource, TP1, TP2, TP3, Task<(TValue Value, bool IsComplete)>> Producer,
        Func<TValue, TP1, TP2, TP3, Task> Consumer)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Producer is null) throw new ArgumentNullException(nameof(Producer));
        if (Consumer is null) throw new ArgumentNullException(nameof(Consumer));

        var reading = Producer(source, p1, p2, p3);
        do
        {
            var (value, complete) = await reading.ConfigureAwait(false);
            reading = complete ? null : Producer(source, p1, p2, p3);
            await Consumer(value, p1, p2, p3).ConfigureAwait(false);
        } while (reading != null);
    }

    /* ------------------------------------------------------------------------------------ */

    /// <summary>Выполняет действие асинхронно</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T>(this T obj, Action<T> action, CancellationToken Cancel = default)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        return Task.Factory.StartNew(pp =>
        {
            var method = (Action<T>)((object[]?)pp)[0];
            var arg = (T)((object[]?)pp)[1];
            method(arg);
        }, new object[] { action, obj }, Cancel);
    }

    /// <summary>Выполняет действие асинхронно с поддержкой отмены</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T>(this T obj, Action<T, CancellationToken> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, CancellationToken>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var c = (CancellationToken)((object[]?)pp)[2];
                method(arg, c);
            },
            new object?[] { action, obj, Cancel },
            Cancel);

    /// <summary>Выполняет действие асинхронно с дополнительным параметром</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p">Дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP>(this T obj, TP p, Action<T, TP> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP)((object[]?)pp)[2];
                method(arg, pp1);
            }, new object?[] { action, obj, p }, Cancel);

    /// <summary>Выполняет действие асинхронно с дополнительным параметром и поддержкой отмены</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p">Дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP>(this T obj, TP p, Action<T, TP, CancellationToken> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP, CancellationToken>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP)((object[]?)pp)[2];
                var c = (CancellationToken)((object[]?)pp)[3];
                method(arg, pp1, c);
            }, new object?[] { action, obj, p, Cancel }, Cancel);

    /// <summary>Выполняет действие асинхронно с двумя дополнительными параметрами</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, Action<T, TP1, TP2> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP1, TP2>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                method(arg, pp1, pp2);
            }, new object?[] { action, obj, p1, p2 }, Cancel);

    /// <summary>Выполняет действие асинхронно с двумя дополнительными параметрами и поддержкой отмены</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, Action<T, TP1, TP2, CancellationToken> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP1, TP2, CancellationToken>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                var c = (CancellationToken)((object[]?)pp)[4];
                method(arg, pp1, pp2, c);
            }, new object?[] { action, obj, p1, p2, Cancel }, Cancel);

    /// <summary>Выполняет действие асинхронно с тремя дополнительными параметрами</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="p3">Третий дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, Action<T, TP1, TP2, TP3> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP1, TP2, TP3>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                var pp3 = (TP3)((object[]?)pp)[4];
                method(arg, pp1, pp2, pp3);
            },
            new object?[] { action, obj, p1, p2, p3 },
            Cancel);

    /// <summary>Выполняет действие асинхронно с тремя дополнительными параметрами и поддержкой отмены</summary>
    /// <param name="obj">Объект для передачи в действие</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="p3">Третий дополнительный параметр</param>
    /// <param name="action">Действие для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task Async<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, Action<T, TP1, TP2, TP3, CancellationToken> action, CancellationToken Cancel = default) => action is null
        ? throw new ArgumentNullException(nameof(action))
        : Task.Factory.StartNew(
            pp =>
            {
                var method = (Action<T, TP1, TP2, TP3, CancellationToken>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                var pp3 = (TP3)((object[]?)pp)[4];
                var c = (CancellationToken)((object[]?)pp)[5];
                method(arg, pp1, pp2, pp3, c);
            },
            new object?[] { action, obj, p1, p2, p3, Cancel },
            Cancel);

    /// <summary>Выполняет функцию асинхронно и возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TResult>(this T obj, Func<T, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                return method(arg);
            },
            new object?[] { func, obj },
            Cancel);

    /// <summary>Выполняет функцию асинхронно с поддержкой отмены и возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TResult>(this T obj, Func<T, CancellationToken, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, CancellationToken, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var c = (CancellationToken)((object[]?)pp)[2];
                return method(arg, c);
            }, new object?[] { func, obj, Cancel }, Cancel);

    /// <summary>Выполняет функцию асинхронно с дополнительным параметром и возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p">Дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP, TResult>(this T obj, TP p, Func<T, TP, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, TP, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP)((object[]?)pp)[2];
                return method(arg, pp1);
            },
            new object?[] { func, obj, p },
            Cancel);

    /// <summary>Выполняет функцию асинхронно с дополнительным параметром и поддержкой отмены, возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p">Дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP, TResult>(this T obj, TP p, Func<T, TP, CancellationToken, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, TP, CancellationToken, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP)((object[]?)pp)[2];
                var c = (CancellationToken)((object[]?)pp)[3];
                return method(arg, pp1, c);
            }, new object?[] { func, obj, p, Cancel }, Cancel);

    /// <summary>Выполняет функцию асинхронно с двумя дополнительными параметрами и возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, TP1, TP2, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                return method(arg, pp1, pp2);
            }, new object?[] { func, obj, p1, p2 }, Cancel);

    /// <summary>Выполняет функцию асинхронно с двумя дополнительными параметрами и поддержкой отмены, возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, CancellationToken, TResult> func, CancellationToken Cancel = default) => func is null
        ? throw new ArgumentNullException(nameof(func))
        : Task<TResult>.Factory.StartNew(
            pp =>
            {
                var method = (Func<T, TP1, TP2, CancellationToken, TResult>)((object[]?)pp)[0];
                var arg = (T)((object[]?)pp)[1];
                var pp1 = (TP1)((object[]?)pp)[2];
                var pp2 = (TP2)((object[]?)pp)[3];
                var c = (CancellationToken)((object[]?)pp)[4];
                return method(arg, pp1, pp2, c);
            }, new object[] { func, obj, p1, p2, Cancel }, Cancel);

    /// <summary>Выполняет функцию асинхронно с тремя дополнительными параметрами и возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="p3">Третий дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP1, TP2, TP3, TResult>(
        this T obj,
        TP1 p1,
        TP2 p2,
        TP3 p3,
        Func<T, TP1, TP2, TP3, TResult> func,
        CancellationToken Cancel = default) =>
        func is null
            ? throw new ArgumentNullException(nameof(func))
            : Task<TResult>.Factory.StartNew(
                pp =>
                {
                    var method = (Func<T, TP1, TP2, TP3, TResult>)((object[]?)pp)[0];
                    var arg = (T)((object[]?)pp)[1];
                    var pp1 = (TP1)((object[]?)pp)[2];
                    var pp2 = (TP2)((object[]?)pp)[3];
                    var pp3 = (TP3)((object[]?)pp)[4];
                    return method(arg, pp1, pp2, pp3);
                },
                new object?[] { func, obj, p1, p2, p3 },
                Cancel);

    /// <summary>Выполняет функцию асинхронно с тремя дополнительными параметрами и поддержкой отмены, возвращает результат</summary>
    /// <param name="obj">Объект для передачи в функцию</param>
    /// <param name="p1">Первый дополнительный параметр</param>
    /// <param name="p2">Второй дополнительный параметр</param>
    /// <param name="p3">Третий дополнительный параметр</param>
    /// <param name="func">Функция для выполнения</param>
    /// <param name="Cancel">Токен отмены</param>
    public static Task<TResult> Async<T, TP1, TP2, TP3, TResult>(
        this T obj,
        TP1 p1,
        TP2 p2,
        TP3 p3,
        Func<T, TP1, TP2, TP3, CancellationToken, TResult> func,
        CancellationToken Cancel = default) =>
        func is null
            ? throw new ArgumentNullException(nameof(func))
            : Task<TResult>.Factory.StartNew(
                pp =>
                {
                    var method = (Func<T, TP1, TP2, TP3, CancellationToken, TResult>)((object[]?)pp)[0];
                    var arg = (T)((object[]?)pp)[1];
                    var pp1 = (TP1)((object[]?)pp)[2];
                    var pp2 = (TP2)((object[]?)pp)[3];
                    var pp3 = (TP3)((object[]?)pp)[4];
                    var c = (CancellationToken)((object[]?)pp)[5];
                    return method(arg, pp1, pp2, pp3, c);
                },
                new object?[] { func, obj, p1, p2, p3, Cancel },
                Cancel);
}