using System;

namespace MathCore.Hash;

public struct Digest
{
    public uint H0, H1, H2, H3, H4, H5, H6, H7;

    private static void SetBytes(byte[] data, uint H, int index)
    {
        static ref byte Index(byte[] d, int i, int j) => ref d[4 * i + j];

        Index(data, index, 3) = (byte)H;
        Index(data, index, 2) = (byte)(H >> 8);
        Index(data, index, 1) = (byte)(H >> 16);
        Index(data, index, 0) = (byte)(H >> 24);
    } 

    public byte[] ToByteArray()
    {
        var result = new byte[32];
        SetBytes(result, H0, 0);
        SetBytes(result, H1, 1);
        SetBytes(result, H2, 2);
        SetBytes(result, H3, 3);
        SetBytes(result, H4, 4);
        SetBytes(result, H5, 5);
        SetBytes(result, H6, 6);
        SetBytes(result, H7, 7);

        return result;
    }
}

public class Sha256Digest
{
    //private byte[] _Buffer;
    //private byte[] _Message;
    private Digest _Digest;
    //private ulong _MessageLength;
    private ulong _BlocksQty;
    //private readonly uint[] _W = new uint[64];

    public Sha256Digest()
    {
        _Digest.H0 = 0x6a09e667;
        _Digest.H1 = 0xbb67ae85;
        _Digest.H2 = 0x3c6ef372;
        _Digest.H3 = 0xa54ff53a;
        _Digest.H4 = 0x510e527f;
        _Digest.H5 = 0x9b05688c;
        _Digest.H6 = 0x1f83d9ab;
        _Digest.H7 = 0x5be0cd19;
    }

    private static readonly uint[] K = 
    {
        0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
        0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
        0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
        0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
        0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
        0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
        0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
        0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
        0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
        0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
        0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
        0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
        0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
        0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
        0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
        0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
    };

    public byte[] Compute(byte[] message)
    { 
        var buffer = new byte[message.LongLength];
        Array.Copy(message, buffer, message.LongLength);


        CompleteMessageLength(buffer, out var full_message);
        _BlocksQty = (ulong)full_message.LongLength / 64;

        var w = new uint[64];

        for (ulong i = 0; i < _BlocksQty; i++)
        {
            ExpandBlock(w, full_message, i);
            ProcessBlock(w);
        }
        return _Digest.ToByteArray();
    }

    private static void CompleteMessageLength(byte[] buffer, out byte[] Message)
    {
        var length = buffer.LongLength;

        var zeros_bits_count = 512 - (int)((length * 8 + 1 + 64) % 512);

        Message = new byte[length + (1 + zeros_bits_count) / 8 + 8];
        Array.Copy(buffer, Message, buffer.LongLength);

        Message[buffer.LongLength] = 0x80;

        //for (var i = length + 1; i < Message.LongLength; i++) 
        //    Message[i] = 0;

        var length_bytes = BitConverter.GetBytes(length * 8);

        Array.Reverse(length_bytes);

        //    message_bit_length_big_endian[i] = length_bytes[j];

        Array.Copy(length_bytes, 0, Message, Message.LongLength - 8, 8);
    }

    private static void ExpandBlock(uint[] w, byte[] message, ulong BlockNumber)
    {
        static uint Bytes2UInt32(byte[] Data, ulong Offset) =>
            ((uint)Data[Offset] << 24) | ((uint)Data[Offset + 1] << 16) | ((uint)Data[Offset + 2] << 8) | (Data[Offset + 3]);

        for (var i = 0; i < 16; i++) 
            w[i] = Bytes2UInt32(message, BlockNumber * 64 + (ulong)i * 4);

        for (var i = 16; i <= 63; i++) 
            w[i] = w[i - 16] + S0(w[i - 15]) + w[i - 7] + S1(w[i - 2]);

        static uint S0(uint x) => RightRotate(x, 7) ^ RightRotate(x, 18) ^ x >> 3;
        static uint S1(uint x) => RightRotate(x, 17) ^ RightRotate(x, 19) ^ x >> 10;
    }

    private void ProcessBlock(uint[] _W)
    {
        var a = _Digest.H0;
        var b = _Digest.H1;
        var c = _Digest.H2;
        var d = _Digest.H3;
        var e = _Digest.H4;
        var f = _Digest.H5;
        var g = _Digest.H6;
        var h = _Digest.H7;

        for (var i = 0; i < 64; i++)
        {
            static uint Ch(uint x, uint y, uint z) => x & y ^ ~x & z;
            static uint Maj(uint x, uint y, uint z) => x & y ^ x & z ^ y & z;

            static uint E0(uint x) => RightRotate(x, 2) ^ RightRotate(x, 13) ^ RightRotate(x, 22);
            static uint E1(uint x) => RightRotate(x, 6) ^ RightRotate(x, 11) ^ RightRotate(x, 25);

            var t1 = h + E1(e) + Ch(e, f, g) + K[i] + _W[i];
            var t2 = E0(a) + Maj(a, b, c);

            (h, g, f, e, d, c, b, a) = (g, f, e, d + t1, c, b, a, t1 + t2);
        }

        _Digest.H0 += a;
        _Digest.H1 += b;
        _Digest.H2 += c;
        _Digest.H3 += d;
        _Digest.H4 += e;
        _Digest.H5 += f;
        _Digest.H6 += g;
        _Digest.H7 += h;
    }

    private static uint RightRotate(uint x, int n) => x >> n | x << 32 - n;
}
