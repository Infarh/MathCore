using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace MathCore.CRC;

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
                if (((value ^ temp) & 0x8000) != 0)
                    value = (ushort)((value << 1) ^ Polynimial);
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
            crc = (ushort)((crc << 8) ^ _Table[(crc >> 8) ^ b]);

        if (UpdateState)
            State = crc;

        return crc;
    }

    public ushort Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public ushort ContinueCompute(ushort crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)((crc << 8) ^ _Table[(crc >> 8) ^ b]);

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref ushort crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)((crc << 8) ^ _Table[(crc >> 8) ^ b]);
    }

    public void Compute(ref ushort crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (ushort)((crc << 8) ^ _Table[(crc >> 8) ^ b]);
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
