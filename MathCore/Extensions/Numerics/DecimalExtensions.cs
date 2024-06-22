// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Расширения для чисел двойной точности</summary>
public static class DecimalExtensions
{
    // ReSharper disable InconsistentNaming
    private const decimal pi = 3.14159265358979323846264338327950288419716939937510M;
    //private const decimal eps = 0.0000000000000000001M;
    private const decimal pi2 = 6.28318530717958647692528676655900576839433879875021M;
    private const decimal e = 2.7182818284590452353602874713526624977572470936999595749M;
    private const decimal pi05 = 1.570796326794896619231321691639751442098584699687552910487M;
    private const decimal pi025 = 0.785398163397448309615660845819875721049292349843776455243M;
    private const decimal e_inv = 0.3678794411714423215955237701614608674458111310317678M;
    private const decimal log10_inv = 0.434294481903251827651128918916605082294397005803666566114M;
    // ReSharper restore InconsistentNaming

    /// <summary>Вычисление квадратного корня указанной точности последовательными приближениями</summary>
    /// <param name="x">Число, квадратный корень которого требуется вычислить</param>
    /// <param name="epsilon">Требуемая точность</param>
    /// <returns>Квадратный корень числа</returns>
    [DST]
    public static decimal Sqrt(this decimal x, decimal epsilon = 0)
    {
        switch (x)
        {
            case < 0: throw new ArgumentOutOfRangeException(nameof(x), x, "Мнимый результат");
            case 0: return 0;
            case 1: return 1;
            case 4: return 2;
            case 9: return 3;
            case 16: return 4;
            case 25: return 5;
        }

        var current = (decimal)Math.Sqrt((double)x);
        decimal previous;
        do
        {
            previous = current;
            if (previous == 0) return 0;
            current = (previous + x / previous)  * 0.5m;
        }
        while (Math.Abs(previous - current) > epsilon);
        return current;
    }

    /// <summary>Является ли число целым?</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число целое</returns>
    [DST]
    public static bool IsInt(this decimal x, decimal epsilon = 0) => Math.Abs(decimal.Truncate(x) - x) <= epsilon;

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

    [Copyright("https://github.com/raminrahimzada/CSharp-Helper-Classes/blob/master/Math/DecimalMath/DecimalMath.cs#L207")]
    public static decimal Exp(this decimal x)
    {
        var count = 0;

        if (x > 1)
        {
            count = decimal.ToInt32(decimal.Truncate(x));
            x -= decimal.Truncate(x);
        }

        if (x < 0)
        {
            count = decimal.ToInt32(decimal.Truncate(x) - 1);
            x = 1 + (x - decimal.Truncate(x));
        }

        var iteration = 1;
        var result = 1m;
        var factorial = 1m;
        decimal cached_result;
        do
        {
            cached_result = result;
            factorial *= x / iteration++;
            result += factorial;
        } while (cached_result != result);

        if (count == 0)
            return result;

        return result * e.Pow(count);
    }

    [Copyright("https://github.com/raminrahimzada/CSharp-Helper-Classes/blob/master/Math/DecimalMath/DecimalMath.cs#L207")]
    public static decimal Log(this decimal x)
    {
        if (x <= 0) throw new ArgumentException("x must be greater than zero");

        var count = 0;
        while (x >= 1)
        {
            x *= e_inv;
            count++;
        }

        while (x <= e_inv)
        {
            x *= e;
            count--;
        }

        x--;
        
        if (x == 0) return count;
        
        var result = 0m;
        var iteration = 0;
        var y = 1m;
        var cache_result = result - 1;
        
        const int max_iteration = 100;
        while (cache_result != result && iteration < max_iteration)
        {
            iteration++;
            cache_result = result;
            y *= -x;
            result += y / iteration;
        }

        return count - result;
    }

    public static decimal Log10(this decimal x) => Log(x) * log10_inv;

    private static void TruncateToPeriodicInterval(ref decimal x)
    {
        while (x >= pi2)
        {
            var divide = Math.Abs(decimal.ToInt32(x / pi2));
            x -= divide * pi2;
        }

        while (x <= -pi2)
        {
            var divide = Math.Abs(decimal.ToInt32(x / pi2));
            x += divide * pi2;
        }
    }
    public static decimal Sin(this decimal x) => CalculateSinFromCos(x, Cos(x));

    public static decimal Cos(this decimal x)
    {
        //truncating to  [-2*PI;2*PI]
        TruncateToPeriodicInterval(ref x);

        // now x in (-2pi,2pi)
        switch (x)
        {
            case >= +pi and <= +pi2: return -Cos(x - pi);
            case >= -pi2 and <= -pi: return -Cos(x + pi);
        }

        x *= x;
        //y=1-x/2!+x^2/4!-x^3/6!...

        var xx = -x * 0.5m;
        var y = 1 + xx;
        var cached_y = y - 1;//init cache  with different value

        const int max_iteration = 100;
        for (var i = 1; cached_y != y && i < max_iteration; i++)
        {
            cached_y = y;
            decimal factor = i * ((i << 1) + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
            factor = -0.5m / factor;
            xx *= x * factor;
            y += xx;
        }

        return y;
    }

    private static bool IsSignOfSinePositive(decimal x)
    {
        //truncating to  [-2*PI;2*PI]
        TruncateToPeriodicInterval(ref x);

        //now x in [-2*PI;2*PI]
        return x switch
        {
            >= -pi2 and <= -pi => true,
            >= -pi and <= 0 => false,
            >= 0 and <= pi => true,
            >= pi and <= pi2 => false,
            _ => throw new ArgumentException(nameof(x))
        };

        //will not be reached
    }

    private static decimal CalculateSinFromCos(decimal x, decimal cos) => IsSignOfSinePositive(x) 
        ? +Sqrt(1 - cos * cos) 
        : -Sqrt(1 - cos * cos);

    public static decimal Tan(this decimal x)
    {
        var cos = Cos(x);
        if (cos is 0) throw new ArgumentException(nameof(x));

        //calculate sin using cos
        var sin = CalculateSinFromCos(x, cos);
        return sin / cos;
    }

    public static decimal Sinh(this decimal x)
    {
        var exp = Exp(x);
        var exp_inv = 1 / exp;
        return (exp - exp_inv) * 0.5m;
    }

    public static decimal Cosh(this decimal x)
    {
        var exp = Exp(x);
        var exp_inv = 1 / exp;
        return (exp + exp_inv) * 0.5m;
    }

    public static decimal Tanh(this decimal x)
    {
        var exp = Exp(x);
        var exp_inv = 1 / exp;
        return (exp - exp_inv) / (exp + exp_inv);
    }

    public static decimal Asin(this decimal x)
    {
        if (x is > 1 or < -1) throw new ArgumentException("x must be in [-1,1]");

        //known values
        if (x == 0) return 0;
        if (x == 1) return pi05;

        //asin function is odd function
        if (x < 0) return -Asin(-x);

        //my optimize trick here

        // used a math formula to speed up :
        // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
        // if x>=0 is true

        var new_x = 1 - 2 * x * x;

        //for calculating new value near to zero than current
        //because we gain more speed with values near to zero
        if (x.Abs() > new_x.Abs())
        {
            var t = Asin(new_x);
            return 0.5m * (pi05 - t);
        }

        var y = 0m;
        var result = x;
        decimal cached_result;
        var i = 1;
        y += result;
        var xx = x * x;
        do
        {
            cached_result = result;
            result *= xx * (1 - 0.5m / i);
            y += result / ((i << 1) + 1);
            i++;
        } while (cached_result != result);

        return y;
    }

    public static decimal ATan(this decimal x) => x switch
    {
        0 => 0,
        1 => pi025,
        _ => Asin(x / Sqrt(1 + x * x))
    };

    public static decimal Acos(this decimal x) => x switch
    {
        0 => pi05,
        1 => 0,
        < 0 => pi - Acos(-x),
        _ => pi05 - Asin(x)
    };

    public static decimal Atan2(this (decimal x, decimal y) point) => point switch
    {
        (> 0,    _) => ATan(point.y / point.x),
        (< 0, >= 0) => ATan(point.y / point.x) + pi,
        (< 0,  < 0) => ATan(point.y / point.x) - pi,
        (  _,  > 0) => +pi05,
        (  _,  < 0) => -pi05,
        _ => throw new ArgumentException("invalid atan2 arguments")
    };

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
    public static decimal Pow(this decimal x, decimal y) => (decimal)Math.Pow((double)x, (double)y);

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

    //[DST]
    //public static decimal Pow(this decimal x, int p)
    //{
    //    switch (x)
    //    {
    //        case 0m: return 0m;
    //        case 1m: return 1m;
    //    }

    //    switch (p)
    //    {
    //        case -4: return 1m / (x * x * x * x);
    //        case -3: return 1m / (x * x * x);
    //        case -2: return 1m / (x * x);
    //        case -1: return 1m / x;
    //        case < 0: return 1m / x.Pow(-p);
    //        case 0: return 1m;
    //        case 1: return x;
    //        case 2: return x * x;
    //        case 3: return x * x * x;
    //        case 4: return x * x * x * x;
    //        default:
    //            var result = x;
    //            for (var i = 1; i < p; i++)
    //                result *= x;
    //            return result;
    //    }
    //}

    public static decimal Pow(this decimal x, int p)
    {
        switch (x)
        {
            case 0: return 0;
            case +1: return 1;
            case -1: return p % 2 == 0 ? 1 : -1;
        }

        switch (p)
        {
            case -4: return 1 / (x * x * x * x);
            case -3: return 1 / (x * x * x);
            case -2: return 1 / (x * x);
            case -1: return 1 / x;
            case < 0: return 1 / x.Pow(-p);
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

    /// <summary>
    /// Подсчёт количества нулей между целой частью и первым ненулевым дробным разрядом. На пример для числа 123.00123 значение будет равно 2.
    /// </summary>
    public static int GetZerosCount(this decimal x)
    {
        switch (x)
        {
            case 0: return 0;
            case 1: return 0;
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
        if (x < 0) return -(-x).RoundAdaptive(n);
        if (x.Truncate() == x) return x;
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Число разрядов должно быть больше, либо равно 0");
        if (n == 0) return x.Round();

        var fraction_digits = x.GetZerosCount();
        var b = 10m.Pow(fraction_digits);

        return (x * b).Round(n) / b;
    }
}