using NN = MathCore.Annotations.NotNullAttribute;
using IcN = MathCore.Annotations.ItemCanBeNullAttribute;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public static class IDisposableAsyncExtensions
    {
        [NN]
        public static async Task DisposeAfterAsync<T>(this T obj, [NN] Action<T> action, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            using (obj) await obj.Async(action, Cancel).ConfigureAwait(false);
        } 

        [NN]
        public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, [NN] Action<T, TP> action, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            using (obj) await obj.Async(p, action, Cancel).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, [NN] Action<T, TP1, TP2> action, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            using (obj) await obj.Async(p1, p2, action, Cancel).ConfigureAwait(false);
        }  

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Action<T, TP1, TP2, TP3> action, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(action is null) throw new ArgumentNullException(nameof(action));

            using (obj) await obj.Async(p1, p2, p3, action, Cancel).ConfigureAwait(false);
        }

        /* --------------------------------------------------------------------------------- */

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TResult>(this T obj, [NN] Func<T, TResult> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await obj.Async(func, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, [NN] Func<T, TP, TResult> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await obj.Async(p, func, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, TResult> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await obj.Async(p1, p2, func, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, TResult> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if(func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await obj.Async(p1, p2, p3, func, Cancel).ConfigureAwait(false);
        }

        /* --------------------------------------------------------------------------------- */

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TResult>(this T obj, [NN] Func<T, Task<TResult>> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, [NN] Func<T, TP, Task<TResult>> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj, p).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, Task<TResult>> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj,p1, p2).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, Task<TResult>> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj,p1, p2, p3).ConfigureAwait(false);
        }

        /* --------------------------------------------------------------------------------- */

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TResult>(this T obj, [NN] Func<T, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP, TResult>(this T obj, TP p, [NN] Func<T, TP, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj, p, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj,p1, p2, Cancel).ConfigureAwait(false);
        }

        [NN, IcN]
        public static async Task<TResult> DisposeAfterAsync<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, CancellationToken, Task<TResult>> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return await func(obj,p1, p2, p3, Cancel).ConfigureAwait(false);
        }

        /* --------------------------------------------------------------------------------- */

        [NN]
        public static async Task DisposeAfterAsync<T>(this T obj, [NN] Func<T, Task> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, [NN] Func<T, TP, Task> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, Task> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p1, p2).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, Task> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p1, p2, p3).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T>(this T obj, [NN] Func<T, CancellationToken, Task> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, Cancel).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP>(this T obj, TP p, [NN] Func<T, TP, CancellationToken, Task> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p, Cancel).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, CancellationToken, Task> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p1, p2, Cancel).ConfigureAwait(false);
        }

        [NN]
        public static async Task DisposeAfterAsync<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, CancellationToken, Task> func, CancellationToken Cancel = default)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) await func(obj, p1, p2, p3, Cancel).ConfigureAwait(false);
        }
    }
}