using System.Linq;
using MathCore.Annotations;

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    public static class LinkedListNodeExtensions
    {
        [NotNull]
        public static LinkedListNode<T> AddBefore<T>([NotNull] this LinkedListNode<T> node, T value) => node.List.AddBefore(node, value);

        [NotNull]
        public static LinkedListNode<T> AddAfter<T>([NotNull] this LinkedListNode<T> node, T value) => node.List.AddAfter(node, value);

        public static int GetIndex<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n.Previous).Count() - 1;

        [NotNull]
        public static IEnumerable<LinkedListNode<T>> GetNextNodes<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n.Next).Skip(1);

        [NotNull]
        public static IEnumerable<LinkedListNode<T>> GetPreviousNodes<T>(this LinkedListNode<T> node) => node.AsEnumerable(n => n.Previous).Skip(1);
    }
}