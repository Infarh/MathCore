#nullable enable
namespace MathCore.Hash.CRC;

public class CRC8(byte Polynimial)
{
    public enum Mode : byte
    {
        P0x07 = 0x07,
        P0x9B = 0x9B,
        P0x39 = 0x39,
        P0xD5 = 0xD5,
        P0x1D = 0x1D,
        P0x31 = 0x31,

        CRC8 = P0x07,
        CDMA2000 = P0x9B,
        DARC = P0x39,
        DVB_S2 = P0xD5,
        EBU = P0x1D,
        ITU = P0x07,
        MAXIM = P0x31,
    }

    public static byte Hash(byte[] data, Mode mode = Mode.CRC8, byte crc = 0xFF, byte xor = 0xFF)
    {
        var poly = (byte)mode;

        foreach (var b in data)
            crc = (byte)(crc >> 8 ^ Table((crc ^ b) & 0xFF, poly));

        return (byte)(crc ^ xor);

        static byte Table(int i, byte poly)
        {
            var temp = i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = temp << 1 ^ poly;
                else
                    temp <<= 1;
            return (byte)temp;
        }
    }

    private const int __TableLength = 256;
    private readonly byte[] _Table = GetTable(Polynimial);

    public byte State { get; set; }

    public bool UpdateState { get; set; }

    public byte XOR { get; set; } = 0;

    public CRC8(Mode mode = Mode.CRC8) : this((byte)mode) { }

    public static byte[] GetTable(byte Polynimial)
    {
        var table = new byte[__TableLength];
        for (var i = 0; i < __TableLength; i++)
        {
            var temp = i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = temp << 1 ^ Polynimial;
                else
                    temp <<= 1;
            table[i] = (byte)temp;
        }

        return table;
    }

    public static void FillTable(byte[] table, byte Polynimial)
    {
        if (table is not { Length: __TableLength })
            throw new ArgumentException($"Размер таблицы должен быть {__TableLength}, а составляет {table.Length}", nameof(table));
        for (var i = 0; i < __TableLength; i++)
        {
            var temp = i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = temp << 1 ^ Polynimial;
                else
                    temp <<= 1;
            table[i] = (byte)temp;
        }
    }

    public byte Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public byte ContinueCompute(byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc >> 8 ^ _Table[(crc ^ b) & 0xFF]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

    public byte Compute(IEnumerable<byte> bytes) => ContinueCompute(State, bytes);

    public byte ContinueCompute(byte crc, IEnumerable<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);
        crc ^= XOR;
    }

    public void Compute(ref byte crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);
        crc ^= XOR;
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return [crc];
    }

    private static byte Convert(byte b, byte State, byte[] Table) => (byte)(State << 8 ^ Table[State >> 8 ^ b]);
}
