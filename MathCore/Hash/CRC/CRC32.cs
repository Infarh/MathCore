using System;
using System.Collections.Generic;

namespace MathCore.Hash.CRC;

public class CRC32
{
    public enum Mode : uint
    {
        Zip = 0xEDB88320,
        POSIX = 0x04C11DB7,
        CRC32C = 0x1EDC6F41,
        CRC32D = 0xA833982B,
        CRC32Q = 0x814141AB,
        XFER = 0x000000AF,
    }

    private const int __TableLength = 256;
    private readonly uint[] _Table = new uint[__TableLength];

    public uint State { get; set; }

    public bool UpdateState { get; set; }

    public CRC32(Mode mode = Mode.Zip) : this((uint)mode) { }

    public CRC32(uint Polynimial)
    {
        for (uint i = 0; i < __TableLength; i++)
        {
            var crc = i << 24;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & 1u << 31) != 0
                    ? crc << 1 ^ Polynimial
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

        if (UpdateState)
            State = crc;

        return crc;
    }

    public uint Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public uint ContinueCompute(uint crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref uint crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];
    }

    public void Compute(ref uint crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = crc << 8 ^ _Table[crc >> 8 ^ b];
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
