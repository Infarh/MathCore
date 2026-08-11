using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using MathCore;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Методы-расширения класса <see cref="T:System.String">строк</see></summary>
public static partial class StringExtensions
{
    /// <summary>Подсчитывает количество вхождений символа в строке</summary>
    /// <param name="s">Исходная строка</param>
    /// <param name="c">Символ для подсчёта</param>
    /// <returns>Количество вхождений символа в строке</returns>
    public static int CountChar(this string s, char c)
    {
        var length = s.Length;
        if (length == 0) return 0;

        var count = 0;
        for (var i = 0; i < length; i++)
            if (s[i] == c)
                count++;

        return count;
    }

    /// <summary>Пытается преобразовать строку в указанный тип</summary>
    /// <typeparam name="T">Тип, в который нужно преобразовать строку</typeparam>
    /// <param name="str">Исходная строка</param>
    /// <param name="value">Результат преобразования</param>
    /// <returns>Истина, если преобразование прошло успешно</returns>
    public static bool TryConvertTo<T>(this string? str, out T? value)
    {
        if (str is null || typeof(T).GetTypeConverter() is not { } converter || !converter.CanConvertFrom(typeof(string)))
        {
            value = default;
            return false;
        }

        if (typeof(T) == typeof(bool?) || typeof(T) == typeof(bool))
            str = str switch
            {
                "yes" or "Yes" or "YES" or "да" or "Да" or "ДА" => "true",
                "no" or "No" or "NO" or "нет" or "Нет" or "НЕТ" => "false",
                _ => str
            };

        try
        {
            value = (T?)converter.ConvertFrom(str);
            return true;
        }
        catch (ArgumentException e)
        {
            Debug.WriteLine(e);
        }
        catch (FormatException e)
        {
            Debug.WriteLine(e);
        }

        value = default;
        return false;
    }

    /// <summary>Перечисляет строки в исходной строке</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <param name="Trim">Обрезать строки</param>
    /// <returns>Перечисление строк</returns>
    public static IEnumerable<string> EnumLines(this string str, bool SkipEmpty = false, bool Trim = false)
    {
        using var reader = str.CreateReader();
        if (Trim)
        {
            while (reader.ReadLine() is { } line)
                if (line.Length > 0 || !SkipEmpty)
                    yield return line.Trim();
        }
        else
            while (reader.ReadLine() is { } line)
                if (line.Length > 0 || !SkipEmpty)
                    yield return line;
    }

    /// <summary>Перечисляет строки в исходной строке с обрезкой символов</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="TrimChar">Символ для обрезки</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление строк</returns>
    public static IEnumerable<string> EnumLines(this string str, char TrimChar, bool SkipEmpty = false)
    {
        using var reader = str.CreateReader();
        while (reader.ReadLine() is { } line)
            if (line.Length > 0 || !SkipEmpty)
                yield return line.Trim(TrimChar);
    }

    /// <summary>Перечисляет строки в исходной строке с обрезкой символов</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <param name="TrimChars">Символы для обрезки</param>
    /// <returns>Перечисление строк</returns>
    public static IEnumerable<string> EnumLines(this string str, bool SkipEmpty = false, params char[] TrimChars)
    {
        using var reader = str.CreateReader();
        while (reader.ReadLine() is { } line)
            if (line.Length > 0 || !SkipEmpty)
                yield return line.Trim(TrimChars);
    }

    /// <summary>Перечисляет строки в исходной строке с преобразованием</summary>
    /// <typeparam name="T">Тип результата преобразования</typeparam>
    /// <param name="str">Исходная строка</param>
    /// <param name="Selector">Функция преобразования строки</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление преобразованных строк</returns>
    public static IEnumerable<T> EnumLines<T>(this string str, Func<string, T> Selector, bool SkipEmpty = false)
    {
        using var reader = str.CreateReader();
        while (reader.ReadLine() is { } line)
            if (line.Length > 0 || !SkipEmpty)
                yield return Selector(line);
    }

    /// <summary>Перечисляет строки в исходной строке с преобразованием и индексом</summary>
    /// <typeparam name="T">Тип результата преобразования</typeparam>
    /// <param name="str">Исходная строка</param>
    /// <param name="Selector">Функция преобразования строки с индексом</param>
    /// <param name="SkipEmpty">Пропускать пустые строки</param>
    /// <returns>Перечисление преобразованных строк</returns>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringReader CreateReader(this string str) => new(str);

    /// <summary>Создать построитель строки</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Объект <see cref="StringBuilder"/> для формирования строки</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder CreateBuilder(this string str) => new(str);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Указатель на позицию в строке</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringPtr AsStringPtr(this string str) => new(str, 0, str.Length);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Pos">Положение в строке</param>
    /// <returns>Указатель на позицию в строке</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringPtr AsStringPtr(this string str, int Pos) => new(str, Pos, str.Length - Pos);

    /// <summary>Преобразовать строку в указатель</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="Pos">Положение в строке</param>
    /// <param name="Length">Длина подстроки</param>
    /// <returns>Указатель on позицию в строке</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
#if NET8_0_OR_GREATER
        await
#endif
        using (var compressor = new GZipStream(output, CompressionLevel.Optimal))
        {
            var bytes = Encoding.UTF8.GetBytes(str);
#if NET8_0_OR_GREATER
            await compressor.WriteAsync(bytes, Cancel).ConfigureAwait(false);
#else
            await compressor.WriteAsync(bytes, 0, bytes.Length, Cancel).ConfigureAwait(false);
#endif
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

    /// <summary>Объединяет строки с указанным разделителем</summary>
    /// <param name="strings">Строки для объединения</param>
    /// <param name="separator">Разделитель</param>
    /// <returns>Результирующая строка</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinStrings(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

#if NET5_0_OR_GREATER
    /// <summary>Объединяет строки с указанным символьным разделителем</summary>
    /// <param name="strings">Строки для объединения</param>
    /// <param name="separator">Символ-разделитель</param>
    /// <returns>Результирующая строка</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinStrings(this IEnumerable<string> strings, char separator) => string.Join(separator, strings);
#endif

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
        if (stop_index == -1) throw new FormatException($"Не найдена парная закрывающая скобка '{Close}'");
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

    /// <summary>Выделение подстроки, ограниченной шаблоном начала и шаблоном окончания строки начиная с указанного смещения, с возвратом текста до и после</summary>
    /// <param name="Str">Входная строка</param>
    /// <param name="Offset">Смещение во входной строке начала поиска</param>
    /// <param name="Open">Шаблон начала подстроки</param>
    /// <param name="Close">Шаблон окончания подстроки</param>
    /// <param name="TextBefore">Текст до найденной подстроки</param>
    /// <param name="TextAfter">Текст после найденной подстроки</param>
    /// <returns>Подстрока, заключённая между указанными шаблонами начала и окончания</returns>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty(this string? Str) => string.IsNullOrEmpty(Str);

    /// <summary>Строка присутствует и не пуста</summary>
    /// <param name="Str">Проверяемая строка</param>
    /// <returns>Истина, если строка не  пуста, и если передана ненулевая ссылка</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrEmpty(this string? Str) => !string.IsNullOrEmpty(Str);

    /// <summary>Проверка строки на пустоту или пробелы</summary>
    /// <param name="Str">Проверяемая строка</param>
    /// <returns>Истина, если строка пуста, либо если передана нулевая ссылка, либо состоит только из пробелов</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrWhiteSpace(this string? Str) => string.IsNullOrWhiteSpace(Str);

    /// <summary>Строка присутствует и не состоит только из пробелов</summary>
    /// <param name="Str">Проверяемая строка</param>
    /// <returns>Истина, если строка не пуста, не состоит только из пробелов и если передана ненулевая ссылка</returns>
    [DST]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotNullOrWhiteSpace(this string? Str) => !string.IsNullOrWhiteSpace(Str);

    /// <summary>Удаление символов в начале строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <param name="symbols">Перечень удаляемых символов</param>
    /// <returns>Новая строка с удалёнными символами в начале</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ClearSymbolsAtBeginAndEnd(this string str, params char[] symbols) => str.ClearSymbolsAtBegin(symbols).ClearSymbolsAtEnd(symbols);

    /// <summary>Удаление служебных символов в начале и конце строки</summary>
    /// <param name="str">Обрабатываемая строка</param>
    /// <returns>Новая строка с удалёнными служебными символами в начале и конце</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ClearSystemSymbolsAtBeginAndEnd(this string str) => str.ClearSymbolsAtBeginAndEnd(' ', '\n', '\r');

    /// <summary>Проверка на пустоту строки</summary>
    /// <param name="str">Проверяемая строка</param>
    /// <param name="ParameterName">Имя параметра, добавляемое в исключение в случае его генерации</param>
    /// <param name="Message">Сообщение, добавляемое в исключение в случае его генерации</param>
    /// <exception cref="ArgumentNullException">Если передана пустая ссылка на строку</exception>
    /// <exception cref="ArgumentException">Если переданная строка является пустой</exception>
    /// <returns>Строка, гарантированно не являющаяся пустой</returns>
    public static string NotEmpty(this string? str, string ParameterName, string? Message = "Передана пустая строка") =>
        string.IsNullOrEmpty(str ?? throw new ArgumentNullException(ParameterName))
            ? throw new ArgumentException(Message ?? "Передана пустая строка", ParameterName)
            : str;

    /// <summary>Зашифровать строку</summary>
    /// <param name="str">Шифруемая строка</param>
    /// <param name="password">Пароль шифрования</param>
    /// <returns>Зашифрованная строка</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Encrypt(this string str, string password) => str.Encrypt(password, __Salt);

    /// <summary>Зашифровать строку</summary>
    /// <param name="str">Шифруемая строка</param>
    /// <param name="password">Пароль шифрования</param>
    /// <param name="Salt">Соль алгоритма</param>
    /// <returns>Зашифрованная строка</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Encrypt(this string str, string password, byte[] Salt) => Convert.ToBase64String(str.Compress().Encrypt(password, Salt));

    /// <summary>Зашифровать массив байт</summary>
    /// <param name="data">Шифруемая последовательность байт</param>
    /// <param name="password">Ключ шифрования</param>
    /// <returns>Зашифрованная последовательность байт</returns>
    public static byte[] Encrypt(this byte[] data, string password) => data.Encrypt(password, __Salt);

    /// <summary>Зашифровать массив байт с солью</summary>
    /// <param name="data">Шифруемый массив байт</param>
    /// <param name="password">Пароль</param>
    /// <param name="Salt">Соль</param>
    /// <returns>Зашифрованный массив байт</returns>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Decrypt(this string str, string password) => Convert.FromBase64String(str).Decrypt(password).DecompressAsString();

    /// <summary>Массив байт - "соль" алгоритма шифрования Rfc2898</summary>
    private static readonly byte[] __Salt =
    [
        0x26, 0xdc, 0xff, 0x00,
        0xad, 0xed, 0x7a, 0xee,
        0xc5, 0xfe, 0x07, 0xaf,
        0x4d, 0x08, 0x22, 0x3c
    ];

#pragma warning disable SYSLIB0022
    private static Rijndael CreateRijndael(string Password, byte[]? Salt = null)
    {
#pragma warning disable SYSLIB0060 // Используется для совместимости с ранее зашифрованными данными
#pragma warning disable SYSLIB0041
        var pdb = new Rfc2898DeriveBytes(Password, Salt ?? []);
#pragma warning restore SYSLIB0041
#pragma warning restore SYSLIB0060
        var algorithm = Rijndael.Create();
        algorithm.Key = pdb.GetBytes(32);
        algorithm.IV = pdb.GetBytes(16);
        return algorithm;
    }
#pragma warning restore SYSLIB0022

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

    /// <summary>Выполнить регулярное выражение для строки</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Регулярное выражение</param>
    /// <returns>Результат сопоставления</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Match MatchRegEx(this string str, string expr) => Regex.Match(str, expr);

    /// <summary>Выполнить регулярное выражение для строки с опциями</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Регулярное выражение</param>
    /// <param name="options">Опции регулярного выражения</param>
    /// <returns>Результат сопоставления</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Match MatchRegEx(this string str, string expr, RegexOptions options) => Regex.Match(str, expr, options);

    /// <summary>Выполнить регулярное выражение для строки</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Объект регулярного выражения</param>
    /// <returns>Результат сопоставления</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Match MatchRegEx(this string str, Regex expr) => expr.Match(str);

    /// <summary>Проверить соответствие строки регулярному выражению</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Регулярное выражение</param>
    /// <returns>Истина, если строка соответствует выражению</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatchRegEx(this string str, string expr) => Regex.IsMatch(str, expr);

    /// <summary>Проверить соответствие строки регулярному выражению с опциями</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Регулярное выражение</param>
    /// <param name="options">Опции регулярного выражения</param>
    /// <returns>Истина, если строка соответствует выражению</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIsMatchRegEx(this string str, string expr, RegexOptions options) => Regex.IsMatch(str, expr, options);

    /// <summary>Проверить соответствие строки регулярному выражению</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="expr">Объект регулярного выражения</param>
    /// <returns>Истина, если строка соответствует выражению</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMatchRegEx(this string str, Regex expr) => expr.IsMatch(str);

    /// <summary>Преобразовать строку в целое число</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Целое число</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string str) => int.Parse(str);

    /// <summary>Преобразовать строку в целое число с учетом культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Целое число</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string str, IFormatProvider provider) => int.Parse(str, provider);

    /// <summary>Преобразовать строку в целое число с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Целое число</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string str, NumberStyles style, IFormatProvider provider) => int.Parse(str, style, provider);

    /// <summary>Преобразовать строку в целое число с учетом стиля</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <returns>Целое число</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt(this string str, NumberStyles style) => int.Parse(str, style);

    /// <summary>Преобразовать строку в целое число, если возможно</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Целое число или null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? ToIntNull(this string str) => int.TryParse(str, out var v) ? v : null;

    /// <summary>Преобразовать строку в целое число, если возможно, с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Целое число или null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? ToIntNull(this string str, NumberStyles style, IFormatProvider provider) => int.TryParse(str, style, provider, out var v) ? v : null;

    /// <summary>Преобразовать строку в целое число, если возможно</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="value">Результат преобразования</param>
    /// <returns>Истина, если преобразование прошло успешно</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt(this string str, out int value) => int.TryParse(str, out value);

    /// <summary>Преобразовать строку в целое число, если возможно, с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <param name="value">Результат преобразования</param>
    /// <returns>Истина, если преобразование прошло успешно</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt(this string str, NumberStyles style, IFormatProvider provider, out int value) => int.TryParse(str, style, provider, out value);

    /// <summary>Преобразовать строку в число с плавающей точкой</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Число с плавающей точкой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string str) => double.Parse(str);

    /// <summary>Преобразовать строку в число с плавающей точкой с учетом культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Число с плавающей точкой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string str, IFormatProvider provider) => double.Parse(str, provider);

    /// <summary>Преобразовать строку в число с плавающей точкой с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Число с плавающей точкой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string str, NumberStyles style, IFormatProvider provider) => double.Parse(str, style, provider);

    /// <summary>Преобразовать строку в число с плавающей точкой с учетом стиля</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <returns>Число с плавающей точкой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDouble(this string str, NumberStyles style) => double.Parse(str, style);

    /// <summary>Преобразовать строку в число с плавающей точкой, если возможно</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Число с плавающей точкой или null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? ToDoubleNull(this string str) => double.TryParse(str, out var v) ? v : null;

    /// <summary>Преобразовать строку в число с плавающей точкой, если возможно, с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <returns>Число с плавающей точкой или null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? ToDoubleNull(this string str, NumberStyles style, IFormatProvider provider) => double.TryParse(str, style, provider, out var v) ? v : null;

    /// <summary>Преобразовать строку в число с плавающей точкой, если возможно</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="value">Результат преобразования</param>
    /// <returns>Истина, если преобразование прошло успешно</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDouble(this string str, out double value) => double.TryParse(str, out value);

    /// <summary>Преобразовать строку в число с плавающей точкой, если возможно, с учетом стиля и культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="style">Стиль числа</param>
    /// <param name="provider">Провайдер формата</param>
    /// <param name="value">Результат преобразования</param>
    /// <returns>Истина, если преобразование прошло успешно</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDouble(this string str, NumberStyles style, IFormatProvider provider, out double value) => double.TryParse(str, style, provider, out value);

    /// <summary>Преобразовать строку в число с плавающей точкой с учетом культуры (инвариантная или ru-RU)</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Число с плавающей точкой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDoubleInvariant(this string str)
    {
#if NET8_0_OR_GREATER
        if (str.Contains('.'))
#else
        if (str.IndexOf('.') >= 0)
#endif
            return double.Parse(str, CultureInfo.InvariantCulture);
        return double.Parse(str, CultureInfo.GetCultureInfo("ru-RU"));
    }

    /// <summary>Проверить, является ли строка целым числом</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Истина, если строка содержит только цифры</returns>
    public static bool IsInt(this string str)
    {
        if (str is not { Length: > 0 }) return false;
        for (var i = 0; i < str.Length; i++)
            if (!char.IsDigit(str, i))
                return false;
        return true;
    }

    /// <summary>Проверить, является ли строка числом с плавающей точкой</summary>
    /// <param name="str">Исходная строка</param>
    /// <returns>Истина, если строка содержит только цифры и одну точку или запятую</returns>
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

    /// <summary>Проверить, содержит ли строка подстроку с учетом культуры</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="pattern">Искомая подстрока</param>
    /// <returns>Истина, если строка содержит подстроку</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsInvariant(this string str, string pattern) => str.IndexOf(pattern, StringComparison.InvariantCulture) >= 0;

    /// <summary>Проверить, содержит ли строка подстроку с учетом культуры без учета регистра</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="pattern">Искомая подстрока</param>
    /// <returns>Истина, если строка содержит подстроку</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsInvariantIgnoreCase(this string str, string pattern) => str.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0;

    /// <summary>Проверить, содержит ли строка подстроку с учетом порядка байт</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="pattern">Искомая подстрока</param>
    /// <returns>Истина, если строка содержит подстроку</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsOrdinal(this string str, string pattern) => str.IndexOf(pattern, StringComparison.Ordinal) >= 0;

    /// <summary>Проверить, содержит ли строка подстроку с учетом порядка байт без учета регистра</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="pattern">Искомая подстрока</param>
    /// <returns>Истина, если строка содержит подстроку</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsOrdinalIgnoreCase(this string str, string pattern) => str.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsInvariant(this string str, string other) => string.Equals(str, other, StringComparison.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsInvariantIgnoreCase(this string str, string other) => string.Equals(str, other, StringComparison.InvariantCultureIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsOrdinal(this string str, string other) => string.Equals(str, other, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsOrdinalIgnoreCase(this string str, string other) => string.Equals(str, other, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartWithInvariant(this string str, string other) => str.StartsWith(other, StringComparison.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartWithInvariantIgnoreCase(this string str, string other) => str.StartsWith(other, StringComparison.InvariantCultureIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartWithOrdinal(this string str, string other) => str.StartsWith(other, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartWithOrdinalIgnoreCase(this string str, string other) => str.StartsWith(other, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndWithInvariant(this string str, string other) => str.EndsWith(other, StringComparison.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndWithInvariantIgnoreCase(this string str, string other) => str.EndsWith(other, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>Проверка окончания строки с порядковым сравнением</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="other">Окончание для проверки</param>
    /// <returns>Истина, если строка заканчивается указанной подстрокой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndWithOrdinal(this string str, string other) => str.EndsWith(other, StringComparison.Ordinal);

    /// <summary>Проверка окончания строки с порядковым сравнением без учёта регистра</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="other">Окончание для проверки</param>
    /// <returns>Истина, если строка заканчивается указанной подстрокой</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndWithOrdinalIgnoreCase(this string str, string other) => str.EndsWith(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>Перечисляет сегменты строки заданной длины</summary>
    /// <param name="s">Исходная строка</param>
    /// <param name="SegmentLength">Длина сегмента</param>
    /// <returns>Перечисление сегментов строки</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSegmentsEnumerable EnumerateSegments(this string s, int SegmentLength) => new(s, SegmentLength);

    /// <summary>Делегат преобразования сегмента строки</summary>
    /// <param name="ptr">Указатель на сегмент</param>
    /// <returns>Преобразованный сегмент</returns>
    public delegate StringPtr StringPtrSelector(StringPtr ptr);

    /// <summary>Делегат проверки сегмента строки</summary>
    /// <param name="ptr">Указатель на сегмент</param>
    /// <returns>Истина, если сегмент удовлетворяет условию</returns>
    public delegate bool StringPtrWhereChecker(StringPtr ptr);

    /// <summary>Перечисление сегментов строки</summary>
    public readonly struct StringSegmentsEnumerable(string Str, int SegmentLength, StringPtrSelector? Selector = null, StringPtrWhereChecker? Checker = null)
    {
        /// <summary>Исходная строка</summary>
        public string SourceString => Str;

        /// <summary>Длина сегмента</summary>
        public int SegmentLength { get; } = SegmentLength;

        /// <summary>Возвращает перечислитель сегментов</summary>
        /// <returns>Перечислитель сегментов</returns>
        public StringSegmentEnumerator GetEnumerator() => new(Str, SegmentLength, Selector, Checker);

        /// <summary>Перечислитель сегментов строки</summary>
        public ref struct StringSegmentEnumerator(string Str, int SegmentLength, StringPtrSelector? Selector, StringPtrWhereChecker? Checker)
        {
            private int _Offset;
            private readonly int _Length = Str.Length;

            /// <summary>Текущий сегмент</summary>
            public StringPtr Current { get; private set; }

            /// <summary>Переход к следующему сегменту</summary>
            /// <returns>Истина, если есть следующий сегмент</returns>
            public bool MoveNext()
            {
                do
                {
                    if (_Offset >= _Length) return false;
                    var str = new StringPtr(Str, _Offset, Math.Min(SegmentLength, _Length - _Offset));
                    var current = Selector is not null ? Selector(str) : str;

                    Current = current;
                    _Offset += SegmentLength;
                } while (Checker?.Invoke(Current) != true);
                return true;
            }
        }
    }

    /// <summary>Объединяет сегменты строки указанным символом-разделителем</summary>
    /// <param name="strings">Сегменты строки</param>
    /// <param name="Separator">Символ-разделитель</param>
    /// <returns>Объединённая строка</returns>
    public static string JoinStrings(this StringSegmentsEnumerable strings, char Separator)
    {
        var result = new StringBuilder(strings.SourceString.Length + strings.SourceString.Length / strings.SegmentLength);
        foreach (var item in strings)
            result.Append(item).Append(Separator);

        if (result.Length > 0)
            result.Length--;

        return result.ToString();
    }

    /// <summary>Преобразовать сегменты строки</summary>
    /// <param name="strings">Сегменты строки</param>
    /// <param name="Selector">Функция преобразования сегмента</param>
    /// <returns>Перечисление преобразованных сегментов</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSegmentsEnumerable Select(this StringSegmentsEnumerable strings, StringPtrSelector Selector) =>
        new(strings.SourceString, strings.SegmentLength, Selector);

    /// <summary>Преобразует строку в поток байт</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="encoding">Кодировка (по умолчанию UTF-8)</param>
    /// <returns>Поток байт строки</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream ToByteStream(this string str, Encoding? encoding = null) => new StringByteStream(str, encoding ?? Encoding.UTF8);

#if NET8_0_OR_GREATER

    /// <summary>Гарантирует, что строка начинается с заданного символа</summary>
    /// <remarks>
    /// Если строка уже начинается с символа с учётом Comparison, возвращается исходная строка
    /// Иначе символ добавляется в начало
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Требуемый начальный символ</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureStartWith(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) => str.AsSpan().StartsWith([c], Comparison) ? str : $"{c}{str}";

    /// <summary>Гарантирует, что строка кончается заданным символом</summary>
    /// <remarks>
    /// Если строка уже кончается символом с учётом Comparison, возвращается исходная строка
    /// Иначе символ добавляется в конец
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Требуемый конечный символ</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureEndWith(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) => str.AsSpan().EndsWith([c], Comparison) ? str : $"{str}{c}";

#else

    /// <summary>Гарантирует, что строка начинается с заданного символа</summary>
    /// <remarks>
    /// Если строка уже начинается с символа с учётом Comparison, возвращается исходная строка
    /// Иначе символ добавляется в начало
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Требуемый начальный символ</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureStartWith(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) =>
        str.Length == 0
        ? str
        : Comparison switch
        {
            StringComparison.Ordinal => str[0] == c ? str : $"{c}{str}",
            StringComparison.OrdinalIgnoreCase => char.ToUpperInvariant(str[0]) == char.ToUpperInvariant(c) ? str : $"{c}{str}",
            StringComparison.InvariantCulture => str[0].ToString().Equals(c.ToString(), StringComparison.InvariantCulture) ? str : $"{c}{str}",
            StringComparison.InvariantCultureIgnoreCase => str[0].ToString().Equals(c.ToString(), StringComparison.InvariantCultureIgnoreCase) ? str : $"{c}{str}",
            StringComparison.CurrentCulture => str[0].ToString().Equals(c.ToString(), StringComparison.CurrentCulture) ? str : $"{c}{str}",
            StringComparison.CurrentCultureIgnoreCase => str[0].ToString().Equals(c.ToString(), StringComparison.CurrentCultureIgnoreCase) ? str : $"{c}{str}",
            _ => str[0].Equals(c) ? str : $"{c}{str}",
        };
    //str.AsSpan().StartsWith([c], Comparison) ? str : $"{c}{str}";

    /// <summary>Гарантирует, что строка кончается заданным символом</summary>
    /// <remarks>
    /// Если строка уже кончается символом с учётом Comparison, возвращается исходная строка
    /// Иначе символ добавляется в конец
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="c">Требуемый конечный символ</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureEndWith(this string str, char c, StringComparison Comparison = StringComparison.Ordinal) =>
        str.Length == 0
        ? str
        : Comparison switch
        {
            StringComparison.Ordinal => str[^1] == c ? str : $"{str}{c}",
            StringComparison.OrdinalIgnoreCase => char.ToUpperInvariant(str[^1]) == char.ToUpperInvariant(c) ? str : $"{str}{c}",
            StringComparison.InvariantCulture => str[^1].ToString().Equals(c.ToString(), StringComparison.InvariantCulture) ? str : $"{str}{c}",
            StringComparison.InvariantCultureIgnoreCase => str[^1].ToString().Equals(c.ToString(), StringComparison.InvariantCultureIgnoreCase) ? str : $"{str}{c}",
            StringComparison.CurrentCulture => str[^1].ToString().Equals(c.ToString(), StringComparison.CurrentCulture) ? str : $"{str}{c}",
            StringComparison.CurrentCultureIgnoreCase => str[^1].ToString().Equals(c.ToString(), StringComparison.CurrentCultureIgnoreCase) ? str : $"{str}{c}",
            _ => str[^1].Equals(c) ? str : $"{str}{c}",
        };

#endif

    /// <summary>Гарантирует, что строка начинается с заданной подстроки</summary>
    /// <remarks>
    /// Если строка уже начинается с подстроки с учётом Comparison, возвращается исходная строка
    /// Иначе подстрока добавляется в начало
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="s">Требуемая начальная подстрока</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureStartWith(this string str, string s, StringComparison Comparison = StringComparison.Ordinal) => str.StartsWith(s, Comparison) ? str : $"{s}{str}";


    /// <summary>Гарантирует, что строка кончается заданной подстрокой</summary>
    /// <remarks>
    /// Если строка уже кончается подстрокой с учётом Comparison, возвращается исходная строка
    /// Иначе подстрока добавляется в конец
    /// </remarks>
    /// <param name="str">Исходная строка</param>
    /// <param name="s">Требуемая конечная подстрока</param>
    /// <param name="Comparison">Тип сравнения</param>
    public static string EnsureEndWith(this string str, string s, StringComparison Comparison = StringComparison.Ordinal) => str.EndsWith(s, Comparison) ? str : $"{str}{s}";

    /// <summary>Обрезать середину строки так, чтобы длина результата не превышала заданной</summary>
    /// <param name="str">Исходная строка</param>
    /// <param name="MaxLength">Максимально допустимая длина результата</param>
    /// <param name="Replacement">Шаблон, заменяющий вырезанную часть</param>
    /// <returns>Строка, в которой сохранены начало и конец, а середина заменена шаблоном</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TruncateMiddle(this string str, int MaxLength, string Replacement = "...")
    {
        if (str == null) throw new ArgumentNullException(nameof(str));
        if (Replacement == null) throw new ArgumentNullException(nameof(Replacement));
        if (MaxLength < 0) throw new ArgumentOutOfRangeException(nameof(MaxLength));

        if (str.Length <= MaxLength) return str; // ничего не надо обрезать
        if (MaxLength == 0) return string.Empty; // результат пустой

        var rep_len = Replacement.Length;
        // Если replacement длиннее или равен максимальной длине, формируем итог через StringBuilder
        if (rep_len >= MaxLength)
        {
            var sb = new StringBuilder(MaxLength);
            sb.Append(Replacement, 0, MaxLength);
            return sb.ToString();
        }

        var remaining = MaxLength - rep_len; // сколько символов оставить для начала+конца
        var left = (remaining + 1) / 2; // предпочитаем немного больше для начала
        var right = remaining - left;

        var result = new StringBuilder(MaxLength);
        // добавляем начало строки без создания подстроки
        if (left > 0)
            result.Append(str, 0, left);
        // добавляем replacement
        result.Append(Replacement);
        // добавляем конец строки без создания подстроки
        if (right > 0)
            result.Append(str, str.Length - right, right);

        return result.ToString();
    }

}