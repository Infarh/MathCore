#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Методы-расширения для <see cref="Queue{T}"/></summary>
public static class QueueExtensions
{
    public static Queue<T> ToQueue<T>(this IEnumerable<T> items) => new(items);

    /// <summary>Добавить элемент в очередь</summary>
    /// <typeparam name="T">Тип элементов очереди</typeparam>
    /// <param name="queue">Очередь, в которую надо добавить элемент</param>
    /// <param name="value">Добавляемый элемент</param>
    public static void Add<T>(this Queue<T> queue, T value) => queue.Enqueue(value);

    /// <summary>Добавить элемент в очередь и вернуть модифицированную очередь в качестве результата</summary>
    /// <typeparam name="T">Тип элементов очереди</typeparam>
    /// <param name="queue">Очередь, в которую надо добавить элемент</param>
    /// <param name="value">Добавляемый элемент</param>
    /// <returns>Модифицированная очередь</returns>
    public static Queue<T> AddValue<T>(this Queue<T> queue, T value)
    {
        queue.Enqueue(value);
        return queue;
    }

    public static Queue<T> AddValuesRange<T>(this Queue<T> queue, IEnumerable<T>? values)
    {
        switch (values)
        {
            case null:                                return queue;
            case T[] { Length                  : 0 }: return queue;
            case List<T> { Count               : 0 }: return queue;
            case IList<T> { Count              : 0 }: return queue;
            case ICollection<T> { Count        : 0 }: return queue;
            case IReadOnlyCollection<T> { Count: 0 }: return queue;

            case T[] items:
                foreach (var item in items)
                    queue.Enqueue(item);
                return queue;

            case List<T> { Count: var list_items_count } items:
                for (var i = 0; i < list_items_count; i++)
                    queue.Enqueue(items[i]);
                return queue;

            case IList<T> { Count: var list_items_count } items:
                for (var i = 0; i < list_items_count; i++)
                    queue.Enqueue(items[i]);
                return queue;

            default:
                foreach (var item in values)
                    queue.Enqueue(item);
                return queue;
        }
    }

    public static IEnumerable<T> EnumerateToEnd<T>(this Queue<T> items)
    {
        while (items.Count > 0)
            yield return items.Dequeue();
    }

    /// <summary>Преобразовать очередь в стек в обратном порядке</summary>
    /// <typeparam name="T">Тип элементов очереди</typeparam>
    /// <param name="queue">Преобразуемая очередь</param>
    /// <returns>Стек из элементов очереди, взятых в обратном порядке</returns>
    public static Stack<T> ToStackReverse<T>(this Queue<T> queue)
    {
        var items = new T[queue.Count];
        queue.CopyTo(items,0);
        Array.Reverse(items);
        return new Stack<T>(items);
    }
}