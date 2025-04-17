using System.Collections.Concurrent;

using MathCore.Annotations;

namespace MathCore.Hash.CRC;

// https://microsin.net/programming/arm/crc32-demystified.html

public class CRC32(uint poly = (uint)CRC32.Mode.ZipInv)
{
    [PublicAPI]
    public enum Mode : uint
    {
        /// <summary>Инвертированный полином относительно <see cref="P0x04C11DB7"/></summary>
        P0xEDB88320 = 0xEDB88320, // 0b11101101_10111000_10000011_00100000,
        /// <summary>Нормальный полином относительно <see cref="P0xEDB88320"/></summary>
        P0x04C11DB7 = 0x04C11DB7, // 0b00000100_11000001_00011101_10110111 = x32+x26+x23+x22+x16+x12+x11+x10+x8+x7+x5+x4+x2+x1+x0
        P0x1EDC6F41 = 0x1EDC6F41,
        P0xA833982B = 0xA833982B,
        P0x814141AB = 0x814141AB,
        P0x000000AF = 0x000000AF,

        ZipInv = P0x04C11DB7,
        Zip = P0xEDB88320,
        POSIX = P0x04C11DB7,
        CRC32C = P0x1EDC6F41,
        CRC32D = P0xA833982B,
        CRC32Q = P0x814141AB,
        XFER = P0x000000AF,
    }

    /// <summary>Отражение байта</summary>
    /// <param name="b">Байт для отражения</param>
    /// <returns>Отражённый байт</returns>
    private static byte ReflectByte(byte b) =>
        (byte)(((b & 0x01) << 7) |
               ((b & 0x02) << 5) |
               ((b & 0x04) << 3) |
               ((b & 0x08) << 1) |
               ((b & 0x10) >> 1) |
               ((b & 0x20) >> 3) |
               ((b & 0x40) >> 5) |
               ((b & 0x80) >> 7));

    /// <summary>Отражение 32-битного значения</summary>
    /// <param name="x">Значение для отражения</param>
    /// <returns>Отражённое значение</returns>
    private static uint ReflectUInt(uint x)
    {
        x = ((x & 0x55555555) << 1) | ((x & 0xAAAAAAAA) >> 1);
        x = ((x & 0x33333333) << 2) | ((x & 0xCCCCCCCC) >> 2);
        x = ((x & 0x0F0F0F0F) << 4) | ((x & 0xF0F0F0F0) >> 4);
        x = ((x & 0x00FF00FF) << 8) | ((x & 0xFF00FF00) >> 8);
        x = ((x & 0x0000FFFF) << 16) | ((x & 0xFFFF0000) >> 16);
        return x;
    }

    /// <summary>Генерирует таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="poly">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Таблица коэффициентов для вычисления CRC</returns>
    public static uint[] GetTable(uint poly, bool RefIn) => FillTable(new uint[256], poly, RefIn);

    /// <summary>Заполняет таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="table">Таблица для заполнения</param>
    /// <param name="poly">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Заполненная таблица коэффициентов для вычисления CRC</returns>
    public static uint[] FillTable(uint[] table, uint poly, bool RefIn)
    {
        for (uint i = 0; i < 256; i++)
        {
            ref var entry = ref table[i];
            entry = RefIn ? ReflectUInt(i) : i;

            entry <<= 24;
            for (var j = 0; j < 8; j++)
                entry = (entry & 0x80000000) != 0
                    ? (entry << 1) ^ poly
                    : entry << 1;

            if (RefIn)
                entry = ReflectUInt(entry);
        }

        return table;
    }

    private static readonly ConcurrentDictionary<(uint Polynomial, bool RefIn), uint[]> __CRCTableCache = [];

    /// <summary>Синхронный метод-расширение для вычисления CRC-32 для потока</summary>
    /// <param name="stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="poly">Полином для вычисления CRC-32</param>
    /// <param name="CRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-32</returns>
    public static uint Hash(
        byte[] data,
        uint poly = 0x04C11DB7,
        uint CRC = 0xFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFF)
    {
        data.NotNull();

        var table = __CRCTableCache
            .GetOrAdd(
                (poly, RefIn),
                key => GetTable(key.Polynomial, key.RefIn));

        if (RefIn)
            foreach (var b in data)
            {
                var index = ((CRC ^ (uint)(ReflectByte(b) << 24)) & 0xFF000000) >> 24;
                CRC = (CRC << 8) ^ table[index];
            }
        else
            foreach (var b in data)
            {
                var index = ((CRC ^ (uint)(b << 24)) & 0xFF000000) >> 24;
                CRC = (CRC << 8) ^ table[index];
            }

        if (RefOut)
            CRC = ReflectUInt(CRC);

        return CRC ^ XOROut;
    }

#if NET5_0_OR_GREATER
    /// <summary>Синхронный метод-расширение для вычисления CRC-32 для потока</summary>
    /// <param name="stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="poly">Полином для вычисления CRC-32</param>
    /// <param name="CRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-32</returns>
    public static uint Hash(
        Stream stream,
        uint poly = 0x04C11DB7,
        uint CRC = 0xFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFF)
    {
        stream.NotNull();

        var table = __CRCTableCache
            .GetOrAdd(
                (poly, RefIn),
                key => GetTable(key.Polynomial, key.RefIn));

        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = stream.Read(buffer)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(ReflectByte(buffer[i]) << 24)) & 0xFF000000) >> 24;
                    CRC = (CRC << 8) ^ table[index];
                }
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(buffer[i] << 24)) & 0xFF000000) >> 24;
                    CRC = (CRC << 8) ^ table[index];
                }

        if (RefOut)
            CRC = ReflectUInt(CRC);

        return CRC ^ XOROut;
    }

    /// <summary>Метод-расширение для вычисления CRC-32 для потока</summary>
    /// <param name="stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="CRC">Начальное значение суммы</param>
    /// <param name="poly">Полином для вычисления CRC-32</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XorOut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Отмена операции</param>
    /// <returns>Значение CRC-32</returns>
    public static async Task<uint> GetCRC32Async(
        Stream stream,
        uint CRC = 0xFFFFFFFF,
        uint poly = 0x04C11DB7,
        bool RefIn = false,
        bool RefOut = false,
        uint XorOut = 0xFFFFFFFF,
        CancellationToken Cancel = default)
    {
        stream.NotNull();

        var table = __CRCTableCache.GetOrAdd(
            (poly, RefIn),
            key => GetTable(key.Polynomial, key.RefIn));

        int bytes_read;
        var buffer = new byte[8192];

        while ((bytes_read = await stream.ReadAsync(buffer, Cancel).ConfigureAwait(false)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(ReflectByte(buffer[i]) << 24)) & 0xFF000000) >> 24;
                    CRC = (CRC << 8) ^ table[index];
                }
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(buffer[i] << 24)) & 0xFF000000) >> 24;
                    CRC = (CRC << 8) ^ table[index];
                }

        if (RefOut)
            CRC = ReflectUInt(CRC);

        return CRC ^ XorOut;
    }
#endif
}
