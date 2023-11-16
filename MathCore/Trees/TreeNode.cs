#nullable enable
using MathCore.Annotations;

namespace MathCore.Trees;

public class TreeNode<T>(T Value, Func<T, T>? ParentSelector, Func<T, IEnumerable<T>>? ChildsSelector) : ITreeValuedNode<T>
{
    private readonly Func<T, T>? _ParentSelector = ParentSelector;
    private readonly Func<T, IEnumerable<T>>? _ChildsSelector = ChildsSelector;

    public T Value { get; } = Value;

    public int Level { get; }

    public ITreeValuedNode<T> Parent
    {
        get
        {
            if (_ParentSelector is null) return null;
            var parent_item = _ParentSelector.Invoke(Value);
            return parent_item is null ? null : new TreeNode<T>(parent_item, _ParentSelector, _ChildsSelector, Level - 1);
        }
    }

    public IEnumerable<ITreeValuedNode<T>> Childs
    {
        get
        {
            var childs = _ChildsSelector?.Invoke(Value);
            if(childs is null) yield break;
            var child_level = Level + 1;
            foreach (var child in childs)
                yield return new TreeNode<T>(child, _ParentSelector, _ChildsSelector, child_level);
        }
    }

    private TreeNode(T Value, Func<T, T>? ParentSelector, Func<T, IEnumerable<T>>? ChildsSelector, int Level)
        : this(Value, ParentSelector, ChildsSelector)
        => this.Level = Level;
}