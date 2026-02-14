
using System.Text;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Ограничение длины строки с удалением центральной её части</summary>
public static class StringExtensionsTrim
{
    /// <summary>Ограничение длины строки с удалением центральной её части</summary>
    /// <param name="Str">Обрезаемая строка</param>
    /// <param name="Length">Требуемая длина</param>
    /// <param name="ReplacementPattern">Шаблон замены</param>
    /// <returns>Строка с удалённой внутренней частью</returns>
    public static string TrimByLength(this string Str, int Length, string ReplacementPattern = "..")
    {
        if (Str is null) throw new ArgumentNullException(nameof(Str));

        if (Str.Length <= Length) return Str;
        if (Length == 0) return string.Empty;

        var dl1 = Str.Length - Length + ReplacementPattern.Length;
        var dl2 = dl1 / 2;
        dl1 -= dl2;

        var str = Str.AsStringPtr();
        var s1 = str.Substring(0, Str.Length / 2 - dl1);
        var start = Str.Length / 2 + dl2;
        var len = Str.Length - Str.Length / 2 - dl2;
        var s2 = str.Substring(start, len);

        return new StringBuilder(s1.Length + ReplacementPattern.Length + s2.Length)
           .Append(s1)
           .Append(ReplacementPattern)
           .Append(s2)
           .ToString();
    }

    /// <summary>Обрезает заданный префикс-строку</summary>
    /// <remarks>
    /// Если строка начинается с s с учётом Comparison, префикс удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="s">Удаляемая подстрока в начале</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimStart(this string str, string s, StringComparison Comparison = StringComparison.Ordinal) => str.StartsWith(s, Comparison) ? str[s.Length..] : str;

    /// <summary>Обрезает заданный суффикс-строку</summary>
    /// <remarks>
    /// Если строка кончается s с учётом Comparison, суффикс удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="s">Удаляемая подстрока в конце</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimEnd(this string str, string s, StringComparison Comparison = StringComparison.Ordinal) => str.EndsWith(s, Comparison) ? str[..^s.Length] : str;

    /// <summary>Обрезает заданную подстроку в начале и в конце</summary>
    /// <remarks>
    /// Выполняет TrimStart и TrimEnd для подстроки s с учётом Comparison
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="s">Удаляемая подстрока</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string Trim(this string str, string s, StringComparison Comparison = StringComparison.Ordinal) => str.TrimStart(s, Comparison).TrimEnd(s, Comparison);

#if NET8_0_OR_GREATER

    /// <summary>Обрезает заданный символ в начале</summary>
    /// <remarks>
    /// Если строка начинается с символа с учётом Comparison, символ удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Удаляемый символ в начале</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimStart(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) => str.AsSpan().StartsWith([c], Comparison) ? str[1..] : str;

    /// <summary>Обрезает заданный символ в конце</summary>
    /// <remarks>
    /// Если строка кончается символом с учётом Comparison, символ удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Удаляемый символ в конце</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimEnd(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) => str.AsSpan().EndsWith([c], Comparison) ? str[..^1] : str;

#else

    /// <summary>Обрезает заданный символ в начале</summary>
    /// <remarks>
    /// Если строка начинается с символа с учётом Comparison, символ удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Удаляемый символ в начале</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimStart(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) =>
        str.Length == 0
        ? str
        : Comparison switch
        {
            StringComparison.Ordinal => str[0] == c ? str[1..] : str,
            StringComparison.OrdinalIgnoreCase => char.ToUpperInvariant(str[0]) == char.ToUpperInvariant(c) ? str[1..] : str,
            StringComparison.CurrentCulture => char.ToString(str[0]).Equals(char.ToString(c), Comparison) ? str[1..] : str,
            StringComparison.CurrentCultureIgnoreCase => char.ToString(str[0]).Equals(char.ToString(c), Comparison) ? str[1..] : str,
            StringComparison.InvariantCulture => char.ToString(str[0]).Equals(char.ToString(c), Comparison) ? str[1..] : str,
            StringComparison.InvariantCultureIgnoreCase => char.ToString(str[0]).Equals(char.ToString(c), Comparison) ? str[1..] : str,
            _ => throw new ArgumentOutOfRangeException(nameof(Comparison), Comparison, "Недопустимый тип сравнения"),
        };

    /// <summary>Обрезает заданный символ в конце</summary>
    /// <remarks>
    /// Если строка кончается символом с учётом Comparison, символ удаляется
    /// Иначе возвращается исходная строка
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Удаляемый символ в конце</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string TrimEnd(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) =>
        //str.AsSpan().EndsWith([c], Comparison) ? str[..^1] : str;
        str.Length == 0
        ? str
        : Comparison switch
        {
            StringComparison.Ordinal => str[^1] == c ? str[..^1] : str,
            StringComparison.OrdinalIgnoreCase => char.ToUpperInvariant(str[^1]) == char.ToUpperInvariant(c) ? str[..^1] : str,
            StringComparison.CurrentCulture => char.ToString(str[^1]).Equals(char.ToString(c), Comparison) ? str[..^1] : str,
            StringComparison.CurrentCultureIgnoreCase => char.ToString(str[^1]).Equals(char.ToString(c), Comparison) ? str[..^1] : str,
            StringComparison.InvariantCulture => char.ToString(str[^1]).Equals(char.ToString(c), Comparison) ? str[..^1] : str,
            StringComparison.InvariantCultureIgnoreCase => char.ToString(str[^1]).Equals(char.ToString(c), Comparison) ? str[..^1] : str,
            _ => throw new ArgumentOutOfRangeException(nameof(Comparison), Comparison, "Недопустимый тип сравнения"),
        };

#endif

    /// <summary>Обрезает заданный символ в начале и в конце</summary>
    /// <remarks>
    /// Выполняет TrimStart и TrimEnd для символа c с учётом Comparison
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Удаляемый символ</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string Trim(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) => str.TrimStart(c, Comparison).TrimEnd(c, Comparison);
}