// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Расширения для чисел двойной точности</summary>
public static class DecimalExtensions
{
    /// <summary>Вычисление квадратного корня указанной точности последовательными приближениями</summary>
    /// <param name="x">Число, квадратный корень которого требуется вычислить</param>
    /// <param name="epsilon">Требуемая точность</param>
    /// <returns>Квадратный корень числа</returns>
    [DST]
    public static decimal Sqrt(this decimal x, decimal epsilon = 0.0M)
    {
        var current = (decimal)Math.Sqrt((double)x);
        decimal previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) / 2;
        }
        while (Math.Abs(previous - current) > epsilon);
        return current;
    }

    /// <summary>Является ли число целым?</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число целое</returns>
    [DST]
    public static bool IsInt(this decimal x) => decimal.Round(x) - x == 0;

    [DST]
    public static decimal Round(this decimal x) => decimal.Round(x);
    [DST]
    public static decimal Round(this decimal x, int n) => decimal.Round(x, n);

    [DST]
    public static decimal Truncate(this decimal x) => decimal.Truncate(x);
    [DST]
    public static decimal Floor(this decimal x) => decimal.Floor(x);
    [DST]
    public static decimal Ceiling(this decimal x) => decimal.Ceiling(x);

    /// <summary>Получить обратное число</summary>
    /// <param name="x">Инвертируемое число</param>
    /// <returns>Число, обратное к исходном</returns>
    [DST]
    public static decimal GetInverse(this decimal x) => 1 / x;

    //[Diagnostics.DST]
    //public static double GetAbsMod(this decimal x, decimal mod) { return x % mod + (x < 0 ? mod : 0); }

    [DST]
    public static decimal GetAbs(this decimal x) => x < 0 ? -x : x;

    //[DST]
    //public static double GetPower(this decimal x, int n)
    //{
    //    if(n > 1000 || n < -1000) return Math.Pow(x, n);
    //    if(n < 0) return (1 / x).GetPower(-n);
    //    var result = 1.0;
    //    for(var i = 0; i < n; i++)
    //        result *= x;
    //    return result;
    //}

    [DST]
    public static decimal Power(this decimal x, decimal y) => (decimal)Math.Pow((double)x, (double)y);

    //[Diagnostics.DST]
    //public static Complex GetPower(this decimal x, Complex z) { return x ^ z; }

    [DST]
    public static decimal In_dB(this decimal x) => 20 * (decimal)Math.Log10((double)x);
    //[Diagnostics.DST]
    //public static double In_dB_byPower(this decimal x) { return 10 * Math.Log10(x); }

    [DST]
    public static double From_dB(this decimal db) => Math.Pow(10, (double)(db / 20));
    [DST]
    public static double From_dB_byPower(this decimal db) => Math.Pow(10, (double)(db / 10));

    //[Diagnostics.DST]
    //public static double ToRad(this decimal deg) { return deg * Consts.Geometry.ToRad; }
    //[Diagnostics.DST]
    //public static double ToDeg(this decimal rad) { return rad * Consts.Geometry.ToDeg; }

    [DST]
    public static decimal Pow(this decimal x, int p)
    {
        switch (x)
        {
            case 0m: return 0m;
            case 1m: return 1m;
        }

        switch (p)
        {
            case -4: return 1m / (x * x * x * x);
            case -3: return 1m / (x * x * x);
            case -2: return 1m / (x * x);
            case -1: return 1m / x;
            case < 0: return 1m / x.Pow(-p);
            case 0: return 1m;
            case 1: return x;
            case 2: return x * x;
            case 3: return x * x * x;
            case 4: return x * x * x * x;
            default:
                var result = x;
                for (var i = 1; i < p; i++)
                    result *= x;
                return result;
        }
    }

    /// <summary>
    /// Подсчёт количества нулей между целой частью и первым ненулевым дробным разрядом. На пример для числа 123.00123 значение будет равно 2.
    /// </summary>
    public static int GetZerosCount(this decimal x)
    {
        switch (x)
        {
            case 0m: return 0;
            case 1m: return 0;
            case < 0:
                x = -x;
                break;
        }

        var fraction = x - Math.Truncate(x);
        if (fraction == x) return 0;

        var count = 0;
        while (fraction < 0.1m)
        {
            count++;
            fraction *= 10m;
        }
        return count;
    }

    public static decimal RoundAdaptive(this decimal x, int n = 1)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Число разрядов должно быть больше, либо равно 0");
        if (n == 0m) return x.Round();
        if (x < 0) return -(-x).RoundAdaptive(n);

        var fraction_digits = x.GetZerosCount();
        var b = 10m.Pow(fraction_digits);

        return (x * b).Round(n) / b;
    }
}