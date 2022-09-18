#nullable enable
using System.Globalization;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Text.RegularExpressions;

public static class RegExExtensions
{
    /// <summary>Метод получения значения из результатов совпадения поиска</summary>
    /// <param name="match">Результат совпадения поиска</param>
    /// <param name="DefaultString">Значение по умолчанию</param>
    /// <returns>Результат поиска, либо значение по умолчанию</returns>
    public static string OrDefault(this Match match, string DefaultString = "") => match.Success ? match.Value : DefaultString;

    public static int OrDefault(this Match match, int DefaultInt) => int.TryParse(match.OrDefault(), out var v) ? v : DefaultInt;

    public static string OrDefault(this Group group, string DefaultString = "") => group.Success ? group.Value : DefaultString;

    public static int OrDefault(this Group group, int DefaultInt) => int.TryParse(group.OrDefault(), out var v) ? v : DefaultInt;

    public static Match Find(this string Str, Regex regex) => regex.Match(Str);

    public static Match FindRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Match(Str, Pattern);

    public static MatchCollection FindAllRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Matches(Str, Pattern);

    public static string FindRegEx(this string Str, [RegexPattern] string Pattern, string Default) => Regex.Match(Str, Pattern).OrDefault(Default);

    public static int FindRegEx(this string Str, [RegexPattern] string Pattern, int DefaultValue) => int.TryParse(Str.FindRegEx(Pattern, string.Empty), out var v) ? v : DefaultValue;

    public static string? ValueOrDefault(this Group? g, string? Default = null) => g is null || !g.Success ? Default : g.Value;

    public static int ValueIntOrDefault(this Group? g, int Default = 0) => g is null || !g.Success || !int.TryParse(g.Value, out var v) ? Default : v;

    public static double ValuedDoubleOrDefault(this Group? g, double Default = double.NaN) => g is null || !g.Success || !double.TryParse(g.Value, out var v) ? Default : v;

    public static double ValuedDoubleOrDefault(this Group? g, IFormatProvider format, NumberStyles style = NumberStyles.Float, double Default = double.NaN) => g is null || !g.Success || !double.TryParse(g.Value, style, format, out var v) ? Default : v;

    public static bool ValueBoolOrDefault(this Group? g, bool Default = false) => g is null || !g.Success || !bool.TryParse(g.Value, out var v) ? Default : v;
}