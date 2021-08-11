using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        /// <summary>Число строк, пропускаемых в начале файла</summary>
        public int SkipRowsCount { get; init; }

        /// <summary>Число строк, пропускаемых в начале файла после строки заголовка (даже при её отсутствии)</summary>
        public int SkipRowsAfterHeaderCount { get; init; }

        /// <summary>Число строк, извлекаемых из области данных</summary>
        public int TakeRowsCount { get; init; }

        /// <summary>В процессе чтения данных будет учитываться наличие строки заголовка</summary>
        public bool ContainsHeader { get; init; }

        /// <summary>Символ-разделитель значений строки</summary>
        public char Separator { get; init; }

        /// <summary>Формат конца строки для расчёта положения в потоке</summary>
        public string EoL { get; init; }

        public CultureInfo Culture { get; init; }

        /// <summary>Информация о заголовке файла - имена колонок : номера колонок</summary>
        private IDictionary<string, int> Headers { get; init; }

        /// <summary>Инициализация нового экземпляра <see cref="CSVQuery"/></summary>
        /// <param name="ReaderFactory">Метод-фабрика объектов чтения данных</param>
        /// <param name="Separator">Символ-разделитель значений</param>
        public CSVQuery([NotNull] Func<TextReader> ReaderFactory, char Separator = ',')
            : this(ReaderFactory, 0, false, 0, Separator, -1, null, null, null) { }

        /// <summary>Инициализация нового экземпляра <see cref="CSVQuery"/></summary>
        private CSVQuery(
            [NotNull] Func<TextReader> ReaderFactory,
            int SkipRows,
            bool ContainsHeader,
            int SkipRowsAfterHeader,
            char ValuesSeparator,
            int TakeRows,
            IDictionary<string, int> Headers,
            string EoL,
            CultureInfo Culture
            )
        {
            _ReaderFactory = ReaderFactory ?? throw new ArgumentNullException(nameof(ReaderFactory));
            SkipRowsCount = SkipRows;
            this.ContainsHeader = ContainsHeader;
            SkipRowsAfterHeaderCount = SkipRowsAfterHeader;
            Separator = ValuesSeparator;
            TakeRowsCount = TakeRows;
            this.Headers = Headers;
            this.EoL = EoL;
            this.Culture = Culture;
        }

        private CSVQuery(in CSVQuery query)
        {
            _ReaderFactory = query._ReaderFactory;
            SkipRowsCount = query.SkipRowsCount;
            ContainsHeader = query.ContainsHeader;
            SkipRowsAfterHeaderCount = query.SkipRowsAfterHeaderCount;
            Separator = query.Separator;
            TakeRowsCount = query.TakeRowsCount;
            Headers = query.Headers;
            EoL = query.EoL;
            Culture = query.Culture;
        }

        /// <summary>Установить число пропускаемых строк в начале файла</summary>
        /// <param name="RowsCount">Количество пропускаемых строк в начале файла</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery SkipRowsBeforeHeader(int RowsCount) => new(this) { SkipRowsCount = RowsCount };

        /// <summary>Установить число строк, пропускаемых после заголовка</summary>
        /// <param name="RowsCount">Новое значение числа строк, пропускаемых после заголовка</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery SkipRowsAfterHeader(int RowsCount) => new(this) { SkipRowsAfterHeaderCount = RowsCount };

        /// <summary>Данные содержат заголовок?</summary>
        /// <param name="IsExist">Истина - заголовок будет учитываться при чтении</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery WithHeader(bool IsExist = true) => new(this) { ContainsHeader = IsExist };

        /// <summary>Установить символ-разделитель значений в строке</summary>
        /// <param name="NewSeparator">Новый символ-разделитель значений строки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery ValuesSeparator(char NewSeparator) => new(this) { Separator = NewSeparator };

        /// <summary>Установить число читаемых строк</summary>
        /// <param name="RowsCount">Число читаемых строк области данных (если -1, то читать всё)</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery TakeRows(int RowsCount) => new(this) { TakeRowsCount = RowsCount };

        /// <summary>Установить заголовок</summary>
        /// <param name="Header">Новый заголовок данных - словарь соответствия имени колонки и её индекса</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery Header(IDictionary<string, int> Header) => new(this) { Headers = { } };

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
        public CSVQuery MergeHeader(IDictionary<string, int> Header) => new(this) { Headers = Merge(Headers, Header) };

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
            if (Headers is not { Count: > 0 } header) return this;
            header = Merge(header);
            header.Remove(ColumnName);

            return new(this) { Headers = header };
        }

        /// <summary>Удалить колонку по указанному индексу</summary>
        /// <param name="ColumnIndex">Индекс удаляемой колонки</param>
        /// <returns>Модифицированных новый экземпляр <see cref="CSVQuery"/></returns>
        public CSVQuery RemoveColumn(int ColumnIndex)
        {
            if (Headers is not { Count: > 0 } header) return this;

            header = Merge(header);
            foreach (var column in Headers.Where(v => v.Value == ColumnIndex).Select(v => v.Key))
                header.Remove(column);
            return new(this) { Headers = header };
        }

        /// <summary>Установка символа конца строки для файла для корректного подсчёта положения в нём</summary>
        /// <param name="eol">Символ конца строки (по умолчанию \r\n)</param>
        public CSVQuery WithEoL(string eol) => new(this) { EoL = eol };

        /// <summary>Установка культуры преобразвоания строк в базовые типы данных</summary>
        /// <param name="culture">Устанавливаемая культура</param>
        public CSVQuery WithCulture(CultureInfo culture = null) => new(this) { Culture = culture };

        /// <summary>Считать заголовок данных</summary>
        /// <param name="MergeWithDefault"></param>
        /// <returns>Заголовок</returns>
        [NotNull]
        public IDictionary<string, int> GetHeader(bool MergeWithDefault = true)
        {
            using var reader = _ReaderFactory();

            var count = SkipRowsCount;
            while (count-- > 0)
                if (reader.ReadLine() is null) break;

            if (count > 0)
                throw new FormatException("Неожиданный конец потока");

            char[] separator = { Separator };
            var line = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                throw new FormatException("Пустая строка заголовка");

            var header = MergeWithDefault && Headers != null
                ? new SortedList<string, int>(Headers)
                : new SortedList<string, int>();

            var splitter = new Regex($@"(?<=(?:{Separator}|\n|^))(""(?:(?:"""")*[^""]*)*""|[^""{Separator}\n]*|(?:\n|$))", RegexOptions.Compiled);
            var headers = splitter
               .Matches(line)
               .Cast<Match>()
               .ToArray(m => m.Value is { Length: > 2 } v ? v.Trim('"') : m.Value);

            for (var i = 0; i < headers.Length; i++)
                header[headers[i]] = i;

            return header;
        }

        /// <summary>Получить объект-перечислитель строк данных</summary>
        /// <returns>Перечисление строк данных <see cref="CSVQueryRow"/></returns>
        public IEnumerator<CSVQueryRow> GetEnumerator()
        {
            using var reader = _ReaderFactory();

            if (reader is not StreamReader { CurrentEncoding: var encoding }) encoding = Encoding.Default;

            var eol = EoL is { Length: > 0 } s_eol ? encoding.GetByteCount(s_eol) : 0;

            var splitter = new Regex($@"(?<=(?:{Separator}|\n|^))(""(?:(?:"""")*[^""]*)*""|[^""{Separator}\n]*|(?:\n|$))", RegexOptions.Compiled);

            var position = 0L;

            var index = SkipRowsCount;
            while (index-- > 0)
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
                position += encoding.GetByteCount(line) + eol;
            }

            //char[] separator = { _ValuesSeparator };

            var header = Merge(Headers);
            if (ContainsHeader)
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
                position += encoding.GetByteCount(line) + eol;

                if (!string.IsNullOrWhiteSpace(line))
                {
                    //var headers = line.Split(separator);
                    var headers = splitter
                       .Matches(line)
                       .Cast<Match>()
                       .ToArray(m => m.Value is { Length: > 2 } v ? v.Trim('"') : m.Value);

                    for (var i = 0; i < headers.Length; i++)
                        header[headers[i]] = i;
                }
            }

            index = SkipRowsAfterHeaderCount;
            while (index-- > 0)
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
                position += encoding.GetByteCount(line) + eol;
            }

            index = 0;
            var culture = Culture ?? CultureInfo.CurrentCulture;
            do
            {
                var line = reader.ReadLine();
                if (line is null) yield break;
                var line_length = encoding.GetByteCount(line);

                if (string.IsNullOrWhiteSpace(line)) continue;
                var items = splitter.Matches(line).Cast<Match>().ToArray(m => m.Value is { Length: > 2 } v ? v.Trim('"') : m.Value);
                //var items = line.Split(separator);

                //for (var i = 0; i < items.Length; i++)
                //    if (items[i] is { Length: > 2 } item && item[0] == '"' && item[^1] == '"')
                //        items[i] = item.Trim('"');

                yield return new(line, index, items, header, position, (position += line_length + eol) - 1, culture);
                index++;
            }
            while (index != TakeRowsCount);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(object other, IEqualityComparer comparer) =>
            other is CSVQuery query
            && comparer.Equals(_ReaderFactory, query._ReaderFactory)
            && comparer.Equals(SkipRowsCount, query.SkipRowsCount)
            && comparer.Equals(ContainsHeader, query.ContainsHeader)
            && comparer.Equals(SkipRowsAfterHeaderCount, query.SkipRowsAfterHeaderCount)
            && comparer.Equals(Separator, query.Separator)
            && comparer.Equals(TakeRowsCount, query.TakeRowsCount)
            && comparer.Equals(Headers, query.Headers);

        public int GetHashCode(IEqualityComparer comparer)
        {
            unchecked
            {
                var hash_code = comparer.GetHashCode(_ReaderFactory);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(SkipRowsCount);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(ContainsHeader.GetHashCode());
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(SkipRowsAfterHeaderCount);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(Separator);
                hash_code = (hash_code * 397) ^ comparer.GetHashCode(TakeRowsCount);
                hash_code = (hash_code * 397) ^ (Headers != null ? comparer.GetHashCode(Headers) : 0);
                return hash_code;
            }
        }

        public override bool Equals(object obj) => obj is CSVQuery query && Equals(query);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code = _ReaderFactory.GetHashCode();
                hash_code = (hash_code * 397) ^ SkipRowsCount;
                hash_code = (hash_code * 397) ^ ContainsHeader.GetHashCode();
                hash_code = (hash_code * 397) ^ SkipRowsAfterHeaderCount;
                hash_code = (hash_code * 397) ^ Separator.GetHashCode();
                hash_code = (hash_code * 397) ^ TakeRowsCount;
                hash_code = (hash_code * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                return hash_code;
            }
        }

        public static bool operator ==(CSVQuery left, CSVQuery right) => left.Equals(right);

        public static bool operator !=(CSVQuery left, CSVQuery right) => !(left == right);

        public bool Equals(CSVQuery other) =>
            _ReaderFactory.Equals(other._ReaderFactory)
            && SkipRowsCount == other.SkipRowsCount
            && ContainsHeader == other.ContainsHeader
            && SkipRowsAfterHeaderCount == other.SkipRowsAfterHeaderCount
            && Separator == other.Separator
            && TakeRowsCount == other.TakeRowsCount
            && Equals(Headers, other.Headers);
    }
}