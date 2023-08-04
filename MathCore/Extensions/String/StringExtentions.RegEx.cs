#nullable enable
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class StringExtensionsRegEx
{
    public static string FindRegEx(this string str, Regex regex, string Default = "") => regex.Match(str).OrDefault(Default);
    public static string FindRegEx(this string str, [RegexPattern] string RegexStr, string Default = "") => Regex.Match(str, RegexStr).OrDefault(Default);

    public static IEnumerable<Match> FindAllRegExMatch(this string str, Regex regex) => regex.Matches(str).Cast<Match>();
    public static IEnumerable<Match> FindAllRegExMatch(this string str, [RegexPattern] string RegexStr) => Regex.Matches(str, RegexStr).Cast<Match>();

    public static IEnumerable<string> FindAllRegEx(this string str, Regex regex) => str.FindAllRegExMatch(regex).Select(m => m.Value);
    public static IEnumerable<string> FindAllRegEx(this string str, [RegexPattern] string RegexStr) => str.FindAllRegExMatch(RegexStr).Select(m => m.Value);

    public static bool EqualsRegex(this string str, [RegexPattern] string RegexStr) => Regex.IsMatch(str, RegexStr);

    public static bool EqualsRegex(this string str, [RegexPattern] string RegexStr, RegexOptions opt) => Regex.IsMatch(str, RegexStr, opt);

}