using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.CRC;

public class CRC8
{
    public enum Mode : byte
    {
        Default = 0xd5,
    }

    private const int __TableLength = 256;
    private readonly byte[] _Table = new byte[__TableLength];

    public byte State { get; set; }

    public bool UpdateState { get; set; }

    public CRC8(Mode mode = Mode.Default) : this((byte)mode) { }

    public CRC8(byte Polynimial)
    {
        for (var i = 0; i < __TableLength; ++i)
        {
            var temp = i;
            for (var j = 0; j < 8; ++j)
                if ((temp & 0x80) != 0)
                    temp = (temp << 1) ^ Polynimial;
                else
                    temp <<= 1;
            _Table[i] = (byte)temp;
        }
    }

    public byte Compute(params byte[] bytes) => ContinueCompute(State, bytes);

    public byte ContinueCompute(byte crc, byte[] bytes)
    {
        //for (var i = 0; i < bytes.Length; i++)
        //    crc = (crc >> 8) ^ _Table[(crc ^ bytes[i]) & 0xFF];

        //foreach (var b in bytes)
        //    crc = (crc >> 8) ^ _Table[(crc ^ b) & 0xFF];

        //foreach (var b in bytes)
        //    crc = (crc << 8) ^ _Table[(crc >> 8) ^ b]; 

        foreach (var b in bytes)
            crc = (byte)((crc >> 8) ^ _Table[(crc ^ b) & 0xFF]);

        if (UpdateState)
            State = crc;

        return crc;
    }

    public byte Compute(IReadOnlyList<byte> bytes) => ContinueCompute(State, bytes);

    public byte ContinueCompute(byte crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)((crc << 8) ^ _Table[(crc >> 8) ^ b]);

        if (UpdateState)
            State = crc;

        return crc;
    }

    public void Compute(ref byte crc, byte[] bytes)
    {
        foreach (var b in bytes)
            crc = (byte)((crc << 8) ^ _Table[(crc >> 8) ^ b]);
    }

    public void Compute(ref byte crc, IReadOnlyList<byte> bytes)
    {
        foreach (var b in bytes)
            crc = (byte)((crc << 8) ^ _Table[(crc >> 8) ^ b]);
    }

    public byte[] ComputeChecksumBytes(params byte[] bytes)
    {
        var crc = Compute(bytes);
        return BitConverter.GetBytes(crc);
    }
}
