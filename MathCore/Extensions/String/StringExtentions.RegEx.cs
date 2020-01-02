using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MathCore.Annotations;

namespace System
{
    public static class StringExtensionsRegEx
    {
        public static string FindRegEx([NotNull] this string str, [NotNull] Regex regex, string Default = "") => regex.Match(str).OrDefault(Default);

        [NotNull] public static IEnumerable<Match> FindAllRegExMatch([NotNull] this string str, [NotNull] Regex regex) => regex.Matches(str).Cast<Match>();

        [NotNull] public static IEnumerable<string> FindAllRegEx([NotNull] this string str, [NotNull] Regex regex) => str.FindAllRegExMatch(regex).Select(m => m.Value);
    }
}namespace System.Text.RegularExpressions
{
    public static class RegExExtensions
    {
        /// <summary>Метод получения значения из результатов совпадения поиска</summary>
        /// <param name="match">Результат совпадения поиска</param>
        /// <param name="DefaultString">Значение по умолчанию</param>
        /// <returns>Результат поиска, либо значение по умолчанию</returns>
        public static string OrDefault([NotNull] this Match match, string DefaultString = "") => match.Success ? match.Value : DefaultString;

        public static int OrDefault([NotNull] this Match match, int DefaultInt) => int.TryParse(match.OrDefault(), out var v) ? v : DefaultInt;

        public static string OrDefault([NotNull] this Group group, string DefaultString = "") => group.Success ? group.Value : DefaultString;

        public static int OrDefault([NotNull] this Group group, int DefaultInt) => int.TryParse(group.OrDefault(), out var v) ? v : DefaultInt;

        [NotNull] public static Match Find([NotNull] this string Str, [NotNull] Regex regex) => regex.Match(Str);

        [NotNull] public static Match FindRegEx([NotNull] this string Str, [RegexPattern] [NotNull] string Pattern) => Regex.Match(Str, Pattern);

        [NotNull] public static MatchCollection FindAllRegEx([NotNull] this string Str, [RegexPattern] [NotNull] string Pattern) => Regex.Matches(Str, Pattern);

        public static string FindRegEx([NotNull] this string Str, [RegexPattern] [NotNull] string Pattern, string Default) => Regex.Match(Str, Pattern).OrDefault(Default);

        public static int FindRegEx([NotNull] this string Str, [RegexPattern] [NotNull] string Pattern, int DefaultValue) => int.TryParse(Str.FindRegEx(Pattern, ""), out var v) ? v : DefaultValue;

        [CanBeNull] public static string ValueOrDefault([CanBeNull] this Group g, [CanBeNull] string Default = null) => g is null || !g.Success ? Default : g.Value;

        public static int ValueIntOrDefault([CanBeNull] this Group g, int Default = 0) => g is null || !g.Success || !int.TryParse(g.Value, out var v) ? Default : v;

        public static double ValuedDoubleOrDefault([CanBeNull] this Group g, double Default = double.NaN) => g is null || !g.Success || !double.TryParse(g.Value, out var v) ? Default : v;

        public static double ValuedDoubleOrDefault([CanBeNull] this Group g, IFormatProvider format, NumberStyles style = NumberStyles.Float, double Default = double.NaN) => g is null || !g.Success || !double.TryParse(g.Value, NumberStyles.Float, format, out var v) ? Default : v;

        public static bool ValueBoolOrDefault([CanBeNull] this Group g, bool Default = false) => g is null || !g.Success || !bool.TryParse(g.Value, out var v) ? Default : v;
    }
}