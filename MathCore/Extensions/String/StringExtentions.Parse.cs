#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

using MathCore.Annotations;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class StringExtensionsParse
{
    /// <summary>Проверяет, можно ли преобразовать строку в значение типа byte</summary>
    /// <param name="s">Строка для проверки</param>
    /// <returns>True, если преобразование возможно</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt8(this string? s) => byte.TryParse(s, out _);

    /// <summary>Проверяет, можно ли преобразовать строку в значение типа short</summary>
    /// <param name="s">Строка для проверки</param>
    /// <returns>True, если преобразование возможно</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt16(this string? s) => short.TryParse(s, out _);

    /// <summary>Проверяет, можно ли преобразовать строку в значение типа int</summary>
    /// <param name="s">Строка для проверки</param>
    /// <returns>True, если преобразование возможно</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt32(this string? s) => int.TryParse(s, out _);

    /// <summary>Проверяет, можно ли преобразовать строку в значение типа long</summary>
    /// <param name="s">Строка для проверки</param>
    /// <returns>True, если преобразование возможно</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt64(this string? s) => long.TryParse(s, out _);

    /// <summary>Проверяет, можно ли преобразовать строку в значение типа short</summary>
    /// <param name="s">Строка для проверки</param>
    /// <returns>True, если преобразование возможно</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this string? s) => short.TryParse(s, out _);

    /// <summary>Преобразует строку в byte, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte AsInt8(this string? s, byte Default) =>
        byte.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в byte, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static byte? AsInt8(this string? s, byte? Default = null) =>
        byte.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в short, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short AsInt16(this string? s, short Default) =>
        short.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в short, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static short? AsInt16(this string? s, short? Default = null) =>
        short.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в int, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsInt32(this string? s, int Default) =>
        int.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в int, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static int? AsInt32(this string? s, int? Default = null) =>
        int.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в long, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long AsInt64(this string? s, long Default) =>
        long.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в long, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static long? AsInt64(this string? s, long? Default = null) =>
        long.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в float, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AsSingle(this string? s, float Default) =>
        float.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в float, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static float? AsSingle(this string? s, float? Default = null) =>
        float.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в double, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double AsDouble(this string? s, double Default) =>
        double.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в double, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static double? AsDouble(this string s, double? Default = null) =>
        double.TryParse(s, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в double с использованием заданного формата, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Format">Формат числового значения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double AsDouble(this string? s, NumberFormatInfo Format, double Default) =>
        double.TryParse(s, NumberStyles.Any, Format, out var value)
            ? value
            : Default;

    /// <summary>Преобразует строку в double с использованием заданного формата, либо возвращает значение по умолчанию</summary>
    /// <param name="s">Строка для преобразования</param>
    /// <param name="Format">Формат числового значения</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Преобразованное значение или Default</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(Default))]
    public static double? AsDouble(this string? s, NumberFormatInfo Format, double? Default = null) =>
        double.TryParse(s, NumberStyles.Any, Format, out var value)
            ? value
            : Default;

    /// <summary>Удаляет заданное количество символов с начала и конца строки</summary>
    /// <param name="S">Исходная строка</param>
    /// <param name="BeginCount">Количество символов для удаления с начала</param>
    /// <param name="EndCount">Количество символов для удаления с конца</param>
    /// <returns>Результирующая строка</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromBeginEnd(this string? S, int BeginCount, int EndCount) => S?.Remove(0, BeginCount).RemoveFromEnd(EndCount);

    /// <summary>Удаляет заданное количество символов с конца строки</summary>
    /// <param name="S">Исходная строка</param>
    /// <param name="count">Количество символов для удаления</param>
    /// <returns>Результирующая строка</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromEnd(this string? S, int count) => S?.Remove(S.Length - count, count);

    /// <summary>Удаляет заданное количество символов с конца строки начиная с указанной позиции</summary>
    /// <param name="S">Исходная строка</param>
    /// <param name="StartPos">Позиция, с которой начинается удаление</param>
    /// <param name="count">Количество символов для удаления</param>
    /// <returns>Результирующая строка</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(S))]
    public static string? RemoveFromEnd(this string? S, int StartPos, int count) => S?.Remove(S.Length - StartPos, count);
}