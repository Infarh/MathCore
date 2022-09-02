using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace MathCore.CRC;

public sealed class CRC16
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum Mode : ushort
    {
        IBM = 0xA001,
        CCITT = 0x1021,
        CCITTKermit = 0x8408,
        CCITT16 = 0x8810
    }

    private const int __TableLength = 256;
    private readonly ushort[] _Table = new ushort[__TableLength];

    public CRC16(Mode mode = Mode.CCITT)
    {
        var polynomial = (ushort)mode;
        for (ushort i = 0; i < __TableLength; ++i)
        {
            ushort value = 0;
            var temp = i;
            for (byte j = 0; j < 8; ++j)
            {
                if (((value ^ temp) & 0x0001) != 0)
                    value = (ushort)((value >> 1) ^ polynomial);
                else
                    value >>= 1;

                temp >>= 1;
            }
            _Table[i] = value;
        }
    }

    //public ushort Compute(ReadOnlySpan<byte> bytes)
    //{
    //    ushort crc = 0;
    //    for (var i = 0; i < bytes.Length; ++i)
    //    {
    //        var index = (byte)(crc ^ bytes[i]);
    //        crc = (ushort)((crc >> 8) ^ _Table[index]);
    //    }
    //    return crc;
    //}

    public ushort Compute(params byte[] bytes)
    {
        ushort crc = 0;
        foreach (var b in bytes)
            crc = (ushort)((crc >> 8) ^ _Table[(byte)(crc ^ b)]);
        return crc;
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
