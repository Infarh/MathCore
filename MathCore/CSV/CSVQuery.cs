using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.CSV
{
    /// <summary>Объект, осуществляющий извлечение данных из файла в формате CSV</summary>
    public readonly struct CSVQuery : IEnumerable<CSVQueryRow>, IEquatable<CSVQuery>, IStructuralEquatable
    {
        /// <summary>Метод-фабрика объектов чтения данных</summary>
        [NotNull]
        private readonly Func<TextReader> _ReaderFactory;

        /// <summary>Число пропускаемых строк в начале файла</summary>
        private readonly int _SkipRows;

        /// <summary>Содержит ли файл строку заголовка?</summary>
        private readonly bool _ContainsHeader;

        /// <summary>Число пропускаемых строк после строки заголовка</summary>
        private readonly int _SkipRowsAfterHeader;

        /// <summary>Символ-разделитель значений в строке</summary>
        private readonly char _ValuesSeparator;

        /// <summary>Число считываемых строк</summary>
        private readonly int _TakeRows;

        /// <summary>Информация о заголовке файла - имена колонок : номера колонок</summary>
        private readonly IDictionary<string, int> _Header;

        /// <summary>Число строк, пропускаемых в начале файла</summary>
        public int SkipRowsCount => _SkipRows;

        /// <summary>Число строк, пропускаемых в начале файла после строки заголовка (даже при её отсутствии)</summary>
        public int SkipRowsAfterHeaderCount => _SkipRowsAfterHeader;

        /// <summary>Число строк, извлекаемых из области данных</summary>
        public int TakeRowsCount => _TakeRows;

        /// <summary>В процессе чтения данных будет учитываться наличие строки заголовка</summary>
        public bool ContainsHeader => _ContainsHeader;

        /// <summary>Символ-разделитель значений строки</summary>
        public char Separator => _ValuesSeparator;

        /// <summary>Инициализация нового экземпляра <see cref="CSVQuery"/></summary>
        /// <param name="ReaderFactory">Метод-фабрика объектов чтения данных</param>
        /// <param name="Separator">Символ-разделитель значений</param>
        public CSVQuery([NotNull] Func<TextReader> ReaderFactory, char Separator = ',')
            : this(ReaderFactory, 0, false, 0, Separator, -1, null) { }

        /// <summary>Инициализация нового экземпляра <see cref="CSVQuery"/></summary>
        private CSVQuery(
            [NotNull] Func<TextReader> ReaderFactory,
            int SkipRows,
            bool ContainsHeader,
            int SkipRowsAfterHeader,
            char ValuesSeparator,
            int TakeRows,
            IDictionary<string, int> Header
            )
        {
            _ReaderFactory = ReaderFactory ?? throw new ArgumentNullException(nameof(ReaderFactory));
            _SkipRows = SkipRows;
            _ContainsHeader = ContainsHeader;
            _SkipRowsAfterHeader = SkipRowsAfterHeader;
            _ValuesSeparator = ValuesSeparator;
            _TakeRows = TakeRows;
            _Header = Header;
        }

        /// <summary>Установить число пропускаемых строк в начале файла</summary>
        /// <param name="RowsCount">Количество пропускаемых строк в начале файла</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery Skip(int RowsCount) => new CSVQuery(
            _ReaderFactory,
            RowsCount,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows,
            _Header
            );

        /// <summary>Установить число строк, пропускаемых после заголовка</summary>
        /// <param name="RowsCount">Новое значение числа строк, пропускаемых после заголовка</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery SkipAfterHeader(int RowsCount) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            _ContainsHeader,
            RowsCount,
            _ValuesSeparator,
            _TakeRows,
            _Header
        );

        /// <summary>Данные содержат заголовок?</summary>
        /// <param name="IsExist">Истина - заголовок будет учитываться при чтении</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery WithHeader(bool IsExist = true) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            IsExist,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows,
            _Header
        );

        /// <summary>Установить символ-разделитель значений в строке</summary>
        /// <param name="NewSeparator">Новый символ-разделитель значений строки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery ValuesSeparator(char NewSeparator) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            NewSeparator,
            _TakeRows,
            _Header
        );

        /// <summary>Установить число читаемых строк</summary>
        /// <param name="RowsCount">Число читаемых строк области данных (если -1, то читать всё)</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery TakeRows(int RowsCount) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            RowsCount,
            _Header
        );

        /// <summary>Установить заголовок</summary>
        /// <param name="Header">Новый заголовок данных - словарь соответствия имени колонки и её индекса</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery Header(IDictionary<string, int> Header) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows,
            Header
        );

        /// <summary>Объединить словари заголовков</summary>
        /// <param name="Source">Исходный словарь значений</param>
        /// <param name="Values">Добавляемые данные</param>
        /// <returns>Новый словарь значений, содержащий в себе исходные значения и добавленные к ним новые</returns>
        [NotNull]
        private static IDictionary<string, int> Merge([CanBeNull] IDictionary<string, int> Source, [CanBeNull] IDictionary<string, int> Values = null)
        {
            var result = Source is null || Source.Count == 0
                ? new SortedList<string, int>()
                : new SortedList<string, int>(Source);

            if (Values is null || Values.Count == 0) return result;

            foreach (var (key, value) in Values)
                result[key] = value;

            return result;
        }

        /// <summary>Добавить в заголовок набор колонок</summary>
        /// <param name="Header">Добавляемые в заголовок колонки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery MergeHeader(IDictionary<string, int> Header) => new CSVQuery(
            _ReaderFactory,
            _SkipRows,
            _ContainsHeader,
            _SkipRowsAfterHeader,
            _ValuesSeparator,
            _TakeRows,
            Merge(_Header, Header)
        );

        /// <summary>Добавить колонку в считываемый заголовок</summary>
        /// <param name="AliasName">Новый псевдоним колонки</param>
        /// <param name="Index">Индекс колонки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery AddColumn([NotNull] string AliasName, int Index) => MergeHeader(new Dictionary<string, int> { { AliasName, Index } });

        /// <summary>Добавить колонки в считываемый заголовок</summary>
        /// <param name="Columns">Новые псевдонимы колонок</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery AddColumns([NotNull] params (string AliasName, int Index)[] Columns) => MergeHeader(Columns.ToDictionary(c => c.AliasName, c => c.Index));

        /// <summary>Удалить колонку по указанному имени</summary>
        /// <param name="ColumnName">Имя удаляемой колонки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery RemoveColumn(string ColumnName)
        {
            var header = _Header;
            if (header != null)
            {
                header = new Dictionary<string, int>(header);
                header.Remove(ColumnName);
            }

            return new CSVQuery(
                _ReaderFactory,
                _SkipRows,
                _ContainsHeader,
                _SkipRowsAfterHeader,
                _ValuesSeparator,
                _TakeRows,
                header
            );
        }

        /// <summary>Удалить колонку по указанному индексу</summary>
        /// <param name="ColumnIndex">Индекс удаляемой колонки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery RemoveColumn(int ColumnIndex)
        {
            var header = _Header;
            if (header != null)
            {
                header = new Dictionary<string, int>(header);
                foreach (var column in _Header.Where(v => v.Value == ColumnIndex).Select(v => v.Key))
                    header.Remove(column);
            }

            return new CSVQuery(
                _ReaderFactory,
                _SkipRows,
                _ContainsHeader,
                _SkipRowsAfterHeader,
                _ValuesSeparator,
                _TakeRows,
                header
            );
        }

        /// <summary>Считать заголовок данных</summary>
        /// <param name="MergeWithDefault"></param>
        /// <returns>Заголовок</returns>
        [NotNull]
        public IDictionary<string, int> GetHeader(bool MergeWithDefault = true)
        {
            using var reader = _ReaderFactory();

            var count = _SkipRows;
            while (count-- > 0)
                if (reader.ReadLine() is null) break;

            if (count > 0)
                throw new FormatException("Неожиданный конец потока");

            char[] separator = { _ValuesSeparator };
            var header_line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(header_line))
                throw new FormatException("Пустая строка заголовка");

            var header = MergeWithDefault && _Header != null
                ? new SortedList<string, int>(_Header)
                : new SortedList<string, int>();

            var headers = header_line.Split(separator);
            for (var i = 0; i < headers.Length; i++)
                header[headers[i]] = i;

            return header;
        }

        /// <summary>Получить объект-перечислитель строк данных</summary>
        /// <returns>Перечисление строк данных <see cref="CSVQueryRow"/></returns>
        public IEnumerator<CSVQueryRow> GetEnumerator()
        {
            using var reader = _ReaderFactory();

            var index = _SkipRows;
            while (index-- > 0)
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
            }

            char[] separator = { _ValuesSeparator };

            var header = Merge(_Header);
            if (_ContainsHeader)
            {
                var header_line = reader.ReadLine();
                if (header_line is null) yield break;

                if (!string.IsNullOrWhiteSpace(header_line))
                {
                    var headers = header_line.Split(separator);
                    header = new SortedList<string, int>();
                    for (var i = 0; i < headers.Length; i++)
                        header[headers[i]] = i;
                }
            }

            index = _SkipRowsAfterHeader;
            while (index-- > 0)
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
            }

            index = 0;
            do
            {
                var line = reader.ReadLine();
                if (line is null) yield break;

                if (string.IsNullOrWhiteSpace(line)) continue;
                var items = line.Split(separator);
                yield return new CSVQueryRow(index, items, header);
                index++;
            }
            while (index != _TakeRows);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(object other, IEqualityComparer comparer) =>
            other is CSVQuery query
            && comparer.Equals(_ReaderFactory, query._ReaderFactory)
            && comparer.Equals(_SkipRows, query._SkipRows)
            && comparer.Equals(_ContainsHeader, query._ContainsHeader)
            && comparer.Equals(_SkipRowsAfterHeader, query._SkipRowsAfterHeader)
            && comparer.Equals(_ValuesSeparator, query._ValuesSeparator)
            && comparer.Equals(_TakeRows, query._TakeRows)
            && comparer.Equals(_Header, query._Header);

        public int GetHashCode(IEqualityComparer comparer)
        {
            unchecked
            {
                var hash_code = comparer.GetHashCode(_ReaderFactory);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(_SkipRows);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(_ContainsHeader.GetHashCode());
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(_SkipRowsAfterHeader);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(_ValuesSeparator);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(_TakeRows);
                hash_code = (hash_code * 397) ^ (_Header != null ? comparer.GetHashCode(_Header) : 0);
                return hash_code;
            }
        }

        public override bool Equals(object obj) => obj is CSVQuery query && Equals(query);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code = _ReaderFactory.GetHashCode();
                hash_code = (hash_code * 397) ^ _SkipRows;
                hash_code = (hash_code * 397) ^ _ContainsHeader.GetHashCode();
                hash_code = (hash_code * 397) ^ _SkipRowsAfterHeader;
                hash_code = (hash_code * 397) ^ _ValuesSeparator.GetHashCode();
                hash_code = (hash_code * 397) ^ _TakeRows;
                hash_code = (hash_code * 397) ^ (_Header != null ? _Header.GetHashCode() : 0);
                return hash_code;
            }
        }

        public static bool operator ==(CSVQuery left, CSVQuery right) => left.Equals(right);

        public static bool operator !=(CSVQuery left, CSVQuery right) => !(left == right);

        public bool Equals(CSVQuery other) =>
            _ReaderFactory.Equals(other._ReaderFactory)
            && _SkipRows == other._SkipRows
            && _ContainsHeader == other._ContainsHeader
            && _SkipRowsAfterHeader == other._SkipRowsAfterHeader
            && _ValuesSeparator == other._ValuesSeparator
            && _TakeRows == other._TakeRows
            && Equals(_Header, other._Header);
    }
}