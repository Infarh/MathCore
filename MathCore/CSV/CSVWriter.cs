using System.Linq.Expressions;
using System.Text;

using MathCore.Extensions.Expressions;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.CSV;

/// <summary>
/// Построитель для записи данных в формате CSV
/// </summary>
/// <typeparam name="T">Тип элементов для записи</typeparam>
/// <remarks>
/// Структура предоставляет fluent API для конфигурации записи данных в CSV-формат.
/// Поддерживает:
/// - Автоматическое создание колонок на основе свойств типа T
/// - Добавление/удаление колонок с пользовательскими селекторами
/// - Запись в файл, поток или TextWriter
/// - Асинхронную запись с поддержкой отмены операции
/// 
/// Примеры использования:
/// var people = new[] { new Person { Id = 1, Name = "Alice" }, ... };
/// var csv = people.AsCSV()
///     .AddDefaultHeaders()
///     .WriteTo("people.csv");
/// </remarks>
public readonly struct CSVWriter<T>
{
    /// <summary>Перечисление записываемых элементов данных</summary>
    private readonly IEnumerable<T> _Items;

    /// <summary>Символ-разделитель значений в строке</summary>
    private readonly char _Separator;

    /// <summary>Словарь соответствия имени колонки методу извлечения значения и индексу колонки</summary>
    private readonly IDictionary<string, (Func<T, object> Selector, int Index)>? _Selectors;

    /// <summary>Требуется ли записывать заголовок в начало файла?</summary>
    private readonly bool _WriteHeaders;

    /// <summary>
    /// Получить упорядоченное перечисление колонок по индексу, затем по имени
    /// </summary>
    /// <param name="Columns">Колонки для сортировки</param>
    /// <returns>Отсортированное перечисление</returns>
    private static IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>> GetOrdered(
        IEnumerable<KeyValuePair<string, (Func<T, object> Selector, int Index)>>? Columns) =>
        Columns?.OrderBy(s => s.Value.Index).ThenBy(s => s.Key)
        ?? Enumerable.Empty<KeyValuePair<string, (Func<T, object> Selector, int Index)>>();

    /// <summary>
    /// Получить перечисление имён колонок в порядке записи
    /// </summary>
    /// <value>Упорядоченное перечисление имён колонок</value>
    public IEnumerable<string> Headers => GetOrdered(_Selectors).Select(s => s.Key);

    /// <summary>
    /// Инициализация нового построителя CSV с указанными элементами и разделителем
    /// </summary>
    /// <param name="items">Перечисление элементов для записи</param>
    /// <param name="Separator">Символ-разделитель (по умолчанию ',')</param>
    /// <example>
    /// <![CDATA[
    /// var items = new[] { new Product { Id = 1, Name = "Item" } };
    /// var writer = new CSVWriter<Product>(items, ',');
    /// ]]>
    /// </example>
    public CSVWriter(IEnumerable<T> items, char Separator)
        : this(
            items,
            Separator,
            WriteHeaders: true,
            Selectors: null)
    { }

    /// <summary>Инициализация внутреннего построителя с полным набором параметров</summary>
    private CSVWriter(
        IEnumerable<T> items,
        char Separator,
        bool WriteHeaders,
        IDictionary<string, (Func<T, object> Selector, int Index)>? Selectors
    )
    {
        _Items = items.NotNull();
        _Separator = Separator;
        _WriteHeaders = WriteHeaders;
        _Selectors = Selectors;
    }

    /// <summary>
    /// Автоматически создать селекторы колонок на основе публичных свойств типа T
    /// </summary>
    /// <returns>Словарь селекторов</returns>
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

    /// <summary>
    /// Изменить символ-разделитель значений в строках
    /// </summary>
    /// <param name="separator">Новый символ-разделитель</param>
    /// <returns>Модифицированный построитель</returns>
    /// <example>
    /// <![CDATA[
    /// var writer = new CSVWriter<T>(items, ',')
    ///     .Separator(';');  // использовать точку с запятой в качестве разделителя
    /// ]]>
    /// </example>
    public CSVWriter<T> Separator(char separator) =>
        new(
            _Items,
            separator,
            _WriteHeaders,
            _Selectors
        );

    /// <summary>
    /// Указать, требуется ли записывать заголовок в начало файла
    /// </summary>
    /// <param name="write">true = записать заголовок; false = только данные</param>
    /// <returns>Модифицированный построитель</returns>
    public CSVWriter<T> WriteHeader(bool write = true) =>
        new(
            _Items,
            _Separator,
            write,
            _Selectors
        );

    /// <summary>
    /// Добавить колонки по умолчанию на основе свойств типа T
    /// </summary>
    /// <returns>Модифицированный построитель с добавленными колонками</returns>
    /// <remarks>
    /// Метод рефлексирует все публичные читаемые свойства типа T
    /// и создаёт селекторы для каждого свойства.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// var writer = new CSVWriter<Person>(people)
    ///     .AddDefaultHeaders()  // добавит столбцы для каждого свойства
    ///     .WriteTo("output.csv");
    /// ]]>
    /// </example>
    public CSVWriter<T> AddDefaultHeaders() =>
        new(
            _Items,
            _Separator,
            _WriteHeaders,
            MergeSelectors(_Selectors, CreateHeaders())
        );

    /// <summary>
    /// Объединить два набора селекторов колонок
    /// </summary>
    /// <param name="SourceColumns">Исходные колонки</param>
    /// <param name="NewColumns">Добавляемые колонки</param>
    /// <returns>Новый словарь, объединяющий оба набора</returns>
    private static IDictionary<string, (Func<T, object> Selector, int Index)>? MergeSelectors(
        IDictionary<string, (Func<T, object> Selector, int Index)>? SourceColumns,
        IDictionary<string, (Func<T, object> Selector, int Index)>? NewColumns = null)
    {
        if (SourceColumns is null)
            return NewColumns;

        var result = new Dictionary<string, (Func<T, object> Selector, int Index)>(SourceColumns);
        if (NewColumns is null) return result;

        foreach (var (header, selector) in NewColumns)
            result[header] = selector;

        return result;
    }

    /// <summary>
    /// Объединить селекторы при добавлении одной новой колонки
    /// </summary>
    /// <param name="SourceColumns">Исходные селекторы</param>
    /// <param name="NewColumnName">Имя новой колонки</param>
    /// <param name="NewColumnValueSelector">Функция-селектор для извлечения значения</param>
    /// <returns>Новый словарь с добавленной колонкой</returns>
    private static IDictionary<string, (Func<T, object> Selector, int Index)> MergeSelectors(
        IDictionary<string, (Func<T, object> Selector, int Index)>? SourceColumns,
        string NewColumnName, Func<T, object> NewColumnValueSelector)
    {
        Dictionary<string, (Func<T, object> Selector, int Index)> result = SourceColumns is null
            ? new()
            : new(SourceColumns);

        var index = result.Count == 0 ? 0 : result.Max(s => s.Value.Index) + 1;
        result[NewColumnName] = (NewColumnValueSelector, index);

        return result;
    }

    /// <summary>
    /// Добавить новую колонку с пользовательским селектором
    /// </summary>
    /// <param name="NewColumnName">Имя новой колонки в заголовке</param>
    /// <param name="NewColumnValueSelector">Функция для извлечения значения из элемента T</param>
    /// <returns>Модифицированный построитель</returns>
    /// <example>
    /// <![CDATA[
    /// var writer = new CSVWriter<Person>(people)
    ///     .AddColumn("FullName", p => $"{p.FirstName} {p.LastName}")
    ///     .AddColumn("Age", p => p.DateOfBirth.Year);
    /// ]]>
    /// </example>
    public CSVWriter<T> AddColumn(string NewColumnName, Func<T, object> NewColumnValueSelector) =>
        new(
            _Items,
            _Separator,
            _WriteHeaders,
            MergeSelectors(_Selectors, NewColumnName, NewColumnValueSelector)
        );

    /// <summary>
    /// Удалить колонку по имени
    /// </summary>
    /// <param name="Name">Имя удаляемой колонки</param>
    /// <returns>Модифицированный построитель без указанной колонки</returns>
    public CSVWriter<T> RemoveColumn(string Name)
    {
        var columns = _Selectors;
        if (columns?.ContainsKey(Name) != true)
            return new(_Items, _Separator, _WriteHeaders, columns);

        columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
        columns.Remove(Name);
        return new(_Items, _Separator, _WriteHeaders, columns);
    }

    /// <summary>
    /// Удалить колонку по индексу
    /// </summary>
    /// <param name="index">Индекс удаляемой колонки (0-based)</param>
    /// <returns>Модифицированный построитель без указанной колонки</returns>
    public CSVWriter<T> RemoveColumn(int index)
    {
        if (_Selectors is not { } columns)
            return new(_Items, _Separator, _WriteHeaders, null);

        var i = 0;
        foreach (var (header, _) in columns)
            if (i++ == index)
            {
                columns = new Dictionary<string, (Func<T, object> Selector, int Index)>(columns);
                columns.Remove(header);
                break;
            }

        return new(_Items, _Separator, _WriteHeaders, columns);
    }

    #region Write

    /// <summary>
    /// Записать данные в файл по указанному пути
    /// </summary>
    /// <param name="FileName">Путь к файлу для записи</param>
    /// <param name="encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <remarks>
    /// Файл будет создан или перезаписан. Если папка не существует, возбуждается исключение.
    /// </remarks>
    public void WriteTo(string FileName, Encoding? encoding = null)
    {
        using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        WriteTo(file_stream, encoding);
    }

    /// <summary>
    /// Асинхронно записать данные в файл по указанному пути
    /// </summary>
    /// <param name="FileName">Путь к файлу для записи</param>
    /// <param name="encoding">Кодировка текста (по умолчанию UTF-8)</param>
    /// <param name="Cancel">Токен отмены асинхронной операции</param>
    /// <returns>Задача асинхронной записи</returns>
    public async Task WriteToAsync(string FileName, Encoding? encoding = null, CancellationToken Cancel = default)
    {
        using var file_stream = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        await WriteToAsync(file_stream, encoding, Cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Записать данные в указанный файл
    /// </summary>
    /// <param name="File">Объект FileInfo для записи</param>
    public void WriteTo(FileInfo File)
    {
        using var writer = File.CreateText();
        WriteTo(writer);
    }

    /// <summary>
    /// Асинхронно записать данные в указанный файл
    /// </summary>
    /// <param name="File">Объект FileInfo для записи</param>
    /// <param name="Cancel">Токен отмены асинхронной операции</param>
    /// <returns>Задача асинхронной записи</returns>
    public async Task WriteToAsync(FileInfo File, CancellationToken Cancel = default)
    {
        using var writer = File.CreateText();
        await WriteToAsync(writer, Cancel).ConfigureAwait(false);
    }

    /// <summary>
    /// Записать данные в поток с указанной кодировкой
    /// </summary>
    /// <param name="stream">Поток для записи</param>
    /// <param name="encoding">Кодировка (по умолчанию UTF-8)</param>
    public void WriteTo(Stream stream, Encoding? encoding = null) =>
        WriteTo(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true));

    /// <summary>
    /// Асинхронно записать данные в поток
    /// </summary>
    /// <param name="stream">Поток для записи</param>
    /// <param name="encoding">Кодировка (по умолчанию UTF-8)</param>
    /// <param name="Cancel">Токен отмены асинхронной операции</param>
    /// <returns>Задача асинхронной записи</returns>
    public async Task WriteToAsync(Stream stream, Encoding? encoding = null, CancellationToken Cancel = default) =>
        await WriteToAsync(new StreamWriter(stream, encoding ?? Encoding.UTF8, 1024, true), Cancel).ConfigureAwait(false);

    /// <summary>
    /// Записать данные в объект TextWriter (синхронно)
    /// </summary>
    /// <param name="writer">Объект для записи текста</param>
    /// <remarks>
    /// Сначала записывается заголовок (если включен), затем все строки данных.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// using var writer = new StreamWriter("output.csv");
    /// csvWriter.WriteTo(writer);
    /// ]]>
    /// </example>
    public void WriteTo(TextWriter writer)
    {
        var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

        var values = selectors_key.Select(s => s.Key).ToArray();
        var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

        var separator = _Separator;
        writer.WriteLineValues(separator, values);

        foreach (var item in _Items)
        {
            for (var i = 0; i < values.Length; i++)
                values[i] = Convert.ToString(selectors[i](item))!;
            writer.WriteLineValues(separator, values);
        }
    }

    /// <summary>
    /// Асинхронно записать данные в объект TextWriter
    /// </summary>
    /// <param name="writer">Объект для записи текста</param>
    /// <param name="Cancel">Токен отмены асинхронной операции</param>
    /// <returns>Задача асинхронной записи</returns>
    /// <remarks>
    /// Поддерживает отмену операции через CancellationToken.
    /// </remarks>
    public async Task WriteToAsync(TextWriter writer, CancellationToken Cancel = default)
    {
        Cancel.ThrowIfCancellationRequested();
        var selectors_key = GetOrdered(_Selectors ?? CreateHeaders()).ToArray();

        var values = selectors_key.Select(s => s.Key).ToArray();
        var selectors = selectors_key.Select(s => s.Value.Selector).ToArray();

        var separator = _Separator;
        Cancel.ThrowIfCancellationRequested();
        await writer.WriteLineValuesAsync(separator, values).ConfigureAwait(false);

        Cancel.ThrowIfCancellationRequested();
        foreach (var item in _Items)
        {
            Cancel.ThrowIfCancellationRequested();
            for (var i = 0; i < values.Length; i++)
                values[i] = Convert.ToString(selectors[i](item))!;

            Cancel.ThrowIfCancellationRequested();
            await writer.WriteLineValuesAsync(separator, values).ConfigureAwait(false);
        }
    }

    #endregion
}