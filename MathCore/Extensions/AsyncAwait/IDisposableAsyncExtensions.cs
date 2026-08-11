
// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

/// <summary>Методы-расширения для асинхронного выполнения действия с последующим освобождением ресурсов</summary>
// ReSharper disable once InconsistentNaming
public static class IDisposableAsyncExtensions
{
    /// <summary>Выполняет действие с объектом и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="action">Выполняемое действие</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T>(this T obj, Action<T> action, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        using (obj) await obj.Async(action, Cancel).ConfigureAwait(false);
    } 

    /// <summary>Выполняет действие с объектом и одним параметром и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP">Тип параметра</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="action">Выполняемое действие</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, Action<T, TP> action, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(action is null) throw new ArgumentNullException(nameof(action));

        using (obj) await obj.Async(p, action, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет действие с объектом и двумя параметрами и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP1">Тип первого параметра</typeparam>
    /// <typeparam name="TP2">Тип второго параметра</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="action">Выполняемое действие</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, Action<T, TP1, TP2> action, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(action is null) throw new ArgumentNullException(nameof(action));

        using (obj) await obj.Async(p1, p2, action, Cancel).ConfigureAwait(false);
    }  

    /// <summary>Выполняет действие с объектом и тремя параметрами и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP1">Тип первого параметра</typeparam>
    /// <typeparam name="TP2">Тип второго параметра</typeparam>
    /// <typeparam name="TP3">Тип третьего параметра</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="action">Выполняемое действие</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, Action<T, TP1, TP2, TP3> action, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(action is null) throw new ArgumentNullException(nameof(action));

        using (obj) await obj.Async(p1, p2, p3, action, Cancel).ConfigureAwait(false);
    }

    /* --------------------------------------------------------------------------------- */

    /// <summary>Выполняет функцию с объектом и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TResult">Тип результата функции</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TResult>(this T obj, Func<T, TResult> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await obj.Async(func, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет функцию с объектом и одним параметром и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP">Тип параметра</typeparam>
    /// <typeparam name="TResult">Тип результата функции</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, Func<T, TP, TResult> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await obj.Async(p, func, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет функцию с объектом и двумя параметрами и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP1">Тип первого параметра</typeparam>
    /// <typeparam name="TP2">Тип второго параметра</typeparam>
    /// <typeparam name="TResult">Тип результата функции</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, TResult> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await obj.Async(p1, p2, func, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет функцию с объектом и тремя параметрами и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TP1">Тип первого параметра</typeparam>
    /// <typeparam name="TP2">Тип второго параметра</typeparam>
    /// <typeparam name="TP3">Тип третьего параметра</typeparam>
    /// <typeparam name="TResult">Тип результата функции</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, Func<T, TP1, TP2, TP3, TResult> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if(func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await obj.Async(p1, p2, p3, func, Cancel).ConfigureAwait(false);
    }

    /* --------------------------------------------------------------------------------- */

    /// <summary>Выполняет асинхронную функцию с объектом и освобождает его ресурсы</summary>
    /// <typeparam name="T">Тип освобождаемого объекта</typeparam>
    /// <typeparam name="TResult">Тип результата функции</typeparam>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="func">Выполняемая функция</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TResult>(this T obj, Func<T, Task<TResult>> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и одним параметром и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, Func<T, TP, Task<TResult>> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj, p).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и двумя параметрами и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, Task<TResult>> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj,p1, p2).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и тремя параметрами и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, Func<T, TP1, TP2, TP3, Task<TResult>> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj,p1, p2, p3).ConfigureAwait(false);
    }

    /* --------------------------------------------------------------------------------- */

    /// <summary>Выполняет асинхронную функцию с объектом и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TResult>(this T obj, Func<T, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, одним параметром и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, Func<T, TP, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj, p, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, двумя параметрами и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj,p1, p2, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, тремя параметрами и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Результат функции</returns>
    public static async Task<TResult?> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, Func<T, TP1, TP2, TP3, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) return await func(obj,p1, p2, p3, Cancel).ConfigureAwait(false);
    }

    /* --------------------------------------------------------------------------------- */

    /// <summary>Выполняет асинхронную функцию с объектом и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="func">Выполняемая функция</param>
    public static async Task DisposeAfterAsync<T>(this T obj, Func<T, Task> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и одним параметром и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="func">Выполняемая функция</param>
    public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, Func<T, TP, Task> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и двумя параметрами и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="func">Выполняемая функция</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, Task> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p1, p2).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и тремя параметрами и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="func">Выполняемая функция</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, Func<T, TP1, TP2, TP3, Task> func)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p1, p2, p3).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T>(this T obj, Func<T, CancellationToken, Task> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, одним параметром и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p">Параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, Func<T, TP, CancellationToken, Task> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, двумя параметрами и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, Func<T, TP1, TP2, CancellationToken, Task> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p1, p2, Cancel).ConfigureAwait(false);
    }

    /// <summary>Выполняет асинхронную функцию с объектом, тремя параметрами и токеном отмены и освобождает его ресурсы</summary>
    /// <param name="obj">Освобождаемый объект</param>
    /// <param name="p1">Первый параметр</param>
    /// <param name="p2">Второй параметр</param>
    /// <param name="p3">Третий параметр</param>
    /// <param name="func">Выполняемая функция</param>
    /// <param name="Cancel">Токен отмены</param>
    public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, Func<T, TP1, TP2, TP3, CancellationToken, Task> func, CancellationToken Cancel = default)
        where T : IDisposable
    {
        if (func is null) throw new ArgumentNullException(nameof(func));

        using (obj) await func(obj, p1, p2, p3, Cancel).ConfigureAwait(false);
    }
}