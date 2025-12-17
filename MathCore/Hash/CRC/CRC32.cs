using System.Collections.Concurrent;

using MathCore.Annotations;

namespace MathCore.Hash.CRC;

// https://microsin.net/programming/arm/crc32-demystified.html

/// <summary>Класс для вычисления контрольной суммы CRC-32</summary>
public class CRC32
{
    [PublicAPI]
    public enum Mode : uint
    {
        /// <summary>Инвертированный полином относительно <see cref="P0x04C11DB7"/></summary>
        P0xEDB88320 = 0xEDB88320,
        /// <summary>Нормальный полином относительно <see cref="P0xEDB88320"/></summary>
        P0x04C11DB7 = 0x04C11DB7,
        P0x1EDC6F41 = 0x1EDC6F41,
        P0xA833982B = 0xA833982B,
        P0x814141AB = 0x814141AB,
        P0x000000AF = 0x000000AF,

        ZipInv = P0x04C11DB7,
        Zip = P0xEDB88320,
        POSIX = P0x04C11DB7,
        CRC32C = 0x1EDC6F41,
        CRC32D = 0xA833982B,
        CRC32Q = 0x814141AB,
        XFER = 0x000000AF,
    }

    private static readonly ConcurrentDictionary<(uint Polynomial, bool RefIn), uint[]> __CRCTableCache = [];

    private readonly uint _Polynomial;
    private readonly uint[] _Table;
    private readonly bool _RefIn;
    private readonly bool _RefOut;
    private readonly uint _InitialValue;
    private readonly uint _XOROut;
    
    private uint _State;

    /// <summary>Полином для вычисления CRC</summary>
    public uint Polynomial => _Polynomial;

    /// <summary>Отражение входных байтов</summary>
    public bool RefIn => _RefIn;

    /// <summary>Отражение выходного значения</summary>
    public bool RefOut => _RefOut;

    /// <summary>Начальное значение CRC</summary>
    public uint InitialValue => _InitialValue;

    /// <summary>Значение для XOR с окончательным CRC</summary>
    public uint XOROut => _XOROut;

    /// <summary>Текущее состояние вычисления CRC</summary>
    public uint State
    {
        get => _State;
        set => _State = value;
    }

    /// <summary>Инициализирует экземпляр CRC32 с заданным полиномом</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0xFFFFFFFF)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    public CRC32(
        uint Polynomial = (uint)Mode.ZipInv,
        uint InitialValue = 0xFFFFFFFF,
        uint XOROut = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false)
    {
        _Polynomial = Polynomial;
        _InitialValue = InitialValue;
        _XOROut = XOROut;
        _RefIn = RefIn;
        _RefOut = RefOut;
        _State = InitialValue;

        _Table = __CRCTableCache.GetOrAdd(
            (Polynomial, RefIn),
            key => CreateTable(key.Polynomial, key.RefIn));
    }

    /// <summary>Сбрасывает состояние CRC к начальному значению</summary>
    public void Reset() => _State = _InitialValue;

    /// <summary>Вычисляет CRC для данных и сбрасывает состояние</summary>
    /// <param name="Data">Массив данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public uint Compute(byte[] Data)
    {
        Data.NotNull();
        Reset();
        ContinueCompute(Data);
        return GetResult();
    }

#if NET5_0_OR_GREATER
    /// <summary>Вычисляет CRC для данных и сбрасывает состояние</summary>
    /// <param name="Data">Диапазон данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public uint Compute(ReadOnlySpan<byte> Data)
    {
        Reset();
        ContinueCompute(Data);
        return GetResult();
    }

    /// <summary>Вычисляет CRC для данных из потока</summary>
    /// <param name="Stream">Поток данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public uint Compute(Stream Stream)
    {
        Stream.NotNull();
        Reset();
        ContinueCompute(Stream);
        return GetResult();
    }

    /// <summary>Вычисляет CRC для данных из потока асинхронно</summary>
    /// <param name="Stream">Поток данных</param>
    /// <param name="Cancel">Токен отмены</param>
    /// <returns>Вычисленное значение CRC</returns>
    public async Task<uint> ComputeAsync(Stream Stream, CancellationToken Cancel = default)
    {
        Stream.NotNull();
        Reset();
        await ContinueComputeAsync(Stream, Cancel).ConfigureAwait(false);
        return GetResult();
    }
#endif

    /// <summary>Продолжает вычисление CRC для новых данных</summary>
    /// <param name="Data">Массив данных</param>
    public void ContinueCompute(byte[] Data)
    {
        Data.NotNull();

        if (_RefIn)
            foreach (var b in Data)
                _State = _Table[(_State ^ b) & 0xFF] ^ (_State >> 8);
        else
            foreach (var b in Data)
            {
                var index = ((_State >> 24) ^ b) & 0xFF;
                _State = (_State << 8) ^ _Table[index];
            }
    }

#if NET5_0_OR_GREATER
    /// <summary>Продолжает вычисление CRC для новых данных</summary>
    /// <param name="Data">Диапазон данных</param>
    public void ContinueCompute(ReadOnlySpan<byte> Data)
    {
        if (_RefIn)
            foreach (var b in Data)
                _State = _Table[(_State ^ b) & 0xFF] ^ (_State >> 8);
        else
            foreach (var b in Data)
            {
                var index = ((_State >> 24) ^ b) & 0xFF;
                _State = (_State << 8) ^ _Table[index];
            }
    }

    /// <summary>Продолжает вычисление CRC для данных из потока</summary>
    /// <param name="Stream">Поток данных</param>
    public void ContinueCompute(Stream Stream)
    {
        Stream.NotNull();

        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = Stream.Read(buffer)) > 0)
            ContinueCompute(buffer[..bytes_read]);
    }

    /// <summary>Продолжает вычисление CRC для данных из потока асинхронно</summary>
    /// <param name="Stream">Поток данных</param>
    /// <param name="Cancel">Токен отмены</param>
    public async Task ContinueComputeAsync(Stream Stream, CancellationToken Cancel = default)
    {
        Stream.NotNull();

        int bytes_read;
        var buffer = new byte[8192];

        while ((bytes_read = await Stream.ReadAsync(buffer, Cancel).ConfigureAwait(false)) > 0)
            ContinueCompute(buffer.AsSpan(0, bytes_read));
    }
#endif

    /// <summary>Получает итоговое значение CRC</summary>
    /// <returns>Вычисленное значение CRC</returns>
    public uint GetResult()
    {
        var crc = _State;
        if (_RefOut)
            crc = ReflectUInt(crc);
        return crc ^ _XOROut;
    }

    /// <summary>Получает итоговое значение CRC в виде массива байтов</summary>
    /// <returns>Массив из 4 байтов с вычисленным значением CRC</returns>
    public byte[] ComputeChecksumBytes(byte[] Data)
    {
        var crc = Compute(Data);
        return
        [
            (byte)(crc >> 24),
            (byte)(crc >> 16),
            (byte)(crc >> 8),
            (byte)crc
        ];
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

    private static uint[] CreateTable(uint polynomial, bool refIn)
    {
        var table = new uint[256];
        return FillTable(table, polynomial, refIn);
    }

    /// <summary>Генерирует таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Таблица коэффициентов для вычисления CRC</returns>
    public static uint[] GetTable(uint Polynomial, bool RefIn) =>
        __CRCTableCache.GetOrAdd(
            (Polynomial, RefIn),
            key => CreateTable(key.Polynomial, key.RefIn));

    /// <summary>Заполняет таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Table">Таблица для заполнения</param>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Заполненная таблица коэффициентов для вычисления CRC</returns>
    public static uint[] FillTable(uint[] Table, uint Polynomial, bool RefIn)
    {
        if (RefIn)
        {
            for (uint i = 0; i < 256; i++)
            {
                var crc = i;
                for (var j = 0; j < 8; j++)
                    crc = (crc & 1) != 0
                        ? (crc >> 1) ^ Polynomial
                        : crc >> 1;
                Table[i] = crc;
            }
        }
        else
        {
            for (uint i = 0; i < 256; i++)
            {
                var crc = i << 24;
                for (var j = 0; j < 8; j++)
                    crc = (crc & 0x80000000) != 0
                        ? (crc << 1) ^ Polynomial
                        : crc << 1;
                Table[i] = crc;
            }
        }

        return Table;
    }

    /// <summary>Статический метод для вычисления CRC-32 для массива байт</summary>
    /// <param name="Data">Массив данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-32</returns>
    public static uint Hash(
        byte[] Data,
        uint Polynomial = 0x04C11DB7,
        uint InitialCRC = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFFFF)
    {
        Data.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;

        if (RefIn)
            foreach (var b in Data)
                crc = table[(crc ^ b) & 0xFF] ^ (crc >> 8);
        else
            foreach (var b in Data)
            {
                var index = ((crc >> 24) ^ b) & 0xFF;
                crc = (crc << 8) ^ table[index];
            }

        if (RefOut)
            crc = ReflectUInt(crc);

        return crc ^ XOROut;
    }

#if NET5_0_OR_GREATER
    /// <summary>Статический метод для вычисления CRC-32 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-32</returns>
    public static uint Hash(
        Stream Stream,
        uint Polynomial = 0x04C11DB7,
        uint InitialCRC = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFFFF)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = Stream.Read(buffer)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                    crc = table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((crc >> 24) ^ buffer[i]) & 0xFF;
                    crc = (crc << 8) ^ table[index];
                }

        if (RefOut)
            crc = ReflectUInt(crc);

        return crc ^ XOROut;
    }

    /// <summary>Асинхронный метод для вычисления CRC-32 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-32</param>
    /// <param name="Polynomial">Полином для вычисления CRC-32</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Значение CRC-32</returns>
    public static async Task<uint> HashAsync(
        Stream Stream,
        uint Polynomial = 0x04C11DB7,
        uint InitialCRC = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false,
        uint XOROut = 0xFFFFFFFF,
        CancellationToken Cancel = default)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        var buffer = new byte[8192];

        while ((bytes_read = await Stream.ReadAsync(buffer, Cancel).ConfigureAwait(false)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                    crc = table[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
            else
                for (var i = 0; i < bytes_read; i++)
                {
                    var index = ((crc >> 24) ^ buffer[i]) & 0xFF;
                    crc = (crc << 8) ^ table[index];
                }

        if (RefOut)
            crc = ReflectUInt(crc);

        return crc ^ XOROut;
    }
#endif
}
