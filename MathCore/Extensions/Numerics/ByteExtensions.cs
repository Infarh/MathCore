// ReSharper disable once CheckNamespace
namespace System;

public static class ByteExtensions
{
    public static byte ReverseBits(this byte b)
    {
        b = (byte)((b & 0b1111_0000) >> 4 | (b & 0b0000_1111) << 4);
        b = (byte)((b & 0b1100_1100) >> 2 | (b & 0b0011_0011) << 2);
        b = (byte)((b & 0b1010_1010) >> 1 | (b & 0b0101_0101) << 1);

        return b;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this byte N)
    {
        if (N % 2 == 0) return N == 2;

        var max = (byte)Math.Sqrt(N);

        for (var i = 3; i <= max; i += 2)
            if (N % i == 0)
                return false;

        return true;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this sbyte N)
    {
        var n = Math.Abs(N);
        if (n % 2 == 0) return n == 2;

        var max = (byte)Math.Sqrt(n);

        for (var i = 3; i <= max; i += 2)
            if (n % i == 0)
                return false;

        return true;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this byte x) => (x & (x - 1)) == 0 || x == 1;

    /// <summary>Число бит числа</summary>
    /// <param name="x">Значащее число</param>
    /// <returns>Число бит числа</returns>
    [DST]
    public static int BitCount(this byte x) => (int)Math.Round(Math.Log(x, 2));

    /// <summary>Реверсирование бит числа</summary>
    /// <param name="x">исходное число</param>
    /// <param name="N">Число резервируемых бит [ = 16 ]</param>
    /// <returns>Инверсированное число</returns>
    [DST]
    public static byte BitReversing(this byte x, int N = 16)
    {
        byte result = 0;
        for(var i = 0; i < N; i++)
        {
            result <<= 1;
            result +=  (byte)(x & 1);
            x      >>= 1;
        }
        return result;
    }

    [DST]
    public static bool IsDeviatedTo(this byte x, byte y) => x % y == 0;

    [DST]
    public static byte GetAbsMod(this byte x, byte mod) => (byte)(x % mod);

    /// <summary>Является ли число нечётным</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число нечётное</returns>
    [DST]
    public static bool IsOdd(this byte x) => !x.IsEven();

    /// <summary>Является ли число чётным</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число чётное</returns>
    [DST]
    public static bool IsEven(this byte x) => x.IsDeviatedTo(2);

    [DST]
    public static bool IsSetBit(this byte x, int BitN)
    {
        var num = (byte)(1 << BitN);
        return (x & num) == num;
    }

    [DST]
    public static byte SetBit(this byte x, int BitN, bool Set)
    {
        var num = (byte)(1 << BitN);
        return Set ? (byte)(x | num) : (byte)(x & ~num);
    }

    [DST]
    public static int ToOctBase(this byte x)
    {
        var num = 0;
        for(var i = 1; x != 0; i *= 10)
        {
            num +=  x % 8 * i;
            x   >>= 3;
        }
        return num;
    }

    [DST] public static byte GetFlags(this byte Value, byte Mask) => (byte)(Value & Mask);

    [DST] public static byte SetFlag(this byte Value, byte Flag, byte Mask) => (byte)((Value & ~Mask) | (Flag & Mask));
}