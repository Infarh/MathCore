namespace MathCore.Trees
{
    /// <summary>Элемент двусвязного дерева</summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeItem<T> where T : class, ITreeItem<T>
    {
        /// <summary>Родительский узел</summary>
        T Parent { get; set; }

        /// <summary>Дочерний узел</summary>
        T Child { get; set; }

        /// <summary>Предыдущий узел уровня</summary>
        T Prev { get; set; }

        /// <summary>Следующий узел дерева</summary>
        T Next { get; set; }
    }
}