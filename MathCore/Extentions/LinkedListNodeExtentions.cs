using System.Diagnostics.Contracts;
using System.Linq;

namespace System.Collections.Generic
{
    public static class LinkedListNodeExtentions
    {
        public static LinkedListNode<T> AddBefore<T>(this LinkedListNode<T> node, T value)
        {
            Contract.Requires(node != null);
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<LinkedListNode<T>>() != null);
            return node.List.AddBefore(node, value);
        }

        public static LinkedListNode<T> AddAfter<T>(this LinkedListNode<T> node, T value)
        {
            Contract.Requires(node != null);
            Contract.Requires(value != null);
            Contract.Ensures(Contract.Result<LinkedListNode<T>>() != null);
            return node.List.AddAfter(node, value);
        }

        public static int GetIndex<T>(this LinkedListNode<T> node)
        {
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<int>() >= 0);
            Contract.Ensures(Contract.Result<int>() < node.List.Count);
            return node.AsEnumerable(n => n.Previous).Count() - 1;
        }

        public static IEnumerable<LinkedListNode<T>> GetNextNodes<T>(this LinkedListNode<T> node)
        {
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<IEnumerable<LinkedListNode<T>>>() != null);
            return node.AsEnumerable(n => n.Next).Skip(1);
        }

        public static IEnumerable<LinkedListNode<T>> GetPreviousNodes<T>(this LinkedListNode<T> node)
        {
            Contract.Requires(node != null);
            Contract.Ensures(Contract.Result<IEnumerable<LinkedListNode<T>>>() != null);
            return node.AsEnumerable(n => n.Previous).Skip(1);
        }
    }
}