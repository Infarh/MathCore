using System.Diagnostics.CodeAnalysis;

namespace MathCore.Hash.CRC;

// https://microsin.net/programming/arm/crc32-demystified.html

public class CRC32(uint poly)
{
    public CRC32(Mode mode = Mode.Zip) : this((uint)mode) { }

    public static uint[] GetTableNormalBits(uint poly)
    {
        var table = new uint[__TableLength];

        FillTableNormalBits(table, poly);

        return table;
    }

    public static uint[] GetTableReverseBits(uint poly)
    {
        var table = new uint[__TableLength];

        FillTableReversBits(table, poly);

        return table;
    }

    public static void FillTableNormalBits(uint[] table, uint poly)
    {
        for (uint i = 0; i < __TableLength; i++)
        {
            ref var crc = ref table[i];
            crc = i;
            //const uint mask = 0b00000000_00000000_00000000_00000001;
            const uint mask = 0x0000_0001;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & mask) == 0
                    ? crc >> 1
                    : crc >> 1 ^ poly;
        }
    }

    private static uint[] GenerateTable(uint poly)
    {
        var table = new uint[256];
        for (var i = 0; i < table.Length; i++)
        {
            var val = (uint)i;
            for (var j = 0; j < 8; j++)
                val = (val & 0b0000_0001) == 0
                    ? val >> 1
                    : val >> 1 ^ poly;

            table[i] = val;
        }

        return table;
    }

    public static void FillTableReversBits(uint[] table, uint poly)
    {
        for (uint i = 0; i < __TableLength; i++)
        {
            ref var crc = ref table[i];
            crc = i << 24;
            //const uint mask = 0b10000000_00000000_00000000_00000000;
            const uint mask = 0x8000_0000;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & mask) == 0
                    ? crc << 1
                    : crc << 1 ^ poly;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Mode : uint
    {
        /// <summary>Инвертированный полином относительно <see cref="P0x04C11DB7"/></summary>
        P0xEDB88320 = 0xEDB88320, // 0b11101101_10111000_10000011_00100000,
        /// <summary>Нормальный полином относительно <see cref="P0xEDB88320"/></summary>
        P0x04C11DB7 = 0x04C11DB7, // 0b00000100_11000001_00011101_10110111 = x32+x26+x23+x22+x16+x12+x11+x10+x8+x7+x5+x4+x2+x1+x0
        P0x1EDC6F41 = 0x1EDC6F41,
        P0xA833982B = 0xA833982B,
        P0x814141AB = 0x814141AB,
        P0x000000AF = 0x000000AF,

        ZipInv = P0x04C11DB7,
        Zip = P0xEDB88320,
        POSIX = P0x04C11DB7,
        CRC32C = P0x1EDC6F41,
        CRC32D = P0xA833982B,
        CRC32Q = P0x814141AB,
        XFER = P0x000000AF,
    }

    public static uint Hash(
        byte[] data,
        Mode mode = Mode.Zip,
        uint crc = 0xFFFFFFFF,
        uint xor = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false)
        => Hash(data, (uint)mode, crc, xor, RefIn, RefOut);

    public static uint Hash(
        byte[] data,
        uint poly, 
        uint crc = 0xFFFFFFFF, 
        uint xor = 0xFFFFFFFF,
        bool RefIn = false,
        bool RefOut = false)
    {
        if (data.NotNull().Length == 0)
            throw new InvalidOperationException();

        if(RefIn)
            foreach (var b in data)
                crc = crc << 8 ^ Table(b.ReverseBits() ^ crc >> 24, poly);
        else
            foreach (var b in data)
                crc = crc << 8 ^ Table(b ^ crc >> 24, poly);

        return RefOut ? (crc ^ xor).ReverseBits() : crc ^ xor;

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

    private readonly uint[] _Table = GetTableReverseBits(poly);

    public uint State { get; set; }

    public bool UpdateState { get; set; }

    public uint XOR { get; set; } = 0;

    public bool RefIn { get; set; }

    public bool RefOut { get; set; }

    public uint Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public uint ContinueCompute(uint crc, byte[] bytes)
    {
        //for (var i = 0; i < bytes.Length; i++)
        //    crc = (crc >> 8) ^ _Table[(crc ^ bytes[i]) & 0xFF];

        //foreach (var b in bytes)
        //    crc = (crc >> 8) ^ _Table[(crc ^ b) & 0xFF];

        if(RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[crc >> 24 ^ b.ReverseBits()];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[crc >> 24 ^ b];

        //foreach (var b in bytes)
        //    crc = (crc >> 8) ^ _Table[(crc ^ b) & 0xFF];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }

#if NET8_0_OR_GREATER

    public uint Compute(Span<byte> bytes) => ContinueCompute(State, bytes);

    public uint Compute(ReadOnlySpan<byte> bytes) => ContinueCompute(State, bytes);

    public uint Compute(Memory<byte> bytes) => ContinueCompute(State, bytes.Span);

    public uint Compute(ReadOnlyMemory<byte> bytes) => ContinueCompute(State, bytes.Span);

    public uint ContinueCompute(uint crc, Span<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b.ReverseBits() ^ crc >> 24];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b ^ crc >> 24];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
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

    public uint Compute(IEnumerable<byte> bytes) => ContinueCompute(State, bytes);

    public uint ContinueCompute(uint crc, IEnumerable<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b.ReverseBits() ^ crc >> 24];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b ^ crc >> 24];

        crc ^= XOR;

        if (UpdateState)
            State = crc;

        return RefOut ? crc.ReverseBits() : crc;
    }

    public void Compute(ref uint crc, byte[] bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b.ReverseBits() ^ crc >> 24];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b ^ crc >> 24];

        crc ^= XOR;

        if (RefOut)
            crc = crc.ReverseBits();
    }

    public void Compute(ref uint crc, IEnumerable<byte> bytes)
    {
        if (RefIn)
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b.ReverseBits() ^ crc >> 24];
        else
            foreach (var b in bytes)
                crc = crc << 8 ^ _Table[b ^ crc >> 24];

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
