#nullable enable
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

/// <summary>Методы-расширения для <see cref="Stack{T}"/></summary>
public static class StackExtensions
{
    public static Stack<T> ToStack<T>(this IEnumerable<T> items) => new(items);

    /// <summary>Добавить элемент в стек</summary>
    /// <typeparam name="T">Тип элементов стека</typeparam>
    /// <param name="stack">Стек, в который надо добавить элемент</param>
    /// <param name="value">Добавляемый элемент</param>
    public static void Add<T>(this Stack<T> stack, T value) => stack.Push(value);

    /// <summary>Добавить элемент в стек и вернуть модифицированный стек в качестве результата</summary>
    /// <typeparam name="T">Тип элементов стека</typeparam>
    /// <param name="stack">Стек, в который надо добавить элемент</param>
    /// <param name="value">Добавляемый элемент</param>
    /// <returns>Модифицированный стек</returns>
    public static Stack<T> AddValue<T>(this Stack<T> stack, T value)
    {
        stack.Push(value);
        return stack;
    }

    public static Stack<T> AddValuesRange<T>(this Stack<T> stack, IEnumerable<T>? values)
    {
        switch (values)
        {
            case null:                                return stack;
            case T[] { Length                  : 0 }: return stack;
            case List<T> { Count               : 0 }: return stack;
            case IList<T> { Count              : 0 }: return stack;
            case ICollection<T> { Count        : 0 }: return stack;
            case IReadOnlyCollection<T> { Count: 0 }: return stack;

            case T[] items:
                foreach (var item in items)
                    stack.Push(item);
                return stack;

            case List<T> { Count: var list_items_count } items:
                for (var i = 0; i < list_items_count; i++)
                    stack.Push(items[i]);
                return stack;

            case IList<T> { Count: var list_items_count } items:
                for (var i = 0; i < list_items_count; i++)
                    stack.Push(items[i]);
                return stack;

            default:
                foreach (var item in values)
                    stack.Push(item);
                return stack;
        }
    }

    public static IEnumerable<T> EnumerateToEnd<T>(this Stack<T> stack)
    {
        while (stack.Count > 0)
            yield return stack.Pop();
    }

    /// <summary>Преобразовать стек в инвертированную очередь</summary>
    /// <typeparam name="T">Тип элементов стека</typeparam>
    /// <param name="stack">Преобразуемый стек</param>
    /// <returns>Очередь из элементов стека, взятых в обратном порядке</returns>
    public static Queue<T> ToQueueReverse<T>(this Stack<T> stack)
    {
        var items = new T[stack.Count];
        stack.CopyTo(items, 0);
        Array.Reverse(items);
        return new Queue<T>(items);
    }
}