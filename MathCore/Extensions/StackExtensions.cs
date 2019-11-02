using MathCore.Annotations;

namespace System.Collections.Generic
{
    public static class StackExtensions
    {
        public static void Add<T>([NotNull] this Stack<T> stack, T value) => stack.Push(value);

        [NotNull]
        public static Stack<T> AddValue<T>([NotNull] this Stack<T> stack, T value)
        {
            stack.Push(value);
            return stack;
        }

        [NotNull] public static Queue<T> ToQueue<T>([NotNull] this Stack<T> stack) => new Queue<T>(stack);

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