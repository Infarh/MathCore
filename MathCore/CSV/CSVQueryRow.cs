using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;

using MathCore.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MathCore.CSV;

/// <summary>
/// Строка данных из CSV-файла с поддержкой доступа по индексу и по имени колонки
/// </summary>
/// <remarks>
/// Структура представляет одну строку данных из CSV-источника и предоставляет удобный способ доступа
/// к значениям ячеек как по индексу колонки, так и по её названию.
/// Поддерживает преобразование строковых значений в типизированные значения через встроенный тип Value.
/// Трекирует положение строки в исходном потоке для целей навигации.
/// </remarks>
public readonly struct CSVQueryRow : IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>Массив элементов данных строки</summary>
    private readonly string[] _Items;

    /// <summary>Словарь столбцов</summary>
    private readonly IDictionary<string, int> _Header;

    private readonly CultureInfo _Culture;

    /// <summary>Число элементов данных строки</summary>
    /// <value>Количество ячеек в строке</value>
    public int ItemsCount => _Items.Length;

    /// <summary>Индекс (номер) строки (отсчёт от 0)</summary>
    /// <value>Порядковый номер строки в CSV-данных</value>
    public int Index { get; }

    /// <summary>Начальное положение строки в источнике (в байтах)</summary>
    /// <value>Позиция первого символа строки</value>
    public long StartPos { get; }

    /// <summary>Конечное положение строки в источнике (в байтах)</summary>
    /// <value>Позиция последнего символа строки</value>
    public long EndPos { get; }

    /// <summary>Исходная текстовая строка из источника</summary>
    /// <value>Необработанная строка из CSV-файла</value>
    public string SourceLine { get; }

    /// <summary>Словарь заголовков: имя колонки -> индекс</summary>
    /// <value>Доступная только для чтения коллекция заголовков</value>
    [NotNull] public IReadOnlyDictionary<string, int> Headers => new ReadOnlyDictionary<string, int>(_Header);

    /// <summary>
    /// Получить типизированное значение по индексу колонки
    /// </summary>
    /// <param name="ValueIndex">Индекс колонки (отрицательные значения отсчитываются с конца: -1 — последняя колонка)</param>
    /// <returns>Обёртка Value для типизированного доступа к значению</returns>
    /// <example>
    /// <![CDATA[
    /// var row = ...;
    /// int id = row[0].Int32Value;
    /// decimal price = row[-1].DecimalValue;
    /// ]]>
    /// </example>
    public Value this[int ValueIndex] => new(ValueIndex >= 0 ? _Items[ValueIndex] : _Items[_Items.Length + ValueIndex], _Culture);

    /// <summary>
    /// Получить ссылку на строковое значение по индексу для изменения
    /// </summary>
    /// <param name="ValueIndex">Индекс колонки (отрицательные значения отсчитываются с конца)</param>
    /// <returns>Ссылка на строковое значение</returns>
    public ref string Value(int ValueIndex) => ref ValueIndex >= 0
        ? ref _Items[ValueIndex]
        : ref _Items[_Items.Length + ValueIndex];

    /// <summary>
    /// Получить типизированное значение по имени колонки
    /// </summary>
    /// <param name="ValueName">Имя колонки в заголовке</param>
    /// <returns>Обёртка Value для типизированного доступа к значению</returns>
    /// <exception cref="KeyNotFoundException">Если колонка с указанным именем не найдена</exception>
    /// <example>
    /// <![CDATA[
    /// var row = ...;
    /// int id = row["ID"].Int32Value;
    /// string name = row["Name"].StringValue;
    /// ]]>
    /// </example>
    public Value this[[NotNull] string ValueName] => new(_Items[_Header[ValueName]], _Culture);

    /// <summary>
    /// Получить ссылку на строковое значение по имени колонки для изменения
    /// </summary>
    /// <param name="ValueName">Имя колонки в заголовке</param>
    /// <returns>Ссылка на строковое значение</returns>
    /// <exception cref="KeyNotFoundException">Если колонка с указанным именем не найдена</exception>
    public ref string Value([NotNull] string ValueName) => ref _Items[_Header[ValueName]];

    /// <summary>
    /// Инициализация нового экземпляра строки данных
    /// </summary>
    /// <param name="SourceLine">Исходная текстовая строка из CSV-источника</param>
    /// <param name="Index">Индекс (номер) строки, начиная с 0</param>
    /// <param name="Items">Массив строковых значений ячеек</param>
    /// <param name="Header">Словарь соответствия имён колонок их индексам</param>
    /// <param name="StartPos">Начальное положение строки в источнике (в байтах)</param>
    /// <param name="EndPos">Конечное положение строки в источнике (в байтах)</param>
    /// <param name="Culture">Сведения о культуре для преобразования данных</param>
    public CSVQueryRow(
        string SourceLine, 
        int Index, 
        string[] Items, 
        IDictionary<string, int> Header, 
        long StartPos, long EndPos, 
        CultureInfo Culture)
    {
        this.SourceLine = SourceLine;
        this.Index      = Index;
        this.StartPos   = StartPos;
        this.EndPos     = EndPos;
        _Items          = Items;
        _Header         = Header;
        _Culture        = Culture;
    }

    /// <summary>
    /// Преобразовать значение ячейки в указанный тип через Convert.ChangeType
    /// </summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <param name="ValueIndex">Индекс колонки</param>
    /// <returns>Преобразованное значение указанного типа</returns>
    /// <exception cref="InvalidCastException">Если преобразование невозможно</exception>
    /// <example>
    /// <![CDATA[
    /// var row = ...;
    /// int value = row.ValueAs<int>(0);
    /// ]]>
    /// </example>
    public T ValueAs<T>(int ValueIndex) => (T)Convert.ChangeType(Value(ValueIndex), typeof(T));

    /// <summary>
    /// Преобразовать значение ячейки по имени колонки в указанный тип
    /// </summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <param name="ValueName">Имя колонки</param>
    /// <returns>Преобразованное значение указанного типа</returns>
    /// <exception cref="KeyNotFoundException">Если колонка не найдена</exception>
    /// <exception cref="InvalidCastException">Если преобразование невозможно</exception>
    public T ValueAs<T>([NotNull] string ValueName) => ValueAs<T>(_Header[ValueName]);

    /// <summary>
    /// Получить ссылку на первое строковое значение в строке
    /// </summary>
    /// <returns>Ссылка на первую ячейку</returns>
    public ref string FirstValue() => ref Value(0);

    /// <summary>
    /// Получить ссылку на последнее строковое значение в строке
    /// </summary>
    /// <returns>Ссылка на последнюю ячейку</returns>
    public ref string LastValue() => ref Value(-1);

    /// <summary>
    /// Получить первое значение, преобразованное в указанный тип
    /// </summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <returns>Преобразованное значение первой ячейки</returns>
    public T FirstValue<T>() => ValueAs<T>(0);

    /// <summary>
    /// Получить последнее значение, преобразованное в указанный тип
    /// </summary>
    /// <typeparam name="T">Требуемый тип значения</typeparam>
    /// <returns>Преобразованное значение последней ячейки</returns>
    public T LastValue<T>() => ValueAs<T>(-1);

    /// <summary>
    /// Преобразовать строку в словарь (имя колонки -> значение)
    /// </summary>
    /// <returns>Новый словарь, содержащий все пары имя-значение</returns>
    /// <example>
    /// <![CDATA[
    /// var row = ...;
    /// var dict = row.ToDictionary();
    /// foreach (var (key, value) in dict)
    ///     Console.WriteLine($"{key}: {value}");
    /// ]]>
    /// </example>
    [NotNull]
    public IDictionary<string, string> ToDictionary()
    {
        var values = _Items;
        return _Header.ToDictionary(h => h.Key, h => values[h.Value]);
    }

    /// <summary>
    /// Получить перечислитель для итерации по парам (имя колонки, значение)
    /// </summary>
    /// <returns>Перечислитель, отсортированный по индексу, затем по имени колонки</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach (var (header, index) in _Header.OrderBy(h => h.Value).ThenBy(h => h.Key))
            yield return new(header, _Items[index]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}