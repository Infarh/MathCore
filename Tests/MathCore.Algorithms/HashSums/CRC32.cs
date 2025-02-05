using System.Collections.Concurrent;

namespace MathCore.Algorithms.HashSums;

internal static class CRC32
{
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
    public static uint[] GetTable(uint poly, bool RefIn)
    {
        var table = new uint[256];
        //for (uint i = 0; i < 256; i++)
        //{
        //    ref var entry = ref table[i];
        //    entry = RefIn ? ReflectUInt(i) : i;

        //    entry <<= 24;

        //    for (var j = 0; j < 8; j++)
        //    {
        //        if ((entry & 0x80000000) != 0)
        //            entry = (entry << 1) ^ poly;
        //        else
        //            entry <<= 1;
        //    }

        //    if (RefIn)
        //        entry = ReflectUInt(entry);
        //}

        return FillTable(table, poly, RefIn);
    }

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

    /// <summary>Метод-расширение для вычисления CRC-32 для потока</summary>
    /// <param name="stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="CRC">Начальное значение</param>
    /// <param name="poly">Полином для вычисления CRC-32</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XorOut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Отмена операции</param>
    /// <returns>Значение CRC-32</returns>
    public static async Task<uint> GetCRC32Async(
        this Stream stream,
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
                    var lookup = table[index];
                    CRC = (CRC << 8) ^ lookup;
                }
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(buffer[i] << 24)) & 0xFF000000) >> 24;
                    var lookup = table[index];
                    CRC = (CRC << 8) ^ lookup;
                }

        if (RefOut)
            CRC = ReflectUInt(CRC);

        return CRC ^ XorOut;
    }

    /// <summary>Синхронный метод-расширение для вычисления CRC-32 для потока</summary>
    /// <param name="stream">Поток, для которого вычисляется CRC-32.</param>
    /// <param name="poly">Полином для вычисления CRC-32.</param>
    /// <param name="CRC"></param>
    /// <param name="RefIn">Отражение входных байтов.</param>
    /// <param name="RefOut">Отражение выходного значения CRC.</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC.</param>
    /// <returns>Значение CRC-32.</returns>
    public static uint GetCRC32(this Stream stream,
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
                    var lookup = table[index];
                    CRC = (CRC << 8) ^ lookup;
                }
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((CRC ^ (uint)(buffer[i] << 24)) & 0xFF000000) >> 24;
                    var lookup = table[index];
                    CRC = (CRC << 8) ^ lookup;
                }

        if (RefOut)
            CRC = ReflectUInt(CRC);

        return CRC ^ XOROut;
    }
}
