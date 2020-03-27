using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.CSV
{
    /// <summary>Строка данных</summary>
    public readonly struct CSVQueryRow
    {
        /// <summary>Массив элементов данных строки</summary>
        private readonly string[] _Items;

        /// <summary>Словарь столбцов</summary>
        private readonly IDictionary<string, int> _Header;

        /// <summary>Число элементов данных строки</summary>
        public int ItemsCount => _Items.Length;

        /// <summary>Индекс (номер) строки (отсчёт от 0)</summary>
        public int Index { get; }

        /// <summary>Заголовки строки</summary>
        [NotNull] public IReadOnlyDictionary<string, int> Headers => new ReadOnlyDictionary<string, int>(_Header);

        /// <summary>Строковое значение по указанному индексу колонки в строке данных</summary>
        /// <param name="ValueIndex">Индекс колонки</param>
        /// <returns>Строковое значение</returns>
        public ref string this[int ValueIndex] => ref _Items[ValueIndex];

        /// <summary>Строковое значение по имени колонки</summary>
        /// <param name="ValueName">Имя колонки в массиве данных</param>
        /// <returns>Строковое значение</returns>
        public ref string this[[NotNull] string ValueName] => ref _Items[_Header[ValueName]];

        /// <summary>Инициализация нового экземпляра строки данных <see cref="CSVQueryRow"/></summary>
        /// <param name="Index">Индекс строки</param>
        /// <param name="Items">Массив строковых значений</param>
        /// <param name="Header">Словарь заголовков</param>
        public CSVQueryRow(int Index, string[] Items, IDictionary<string, int> Header)
        {
            this.Index = Index;
            _Items = Items;
            _Header = Header;
        }

        /// <summary>Представление значения в указанном типе</summary>
        /// <typeparam name="T">Требуемый тип значения</typeparam>
        /// <param name="ValueIndex">Индекс колонки</param>
        /// <returns>Представление значения в указанном типе</returns>
        public T ValueAs<T>(int ValueIndex) => (T)Convert.ChangeType(this[ValueIndex], typeof(T));

        /// <summary>Представление значения в указанном типе</summary>
        /// <typeparam name="T">Требуемый тип значения</typeparam>
        /// <param name="ValueName">Имя столбца</param>
        /// <returns>Представление значения в указанном типе</returns>
        public T ValueAs<T>([NotNull] string ValueName) => ValueAs<T>(_Header[ValueName]);
    }
}