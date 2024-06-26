﻿// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

public static class LongExtensions
{
    public static long ReverseBits(this long b) => (long)((ulong)b).ReverseBits();

    public static ulong ReverseBits(this ulong b)
    {
        //b = (b & 0b11111111_11111111_11111111_11111111_00000000_00000000_00000000_00000000) >> 32 | (b & 0b00000000_00000000_00000000_00000000_11111111_11111111_11111111_11111111) << 32;
        //b = (b & 0b11111111_11111111_00000000_00000000_11111111_11111111_00000000_00000000) >> 16 | (b & 0b00000000_00000000_11111111_11111111_00000000_00000000_11111111_11111111) << 16;
        //b = (b & 0b11111111_00000000_11111111_00000000_11111111_00000000_11111111_00000000) >> 08 | (b & 0b00000000_11111111_00000000_11111111_00000000_11111111_00000000_11111111) << 08;
        //b = (b & 0b11110000_11110000_11110000_11110000_11110000_11110000_11110000_11110000) >> 04 | (b & 0b00001111_00001111_00001111_00001111_00001111_00001111_00001111_00001111) << 04;
        //b = (b & 0b11001100_11001100_11001100_11001100_11001100_11001100_11001100_11001100) >> 02 | (b & 0b00110011_00110011_00110011_00110011_00110011_00110011_00110011_00110011) << 02;
        //b = (b & 0b10101010_10101010_10101010_10101010_10101010_10101010_10101010_10101010) >> 01 | (b & 0b01010101_01010101_01010101_01010101_01010101_01010101_01010101_01010101) << 01;
        
        b = (b & 0xFF_FF_FF_FF_00_00_00_00) >> 32 | (b & 0x00_00_00_00_FF_FF_FF_FF) << 32;
        b = (b & 0xFF_FF_00_00_FF_FF_00_00) >> 16 | (b & 0x00_00_FF_FF_00_00_FF_FF) << 16;
        b = (b & 0xFF_00_FF_00_FF_00_FF_00) >> 08 | (b & 0x00_FF_00_FF_00_FF_00_FF) << 08;
        b = (b & 0xF0_F0_F0_F0_F0_F0_F0_F0) >> 04 | (b & 0x0F_0F_0F_0F_0F_0F_0F_0F) << 04;
        b = (b & 0xCC_CC_CC_CC_CC_CC_CC_CC) >> 02 | (b & 0x33_33_33_33_33_33_33_33) << 02;
        b = (b & 0xAA_AA_AA_AA_AA_AA_AA_AA) >> 01 | (b & 0x55_55_55_55_55_55_55_55) << 01;

        return b;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this long N)
    {
        var n = Math.Abs(N);
        if(n % 2 == 0) return n == 2;

        var max = (long)Math.Sqrt(n);

        for(var i = 3; i <= max; i += 2)
            if(n % i == 0)
                return false;

        return true;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this ulong N)
    {
        if(N % 2 == 0) return N == 2;

        var max = (long)Math.Sqrt(N);

        for(var i = 3u; i <= max; i += 2)
            if(N % i == 0)
                return false;

        return true;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this long N)
    {
        var n = Math.Abs(N);
        return (n & (n - 1)) == 0 || n == 1;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this ulong N) => (N & (N - 1)) == 0 || N == 1;

    /// <summary>Число бит числа</summary>
    /// <param name="x">Значащее число</param>
    /// <returns>Число бит числа</returns>
    [DST]
    public static int BitCount(this long x) => (int)Math.Round(Math.Log(x, 2));

    /// <summary>Реверсирование бит числа</summary>
    /// <param name="x">исходное число</param>
    /// <param name="N">Число реверсируемых бит</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static long BitReversing(this long x, int N)
    {
        var result = 0L;
        for(var i = 0; i < N; i++)
        {
            result <<= 1;
            result +=  x & 1;
            x      >>= 1;
        }
        return result;
    }

    /// <summary>Реверсирование всех 64 бит числа</summary>
    /// <param name="N">исходное число</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static long BitReversing(this long N) => N.BitReversing(16);

    [DST]
    public static bool IsDeviatedTo(this long x, long y) => x % y == 0;

    [DST]
    public static long GetAbsMod(this long x, long mod) => x % mod + (x < 0 ? mod : 0);

    /// <summary>Является ли число нечётным</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число нечётное</returns>
    [DST]
    public static bool IsOdd(this long N) => !N.IsEven();

    /// <summary>Является ли число чётным</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число чётное</returns>
    [DST]
    public static bool IsEven(this long N) => Math.Abs(N).IsDeviatedTo(2);

    /// <summary>Факториал целого числа от 0 до 20</summary>
    /// <param name="n">Исходное число в пределах от 0 до 20</param>
    /// <exception cref="ArgumentOutOfRangeException">При 0 &gt; <paramref name="n"/> &gt; 20</exception>
    /// <returns>Факториал числа</returns>
    [DST]
    public static long Factorial(this long n) => n switch
    {
        0L  => 1L,
        1L  => 1L,
        2L  => 2L,
        3L  => 6L,
        4L  => 24L,
        5L  => 120L,
        6L  => 720L,
        7L  => 5040L,
        8L  => 40320L,
        9L  => 362880L,
        10L => 3628800L,
        11L => 39916800L,
        12L => 479001600L,
        13L => 6227020800L,
        14L => 87178291200L,
        15L => 1307674368000L,
        16L => 20922789888000L,
        17L => 355687428096000L,
        18L => 6402373705728000L,
        19L => 121645100408832000L,
        20L => 2432902008176640000L,
        //21L => 51090942171709440000L,   // <- Тут начинается переполнение long
        //22L => 1124000727777607680000L,
        //23L => 25852016738884976640000L,
        //24L => 620448401733239439360000L,
        //25L => 15511210043330985984000000L,
        //26L => 403291461126605635584000000L,
        //27L => 10888869450418352160768000000L,
        //28L => 304888344611713860501504000000L,
        //29L => 8841761993739701954543616000000L,
        //30L => 265252859812191058636308480000000L,
        < 0 => throw new ArgumentOutOfRangeException(nameof(n), n, "Значение должно быть неотрицательным"),
        _   => throw new ArgumentOutOfRangeException(nameof(n), n, "Вычисление факториала возможно лишь до значения n <= 20")
    };

    /// <summary>Факториал целого числа от 0 до 20</summary>
    /// <param name="n">Исходное число в пределах от 0 до 20</param>
    /// <exception cref="ArgumentOutOfRangeException">При 0 &gt; <paramref name="n"/> &gt; 20</exception>
    /// <returns>Факториал числа</returns>
    [DST]
    public static ulong Factorial(this ulong n) => n switch
    {
        0U  => 1U,
        1U  => 1U,
        2U  => 2U,
        3U  => 6U,
        4U  => 24U,
        5U  => 120U,
        6U  => 720U,
        7U  => 5040U,
        8U  => 40320U,
        9U  => 362880U,
        10U => 3628800U,
        11U => 39916800U,
        12U => 479001600U,
        13U => 6227020800U,
        14U => 87178291200U,
        15U => 1307674368000U,
        16U => 20922789888000U,
        17U => 355687428096000U,
        18U => 6402373705728000U,
        19U => 121645100408832000U,
        20U => 2432902008176640000U,
        //21U => 51090942171709440000U,   // <- Тут начинается переполнение long
        //22U => 1124000727777607680000U,
        //23U => 25852016738884976640000U,
        //24U => 620448401733239439360000U,
        //25U => 15511210043330985984000000U,
        //26U => 403291461126605635584000000U,
        //27U => 10888869450418352160768000000U,
        //28U => 304888344611713860501504000000U,
        //29U => 8841761993739701954543616000000U,
        //30U => 265252859812191058636308480000000U,
        _ => throw new ArgumentOutOfRangeException(nameof(n), n, "Вычисление факториала возможно лишь до значения n <= 20")
    };

    [DST]
    public static long GetFlags(this long Value, long Mask) => Value & Mask;

    [DST]
    public static long SetFlag(this long Value, long Flag, long Mask) => (Value & ~Mask) | (Flag & Mask);

    /// <summary>Возведение целого числа в целую степень</summary>
    /// <param name="x">Целое основание</param>
    /// <param name="p">Целый показатель степени</param>
    /// <returns>Результат возведения целого основания в целую степень</returns>
    [DST]
    public static long Pow(this long x, int p)
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

        var result = x;
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
}