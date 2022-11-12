using System;
using System.Collections.Generic;
using System.Linq;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Trees;

/// <summary>Методы-расширения интерфейса элемента двусвязного дерева</summary>
public static class ITreeNodeEx
{
    /// <summary>Представить значение как узел дерева</summary>
    /// <typeparam name="T">Тип значения дерева</typeparam>
    /// <param name="Value">Значение узла дерева</param>
    /// <param name="ChildsSelector">Метод извлечения дочерних значений текущего узла</param>
    /// <param name="ParentSelector">Метод извлечения значения родительского узла</param>
    /// <returns>Текущий узел дерева</returns>
    [NotNull]
    public static TreeNode<T> AsTreeNode<T>(
        [NotNull] this T Value,
        [CanBeNull] Func<T, IEnumerable<T>> ChildsSelector,
        [CanBeNull] Func<T, T> ParentSelector = null) =>
        new(Value, ParentSelector, ChildsSelector);

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
    public static IEnumerable<T> EnumerateParents<T>([NotNull] this T Node) 
        where T : class, ITreeNode<T>
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
    public static IEnumerable<TValue> EnumerateParentValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node)
        where T : class, ITreeNode<T, TValue> =>
        Node.EnumerateParents().Select(n => n.Value);

    /// <summary>Получить массив значений всех узлов-предков текущего узла</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    /// <typeparam name="TValue">Тип значения узла</typeparam>
    /// <param name="Node">Текущий узел дерева</param>
    /// <returns>Массив значений родительских узлов текущего узла дерева</returns>
    [NotNull]
    public static TValue[] GetParentValues<T, TValue>([NotNull] this ITreeNode<T, TValue> Node) 
        where T : class, ITreeNode<T, TValue> =>
        Node.EnumerateParentValues().ToArray();

    /// <summary>Перечисление всех дочерних узлов дерева</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    /// <param name="Node">Текущий узел дерева</param>
    /// <param name="ProcessChilds">Метод, определяющий необходимость обработки дочерних узлов</param>
    /// <param name="ParentFirst">Формировать в перечислении родительский узел первым</param>
    /// <returns>Перечисление дочерних узлов поддерева</returns>
    public static IEnumerable<T> EnumerateChilds<T>(
        this T Node, 
        [CanBeNull] Func<T, bool> ProcessChilds = null, 
        bool ParentFirst = true) 
        where T : class, ITreeNode<T>
    {
        if (ParentFirst)
        {
            yield return Node;
            if (ProcessChilds?.Invoke(Node) == false) yield break;
            foreach (var node in Node.Childs)
                foreach (var child in node.EnumerateChildsWithRoot(ProcessChilds, true))
                    yield return child;
        }
        else
        {
            if (ProcessChilds?.Invoke(Node) != false)
                foreach (var node in Node.Childs)
                    foreach (var child in node.EnumerateChildsWithRoot(ProcessChilds, false))
                        yield return child;
            yield return Node;
        }
    }

    /// <summary>Перечисление всех дочерних узлов дерева вместе с текущим узлом</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    /// <param name="Node">Текущий узел дерева</param>
    /// <param name="ProcessChilds">Метод, определяющий необходимость обработки дочерних узлов</param>
    /// <param name="CurrentFirst">Формировать в перечислении родительский узел первым</param>
    /// <returns>Перечисление дочерних узлов поддерева</returns>
    public static IEnumerable<T> EnumerateChildsWithRoot<T>(
        this T Node, 
        [CanBeNull] Func<T, bool> ProcessChilds = null, 
        bool CurrentFirst = true) 
        where T : class, ITreeNode<T>
    {
        if (CurrentFirst)
        {
            yield return Node;
            if (ProcessChilds?.Invoke(Node) == false) yield break;
            foreach (var node in Node.Childs)
                foreach (var child in node.EnumerateChildsWithRoot(ProcessChilds, true))
                    yield return child;
        }
        else
        {
            if (ProcessChilds?.Invoke(Node) != false)
                foreach (var node in Node.Childs)
                    foreach (var child in node.EnumerateChildsWithRoot(ProcessChilds, true))
                        yield return child;
            yield return Node;
        }
    }

    /// <summary>Перечисление значений всех дочерних узлов дерева</summary>
    /// <typeparam name="T">Тип значения узла</typeparam>
    /// <param name="Node">Текущий узел дерева</param>
    /// <param name="ProcessChilds">Метод, определяющий необходимость обработки дочерних узлов</param>
    /// <param name="ParentFirst">Формировать в перечислении родительский узел первым</param>
    /// <returns>Перечисление значений дочерних узлов поддерева</returns>
    [NotNull]
    public static IEnumerable<T> EnumerateChildValues<T>(
        [NotNull] this ITreeValuedNode<T> Node,
        [CanBeNull] Func<ITreeValuedNode<T>, bool> ProcessChilds = null,
        bool ParentFirst = true) =>
        Node.EnumerateChilds(ProcessChilds, ParentFirst).Select(child => child.Value);

    /// <summary>Перечисление значений всех дочерних узлов дерева вместе с текущим узлом</summary>
    /// <typeparam name="T">Тип значения узла</typeparam>
    /// <param name="Node">Текущий узел дерева</param>
    /// <param name="ProcessChilds">Метод, определяющий необходимость обработки дочерних узлов</param>
    /// <param name="CurrentFirst">Формировать в перечислении родительский узел первым</param>
    /// <returns>Перечисление значений дочерних узлов поддерева</returns>
    [NotNull]
    public static IEnumerable<T> EnumerateChildValuesWithRoot<T>(
        [NotNull] this ITreeValuedNode<T> Node,
        [CanBeNull] Func<ITreeValuedNode<T>, bool> ProcessChilds = null,
        bool CurrentFirst = true) =>
        Node.EnumerateChildsWithRoot(ProcessChilds, CurrentFirst).Select(node => node.Value);
}