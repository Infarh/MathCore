using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.Trees
{
    /// <summary>Элемент двусвязного дерева</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    public interface ITreeItem<out T> where T : class, ITreeItem<T>
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
    public interface ITreeItem<out T, out TItem> : ITreeItem<T> where T : class, ITreeItem<T, TItem>
    {
        [NotNull] TItem Item { get; }
    }
}