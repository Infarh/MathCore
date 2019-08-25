using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MathCore
{
    /// <summary>Группа объектов, поддерживающих интерфейс <see cref="T:System.IDisposable">освобождения ресурсов</see></summary>
    /// <typeparam name="T">Тип объектов, подерживающих интерфейс <see cref="T:System.IDisposable"/></typeparam>
    public class DisposableGroup<T> : IDisposable, IEnumerable<T>, IIndexableRead<int, T>
        where T : IDisposable
    {
        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Массив содержащихся объектов интерфейса <see cref="T:System.IDisposable"/></summary>
        private readonly T[] _Items;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Число элементов группы</summary>
        public int Count => _Items.Length;

        /// <summary>Массив элементов группы</summary>
        public T[] Items
        {
            [DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<T[]>() != null, "Свойство вернуло нулевую ссылку на массив элементов группы");
                return _Items;
            }
        }

        /// <summary>Элемент группы</summary>
        /// <param name="i">Номер элемента группы</param>
        /// <returns>Элемент группы с номером <paramref name="i"/></returns>
        public T this[int i] { [DebuggerStepThrough] get { return _Items[i]; } }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Группа <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="item"><typeparam name="T">Объект</typeparam> интерфейса <see cref="T:System.IDisposable"/></param>
        [DebuggerStepThrough]
        public DisposableGroup(params T[] item)
        {
            Contract.Requires(item != null, "Передана нуливая ссылка на массив элементов группы");
            _Items = item;
        }

        /// <summary>Группа <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></summary>
        /// <param name="items">Перечисление <typeparam name="T">объектов</typeparam> интерфейса <see cref="T:System.IDisposable"/></param>
        [DebuggerStepThrough]
        public DisposableGroup(IEnumerable<T> items)
            : this(items.ToArray())
        {
            Contract.Requires(items != null, "Передана нулевая ссылка на перечисление элементов группы");
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Освободить ресурсы группы</summary>
        [DebuggerStepThrough]
        public void Dispose() { _Items.Foreach(i => i.Dispose()); }

        /// <summary>Получить перечислитель элементов группы</summary>
        /// <returns>Перечислитель элементов группы</returns>
        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator<T>>() != null,
                "В результате работы метода получения итератора была возвращена нулевая ссылка на перечислитель элементов группы");
            return new List<T>(_Items).GetEnumerator();
        }


        /// <summary>Возвращает перечислитель, который осуществляет перебор элементов коллекции.</summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора элементов коллекции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        /* ------------------------------------------------------------------------------------------ */
    }
}
