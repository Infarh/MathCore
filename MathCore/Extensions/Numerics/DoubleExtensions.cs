using System.Runtime.InteropServices;

using MathCore;
// ReSharper disable UnusedMember.Global

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Расширения для чисел двойной точности</summary>
public static class DoubleExtensions
{
    public static bool EqualWithAccuracy(this double x, double y, double Accuracy = 1.1102230246251565E-15) => x.Check(y, x - y, Accuracy);

    private static bool Check(this double x, double y, double delta, double Accuracy = 1.1102230246251565E-15) => 
        double.IsInfinity(x) || double.IsInfinity(y) 
            ? x == y 
            : x is not double.NaN && y is not double.NaN && Math.Abs(delta) < Math.Abs(Accuracy);

    /// <summary>Модуль числа</summary>
    /// <param name="x">Действительное вещественное число</param>
    /// <returns>Модуль числа</returns>
    [DST] public static double Abs(this double x) => x is double.NaN ? double.NaN : Math.Abs(x);
    [DST] public static float Abs(this float x) => x is float.NaN ? float.NaN : Math.Abs(x);
    [DST] public static decimal Abs(this decimal x) => Math.Abs(x);

    [DST] public static int Abs(this int x) => Math.Abs(x);
    [DST] public static long Abs(this long x) => Math.Abs(x);
    [DST] public static short Abs(this short x) => Math.Abs(x);
    [DST] public static sbyte Abs(this sbyte x) => Math.Abs(x);

    [DST] public static double Abs(this Complex x) => x.Abs;

    /// <summary>Число по модулю</summary>
    /// <param name="x">Исходное число</param>
    /// <param name="mod">Модуль</param>
    /// <returns>Число по модулю</returns>
    [DST] public static double AbsMod(this double x, double mod) => x % mod + (x < 0 ? mod : 0);


    [DST] public static double Sign(this double x) => x is double.NaN ? double.NaN : Math.Sign(x);
    [DST] public static double Round(this double x) => x is double.NaN ? double.NaN : Math.Round(x);
    [DST] public static double Floor(this double x) => x is double.NaN ? double.NaN : Math.Floor(x);
    [DST] public static double Truncate(this double x) => x is double.NaN ? double.NaN : Math.Truncate(x);
    [DST] public static double Ceiling(this double x) => x is double.NaN ? double.NaN : Math.Ceiling(x);

    [DST] public static float Sign(this float x) => x is float.NaN ? float.NaN : Math.Sign(x);
    [DST] public static float Round(this float x) => x is float.NaN ? float.NaN : (float)Math.Round(x);
    [DST] public static float Floor(this float x) => x is float.NaN ? float.NaN : (float)Math.Floor(x);
    [DST] public static float Truncate(this float x) => x is float.NaN ? float.NaN : (float)Math.Truncate(x);
    [DST] public static float Ceiling(this float x) => x is float.NaN ? float.NaN : (float)Math.Ceiling(x);

    //[DST]
    //public static double Pow(this double x, int p)
    //{
    //    switch (x)
    //    {
    //        case double.NaN: return double.NaN;
    //        case 0: return 0;
    //        case 1: return 1;
    //    }

    //    switch (p)
    //    {
    //        case -4:   return 1 / (x * x * x * x);
    //        case -3:   return 1 / (x * x * x);
    //        case -2:   return 1 / (x * x);
    //        case -1:   return 1 / x;
    //        case < 0: return 1 / x.Pow(-p);
    //        case 0:   return 1;
    //        case 1:   return x;
    //        case 2:   return x * x;
    //        case 3:   return x * x * x;
    //        case 4:   return x * x * x * x;
    //        default:
    //            var result = x;

    //            var y = x;
    //            var power = p;
    //            //while (power > 0)
    //            //{

    //            //}

    //            for (var i = 1; i < p; i++)
    //                result *= x;
    //            return result;
    //    }
    //}

    public static double Pow(this double x, int p)
    {
        switch (x)
        {
            case double.NaN: return double.NaN;
            case double.PositiveInfinity: return double.PositiveInfinity;
            case double.NegativeInfinity: return double.NegativeInfinity;
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
            default:
                var result = x;

                var power = p;
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
                
                return result;
        }
    }

    //[DST]
    //public static float Pow(this float x, int p)
    //{
    //    switch (x)
    //    {
    //        case float.NaN: return float.NaN;
    //        case 0: return 0;
    //        case 1: return 1;
    //    }

    //    switch (p)
    //    {
    //        case -4: return 1 / (x * x * x * x);
    //        case -3: return 1 / (x * x * x);
    //        case -2: return 1 / (x * x);
    //        case -1: return 1 / x;
    //        case < 0: return 1 / x.Pow(-p);
    //        case 0: return 1;
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

    public static float Pow(this float x, int p)
    {
        switch (x)
        {
            case float.NaN: return float.NaN;
            case float.PositiveInfinity: return float.PositiveInfinity;
            case float.NegativeInfinity: return float.NegativeInfinity;
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

    public static Complex Pow(this Complex x, int p)
    {
        if (x == Complex.Zero) return Complex.Zero;
        if (x == Complex.Real) return Complex.Real;

        switch (p)
        {
            case -4: return 1 / (x * x * x * x);
            case -3: return 1 / (x * x * x);
            case -2: return 1 / (x * x);
            case -1: return 1 / x;
            case < 0: return 1 / x.Pow(-p);
            case 0: return Complex.Real;
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

    [DST]
    public static Complex Pow(this Complex x, double p) =>
        x.Re is double.NaN || x.Im is double.NaN
            ? double.NaN
            : p switch
            {
                double.NaN => Complex.NaN,
                < 0        => 1 / x.Pow(-p),
                0          => 1,
                1          => x,
                2          => x.Pow2(),
                _          => x ^ p
            };

    [DST] public static double Pow(this double x, double p) => x is double.NaN || p is double.NaN ? double.NaN : Math.Pow(x, p);

    /// <summary>Возведение числа в комплексную степень</summary>
    /// <param name="x">Основание</param><param name="z">Комплексный показатель степень</param>
    /// <returns>Значение x^z, где x - действительное, z - комплексное</returns>
    [DST] public static Complex Pow(this double x, Complex z) => x ^ z;

    [DST] public static double Pow2(this double x) => x * x;
    [DST] public static float Pow2(this float x) => x * x;
    [DST] public static int Pow2(this int x) => x * x;
    [DST] public static uint Pow2(this uint x) => x * x;
    [DST] public static long Pow2(this long x) => x * x;
    [DST] public static ulong Pow2(this ulong x) => x * x;
    [DST] public static short Pow2(this short x) => (short)(x * x);
    [DST] public static ushort Pow2(this ushort x) => (ushort)(x * x);
    [DST] public static byte Pow2(this byte x) => (byte)(x * x);
    [DST] public static byte Pow2(this sbyte x) => (byte)(x * x);

    [DST]
    public static Complex Pow2(this Complex z)
    {
        if(z.IsNaN) return Complex.NaN;
        var (a, b) = z;
        return new(a * a - b * b, 2 * a * b);
    }

    [StructLayout(LayoutKind.Explicit)]
    private readonly ref struct DoubleToLongBytesConverter
    {
        [FieldOffset(0)]
        public readonly double Double;

        [FieldOffset(0)]
        public readonly long Long;

        public DoubleToLongBytesConverter(double x) => Double = x;
        public DoubleToLongBytesConverter(long x) => Long = x;
    }

    private const long __InvSqrtDoubleMagik = 0x5FE6EB50C7B537A9;

    public static double SqrtInvFast(this double x)
    {
        var i = new DoubleToLongBytesConverter(x).Long;
        i = __InvSqrtDoubleMagik - (i >> 1);
        var y = new DoubleToLongBytesConverter(i).Double;
        y *= 1.5 - 0.5 * x * y * y;

        return y;
    }

    public static double SqrtInvFast2(this double x)
    {
        var i = new DoubleToLongBytesConverter(x).Long;
        i = __InvSqrtDoubleMagik - (i >> 1);
        var y = new DoubleToLongBytesConverter(i).Double;
        y *= 1.5 - 0.5 * x * y * y;
        y *= 1.5 - 0.5 * x * y * y;

        return y;
    }

    public static double SqrtInvFast3(this double x)
    {
        var i = new DoubleToLongBytesConverter(x).Long;
        i = __InvSqrtDoubleMagik - (i >> 1);
        var y = new DoubleToLongBytesConverter(i).Double;

        var x05 = 0.5 * x;
        y *= 1.5 - x05 * y * y;
        y *= 1.5 - x05 * y * y;
        y *= 1.5 - x05 * y * y;

        return y;
    }

    public static double SqrtInvFast(this double x, int n)
    {
        var i = new DoubleToLongBytesConverter(x).Long;
        i = __InvSqrtDoubleMagik - (i >> 1);
        var y = new DoubleToLongBytesConverter(i).Double;

        var x05 = 0.5 * x;
        for (var j = 0; j < n; j++)
            y *= 1.5 - x05 * y * y;

        return y;
    }

    [StructLayout(LayoutKind.Explicit)]
    private readonly ref struct FloatToIntBytesConverter
    {
        [FieldOffset(0)]
        public readonly float Float;

        [FieldOffset(0)]
        public readonly int Int;

        public FloatToIntBytesConverter(float x) => Float = x;
        public FloatToIntBytesConverter(int x) => Int = x;
    }

    private const int __InvSqrtFloatMagik = 0x5f3759df;

    public static double SqrtInvFast(this float x)
    {
        var i = new FloatToIntBytesConverter(x).Int;
        i = __InvSqrtFloatMagik - (i >> 1);
        var y = new FloatToIntBytesConverter(i).Float;
        y *= 1.5f - 0.5f * x * y * y;

        return y;
    }

    public static double SqrtInvFast2(this float x)
    {
        var i = new FloatToIntBytesConverter(x).Int;
        i = __InvSqrtFloatMagik - (i >> 1);
        var y = new FloatToIntBytesConverter(i).Float;
        y *= 1.5f - 0.5f * x * y * y;
        y *= 1.5f - 0.5f * x * y * y;

        return y;
    }

    public static double SqrtInvFast3(this float x)
    {
        var i = new FloatToIntBytesConverter(x).Int;
        i = __InvSqrtFloatMagik - (i >> 1);
        var y = new FloatToIntBytesConverter(i).Float;
        var x05 = 0.5f * x;
        y *= 1.5f - x05 * y * y;
        y *= 1.5f - x05 * y * y;
        y *= 1.5f - x05 * y * y;

        return y;
    }

    public static double SqrtInvFast(this float x, int n)
    {
        var i = new FloatToIntBytesConverter(x).Int;
        i = __InvSqrtFloatMagik - (i >> 1);
        var y = new FloatToIntBytesConverter(i).Float;

        var x05 = 0.5f * x;
        for (var j = 0; j < n; j++)
            y *= 1.5f - x05 * y * y;

        return y;
    }

    private const int __SqrtFloatFast = 0x1fbd1df5;

    public static double SqrtFast(this float x)
    {
        var i = new FloatToIntBytesConverter(x).Int;
        i = __SqrtFloatFast + (i >> 1);
        var y = new FloatToIntBytesConverter(i).Float;

        y = 0.5f * (y + x / y);

        return y;
    }

    // https://habr.com/ru/companies/infopulse/articles/336110/
    public static double PowFast(this float x, double p)
    {
        const int l = 1 << 23;
        const int b = 127;
        const double sgm = 0.0450465;
        const int k = (int)(l * (b - sgm));

        var i = new FloatToIntBytesConverter(x).Int;
        i = (int)(k + p * (i - k));
        var y = new FloatToIntBytesConverter(i).Float;

        return y;
    }

    /// <summary>Квадратный корень</summary>
    /// <param name="x">Число из которого извлекается квадратный корень</param>
    /// <returns>Квадратный корень числа</returns>
    [DST] public static double Sqrt(this double x) => x is double.NaN ? double.NaN : Math.Sqrt(x);

    /// <summary>Квадратный корень</summary>
    /// <param name="x">Число из которого извлекается квадратный корень</param>
    /// <returns>Квадратный корень числа</returns>
    [DST] public static float Sqrt(this float x) => x is float.NaN ? float.NaN : (float)Math.Sqrt(x);

    /// <summary>Является ли число целым?</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число целое</returns>
    [DST] public static bool IsInt(this double x) => x is not double.NaN && (int)x - x == 0;

    /// <summary>Является ли число целым?</summary>
    /// <param name="x">Проверяемое число</param>
    /// <returns>Истина, если число целое</returns>
    [DST] public static bool IsInt(this float x) => x is not float.NaN && (int)x - x == 0;

    /// <summary>Является ли значение "не числом"?</summary>
    /// <param name="x">Проверяемое значение</param>
    /// <returns>Истина, если значение - не число</returns>
    [DST] public static bool IsNaN(this double x) => x is double.NaN;

    /// <summary>Является ли значение "не числом"?</summary>
    /// <param name="x">Проверяемое значение</param>
    /// <returns>Истина, если значение - не число</returns>
    [DST] public static bool IsNaN(this float x) => x is float.NaN;

    /// <summary>Округление числа до указанного количества знаков после запятой </summary>
    /// <param name="x">Округляемое число</param>
    /// <param name="n">Количество знаков после запятой при n >= 0 и до запятой при n меньше 0</param>
    /// <returns>Число, округлённое до указанной точности</returns>
    [DST]
    public static double Round(this double x, int n)
    {
        if (x is double.NaN) return x;
        if (n == 0) return Math.Round(x);
        if (n >= 0) return Math.Round(x, n);

        var nn = -n;
        var b  = Math.Pow(10, nn);
        return Math.Round(x / b) * b;
    }

    /// <summary>Округление числа до указанного количества знаков после запятой </summary>
    /// <param name="x">Округляемое число</param>
    /// <param name="n">Количество знаков после запятой при n >= 0 и до запятой при n меньше 0</param>
    /// <returns>Число, округлённое до указанной точности</returns>
    [DST]
    public static float Round(this float x, int n)
    {
        if (x is float.NaN) return x;
        if (n == 0) return (float)Math.Round(x);
        if (n >= 0) return (float)Math.Round(x, n);

        var nn = -n;
        var b  = Math.Pow(10, nn);
        return (float)(Math.Round(x / b) * b);
    }

    //[DST] public static Vector3D RoundAdaptive(this Vector3D r, int n = 1) => new Vector3D(r.X.RoundAdaptive(n), r.Y.RoundAdaptive(n), r.Z.RoundAdaptive(n));


    /// <summary>Адаптивное округление</summary>
    /// <param name="x">Округляемая величина</param>
    /// <param name="n">Количество значащих разрядов</param>
    /// <returns>Число с указанным количеством значащих разрядов</returns>
    [DST]
    public static double RoundAdaptive(this double x, int n = 1)
    {
        switch (x)
        {
            case double.NaN or double.PositiveInfinity or double.NegativeInfinity: return x;
            case < 0: return -(-x).RoundAdaptive(n);
        }

        switch (n)
        {
            case < 0: throw new ArgumentOutOfRangeException(nameof(n), n, "Число разрядов должно быть больше, либо равно 0");
            case 0: return x.Round();
        }

        var integer = x.Truncate();
        if (integer == x) return x;
        var fraction = x - integer;

        if (fraction == 0) return x;

        var fraction_digits = (int)Math.Log10(fraction);
        var b = 10d.Pow(fraction_digits);

        return (x / b).Round(n) * b;
    }

    /// <summary>Адаптивное округление</summary>
    /// <param name="x">Округляемая величина</param>
    /// <param name="n">Количество значащих разрядов</param>
    /// <returns>Число с указанным количеством значащих разрядов</returns>
    [DST]
    public static float RoundAdaptive(this float x, int n = 1)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Число разрядов должно быть больше, либо равно 0");
        if (n == 0) return x.Round();
        if (x is float.NaN or float.PositiveInfinity or float.NegativeInfinity) return x;
        if (x < 0) return -(-x).RoundAdaptive(n);

        var integer = x.Truncate();
        var fraction = x - integer;

        var fraction_digits = (int)Math.Log10(fraction);
        var b = 10f.Pow(fraction_digits);

        return (x / b).Round(n) * b;
    }

    /// <summary>Получить обратное число</summary>
    /// <param name="x">Инвертируемое число</param>
    /// <returns>Число, обратное к исходном</returns>
    [DST] public static double GetInverse(this double x) => 1 / x;

    /// <summary>Получить обратное число</summary>
    /// <param name="x">Инвертируемое число</param>
    /// <returns>Число, обратное к исходном</returns>
    [DST] public static float GetInverse(this float x) => 1 / x;

    /// <summary>Преобразование в децибелы по амплитуде</summary>
    /// <param name="x">Амплитудное значение 20*lg(x)</param>
    /// <returns>Значение в децибелах</returns>
    [DST] public static double In_dB(this double x) => x is double.NaN ? double.NaN : 20 * Math.Log10(x);

    /// <summary>Преобразование в децибелы по мощности</summary>
    /// <param name="x">Значение мощности 10*lg(x)</param>
    /// <returns>Значение в децибелах</returns>
    [DST] public static double In_dB_byPower(this double x) => x is double.NaN ? double.NaN : 10 * Math.Log10(x);

    /// <summary>Преобразование из децибелов в разы по значению (амплитуде)</summary>
    /// <param name="db">Значение в децибелах 10^(x/20)</param>
    /// <returns>Значение в разах по амплитуде</returns>
    [DST] public static double From_dB(this double db) => db is double.NaN ? double.NaN : Math.Pow(10, db / 20);

    /// <summary>Преобразование из децибелов в разы по мощности</summary>
    /// <param name="db">Значение в децибелах 10^(x/10)</param>
    /// <returns>Значение в разах по мощности</returns>
    [DST] public static double From_dB_byPower(this double db) => db is double.NaN ? double.NaN : Math.Pow(10, db / 10);

    /// <summary>Преобразование значения в радианы</summary>
    /// <param name="deg">Значение в градусах</param>
    /// <returns>Значение в радианах</returns>
    [DST] public static double ToRad(this double deg) => deg * Consts.Geometry.ToRad;

    /// <summary>Преобразование значения в градусы</summary>
    /// <param name="rad">Значение в радианах</param>
    /// <returns>Значение в градусах</returns>
    [DST] public static double ToDeg(this double rad) => rad * Consts.Geometry.ToDeg;

    /// <summary>Преобразование значения в радианы</summary>
    /// <param name="deg">Значение в градусах</param>
    /// <returns>Значение в радианах</returns>
    [DST] public static float ToRad(this float deg) => (float) (deg * Consts.Geometry.ToRad);

    /// <summary>Преобразование значения в градусы</summary>
    /// <param name="rad">Значение в радианах</param>
    /// <returns>Значение в градусах</returns>
    [DST] public static float ToDeg(this float rad) => (float) (rad * Consts.Geometry.ToDeg);

    public static long ToIntBits(this double x) => BitConverter.DoubleToInt64Bits(x);
    public static double ToDoubleBits(this long x) => BitConverter.Int64BitsToDouble(x);
}