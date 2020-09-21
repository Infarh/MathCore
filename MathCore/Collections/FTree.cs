#nullable enable
using System;
using System.Collections.Generic;

namespace MathCore.Collections
{
    public class FTree<T>
    {
        public static FTree<T> Empty { get; } = new FTree<T>();

        public bool IsEmpty => ReferenceEquals(this, Empty);

        public bool IsLeaf => IsEmpty || Left.IsEmpty && Right.IsEmpty;

        public T Root { get; }

        public FTree<T> Left { get; }

        public FTree<T> Right { get; }

        private FTree()
        {
            if (Empty is not null)
                throw new InvalidOperationException("Нельзя создать ещё один пустой узел дерева. Используйте статическое свойство Empty");
            Root = default!;
            Left = null!;
            Right = null!;
        }

        private FTree(T Root, FTree<T>? Left = null, FTree<T>? Right = null)
        {
            this.Root = Root;
            this.Left = Left ?? Empty;
            this.Right = Right ?? Empty;
        }

        public static FTree<T> New(T root, FTree<T>? left = null, FTree<T>? right = null) => new FTree<T>(root, left, right);

        public FTree<T> AddLeft(T root, FTree<T>? right = null) => new FTree<T>(root, this, right);

        public FTree<T> AddRight(T root, FTree<T>? left = null) => new FTree<T>(root, left, this);

        public IEnumerable<T> EnumLeftRootRight()
        {
            if(IsEmpty) yield break;
            if(IsLeaf) yield return Root;

            var nodes = new Stack<FTree<T>> { this };
            while (nodes.Count > 0)
            {
                var left = nodes.Peek().Left;
                if (!left.IsEmpty) 
                    nodes.Push(left);
                else
                {
                    var (_, root, right) = nodes.Pop();
                    yield return root;
                    if(!right.IsEmpty)
                        nodes.Push(right);
                }
            }
        }

        public IEnumerable<T> EnumRightRootLeft()
        {
            if (IsEmpty) yield break;
            if (IsLeaf) yield return Root;

            var nodes = new Stack<FTree<T>> { this };
            while (nodes.Count > 0)
            {
                var right = nodes.Peek().Right;
                if (!right.IsEmpty)
                    nodes.Push(right);
                else
                {
                    var (left, root, _) = nodes.Pop();
                    yield return root;
                    if (!left.IsEmpty)
                        nodes.Push(left);
                }
            }
        }

        public IEnumerable<T> EnumRootLeftRight()
        {
            if (IsEmpty) yield break;

            var nodes = new Stack<FTree<T>> { this };

            while (nodes.Count > 0)
            {
                var (left, value, right) = nodes.Pop();
                yield return value;
                if(!left.IsEmpty) nodes.Add(left);
                if(!right.IsEmpty) nodes.Add(right);
            }
        }

        public IEnumerable<T> EnumRootRightLeft()
        {
            if (IsEmpty) yield break;

            var nodes = new Stack<FTree<T>> { this };

            while (nodes.Count > 0)
            {
                var (left, value, right) = nodes.Pop();
                yield return value;
                if (!right.IsEmpty) nodes.Add(right);
                if (!left.IsEmpty) nodes.Add(left);
            }
        }

        public IEnumerable<T> EnumLeftRightRoot()
        {
            if (IsEmpty) yield break;
            if (IsLeaf) yield return Root;

            var nodes = new Stack<FTree<T>> { this };
            if(!Right.IsEmpty) nodes.Push(Right);
            if(!Left.IsEmpty) nodes.Push(Left);

            FTree<T> node = null;
            while (nodes.Count > 0)
            {

            }

            if (!Left.IsEmpty)
                foreach (var item in Left.EnumLeftRootRight())
                    yield return item;

            if (!Right.IsEmpty)
                foreach (var item in Right.EnumLeftRootRight())
                    yield return item;

            yield return Root;
        }

        public IEnumerable<T> EnumRightLeftRoot()
        {
            if (IsEmpty) yield break;
            if (IsLeaf) yield return Root;

            if (!Right.IsEmpty)
                foreach (var item in Right.EnumLeftRootRight())
                    yield return item;

            if (!Left.IsEmpty)
                foreach (var item in Left.EnumLeftRootRight())
                    yield return item;

            yield return Root;
        }

        public void Deconstruct(out FTree<T> left, out T root, out FTree<T> right)
        {
            left = Left;
            root = Root;
            right = Right;
        }
    }
}
