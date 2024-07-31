#nullable enable

using MathCore.Annotations;

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#else
using MathCore.Attributes;
#endif

// ReSharper disable CheckNamespace

namespace System.Text;

public static class StringBuilderExtensions
{
    public static StringBuilder SetLength(this StringBuilder builder, int length)
    {
        builder.Length = length < 0 ? builder.Length + length : length;
        return builder;
    }

    public static IEnumerable<string> EnumLines(this StringBuilder builder, bool SkipEmpty = false) => builder.ToString().EnumLines(SkipEmpty);

    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, int, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    /// <summary>Создать объект чтения</summary>
    /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект чтения строк</returns>
    public static StringReader CreateReader(this StringBuilder str) => new((str.NotNull()).ToString());

    /// <summary>Создать объект записи строк</summary>
    /// <param name="builder">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект записи строк</returns>
    public static StringWriter CreateWriter(this StringBuilder builder) => new(builder.NotNull());

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => builder.AppendFormat(Format, arg0);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => builder.AppendFormat(Format, args);

    public static StringBuilder LN(this StringBuilder builder) => builder.AppendLine();

    public static StringBuilder LN(this StringBuilder builder, string str) => builder.AppendLine(str);

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => builder.AppendFormat(Format, arg0).LN();

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1).LN();

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2).LN();

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => builder.AppendFormat(Format, args).LN();

    public static StringBuilder LN(this StringBuilder builder, bool If) => If ? builder.AppendLine() : builder;

    public static StringBuilder LN(this StringBuilder builder, bool If, string str) => If ? builder.AppendLine(str) : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => If ? builder.AppendFormat(Format, arg0).LN() : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => If ? builder.AppendFormat(Format, arg0, arg1).LN() : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => If ? builder.AppendFormat(Format, arg0, arg1, arg2).LN() : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => If ? builder.AppendFormat(Format, args).LN() : builder;

    public static StringBuilder Append(this StringBuilder builder, bool If, string Value) => If ? builder.Append(Value) : builder;

    public static StringBuilder Append(this StringBuilder builder, bool If, char Value) => If ? builder.Append(Value) : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg) => !If ? builder : builder.AppendFormat(Format, arg);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => If ? builder.AppendFormat(Format, arg0, arg1) : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => If ? builder.AppendFormat(Format, arg0, arg1, arg2) : builder;

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => If ? builder.AppendFormat(Format, args) : builder;

#if NET8_0_OR_GREATER

    public static StringBuilder LN(this StringBuilder builder, ref StringBuilder.AppendInterpolatedStringHandler handler) => builder.Append(ref handler).LN();
    public static StringBuilder LN(this StringBuilder builder, bool If, ref StringBuilder.AppendInterpolatedStringHandler handler) => If ? builder.Append(ref handler).LN() : builder;

    public static StringBuilder Append(this StringBuilder builder, ref StringBuilder.AppendInterpolatedStringHandler handler) => builder.Append(ref handler);
    public static StringBuilder Append(this StringBuilder builder, bool If, ref StringBuilder.AppendInterpolatedStringHandler handler) => If ? builder.Append(ref handler) : builder;

#endif

    public static bool StartWith(this StringBuilder str, string start, StringComparison comparison = StringComparison.Ordinal)
    {
        if (start is not { Length: > 0 and var start_len })
            return true;

        var str_len = str.Length;
        if (str_len < start_len) return false;

        switch (comparison)
        {
            case StringComparison.CurrentCultureIgnoreCase:
            case StringComparison.OrdinalIgnoreCase:
                for (var i = 0; i < start_len; i++)
                    if (char.ToUpper(start[i]) != char.ToUpper(str[i]))
                        return false;
                return true;

            case StringComparison.InvariantCultureIgnoreCase:
                for (var i = 0; i < start_len; i++)
                    if (char.ToUpperInvariant(start[i]) != char.ToUpperInvariant(str[i]))
                        return false;
                return true;

            default:
                for(var i = 0; i < start_len; i++)
                    if (start[i] != str[i])
                        return false;
                return true;

        }
    }

    public static StringBuilder TrimStart(this StringBuilder str, string? start, StringComparison comparison = StringComparison.Ordinal)
    {
        if (start is not { Length: > 0 and var len })
            return str;

        if (str.StartWith(start, comparison))
            str.Remove(0, len);

        return str;
    }

    public static bool EndWith(this StringBuilder str, string end, StringComparison comparison = StringComparison.Ordinal)
    {
        if (end is not { Length: > 0 and var end_len })
            return true;

        var str_len = str.Length;
        if (str_len < end_len) return false;

        switch (comparison)
        {
            case StringComparison.CurrentCultureIgnoreCase:
            case StringComparison.OrdinalIgnoreCase:
                for (var i = 1; i <= end_len; i++)
                    if (char.ToUpper(end[^i]) != char.ToUpper(str[^i]))
                        return false;
                return true;

            case StringComparison.InvariantCultureIgnoreCase:
                for (var i = 1; i <= end_len; i++)
                    if (char.ToUpperInvariant(end[^i]) != char.ToUpperInvariant(str[^i]))
                        return false;
                return true;

            default:
                for (var i = 1; i <= end_len; i++)
                    if (end[^i] != str[^i])
                        return false;
                return true;
        }
    }

    public static StringBuilder TrimEnd(this StringBuilder str, string? end, StringComparison comparison = StringComparison.Ordinal)
    {
        if (end is not { Length: > 0 and var len })
            return str;

        if (str.EndWith(end, comparison))
            str.Length -= len;

        return str;
    }
}