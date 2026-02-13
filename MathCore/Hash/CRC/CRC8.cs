// ReSharper disable InconsistentNaming
using System.Collections.Concurrent;

using MathCore.Annotations;

namespace MathCore.Hash.CRC;

/// <summary>Класс для вычисления контрольной суммы CRC-8</summary>
public class CRC8
{
    [PublicAPI]
    public enum Mode : byte
    {
        P0x07 = 0x07,
        P0x9B = 0x9B,
        P0x39 = 0x39,
        P0xD5 = 0xD5,
        P0x1D = 0x1D,
        P0x31 = 0x31,

        /// <summary>Стандартный полином CRC-8 (x^8 + x^2 + x + 1) 0x07</summary>
        CRC8 = P0x07,
        /// <summary>Полином CDMA2000 0x9B</summary>
        CDMA2000 = P0x9B,
        /// <summary>Полином DARC 0x39</summary>
        DARC = P0x39,
        /// <summary>Полином DVB-S2 0xD5</summary>
        DVB_S2 = P0xD5,
        /// <summary>Полином EBU 0x1D</summary>
        EBU = P0x1D,
        /// <summary>Полином ITU 0x07</summary>
        ITU = P0x07,
        /// <summary>Полином MAXIM 0x31</summary>
        MAXIM = P0x31,
    }

    private static readonly ConcurrentDictionary<(byte Polynomial, bool RefIn), byte[]> __CRCTableCache = [];

    private readonly byte _Polynomial;
    private readonly byte[] _Table;
    private readonly bool _RefIn;
    private readonly bool _RefOut;
    private readonly byte _InitialValue;
    private readonly byte _XOROut;
    
    private byte _State;

    /// <summary>Полином для вычисления CRC</summary>
    public byte Polynomial => _Polynomial;

    /// <summary>Отражение входных байтов</summary>
    public bool RefIn => _RefIn;

    /// <summary>Отражение выходного значения</summary>
    public bool RefOut => _RefOut;

    /// <summary>Начальное значение CRC</summary>
    public byte InitialValue => _InitialValue;

    /// <summary>Значение для XOR с окончательным CRC</summary>
    public byte XOROut => _XOROut;

    /// <summary>Текущее состояние вычисления CRC</summary>
    public byte State
    {
        get => _State;
        set => _State = value;
    }

    /// <summary>Инициализирует экземпляр CRC8 с заданным полиномом</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x00)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x00)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    public CRC8(
        byte Polynomial = (byte)Mode.CRC8,
        byte InitialValue = 0x00,
        byte XOROut = 0x00,
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
    public byte Compute(byte[] Data)
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
    public byte Compute(ReadOnlySpan<byte> Data)
    {
        Reset();
        ContinueCompute(Data);
        return GetResult();
    }

    /// <summary>Вычисляет CRC для данных из потока</summary>
    /// <param name="Stream">Поток данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public byte Compute(Stream Stream)
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
    public async Task<byte> ComputeAsync(Stream Stream, CancellationToken Cancel = default)
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
                _State = _Table[_State ^ b];
        else
            foreach (var b in Data)
                _State = _Table[_State ^ b];
    }

#if NET5_0_OR_GREATER
    /// <summary>Продолжает вычисление CRC для новых данных</summary>
    /// <param name="Data">Диапазон данных</param>
    public void ContinueCompute(ReadOnlySpan<byte> Data)
    {
        if (_RefIn)
            foreach (var b in Data)
                _State = _Table[_State ^ b];
        else
            foreach (var b in Data)
                _State = _Table[_State ^ b];
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
    public byte GetResult()
    {
        var crc = _State;
        if (_RefOut)
            crc = crc.ReverseBits();
        return (byte)(crc ^ _XOROut);
    }

    /// <summary>Получает итоговое значение CRC в виде массива байтов</summary>
    /// <returns>Массив из 1 байта с вычисленным значением CRC</returns>
    public byte[] ComputeChecksumBytes(byte[] Data) => [Compute(Data)];

    private static byte[] CreateTable(byte polynomial, bool refIn)
    {
        var table = new byte[256];
        return FillTable(table, polynomial, refIn);
    }

    /// <summary>Генерирует таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Таблица коэффициентов для вычисления CRC</returns>
    public static byte[] GetTable(byte Polynomial, bool RefIn) =>
        __CRCTableCache.GetOrAdd(
            (Polynomial, RefIn),
            key => CreateTable(key.Polynomial, key.RefIn));

    /// <summary>Заполняет таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Table">Таблица для заполнения</param>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Заполненная таблица коэффициентов для вычисления CRC</returns>
    public static byte[] FillTable(byte[] Table, byte Polynomial, bool RefIn)
    {
        if (RefIn)
        {
            for (var i = 0; i < 256; i++)
            {
                var crc = (byte)i;
                for (var j = 0; j < 8; j++)
                    crc = (crc & 1) != 0
                        ? (byte)((crc >> 1) ^ Polynomial)
                        : (byte)(crc >> 1);
                Table[i] = crc;
            }
        }
        else
        {
            for (var i = 0; i < 256; i++)
            {
                var crc = (byte)i;
                for (var j = 0; j < 8; j++)
                    crc = (crc & 0x80) != 0
                        ? (byte)((crc << 1) ^ Polynomial)
                        : (byte)(crc << 1);
                Table[i] = crc;
            }
        }

        return Table;
    }

    /// <summary>Статический метод для вычисления CRC-8 для массива байт</summary>
    /// <param name="Data">Массив данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-8</returns>
    public static byte Hash(
        byte[] Data,
        byte Polynomial = 0x07,
        byte InitialCRC = 0x00,
        bool RefIn = false,
        bool RefOut = false,
        byte XOROut = 0x00)
    {
        Data.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;

        foreach (var b in Data)
            crc = table[crc ^ b];

        if (RefOut)
            crc = crc.ReverseBits();

        return (byte)(crc ^ XOROut);
    }

#if NET5_0_OR_GREATER
    /// <summary>Статический метод для вычисления CRC-8 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-8</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-8</returns>
    public static byte Hash(
        Stream Stream,
        byte Polynomial = 0x07,
        byte InitialCRC = 0x00,
        bool RefIn = false,
        bool RefOut = false,
        byte XOROut = 0x00)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = Stream.Read(buffer)) > 0)
            for (var i = 0; i < bytes_read; i++)
                crc = table[crc ^ buffer[i]];

        if (RefOut)
            crc = crc.ReverseBits();

        return (byte)(crc ^ XOROut);
    }

    /// <summary>Асинхронный метод для вычисления CRC-8 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-8</param>
    /// <param name="Polynomial">Полином для вычисления CRC-8</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Значение CRC-8</returns>
    public static async Task<byte> HashAsync(
        Stream Stream,
        byte Polynomial = 0x07,
        byte InitialCRC = 0x00,
        bool RefIn = false,
        bool RefOut = false,
        byte XOROut = 0x00,
        CancellationToken Cancel = default)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        var buffer = new byte[8192];

        while ((bytes_read = await Stream.ReadAsync(buffer, Cancel).ConfigureAwait(false)) > 0)
            for (var i = 0; i < bytes_read; i++)
                crc = table[crc ^ buffer[i]];

        if (RefOut)
            crc = crc.ReverseBits();

        return (byte)(crc ^ XOROut);
    }
#endif
}
