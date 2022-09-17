﻿#nullable enable
using System.IO;
using System.Collections.Generic;

using MathCore.Annotations;

// ReSharper disable CheckNamespace

namespace System.Text;

public static class StringBuilderExtensions
{
    public static void Test(this StringBuilder builder)
    {
        if(builder[0] == 'q')
            Console.WriteLine();
    }

    public static IEnumerable<string> EnumLines(this StringBuilder builder, bool SkipEmpty = false) => builder.ToString().EnumLines(SkipEmpty);

    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, int, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    /// <summary>Создать объект чтения</summary>
    /// <param name="str">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект чтения строк</returns>
    public static StringReader CreateReader(this StringBuilder str) => new((str ?? throw new ArgumentNullException(nameof(str))).ToString());

    /// <summary>Создать объект записи строк</summary>
    /// <param name="builder">Исходный объект <see cref="StringBuilder"/></param>
    /// <returns>Объект записи строк</returns>
    public static StringWriter CreateWriter(this StringBuilder builder) =>  new(builder ?? throw new ArgumentNullException(nameof(builder)));

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0) => builder.AppendFormat(Format, arg0);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2);

    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, string Format, params object[] args) => builder.AppendFormat(Format, args);

    public static StringBuilder LN(this StringBuilder builder) => builder.AppendLine();

    public static StringBuilder LN(this StringBuilder builder, string str) => builder.AppendLine(str);
}