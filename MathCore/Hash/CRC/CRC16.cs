﻿using System.Diagnostics.CodeAnalysis;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace MathCore.Hash.CRC;

public class CRC16
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum Mode : ushort
    {
        IBM = 0xA001,
        CCITT = 0x1021,
        XMODEM = 0x1021,
        AUG_CCITT = 0x1021,
        CMS = 0x8005,
        CCITTKermit = 0x8408,
        CCITT16 = 0x8810
    }

    private const int __TableLength = 256;
    private readonly ushort[] _Table = new ushort[__TableLength];

    public ushort State { get; set; }

    public bool UpdateState { get; set; }

    public CRC16(Mode mode = Mode.IBM) : this((ushort)mode) { }

    public CRC16(ushort Polynimial)
    {
        for (var i = 0; i < __TableLength; ++i)
        {
            ushort value = 0;
            var temp = (ushort)(i << 8);
            for (var j = 0; j < 8; ++j)
            {
                if (((value ^ temp) & 0b10000000_00000000) != 0)
                    value = (ushort)(value << 1 ^ Polynimial);
                else
                    value <<= 1;
                temp <<= 1;
            }
            _Table[i] = value;
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

        if (UpdateState)
            State = crc;

        return crc;
    }

    public uint ContinueCompute(ushort crc, ReadOnlySpan<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);

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

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref ushort crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);
    }

    public void Compute(ref ushort crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)(crc << 8 ^ _Table[crc >> 8 ^ b]);
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
