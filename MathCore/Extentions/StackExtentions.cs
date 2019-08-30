namespace System.Collections.Generic
{
    public static class StackExtentions
    {
        public static void Add<T>(this Stack<T> stack, T value) => stack.Push(value);

        public static Stack<T> AddValue<T>(this Stack<T> stack, T value)
        {
            stack.Push(value);
            return stack;
        }

        public static Queue<T> ToQueue<T>(this Stack<T> stack) => new Queue<T>(stack);

        public static Queue<T> ToQueueReverse<T>(this Stack<T> stack)
        {
            var items = new T[stack.Count];
            stack.CopyTo(items, 0);
            Array.Reverse(items);
            return new Queue<T>(items);
        } 
    }
}