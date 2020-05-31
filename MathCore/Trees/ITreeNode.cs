using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.Trees
{
    /// <summary>Элемент двусвязного дерева</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    public interface ITreeNode<out T> where T : class, ITreeNode<T>
    {
        /// <summary>Родительский узел</summary>
        [CanBeNull]
        T Parent { get; }

        /// <summary>Дочерние узлы</summary>
        [CanBeNull]
        IEnumerable<T> Childs { get; }
    }

    /// <summary>Элемент двусвязного дерева</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    /// <typeparam name="TItem">Тип значения</typeparam>
    public interface ITreeNode<out T, out TItem> : ITreeNode<T> where T : class, ITreeNode<T, TItem>
    {
        [NotNull] TItem Value { get; }
    }
}