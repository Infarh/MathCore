using System;

namespace MathCore.Hash;

public class SHA256 : HashAlgorithm
{
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

    public static byte[] Compute(byte[] data)
    {
        uint[] h =
        {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
            0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        var buffer = new byte[data.LongLength];
        Array.Copy(data, buffer, data.LongLength);

        var length = buffer.LongLength;

        var zeros_bits_count = 512 - (int)((length * 8 + 1 + 64) % 512);

        var message = new byte[length + (1 + zeros_bits_count) / 8 + 8];
        Array.Copy(buffer, message, buffer.LongLength);

        message[buffer.LongLength] = 0x80;

        //for (var i = length + 1; i < Message.LongLength; i++) 
        //    Message[i] = 0;

        var length_bytes = BitConverter.GetBytes(length * 8);

        Array.Reverse(length_bytes);
        //message_bit_length_big_endian[i] = length_bytes[j];

        Array.Copy(length_bytes, 0, message, message.LongLength - 8, 8);

        var w = new uint[64];
        for (var i = 0; i < message.LongLength; i += 64)
        {
            for (var j = 0; j < 16 * 4; j += 4) 
                w[j / 4] = 
                    ((uint)message[i + j] << 24) | 
                    ((uint)message[i + j + 1] << 16) | 
                    ((uint)message[i + j + 2] << 8) | 
                    (message[i + j + 3]);

            ProcessBlock(w,
                ref h[0], ref h[1], ref h[2], ref h[3], 
                ref h[4], ref h[5], ref h[6], ref h[7]);
        }

        var result = new byte[32];

        for (var i = 0; i < 8; i++)
        {
            result[4 * i + 0] = (byte)(h[i] >> 24);
            result[4 * i + 1] = (byte)(h[i] >> 16);
            result[4 * i + 2] = (byte)(h[i] >> 08);
            result[4 * i + 3] = (byte)(h[i] >> 00);
        }

        return result;
    }

    private static void ProcessBlock(uint[] w,
        ref uint h0, ref uint h1, ref uint h2, ref uint h3,
        ref uint h4, ref uint h5, ref uint h6, ref uint h7)
    {
        static uint S0(uint x) => RightRotate(x, 7) ^ RightRotate(x, 18) ^ x >> 3;
        static uint S1(uint x) => RightRotate(x, 17) ^ RightRotate(x, 19) ^ x >> 10;

        for (var j = 16; j <= 63; j++)
            w[j] = w[j - 16] + S0(w[j - 15]) + w[j - 7] + S1(w[j - 2]);

        var (a, b, c, d, e, f, g, h) = (h0, h1, h2, h3, h4, h5, h6, h7);

        for (var i = 0; i < 64; i++)
        {
            static uint Ch(uint x, uint y, uint z) => x & y ^ ~x & z;
            static uint Maj(uint x, uint y, uint z) => x & y ^ x & z ^ y & z;

            static uint E0(uint x) => RightRotate(x, 2) ^ RightRotate(x, 13) ^ RightRotate(x, 22);
            static uint E1(uint x) => RightRotate(x, 6) ^ RightRotate(x, 11) ^ RightRotate(x, 25);

            var t1 = h + E1(e) + Ch(e, f, g) + K[i] + w[i];
            var t2 = E0(a) + Maj(a, b, c);

            (h, g, f, e, d, c, b, a) = (g, f, e, d + t1, c, b, a, t1 + t2);
        }

        (h0, h1, h2, h3, h4, h5, h6, h7) = (h0 + a, h1 + b, h2 + c, h3 + d, h4 + e, h5 + f, h6 + g, h7 + h);
    }
}
