#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Класс методов-расширений для массивов байт</summary>
public static class ByteArrayExtensions
{
    /// <summary>Вычислить контрольную сумму массива с применением алгоритма SHA256</summary>
    /// <param name="bytes">Массив байт для которого производится вычисление суммы SHA256</param>
    /// <returns>Массив байт рассчитанной суммы SHA256</returns>
    public static byte[] ComputeSHA256(this byte[] bytes)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(bytes);
    }

    /// <summary>Вычислить контрольную сумму массива с применением алгоритма SHA256</summary>
    /// <param name="bytes">Массив байт для которого производится вычисление суммы SHA256</param>
    /// <param name="offset">Индекс элемента массива с которого требуется вычислить контрольную суммы</param>
    /// <param name="count">Число элементов массива, участвующих в расчёте суммы</param>
    /// <returns>Массив байт рассчитанной суммы SHA256</returns>
    public static byte[] ComputeSHA256(this byte[] bytes, int offset, int count)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(bytes, offset, count);
    }

    /// <summary>Вычислить контрольную сумму массива с применением алгоритма MD5</summary>
    /// <param name="bytes">Массив байт для которого производится вычисление суммы MD5</param>
    /// <returns>Массив байт рассчитанной суммы MD5</returns>
    public static byte[] ComputeMD5(this byte[] bytes)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(bytes);
    }

    /// <summary>Вычислить контрольную сумму массива с применением алгоритма MD5</summary>
    /// <param name="bytes">Массив байт для которого производится вычисление суммы MD5</param>
    /// <param name="offset">Индекс элемента массива с которого требуется вычислить контрольную суммы</param>
    /// <param name="count">Число элементов массива, участвующих в расчёте суммы</param>
    /// <returns>Массив байт рассчитанной суммы MD5</returns>
    public static byte[] ComputeMD5(this byte[] bytes, int offset, int count)
    {
        using var md5 = MD5.Create();
        return md5.ComputeHash(bytes, offset, count);
    }

    /// <summary>Преобразовать массив байт в массив целых чисел длиной два байта каждое</summary>
    /// <param name="array">Массив байт чётной</param>
    /// <param name="Destination">Массив двухбайтовых целых чисел, на который осуществляется проекция в памяти массива байт</param>
    public static void ToInt16Array(this byte[] array, short[] Destination) =>
        Buffer.BlockCopy(array, 0, Destination, 0, Math.Min(array.Length, Destination.Length * 2));

    /// <summary>Преобразовать массив байт в массив целых чисел длиной два байта каждое</summary>
    /// <param name="array">Массив байт чётной</param>
    /// <returns>Новый массив двухбайтовых целых чисел, на который осуществляется проекция в памяти массива байт</returns>
    public static short[] ToInt16Array(this byte[] array)
    {
        var result = new short[array.Length / 2];
        array.ToInt16Array(result);
        return result;
    }

    /// <summary>Преобразовать массив байт в массив целых чисел длиной четыре байта каждое</summary>
    /// <param name="array">Массив байт чётной</param>
    /// <param name="Destination">Массив четырёхбайтных целых чисел, на который осуществляется проекция в памяти массива байт</param>
    public static void ToInt32Array(this byte[] array, int[] Destination) =>
        Buffer.BlockCopy(array, 0, Destination, 0, Math.Min(array.Length, Destination.Length * 4));

    /// <summary>Преобразовать массив байт в массив целых чисел длиной четыре байта каждое</summary>
    /// <param name="array">Массив байт чётной</param>
    /// <returns>Новый массив четырёхбайтных целых чисел, на который осуществляется проекция в памяти массива байт</returns>
    public static int[] ToInt32Array(this byte[] array)
    {
        var result = new int[array.Length / 4];
        Buffer.BlockCopy(array, 0, result, 0, array.Length);
        return result;
    }

    /// <summary>Рассчитать четырёхбайтную сумму массива байт с переполнением</summary>
    /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
    /// <returns>Целое четырёхбайтное со знаком, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
    public static int ToInt(this byte[] array)
    {
        var result = 0;
        unchecked
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < array.Length; i++)
                result += array[i] << (8 * i);
        }
        return result;
    }

    /// <summary>Рассчитать четырёхбайтную (без знака) сумму массива байт с переполнением</summary>
    /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
    /// <returns>Целое четырёхбайтное без знака, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
    public static uint ToUInt(this byte[] array)
    {
        var result = 0U;
        unchecked
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < array.Length; i++)
                result += (uint)(array[i] << (8 * i));
        }
        return result;
    }

    /// <summary>Рассчитать восьмибайтную сумму массива байт с переполнением</summary>
    /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
    /// <returns>Целое восьмибайтное со знаком, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
    public static long ToLong(this byte[] array)
    {
        var result = 0L;
        unchecked
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < array.Length; i++)
                result += array[i] << (8 * i);
        }
        return result;
    }

    /// <summary>Рассчитать восьмибайтную (без знака) сумму массива байт с переполнением</summary>
    /// <param name="array">Массив байт, сумму которого требуется рассчитать</param>
    /// <returns>Целое восьмибайтное без знака, сверяющиеся результатом вычисления суммы байт исходного массива с переполнением</returns>
    public static ulong ToULong(this byte[] array)
    {
        var result = 0UL;
        unchecked
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < array.Length; i++)
                result += (ulong)(array[i] << (8 * i));
        }
        return result;
    }

    /// <summary>Преобразование массива байт в строку в HEX формате</summary>
    /// <param name="array">Кодируемый массив</param>
    /// <param name="UpperString">Преобразование в строку в верхнем регистре</param>
    /// <returns>Строковое представление массива</returns>
    public static string ToStringHex(this byte[] array, bool UpperString = true)
    {
        var length = array.NotNull().Length;
        if (length == 0) return string.Empty;

        var result = new StringBuilder(length * 2);
        if (UpperString)
            foreach (var b in array)
            {
                result.Append((b & 0xF0) switch
                {
                    0x00 => '0', 0x10 => '1', 0x20 => '2', 0x30 => '3', 0x40 => '4', 0x50 => '5', 0x60 => '6', 0x70 => '7',
                    0x80 => '8', 0x90 => '9', 0xA0 => 'A', 0xB0 => 'B', 0xC0 => 'C', 0xD0 => 'D', 0xE0 => 'E', _    => 'F',
                });
                result.Append((b & 0xF) switch
                {
                    0x00 => '0', 0x01 => '1', 0x02 => '2', 0x03 => '3', 0x04 => '4', 0x05 => '5', 0x06 => '6', 0x07 => '7',
                    0x08 => '8', 0x09 => '9', 0x0A => 'A', 0x0B => 'B', 0x0C => 'C', 0x0D => 'D', 0x0E => 'E', _    => 'F',
                });
            }
        else
            foreach (var b in array)
            {
                result.Append((b & 0xF0) switch
                {
                    0x00 => '0', 0x10 => '1', 0x20 => '2', 0x30 => '3', 0x40 => '4', 0x50 => '5', 0x60 => '6', 0x70 => '7',
                    0x80 => '8', 0x90 => '9', 0xA0 => 'a', 0xB0 => 'b', 0xC0 => 'c', 0xD0 => 'd', 0xE0 => 'e', _    => 'f',
                });
                result.Append((b & 0xF) switch
                {
                    0x00 => '0', 0x01 => '1', 0x02 => '2', 0x03 => '3', 0x04 => '4', 0x05 => '5', 0x06 => '6', 0x07 => '7',
                    0x08 => '8', 0x09 => '9', 0x0A => 'a', 0x0B => 'b', 0x0C => 'c', 0x0D => 'd', 0x0E => 'e', _    => 'f',
                });
            }

        return result.ToString();
    }

    /// <summary>Удаление технических символов из строки массива</summary>
    /// <param name="str">Сырая строка массива</param>
    /// <returns>Строка с удалёнными техническими символами</returns>
    private static string ClearByteArrayString(this string str)
    {
        str = str.Trim('[', '{', '(', ' ', '\r', '\n', ')', '}', ']', '|', '&');
        if (str.IndexOf('-') == -1 && str.IndexOf(' ') == -1) return str;

        var result = new StringBuilder(str.Length);
        foreach (var c in str)
            if (c is not ('-' or ' '))
                result.Append(c);

        return result.ToString();
    }

    /// <summary>Преобразование массива байт из строкового представления в формате HEX в массив</summary>
    /// <param name="str">Строковое представление массива байт в HEX-формате</param>
    /// <returns>Массив байт</returns>
    /// <exception cref="FormatException"></exception>
    [return: NotNullIfNotNull(nameof(str))]
    public static byte[]? FromHexByteArrayString(this string? str)
    {
        if (str is null) return null;

        str = str.ClearByteArrayString();
        var length_str = str.Length;
        if (length_str % 1 != 0)
            throw new FormatException("Число символов в строке должно быть чётным");

        var result = new byte[length_str / 2];
        for (var i = 0; i < result.Length; i++)
        {
            byte b = str[i * 2].ToUpperInvariant() switch
            {
                '0' => 0x00, '1' => 0x10, '2' => 0x20, '3' => 0x30, '4' => 0x40, '5' => 0x50, '6' => 0x60, '7' => 0x70,
                '8' => 0x80, '9' => 0x90, 'A' => 0xA0, 'B' => 0xB0, 'C' => 0xC0, 'D' => 0xD0, 'E' => 0xE0, 'F' => 0xF0,
                _   => throw new FormatException($"Символ s[{i * 2}] = {str[i * 2]} должен быть в диапазоне 0-9A-F")
            };

            b |= str[i * 2 + 1].ToUpperInvariant() switch
            {
                '0' => 0x00, '1' => 0x01, '2' => 0x02, '3' => 0x03, '4' => 0x04, '5' => 0x05, '6' => 0x06, '7' => 0x07,
                '8' => 0x08, '9' => 0x09, 'A' => 0x0A, 'B' => 0x0B, 'C' => 0x0C, 'D' => 0x0D, 'E' => 0x0E, 'F' => 0x0F,
                _   => throw new FormatException($"Символ s[{i * 2 + 1}] = {str[i * 2 + 1]} должен быть в диапазоне 0-9A-F")
            };
            result[i] = b;
        }

        return result;
    }

    /// <summary>Преобразование массива в строковое представление в кодировке Base64</summary>
    /// <param name="array">Кодируемый массив</param>
    /// <returns>Строковое представление массива в представлении Base64</returns>
    public static string? ToStringBase64(this byte[]? array) => array switch
    {
        null => null,
        { Length: 0 } => string.Empty,
        _ => Convert.ToBase64String(array)
    };

    /// <summary>Преобразование строки в кодировке Base64 в массив байт</summary>
    /// <param name="str">Строковое представление массива байт в кодировке Base64</param>
    /// <returns>Массив байт</returns>
    [return: NotNullIfNotNull(nameof(str))]
    public static byte[]? FromBase64ByteArrayString(this string? str) => str switch
    {
        null => null,
        { Length: 0 } => Array.Empty<byte>(),
        _ => Convert.FromBase64String(str)
    };
}