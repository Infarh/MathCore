using System.Collections.Concurrent;

using MathCore.Annotations;

namespace MathCore.Hash.CRC;

/// <summary>Класс для вычисления контрольной суммы CRC-16</summary>
public class CRC16
{
    [PublicAPI]
    public enum Mode : ushort
    {
        P0xA001 = 0xA001,
        P0x1021 = 0x1021,
        P0x8005 = 0x8005,
        P0x8408 = 0x8408,
        P0x8810 = 0x8810,

        IBM = P0xA001,
        CCITT = P0x1021,
        XMODEM = P0x1021,
        AUG_CCITT = P0x1021,
        CMS = P0x8005,
        CCITTKermit = P0x8408,
        CCITT16 = P0x8810
    }

    private static readonly ConcurrentDictionary<(ushort Polynomial, bool RefIn), ushort[]> __CRCTableCache = [];

    private readonly ushort _Polynomial;
    private readonly ushort[] _Table;
    private readonly bool _RefIn;
    private readonly bool _RefOut;
    private readonly ushort _InitialValue;
    private readonly ushort _XOROut;
    
    private ushort _State;

    /// <summary>Полином для вычисления CRC</summary>
    public ushort Polynomial => _Polynomial;

    /// <summary>Отражение входных байтов</summary>
    public bool RefIn => _RefIn;

    /// <summary>Отражение выходного значения</summary>
    public bool RefOut => _RefOut;

    /// <summary>Начальное значение CRC</summary>
    public ushort InitialValue => _InitialValue;

    /// <summary>Значение для XOR с окончательным CRC</summary>
    public ushort XOROut => _XOROut;

    /// <summary>Текущее состояние вычисления CRC</summary>
    public ushort State
    {
        get => _State;
        set => _State = value;
    }

    /// <summary>Инициализирует экземпляр CRC16 с заданным полиномом</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="InitialValue">Начальное значение CRC (по умолчанию 0x0000)</param>
    /// <param name="XOROut">Значение для XOR с окончательным CRC (по умолчанию 0x0000)</param>
    /// <param name="RefIn">Отражение входных байтов (по умолчанию false)</param>
    /// <param name="RefOut">Отражение выходного значения (по умолчанию false)</param>
    public CRC16(
        ushort Polynomial = (ushort)Mode.XMODEM,
        ushort InitialValue = 0x0000,
        ushort XOROut = 0x0000,
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
    public ushort Compute(byte[] Data)
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
    public ushort Compute(ReadOnlySpan<byte> Data)
    {
        Reset();
        ContinueCompute(Data);
        return GetResult();
    }

    /// <summary>Вычисляет CRC для данных из потока</summary>
    /// <param name="Stream">Поток данных</param>
    /// <returns>Вычисленное значение CRC</returns>
    public ushort Compute(Stream Stream)
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
    public async Task<ushort> ComputeAsync(Stream Stream, CancellationToken Cancel = default)
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
                _State = (ushort)((_State << 8) ^ _Table[(_State >> 8) ^ b.ReverseBits()]);
        else
            foreach (var b in Data)
                _State = (ushort)((_State << 8) ^ _Table[(_State >> 8) ^ b]);
    }

#if NET5_0_OR_GREATER
    /// <summary>Продолжает вычисление CRC для новых данных</summary>
    /// <param name="Data">Диапазон данных</param>
    public void ContinueCompute(ReadOnlySpan<byte> Data)
    {
        if (_RefIn)
            foreach (var b in Data)
                _State = (ushort)((_State << 8) ^ _Table[(_State >> 8) ^ b.ReverseBits()]);
        else
            foreach (var b in Data)
                _State = (ushort)((_State << 8) ^ _Table[(_State >> 8) ^ b]);
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
    public ushort GetResult()
    {
        var crc = _State;
        if (_RefOut)
            crc = crc.ReverseBits();
        return (ushort)(crc ^ _XOROut);
    }

    /// <summary>Получает итоговое значение CRC в виде массива байтов</summary>
    /// <returns>Массив из 2 байтов с вычисленным значением CRC</returns>
    public byte[] ComputeChecksumBytes(byte[] Data)
    {
        var crc = Compute(Data);
        return
        [
            (byte)(crc >> 8),
            (byte)crc
        ];
    }

    private static ushort[] CreateTable(ushort polynomial, bool refIn)
    {
        var table = new ushort[256];
        return FillTable(table, polynomial, refIn);
    }

    /// <summary>Генерирует таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Таблица коэффициентов для вычисления CRC</returns>
    public static ushort[] GetTable(ushort Polynomial, bool RefIn) =>
        __CRCTableCache.GetOrAdd(
            (Polynomial, RefIn),
            key => CreateTable(key.Polynomial, key.RefIn));

    /// <summary>Заполняет таблицу коэффициентов для вычисления CRC</summary>
    /// <param name="Table">Таблица для заполнения</param>
    /// <param name="Polynomial">Полином для вычисления CRC</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <returns>Заполненная таблица коэффициентов для вычисления CRC</returns>
    public static ushort[] FillTable(ushort[] Table, ushort Polynomial, bool RefIn)
    {
        if (RefIn)
        {
            for (var i = 0; i < 256; i++)
            {
                var crc = (ushort)i;
                for (var j = 0; j < 8; j++)
                    crc = (crc & 1) != 0
                        ? (ushort)((crc >> 1) ^ Polynomial)
                        : (ushort)(crc >> 1);
                Table[i] = crc;
            }
        }
        else
        {
            for (var i = 0; i < 256; i++)
            {
                var crc = (ushort)(i << 8);
                for (var j = 0; j < 8; j++)
                    crc = (crc & 0x8000) != 0
                        ? (ushort)((crc << 1) ^ Polynomial)
                        : (ushort)(crc << 1);
                Table[i] = crc;
            }
        }

        return Table;
    }

    /// <summary>Статический метод для вычисления CRC-16 для массива байт</summary>
    /// <param name="Data">Массив данных</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-16</returns>
    public static ushort Hash(
        byte[] Data,
        ushort Polynomial = 0x1021,
        ushort InitialCRC = 0x0000,
        bool RefIn = false,
        bool RefOut = false,
        ushort XOROut = 0x0000)
    {
        Data.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;

        if (RefIn)
            foreach (var b in Data)
                crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ b.ReverseBits()]);
        else
            foreach (var b in Data)
                crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ b]);

        if (RefOut)
            crc = crc.ReverseBits();

        return (ushort)(crc ^ XOROut);
    }

#if NET5_0_OR_GREATER
    /// <summary>Статический метод для вычисления CRC-16 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-16</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <returns>Значение CRC-16</returns>
    public static ushort Hash(
        Stream Stream,
        ushort Polynomial = 0x1021,
        ushort InitialCRC = 0x0000,
        bool RefIn = false,
        bool RefOut = false,
        ushort XOROut = 0x0000)
    {
        Stream.NotNull();

        var table = GetTable(Polynomial, RefIn);
        var crc = InitialCRC;
        int bytes_read;
        Span<byte> buffer = stackalloc byte[8192];

        while ((bytes_read = Stream.Read(buffer)) > 0)
            if (RefIn)
                for (var i = 0; i < bytes_read; i++)
                    crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ buffer[i].ReverseBits()]);
            else
                for (var i = 0; i < bytes_read; i++)
                    crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ buffer[i]]);

        if (RefOut)
            crc = crc.ReverseBits();

        return (ushort)(crc ^ XOROut);
    }

    /// <summary>Асинхронный метод для вычисления CRC-16 для потока</summary>
    /// <param name="Stream">Поток, для которого вычисляется CRC-16</param>
    /// <param name="Polynomial">Полином для вычисления CRC-16</param>
    /// <param name="InitialCRC">Начальное значение суммы</param>
    /// <param name="RefIn">Отражение входных байтов</param>
    /// <param name="RefOut">Отражение выходного значения CRC</param>
    /// <param name="XOROut">Значение для выполнения XOR с окончательным CRC</param>
    /// <param name="Cancel">Токен отмены операции</param>
    /// <returns>Значение CRC-16</returns>
    public static async Task<ushort> HashAsync(
        Stream Stream,
        ushort Polynomial = 0x1021,
        ushort InitialCRC = 0x0000,
        bool RefIn = false,
        bool RefOut = false,
        ushort XOROut = 0x0000,
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
                    crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ buffer[i].ReverseBits()]);
            else
                for (var i = 0; i < bytes_read; i++)
                    crc = (ushort)((crc << 8) ^ table[(crc >> 8) ^ buffer[i]]);

        if (RefOut)
            crc = crc.ReverseBits();

        return (ushort)(crc ^ XOROut);
    }
#endif
}
