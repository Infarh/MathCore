#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace MathCore.Hash;

/// <summary>RFC for MD5</summary>
/// <remarks>
/// https://tools.ietf.org/html/rfc1321<br/>
/// https://ru.wikipedia.org/wiki/MD5
/// </remarks>
public class MD5 : HashAlgorithm
{
    private MD5() { }

    public static byte[] Compute(string str, Encoding? encoding = null) => Compute((encoding ?? Encoding.UTF8).GetBytes(str));

    public static byte[] Compute(byte[] data)
    {
        uint[] h = { 0x67452301U, 0xefcdab89U, 0x98badcfeU, 0x10325476U };

        var zero_length = 64 - data.Length % 64 - 1 - 8;

        if (zero_length < 0) zero_length += 64;

        const int length_x80 = 1;
        const int length_end = 8;

        var buffer64_length = data.Length + length_x80 + zero_length + length_end;

        var buffer64 = new byte[buffer64_length];

        Array.Copy(data, buffer64, data.Length);
        buffer64[data.Length] = 0x80;

        var length_bytes = BitConverter.GetBytes(data.Length << 3);

        Array.Copy(length_bytes, 0, buffer64, buffer64_length - 8, 4);

        var buffer16 = new uint[16];
        for (var i = 0; i < buffer64_length / 64; i++)
        {
            Buffer.BlockCopy(buffer64, i * 64, buffer16, 0, 64);
            Compute(buffer16, ref h[0], ref h[1], ref h[2], ref h[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(h, 0, result_bytes, 0, result_bytes.Length);

        return result_bytes;
    }

    public static byte[] Compute(Stream data)
    {
        uint[] result =
        {
            0x67452301,
            0xefcdab89,
            0x98badcfe,
            0x10325476,
        };

        var buffer64 = new byte[64];
        var buffer16 = new uint[16];

        var completed = false;
        var length    = 0UL;
        int readed;
        do
        {
            readed = data.FillBuffer(buffer64);

            length += (ulong)readed;

            if (readed < 64)
            {
                Array.Clear(buffer64, readed, 64 - readed);
                buffer64[readed] = 0x80;

                if (64 - readed > 8)
                {
                    var full_length = length << 3;
                    buffer64[^8] = (byte)((full_length) & 0xff);
                    buffer64[^7] = (byte)((full_length >> 8) & 0xff);
                    buffer64[^6] = (byte)((full_length >> 16) & 0xff);
                    buffer64[^5] = (byte)((full_length >> 24) & 0xff);
                    buffer64[^4] = (byte)((full_length >> 32) & 0xff);
                    buffer64[^3] = (byte)((full_length >> 40) & 0xff);
                    buffer64[^2] = (byte)((full_length >> 48) & 0xff);
                    buffer64[^1] = (byte)((full_length >> 56) & 0xff);

                    completed = true;
                }
            }

            Buffer.BlockCopy(buffer64, 0, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }
        while (readed == 64);

        if (readed > 0 && !completed)
        {
            Array.Clear(buffer64, 0, 64);

            var full_length = length << 3;
            buffer64[^8] = (byte)((full_length) & 0xff);
            buffer64[^7] = (byte)((full_length >> 8) & 0xff);
            buffer64[^6] = (byte)((full_length >> 16) & 0xff);
            buffer64[^5] = (byte)((full_length >> 24) & 0xff);
            buffer64[^4] = (byte)((full_length >> 32) & 0xff);
            buffer64[^3] = (byte)((full_length >> 40) & 0xff);
            buffer64[^2] = (byte)((full_length >> 48) & 0xff);
            buffer64[^1] = (byte)((full_length >> 56) & 0xff);

            Buffer.BlockCopy(buffer64, 0, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
        return result_bytes;
    }

    public static async Task<byte[]> ComputeAsync(Stream data, CancellationToken Cancel = default)
    {
        uint[] result =
        {
            0x67452301,
            0xefcdab89,
            0x98badcfe,
            0x10325476,
        };

        var buffer64 = new byte[64];
        var buffer16 = new uint[16];

        var completed = false;
        var length    = 0UL;
        int readed;
        do
        {
            readed = await data.FillBufferAsync(buffer64, Cancel).ConfigureAwait(false);

            length += (ulong)readed;

            if (readed < 64)
            {
                Array.Clear(buffer64, readed, 64 - readed);
                buffer64[readed] = 0x80;

                if (64 - readed > 8)
                {
                    var full_length = length << 3;
                    buffer64[^8] = (byte)((full_length) & 0xff);
                    buffer64[^7] = (byte)((full_length >> 8) & 0xff);
                    buffer64[^6] = (byte)((full_length >> 16) & 0xff);
                    buffer64[^5] = (byte)((full_length >> 24) & 0xff);
                    buffer64[^4] = (byte)((full_length >> 32) & 0xff);
                    buffer64[^3] = (byte)((full_length >> 40) & 0xff);
                    buffer64[^2] = (byte)((full_length >> 48) & 0xff);
                    buffer64[^1] = (byte)((full_length >> 56) & 0xff);

                    completed = true;
                }
            }

            Buffer.BlockCopy(buffer64, 0, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }
        while (readed == 64);

        if (readed > 0 && !completed)
        {
            Array.Clear(buffer64, 0, 64);

            var full_length = length << 3;
            buffer64[^8] = (byte)((full_length) & 0xff);
            buffer64[^7] = (byte)((full_length >> 8) & 0xff);
            buffer64[^6] = (byte)((full_length >> 16) & 0xff);
            buffer64[^5] = (byte)((full_length >> 24) & 0xff);
            buffer64[^4] = (byte)((full_length >> 32) & 0xff);
            buffer64[^3] = (byte)((full_length >> 40) & 0xff);
            buffer64[^2] = (byte)((full_length >> 48) & 0xff);
            buffer64[^1] = (byte)((full_length >> 56) & 0xff);

            Buffer.BlockCopy(buffer64, 0, buffer16, 0, 64);
            Compute(buffer16, ref result[0], ref result[1], ref result[2], ref result[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
        return result_bytes;
    }

    private static void Compute(uint[] buffer16, ref uint a0, ref uint b0, ref uint c0, ref uint d0)
    {
        var (A, B, C, D) = (a0, b0, c0, d0);

        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xd76aa478 + buffer16[00], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xe8c7b756 + buffer16[01], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x242070db + buffer16[02], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xc1bdceee + buffer16[03], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xf57c0faf + buffer16[04], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x4787c62a + buffer16[05], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xa8304613 + buffer16[06], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xfd469501 + buffer16[07], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x698098d8 + buffer16[08], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x8b44f7af + buffer16[09], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xffff5bb1 + buffer16[10], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x895cd7be + buffer16[11], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x6b901122 + buffer16[12], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xfd987193 + buffer16[13], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0xa679438e + buffer16[14], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((B & C) | (~B & D)) + 0x49b40821 + buffer16[15], 22), B, C);

        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xf61e2562 + buffer16[(5 * 16 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xc040b340 + buffer16[(5 * 17 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x265e5a51 + buffer16[(5 * 18 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xe9b6c7aa + buffer16[(5 * 19 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xd62f105d + buffer16[(5 * 20 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x02441453 + buffer16[(5 * 21 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xd8a1e681 + buffer16[(5 * 22 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xe7d3fbc8 + buffer16[(5 * 23 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x21e1cde6 + buffer16[(5 * 24 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xc33707d6 + buffer16[(5 * 25 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xf4d50d87 + buffer16[(5 * 26 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x455a14ed + buffer16[(5 * 27 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xa9e3e905 + buffer16[(5 * 28 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0xfcefa3f8 + buffer16[(5 * 29 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x676f02d9 + buffer16[(5 * 30 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + ((D & B) | (~D & C)) + 0x8d2a4c8a + buffer16[(5 * 31 + 1) % 16], 20), B, C);

        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xfffa3942 + buffer16[(3 * 32 + 5) % 16], 04), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x8771f681 + buffer16[(3 * 33 + 5) % 16], 11), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x6d9d6122 + buffer16[(3 * 34 + 5) % 16], 16), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xfde5380c + buffer16[(3 * 35 + 5) % 16], 23), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xa4beea44 + buffer16[(3 * 36 + 5) % 16], 04), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x4bdecfa9 + buffer16[(3 * 37 + 5) % 16], 11), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xf6bb4b60 + buffer16[(3 * 38 + 5) % 16], 16), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xbebfbc70 + buffer16[(3 * 39 + 5) % 16], 23), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x289b7ec6 + buffer16[(3 * 40 + 5) % 16], 04), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xeaa127fa + buffer16[(3 * 41 + 5) % 16], 11), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xd4ef3085 + buffer16[(3 * 42 + 5) % 16], 16), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x04881d05 + buffer16[(3 * 43 + 5) % 16], 23), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xd9d4d039 + buffer16[(3 * 44 + 5) % 16], 04), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xe6db99e5 + buffer16[(3 * 45 + 5) % 16], 11), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0x1fa27cf8 + buffer16[(3 * 46 + 5) % 16], 16), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B ^ C ^ D) + 0xc4ac5665 + buffer16[(3 * 47 + 5) % 16], 23), B, C);

        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xf4292244 + buffer16[7 * 48 % 16], 06), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x432aff97 + buffer16[7 * 49 % 16], 10), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xab9423a7 + buffer16[7 * 50 % 16], 15), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xfc93a039 + buffer16[7 * 51 % 16], 21), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x655b59c3 + buffer16[7 * 52 % 16], 06), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x8f0ccc92 + buffer16[7 * 53 % 16], 10), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xffeff47d + buffer16[7 * 54 % 16], 15), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x85845dd1 + buffer16[7 * 55 % 16], 21), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x6fa87e4f + buffer16[7 * 56 % 16], 06), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xfe2ce6e0 + buffer16[7 * 57 % 16], 10), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xa3014314 + buffer16[7 * 58 % 16], 15), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x4e0811a1 + buffer16[7 * 59 % 16], 21), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xf7537e82 + buffer16[7 * 60 % 16], 06), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xbd3af235 + buffer16[7 * 61 % 16], 10), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0x2ad7d2bb + buffer16[7 * 62 % 16], 15), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (C ^ (B | ~D)) + 0xeb86d391 + buffer16[7 * 63 % 16], 21), B, C);

        a0 += A;
        b0 += B;
        c0 += C;
        d0 += D;
    }

    //private static void Compute0(uint[] buffer16, ref uint a0, ref uint b0, ref uint c0, ref uint d0)
    //{
    //    var (A, B, C, D) = (a0, b0, c0, d0);

    //    for (uint j = 0; j < 64; j++)
    //    {
    //        var (f, g) = j switch
    //        {
    //            <= 15           => ((B & C) | (~B & D), j),
    //            >= 16 and <= 31 => ((D & B) | (~D & C), (5 * j + 1) % 16),
    //            >= 32 and <= 47 => (B ^ C ^ D, (3 * j + 5) % 16),
    //            >= 48           => (C ^ (B | ~D), 7 * j % 16)
    //        };

    //        (A, B, C, D) = (D, B + LeftRotate(A + f + __K[j] + buffer16[g], __S[j]), B, C);

    //        static uint LeftRotate(uint x, int c) => (x << c) | (x >> (32 - c));
    //    }

    //    a0 += A;
    //    b0 += B;
    //    c0 += C;
    //    d0 += D;
    //}

    //private static readonly int[] __S =
    //{
    //    7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
    //    5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
    //    4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
    //    6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
    //};

    //private static readonly uint[] __K =
    //{
    //    0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
    //    0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
    //    0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
    //    0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
    //    0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
    //    0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
    //    0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
    //    0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
    //    0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
    //    0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
    //    0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
    //    0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
    //    0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
    //    0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
    //    0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
    //    0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    //};
}
