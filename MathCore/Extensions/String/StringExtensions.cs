#nullable enable
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using MathCore;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Методы-расширения класса <see cref="T:System.String">строк</see></summary>
public static class StringExtensions
{
    /// <summary>Перечисление строк в строке</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление строк в строке</returns>
    public static IEnumerable<string> EnumLines(this string str, bool SkipEmpty = false)
    {
        using var reader = str.CreateReader();
        while (reader.ReadLine() is { } line)
            if (line.Length > 0 || !SkipEmpty)
                yield return line;
    }

    /// <summary>Перечисление строк в строке</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Selector">Преобразователь значения</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление строк в строке</returns>
    public static IEnumerable<T> EnumLines<T>(this string str, Func<string, T> Selector, bool SkipEmpty = false)
    {
        using var reader = str.CreateReader();
        while (reader.ReadLine() is { } line)
            if (line.Length > 0 || !SkipEmpty)
                yield return Selector(line);
    }

    /// <summary>Перечисление строк в строке</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Selector">Преобразователь значения</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление строк в строке</returns>
    public static IEnumerable<T> EnumLines<T>(this string str, Func<string, int, T> Selector, bool SkipEmpty = false)
    {
        using var reader = str.CreateReader();
        var i = 0;
        while (reader.ReadLine() is { } line)
        {
            if (line.Length > 0 || !SkipEmpty)
                yield return Selector(line, i);
            i++;
        }
    }

    /// <summary>Создать объект чтения данных строки</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Объект <see cref="StringReader"/> для чтения данных строки</returns>
    public static StringReader CreateReader(this string str) => new(str);

    /// <summary>Создать построитель строки</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Объект <see cref="StringBuilder"/> для формирования строки</returns>
    public static StringBuilder CreateBuilder(this string str) => new(str);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Указатель на позицию в строке</returns>
    public static StringPtr AsStringPtr(this string str) => new(str, 0, str.Length);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Pos">Положение в строке</param>
    /// <returns>Указатель на позицию в строке</returns>
    public static StringPtr AsStringPtr(this string str, int Pos) => new(str, Pos, str.Length - Pos);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Pos">Положение в строке</param>
    /// <param name="Length">Длина подстроки</param>
    /// <returns>Указатель на позицию в строке</returns>
    public static StringPtr AsStringPtr(this string str, int Pos, int Length) => new(str, Pos, Length);

    /// <summary>Сжать строку в последовательность байт</summary>
    /// <param name="str">Сжимаемая строка</param>
    /// <returns>Сжатая строка в виде последовательности байт</returns>
    public static byte[] Compress(this string str)
    {
        using var output = new MemoryStream();
        using (var compressor = new GZipStream(output, CompressionLevel.Optimal))
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            compressor.Write(bytes, 0, bytes.Length);
        }

        return output.ToArray();
    }

    /// <summary>Сжать строку в последовательность байт</summary>
    /// <param name="str">Сжимаемая строка</param>
    /// <param name="Cancel">Отмена операции</param>
    /// <returns>Сжатая строка в виде последовательности байт</returns>
    public static async Task<byte[]> CompressAsync(this string str, CancellationToken Cancel = default)
    {
        using var output = new MemoryStream();
        using (var compressor = new GZipStream(output, CompressionLevel.Optimal))
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            await compressor.WriteAsync(bytes, 0, bytes.Length, Cancel).ConfigureAwait(false);
        }

        return output.ToArray();
    }

    /// <summary>Разархивировать последовательность байт в строку</summary>
    /// <param name="bytes">Сжатая последовательность бай, содержащая строку</param>
    /// <returns>Распакованная последовательность байт в строковом представлении</returns>
    public static string DecompressAsString(this byte[] bytes)
    {
        using var input_stream = new MemoryStream(bytes);
        using var output_stream = new MemoryStream();
        using var g_zip_stream = new GZipStream(input_stream, CompressionMode.Decompress);
        g_zip_stream.CopyTo(output_stream);

        return Encoding.UTF8.GetString(output_stream.ToArray());
    }

    /// <summary>Разархивировать последовательность байт в строку</summary>
    /// <param name="bytes">Сжатая последовательность бай, содержащая строку</param>
    /// <param name="Cancel">Отмена операции</param>
    /// <returns>Распакованная последовательность байт в строковом представлении</returns>
    public static async Task<string> DecompressAsStringAsync(this byte[] bytes, CancellationToken Cancel = default)
    {
        using var input_stream = new MemoryStream(bytes);
        using var output_stream = new MemoryStream();
        using var g_zip_stream = new GZipStream(input_stream, CompressionMode.Decompress);
        await g_zip_stream.CopyToAsync(output_stream, 102400, Cancel).ConfigureAwait(false);

        return Encoding.UTF8.GetString(output_stream.ToArray());
    }

    public static string JoinStrings(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    public static byte[] ComputeSHA256(this string text, Encoding? encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeSHA256();

    public static byte[] ComputeMD5(this string text, Encoding? encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeMD5();

    /// <summary>Перечисление подстрок, разделяемых указанным строковым шаблоном</summary>
    /// <param name="Str">Разбиваемая строка</param>
    /// <param name="EndPattern">Строковый шаблон разбиения</param>
    /// <returns>Перечисление подстрок</returns>
    public static IEnumerable<string> FindBlock(this string? Str, string EndPattern)
    {
        if (Str is not { Length: > 0 }) yield break;
        var len = Str.Length;
        var pattern_len = EndPattern.Length;
        var pos = 0;
        do
        {
            var index = Str.IndexOf(EndPattern, StringComparison.Ordinal);
            yield return Str.Substring(pos, index - pos + pattern_len);
            pos = index + pattern_len + 1;
        } while (pos < len);
    }

    /// <summary>Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения</summary>
    /// <param name="Str">Входная строка</param>
    /// <param name="Offset">Смещение во входной строке начала поиска - в конце работы метода соответствует месту окончания поиска</param>
    /// <param name="Open">Шаблон начала подстроки</param>
    /// <param name="Close">Шаблон окончания подстроки</param>
    /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
    /// <exception cref="FormatException">
    /// Если шаблон завершения строки на найден, либо если количество шаблонов начала строки превышает 
    /// количество шаблонов окончания во входной строке
    /// </exception>
    public static string? GetBracketText(this string Str, ref int Offset, string Open = "(", string Close = ")")
    {
        var start_index = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
        if (start_index == -1) return null;
        var stop_index = Str.IndexOf(Close, start_index + 1, StringComparison.Ordinal);
        if (stop_index == -1) throw new FormatException();
        var start = start_index;
        do
        {
            start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
            if (start != -1 && start < stop_index)
                stop_index = Str.IndexOf(Close, stop_index + 1, StringComparison.Ordinal);
        } while (start != -1 && start < stop_index);
        if (stop_index == -1 || stop_index < start_index) throw new FormatException();
        Offset = stop_index + Close.Length;
        start_index += Open.Length;
        return Str[start_index..stop_index];
    }

    public static string? GetBracketText(
        this string Str,
        ref int Offset,
        string Open,
        string Close,
        out string TextBefore,
        out string? TextAfter)
    {
        TextAfter = null;
        var start_index = Str.IndexOf(Open, Offset, StringComparison.Ordinal);
        if (start_index == -1)
        {
            TextBefore = Str[Offset..];
            return null;
        }
        var stop_index = Str.IndexOf(Close, start_index + 1, StringComparison.Ordinal);
        if (stop_index == -1) throw new FormatException();
        var start = start_index;
        do
        {
            start = Str.IndexOf(Open, start + 1, StringComparison.Ordinal);
            if (start != -1 && start < stop_index)
                stop_index = Str.IndexOf(Close, stop_index + 1, StringComparison.Ordinal);
        } while (start != -1 && start < stop_index);
        if (stop_index == -1 || stop_index < start_index) throw new FormatException();
        TextBefore = Str[Offset..start_index];
        Offset = stop_index + Close.Length;
        TextAfter = Str.Length - Offset > 0 ? Str[Offset..] : string.Empty;
        start_index += Open.Length;
        return Str[start_index..stop_index];
    }

    /// <summary>Проверка строки на пустоту, либо нулевую ссылку</summary>
    /// <param name="Str">Проверяемая строка</param>
    /// <returns>Истина, если строка пуста, либо если передана нулевая ссылка</returns>
    [DST]
    public static bool IsNullOrEmpty(this string? Str) => string.IsNullOrEmpty(Str);

    /// <summary>Строка присутствует и не пуста</summary>
    /// <param name="Str">Проверяемая строка</param>
    /// <returns>Истина, если строка не  пуста, и если передана ненулевая ссылка</returns>
    [DST]
    public static bool IsNotNullOrEmpty(this string? Str) => !string.IsNullOrEmpty(Str);

    [DST]
    public static bool IsNullOrWhiteSpace(this string? Str) => string.IsNullOrWhiteSpace(Str);

    [DST]
    public static bool IsNotNullOrWhiteSpace(this string? Str) => !string.IsNullOrWhiteSpace(Str);

    /// <summary>Удаление символов в начале строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <param name="symbols">Перечень удаляемых символов</param>
    /// <returns>Новая строка с удалёнными символами в начале</returns>
    public static string ClearSymbolsAtBegin(this string str, params char[] symbols)
    {
        var i = 0;
        var len = str.Length;
        while (i < len && symbols.IsContains(str[i])) i++;

        return i == 0 || len == 0 ? str : str[i..len];
    }

    /// <summary>Удаление символов в конце строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <param name="symbols">Перечень удаляемых символов</param>
    /// <returns>Новая строка с удалёнными символами в конце</returns>
    public static string ClearSymbolsAtEnd(this string str, params char[] symbols)
    {
        var len = str.Length;
        var i = 0;
        while (i < len && symbols.IsContains(str[len - i - 1])) i++;

        return len == 0 || i == 0 ? str : str[..(len - i)];
    }

    /// <summary>Удаление символов в начале и конце строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <param name="symbols">Перечень удаляемых символов</param>
    /// <returns>Новая строка с удалёнными символами в начале и конце</returns>
    public static string ClearSymbolsAtBeginAndEnd(this string str, params char[] symbols) => str.ClearSymbolsAtBegin(symbols).ClearSymbolsAtEnd(symbols);

    /// <summary>Удаление служебных символов в начале и конце строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <returns>Новая строка с удалёнными служебными символами в начали и конце</returns>
    public static string ClearSystemSymbolsAtBeginAndEnd(this string str) => str.ClearSymbolsAtBeginAndEnd(' ', '\n', '\r');

    /// <summary>Проверка на пустоту строки</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <param name="ParameterName">Имя параметра, добавляемое в исключение в случае его генерации</param>
    /// <param name="Message">Сообщение, добавляемое в исключение в случае его генерации</param>
    /// <exception cref="ArgumentNullException">Если переданная пустая ссылка на строку <paramref name="str"/></exception>
    /// <exception cref="ArgumentException">Если переданная строка <paramref name="str"/> является пустой</exception>
    /// <returns>Строка, гарантированно не являющаяся пустой</returns>
    public static string NotEmpty(this string? str, string ParameterName, string? Message = "Передана пустая строка") =>
        string.IsNullOrEmpty(str ?? throw new ArgumentNullException(ParameterName))
            ? throw new ArgumentException(Message ?? "Передана пустая строка", ParameterName)
            : str;

    public static string NotNull(this string? str, string? Message = null) => str ?? throw new InvalidOperationException(Message ?? "Отсутствует ссылка на объект");

    /// <summary>Зашифровать строку</summary>
    /// <param name="str">Шифруемая строка</param>
    /// <param name="password">Пароль шифрования</param>
    /// <returns>Зашифрованная строка</returns>
    public static string Encrypt(this string str, string password) => str.Encrypt(password, __Salt);

    /// <summary>Зашифровать строку</summary>
    /// <param name="str">Шифруемая строка</param>
    /// <param name="password">Пароль шифрования</param>
    /// <param name="Salt">Соль алгоритма</param>
    /// <returns>Зашифрованная строка</returns>
    public static string Encrypt(this string str, string password, byte[] Salt) => Convert.ToBase64String(str.Compress().Encrypt(password, Salt));

    /// <summary>Зашифровать массив байт</summary>
    /// <param name="data">Шифруемая последовательность байт</param>
    /// <param name="password">Ключ шифрования</param>
    /// <returns>Зашифрованная последовательность байт</returns>
    public static byte[] Encrypt(this byte[] data, string password) => data.Encrypt(password, __Salt);


    public static byte[] Encrypt(this byte[] data, string password, byte[] Salt)
    {
        var algorithm = GetAlgorithm(password, Salt);
        using var stream = new MemoryStream();
        using var crypto_stream = new CryptoStream(stream, algorithm, CryptoStreamMode.Write);
        crypto_stream.Write(data, 0, data.Length);
        crypto_stream.FlushFinalBlock();
        return stream.ToArray();
    }

    /// <summary>Расшифровать последовательность байт</summary>
    /// <param name="data">Расшифровываемая последовательность байт</param>
    /// <param name="password">Пароль шифрования</param>
    /// <returns>Расшифрованная последовательность байт</returns>
    public static byte[] Decrypt(this byte[] data, string password)
    {
        var algorithm = GetInverseAlgorithm(password);
        using var stream = new MemoryStream();
        using var crypto_stream = new CryptoStream(stream, algorithm, CryptoStreamMode.Write);
        crypto_stream.Write(data, 0, data.Length);
        crypto_stream.FlushFinalBlock();
        return stream.ToArray();
    }

    /// <summary>Расшифровать строку</summary>
    /// <param name="str">Зашифрованная строка</param>
    /// <param name="password">Пароль шифрования</param>
    /// <returns>Расшифрованная строка</returns>
    public static string Decrypt(this string str, string password) => Convert.FromBase64String(str).Decrypt(password).DecompressAsString();

    /// <summary>Массив байт - "соль" алгоритма шифрования Rfc2898</summary>
    private static readonly byte[] __Salt =
    {
        0x26, 0xdc, 0xff, 0x00,
        0xad, 0xed, 0x7a, 0xee,
        0xc5, 0xfe, 0x07, 0xaf,
        0x4d, 0x08, 0x22, 0x3c
    };

    private static Rijndael CreateRijndael(string Password, byte[]? Salt = null)
    {
        var pdb = new Rfc2898DeriveBytes(Password, Salt ?? Array.Empty<byte>());
        var algorithm = Rijndael.Create();
        algorithm.Key = pdb.GetBytes(32);
        algorithm.IV = pdb.GetBytes(16);
        return algorithm;
    }

    /// <summary>Получить алгоритм шифрования с указанным паролем</summary>
    /// <param name="password">Пароль шифрования</param>
    /// <param name="Salt">Соль алгоритма</param>
    /// <returns>Алгоритм шифрования</returns>
    private static ICryptoTransform GetAlgorithm(string password, byte[] Salt)
    {
        //var pdb = new Rfc2898DeriveBytes(password, Salt);
        //var algorithm = Rijndael.Create();
        //algorithm.Key = pdb.GetBytes(32);
        //algorithm.IV = pdb.GetBytes(16);
        var algorithm = CreateRijndael(password, Salt);
        return algorithm.CreateEncryptor();
    }

    /// <summary>Получить алгоритм для расшифровки</summary>
    /// <param name="password">Пароль</param>
    /// <returns>Алгоритм расшифровки</returns>
    private static ICryptoTransform GetInverseAlgorithm(string password)
    {
        //var pdb = new Rfc2898DeriveBytes(password, Array.Empty<byte>());
        //var algorithm = Rijndael.Create();
        //algorithm.Key = pdb.GetBytes(32);
        //algorithm.IV = pdb.GetBytes(16);
        var algorithm = CreateRijndael(password);
        return algorithm.CreateDecryptor();
    }

    public static Match MatchRegEx(this string str, Regex expr) => expr.Match(str);

    public static int ToInt(this string str) => int.Parse(str);
    public static int ToInt(this string str, IFormatProvider provider) => int.Parse(str, provider);
    public static int ToInt(this string str, NumberStyles style, IFormatProvider provider) => int.Parse(str, style, provider);
    public static int ToInt(this string str, NumberStyles style) => int.Parse(str, style);
    public static int? ToIntNull(this string str) => int.TryParse(str, out var v) ? v : null;
    public static int? ToIntNull(this string str, NumberStyles style, IFormatProvider provider) => int.TryParse(str, style, provider, out var v) ? v : null;
    public static bool TryParseInt(this string str, out int value) => int.TryParse(str, out value);
    public static bool TryParseInt(this string str, NumberStyles style, IFormatProvider provider, out int value) => int.TryParse(str, style, provider, out value);

    public static double ToDouble(this string str) => double.Parse(str);
    public static double ToDouble(this string str, IFormatProvider provider) => double.Parse(str, provider);
    public static double ToDouble(this string str, NumberStyles style, IFormatProvider provider) => double.Parse(str, style, provider);
    public static double ToDouble(this string str, NumberStyles style) => double.Parse(str, style);
    public static double? ToDoubleNull(this string str) => double.TryParse(str, out var v) ? v : null;
    public static double? ToDoubleNull(this string str, NumberStyles style, IFormatProvider provider) => double.TryParse(str, style, provider, out var v) ? v : null;
    public static bool TryParseDouble(this string str, out double value) => double.TryParse(str, out value);
    public static bool TryParseDouble(this string str, NumberStyles style, IFormatProvider provider, out double value) => double.TryParse(str, style, provider, out value);

    public static bool IsInt(this string str)
    {
        if (str is not { Length: > 0 }) return false;
        for (var i = 0; i < str.Length; i++)
            if (!char.IsDigit(str, i))
                return false;
        return true;
    }

    public static bool IsDouble(this string str)
    {
        if (str is not { Length: > 0 } || str[^1] is '.' or ',')
            return false;

        var is_fraction = false;
        for (var i = 0; i < str.Length; i++)
            if (!is_fraction)
            {
                if (char.IsDigit(str, i)) continue;
                if (str[i] is '.' or ',')
                    is_fraction = true;
                else
                    return false;
            }
            else if (!char.IsDigit(str, i))
                return false;

        return true;
    }

    public static bool ContainsInvariant(this string str, string pattern) => str.IndexOf(pattern, StringComparison.InvariantCulture) >= 0;
    public static bool ContainsInvariantIgnoreCase(this string str, string pattern) => str.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0;
    public static bool ContainsOrdinal(this string str, string pattern) => str.IndexOf(pattern, StringComparison.Ordinal) >= 0;
    public static bool ContainsOrdinalIgnoreCase(this string str, string pattern) => str.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;

    public static bool EqualsInvariant(this string str, string other) => string.Equals(str, other, StringComparison.InvariantCulture);
    public static bool EqualsInvariantIgnoreCase(this string str, string other) => string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);
    public static bool EqualsOrdinal(this string str, string other) => string.Equals(str, other, StringComparison.Ordinal);
    public static bool EqualsOrdinalIgnoreCase(this string str, string other) => string.Equals(str, other, StringComparison.OrdinalIgnoreCase);

    public static bool StartWithInvariant(this string str, string other) => str.StartsWith(other, StringComparison.InvariantCulture);
    public static bool StartWithInvariantIgnoreCase(this string str, string other) => str.StartsWith(other, StringComparison.InvariantCultureIgnoreCase);
    public static bool StartWithOrdinal(this string str, string other) => str.StartsWith(other, StringComparison.Ordinal);
    public static bool StartWithOrdinalIgnoreCase(this string str, string other) => str.StartsWith(other, StringComparison.OrdinalIgnoreCase);

    public static bool EndWithInvariant(this string str, string other) => str.EndsWith(other, StringComparison.InvariantCulture);
    public static bool EndWithInvariantIgnoreCase(this string str, string other) => str.EndsWith(other, StringComparison.InvariantCultureIgnoreCase);
    public static bool EndWithOrdinal(this string str, string other) => str.EndsWith(other, StringComparison.Ordinal);
    public static bool EndWithOrdinalIgnoreCase(this string str, string other) => str.EndsWith(other, StringComparison.OrdinalIgnoreCase);
}