namespace MathCore.Hash.CRC;

public class CRC32
{
    public enum Mode : uint
    {
        P0xEDB88320 = 0xEDB88320,
        P0x04C11DB7 = 0x04C11DB7,
        P0x1EDC6F41 = 0x1EDC6F41,
        P0xA833982B = 0xA833982B,
        P0x814141AB = 0x814141AB,
        P0x000000AF = 0x000000AF,

        Zip = P0xEDB88320,
        POSIX = P0x04C11DB7,
        CRC32C = P0x1EDC6F41,
        CRC32D = P0xA833982B,
        CRC32Q = P0x814141AB,
        XFER = P0x000000AF,
    }

    public static uint Hash(byte[] data, Mode mode = Mode.Zip, uint crc = 0xFFFFFFFF, uint xor = 0xFFFFFFFF)
    {
        if (data.NotNull().Length == 0)
            throw new InvalidOperationException();

        var poly = (uint)mode;

        foreach (var b in data)
            crc = crc << 8 ^ Table(b ^ crc >> 24, poly);

        return crc ^ xor;

        static uint Table(uint i, uint poly)
        {
            var table = i << 24;
            const uint mask = 0b10000000_00000000_00000000_00000000U;
            for (var bit = 0; bit < 8; bit++)
                table = (table & mask) != 0
                    ? table << 1 ^ poly
                    : table << 1;

            return table;
        }
    }

    private const int __TableLength = 256;
    private readonly uint[] _Table = new uint[__TableLength];

    public uint State { get; set; }

    public bool UpdateState { get; set; }

    public uint XOR { get; set; } = 0;

    public CRC32(Mode mode = Mode.Zip) : this((uint)mode) { }

    public CRC32(uint Polynomial)
    {
        for (uint i = 0; i < __TableLength; i++)
        {
            var crc = i << 24;
            const uint mask = 0b10000000_00000000_00000000_00000000U;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & mask) != 0
                    ? crc << 1 ^ Polynomial
                    : crc << 1;
            _Table[i] = crc;
        }
    }

    public uint Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public uint ContinueCompute(uint crc, byte[] bytes)
    {
        //for (var i = 0; i < bytes.Length; i++)
        //    crc = (crc >> 8) ^ _Table[(crc ^ bytes[i]) & 0xFF];

        //foreach (var b in bytes)
        //    crc = (crc >> 8) ^ _Table[(crc ^ b) & 0xFF];

        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[b ^ crc >> 24];

        //foreach (var b in bytes)
        //    crc = (crc >> 8) ^ _Table[(crc ^ b) & 0xFF];

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

    public uint ContinueCompute(uint crc, Span<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[b ^ crc >> 24];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    } 

    public uint ContinueCompute(uint crc, ReadOnlySpan<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[b ^ crc >> 24];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    } 
#endif

    public uint Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public uint ContinueCompute(uint crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref uint crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];
        crc ^= XOR;
    }

    public void Compute(ref uint crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];
        crc ^= XOR;
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
