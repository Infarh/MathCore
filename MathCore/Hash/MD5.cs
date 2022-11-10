﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MathCore.Hash;

public class MD5
{
    private const uint __A0 = 0x67_45_23_01;
    private const uint __B0 = 0xEF_CD_AB_89;
    private const uint __C0 = 0x98_BA_DC_FE;
    private const uint __D0 = 0x10_32_54_76;

    // https://ru.wikipedia.org/wiki/MD5#Псевдокод
    // s[ 0..15] := { 7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22 }
    // s[16..31] := { 5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20 }
    // s[32..47] := { 4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23 }
    // s[48..63] := { 6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21 }

    private static readonly int[] __S =
    {
        7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
        5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
        4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
        6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,
    };

    // for i from 0 to 63 do K[i] := floor(2^32 × abs(sin(i + 1)))
    // for i from 0 to 63 do K[i] := floor(4294967296L × abs(sin(i + 1)))
    // end for
    // // (Or just use the following precomputed table):
    // K[ 0.. 3] := { 0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee }
    // K[ 4.. 7] := { 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501 }
    // K[ 8..11] := { 0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be }
    // K[12..15] := { 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821 }
    // K[16..19] := { 0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa }
    // K[20..23] := { 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8 }
    // K[24..27] := { 0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed }
    // K[28..31] := { 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a }
    // K[32..35] := { 0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c }
    // K[36..39] := { 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70 }
    // K[40..43] := { 0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05 }
    // K[44..47] := { 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665 }
    // K[48..51] := { 0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039 }
    // K[52..55] := { 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1 }
    // K[56..59] := { 0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1 }
    // K[60..63] := { 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391 }

    private static readonly uint[] __K = Enumerable
       .Range(1, 64)
       .ToArray(i => (uint)(4294967296L * Math.Abs(Math.Sin(i))));
    //.Select(i => Math.Sin(i))
    //.Select(Math.Abs)
    //.Select(x => x * 4294967296L)
    //.Select(i => (uint)i)
    //.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FuncF(uint X, uint Y, uint Z) => (X & Y) | (~X & Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FuncG(uint X, uint Y, uint Z) => (X & Z) | (~Z & Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FuncH(uint X, uint Y, uint Z) => X ^ Y ^ Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint FuncI(uint X, uint Y, uint Z) => Y ^ (~Z | X);

    public static byte[] Hash(Stream data)
    {
        const int buffer_length = 64; // 512 бит

        uint[] result = { __A0, __B0, __C0, __D0 };

        var buffer = new byte[buffer_length];

        ulong data_length = 0;
        while (data.FillBuffer(buffer) is > 0 and var readed)
        {
            if (readed < buffer_length)
            {
                Array.Clear(buffer, readed, buffer_length - readed);
                buffer[readed] = 0x80;
            }

            data_length = unchecked(data_length + (ulong)readed);

            var (a, b, c, d) = (__A0, __B0, __C0, __D0);

            for (var i = 0; i < 63; i++)
            {
                var (f, g) = i switch
                {
                    <= 15 => ((b & c) | (~b & d),      i          ),
                    <= 31 => ((d & b) | (~d & c), (5 * i + 1) % 16),
                    <= 47 => (b ^ c ^ d         , (3 * i + 5) % 16),
                    _     => (c ^ (b & ~d)      ,  7 * i      % 16)
                };

                (f, a, d, c) = (f + a + __K[i] + buffer[g], d, c, b);

                b += f << __S[i];
            }

            result[0] += a;
            result[1] += b;
            result[2] += c;
            result[3] += d;
        }

        var bytes_result = new byte[16];
        Buffer.BlockCopy(result, 0, result, 0, bytes_result.Length);

        return bytes_result;
    }
}