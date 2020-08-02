namespace MathCore.Trees
{
    /// <summary>Узел дерева со значением</summary>
    /// <typeparam name="T">Тип значения узла</typeparam>
    public interface ITreeValuedNode<out T> : ITreeNode<ITreeValuedNode<T>, T>, ITreeLeveledNode<ITreeValuedNode<T>> { }
}