using System.Globalization;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Text.RegularExpressions;

/// <summary>Статический класс с методами-расширениями для работы с регулярными выражениями</summary>
public static class RegExExtensions
{
    /// <summary>Возвращает значение совпадения или строку по умолчанию</summary>
    /// <param name="match">Результат поиска</param>
    /// <param name="DefaultString">Строка по умолчанию</param>
    /// <returns>Значение совпадения или строка по умолчанию</returns>
    public static string OrDefault(this Match match, string DefaultString = "") => match.Success ? match.Value : DefaultString;

    /// <summary>Возвращает целое значение совпадения или значение по умолчанию</summary>
    /// <param name="match">Результат поиска</param>
    /// <param name="DefaultInt">Значение по умолчанию</param>
    /// <returns>Целое значение совпадения или значение по умолчанию</returns>
    public static int OrDefault(this Match match, int DefaultInt) => int.TryParse(match.OrDefault(), out var v) ? v : DefaultInt;

    /// <summary>Возвращает значение группы или строку по умолчанию</summary>
    /// <param name="group">Группа совпадения</param>
    /// <param name="DefaultString">Строка по умолчанию</param>
    /// <returns>Значение группы или строка по умолчанию</returns>
    public static string OrDefault(this Group group, string DefaultString = "") => group.Success ? group.Value : DefaultString;

    /// <summary>Возвращает целое значение группы или значение по умолчанию</summary>
    /// <param name="group">Группа совпадения</param>
    /// <param name="DefaultInt">Значение по умолчанию</param>
    /// <returns>Целое значение группы или значение по умолчанию</returns>
    public static int OrDefault(this Group group, int DefaultInt) => int.TryParse(group.OrDefault(), out var v) ? v : DefaultInt;

    /// <summary>Выполняет поиск по регулярному выражению</summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="regex">Регулярное выражение</param>
    /// <returns>Результат поиска</returns>
    public static Match Find(this string Str, Regex regex) => regex.Match(Str);

    /// <summary>Выполняет поиск по строковому шаблону регулярного выражения</summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="Pattern">Шаблон регулярного выражения</param>
    /// <returns>Результат поиска</returns>
    public static Match FindRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Match(Str, Pattern);

    /// <summary>Возвращает все совпадения по шаблону</summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="Pattern">Шаблон регулярного выражения</param>
    /// <returns>Коллекция совпадений</returns>
    public static MatchCollection FindAllRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Matches(Str, Pattern);

    /// <summary>Возвращает значения всех совпадений по регулярному выражению</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="regex">Регулярное выражение</param>
    /// <returns>Перечисление значений совпадений</returns>
    public static IEnumerable<string> FindAllRegEx(this string str, Regex regex) => str.FindAllRegExMatch(regex).Select(m => m.Value);

    /// <summary>Возвращает первое совпадение по шаблону или строку по умолчанию</summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="Pattern">Шаблон регулярного выражения</param>
    /// <param name="Default">Строка по умолчанию</param>
    /// <returns>Значение совпадения или строка по умолчанию</returns>
    public static string FindRegEx(this string Str, [RegexPattern] string Pattern, string Default) => Regex.Match(Str, Pattern).OrDefault(Default);

    /// <summary>Возвращает первое целое совпадение по шаблону или значение по умолчанию</summary>
    /// <param name="Str">Исходная строка</param>
    /// <param name="Pattern">Шаблон регулярного выражения</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Целое значение совпадения или значение по умолчанию</returns>
    public static int FindRegEx(this string Str, [RegexPattern] string Pattern, int DefaultValue) => int.TryParse(Str.FindRegEx(Pattern, string.Empty), out var v) ? v : DefaultValue;

    /// <summary>Возвращает первое совпадение по регулярному выражению или строку по умолчанию</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="regex">Регулярное выражение</param>
    /// <param name="Default">Строка по умолчанию</param>
    /// <returns>Значение совпадения или строка по умолчанию</returns>
    public static string FindRegEx(this string str, Regex regex, string Default = "") => regex.Match(str).OrDefault(Default);

    /// <summary>Возвращает значение группы или строку по умолчанию</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Строка по умолчанию</param>
    /// <returns>Значение группы или строка по умолчанию</returns>
    public static string? ValueOrDefault(this Group? g, string? Default = null) => g is null || !g.Success ? Default : g.Value;

    /// <summary>Возвращает целое значение группы или значение по умолчанию</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Целое значение группы или значение по умолчанию</returns>
    public static int ValueIntOrDefault(this Group? g, int Default = 0) => g is null || !g.Success || !int.TryParse(g.Value, out var v) ? Default : v;

    private static NumberFormatInfo? __CultureRU;

    /// <summary>Преобразует значение группы в double, учитывая универсальные форматы</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение double или значение по умолчанию</returns>
    public static double ValueDoubleOrDefaultUniversal(this Group? g, double Default = double.NaN) =>
        g is null
        || !g.Success
        || !double.TryParse(g.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
        || !double.TryParse(g.Value, NumberStyles.Float, __CultureRU ??= new() { NumberDecimalSeparator = "," }, out v)
            ? Default
            : v;

    /// <summary>Преобразует значение группы в double с использованием инвариантной культуры</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение double или значение по умолчанию</returns>
    public static double ValueDoubleOrDefaultInvariant(this Group? g, double Default = double.NaN) =>
        g is null
        || !g.Success
        || !double.TryParse(g.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
            ? Default
            : v;

    /// <summary>Преобразует значение группы в double</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение double или значение по умолчанию</returns>
    public static double ValueDoubleOrDefault(this Group? g, double Default = double.NaN) =>
        g is null
        || !g.Success
        || !double.TryParse(g.Value, out var v)
            ? Default
            : v;

    /// <summary>Преобразует значение группы в double с заданным форматом</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="format">Формат</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение double или значение по умолчанию</returns>
    public static double ValueDoubleOrDefault(this Group? g, IFormatProvider format, NumberStyles style = NumberStyles.Float, double Default = double.NaN) =>
        g is null ||
        !g.Success ||
        !double.TryParse(g.Value, style, format, out var v)
            ? Default
            : v;

    /// <summary>Преобразует значение группы в bool или возвращает значение по умолчанию</summary>
    /// <param name="g">Группа совпадения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение bool или значение по умолчанию</returns>
    public static bool ValueBoolOrDefault(this Group? g, bool Default = false) => g is null || !g.Success || !bool.TryParse(g.Value, out var v) ? Default : v;

    /// <summary>Возвращает все совпадения по регулярному выражению</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="regex">Регулярное выражение</param>
    /// <returns>Перечисление совпадений</returns>
    public static IEnumerable<Match> FindAllRegExMatch(this string str, Regex regex) => regex.Matches(str).Cast<Match>();

    /// <summary>Возвращает все совпадения по строковому шаблону регулярного выражения</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="RegexStr">Шаблон регулярного выражения</param>
    /// <returns>Перечисление совпадений</returns>
    public static IEnumerable<Match> FindAllRegExMatch(this string str, [RegexPattern] string RegexStr) => Regex.Matches(str, RegexStr).Cast<Match>();

    /// <summary>Проверяет соответствие строки регулярному выражению</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="RegexStr">Шаблон регулярного выражения</param>
    /// <returns>True, если строка соответствует шаблону</returns>
    public static bool EqualsRegex(this string str, [RegexPattern] string RegexStr) => Regex.IsMatch(str, RegexStr);

    /// <summary>Проверяет соответствие строки регулярному выражению с опциями</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="RegexStr">Шаблон регулярного выражения</param>
    /// <param name="opt">Опции регулярного выражения</param>
    /// <returns>True, если строка соответствует шаблону</returns>
    public static bool EqualsRegex(this string str, [RegexPattern] string RegexStr, RegexOptions opt) => Regex.IsMatch(str, RegexStr, opt);
}