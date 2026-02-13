
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
    /// <summary>Устанавливает длину строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="length">Новая длина строки</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder SetLength(this StringBuilder builder, int length)
    {
        builder.Length = length < 0 ? builder.Length + length : length;
        return builder;
    }

    /// <summary>Перечисляет строки в объекте <see cref="StringBuilder"/></summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="SkipEmpty">Пропускать ли пустые строки</param>
    /// <returns>Перечисление строк</returns>
    public static IEnumerable<string> EnumLines(this StringBuilder builder, bool SkipEmpty = false) => builder.ToString().EnumLines(SkipEmpty);

    /// <summary>Перечисляет строки в объекте <see cref="StringBuilder"/> с преобразованием</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="selector">Функция преобразования строки</param>
    /// <param name="SkipEmpty">Пропускать ли пустые строки</param>
    /// <typeparam name="T">Тип результата преобразования</typeparam>
    /// <returns>Перечисление преобразованных строк</returns>
    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    /// <summary>Перечисляет строки в объекте <see cref="StringBuilder"/> с преобразованием и индексом</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="selector">Функция преобразования строки с индексом</param>
    /// <param name="SkipEmpty">Пропускать ли пустые строки</param>
    /// <typeparam name="T">Тип результата преобразования</typeparam>
    /// <returns>Перечисление преобразованных строк</returns>
    public static IEnumerable<T> EnumLines<T>(this StringBuilder builder, Func<string, int, T> selector, bool SkipEmpty = false) => builder.ToString().EnumLines(selector, SkipEmpty);

    /// <summary>Создает объект чтения строк</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <returns>Объект <see cref="StringReader"/></returns>
    public static StringReader CreateReader(this StringBuilder str) => new(str.NotNull().ToString());

    /// <summary>Создает объект записи строк</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <returns>Объект <see cref="StringWriter"/></returns>
    public static StringWriter CreateWriter(this StringBuilder builder) => new(builder.NotNull());

    /// <summary>Добавляет форматированную строку</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => builder.AppendFormat(Format, arg0);

    /// <summary>Добавляет форматированную строку</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1);

    /// <summary>Добавляет форматированную строку</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <param name="arg2">Третий аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2);

    /// <summary>Добавляет форматированную строку</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="args">Аргументы форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => builder.AppendFormat(Format, args);

    /// <summary>Добавляет перевод строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder LN(this StringBuilder builder) => builder.AppendLine();

    /// <summary>Добавляет строку с переводом строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="str">Добавляемая строка</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder LN(this StringBuilder builder, string str) => builder.AppendLine(str);

    /// <summary>Добавляет форматированную строку и перевод строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => builder.AppendFormat(Format, arg0).LN();

    /// <summary>Добавляет форматированную строку и перевод строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => builder.AppendFormat(Format, arg0, arg1).LN();

    /// <summary>Добавляет форматированную строку и перевод строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <param name="arg2">Третий аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => builder.AppendFormat(Format, arg0, arg1, arg2).LN();

    /// <summary>Добавляет форматированную строку и перевод строки</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="Format">Формат строки</param>
    /// <param name="args">Аргументы форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => builder.AppendFormat(Format, args).LN();

    /// <summary>Добавляет перевод строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder LN(this StringBuilder builder, bool If) => If ? builder.AppendLine() : builder;

    /// <summary>Добавляет строку с переводом строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="str">Добавляемая строка</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder LN(this StringBuilder builder, bool If, string str) => If ? builder.AppendLine(str) : builder;

    /// <summary>Добавляет форматированную строку и перевод строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0) => If ? builder.AppendFormat(Format, arg0).LN() : builder;

    /// <summary>Добавляет форматированную строку и перевод строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => If ? builder.AppendFormat(Format, arg0, arg1).LN() : builder;

    /// <summary>Добавляет форматированную строку и перевод строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <param name="arg2">Третий аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => If ? builder.AppendFormat(Format, arg0, arg1, arg2).LN() : builder;

    /// <summary>Добавляет форматированную строку и перевод строки в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="args">Аргументы форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder LN(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => If ? builder.AppendFormat(Format, args).LN() : builder;

    /// <summary>Добавляет строку в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Value">Добавляемая строка</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder Append(this StringBuilder builder, bool If, string Value) => If ? builder.Append(Value) : builder;

    /// <summary>Добавляет символ в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Value">Добавляемый символ</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder Append(this StringBuilder builder, bool If, char Value) => If ? builder.Append(Value) : builder;

    /// <summary>Добавляет форматированную строку в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg">Аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg) => !If ? builder : builder.AppendFormat(Format, arg);

    /// <summary>Добавляет форматированную строку в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1) => If ? builder.AppendFormat(Format, arg0, arg1) : builder;

    /// <summary>Добавляет форматированную строку в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="arg0">Первый аргумент форматирования</param>
    /// <param name="arg1">Второй аргумент форматирования</param>
    /// <param name="arg2">Третий аргумент форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, object arg0, object arg1, object arg2) => If ? builder.AppendFormat(Format, arg0, arg1, arg2) : builder;

    /// <summary>Добавляет форматированную строку в зависимости от условия</summary>
    /// <param name="builder">Объект <see cref="StringBuilder"/></param>
    /// <param name="If">Условие добавления</param>
    /// <param name="Format">Формат строки</param>
    /// <param name="args">Аргументы форматирования</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    [StringFormatMethod("Format")]
    public static StringBuilder Append(this StringBuilder builder, bool If, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string Format, params object[] args) => If ? builder.AppendFormat(Format, args) : builder;

#if NET8_0_OR_GREATER

    public static StringBuilder LN(this StringBuilder builder, ref StringBuilder.AppendInterpolatedStringHandler handler) => builder.Append(ref handler).LN();

    public static StringBuilder LN(this StringBuilder builder, bool If, ref StringBuilder.AppendInterpolatedStringHandler handler) => If ? builder.Append(ref handler).LN() : builder;

    public static StringBuilder Append(this StringBuilder builder, ref StringBuilder.AppendInterpolatedStringHandler handler) => builder.Append(ref handler);

    public static StringBuilder Append(this StringBuilder builder, bool If, ref StringBuilder.AppendInterpolatedStringHandler handler) => If ? builder.Append(ref handler) : builder;

#endif

    /// <summary>Проверяет начинается ли строка с заданного префикса</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <param name="start">Префикс</param>
    /// <param name="comparison">Тип сравнения</param>
    /// <returns>Истина, если строка начинается с префикса</returns>
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
                for (var i = 0; i < start_len; i++)
                    if (start[i] != str[i])
                        return false;
                return true;

        }
    }

    /// <summary>Удаляет начальную часть строки, если она совпадает с заданной</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <param name="start">Удаляемая часть</param>
    /// <param name="comparison">Тип сравнения</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder TrimStart(this StringBuilder str, string? start, StringComparison comparison = StringComparison.Ordinal)
    {
        if (start is not { Length: > 0 and var len })
            return str;

        if (str.StartWith(start, comparison))
            str.Remove(0, len);

        return str;
    }

    /// <summary>Проверяет заканчивается ли строка заданным суффиксом</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <param name="end">Суффикс</param>
    /// <param name="comparison">Тип сравнения</param>
    /// <returns>Истина, если строка заканчивается на суффикс</returns>
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

    /// <summary>Удаляет конечную часть строки, если она совпадает с заданной</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <param name="end">Удаляемая часть</param>
    /// <param name="comparison">Тип сравнения</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder TrimEnd(this StringBuilder str, string? end, StringComparison comparison = StringComparison.Ordinal)
    {
        if (end is not { Length: > 0 and var len })
            return str;

        if (str.EndWith(end, comparison))
            str.Length -= len;

        return str;
    }

    /// <summary>Убедиться что строка начинается с указанного префикса</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <param name="start">Искомый префикс</param>
    /// <param name="comparison">Вариант сравнения строк</param>
    /// <returns>Строка, начинающаяся с указанного прфикса</returns>
    public static StringBuilder EnsureStartWith(this StringBuilder str, string? start, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str.StartWith(start, comparison))
            return str;

        if (start is { Length: > 0 })
            str.Insert(0, start);

        return str;
    }

    /// <summary>Убедиться что строка заканчивается указанным суффиксом</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <param name="s">Искомый суффикс</param>
    /// <param name="comparison">Вариант сравнения строк</param>
    /// <returns>Строка, завершающаяся указанным суффиксом</returns>
    public static StringBuilder EnsureEndWith(this StringBuilder str, string? s, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str.EndWith(s, comparison))
            return str;

        if (s is { Length: > 0 })
            str.Append(s);

        return str;
    }

    /// <summary>Удаляет начальную и конечную часть строки, если они совпадают с заданной</summary>
    /// <param name="str">Объект <see cref="StringBuilder"/></param>
    /// <param name="end">Удаляемая часть</param>
    /// <param name="comparison">Тип сравнения</param>
    /// <returns>Объект <see cref="StringBuilder"/></returns>
    public static StringBuilder Trim(this StringBuilder str, string s, StringComparison comparison = StringComparison.Ordinal) => str.TrimStart(s, comparison).TrimEnd(s, comparison);

#if NET8_0_OR_GREATER

    public static string ToString(this StringBuilder str, Range range)
    {
        var (start, end) = range.ToIndexes(str.Length);
        return str.ToString(start, end - start);
    }

#endif
}