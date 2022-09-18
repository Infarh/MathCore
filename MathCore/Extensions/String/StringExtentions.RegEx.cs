#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class StringExtensionsRegEx
{
    public static string FindRegEx(this string str, Regex regex, string Default = "") => regex.Match(str).OrDefault(Default);

    public static IEnumerable<Match> FindAllRegExMatch(this string str, Regex regex) => regex.Matches(str).Cast<Match>();

    public static IEnumerable<string> FindAllRegEx(this string str, Regex regex) => str.FindAllRegExMatch(regex).Select(m => m.Value);
}