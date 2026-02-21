using System.Collections;
using System.Globalization;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.CSV;

/// <summary>
/// Объект для чтения и обработки данных из CSV-файла с поддержкой гибкой конфигурации
/// </summary>
/// <remarks>
/// Структура предоставляет fluent API для конфигурации параметров чтения CSV-данных.
/// Поддерживает пропуск строк в начале файла и после заголовка, ограничение количества читаемых строк,
/// преобразование колонок, работу с разными разделителями и культурами.
/// Данные читаются лениво при итерации по результату GetEnumerator().
/// </remarks>
public readonly struct CSVQuery : IEnumerable<CSVQueryRow>, IEquatable<CSVQuery>, IStructuralEquatable
{
    /// <summary>Метод-фабрика для создания объектов чтения текста</summary>
    private readonly Func<TextReader> _ReaderFactory;

    /// <summary>
    /// Число строк, пропускаемых в начале файла перед заголовком
    /// </summary>
    /// <value>Количество пропускаемых строк (по умолчанию 0)</value>
    public int SkipRowsCount { get; init; }

    /// <summary>
    /// Число строк, пропускаемых после строки заголовка (независимо от его наличия)
    /// </summary>
    /// <value>Количество строк для пропуска после заголовка (по умолчанию 0)</value>
    public int SkipRowsAfterHeaderCount { get; init; }

    /// <summary>
    /// Максимальное число строк данных для чтения (-1 = читать все)
    /// </summary>
    /// <value>Количество читаемых строк или -1 для чтения всех (по умолчанию -1)</value>
    public int TakeRowsCount { get; init; }

    /// <summary>
    /// Указывает, содержит ли CSV-файл строку заголовка
    /// </summary>
    /// <value>true, если первая строка данных — заголовок; иначе false</value>
    public bool ContainsHeader { get; init; }

    /// <summary>
    /// Символ для разделения значений в строке
    /// </summary>
    /// <value>Символ-разделитель (по умолчанию ',')</value>
    public char Separator { get; init; }

    /// <summary>
    /// Строка конца строки для корректного расчёта позиции в потоке
    /// </summary>
    /// <value>Последовательность символов конца строки (по умолчанию Environment.NewLine)</value>
    public string EoL { get; init; }

    /// <summary>
    /// Культура для преобразования строк в типизированные значения
    /// </summary>
    /// <value>Информация о культуре или null для использования текущей культуры</value>
    public CultureInfo? Culture { get; init; }

    /// <summary>Словарь заголовков (имя колонки -> индекс)</summary>
    private IDictionary<string, int>? Headers { get; init; }

    /// <summary>
    /// Инициализация нового экземпляра для чтения CSV-данных
    /// </summary>
    /// <param name="ReaderFactory">Метод-фабрика, возвращающий объект для чтения текста</param>
    /// <param name="Separator">Символ-разделитель значений (по умолчанию ',')</param>
    /// <example>
    /// <![CDATA[
    /// var file = new FileInfo("data.csv");
    /// var query = new CSVQuery(file.OpenText, ',');
    /// foreach (var row in query.WithHeader())
    ///     Console.WriteLine(row["Name"]);
    /// ]]>
    /// </example>
    public CSVQuery(Func<TextReader> ReaderFactory, char Separator = ',')
        : this(ReaderFactory, 0, false, 0, Separator, -1, null, null, null) { }

    /// <summary>Инициализация внутреннего экземпляра с полным набором параметров</summary>
    private CSVQuery(
        Func<TextReader> ReaderFactory,
        int SkipRows,
        bool ContainsHeader,
        int SkipRowsAfterHeader,
        char ValuesSeparator,
        int TakeRows,
        IDictionary<string, int>? Headers,
        string? EoL,
        CultureInfo? Culture
    )
    {
        _ReaderFactory = ReaderFactory.NotNull();
        SkipRowsCount = SkipRows;
        this.ContainsHeader = ContainsHeader;
        SkipRowsAfterHeaderCount = SkipRowsAfterHeader;
        Separator = ValuesSeparator;
        TakeRowsCount = TakeRows;
        this.Headers = Headers;
        this.EoL = EoL ?? Environment.NewLine;
        this.Culture = Culture;
    }

    /// <summary>Копирующий конструктор для модификации параметров</summary>
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

    /// <summary>
    /// Установить число пропускаемых строк в начале файла (перед заголовком)
    /// </summary>
    /// <param name="RowsCount">Количество пропускаемых строк</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    /// <example>
    /// <![CDATA[
    /// var query = new CSVQuery(...)
    ///     .SkipRowsBeforeHeader(2)  // пропустить первые 2 строки
    ///     .WithHeader();
    /// ]]>
    /// </example>
    public CSVQuery SkipRowsBeforeHeader(int RowsCount) => new(this) { SkipRowsCount = RowsCount };

    /// <summary>
    /// Установить число пропускаемых строк после заголовка
    /// </summary>
    /// <param name="RowsCount">Количество строк для пропуска</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    public CSVQuery SkipRowsAfterHeader(int RowsCount) => new(this) { SkipRowsAfterHeaderCount = RowsCount };

    /// <summary>
    /// Указать, содержит ли файл строку заголовка
    /// </summary>
    /// <param name="IsExist">true, если заголовок присутствует; false иначе</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    /// <example>
    /// <![CDATA[
    /// var query = new CSVQuery(...)
    ///     .WithHeader(true);  // файл содержит заголовок
    /// ]]>
    /// </example>
    public CSVQuery WithHeader(bool IsExist = true) => new(this) { ContainsHeader = IsExist };

    /// <summary>
    /// Установить символ-разделитель для значений в строке
    /// </summary>
    /// <param name="NewSeparator">Новый разделитель (например, ';' для формата CSV по RFC)</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    public CSVQuery ValuesSeparator(char NewSeparator) => new(this) { Separator = NewSeparator };

    /// <summary>
    /// Установить максимальное число читаемых строк данных
    /// </summary>
    /// <param name="RowsCount">Число строк (-1 = читать всё)</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    /// <example>
    /// <![CDATA[
    /// var query = new CSVQuery(...)
    ///     .WithHeader()
    ///     .TakeRows(100);  // прочитать первые 100 строк данных
    /// ]]>
    /// </example>
    public CSVQuery TakeRows(int RowsCount) => new(this) { TakeRowsCount = RowsCount };

    /// <summary>
    /// Установить полный словарь заголовков (переопределить автоматически читаемый)
    /// </summary>
    /// <param name="Header">Словарь (имя колонки -> индекс)</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    public CSVQuery Header(IDictionary<string, int> Header) => new(this) { Headers = Header };

    /// <summary>Объединить словари заголовков</summary>
    /// <param name="Source">Исходный словарь</param>
    /// <param name="Values">Добавляемые значения</param>
    /// <returns>Новый объединённый словарь</returns>
    private static SortedList<string, int> Merge(IDictionary<string, int>? Source, IDictionary<string, int>? Values = null)
    {
        SortedList<string, int> result = Source is { Count: > 0 } ? new(Source) : [];

        if (Values is not { Count: > 0 }) return result;

        foreach (var (key, value) in Values)
            result[key] = value;

        return result;
    }

    /// <summary>
    /// Добавить новые колонки к существующему заголовку
    /// </summary>
    /// <param name="Header">Словарь с новыми колонками</param>
    /// <returns>Новый экземпляр с объединённым заголовком</returns>
    public CSVQuery MergeHeader(IDictionary<string, int> Header) => new(this) { Headers = Merge(Headers, Header) };

    /// <summary>
    /// Добавить одну колонку с псевдонимом к заголовку
    /// </summary>
    /// <param name="AliasName">Имя (псевдоним) колонки</param>
    /// <param name="Index">Индекс колонки в исходных данных</param>
    /// <returns>Новый экземпляр с дополненным заголовком</returns>
    /// <example>
    /// <![CDATA[
    /// var query = new CSVQuery(...)
    ///     .AddColumn("ID", 0)
    ///     .AddColumn("Name", 1);
    /// ]]>
    /// </example>
    public CSVQuery AddColumn(string AliasName, int Index) => MergeHeader(new Dictionary<string, int> { { AliasName, Index } });

    /// <summary>
    /// Добавить несколько колонок с псевдонимами к заголовку
    /// </summary>
    /// <param name="Columns">Перечисление кортежей (имя, индекс)</param>
    /// <returns>Новый экземпляр с дополненным заголовком</returns>
    public CSVQuery AddColumns(params IEnumerable<(string AliasName, int Index)> Columns) => MergeHeader(Columns.ToDictionary(c => c.AliasName, c => c.Index));

    /// <summary>
    /// Удалить колонку из заголовка по имени
    /// </summary>
    /// <param name="ColumnName">Имя удаляемой колонки</param>
    /// <returns>Новый экземпляр с изменённым заголовком</returns>
    public CSVQuery RemoveColumn(string ColumnName)
    {
        if (Headers is not { Count: > 0 } header) return this;
        header = Merge(header);
        header.Remove(ColumnName);

        return new(this) { Headers = header };
    }

    /// <summary>
    /// Удалить колонку из заголовка по индексу
    /// </summary>
    /// <param name="ColumnIndex">Индекс удаляемой колонки</param>
    /// <returns>Новый экземпляр с изменённым заголовком</returns>
    public CSVQuery RemoveColumn(int ColumnIndex)
    {
        if (Headers is not { Count: > 0 } header) return this;

        header = Merge(header);
        foreach (var column in Headers.Where(v => v.Value == ColumnIndex).Select(v => v.Key))
            header.Remove(column);
        return new(this) { Headers = header };
    }

    /// <summary>
    /// Установить символ(ы) конца строки для корректного расчёта позиции
    /// </summary>
    /// <param name="eol">Строка конца строки ("\r\n", "\n", "\r")</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    public CSVQuery WithEoL(string eol) => new(this) { EoL = eol };

    /// <summary>
    /// Установить культуру для преобразования данных при чтении
    /// </summary>
    /// <param name="culture">Культура (null = текущая культура потока)</param>
    /// <returns>Новый экземпляр с изменённым параметром</returns>
    /// <example>
    /// <![CDATA[
    /// var query = new CSVQuery(...)
    ///     .WithCulture(CultureInfo.GetCultureInfo("de-DE"));  // немецкий формат чисел
    /// ]]>
    /// </example>
    public CSVQuery WithCulture(CultureInfo? culture = null) => new(this) { Culture = culture };

    /// <summary>
    /// Прочитать строку заголовка из источника и вернуть словарь колонок
    /// </summary>
    /// <param name="MergeWithDefault">true = объединить с предварительно установленным заголовком</param>
    /// <returns>Словарь (имя колонки -> индекс)</returns>
    /// <exception cref="FormatException">Если заголовок пуст или источник неожиданно завершился</exception>
    public IDictionary<string, int> GetHeader(bool MergeWithDefault = true)
    {
        using var reader = _ReaderFactory();

        var count = SkipRowsCount;
        while (count-- > 0)
            if (reader.ReadLine() is null) break;

        if (count > 0)
            throw new FormatException("Неожиданный конец потока");

        char[] separator = [Separator];
        var line = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(line))
            throw new FormatException("Пустая строка заголовка");

        SortedList<string, int> header = MergeWithDefault && Headers != null ? new(Headers) : [];

        var headers = CSVParser.ParseLine(line, Separator, true).ToArray();
        for (var i = 0; i < headers.Length; i++)
            header[headers[i]] = i;

        return header;
    }

    /// <summary>
    /// Получить перечислитель для чтения строк данных из CSV-источника
    /// </summary>
    /// <returns>Перечисление объектов CSVQueryRow</returns>
    /// <remarks>
    /// Чтение данных ленивое — строки читаются по запросу при итерации.
    /// Поток остаётся открытым во время итерации.
    /// </remarks>
    public IEnumerator<CSVQueryRow> GetEnumerator()
    {
        using var reader = _ReaderFactory();

        if (reader is not StreamReader { CurrentEncoding: var encoding }) encoding = Encoding.Default;

        var eol = EoL is { Length: > 0 } s_eol ? encoding.GetByteCount(s_eol) : 0;

        var position = 0L;

        var index = SkipRowsCount;
        while (index-- > 0)
        {
            var line = reader.ReadLine();
            if (line is null) yield break;
            position += encoding.GetByteCount(line) + eol;
        }

        var header = Merge(Headers);
        if (ContainsHeader)
        {
            var line = reader.ReadLine();
            if (line is null) yield break;
            position += encoding.GetByteCount(line) + eol;

            if (!string.IsNullOrWhiteSpace(line))
            {
                var headers = CSVParser.ParseLine(line, Separator, true).ToArray();
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

            var items = CSVParser.ParseLine(line, Separator).ToArray();
            yield return new(line, index, items, header, position, (position += line_length + eol) - 1, culture);
            index++;
        }
        while (index != TakeRowsCount);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(object? other, IEqualityComparer comparer) =>
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

    public override bool Equals(object? obj) => obj is CSVQuery query && Equals(query);

    public override int GetHashCode() => HashBuilder.New(_ReaderFactory)
       .Append(SkipRowsCount)
       .Append(ContainsHeader)
       .Append(SkipRowsAfterHeaderCount)
       .Append(Separator)
       .Append(TakeRowsCount);

    public static bool operator ==(CSVQuery left, CSVQuery right) => left.Equals(right);

    public static bool operator !=(CSVQuery left, CSVQuery right) => !(left == right);

    public bool Equals(CSVQuery other) =>
        Equals(_ReaderFactory, other._ReaderFactory)
        && SkipRowsCount == other.SkipRowsCount
        && ContainsHeader == other.ContainsHeader
        && SkipRowsAfterHeaderCount == other.SkipRowsAfterHeaderCount
        && Separator == other.Separator
        && TakeRowsCount == other.TakeRowsCount
        && Equals(Headers, other.Headers);
}