using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Graphs
{
    public class TreeListNode<TValue> : IList<TreeListNode<TValue>>, IList<TValue>
    {
        private TreeListNode<TValue> _Prev;
        private TreeListNode<TValue> _Next;

        private TreeListNode<TValue> _Child;

        public TreeListNode<TValue> Prev
        {
            get => _Prev;
            set
            {
                var last = _Prev;
                if (last is { })
                {
                    if (ReferenceEquals(last.Next, this)) last._Next = null;
                    else if (ReferenceEquals(last.Child, this)) last._Child = null;
                }
                _Prev = value;
            }
        }

        public TreeListNode<TValue> Next
        {
            get => _Next;
            set
            {
                var last = _Next;
                if (last is { }) last.Prev = null;
                _Next = value;
                value.Prev = this;
            }
        }

        public TreeListNode<TValue> Child
        {
            get => _Child;
            set
            {
                var last = _Child;
                if (last is { }) last.Prev = null;
                _Child = value;
                value.Prev = this;
            }
        }

        public TValue Value { get; set; }

        public int Length => this[n => n.Next].Count();

        /// <summary>
        /// Определяет индекс заданного элемента коллекции <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <returns>
        /// Индекс <paramref name="item"/> если он найден в списке; в противном случае его значение равно -1.
        /// </returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(TreeListNode<TValue> item)
        {
            var index = 0;
            bool find;
            for (var node = this; !(find = ReferenceEquals(node, item)) && node != null; node = node.Next)
                index++;
            return find ? index : -1;
        }

        /// <summary>Определяет индекс заданного элемента коллекции <see cref="T:System.Collections.Generic.IList`1"/>.</summary>
        /// <returns>Индекс <paramref name="item"/> если он найден в списке; в противном случае его значение равно -1.</returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public int IndexOf(TValue item)
        {
            var index = 0;
            var find = false;
            if (item is { })
                for (var node = this; node != null && !(find = Equals(item, node.Value)); node = node.Next)
                    index++;
            else
                for (var node = this; node != null && !(find = node.Value is null); node = node.Next)
                    index++;
            return find ? index : -1;
        }

        /// <summary>Вставляет элемент в список <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.</summary>
        /// <param name="index">Индекс (с нуля), по которому следует вставить параметр <paramref name="item"/>.</param><param name="item">Объект, вставляемый в <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> не является допустимым индексом в <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
        public void Insert(int index, TreeListNode<TValue> item)
        {
            if (index == 0 || item is null) return;
            var prev = this[index - 1] ?? Last;
            var next = prev.Next;
            prev.Next = item;
            item.Next = next;
        }

        /// <summary>Вставляет элемент в список <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.</summary>
        /// <param name="index">Индекс (с нуля), по которому следует вставить параметр <paramref name="item"/>.</param><param name="item">Объект, вставляемый в <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> не является допустимым индексом в <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
        public void Insert(int index, TValue item) => Insert(index, new TreeListNode<TValue>(item));

        /// <summary>Удаляет элемент <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.</summary>
        /// <param name="index">Индекс (с нуля) удаляемого элемента.</param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> не является допустимым индексом в <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
        public void RemoveAt(int index)
        {
            if (index == 0) return;
            var node = this[index];
            var prev = node.Prev;
            var next = node.Next;
            prev.Next = next;
        }

        void IList<TreeListNode<TValue>>.RemoveAt(int index) => RemoveAt(index);

        [NotNull]
        public TreeListNode<TValue> this[int i]
        {
            get
            {
                var node = this[n => n.Next].FirstOrDefault(n => i-- == 0);
                if (node is null) throw new IndexOutOfRangeException();
                return node;
            }
            set
            {
                i--;
                var node = this[n => n.Next].FirstOrDefault(n => i-- == 0);
                if (node is null) throw new IndexOutOfRangeException();
                var next = node.Next;
                node.Next = value;
                value.Next = next;
            }
        }

        /// <summary>Получает или задает элемент по указанному индексу.</summary>
        /// <returns>Элемент с указанным индексом.</returns>
        /// <param name="index">Индекс (с нуля) элемента, который необходимо получить или задать.</param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> не является допустимым индексом в <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Свойство задано, и объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
        TValue IList<TValue>.this[int index]
        {
            get => this[index].Value;
            set => this[index].Value = value;
        }

        [CanBeNull]
        public TreeListNode<TValue> this[[CanBeNull] params int[] index]
        {
            get
            {
                if (index is null || index.Length == 1 && index[0] == 0) return this;
                var result = this;
                for (var i = 0; result != null && i < index.Length; i++)
                {
                    result = result[index[i]];
                    if (i < index.Length - 1)
                        result = result?.Child;
                }
                return result;
            }
        }

        public bool IsFirst => _Prev is null || ReferenceEquals(_Prev.Child, this);
        public bool IsLast => _Next is null;
        public bool IsRoot => _Prev is null;
        public bool IsChild => !IsRoot && ReferenceEquals(_Prev.Child, this);

        public TreeListNode<TValue> Root => this[n => n.Prev].First(n => n.IsRoot);
        public TreeListNode<TValue> First => this[n => n.Prev].First(n => n.IsFirst);
        public TreeListNode<TValue> Last => this[n => n.Next].First(n => n.IsLast);

        public delegate TreeListNode<TValue> NodeSelector(TreeListNode<TValue> Prev, TreeListNode<TValue> Next, TreeListNode<TValue> Chield);

        [ItemNotNull]
        public IEnumerable<TreeListNode<TValue>> this[NodeSelector Selector]
        {
            get
            {
                for (var node = this; node != null; node = Selector(_Prev, _Next, _Child))
                    yield return node;
            }
        }

        [ItemNotNull]
        public IEnumerable<TreeListNode<TValue>> this[Func<TreeListNode<TValue>, TreeListNode<TValue>> Selector]
        {
            get
            {
                for (var node = this; node != null; node = Selector(node))
                    yield return node;
            }
        }

        public TreeListNode() { }
        public TreeListNode(TValue Value) => this.Value = Value;

        public TreeListNode([NotNull] IEnumerable<TValue> Collection)
        {
            var first = true;
            var node = this;
            foreach (var item in Collection)
                if (first)
                {
                    Value = item;
                    first = false;
                }
                else
                    node = node.Add(item);
        }

        [NotNull]
        public TreeListNode<TValue> Add(TValue Value)
        {
            var node = new TreeListNode<TValue>(Value);
            Add(node);
            return node;
        }

        public void Add(TreeListNode<TValue> Node)
        {
            var node = this;
            var next = node.Next;
            while (next != null)
            {
                node = next;
                next = node.Next;
            }
            node.Next = Node;
        }

        /// <summary>Добавляет элемент в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <param name="item">Объект, добавляемый в интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        void ICollection<TValue>.Add(TValue item) => Add(new TreeListNode<TValue>(item));

        /// <summary>Удаляет все элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public void Clear() => Next = null;

        public void ClearChild() => Child = null;

        public void ClearFull() { Clear(); ClearChild(); }

        /// <summary>Определяет, содержит ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.</summary>
        /// <returns>Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.</returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TValue item) => item is null
            ? ((IEnumerable<TreeListNode<TValue>>)this).Any(n => n is null)
            : ((IEnumerable<TreeListNode<TValue>>)this).Any(n => item.Equals(n.Value));

        /// <summary>
        /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
        /// </summary>
        /// <param name="array">Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.</param><param name="index">Значение индекса (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> меньше 0.</exception><exception cref="T:System.ArgumentException">Массив <paramref name="array"/> является многомерным.-или-
        ///                 Значение индекса массива <paramref name="index"/> больше или равно длине массива <paramref name="array"/>.-или-Количество элементов в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/> превышает размер доступного места, начиная с индекса <paramref name="index"/> и до конца массива назначения <paramref name="array"/>.-или-Тип <paramref name="TValue"/> не может быть автоматически приведен к типу массива назначения <paramref name="array"/>.</exception>
        public void CopyTo(TValue[] array, int index)
        {
            var node = this;
            for (var i = index; i < array.Length && node != null; i++, node = node.Next)
                array[i] = node.Value;
        }

        /// <summary>Удаляет первое вхождение указанного объекта из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">Объект, который необходимо удалить из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public bool Remove(TValue item)
        {
            if (ReferenceEquals(item, Value)) return false;
            var node = item is null
                ? ((IEnumerable<TreeListNode<TValue>>)this).FirstOrDefault(n => n.Value is null)
                : ((IEnumerable<TreeListNode<TValue>>)this).FirstOrDefault(n => item.Equals(n.Value));
            if (node is null) return false;
            if (node.IsChild) node.Prev.Child = node.Next; else node.Prev.Next = node.Next;
            return true;
        }

        /// <summary>Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <returns>Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<TValue>.Count => Length;

        /// <summary>Определяет, содержит ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.</summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
        /// </returns>
        /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(TreeListNode<TValue> item) => ((IEnumerable<TreeListNode<TValue>>)this).Any(n => ReferenceEquals(n, item));

        /// <summary>
        /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
        /// </summary>
        /// <param name="array">Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.</param><param name="index">Значение индекса (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="index"/> меньше 0.</exception><exception cref="T:System.ArgumentException">Массив <paramref name="array"/> является многомерным.-или-
        ///                 Значение индекса массива <paramref name="index"/> больше или равно длине массива <paramref name="array"/>.-или-Количество элементов в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/> превышает размер доступного места, начиная с индекса <paramref name="index"/> и до конца массива назначения <paramref name="array"/>.-или-Тип <paramref name="T"/> не может быть автоматически приведен к типу массива назначения <paramref name="array"/>.</exception>
        public void CopyTo(TreeListNode<TValue>[] array, int index)
        {
            var node = this;
            for (var i = index; i < array.Length && node != null; i++, node = node.Next)
                array[i] = node;
        }

        /// <summary>Удаляет первое вхождение указанного объекта из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <returns>
        /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">Объект, который необходимо удалить из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
        public bool Remove(TreeListNode<TValue> item)
        {
            if (ReferenceEquals(item, this)) return false;
            var node = ((IEnumerable<TreeListNode<TValue>>)this).FirstOrDefault(n => ReferenceEquals(n, item));
            if (node is null) return false;
            if (node.IsChild) node.Prev.Child = node.Next; else node.Prev.Next = node.Next; return true;
        }

        /// <summary>Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.</summary>
        /// <returns>Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        int ICollection<TreeListNode<TValue>>.Count => Length;

        /// <summary>Получает значение, указывающее, доступен ли интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> только для чтения.</summary>
        /// <returns>Значение true, если интерфейс <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения, в противном случае — значение false.</returns>
        public bool IsReadOnly => false;

        public void Add([NotNull] IEnumerable<TValue> collection) => collection.Aggregate(this, (current, value) => current.Add(value));

        public void AddChild(TreeListNode<TValue> Node) { if (Child is null) Child = Node; else Child.Add(Node); }
        [NotNull]
        public TreeListNode<TValue> AddChild(TValue value)
        {
            var node = new TreeListNode<TValue>(value);
            AddChild(node);
            return node;
        }

        public void AddChild([NotNull] IEnumerable<TValue> collection) => collection.Aggregate<TValue, TreeListNode<TValue>>(null, (current, item) => current is null ? AddChild(item) : current.Add(item));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TreeListNode<TValue>> GetEnumerator()
        {
            for (var node = this; node != null; node = node.Next)
                yield return node;
        }

        /// <summary>Возвращает перечислитель, выполняющий перебор элементов в коллекции</summary>
        /// <returns>Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.</returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => ((IEnumerable<TreeListNode<TValue>>)this).Select(n => n.Value).GetEnumerator();

        public override string ToString() => $"{Value}{(_Child is null ? "" : $"{{{_Child}}}")}{(_Next is null ? "" : $",{_Next}")}";

        public static implicit operator TValue([NotNull] TreeListNode<TValue> Node) => Node.Value;
        [NotNull]
        public static implicit operator TreeListNode<TValue>(TValue Value) => new TreeListNode<TValue>(Value);
    }
}