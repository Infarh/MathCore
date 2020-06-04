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
            [CanBeNull] Func<TValue, IEnumerable<TValue>> ChildsSelector,
            [CanBeNull] Func<TValue, TValue> ParentSelector = null) =>
            new TreeNode<TValue>(Value, ParentSelector, ChildsSelector);

        /// <summary>Определение корня дерева</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Node">Объект с интерфейсом элемента дерева</param>
        /// <returns>Корневой объект дерева объектов</returns>
        [NotNull]
        public static T GetRootNode<T>([NotNull] this T Node) where T : class, ITreeNode<T> => Node.EnumerateParents().Last();

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
                    : Node.EnumerateParents().ToArray();

        /// <summary>Перечислить всех предков текущего узла</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <param name="Node">Текущий узел</param>
        /// <returns>Перечисление предков</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<T> EnumerateParents<T>([NotNull] this T Node) where T : class, ITreeNode<T>
        {
            for (var parent = Node.Parent; parent != null; parent = parent.Parent)
                yield return parent;
        }

        /// <summary>Перечислить значения всех узлов-предков текущего узла</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <typeparam name="TValue">Тип значения узла</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <returns>Перечисление значений родительских узлов текущего узла дерева</returns>
        [NotNull]
        public static IEnumerable<TValue> EnumerateParentValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node) where T : class, ITreeNode<T, TValue> =>
            Node.EnumerateParents().Select(n => n.Value);

        /// <summary>Получить массив значений всех узлов-предков текущего узла</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <typeparam name="TValue">Тип значения узла</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <returns>Массив значений родительских узлов текущего узла дерева</returns>
        [NotNull]
        public static TValue[] GetParentValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node) where T : class, ITreeNode<T, TValue> =>
            Node.EnumerateParentValues().ToArray();

        /// <summary>Перечисление всех дочерних узлов дерева</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <param name="ParentFirst">Формировать в перечислении родительский узел первым</param>
        /// <returns>Перечисление дочерних узлов поддерева</returns>
        public static IEnumerable<T> EnumerateChilds<T>(this T Node, bool ParentFirst = true) where T : class, ITreeNode<T>
        {
            if (ParentFirst)
            {
                yield return Node;
                foreach (var node in Node.Childs.SelectMany(node => node.EnumerateThisWithChilds()))
                    yield return node;
            }
            else
            {
                foreach (var node in Node.Childs.SelectMany(node => node.EnumerateThisWithChilds(false)))
                    yield return node;
                yield return Node;
            }
        }

        /// <summary>Перечисление всех дочерних узлов дерева вместе с текущим узлом</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <param name="CurrentFirst">Формировать в перечислении родительский узел первым</param>
        /// <returns>Перечисление дочерних узлов поддерева</returns>
        public static IEnumerable<T> EnumerateThisWithChilds<T>(this T Node, bool CurrentFirst = true) where T : class, ITreeNode<T>
        {
            if (CurrentFirst)
            {
                yield return Node;
                foreach (var node in Node.Childs.SelectMany(node => node.EnumerateThisWithChilds()))
                    yield return node;
            }
            else
            {
                foreach (var node in Node.Childs.SelectMany(node => node.EnumerateThisWithChilds(false)))
                    yield return node;
                yield return Node;
            }
        }

        /// <summary>Перечисление значений всех дочерних узлов дерева</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <typeparam name="TValue">Тип значения узла</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <param name="ParentFirst">Формировать в перечислении родительский узел первым</param>
        /// <returns>Перечисление значений дочерних узлов поддерева</returns>
        [NotNull]
        public static IEnumerable<TValue> EnumerateChildValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node, bool ParentFirst = true) where T : class, ITreeNode<T, TValue> => 
            Node.EnumerateChilds(ParentFirst).Select(node => node.Value);

        /// <summary>Перечисление значений всех дочерних узлов дерева вместе с текущим узлом</summary>
        /// <typeparam name="T">Тип узла дерева</typeparam>
        /// <typeparam name="TValue">Тип значения узла</typeparam>
        /// <param name="Node">Текущий узел дерева</param>
        /// <param name="CurrentFirst">Формировать в перечислении родительский узел первым</param>
        /// <returns>Перечисление значений дочерних узлов поддерева</returns>
        [NotNull]
        public static IEnumerable<TValue> EnumerateThisWithChildValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node, bool CurrentFirst = true) where T : class, ITreeNode<T, TValue> =>
            Node.EnumerateThisWithChilds(CurrentFirst).Select(node => node.Value);
    }
}