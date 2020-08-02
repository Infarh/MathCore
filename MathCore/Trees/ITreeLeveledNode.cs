namespace MathCore.Trees
{
    /// <summary>Узел дерева, для которого уопределён его уровень</summary>
    /// <typeparam name="T">Тип узла дерева</typeparam>
    public interface ITreeLeveledNode<out T> : ITreeNode<T> where T : class, ITreeNode<T>
    {
        /// <summary>Уровень узла дерева</summary>
        int Level { get; }
    }
}