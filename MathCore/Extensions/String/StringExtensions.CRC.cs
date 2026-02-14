using System.Text;

using MathCore.Hash.CRC;

namespace System;

public static partial class StringExtensions
{
    /// <summary>Вычисляет SHA256-хеш строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Массив байт с хешем</returns>
    public static byte[] ComputeSHA256(this string text, Encoding? encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeSHA256();

    /// <summary>Вычисляет MD5-хеш строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Массив байт с хешем</returns>
    public static byte[] ComputeMD5(this string text, Encoding? encoding = null) => (encoding ?? Encoding.Default).GetBytes(text).ComputeMD5();

    /// <summary>Вычисляет CRC-8 строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8</param>
    /// <param name="InitialValue">Начальное значение CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC</param>
    /// <returns>Вычисленное значение CRC-8</returns>
    public static byte ComputeCRC8(
        this string text,
        Encoding? encoding = null,
        byte Polynomial = 0x07,
        byte InitialValue = 0x00,
        bool RefIn = false,
        bool RefOut = false,
        byte XOROut = 0x00) =>
        CRC8.Hash((encoding ?? Encoding.Default).GetBytes(text), Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Вычисляет CRC-16 строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16</param>
    /// <param name="InitialValue">Начальное значение CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC</param>
    /// <returns>Вычисленное значение CRC-16</returns>
    public static ushort ComputeCRC16(
        this string text,
        Encoding? encoding = null,
        ushort Polynomial = 0x1021,
        ushort InitialValue = 0x0000,
        bool RefIn = false,
        bool RefOut = false,
        ushort XOROut = 0x0000) =>
        CRC16.Hash((encoding ?? Encoding.Default).GetBytes(text), Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Вычисляет CRC-32 строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32</param>
    /// <param name="InitialValue">Начальное значение CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC</param>
    /// <returns>Вычисленное значение CRC-32</returns>
    public static uint ComputeCRC32(
        this string text,
        Encoding? encoding = null,
        uint Polynomial = 0x04C11DB7,
        uint InitialValue = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFFFF) =>
        CRC32.Hash((encoding ?? Encoding.Default).GetBytes(text), Polynomial, InitialValue, RefIn, RefOut, XOROut);

    /// <summary>Вычисляет CRC-64 строки</summary>
    /// <param name="text">Исходная строка</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64</param>
    /// <param name="InitialValue">Начальное значение CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC</param>
    /// <returns>Вычисленное значение CRC-64</returns>
    public static ulong ComputeCRC64(
        this string text,
        Encoding? encoding = null,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialValue = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false,
        ulong XOROut = 0x0000000000000000) =>
        CRC64.Hash((encoding ?? Encoding.Default).GetBytes(text), Polynomial, InitialValue, RefIn, RefOut, XOROut);
}
