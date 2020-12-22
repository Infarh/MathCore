using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>Методы-расширения для <see cref="Queue{T}"/></summary>
    public static class QueueExtensions
    {
        /// <summary>Добавить элемент в очередь</summary>
        /// <typeparam name="T">Тип элементов очереди</typeparam>
        /// <param name="queue">Очередь, в которую надо добавить элемент</param>
        /// <param name="value">Добавляемый элемент</param>
        public static void Add<T>([NotNull] this Queue<T> queue, T value) => queue.Enqueue(value);

        /// <summary>Добавить элемент в очередь и вернуть модифицированную очередь в качестве результата</summary>
        /// <typeparam name="T">Тип элементов очереди</typeparam>
        /// <param name="queue">Очередь, в которую надо добавить элемент</param>
        /// <param name="value">Добавляемый элемент</param>
        /// <returns>Модифицированная очередь</returns>
        [NotNull]
        public static Queue<T> AddValue<T>([NotNull] this Queue<T> queue, T value)
        {
            queue.Enqueue(value);
            return queue;
        }

        /// <summary>Преобразовать очередь в стек</summary>
        /// <typeparam name="T">Тип элементов очереди</typeparam>
        /// <param name="queue">Преобразуемая очередь</param>
        /// <returns>Стек из элементов очереди</returns>
        [NotNull] public static Stack<T> ToStack<T>([NotNull] this Queue<T> queue) => new(queue);

        /// <summary>Преобразовать очередь в стек в обратном порядке</summary>
        /// <typeparam name="T">Тип элементов очереди</typeparam>
        /// <param name="queue">Преобразуемая очередь</param>
        /// <returns>Стек из элементов очереди, взятых в обратном порядке</returns>
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