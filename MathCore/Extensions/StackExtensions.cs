using MathCore.Annotations;
// ReSharper disable ParameterTypeCanBeEnumerable.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>Методы-расширения для <see cref="Stack{T}"/></summary>
    public static class StackExtensions
    {
        /// <summary>Добавить элемент в стек</summary>
        /// <typeparam name="T">Тип элементов стека</typeparam>
        /// <param name="stack">Стек, в который надо добавить элемент</param>
        /// <param name="value">Добавляемый элемент</param>
        public static void Add<T>([NotNull] this Stack<T> stack, T value) => stack.Push(value);

        /// <summary>Добавить элемент в стек и вернуть модифицированный стек в качестве результата</summary>
        /// <typeparam name="T">Тип элементов стека</typeparam>
        /// <param name="stack">Стек, в который надо добавить элемент</param>
        /// <param name="value">Добавляемый элемент</param>
        /// <returns>Модифицированный стек</returns>
        [NotNull]
        public static Stack<T> AddValue<T>([NotNull] this Stack<T> stack, T value)
        {
            stack.Push(value);
            return stack;
        }

        /// <summary>Преобразовать стек в очередь</summary>
        /// <typeparam name="T">Тип элементов стека</typeparam>
        /// <param name="stack">Преобразуемый стек</param>
        /// <returns>Очередь из элементов стека</returns>
        [NotNull] public static Queue<T> ToQueue<T>([NotNull] this Stack<T> stack) => new(stack);

        /// <summary>Преобразовать стек в инвертированную очередь</summary>
        /// <typeparam name="T">Тип элементов стека</typeparam>
        /// <param name="stack">Преобразуемый стек</param>
        /// <returns>Очередь из элементов стека, взятых в обратном порядке</returns>
        [NotNull]
        public static Queue<T> ToQueueReverse<T>([NotNull] this Stack<T> stack)
        {
            var items = new T[stack.Count];
            stack.CopyTo(items, 0);
            Array.Reverse(items);
            return new Queue<T>(items);
        } 
    }
}