using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Values;

namespace MathCore.Trees
{
    public class Tree<T> : ITree<T>
    {
        /* ------------------------------------------------------------------------------------------ */

        private readonly LazyValue<List<Tree<T>>> _SubTreeList =
                    new LazyValue<List<Tree<T>>>(() => new List<Tree<T>>());

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Число элементов</summary>
        public int Count => _SubTreeList.Initialized ? _SubTreeList.Value.Count : 0;

        /// <summary>Индексатор объекта</summary><param name="index">Индекс</param>
        public Tree<T> this[int index]
        {
            get => _SubTreeList.Initialized ? _SubTreeList.Value[index] : null;
            set
            {
                if(_SubTreeList.Initialized)
                    _SubTreeList.Value[index] = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>Индексатор объекта</summary><param name="index">Индекс</param>
        ITree<T> IIndexable<int, ITree<T>>.this[int index] { get => this[index]; set => this[index] = value as Tree<T>; }

        /// <summary>Индексатор объекта только для записи</summary><param name="index">Индекс</param>
        ITree<T> IIndexableWrite<int, ITree<T>>.this[int index] { set => this[index] = value as Tree<T>; }

        /// <summary>Индексатор объекта только для чтения</summary>
        /// <param name="index">Индекс</param>
        ITree<T> IIndexableRead<int, ITree<T>>.this[int index] => this[index];

        public T Value { get; set; }

        public int Depth
        {
            get
            {
                var depth = new MaxValue();

                var queue = new Queue<ITree<T>>();
                queue.Enqueue(this);
                var d = 1;

                while(queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    if(current.Count == 0)
                        d--;
                    else
                    {
                        depth.AddValue(d);
                        current.Foreach(queue.Enqueue);
                        d++;
                    }
                }

                return (int)depth;
            }
        }

        /* ------------------------------------------------------------------------------------------ */


        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции.</summary>
        /// <returns>
        /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        IEnumerator<ITree<T>> IEnumerable<ITree<T>>.GetEnumerator()
        {
            return _SubTreeList.Initialized
                ? _SubTreeList.Value.Cast<ITree<T>>().GetEnumerator()
                : new List<ITree<T>>().GetEnumerator();
        }

        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ITree<T>>)this).GetEnumerator();

        /* ------------------------------------------------------------------------------------------ */
    }
}