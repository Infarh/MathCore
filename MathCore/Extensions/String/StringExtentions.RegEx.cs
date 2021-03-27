using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace System
{
    public static class StringExtensionsRegEx
    {
        public static string FindRegEx([NotNull] this string str, [NotNull] Regex regex, string Default = "") => regex.Match(str).OrDefault(Default);

        [NotNull] public static IEnumerable<Match> FindAllRegExMatch([NotNull] this string str, [NotNull] Regex regex) => regex.Matches(str).Cast<Match>();

        [NotNull] public static IEnumerable<string> FindAllRegEx([NotNull] this string str, [NotNull] Regex regex) => str.FindAllRegExMatch(regex).Select(m => m.Value);
    }
}

namespace System.Text.RegularExpressions
{
}