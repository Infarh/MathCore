using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.Hash.CRC;

public class CRC8
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
    private readonly byte[] _Table = new byte[__TableLength];

    public byte State { get; set; }

    public bool UpdateState { get; set; }

    public CRC8(Mode mode = Mode.CRC8) : this((byte)mode) { }

    public CRC8(byte Polynimial)
    {
        for (var i = 0; i < __TableLength; ++i)
        {
            var temp = i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = temp << 1 ^ Polynimial;
                else
                    temp <<= 1;
            _Table[i] = (byte)temp;
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
        return BitConverter.GetBytes(crc);
    }
}
