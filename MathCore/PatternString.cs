#nullable enable
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>Строковый процессор, формирующий строку на основе шаблона</summary>
public class PatternString : IEnumerable<KeyValuePair<string, object>>
{
    /// <summary>Регулярное выражение поиска составных частей шаблона</summary>
    private static readonly Regex __Regex = new(@"{(?<name>\w+)(?::(?<format>.+?))?}", RegexOptions.Compiled);

    private readonly string _Pattern;
    private readonly Dictionary<string, object> _Fields = new();
    private string? _Regex;

    /// <summary>Способ форматирования типов <seealso cref="IFormattable"/></summary>
    public IFormatProvider FormatProvider { get; init; } = CultureInfo.CurrentCulture;

    /// <summary>
    /// Искомый шаблон в формате регулярного выражения. <br/>
    /// Должен содержать обязательно группу (?&lt;name&gt;\w), обнаруживающую имя поля и группу (?&lt;format&gt;.+?) с форматом значения поля. <br/>
    /// Примеры:<br/>
    /// &#0032;{(?&lt;name&gt;\w+)(?::(?&lt;format&gt;.+?))?} преобразует {FileName}[{date:yyy-MM-ddTHH-mm-ss}].{ext}<br/>
    /// &#x0020;\[\[(?&lt;name&gt;\w+)(:(?&lt;format&gt;.*?))?\]\] преобразует [[FileName]][[[date:yyy-MM-ddTHH-mm-ss]]].[[ext]]
    /// </summary>
    public string? Pattern
    {
        get => _Regex ?? __Regex.ToString();
        set
        {
            if (value is null)
            {
                _Regex = null;
                return;
            }

            if (!value.Contains("(?<name>"))
                throw new ArgumentException("Строка регулярного выражения должна содержать группу (?:<name>...)");
            _Regex = value;
        }
    }

    /// <summary>Добавление источников данных для замены полей в строке формата</summary>
    /// <param name="FieldName">Замещаемое поле в строке формата</param>
    /// <returns>Функция генерации значения</returns>
    public Func<object>? this[string FieldName]
    {
        get => !_Fields.TryGetValue(FieldName, out var value)
            ? null
            : value is Func<object> f
                ? f
                : value is Func<string, object> nf
                    ? () => nf(FieldName)
                    : () => value;
        set
        {
            if (value is null)
                _Fields.Remove(FieldName);
            else
                _Fields[FieldName] = value;
        }
    }

    /// <summary>Новый процессор шаблона строки</summary>
    /// <param name="Pattern">Шаблон строки, содержащий набор полей для подстановки в них данных</param>
    public PatternString(string Pattern) => _Pattern = Pattern.NotNull();

    /// <summary>Добавить значение для поля</summary>
    /// <param name="FieldName">Имя поля для подстановки</param>
    /// <param name="value">Значение, замещающее указанный <see cref="FieldName"/></param>
    public void Add(string FieldName, object value) => _Fields[FieldName] = value;

    /// <summary>Добавить значение</summary>
    /// <param name="FieldName">Имя поля для подстановки</param>
    /// <param name="Selector">Функция генерации значения для замещения <see cref="FieldName"/></param>
    public void Add(string FieldName, Func<object> Selector) => _Fields[FieldName] = Selector;

    /// <summary>Добавить значение</summary>
    /// <param name="FieldName">Имя поля для подстановки</param>
    /// <param name="Selector">Функция генерации значения для замещения <see cref="FieldName"/>, в которую передаётся имя токена</param>
    public void Add(string FieldName, Func<string, object> Selector) => _Fields[FieldName] = Selector;

    /// <summary>Удалить значение токена</summary>
    /// <param name="FieldName">Имя удаляемого поля</param>
    public void Remove(string FieldName) => _Fields.Remove(FieldName);

    /// <summary>Проверка наличия значения для указанного поля по его имени</summary>
    /// <param name="FieldName">Имя проверяемого поля</param>
    /// <returns>Истина, если поле значение для поля определено</returns>
    public bool Contains(string FieldName) => _Fields.ContainsKey(FieldName);

    public override string ToString() =>
        _Pattern is not { Length: > 0 } pattern
            ? string.Empty
            : _Regex is { Length: > 0 } regex
                ? Regex.Replace(pattern, regex, PatternPartSelector)
                : __Regex.Replace(pattern, PatternPartSelector);

    /// <summary>Метод, выполняемый регулярным выражением в процессе подстановки значений в строку шаблона</summary>
    /// <param name="Match">Найденное поле</param>
    /// <returns>Строка, подставляемая вместо найденного поля</returns>
    private string? PatternPartSelector(Match Match)
    {
        var name = Match.Groups["name"].Value;
        if (!_Fields.TryGetValue(name, out var selector)) return Match.Value;
        var value = selector switch
        {
            Func<object> f         => f(),
            Func<string, object> f => f(name),
            _                      => selector
        };
        return Match.Groups["format"].Success && value is IFormattable formattable_value
            ? formattable_value.ToString(Match.Groups["format"].Value, FormatProvider)
            : value as string ?? value?.ToString();
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _Fields.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Fields).GetEnumerator();
}