using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtentionsRegEx
    {
        public static string FindRegEx(this string str, Regex regex, string Default = "")
        {
            return regex.Match(str).OrDefault(Default);
        }

        public static IEnumerable<Match> FindAllRegExMatch(this string str, Regex regex)
        {
            return regex.Matches(str).Cast<Match>();
        }

        public static IEnumerable<string> FindAllRegEx(this string str, Regex regex)
        {
            return str.FindAllRegExMatch(regex).Select(m => m.Value);
        }
    }
}

namespace System.Text.RegularExpressions
{
    public static class RegExExtentions
    {
        /// <summary>Метод получения значения из результатов совпадения поиска</summary>
        /// <param name="match">Результат совпадения поиска</param>
        /// <param name="DefaultString">Значение по умолчанию</param>
        /// <returns>Результат поска, либо значение по умолчанию</returns>
        public static string OrDefault(this Match match, string DefaultString = "") => match.Success ? match.Value : DefaultString;

        public static int OrDefault(this Match match, int DefaultInt)
        {
            int v; return int.TryParse(match.OrDefault(), out v) ? v : DefaultInt;
        }

        public static string OrDefault(this Group group, string DefaultString = "") => @group.Success ? @group.Value : DefaultString;

        public static int OrDefault(this Group group, int DefaultInt)
        {
            int v; return int.TryParse(@group.OrDefault(), out v) ? v : DefaultInt;
        }

        public static Match Find(this string Str, Regex regex) => regex.Match(Str);

        public static Match FindRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Match(Str, Pattern);

        public static MatchCollection FindAllRegEx(this string Str, [RegexPattern] string Pattern) => Regex.Matches(Str, Pattern);

        public static string FindRegEx(this string Str, [RegexPattern] string Pattern, string Default) => Regex.Match(Str, Pattern).OrDefault(Default);

        public static int FindRegEx(this string Str, [RegexPattern] string Pattern, int DefaultValue)
        {
            int v; return int.TryParse(Str.FindRegEx(Pattern, ""), out v) ? v : DefaultValue;
        }

        public static string ValueOrDefault(this Group g, string Default = null) => g == null || !g.Success ? Default : g.Value;

        public static int ValueIntOrDefault(this Group g, int Default = 0)
        {
            int v; return g == null || !g.Success || !int.TryParse(g.Value, out v) ? Default : v;
        }

        public static double ValuedDoubleOrDefault(this Group g, double Default = double.NaN)
        {
            double v; return g == null || !g.Success || !double.TryParse(g.Value, out v) ? Default : v;
        }

        public static double ValuedDoubleOrDefault(this Group g, IFormatProvider format, NumberStyles style = NumberStyles.Float, double Default = double.NaN)
        {
            double v; return g == null || !g.Success || !double.TryParse(g.Value, NumberStyles.Float, format, out v) ? Default : v;
        }

        public static bool ValueBoolOrDefault(this Group g, bool Default = false)
        {
            bool v; return g == null || !g.Success || !bool.TryParse(g.Value, out v) ? Default : v;
        }
    }
}