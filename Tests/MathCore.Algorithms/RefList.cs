using System;
using System.Collections;
using System.Collections.Generic;

namespace MathCore.Algorithms
{
    public class RefList<T> : IEnumerable<T>
    {
        public class Node
        {
            internal RefList<T> _List;

            private readonly T _Value;

            public T Value => _Value;

            public Node Next { get; internal set; }

            public Node Prev { get; internal set; }

            internal Node(RefList<T> List, T Value)
            {
                _List = List;
                _Value = Value;
            }
        }

        private int _Count;

        public Node First { get; private set; }

        public Node Last { get; private set; }

        public int Count => _Count;

        public RefList() { }

        public RefList(IEnumerable<T> items)
        {
            foreach (var item in items)
                AddLast(item);
        }

        public Node AddFirst(T value)
        {
            var node = new Node(this, value);
            _Count++;

            if (First is null)
            {
                First = node;
                Last = node;
                return node;
            }

            First.Prev = node;
            node.Next = First;
            First = node;

            return node;
        }

        public Node AddLast(T value)
        {
            var node = new Node(this, value);
            _Count++;

            if (Last is null)
            {
                First = node;
                Last = node;
                return node;
            }

            Last.Next = node;
            node.Prev = First;
            Last = node;

            return node;
        }

        public Node AddAfter(Node Position, T value)
        {
            if (!ReferenceEquals(Position._List, this))
                throw new InvalidOperationException("Попытка добавления в позицию элемента от другого списка");

            if (ReferenceEquals(Position, Last))
                return AddLast(value);

            var node = new Node(this, value)
            {
                Prev = Position,
                Next = Position.Next
            };
            Position.Next = node;
            node.Next.Prev = node;
            return node;
        }

        public Node AddBefore(Node Position, T value)
        {
            if (!ReferenceEquals(Position._List, this))
                throw new InvalidOperationException("Попытка добавления в позицию элемента от другого списка");

            if (ReferenceEquals(Position, First))
                return AddFirst(value);

            var node = new Node(this, value)
            {
                Next = Position,
                Prev = Position.Prev
            };
            Position.Prev = node;
            node.Prev.Next = node;
            return node;
        }

        public T Remove(Node node)
        {
            if (!ReferenceEquals(node._List, this))
                throw new InvalidOperationException("Попытка удаление элемента от другого списка");

            node._List = null;
            _Count--;
            if (_Count == 1)
            {
                First = null;
                Last = null;
            }
            else if (ReferenceEquals(First, node))
            {
                First = node.Next;
                First.Prev = null;
            }
            else if (ReferenceEquals(Last, node))
            {
                Last = node.Prev;
                Last.Next = null;
            }
            else
            {
                node.Prev.Next = node.Next;
                node.Next.Prev = node.Prev;
            }

            return node.Value;
        }

        public void Clear()
        {
            if (_Count == 0) return;
            var node = First;
            while (node != null)
            {
                node._List = null;
                node.Prev = null;
                var tmp = node;
                node = node.Next;
                tmp.Next = null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_Count == 0) yield break;
            var node = First;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
