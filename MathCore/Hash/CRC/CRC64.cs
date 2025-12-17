using System.Collections.Concurrent;

using MathCore.Annotations;

namespace MathCore.Hash.CRC;

/// <summary>Класс для вычисления контрольной суммы CRC-64</summary>
public class CRC64
{
    [PublicAPI]
    public enum Mode : ulong
    {
        ISO3309 = 0x000000000000001B,
        ECMA = 0x42F0E1EBA9EA3693,
    }

    private static readonly ConcurrentDictionary<(ulong Polynomial, bool RefIn), ulong[]> __CRCTableCache = [];

    private readonly ulong _Polynomial;
    private readonly ulong[] _Table;
    private readonly bool _RefIn;
    private readonly bool _RefOut;
    private readonly ulong _InitialValue;
    private readonly ulong _XOROut;
    
    private ulong _State;

    /// <summary>Полином для вычисления CRC</summary>
    public ulong Polynomial => _Polynomial;

    /// <summary>Отражение входных байтов</summary>
    public bool RefIn => _RefIn;

    /// <summary>Отражение выходного значения</summary>
    public bool RefOut => _RefOut;

    /// <summary>Начальное значение CRC</summary>
    public ulong InitialValue => _InitialValue;

    /// <summary>Значение для XOR с окончательным CRC</summary>
    public ulong XOROut => _XOROut;

    /// <summary>Текущее состояние вычисления CRC</summary>
    public ulong State
    {
        get => _State;
        set => _State = value;
    }

    /// <summary>Инициализирует экземпляр CRC64 с заданным полиномом</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000000000000000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    public CRC64(
        ulong Polynomial = (ulong)Mode.ISO3309,
        ulong InitialValue = 0x0000000000000000,
        ulong XOROut = 0x0000000000000000,
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
    public ulong Compute(byte[] Data)
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
    public ulong Compute(ReadOnlySpan<byte> Data)
    {
        Reset();
        ContinueCompute(Data);
        return GetResult();
    }

    /// <summary>Вычисляет CRC для данных из потока</summary>
    /// <param name="Stream">Поток данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public ulong Compute(Stream Stream)
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
    public async Task<ulong> ComputeAsync(Stream Stream, CancellationToken Cancel = default)
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
                _State = (_State << 8) ^ _Table[(int)((_State >> 56) ^ b.ReverseBits())];
        else
            foreach (var b in Data)
                _State = (_State << 8) ^ _Table[(int)((_State >> 56) ^ b)];
    }

#if NET5_0_OR_GREATER
    /// <summary>Продолжает вычисление CRC для новых данных</summary>
    /// <param name="Data">Диапазон данных</param>
    public void ContinueCompute(ReadOnlySpan<byte> Data)
    {
        if (_RefIn)
            foreach (var b in Data)
                _State = (_State << 8) ^ _Table[(int)((_State >> 56) ^ b.ReverseBits())];
        else
            foreach (var b in Data)
                _State = (_State << 8) ^ _Table[(int)((_State >> 56) ^ b)];
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
    public ulong GetResult()
    {
        var crc = _State;
        if (_RefOut)
            crc = crc.ReverseBits();
        return crc ^ _XOROut;
    }

    /// <summary>Получает итоговое значение CRC в виде массива байтов</summary>
    /// <returns>Массив из 8 байтов с вычисленным значением CRC</returns>
    public byte[] ComputeChecksumBytes(byte[] Data)
    {
        var crc = Compute(Data);
        return
        [
            (byte)(crc >> 56),
            (byte)(crc >> 48),
            (byte)(crc >> 40),
            (byte)(crc >> 32),
            (byte)(crc >> 24),
            (byte)(crc >> 16),
            (byte)(crc >> 8),
            (byte)crc
        ];
    }

    private static ulong[] CreateTable(ulong polynomial, bool refIn)
    {
        var table = new ulong[256];
        return FillTable(table, polynomial, refIn);
    }

    /// <summary>Генерирует таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Таблица коэффициентов для вычисления CRC</returns>
    public static ulong[] GetTable(ulong Polynomial, bool RefIn) =>
        __CRCTableCache.GetOrAdd(
            (Polynomial, RefIn),
            key => CreateTable(key.Polynomial, key.RefIn));

    /// <summary>Заполняет таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Table">Таблица для заполнения</param>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Заполненная таблица коэффициентов для вычисления CRC</returns>
    public static ulong[] FillTable(ulong[] Table, ulong Polynomial, bool RefIn)
    {
        if (RefIn)
        {
            for (ulong i = 0; i < 256; i++)
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
            for (ulong i = 0; i < 256; i++)
            {
                var crc = i << 56;
                const ulong mask = 0x80_00_00_00_00_00_00_00UL;
                for (var j = 0; j < 8; j++)
                    crc = (crc & mask) != 0
                        ? (crc << 1) ^ Polynomial
                        : crc << 1;
                Table[i] = crc;
            }
        }

        return Table;
    }

    /// <summary>Статический метод для вычисления CRC-64 для массива байт</summary>
    /// <param name="Data">Массив данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-64</returns>
    public static ulong Hash(
        byte[] Data,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialCRC = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false,
        ulong XOROut = 0x0000000000000000)
    {
        Data.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;

        if (RefIn)
            foreach (var b in Data)
                crc = (crc << 8) ^ table[(int)((crc >> 56) ^ b.ReverseBits())];
        else
            foreach (var b in Data)
                crc = (crc << 8) ^ table[(int)((crc >> 56) ^ b)];

        if (RefOut)
            crc = crc.ReverseBits();

        return crc ^ XOROut;
    }

#if NET5_0_OR_GREATER
    /// <summary>Статический метод для вычисления CRC-64 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-64</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-64</returns>
    public static ulong Hash(
        Stream Stream,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialCRC = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false,
        ulong XOROut = 0x0000000000000000)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = Stream.Read(buffer)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                    crc = (crc << 8) ^ table[(int)((crc >> 56) ^ buffer[i].ReverseBits())];
            else
                for (var i = 0; i < bytes_read; i++)
                    crc = (crc << 8) ^ table[(int)((crc >> 56) ^ buffer[i])];

        if (RefOut)
            crc = crc.ReverseBits();

        return crc ^ XOROut;
    }

    /// <summary>Асинхронный метод для вычисления CRC-64 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-64</param>
    /// <param name="Polynomial">Полином для вычисления CRC-64</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Значение CRC-64</returns>
    public static async Task<ulong> HashAsync(
        Stream Stream,
        ulong Polynomial = 0x000000000000001B,
        ulong InitialCRC = 0x0000000000000000,
        bool RefIn = false,
        bool RefOut = false,
        ulong XOROut = 0x0000000000000000,
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
                    crc = (crc << 8) ^ table[(int)((crc >> 56) ^ buffer[i].ReverseBits())];
            else
                for (var i = 0; i < bytes_read; i++)
                    crc = (crc << 8) ^ table[(int)((crc >> 56) ^ buffer[i])];

        if (RefOut)
            crc = crc.ReverseBits();

        return crc ^ XOROut;
    }
#endif
}
