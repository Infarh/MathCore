// ReSharper disable InconsistentNaming
#nullable enable
namespace MathCore.Hash.CRC;

/// <summary>Реализация алгоритма вычисления CRC8 с поддержкой различных полиномов и режимов</summary>
/// <remarks>Создаёт экземпляр CRC8 с выбранным полиномом.</remarks>
/// <param name="Polynomial">Полином CRC8</param>
/// <param name="RefIn">Инвертировать входные байты</param>
public class CRC8(byte Polynomial, bool RefIn = false)
{
    private const int __TableLength = 256;

    /// <summary>Таблица CRC8 для текущего полинома.</summary>
    private readonly byte[] _Table = GetTable(Polynomial, RefIn);

    /// <summary>Текущее состояние CRC.</summary>
    public byte State { get; set; }

    /// <summary>Обновлять ли состояние State после вычисления.</summary>
    public bool UpdateState { get; init; }

    /// <summary>Значение для XOR на выходе.</summary>
    public byte XOR { get; init; } = 0;

    /// <summary>Инвертировать входные байты.</summary>
    public bool RefIn { get; init; } = RefIn;

    /// <summary>Инвертировать результат.</summary>
    public bool RefOut { get; init; }

    /// <summary>Создаёт экземпляр CRC8 с выбранным режимом.</summary>
    /// <param name="mode">Режим/полином CRC8</param>
    /// <param name="RefIn">Инвертировать входные байты</param>
    public CRC8(Mode mode = Mode.CRC8, bool RefIn = false) : this((byte)mode, RefIn) { }

    /// <summary>Получить таблицу CRC8 для заданного полинома и режима отражения входных байтов.</summary>
    /// <param name="Polynomial">Полином CRC8</param>
    /// <param name="RefIn">Отражать входные байты</param>
    /// <returns>Таблица CRC8</returns>
    public static byte[] GetTable(byte Polynomial, bool RefIn = false)
    {
        var table = new byte[__TableLength];
        FillTable(table, Polynomial, RefIn);
        return table;
    }

    /// <summary>Заполнить таблицу CRC8 для заданного полинома и режима отражения входных байтов.</summary>
    /// <param name="table">Таблица для заполнения</param>
    /// <param name="Polynomial">Полином CRC8</param>
    /// <param name="RefIn">Отражать входные байты</param>
    public static void FillTable(byte[] table, byte Polynomial, bool RefIn = false)
    {
        if (table == null || table.Length != __TableLength)
            throw new ArgumentException($"Размер таблицы должен быть {__TableLength}, а составляет {table?.Length}", nameof(table));

        for (var i = 0; i < __TableLength; i++)
        {
            var temp = RefIn ? ((byte)i).ReverseBits() : i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = temp << 1 ^ Polynomial;
                else
                    temp <<= 1;
            table[i] = (byte)(RefIn ? ((byte)temp).ReverseBits() : temp); // отражаем и индекс, и результат при RefIn
        }
    }

    /// <summary>Варианты стандартных полиномов CRC8.</summary>
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

    /// <summary>Вычислить CRC8 для массива байт с заданными параметрами</summary>
    /// <param name="data">Входные данные</param>
    /// <param name="mode">Режим/полином CRC8</param>
    /// <param name="crc">Начальное значение CRC</param>
    /// <param name="xor">Значение для XOR на выходе</param>
    /// <param name="RefIn">Инвертировать входные байты</param>
    /// <param name="RefOut">Инвертировать результат</param>
    /// <returns>Контрольная сумма CRC8</returns>
    public static byte Hash(
        byte[] data,
        Mode mode = Mode.CRC8,
        byte crc = 0xFF,
        byte xor = 0xFF,
        bool RefIn = false,
        bool RefOut = false)
    {
        if (data.NotNull().Length == 0)
            throw new InvalidOperationException();

        var table = GetTable((byte)mode, RefIn);

        foreach (var b in data)
            crc = table[(crc ^ b) & 0xFF];

        crc ^= xor;

        if (RefOut)
            crc = crc.ReverseBits();

        return crc;
    }

    /// <summary>Вычислить CRC8 для массива байт</summary>
    /// <param name="bytes">Входные данные</param>
    /// <returns>Контрольная сумма CRC8</returns>
    public byte Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    /// <summary>Продолжить вычисление CRC8 для массива байт с заданным начальным значением</summary>
    /// <param name="crc">Начальное значение CRC</param>
    /// <param name="bytes">Входные данные</param>
    /// <returns>Контрольная сумма CRC8</returns>
    public byte ContinueCompute(byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = _Table[(crc ^ b) & 0xFF];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        if (RefOut)
            crc = crc.ReverseBits();

        return crc;
    }

    /// <summary>Вычислить CRC8 для последовательности байт</summary>
    /// <param name="bytes">Входные данные</param>
    /// <returns>Контрольная сумма CRC8</returns>
    public byte Compute(IEnumerable<byte> bytes) => ContinueCompute(State, bytes);

    /// <summary>Продолжить вычисление CRC8 для последовательности байт с заданным начальным значением</summary>
    /// <param name="crc">Начальное значение CRC</param>
    /// <param name="bytes">Входные данные</param>
    /// <returns>Контрольная сумма CRC8</returns>
    public byte ContinueCompute(byte crc, IEnumerable<byte> bytes)
    {
        foreach (var b in bytes)
            crc = _Table[(crc ^ b) & 0xFF];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        if (RefOut)
            crc = crc.ReverseBits();

        return crc;
    }

    /// <summary>Продолжить вычисление CRC8 для массива байт с передачей CRC по ссылке</summary>
    /// <param name="crc">CRC по ссылке</param>
    /// <param name="bytes">Входные данные</param>
    public void Compute(ref byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = _Table[(crc ^ b) & 0xFF];

        crc ^= XOR;

        if (RefOut)
            crc = crc.ReverseBits();
    }

    /// <summary>Продолжить вычисление CRC8 для последовательности байт с передачей CRC по ссылке</summary>
    /// <param name="crc">CRC по ссылке</param>
    /// <param name="bytes">Входные данные</param>
    public void Compute(ref byte crc, IEnumerable<byte> bytes)
    {
        foreach (var b in bytes)
            crc = _Table[(crc ^ b) & 0xFF];

        crc ^= XOR;

        if (RefOut)
            crc = crc.ReverseBits();
    }

    /// <summary>Получить контрольную сумму CRC8 в виде массива байт</summary>
    /// <param name="bytes">Входные данные</param>
    /// <returns>Массив из одного байта с контрольной суммой CRC8</returns>
    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return [crc];
    }
}
