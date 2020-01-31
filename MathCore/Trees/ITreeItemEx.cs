using System;
using System.Collections.Generic;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.Trees
{
    /// <summary>Методы-расширения интерфейса элемента двусвязного дерева</summary>
    public static class ITreeItemEx
    {
        /// <summary>Тип обхода узлов дерева</summary>
        [Serializable]
        public enum OrderWalkType : byte
        {
            /// <summary>(К-Л-П) Сначала текущий узел в уровне, затем все дочерние узлы текущего, затем следующий узел уровня</summary>
            CurrentChildNext,

            /// <summary>(К-П-Л) Сначала текущий узел в уровне, затем все узлы текущего уровня, затем дочерний узел</summary>
            CurrentNextChild,

            /// <summary>(Л-К-П) Сначала дочерние узлы текущего узла, затем текущий узел, потом все оставшиеся узлы текущего уровня</summary>
            ChildCurrentNext,

            /// <summary>(П-К-Л) Сначала все остальные узлы текущего уровня, затем текущий узел, потом все дочерние узлы текущего узла</summary>
            NextCurrentChild,

            /// <summary>(Л-П-К) Сначала все дочерние узлы текущего узла, затем все узлы текущего уровня, в конце текущий узел</summary>
            ChildNextCurrent,

            /// <summary>(П-Л-К) Сначала все узлы текущего уровня, затем все дочерние узлы текущего узла, В конце сам текущий узел</summary>
            NextChildCurrent
        }

        /// <summary>Представление узла дерева</summary>
        /// <typeparam name="T">Тип значения узла</typeparam>
        public readonly struct TreeLevelItem<T> : IEquatable<TreeLevelItem<T>> where T : class, ITreeItem<T>
        {
            /// <summary>Значение узла</summary>
            public T Item { get; }

            /// <summary>Уровень узла в дереве</summary>
            public int Level { get; }

            /// <summary>Инициализация нового экземпляра <see cref="TreeLevelItem{T}"/></summary>
            /// <param name="Item">Значение узла</param>
            /// <param name="Level">Уровень узла</param>
            public TreeLevelItem(T Item, int Level)
            {
                this.Item = Item;
                this.Level = Level;
            }

            /// <inheritdoc />
            public bool Equals(TreeLevelItem<T> other) => EqualityComparer<T>.Default.Equals(Item, other.Item) && Level == other.Level;

            /// <inheritdoc />
            public override bool Equals(object obj) => obj is TreeLevelItem<T> other && Equals(other);

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return (EqualityComparer<T>.Default.GetHashCode(Item) * 397) ^ Level;
                }
            }

            /// <summary>Оператор определения равенства между двумя экземплярам <see cref="TreeLevelItem{T}"/></summary>
            /// <returns>Истина, если свойства экземпляров равны</returns>
            public static bool operator ==(TreeLevelItem<T> left, TreeLevelItem<T> right) => left.Equals(right);

            /// <summary>Оператор определения неравенства между двумя экземплярам <see cref="TreeLevelItem{T}"/></summary>
            /// <returns>Истина, если хотя бы одно свойство экземпляров отличается</returns>
            public static bool operator !=(TreeLevelItem<T> left, TreeLevelItem<T> right) => !left.Equals(right);
        }

        //private static void DebugWrite([NotNull] string str, [NotNull] params object[] obj) => Console.Title = string.Format(str, obj);

        /// <summary>Определение корня дерева</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Item">Объект с интерфейсом элемента дерева</param>
        /// <returns>Корневой объект дерева объектов</returns>
        [NotNull]
        public static T GetRootItem<T>(this T Item) where T : class, ITreeItem<T>
        {
            while(Item.Parent != null) Item = Item.Parent;
            return Item;
        }

        /// <summary>Обход элементов поддерева начиная с текущего в порядке: текущий, дочерний, следующий по уровню</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Item">Объект с интерфейсом элемента дерева</param>
        /// <param name="WalkType">Тип обхода</param>
        /// <param name="Level">Уровень дерева</param>
        /// <returns>Последовательность элементов дерева</returns>
        public static IEnumerable<TreeLevelItem<T>> OrderWalk<T>(
            this T Item,
            OrderWalkType WalkType = OrderWalkType.ChildCurrentNext,
            int Level = 0)
            where T : class, ITreeItem<T>
        {
            var stack = new Stack<TreeLevelItem<T>>();
            stack.Push(new TreeLevelItem<T>(Item, Level));

            void Push(T t, int l)
            {
                if (t is null) return;
                stack.Push(new TreeLevelItem<T>(t, l));
            }

            switch(WalkType)
            {
                case OrderWalkType.CurrentChildNext:
                    do
                    {
                        var node = stack.Pop();
                        yield return node;

                        var node_level = node.Level;
                        Push(node.Item.Next, node_level);
                        Push(node.Item.Child, node_level + 1);
                    } while(stack.Count > 0);
                    break;
                case OrderWalkType.CurrentNextChild:
                    do
                    {
                        var node = stack.Pop();
                        yield return node;

                        var node_level = node.Level;
                        Push(node.Item.Child, node_level + 1);
                        Push(node.Item.Next, node_level);
                    } while(stack.Count > 0);
                    break;
                case OrderWalkType.ChildCurrentNext:
                    throw new NotImplementedException();
                //break;
                case OrderWalkType.NextCurrentChild:
                    throw new NotImplementedException();
                //break;
                case OrderWalkType.ChildNextCurrent:
                    throw new NotImplementedException();
                //break;
                case OrderWalkType.NextChildCurrent:
                    throw new NotImplementedException();
                //break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(WalkType));
            }
        }

        /// <summary>Получить все родительские элементы</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Item">Объект с интерфейсом элемента дерева</param>
        /// <returns>Массив элементов родительских узлов дерева</returns>
        [NotNull]
        public static T[] GetParents<T>(this T Item) where T : class, ITreeItem<T>
        {
            var stack = new Stack<T>();

            while(Item != null)
            {
                var parent = Item.Parent;
                if(parent != null)
                    stack.Push(parent);
                Item = parent;
            }

            return stack.ToArray();
        }

        /// <summary>Получить все элементы дочернего уровня поддерева</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Item">Объект с интерфейсом элемента дерева</param>
        /// <returns>Массив элементов текущего уровня дерева</returns>
        [NotNull]
        public static T[] GetLevelItems<T>(this T Item) where T : class, ITreeItem<T>
        {
            var items = new List<T>();
            if(Item.Parent != null)
            {
                Item = Item.Parent.Child;
                do
                {
                    items.Add(Item);
                    Item = Item.Next;
                } while(Item != null);
            }
            else
            {
                var item = Item.Prev;
                while(item != null)
                {
                    items.Add(item);
                    item = item.Prev;
                }
                if(items.Count != 0)
                    items.Reverse();
                item = Item;
                do
                {
                    items.Add(item);
                    item = item.Next;
                } while(item != null);
            }
            return items.ToArray();
        }

        /// <summary>Получить все дочерние элементы поддерева</summary>
        /// <typeparam name="T">Тип элемента, являющегося классом и определяющего интерфейс элемента дерева</typeparam>
        /// <param name="Item">Объект с интерфейсом элемента дерева</param>
        /// <returns>Массив элементов дочерних узлов</returns>
        [NotNull]
        public static T[] GetChilds<T>(this T Item) where T : class, ITreeItem<T>
        {
            Item = Item.Child;
            var items = new List<T>();
            do
            {
                items.Add(Item);
                Item = Item.Next;
            } while(Item != null);
            return items.ToArray();
        }

        //public static int ChildsCount<T>(this T Item) where T : class, ITreeItem<T>
        //{

        //}
    }
}