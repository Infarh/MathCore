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
    private byte[] buf, m;
    private Digest digest;
    private ulong messageLength, blocksQty;
    private uint[] W = new uint[64];

    public Sha256Digest()
    {
        initHs();
    }

    private void initHs()
    {
        /* SHA-256 initial hash value
        * The first 32 bits of the fractional parts of the square roots
        * of the first eight prime numbers
        */
        digest.H0 = 0x6a09e667;
        digest.H1 = 0xbb67ae85;
        digest.H2 = 0x3c6ef372;
        digest.H3 = 0xa54ff53a;
        digest.H4 = 0x510e527f;
        digest.H5 = 0x9b05688c;
        digest.H6 = 0x1f83d9ab;
        digest.H7 = 0x5be0cd19;
    }

    /* SHA-256 Constants
    * (represent the first 32 bits of the fractional parts of the
    * cube roots of the first sixty-four prime numbers)
    */
    private static readonly uint[] K = {
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

    public Digest hash(byte[] message)
    { //operates only with byte-multiple messages 
        buf = new byte[message.LongLength];
        Array.Copy(message, buf, message.LongLength);
        messageLength = (ulong)message.LongLength;
        completeMessageLength();
        blocksQty = (ulong)m.LongLength / 64;
        for (ulong i = 0; i < blocksQty; i++)
        {
            expandBlock(i);
            processBlock();
        }
        return digest;
    }

    private void completeMessageLength()
    {
        int zeroBitsToAddQty = 512 - (int)(((ulong)buf.LongLength * 8 + 1 + 64) % 512);
        m = new byte[((ulong)buf.LongLength * 8 + 1 + 64 + (ulong)zeroBitsToAddQty) / 8];
        Array.Copy(buf, m, buf.LongLength);

        m[buf.LongLength] = 128; //set 1-st bit to "1", 7 remaining to "0" (may not work with bit-multiple message!!)

        for (ulong i = (messageLength + 1); i < (ulong)m.LongLength; i++)
        {
            m[i] = 0;
        }

        byte[] messageBitLength_little_endian = BitConverter.GetBytes(messageLength * 8);
        byte[] messageBitLength_big_endian = new byte[messageBitLength_little_endian.Length];
        for (int i = 0, j = messageBitLength_little_endian.Length - 1; i < messageBitLength_little_endian.Length; i++, j--)
        {
            messageBitLength_big_endian[i] = messageBitLength_little_endian[j];
        }
        Array.Copy(messageBitLength_big_endian, 0, m, m.LongLength - 8, 8);
    }

    private void expandBlock(ulong blockNumber)
    {
        for (int i = 0; i < 16; i++)
        {
            W[i] = Bytes_To_UInt32(m, blockNumber * 64 + (ulong)i * 4);
        }

        for (int i = 16; i <= 63; i++)
        {
            W[i] = W[i - 16] + S0(W[i - 15]) + W[i - 7] + S1(W[i - 2]);
        }
    }

    internal static uint Bytes_To_UInt32(byte[] bs, ulong off)
    {
        uint n = (uint)bs[off] << 24;
        n |= (uint)bs[++off] << 16;
        n |= (uint)bs[++off] << 8;
        n |= (uint)bs[++off];
        return n;
    }

    private static uint rotr(uint x, int n)
    {
        return ((x >> n) | (x << 32 - n));
    }

    private static uint S0(uint x)
    {
        return rotr(x, 7) ^ rotr(x, 18) ^ (x >> 3);
    }

    private static uint S1(uint x)
    {
        return rotr(x, 17) ^ rotr(x, 19) ^ (x >> 10);
    }

    private static uint Ch(uint x, uint y, uint z)
    {
        return (x & y) ^ ((~x) & z);
    }

    private static uint Maj(uint x, uint y, uint z)
    {
        return (x & y) ^ (x & z) ^ (y & z);
    }

    private static uint E0(uint x)
    {
        return rotr(x, 2) ^ rotr(x, 13) ^ rotr(x, 22);
    }

    private static uint E1(uint x)
    {
        return rotr(x, 6) ^ rotr(x, 11) ^ rotr(x, 25);
    }

    private void processBlock()
    {
        uint a = digest.H0;
        uint b = digest.H1;
        uint c = digest.H2;
        uint d = digest.H3;
        uint e = digest.H4;
        uint f = digest.H5;
        uint g = digest.H6;
        uint h = digest.H7;

        uint T1 = 0, T2 = 0;

        for (int i = 0; i < 64; i++)
        {
            T1 = h + E1(e) + Ch(e, f, g) + K[i] + W[i];
            T2 = E0(a) + Maj(a, b, c);
            h = g;
            g = f;
            f = e;
            e = d + T1;
            d = c;
            c = b;
            b = a;
            a = T1 + T2;
        }
        digest.H0 += a;
        digest.H1 += b;
        digest.H2 += c;
        digest.H3 += d;
        digest.H4 += e;
        digest.H5 += f;
        digest.H6 += g;
        digest.H7 += h;
    }
}
