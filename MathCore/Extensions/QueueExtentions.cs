namespace System.Collections.Generic
{
    public static class QueueExtentions
    {
        public static void Add<T>(this Queue<T> queue, T value) => queue.Enqueue(value);

        public static Queue<T> AddValue<T>(this Queue<T> queue, T value)
        {
            queue.Enqueue(value);
            return queue;
        }

        public static Stack<T> ToStack<T>(this Queue<T> queue) => new Stack<T>(queue);

        public static Stack<T> ToStackReverse<T>(this Queue<T> queue)
        {
            var items = new T[queue.Count];
            queue.CopyTo(items,0);
            Array.Reverse(items);
            return new Stack<T>(items);
        }
    }
}