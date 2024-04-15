using System.Diagnostics.CodeAnalysis;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace MathCore.Hash.CRC;

public class CRC16
{
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

    public static ushort Hash(byte[] data, Mode mode = Mode.XMODEM, ushort crc = 0xFFFF, ushort xor = 0xFFFF)
    {
        if (data.NotNull().Length == 0)
            throw new InvalidOperationException();

        var poly = (ushort)mode;

        foreach (var b in data)
            crc = (ushort)(crc << 8 ^ Table(crc >> 8 ^ b, poly));

        return (ushort)(crc ^ xor);

        static ushort Table(int i, ushort poly)
        {
            ushort table = 0;
            var temp = (ushort)(i << 8);
            for (var j = 0; j < 8; j++)
            {
                if (((table ^ temp) & 0b10000000_00000000) != 0)
                    table = (ushort)(table << 1 ^ poly);
                else
                    table <<= 1;
                temp <<= 1;
            }
            return table;
        }
    }

    private const int __TableLength = 256;
    private readonly ushort[] _Table = new ushort[__TableLength];

    public ushort State { get; set; }

    public bool UpdateState { get; set; }

    public ushort XOR { get; set; } = 0;

    public CRC16(Mode mode = Mode.IBM) : this((ushort)mode) { }

    public CRC16(ushort Polynimial)
    {
        for (var i = 0; i < __TableLength; i++)
        {
            ushort table = 0;
            var temp = (ushort)(i << 8);
            for (var j = 0; j < 8; ++j)
            {
                table = ((table ^ temp) & 0b10000000_00000000) != 0
                    ? (ushort)(table << 1 ^ Polynimial)
                    : (ushort)(table << 1);
                temp <<= 1;
            }
            _Table[i] = table;
        }
    }

    //public ushort Compute(ReadOnlySpan<byte> bytes)
    //{
    //    var crc = Initial;
    //    foreach (var b in bytes)
    //        crc = (ushort)((crc << 8) ^ _Table[(crc >> 8) ^ b]);

    //    return crc;
    //}

    public ushort Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public ushort ContinueCompute(ushort crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

#if NET8_0_OR_GREATER
    public uint Compute(Span<byte> bytes) => ContinueCompute(State, bytes);
    public uint Compute(ReadOnlySpan<byte> bytes) => ContinueCompute(State, bytes);

    public uint Compute(Memory<byte> bytes) => ContinueCompute(State, bytes.Span);
    public uint Compute(ReadOnlyMemory<byte> bytes) => ContinueCompute(State, bytes.Span);

    public uint ContinueCompute(ushort crc, Span<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

    public uint ContinueCompute(ushort crc, ReadOnlySpan<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }
#endif

    public ushort Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public ushort ContinueCompute(ushort crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref ushort crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);
        crc ^= XOR;
    }

    public void Compute(ref ushort crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);
        crc ^= XOR;
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
