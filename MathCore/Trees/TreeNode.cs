using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.Trees
{
    public class TreeNode<T> : ITreeNode<TreeNode<T>, T>
    {
        [NotNull] private readonly Func<T, T> _ParentSelector;
        [CanBeNull] private readonly Func<T, IEnumerable<T>> _ChildsSelector;

        public T Value { get; }

        public TreeNode<T> Parent
        {
            get
            {
                var parent_item = _ParentSelector.Invoke(Value);
                return parent_item is null ? null : new TreeNode<T>(parent_item, _ParentSelector, _ChildsSelector);
            }
        }

        public IEnumerable<TreeNode<T>> Childs
        {
            get
            {
                var childs = _ChildsSelector?.Invoke(Value);
                if(childs is null) yield break;
                foreach (var child in childs)
                    yield return new TreeNode<T>(child, _ParentSelector, _ChildsSelector);
            }
        }

        public TreeNode([NotNull] T Value, [NotNull] Func<T, T> ParentSelector, [CanBeNull] Func<T, IEnumerable<T>> ChildsSelector)
        {
            this.Value = Value;
            _ParentSelector = ParentSelector;
            _ChildsSelector = ChildsSelector;
        }
    }
}
