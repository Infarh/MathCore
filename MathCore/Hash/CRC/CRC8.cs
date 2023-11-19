#nullable enable
namespace MathCore.Hash.CRC;

public class CRC8(byte Polynimial)
{
    public enum Mode : byte
    {
        CRC8 = 0x07,
        CDMA2000 = 0x9B,
        DARC = 0x39,
        DVB_S2 = 0xd5,
        EBU = 0x1D,
        ITU = 0x07,
        MAXIM = 0x31,
    }

    private const int __TableLength = 256;
    private readonly byte[] _Table = GetTable(Polynimial);

    public byte State { get; set; }

    public bool UpdateState { get; set; }

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

        if (UpdateState)
            State = crc;

        return crc;
    }

    public byte Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public byte ContinueCompute(byte crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);
    }

    public void Compute(ref byte crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)(crc << 8 ^ _Table[crc >> 8 ^ b]);
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return new[] { crc };
    }

    public static byte Compute(Stream stream, byte Polynimial, byte State = 0)
    {
        const int buffer_size = 512;
        if (stream.Length <= buffer_size)
            State = stream
                .ToArray()
                .Aggregate((State, Table : GetTable(Polynimial)), (v, b) => (Convert(b, v.State, v.Table), v.Table))
                .State;
        else
        {
            var table = GetTable(Polynimial);
            var buffer = new byte[buffer_size];
            while(stream.Read(buffer, 0, buffer_size) is > 0 and var readed)
                for(var i = 0; i < readed; i++)
                    State = Convert(buffer[i], State, table);
        }

        return State;
    }

    private static byte Convert(byte b, byte State, byte[] Table) => (byte)(State << 8 ^ Table[State >> 8 ^ b]);
}
