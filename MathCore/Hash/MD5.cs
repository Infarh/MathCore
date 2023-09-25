#nullable enable
using System.Text;

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

    private static void SetLength(byte[] buffer64, ulong length)
    {
        buffer64[^8] = (byte)(length << 3);
        buffer64[^7] = (byte)(length >> 5);
        buffer64[^6] = (byte)(length >> 13);
        buffer64[^5] = (byte)(length >> 21);
        buffer64[^4] = (byte)(length >> 29);
        buffer64[^3] = (byte)(length >> 37);
        buffer64[^2] = (byte)(length >> 45);
        buffer64[^1] = (byte)(length >> 53);
    }

    public static byte[] Compute(byte[] data)
    {
        uint[] h = { 0x67452301U, 0xefcdab89U, 0x98badcfeU, 0x10325476U };

        const int length_0x80  = 1;
        const int length_end  = 8;

        var length = data.Length;
        var zero_length = 64 - length % 64 - length_0x80 - length_end;

        if (zero_length < 0) zero_length += 64;

        var buffer64_length = length + length_0x80 + zero_length + length_end;

        var buffer64 = new byte[buffer64_length];

        Array.Copy(data, 0, buffer64, 0, length);
        buffer64[length] = 0x80;

        SetLength(buffer64, (ulong)length);

        var words = new uint[16];
        for (var i = 0; i < buffer64_length / 64; i++)
        {
            Buffer.BlockCopy(buffer64, i * 64, words, 0, 64);
            Compute(words, ref h[0], ref h[1], ref h[2], ref h[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(h, 0, result_bytes, 0, result_bytes.Length);

        return result_bytes;
    }

    //public static byte[] Compute(string str, Encoding? encoding = null) => Compute((encoding ?? Encoding.UTF8).GetBytes(str));
    public static byte[] Compute(string str, Encoding? encoding = null) => Compute(str.ToByteStream(encoding));
    //{
    //    encoding ??= Encoding.UTF8;
    //    var str_length = str.Length;
    //    uint[] result = { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };

    //    var buffer64 = new byte[64];
    //    var words = new uint[16];

    //    var buffer_char_size = encoding.GetCharCount(buffer64);
    //    var bytes_per_char = buffer64.Length / buffer_char_size;

    //    var completed = false;
    //    var length = 0UL;
    //    var str_offset = 0;
    //    int readed;
    //    do
    //    {
    //        var readed_chars = encoding.GetBytes(str, str_offset, Math.Min(buffer_char_size, str_length - str_offset), buffer64, 0);
    //        str_offset += readed_chars;
    //        readed = readed_chars * bytes_per_char;

    //        length += (ulong)readed;

    //        if (readed < 64)
    //        {
    //            Array.Clear(buffer64, readed, 64 - readed);
    //            buffer64[readed] = 0x80;

    //            if (64 - readed > 8)
    //            {
    //                SetLength(buffer64, length);

    //                completed = true;
    //            }
    //        }

    //        Buffer.BlockCopy(buffer64, 0, words, 0, 64);
    //        Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
    //    }
    //    while (readed == 64);

    //    if (readed > 0 && !completed)
    //    {
    //        Array.Clear(buffer64, 0, 64);

    //        SetLength(buffer64, length);

    //        Buffer.BlockCopy(buffer64, 0, words, 0, 64);
    //        Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
    //    }

    //    var result_bytes = new byte[16];
    //    Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
    //    return result_bytes;
    //}

    public static byte[] Compute(Stream data)
    {
        uint[] result = { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };

        var buffer64 = new byte[64];
        var words = new uint[16];

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
                    SetLength(buffer64, length);

                    completed = true;
                }
            }

            Buffer.BlockCopy(buffer64, 0, words, 0, 64);
            Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
        }
        while (readed == 64);

        if (readed > 0 && !completed)
        {
            Array.Clear(buffer64, 0, 64);

            SetLength(buffer64, length);

            Buffer.BlockCopy(buffer64, 0, words, 0, 64);
            Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
        return result_bytes;
    }

    public static async Task<byte[]> ComputeAsync(Stream data, CancellationToken Cancel = default)
    {
        uint[] result = { 0x67452301, 0xefcdab89, 0x98badcfe, 0x10325476 };

        var buffer64 = new byte[64];
        var words = new uint[16];

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
                    SetLength(buffer64, length);

                    completed = true;
                }
            }

            Buffer.BlockCopy(buffer64, 0, words, 0, 64);
            Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
        }
        while (readed == 64);

        if (readed > 0 && !completed)
        {
            Array.Clear(buffer64, 0, 64);

            SetLength(buffer64, length);

            Buffer.BlockCopy(buffer64, 0, words, 0, 64);
            Compute(words, ref result[0], ref result[1], ref result[2], ref result[3]);
        }

        var result_bytes = new byte[16];
        Buffer.BlockCopy(result, 0, result_bytes, 0, result_bytes.Length);
        return result_bytes;
    }

    private static void Compute(uint[] buffer16, ref uint a0, ref uint b0, ref uint c0, ref uint d0)
    {
        var (A, B, C, D) = (a0, b0, c0, d0);

        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xd76aa478 + buffer16[00], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xe8c7b756 + buffer16[01], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x242070db + buffer16[02], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xc1bdceee + buffer16[03], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xf57c0faf + buffer16[04], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x4787c62a + buffer16[05], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xa8304613 + buffer16[06], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xfd469501 + buffer16[07], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x698098d8 + buffer16[08], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x8b44f7af + buffer16[09], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xffff5bb1 + buffer16[10], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x895cd7be + buffer16[11], 22), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x6b901122 + buffer16[12], 07), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xfd987193 + buffer16[13], 12), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0xa679438e + buffer16[14], 17), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (B & C | ~B & D) + 0x49b40821 + buffer16[15], 22), B, C);

        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xf61e2562 + buffer16[(5 * 16 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xc040b340 + buffer16[(5 * 17 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x265e5a51 + buffer16[(5 * 18 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xe9b6c7aa + buffer16[(5 * 19 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xd62f105d + buffer16[(5 * 20 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x02441453 + buffer16[(5 * 21 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xd8a1e681 + buffer16[(5 * 22 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xe7d3fbc8 + buffer16[(5 * 23 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x21e1cde6 + buffer16[(5 * 24 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xc33707d6 + buffer16[(5 * 25 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xf4d50d87 + buffer16[(5 * 26 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x455a14ed + buffer16[(5 * 27 + 1) % 16], 20), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xa9e3e905 + buffer16[(5 * 28 + 1) % 16], 05), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0xfcefa3f8 + buffer16[(5 * 29 + 1) % 16], 09), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x676f02d9 + buffer16[(5 * 30 + 1) % 16], 14), B, C);
        (A, B, C, D) = (D, B + LeftRotate(A + (D & B | ~D & C) + 0x8d2a4c8a + buffer16[(5 * 31 + 1) % 16], 20), B, C);

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
}
