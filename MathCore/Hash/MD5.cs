using System;
using System.Linq;

namespace MathCore.Hash;

/// <summary>
/// RFC for MD5 https://tools.ietf.org/html/rfc1321
/// Based on the pseudo code from Wikipedia: https://en.wikipedia.org/wiki/MD5
/// </summary>
public class MD5
{
    private static int[] _S = 
    {
        7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
    };

    private static uint[] __K =
    {
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    };

    public static string Compute(byte[] input)
    {
        var a0 = 0x67452301U;   // A
        var b0 = 0xefcdab89U;   // B
        var c0 = 0x98badcfeU;   // C
        var d0 = 0x10325476U;   // D

        var add_length = (56 - ((input.Length + 1) % 64)) % 64;
        var processed_input = new byte[input.Length + 1 + add_length + 8];

        Array.Copy(input, processed_input, input.Length);
        processed_input[input.Length] = 0x80; // add 1

        var length = BitConverter.GetBytes(input.Length * 8);

        Array.Copy(length, 0, processed_input, processed_input.Length - 8, 4);

        for (var i = 0; i < processed_input.Length / 64; ++i)
        {
            var M = new uint[16];
            for (var j = 0; j < 16; ++j)
                M[j] = BitConverter.ToUInt32(processed_input, (i * 64) + (j * 4));

            //uint A = a0, B = b0, C = c0, D = d0, F = 0U, g = 0U;
            var (A, B, C, D, F, g) = (a0, b0, c0, d0, 0u, 0u);

            // primary loop
            for (uint k = 0; k < 64; ++k)
            {
                switch (k)
                {
                    case <= 15:
                        F = (B & C) | (~B & D);
                        g = k;
                        break;
                    case >= 16 and <= 31:
                        F = (D & B) | (~D & C);
                        g = (5 * k + 1) % 16;
                        break;
                    case >= 32 and <= 47:
                        F = B ^ C ^ D;
                        g = (3 * k + 5) % 16;
                        break;
                    case >= 48:
                        F = C ^ (B | ~D);
                        g = (7 * k) % 16;
                        break;
                }

                static uint LeftRotate(uint x, int c) => (x << c) | (x >> (32 - c));

                var d_temp = D;
                D = C;
                C = B;
                B += LeftRotate(A + F + __K[k] + M[g], _S[k]);
                A =  d_temp;
            }

            a0 += A;
            b0 += B;
            c0 += C;
            d0 += D;
        }

        return GetByteString(a0) + GetByteString(b0) + GetByteString(c0) + GetByteString(d0);
    }

    private static string GetByteString(uint x) => string.Join("", BitConverter.GetBytes(x).Select(y => y.ToString("x2")));
}
