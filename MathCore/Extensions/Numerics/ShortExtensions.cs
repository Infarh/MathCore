﻿using MathCore;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class ShortExtensions
{
    public static short ReverseBits(this short b) => (short)((ushort)b).ReverseBits();

    public static ushort ReverseBits(this ushort b)
    {
        b = (ushort)((b & 0b1111_1111_0000_0000) >> 8 | (b & 0b0000_0000_1111_1111) << 8);
        b = (ushort)((b & 0b1111_0000_1111_0000) >> 4 | (b & 0b0000_1111_0000_1111) << 4);
        b = (ushort)((b & 0b1100_1100_1100_1100) >> 2 | (b & 0b0011_0011_0011_0011) << 2);
        b = (ushort)((b & 0b1010_1010_1010_1010) >> 1 | (b & 0b0101_0101_0101_0101) << 1);

        return b;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this short N)
    {
        var n = Math.Abs(N);
        if (n % 2 == 0) return false;

        var max = (short)Math.Sqrt(n);

        for (var i = 3; i <= max; i += 2)
            if (n % i == 0)
                return false;

        return true;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this ushort N)
    {
        if (N % 2 == 0) return N == 2;

        var max = (short)Math.Sqrt(N);

        for (var i = 3; i <= max; i += 2)
            if (N % i == 0)
                return false;

        return true;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this short N)
    {
        var n = Math.Abs(N);
        return (n & (n - 1)) == 0 || n == 1;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this ushort N) => (N & (N - 1)) == 0 || N == 1;

    /// <summary>Число бит числа</summary>
    /// <param name="N">Значащее число</param>
    /// <returns>Число бит числа</returns>
    [DST]
    public static int BitCount(this short N) => (int)Math.Round(Math.Log(N, 2));

    /// <summary>Реверсирование бит числа</summary>
    /// <param name="x">исходное число</param>
    /// <param name="N">Число реверсируемых бит</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static short BitReversing(this short x, int N)
    {
        short result = 0;
        for (var i = 0; i < N; i++)
        {
            result <<= 1;
            result +=  (short)(x & 1);
            x      >>= 1;
        }
        return result;
    }

    /// <summary>Реверсирование всех 16 бит числа</summary>
    /// <param name="N">исходное число</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static short BitReversing(this short N) => N.BitReversing(16);

    [DST]
    public static bool IsDeviatedTo(this short x, short y) => x % y == 0;

    [DST]
    public static short GetAbsMod(this short x, short mod) => (short)(x % mod + (x < 0 ? mod : 0));

    /// <summary>Является ли число нечётным</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число нечётное</returns>
    [DST]
    public static bool IsOdd(this short N) => !N.IsEven();

    /// <summary>Является ли число чётным</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число чётное</returns>
    [DST]
    public static bool IsEven(this short N) => Math.Abs(N).IsDeviatedTo(2);

    [DST]
    public static int Pow(this short x, int p)
    {
        switch (x)
        {
            case 0: return 0;
            case 1: return 1;
        }

        switch (p)
        {
            case < 0: return 0;
            case 0: return 1;
            case 1: return x;
            case 2: return x * x;
            case 3: return x * x * x;
            case 4: return x * x * x * x;
            }

        int result = x;
        var power = p;

        if (p < 11)
            while (--power > 0)
                result *= x;
        else
        {
            while (power > 0 && power % 2 == 0)
            {
                result *= result;
                power >>= 1;
            }

            while (power > 0 && power % 3 == 0)
            {
                result *= result * result;
                power /= 3;
            }

            if (power > 1)
                return result * result.Pow(power - 1);
        }

        return result;
    }

    [DST]
    public static Complex Pow(this short x, Complex z) => x ^ z;

    [DST]
    public static short GetFlags(this short Value, short Mask) => (short)(Value & Mask);

    [DST]
    public static short SetFlag(this short Value, short Flag, short Mask) => (short)((Value & ~Mask) | (Flag & Mask));
}