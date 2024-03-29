﻿#nullable enable
using System.Numerics;

using static System.Math;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore;

/// <summary>Дробь</summary>
/// <remarks>Новая дробь</remarks>
/// <param name="numerator">Числитель</param>
/// <param name="denominator">Знаменатель</param>
public readonly struct Fraction(long numerator, ulong denominator)
{
    /// <summary>Упрощение дроби</summary>
    /// <param name="Numerator">Числитель</param>
    /// <param name="Denominator">Знаменатель</param>
    /// <returns>Истина, если дробь можно упростить</returns>
    public static bool Simplify(ref int Numerator, ref int Denominator)
    {
        var gcd = Numeric.GCD(Abs(Numerator), Abs(Denominator));
        if (gcd == 1) return false;
        Numerator   /= gcd;
        Denominator /= gcd;
        return true;
    }

    /// <summary>Упрощение дроби</summary>
    /// <param name="Numerator">Числитель</param>
    /// <param name="Denominator">Знаменатель</param>
    /// <returns>Истина, если дробь можно упростить</returns>
    public static bool Simplify(ref long Numerator, ref long Denominator)
    {
        var gcd = Numeric.GCD(Abs(Numerator), Abs(Denominator));
        if (gcd == 1) return false;
        Numerator   /= gcd;
        Denominator /= gcd;
        return true;
    }

    /// <summary>Упрощение дроби</summary>
    /// <param name="Numerator">Числитель</param>
    /// <param name="Denominator">Знаменатель</param>
    /// <returns>Истина, если дробь можно упростить</returns>
    public static bool Simplify(ref ulong Numerator, ref ulong Denominator)
    {
        var gcd = Numeric.GCD(Numerator, Denominator);
        if (gcd == 1) return false;
        Numerator   /= gcd;
        Denominator /= gcd;
        return true;
    }

    /// <summary>Упрощение дроби</summary>
    /// <param name="Numerator">Числитель</param>
    /// <param name="Denominator">Знаменатель</param>
    /// <returns>Истина, если дробь можно упростить</returns>
    public static bool Simplify(ref BigInt Numerator, ref BigInt Denominator)
    {
        var gcd = Numerator.Abs().Gcd(Denominator.Abs());
        if (gcd == 1) return false;
        Numerator   /= gcd;
        Denominator /= gcd;
        return true;
    }

    /// <summary>Упрощение дроби</summary>
    /// <param name="Numerator">Числитель</param>
    /// <param name="Denominator">Знаменатель</param>
    /// <returns>Истина, если дробь можно упростить</returns>
    public static bool Simplify(ref BigInteger Numerator, ref BigInteger Denominator)
    {
        var gcd = Numeric.GCD(BigInteger.Abs(Numerator), BigInteger.Abs(Denominator));
        if (gcd == 1) return false;
        Numerator   /= gcd;
        Denominator /= gcd;
        return true;
    }

    /// <summary>Числитель</summary>
    private readonly long _Numerator = numerator;

    /// <summary>Знаменатель</summary>
    private readonly ulong _Denominator = denominator;

    /// <summary>Числитель</summary>
    public long Numerator => _Numerator;

    /// <summary>Знаменатель</summary>
    public ulong Denominator => _Denominator;

    /// <summary>Десятичное значение дроби</summary>
    public double DecimalValue => (double)_Numerator / _Denominator;

    public bool IsNaN => _Denominator == 0 && _Numerator == 0;

    public bool IsPositiveInfinity => _Denominator == 0 && _Numerator > 0;

    public bool IsNegativeInfinity => _Denominator == 0 && _Numerator < 0;

    public bool IsInfinity => _Denominator == 0 && _Numerator != 0;

    public bool IsNormal => _Denominator > 0;

    [NotImplemented]
    private static Fraction FromDoubleIEEE754(double x)
    {
        var bits = (ulong)BitConverter.DoubleToInt64Bits(x);

        //var ss = 0b00;
        //var ee = 0b__111111_1000; // 1016
        //var em = 0b__111111_1111;
        //var ff = 0b_____________1000_00000000_00000000_00000000_00000000_00000000_00000000; // 2251799813685248
        //var fm = 0b_____________1111_11111111_11111111_11111111_11111111_11111111_11111111; // 2251799813685248
        //                        F    FF       FF       FF       FF       FF       FF

        const ulong fraction_mask = 0xF_FFFF_FFFF_FFFF; // (1 << 52) - 1 :: младшие 52 бита
        var         fraction      = bits & fraction_mask;

        const int exponent_mask = 0x7FF; // (1 << 11) - 1 :: 11 бит
        var       exponent      = (int)((bits >> 52) & exponent_mask);

        var sign = bits >> 63 == 1;

        var s = sign ? -1 : 1;

        var v0 = s * (1 + (double)fraction / (1L << 52)) * Pow(2, exponent - 1023);

        var l  = 0x10000000000000UL; // 1 << 52
        var v2 = s * (double)(l + fraction) / l * Pow(2, exponent - 1023);
        var v3 = s * (double)(l + fraction) * Pow(2, exponent - (1023 + 52));
        var v4 = s * (double)(l + fraction) * Pow(2, exponent - 1075);

        var ee  = 1075 - exponent;
        var pow = Pow(2, ee);
        var ll  = l + fraction;
        var v5  = ll / pow;

        var ee1 = ee;
        var ll1 = ll;
        while ((ll1 & 1) == 0)
        {
            ll1 >>= 1;
            ee1--;
        }

        // x = (-1)^sign * (1 + fraction / 2^52) * 2^(exponent - 1023)
        // x =         s * (1 +        f /    b) * e
        // x = s * (b + f )/b * e

        const ulong b   = 0x10000000000000; // 2^52
        var         exp = exponent - 1023;

        // 0x 3ff0 0000 0000 0000 =  1
        // 0x 3ff0 0000 0000 0001 ≈  1.0000000000000002
        // 0x 3ff0 0000 0000 0002 ≈  1.0000000000000004
        // 0x 4000 0000 0000 0000 =  2
        // 0x c000 0000 0000 0000 = –2

        // 0x 0000 0000 0000 0000 =  0
        // 0x 8000 0000 0000 0000 = –0
        // 0x 7ff0 0000 0000 0000 =  Infinity
        // 0x fff0 0000 0000 0000 = −Infinity
        // 0x 7fff ffff ffff ffff =  NaN
        // 0x 3fd5 5555 5555 5555 ≈  1/3

        var numerator = b + fraction;
        var k         = 52 - exp;
        while (numerator % 2 == 0 && k > 0)
        {
            numerator >>= 1;
            k--;
        }

        var denominator = 1UL << k;

        Simplify(ref numerator, ref denominator);

        var result = new Fraction(sign ? -(long)numerator : (long)numerator, denominator);
        return result;
    }

    /// <summary>Получить упрощённую дробь</summary>
    /// <returns>Упрощённая дробь</returns>
    public Fraction GetSimplified()
    {
        if (!IsNormal) return this;

        var sign          = Sign(_Numerator);
        var numerator_abs = (ulong)Abs(_Numerator);
        var gcd           = Numeric.GCD(numerator_abs, _Denominator);
        return gcd == 1
            ? this
            : new
            (
                numerator:   (long)(numerator_abs / gcd) * sign,
                denominator: _Denominator / gcd
            );
    }

    /// <inheritdoc />
    public override string ToString() => (_Numerator, _Denominator) switch
    {
        (0,    0) => "NaN",
        ( > 0, 0) => "+Infinity",
        ( < 0, 0) => "-Infinity",
        _         => $"{_Numerator}/{_Denominator}"
    };

    /// <inheritdoc />
    public override bool Equals(object obj) => base.Equals(obj);

    /// <summary>Проверка на эквивалентность дроби</summary>
    /// <param name="other">Проверяемая дробь</param>
    /// <returns>Истина, если дроби идентичные</returns>
    public bool Equals(Fraction other)
    {
        var sign        = Sign(_Numerator);
        var numerator   = (ulong)Abs(_Numerator);
        var denominator = _Denominator;

        var other_sign        = Sign(other._Numerator);
        var other_numerator   = (ulong)Abs(other._Numerator);
        var other_denominator = other._Denominator;

        Simplify(ref numerator, ref denominator);
        Simplify(ref other_numerator, ref other_denominator);

        return sign == other_sign
            && numerator == other_numerator
            && denominator == other_denominator;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var sign        = Sign(_Numerator);
        var numerator   = (ulong)Abs(_Numerator);
        var denominator = _Denominator;
        Simplify(ref numerator, ref denominator);
        unchecked
        {
            var hash = numerator;
            hash = hash * 0x18D ^ denominator;
            hash = hash * 0x18D ^ (ulong)sign;
            return (int)hash;
        }
    }
}