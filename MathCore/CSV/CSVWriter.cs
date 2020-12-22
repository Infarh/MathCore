using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.CSV
{
    /// <summary>Объект для записи данных в формате CSV</summary>
    /// <typeparam name="T">ТИп записываемых элементов данных</typeparam>
    public readonly struct CSVWriter<T>
    {
        /// <summary>Перечисление записываемых элементов данных</summary>
        private readonly IEnumerable<T> _Items;

        /// <summary>Символ-разделитель значений в строке</summary>
        private readonly char _Separator;

        /// <summary>Словарь соответствия имени колонки методу извлечения значения и индексу колонки</summary>
        private readonly IDictionary<string, (Func<T, object> Selector, int Index)> _Selectors;

        /// <summary>Требуется ли записывать заголовок?</summary>
        private readonly bool _WriteHeaders;

        /// <summary>Получить упорядоченное перечисление колонок</summary>
        /// <param name="Columns">Колонки</param>
        /// <returns>Упорядоченное по индексу, затем по имени перечисление колонок</returns>
        [NotNull]
        private static IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>> GetOrdered(
            [CanBeNull] IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>> Columns) =>
            Columns?.OrderBy(s => s.Value.Index).ThenBy(s => s.Key)
               ?? Enumerable.Empty<KeyValuePair<string, (Func<T, object> Selector, int Index)>>();

        /// <summary>Колонки данных</summary>
        [NotNull] public IEnumerable<string> Headers => GetOrdered(_Selectors).Select(s => s.Key);

        /// <summary>Инициализация нового экземпляра <see cref="CSVWriter{T}"/></summary>
        /// <param name="items">Перечисление элементов, значения которых надо записать в формате CSV</param>
        /// <param name="Separator">Символ-разделитель значений в строках</param>
        public CSVWriter([NotNull] IEnumerable<T> items, char Separator)
            : this(
                items,
                Separator,
                WriteHeaders: true,
                Selectors: null)
        { }

        /// <summary>Инициализация нового экземпляра <see cref="CSVWriter{T}"/></summary>
        private CSVWriter(
            [NotNull] IEnumerable<T> items,
            char Separator,
            bool WriteHeaders,
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> Selectors
            )
        {
            _Items = items ?? throw new ArgumentNullException(nameof(items));
            _Separator = Separator;
            _WriteHeaders = WriteHeaders;
            _Selectors = Selectors;
        }

        /// <summary>Создать заголовок</summary>
        /// <returns>Словарь колонок данных файла</returns>
        [NotNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> CreateHeaders()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.CanRead).Select((p, i) => (p, i));
            var selectors = new Dictionary<string, (Func<T, object> Selector, int Index)>();

            var item = Expression.Parameter(type, "item");
            foreach (var (property, index) in properties)
            {
                var selector_expr = property.PropertyType.IsValueType
                    ? item.GetProperty(property).ConvertTo<object>()
                    : (Expression)item.GetProperty(property);

                selectors[property.Name] = (selector_expr.CompileTo<Func<T, object>>(item), index);
            }

            return selectors;
        }

        /// <summary>Изменить символ-разделитель значений в строках</summary>
        /// <param name="separator">Новый символ-разделитель</param>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> Separator(char separator) =>
            new(
                _Items,
                separator,
                _WriteHeaders,
                _Selectors
            );

        /// <summary>Записывать ли заголовок в начало файла?</summary>
        /// <param name="write">Истина, если заголовок требуется записать</param>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> WriteHeader(bool write = true) =>
            new(
                _Items,
                _Separator,
                write,
                _Selectors
            );

        /// <summary>Добавить колонки по умолчанию - на основе имён свойств <typeparamref name="T"/></summary>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> AddDefaultHeaders() =>
            new(
                _Items,
                _Separator,
                _WriteHeaders,
                MergeSelectors(_Selectors, CreateHeaders())
                );

        /// <summary>Объединить колонки</summary>
        /// <param name="SourceColumns">Исходное описание столбцов</param>
        /// <param name="NewColumns">Добавляемое описание столбцов</param>
        /// <returns>Новый словарь колонок</returns>
        [CanBeNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> MergeSelectors(
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> SourceColumns,
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> NewColumns = null)
        {
            if (SourceColumns is null)
                return NewColumns;

            var result = new Dictionary<string, (Func<T, object> Selector, int Index)>(SourceColumns);
            if (NewColumns is null) return result;

            foreach (var (header, selector) in NewColumns)
                result[header] = selector;

            return result;
        }

        /// <summary>Добавить новую колонку</summary>
        /// <param name="SourceColumns">Исходный набор колонок</param>
        /// <param name="NewColumnName">Имя добавляемой колонки</param>
        /// <param name="NewColumnValueSelector">Метод извлечения значения для новой колонки</param>
        /// <returns>Новый словарь колонок</returns>
        [NotNull]
        private static IDictionary<string, (Func<T, object> Selector, int Index)> MergeSelectors(
            [CanBeNull] IDictionary<string, (Func<T, object> Selector, int Index)> SourceColumns,
            string NewColumnName, Func<T, object> NewColumnValueSelector)
        {
            var result = SourceColumns is null
                ? new Dictionary<string, (Func<T, object> Selector, int Index)>()
                : new Dictionary<string, (Func<T, object> Selector, int Index)>(SourceColumns);

            var index = result.Count == 0 ? 0 : result.Max(s => s.Value.Index) + 1;
            result[NewColumnName] = (NewColumnValueSelector, index);

            return result;
        }

        /// <summary>Добавить новую колонку</summary>
        /// <param name="NewColumnName">Имя добавляемой колонки</param>
        /// <param name="NewColumnValueSelector">Метод извлечения значения для новой колонки</param>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> AddColumn(string NewColumnName, Func<T, object> NewColumnValueSelector) =>
            new(
                _Items,
                _Separator,
                _WriteHeaders,
                MergeSelectors(_Selectors, NewColumnName, NewColumnValueSelector)
            );

        /// <summary>Удалить колонку</summary>
        /// <param name="Name">Имя удаляемой колонки</param>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> RemoveColumn(string Name)
        {
            var columns = _Selectors;
            if (columns?.ContainsKey(Name) == true)
            {
                columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
                columns.Remove(Name);
            }
            return new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                columns
            );
        }

        /// <summary>Удалить колонку</summary>
        /// <param name="index">Индекс удаляемой колонки</param>
        /// <returns>Модифицированный <see cref="CSVWriter{T}"/></returns>
        public CSVWriter<T> RemoveColumn(int index)
        {
            var columns = _Selectors;
            if (columns != null)
            {
                var i = 0;
                foreach (var (header, _) in columns)
                {
                    if (i == index)
                    {
                        columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
                        columns.Remove(header);
                        break;
                    }
                    i++;
                }
            }
            return new CSVWriter<T>(
                _Items,
                _Separator,
                _WriteHeaders,
                columns
            );
        }

        #region Write

        /// <summary>Записать данные в файл по указанному пути</summary>
        /// <param name="FileName">Путь к файлу данных</param>
        /// <param name="encoding">Кодировка (если не указана, то используется <see cref="Encoding.UTF8"/>)</param>
        public void WriteTo([NotNull] string FileName, [CanBeNull] Encoding encoding = null)
        {
            using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            WriteTo(file_stream, encoding);
        }

        /// <summary>Выполнить асинхронную запись в файл по указанному пути</summary>
        /// <param name="FileName">Путь к файлу данных</param>
        /// <param name="encoding">Кодировка (если не указана, то используется <see cref="Encoding.UTF8"/>)</param>
        /// <param name="Cancel">Признак отмены асинхронной операции</param>
        /// <returns>Задача асинхронной записи данных</returns>
        public async Task WriteToAsync([NotNull] string FileName, [CanBeNull] Encoding encoding = null, CancellationToken Cancel = default)
        {
            using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            await WriteToAsync(file_stream, encoding, Cancel).ConfigureAwait(false);
        }

        /// <summary>Записать данные в указанный файл</summary>
        /// <param name="File">Файл, в который требуется выполнить запись данных</param>
        public void WriteTo([NotNull] FileInfo File) => WriteTo(File.CreateText());

        /// <summary>Выполнить асинхронную запись в указанный файл</summary>
        /// <param name="File">Файл, в который требуется выполнить запись данных</param>
        /// <param name="Cancel">Признак отмены асинхронной операции</param>
        /// <returns>Задача асинхронной записи данных</returns>
        public async Task WriteToAsync([NotNull] FileInfo File, CancellationToken Cancel = default) =>
            await WriteToAsync(File.CreateText(), Cancel).ConfigureAwait(false);

        /// <summary>Записать данные в поток</summary>
        /// <param name="stream">Поток данных, в который осуществляется запись</param>
        /// <param name="encoding">Кодировка (если не указана, то используется <see cref="Encoding.UTF8"/>)</param>
        public void WriteTo([NotNull] Stream stream, [CanBeNull] Encoding encoding = null) =>
            WriteTo(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true));

        /// <summary>Выполнить асинхронную запись данных в поток</summary>
        /// <param name="stream">Поток данных, в который осуществляется запись</param>
        /// <param name="encoding">Кодировка (если не указана, то используется <see cref="Encoding.UTF8"/>)</param>
        /// <param name="Cancel">Признак отмены асинхронной операции</param>
        /// <returns>Задача асинхронной записи данных</returns>
        public async Task WriteToAsync([NotNull] Stream stream, [CanBeNull] Encoding encoding = null, CancellationToken Cancel = default) =>
            await WriteToAsync(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true), Cancel).ConfigureAwait(false);

        /// <summary>Записать данные в объект записи текстовых данных</summary>
        /// <param name="writer">Объект записи текстовых данных</param>
        public void WriteTo([NotNull] TextWriter writer)
        {
            var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

            var values = selectors_key.Select(s => s.Key).ToArray();
            var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

            var separator = new string(_Separator, 1);
            writer.WriteLine(string.Join(separator, values));

            foreach (var item in _Items)
            {
                for (var i = 0; i < values.Length; i++) 
                    values[i] = Convert.ToString(selectors[i](item));
                writer.WriteLine(string.Join(separator, values));
            }
        }

        /// <summary>Асинхронно записать данные в объект записи текстовых данных</summary>
        /// <param name="writer">Объект записи текстовых данных</param>
        /// <param name="Cancel">Признак отмены асинхронной операции</param>
        /// <returns>Задача асинхронной записи данных</returns>
        public async Task WriteToAsync([NotNull] TextWriter writer, CancellationToken Cancel = default)
        {
            Cancel.ThrowIfCancellationRequested();
            var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

            var values = selectors_key.Select(s => s.Key).ToArray();
            var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

            var separator = new string(_Separator, 1);
            Cancel.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(string.Join(separator, values)).ConfigureAwait(false);

            Cancel.ThrowIfCancellationRequested();
            foreach (var item in _Items)
            {
                Cancel.ThrowIfCancellationRequested();
                for (var i = 0; i < values.Length; i++)
                    values[i] = Convert.ToString(selectors[i](item));

                Cancel.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(string.Join(separator, values)).ConfigureAwait(false);
            }
        }

        #endregion
    }
}