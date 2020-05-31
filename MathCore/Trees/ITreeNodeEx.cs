using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore.Trees
{
    /// <summary>Методы-расширения интерфейса элемента двусвязного дерева</summary>
    public static class ITreeNodeEx
    {
        [NotNull]
        public static TreeNode<TValue> AsTreeNode<TValue>(
            [NotNull] this TValue Value,
            Func<TValue, IEnumerable<TValue>> ChildsSelector, 
            [CanBeNull] Func<TValue, TValue> ParentSelector = null) =>
            new TreeNode<TValue>(Value, ParentSelector, ChildsSelector);

        ///// <summary>Тип обхода узлов дерева</summary>
        //[Serializable]
        //public enum OrderWalkType : byte
        //{
        //    /// <summary>(К-Л-П) Сначала текущий узел в уровне, затем все дочерние узлы текущего, затем следующий узел уровня</summary>
        //    CurrentChildNext,

        //    /// <summary>(К-П-Л) Сначала текущий узел в уровне, затем все узлы текущего уровня, затем дочерний узел</summary>
        //    CurrentNextChild,

        //    /// <summary>(Л-К-П) Сначала дочерние узлы текущего узла, затем текущий узел, потом все оставшиеся узлы текущего уровня</summary>
        //    ChildCurrentNext,

        //    /// <summary>(П-К-Л) Сначала все остальные узлы текущего уровня, затем текущий узел, потом все дочерние узлы текущего узла</summary>
        //    NextCurrentChild,

        //    /// <summary>(Л-П-К) Сначала все дочерние узлы текущего узла, затем все узлы текущего уровня, в конце текущий узел</summary>
        //    ChildNextCurrent,

        //    /// <summary>(П-Л-К) Сначала все узлы текущего уровня, затем все дочерние узлы текущего узла, В конце сам текущий узел</summary>
        //    NextChildCurrent
        //}

        ///// <summary>Представление узла дерева</summary>
        ///// <typeparam name="T">Тип значения узла</typeparam>
        //public readonly struct TreeLevelNode<T> : IEquatable<TreeLevelNode<T>> where T : class, ITreeNode<T>
        //{
        //    /// <summary>Значение узла</summary>
        //    public T Node { get; }

        //    /// <summary>Уровень узла в дереве</summary>
        //    public int Level { get; }

        //    /// <summary>Инициализация нового экземпляра <see cref="TreeLevelNode{T}"/></summary>
        //    /// <param name="Node">Значение узла</param>
        //    /// <param name="Level">Уровень узла</param>
        //    public TreeLevelNode(T Node, int Level)
        //    {
        //        this.Node = Node;
        //        this.Level = Level;
        //    }

        //    /// <inheritdoc />
        //    public bool Equals(TreeLevelNode<T> other) => EqualityComparer<T>.Default.Equals(Node, other.Node) && Level == other.Level;

        //    /// <inheritdoc />
        //    public override bool Equals(object obj) => obj is TreeLevelNode<T> other && Equals(other);

        //    /// <inheritdoc />
        //    public override int GetHashCode()
        //    {
        //        unchecked
        //        {
        //            return (EqualityComparer<T>.Default.GetHashCode(Node) * 397) ^ Level;
        //        }
        //    }

        //    /// <summary>Оператор определения равенства между двумя экземплярам <see cref="TreeLevelNode{T}"/></summary>
        //    /// <returns>Истина, если свойства экземпляров равны</returns>
        //    public static bool operator ==(TreeLevelNode<T> left, TreeLevelNode<T> right) => left.Equals(right);

        //    /// <summary>Оператор определения неравенства между двумя экземплярам <see cref="TreeLevelNode{T}"/></summary>
        //    /// <returns>Истина, если хотя бы одно свойство экземпляров отличается</returns>
        //    public static bool operator !=(TreeLevelNode<T> left, TreeLevelNode<T> right) => !left.Equals(right);
        //}

        //private static void DebugWrite([NotNull] string str, [NotNull] params object[] obj) => Console.Title = string.Format(str, obj);

        /// <summary>Определение корня дерева</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Node">Объект с интерфейсом элемента дерева</param>
        /// <returns>Корневой объект дерева объектов</returns>
        [NotNull]
        public static T GetRootNode<T>(this T Node) where T : class, ITreeNode<T> => Node.EnumerateParents().Last();

        ///// <summary>Обход элементов поддерева начиная с текущего в порядке: текущий, дочерний, следующий по уровню</summary>
        ///// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        ///// <param name="Node">Объект с интерфейсом элемента дерева</param>
        ///// <param name="WalkType">Тип обхода</param>
        ///// <param name="Level">Уровень дерева</param>
        ///// <returns>Последовательность элементов дерева</returns>
        //public static IEnumerable<TreeLevelNode<T>> OrderWalk<T>(
        //    this T Node,
        //    OrderWalkType WalkType = OrderWalkType.ChildCurrentNext,
        //    int Level = 0)
        //    where T : class, ITreeNode<T>
        //{
        //    var stack = new Stack<TreeLevelNode<T>>();
        //    stack.Push(new TreeLevelNode<T>(Node, Level));

        //    void Push(T t, int l)
        //    {
        //        if (t is null) return;
        //        stack.Push(new TreeLevelNode<T>(t, l));
        //    }

        //    switch(WalkType)
        //    {
        //        case OrderWalkType.CurrentChildNext:
        //            do
        //            {
        //                var node = stack.Pop();
        //                yield return node;

        //                var node_level = node.Level;
        //                Push(node.Node.Next, node_level);
        //                Push(node.Node.Child, node_level + 1);
        //            } while(stack.Count > 0);
        //            break;
        //        case OrderWalkType.CurrentNextChild:
        //            do
        //            {
        //                var node = stack.Pop();
        //                yield return node;

        //                var node_level = node.Level;
        //                Push(node.Node.Child, node_level + 1);
        //                Push(node.Node.Next, node_level);
        //            } while(stack.Count > 0);
        //            break;
        //        case OrderWalkType.ChildCurrentNext:
        //            throw new NotImplementedException();
        //        //break;
        //        case OrderWalkType.NextCurrentChild:
        //            throw new NotImplementedException();
        //        //break;
        //        case OrderWalkType.ChildNextCurrent:
        //            throw new NotImplementedException();
        //        //break;
        //        case OrderWalkType.NextChildCurrent:
        //            throw new NotImplementedException();
        //        //break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(WalkType));
        //    }
        //}

        /// <summary>Получить все родительские элементы</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Node">Объект с интерфейсом элемента дерева</param>
        /// <returns>Массив элементов родительских узлов дерева</returns>
        [NotNull]
        public static T[] GetParents<T>([NotNull] this T Node) where T : class, ITreeNode<T> =>
            Node is null
                ? throw new ArgumentNullException(nameof(Node))
                : Node.Parent is null
                    ? Array.Empty<T>()
                    : new Stack<T>(Node.EnumerateParents()).ToArray();

        [ItemNotNull]
        public static IEnumerable<T> EnumerateParents<T>([NotNull] this T Node) where T : class, ITreeNode<T>
        {
            for (var parent = Node.Parent; parent != null; parent = parent.Parent)
                yield return parent;
        }

        ///// <summary>Получить все элементы дочернего уровня поддерева</summary>
        ///// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        ///// <param name="Node">Объект с интерфейсом элемента дерева</param>
        ///// <returns>Массив элементов текущего уровня дерева</returns>
        //[NotNull]
        //public static T[] GetLevelNodes<T>(this T Node) where T : class, ITreeNode<T>
        //{
        //    var Nodes = new List<T>();
        //    if(Node.Parent != null)
        //    {
        //        Node = Node.Parent.Child;
        //        do
        //        {
        //            Nodes.Add(Node);
        //            Node = Node.Next;
        //        } while(Node != null);
        //    }
        //    else
        //    {
        //        var Node = Node.Prev;
        //        while(Node != null)
        //        {
        //            Nodes.Add(Node);
        //            Node = Node.Prev;
        //        }
        //        if(Nodes.Count != 0)
        //            Nodes.Reverse();
        //        Node = Node;
        //        do
        //        {
        //            Nodes.Add(Node);
        //            Node = Node.Next;
        //        } while(Node != null);
        //    }
        //    return Nodes.ToArray();
        //}

        ///// <summary>Получить все дочерние элементы поддерева</summary>
        ///// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        ///// <param name="Node">Объект с интерфейсом элемента дерева</param>
        ///// <returns>Массив элементов дочерних узлов</returns>
        //[NotNull]
        //public static T[] GetChilds<T>(this T Node) where T : class, ITreeNode<T>
        //{
        //    Node = Node.Child;
        //    var Nodes = new List<T>();
        //    do
        //    {
        //        Nodes.Add(Node);
        //        Node = Node.Next;
        //    } while(Node != null);
        //    return Nodes.ToArray();
        //}

        //public static int ChildsCount<T>(this T Node) where T : class, ITreeNode<T>
        //{

        //}
    }
}