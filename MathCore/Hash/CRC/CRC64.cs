namespace MathCore.Hash.CRC;

public class CRC64(ulong Polynomial)
{
    public CRC64(Mode mode) : this((ulong)mode) { }

    public enum Mode : ulong
    {
        //ISO3309 = 0xD800000000000000,
        ISO3309 = 0x000000000000001B,
        ECMA = 0x42F0E1EBA9EA3693,
    }

    public static ulong[] CreateTable(ulong Polynomial)
    {
        var table = new ulong[__TableLength];

        FillTable(table, Polynomial);

        return table;
    }

    public static void FillTable(ulong[] table, ulong Polynomial)
    {
        for (ulong i = 0; i < __TableLength; i++)
        {
            ref var crc = ref table[i];

            crc = i << 56;
            const ulong mask = 0x80_00_00_00_00_00_00_00UL;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & mask) == mask
                    ? crc << 1 ^ Polynomial
                    : crc << 1;
        }
    }

    public static ulong Hash(
        byte[] data,
        Mode mode = Mode.ISO3309,
        ulong crc = 0xFFFFFFFFFFFFFFFF, 
        ulong xor = 0xFFFFFFFFFFFFFFFF, 
        bool RefIn = false, 
        bool RefOut = false)
    {
        if (data.NotNull().Length == 0)
            throw new InvalidOperationException();

        var poly = (ulong)mode;

        if (RefIn)
            foreach (var b in data)
                crc = (crc << 8) ^ Table((crc >> 56) ^ b.ReverseBits(), poly);
        else
            foreach (var b in data)
                crc = (crc << 8) ^ Table((crc >> 56) ^ b, poly);

        crc ^= xor;

        return RefOut ? crc.ReverseBits() : crc;

        static ulong Table(ulong i, ulong poly)
        {
            var crc = i << 56;
            const ulong mask = 0x80_00_00_00_00_00_00_00UL;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & mask) == mask
                    ? crc << 1 ^ poly
                    : crc << 1;

            return crc;
        }
    }

    private const int __TableLength = 256;

    private readonly ulong[] _Table = CreateTable(Polynomial);

    public ulong State { get; set; }

    public bool UpdateState { get; set; }

    public ulong XOR { get; set; } = 0;

    public bool RefIn { get; set; }

    public bool RefOut { get; set; }

    public ulong Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public ulong ContinueCompute(ulong crc, byte[] bytes)
    {
        if(RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }

#if NET8_0_OR_GREATER

    public ulong Compute(Span<byte> bytes) => ContinueCompute(State, bytes);

    public ulong Compute(ReadOnlySpan<byte> bytes) => ContinueCompute(State, bytes);

    public ulong Compute(Memory<byte> bytes) => ContinueCompute(State, bytes.Span);

    public ulong Compute(ReadOnlyMemory<byte> bytes) => ContinueCompute(State, bytes.Span);

    public ulong ContinueCompute(ulong crc, Span<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }

    public ulong ContinueCompute(ulong crc, ReadOnlySpan<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }
#endif

    public ulong Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public ulong ContinueCompute(ulong crc, IReadOnlyList<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }

    public void Compute(ref ulong crc, byte[] bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (RefOut)
            crc = crc.ReverseBits();
    }

    public void Compute(ref ulong crc, IReadOnlyList<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b.ReverseBits())];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[(int)(crc >> 56 ^ b)];

        crc ^= XOR;

        if (RefOut)
            crc = crc.ReverseBits();
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
