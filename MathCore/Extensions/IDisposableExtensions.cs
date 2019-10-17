using NN = MathCore.Annotations.NotNullAttribute;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class IDisposableExtensions
    {
        public static void DisposeAfter<T>(this T obj, [NN] Action<T> action)
            where T : IDisposable
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            using (obj) action(obj);
        }

        public static void DisposeAfter<T, TP>(this T obj, TP p, [NN] Action<T, TP> action)
            where T : IDisposable
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            using (obj) action(obj, p);
        }

        public static void DisposeAfter<T, TP1, TP2>(this T obj, TP1 p1, TP2 p2, [NN] Action<T, TP1, TP2> action)
            where T : IDisposable
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            using (obj) action(obj, p1, p2);
        }  

        public static void DisposeAfter<T, TP1, TP2, TP3>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Action<T, TP1, TP2, TP3> action)
            where T : IDisposable
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            using (obj) action(obj, p1, p2, p3);
        } 
        public static TResult DisposeAfter<T, TResult>(this T obj, [NN] Func<T, TResult> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return func(obj);
        }

        public static TResult DisposeAfter<T, TP, TResult>(this T obj, TP p, [NN] Func<T, TP, TResult> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return func(obj, p);
        }

        public static TResult DisposeAfter<T, TP1, TP2, TResult>(this T obj, TP1 p1, TP2 p2, [NN] Func<T, TP1, TP2, TResult> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return func(obj, p1, p2);
        }  

        public static TResult DisposeAfter<T, TP1, TP2, TP3, TResult>(this T obj, TP1 p1, TP2 p2, TP3 p3, [NN] Func<T, TP1, TP2, TP3, TResult> func)
            where T : IDisposable
        {
            if (func is null) throw new ArgumentNullException(nameof(func));

            using (obj) return func(obj, p1, p2, p3);
        }
    }
}