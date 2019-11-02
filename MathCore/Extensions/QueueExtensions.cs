using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class QueueExtensions
    {
        public static void Add<T>([NotNull] this Queue<T> queue, T value) => queue.Enqueue(value);

        [NotNull]
        public static Queue<T> AddValue<T>([NotNull] this Queue<T> queue, T value)
        {
            queue.Enqueue(value);
            return queue;
        }

        [NotNull] public static Stack<T> ToStack<T>([NotNull] this Queue<T> queue) => new Stack<T>(queue);

        [NotNull]
        public static Stack<T> ToStackReverse<T>([NotNull] this Queue<T> queue)
        {
            var items = new T[queue.Count];
            queue.CopyTo(items,0);
            Array.Reverse(items);
            return new Stack<T>(items);
        }
    }
}