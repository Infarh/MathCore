#nullable enable
using System.Collections;
using System.Numerics;

using MathCore;

using Complex = MathCore.Complex;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Класс методов-расширений для класса целых 4-х-байтовых чисел со знаком</summary>
public static class IntExtensions
{
    public static (int Div, int Mod) GetDivMod(this int x, int y) => (x / y, x % y);

    public static int HiBit(this int x) => Numeric.HiBit(x);

    public static int Log2(this int x) => Numeric.Log2(x);

    /// <summary>Возведение целого числа в целую степень</summary>
    /// <param name="x">Целое основание</param>
    /// <param name="p">Целый показатель степени</param>
    /// <returns>Результат возведения целого основания в целую степень</returns>
    [DST]
    public static int Pow(this int x, int p)
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

    /// <summary>Возведение целого числа в вещественную степень</summary>
    /// <param name="x">Целое основание</param>
    /// <param name="q">Вещественный показатель степени</param>
    /// <returns>Результат возведения целого основания в вещественную степень</returns>
    [DST]
    public static double Power(this int x, double q) => Math.Pow(x, q);

    /// <summary>Возведение целого числа в комплексную степень</summary>
    /// <param name="x">Целое основание</param>
    /// <param name="z">Комплексный показатель степени</param>
    /// <returns>Результат возведения целого основания в комплексную степень</returns>
    [DST]
    public static Complex Power(this int x, Complex z) => x ^ z;

    /// <summary>Факторизация целого числа</summary>
    /// <param name="N">Раскладываемое число</param>
    /// <returns>Последовательность простых чисел составляющих раскладываемое число</returns>
    public static IEnumerable<int> FactorizationEnum(this int N)
    {
        var n = Math.Abs(N);
        if (n == 1) yield break;

        // пока число четное
        while ((n & 1) == 0)
        {
            n >>= 1;
            yield return 2;
        }

        //как только кончились двойки в числе, переходим к 3 и далее по списку простых чисел
        for(var d = 3; n != 1; d += 2)
            while (n % d == 0)
            {
                yield return d;
                n /= d;
            }
    }

    /// <summary>Разложение числа на простые множители</summary>
    /// <param name="N">Раскладываемое число</param>
    /// <returns>Массив простых множителей</returns>
    //[Copyright("Alexandr A Alexeev 2011", url = "http://eax.me")]
    public static int[] FactorizationList(this int N)
    {
        var n = Math.Abs(N);
        if (n == 1) return Array.Empty<int>();

        var result = new List<int>();
        // пока число четное
        while ((n & 1) == 0)
        {
            n >>= 1;
            result.Add(2);
        }

        for (var d = 3; n != 1; d += 2)
            while (n % d == 0)
            {
                result.Add(d);
                n /= d;
            }

        return result.ToArray();
    }

    /// <summary>Разложение числа на простые множители</summary>
    /// <param name="N">Раскладываемое число</param>
    /// <returns>Словарь с делителями числа - значение элементов словаря - кратность делителя</returns>
    public static Dictionary<int, int> Factorization(this int N)
    {
        var n      = Math.Abs(N);
        var result = new Dictionary<int, int>();
        if (n == 1) return result;

        result.Add(2, 0);
        // пока число четное
        while ((n & 1) == 0)
        {
            n >>= 1;
            result[2]++;
        }

        for (var d = 3; n != 1; d += 2)
            while (n % d == 0)
            {
                result[d] = result.TryGetValue(d, out var count) 
                    ? count + 1 
                    : 1;
                n /= d;
            }

        return result;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this int N)
    {
        var n = Math.Abs(N);
        if (n % 2 == 0) return n == 2;

        var max = (int)Math.Sqrt(n);

        for (var i = 3; i <= max; i += 2)
            if (n % i == 0)
                return false;

        return true;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    [DST]
    public static bool IsPrime(this uint N)
    {
        if (N % 2 == 0) return N == 2;

        var max = (int)Math.Sqrt(N);

        for (var i = 3; i <= max; i += 2)
            if (N % i == 0)
                return false;

        return true;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this int N)
    {
        var n = Math.Abs(N);
        return (n & (n - 1)) == 0 || n == 1;
    }

    /// <summary>Является ли число степенью двойки?</summary>
    /// <param name="N">Проверяемое число</param>
    /// <returns>Истина, если число - степень двойки 1,2,4...1024,2048...2^n</returns>
    [DST]
    public static bool IsPowerOf2(this uint N) => (N & (N - 1)) == 0 || N == 1;

    /// <summary>Определяет номер старшего бита в числе (индексация с 1)</summary>
    /// <param name="n">Исходное число</param>
    /// <returns>Число бит всех числа (включая нули)</returns>
    [DST]
    public static int BitCount(this int n) => n.GetNumberOfDigits(2);

    /// <summary>Получить число разрядов в указанной системе счисления</summary>
    /// <param name="n">Рассматриваемое число</param>
    /// <param name="Base">Основание системы счисления. По умолчанию = 10</param>
    /// <returns>Количество разрядов в указанной системе счисления</returns>
    [DST]
    public static int GetNumberOfDigits(this int n, int Base = 10) => (int)Math.Log(n < 0 ? -n : n, Base) + 1;

    /// <summary>Получить битовый массив из числа</summary>
    /// <param name="Value">Преобразуемое число</param>
    /// <param name="Length">Длина результирующего массива бит. По умолчанию = 32 битам</param>
    /// <returns>Битовый массив числа</returns>
    [DST]
    public static BitArray GetBitArray(this int Value, int Length = 32)
    {
        var array = new BitArray(Length);
        for (var i = 0; i < Length; i++)
        {
            array[i] =   (Value & 1) == 1;
            Value    >>= 1;
        }
        return array;
    }

    /// <summary>Реверсирование бит числа</summary>
    /// <param name="x">исходное число</param>
    /// <param name="N">Число реверсируемых бит</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static int BitReversing(this int x, int N)
    {
        var result = 0;
        for (var i = 0; i < N; i++)
        {
            result <<= 1;
            result +=  x & 1;
            x      >>= 1;
        }
        return result;
    }

    /// <summary>Реверсирование всех 32 бит числа</summary>
    /// <param name="x">исходное число</param>
    /// <returns>Реверсированное число</returns>
    [DST]
    public static int BitReversing(this int x) => x.BitReversing(sizeof(int) * 8);

    /// <summary>Проверка делимости числа на делитель</summary>
    /// <param name="x">Делимое</param>
    /// <param name="y">Делитель</param>
    /// <returns>Истина, если остаток от целочисленного деления равен 0</returns>
    [DST]
    public static bool IsDeviatedTo(this int x, int y) => x % y == 0;

    /// <summary>Положительный остаток от деления</summary>
    /// <param name="x">Делимое</param>
    /// <param name="mod">Модуль</param>
    /// <returns>Остаток от деления</returns>
    [DST]
    public static int GetAbsMod(this int x, int mod) => (x % mod) + (x < 0 ? mod : 0);

    /// <summary>Получить абсолютное значение числа</summary>
    /// <param name="x">Вещественное число</param>
    /// <returns>Модуль числа</returns>
    [DST]
    public static int GetAbs(this int x) => Math.Abs(x);

    /// <summary>Наибольший общий делитель</summary>
    /// <param name="a">Первое число</param>
    /// <param name="b">Второе число</param>
    /// <returns>Наибольший общий делитель</returns>
    [DST]
    public static int GetNOD(this int a, int b) => Numeric.GCD(a, b);

    /// <summary>Наибольший общий делитель</summary>
    /// <param name="a">Первое число</param>
    /// <param name="b">Второе число</param>
    /// <returns>Наибольший общий делитель</returns>
    [DST]
    public static long GetNOD(this long a, long b) => Numeric.GCD(a, b);

    /// <summary>Наибольший общий делитель</summary>
    /// <param name="a">Первое число</param>
    /// <param name="b">Второе число</param>
    /// <returns>Наибольший общий делитель</returns>
    [DST]
    public static ulong GetNOD(this ulong a, ulong b) => Numeric.GCD(a, b);

    /// <summary>Наибольший общий делитель</summary>
    /// <param name="a">Первое число</param>
    /// <param name="b">Второе число</param>
    /// <returns>Наибольший общий делитель</returns>
    [DST]
    public static BigInteger GetNOD(this BigInteger a, BigInteger b) => Numeric.GCD(a, b);

    /// <summary>Является ли число нечётным</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число нечётное</returns>
    [DST]
    public static bool IsOdd(this int x) => !x.IsEven();

    /// <summary>Является ли число чётным</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число чётное</returns>
    [DST]
    public static bool IsEven(this int x) => x.IsDeviatedTo(2);

    /// <summary>Факториал целого числа от 0 до 20</summary>
    /// <param name="n">Исходное число в пределах от 0 до 20</param>
    /// <exception cref="ArgumentOutOfRangeException">При 0 &gt; <paramref name="n"/> &gt; 20</exception>
    /// <returns>Факториал числа</returns>
    [DST]
    public static long Factorial(this int n) => n switch
    {
        0  => 1,
        1  => 1,
        2  => 2,
        3  => 6,
        4  => 24,
        5  => 120,
        6  => 720,
        7  => 5040,
        8  => 40320,
        9  => 362880,
        10 => 3628800,
        11 => 39916800,
        12 => 479001600,
        13 => 6227020800,
        14 => 87178291200,
        15 => 1307674368000,
        16 => 20922789888000,
        17 => 355687428096000,
        18 => 6402373705728000,
        19 => 121645100408832000,
        20 => 2432902008176640000,
        //21 => 51090942171709440000,   // <- Тут начинается переполнение long
        //22 => 1124000727777607680000,
        //23 => 25852016738884976640000,
        //24 => 620448401733239439360000,
        //25 => 15511210043330985984000000,
        //26 => 403291461126605635584000000,
        //27 => 10888869450418352160768000000,
        //28 => 304888344611713860501504000000,
        //29 => 8841761993739701954543616000000,
        //30 => 265252859812191058636308480000000,
        < 0 => throw new ArgumentOutOfRangeException(nameof(n), n, "Значение должно быть неотрицательным"),
        _   => throw new ArgumentOutOfRangeException(nameof(n), n, "Вычисление факториала возможно лишь до значения n <= 20")
    };

    //private static readonly long[] __Factorial =
    //{
    //    /*  0 */ 1,
    //    /*  1 */ 1,
    //    /*  2 */ 2,
    //    /*  3 */ 6,
    //    /*  4 */ 24,
    //    /*  5 */ 120,
    //    /*  6 */ 720,
    //    /*  7 */ 5040,
    //    /*  8 */ 40320,
    //    /*  9 */ 362880,
    //    /* 10 */ 3628800,
    //    /* 11 */ 39916800,
    //    /* 12 */ 479001600,
    //    /* 13 */ 6227020800,
    //    /* 14 */ 87178291200,
    //    /* 15 */ 1307674368000,
    //    /* 16 */ 20922789888000,
    //    /* 17 */ 355687428096000,
    //    /* 18 */ 6402373705728000,
    //    /* 19 */ 121645100408832000,
    //    /* 20 */ 2432902008176640000,
    //    // /* 21 */ 51090942171709440000,   // <- Тут начинается переполнение long
    //    // /* 22 */ 1124000727777607680000,
    //    // /* 23 */ 25852016738884976640000,
    //    // /* 24 */ 620448401733239439360000,
    //    // /* 25 */ 15511210043330985984000000,
    //    // /* 26 */ 403291461126605635584000000,
    //    // /* 27 */ 10888869450418352160768000000,
    //    // /* 28 */ 304888344611713860501504000000,
    //    // /* 29 */ 8841761993739701954543616000000,
    //    // /* 30 */ 265252859812191058636308480000000,
    //};

    ///// <summary>Факториал целого числа от 0 до 20</summary>
    ///// <param name="n">Исходное число в пределах от 0 до 20</param>
    ///// <exception cref="ArgumentOutOfRangeException">При <paramref name="n"/> &lt; 0</exception>
    ///// <exception cref="OverflowException">При <paramref name="n"/> &gt; 20</exception>
    ///// <returns>Факториал числа</returns>
    //[DST]
    //public static long Factorial(this int n)
    //{
    //    //if(n < 0) return (long)SpecialFunctions.Gamma.G(n);
    //    if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Значение должно быть неотрицательным") { Data = { { "n", n } } };
    //    if (n < __Factorial.Length)
    //        return __Factorial[n];

    //    throw new OverflowException("Вычисление факториала возможно лишь до значения n == 20") { Data = { { "n", n } } };

    //    //if(n > 40)
    //    //{
    //    //    var sqrt = Math.Sqrt(Consts.pi2 * n);
    //    //    var truncate = Math.Pow(n / Math.E, n);

    //    //    var r1 = new[]
    //    //    {
    //    //        1,
    //    //        1d / (12 * n),
    //    //        1d / (288 * n * n),
    //    //        139d / (51840 * n * n * n)
    //    //    };

    //    //    return (long)(sqrt * truncate * (r1[0] + r1[1] + r1[2] + r1[3]));
    //    //}

    //    //long result = n == 0 ? 1 : n;
    //    //while(n > 1) result *= --n;
    //    //return result;
    //}

    /// <summary>Факториал целого числа >= 0 и значение Г-функции для отрицательных значений</summary>
    /// <param name="n">Исходное число</param>
    /// <exception cref="ArgumentOutOfRangeException">При <paramref name="n"/> &lt; 0</exception>
    /// <returns>Факториал числа</returns>
    [DST]
    public static BigInteger FactorialBigInt(this int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Значение должно быть неотрицательным") { Data = { { "n", n } } };
        //if (n < 0) return (long)SpecialFunctions.Gamma.G(n);

        if (n <= 20) return n.Factorial();

        var result = new BigInteger(n);
        for (var k = n; k > 1; k--)
            result *= k;

        return result;
    }


    /// <summary>Приведение целого числа в 10 системе счисления к виду системы счисления по основанию 8</summary>
    /// <param name="n">Число в 10-ой системе счисления</param>
    /// <returns>Представление числа в 8-ричной системе счисления</returns>
    [DST]
    public static int ToOctBase(this int n)
    {
        var y = 0;
        for (var i = 1; n != 0; i *= 10)
        {
            y +=  (n % 8) * i;
            n >>= 3;
        }
        return y;
    }

    /// <summary>Приведение целого числа в 8 системе счисления к виду системы счисления по основанию 10</summary>
    /// <param name="x">Число в 8-ой системе счисления</param>
    /// <returns>Представление числа в 10-ричной системе счисления</returns>
    [DST]
    public static int FromOctalBase(this int x)
    {
        var n = x % 10;
        x /= 10;
        var b = 8;
        while (x != 0)
        {
            n += (x % 10) * b;
            b *= 8;
            x /= 10;
        }
        return n;
    }

    public static int[] ToBase(this int x, int Base = 16)
    {
        var result = new int[GetNumberOfDigits(x, Base)];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] =  x % Base;
            x         /= Base;
        }

        return result;
    }

    [DST]
    public static int GetFlags(this int Value, int Mask) => Value & Mask;

    [DST]
    public static int SetFlag(this int Value, int Flag, int Mask) => (Value & ~Mask) | (Flag & Mask);

    public static int ToInteger(this byte[] data, int Offset = 0, bool IsMsbFirst = true)
    {
        if (data.Length == 0) return 0;
        var  result = 0;
        bool sign;

        if (IsMsbFirst)
        {
            sign = (data[0] & 0x80) == 0x80;
            for (var i = Offset; i < data.Length; i++)
            {
                result <<= 8;
                result +=  data[i];
            }
        }
        else
        {
            sign = (data[^1] & 0x80) == 0x80;
            for (var i = 0; i + Offset < data.Length; i++)
                result += data[i + Offset] << (i * 8);
        }

        return sign ? -(((~result) & ((1 << (8 * data.Length)) - 1)) + 1) : result;
    }

    public static int ToInteger(this byte[] data, int Offset, int Length, bool IsMsbFirst = true) =>
        IsMsbFirst ? data.ToIntegerMSB(Offset, Length) : data.ToIntegerLSB(Offset, Length);

    public static int ToIntegerMSB(this byte[] data, int Offset, int Length)
    {
        var result = 0;

        for (var i = Offset; i < data.Length && i - Offset < Length; i++) 
            result = unchecked((result << 8) + data[i]);

        return result;
    }

    public static uint ToUIntegerMSB(this byte[] data, int Offset, int Length)
    {
        var result = 0u;

        for (var i = Offset; i < data.Length && i - Offset < Length; i++) 
            result = unchecked((result << 8) + data[i]);

        return result;
    }

    public static int ToIntegerLSB(this byte[] data, int Offset, int Length)
    {
        var result = 0;

        for (var i = 0; i + Offset < data.Length && i < Length; i++) 
            result = unchecked(result + data[i + Offset] << (i << 3));

        return result;
    }

    public static uint ToUIntegerLSB(this byte[] data, int Offset, int Length)
    {
        var result = 0u;

        for (var i = 0; i + Offset < data.Length && i < Length; i++) 
            result = unchecked(result + (uint)(data[i + Offset] << (i << 3)));

        return result;
    }

    public static int FromComplementBinary(this int I, int BitCount = 16)
    {
        var sign = I >> (BitCount - 1) == 1;
        var mask = (1 << (BitCount - 1)) - 1;
        var x    = ((~((long)(I & mask) - 1)) & mask) * (sign ? -1 : 1);
        return (int)x;
    }

    public static string ToBitString(this int value) => Convert.ToString(value, 2);

    public static string ToBitString(this short value) => Convert.ToString(value, 2);
}