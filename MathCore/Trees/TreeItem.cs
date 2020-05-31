using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Annotations;

namespace MathCore.Trees
{
    public class TreeItem<T> : ITreeItem<TreeItem<T>, T>
    {
        private readonly Func<T, T> _ParentSelector;
        private readonly Func<T, IEnumerable<T>> _ChildsSelector;

        [NotNull] public T Item { get; }

        public TreeItem<T> Parent
        {
            get
            {
                var parent_item = _ParentSelector.Invoke(Item);
                return parent_item is null ? null : new TreeItem<T>(parent_item, _ParentSelector, _ChildsSelector);
            }
        }

        public IEnumerable<TreeItem<T>> Childs
        {
            get
            {
                var childs = _ChildsSelector?.Invoke(Item);
                if(childs is null) yield break;
                foreach (var child in childs)
                    yield return new TreeItem<T>(child, _ParentSelector, _ChildsSelector);
            }
        }


        public TreeItem([NotNull] T Item, Func<T, T> ParentSelector, Func<T, IEnumerable<T>> ChildsSelector)
        {
            this.Item = Item;
            _ParentSelector = ParentSelector;
            _ChildsSelector = ChildsSelector;
        }
    }
}
