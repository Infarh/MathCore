#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class StringExtensionsParse
{
    [DST]
    public static bool IsInt8(this string? s) => byte.TryParse(s, out _);

    [DST]
    public static bool IsInt16(this string? s) => short.TryParse(s, out _);

    [DST]
    public static bool IsInt32(this string? s) => int.TryParse(s, out _);

    [DST]
    public static bool IsInt64(this string? s) => long.TryParse(s, out _);

    [DST]
    public static bool IsShort(this string? s) => short.TryParse(s, out _);

    [DST]
    public static bool IsDouble(this string? s) => double.TryParse(s, out _);

    [DST]
    public static byte AsInt8(this string? s, byte Default) =>
        byte.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static byte? AsInt8(this string? s, byte? Default = null) =>
        byte.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static short AsInt16(this string? s, short Default) =>
        short.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static short? AsInt16(this string? s, short? Default = null) =>
        short.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static int AsInt32(this string? s, int Default) =>
        int.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static int? AsInt32(this string? s, int? Default = null) =>
        int.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static long AsInt64(this string? s, long Default) =>
        long.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static long? AsInt64(this string? s, long? Default = null) =>
        long.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static float AsSingle(this string? s, float Default) =>
        float.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static float? AsSingle(this string? s, float? Default = null) =>
        float.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static double AsDouble(this string? s, double Default) =>
        double.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static double? AsDouble(this string s, double? Default = null) =>
        double.TryParse(s, out var value)
            ? value
            : Default;

    [DST]
    public static double AsDouble(this string? s, NumberFormatInfo Format, double Default) =>
        double.TryParse(s, NumberStyles.Any, Format, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(Default))]
    public static double? AsDouble(this string? s, NumberFormatInfo Format, double? Default = null) =>
        double.TryParse(s, NumberStyles.Any, Format, out var value)
            ? value
            : Default;

    [DST]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromBeginEnd(this string? S, int BeginCount, int EndCount) => S?.Remove(0, BeginCount).RemoveFromEnd(EndCount);

    [DST]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromEnd(this string? S, int count) => S?.Remove(S.Length - count, count);

    [DST]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromEnd(this string? S, int StartPos, int count) => S?.Remove(S.Length - StartPos, count);
}