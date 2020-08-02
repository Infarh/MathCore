using System;
using System.Collections.Generic;
using MathCore.Annotations;

namespace MathCore.Trees
{
    public class TreeNode<T> : ITreeValuedNode<T>
    {
        [CanBeNull] private readonly Func<T, T> _ParentSelector;
        [CanBeNull] private readonly Func<T, IEnumerable<T>> _ChildsSelector;

        public T Value { get; }

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

        public TreeNode(
            [NotNull] T Value, 
            [CanBeNull] Func<T, T> ParentSelector, 
            [CanBeNull] Func<T, IEnumerable<T>> ChildsSelector)
        {
            this.Value = Value;
            _ParentSelector = ParentSelector;
            _ChildsSelector = ChildsSelector;
        }

        private TreeNode(
            [NotNull] T Value,
            [CanBeNull] Func<T, T> ParentSelector,
            [CanBeNull] Func<T, IEnumerable<T>> ChildsSelector,
            int Level)
            : this(Value, ParentSelector, ChildsSelector)
            => this.Level = Level;
    }
}
