using System.Numerics;

namespace MathCore.Algorithms.Numbers;

// https://habr.com/ru/post/584662/
public static class Power
{
    /// <summary>Алгоритм - Старая аппроксимация (скорость х11, погрешность &lt;2%)</summary>
    /// <param name="x">Возводимое в степень число</param>
    /// <param name="e">Показатель степени в пределах -1 ... 1</param>
    /// <remarks>Quake III Arena 2005 - применялся для определения 1/sqrt(x)</remarks>
    private static double OldApproximatePower(double x, double e)
    {
        var i = BitConverter.DoubleToInt64Bits(x);
        //long magic = (long)((1L << 52) * ((1L << 10) - 1.0730088)); // 4606853616395542528L
        const long magic = 4606853616395542500L;
        i = (long)(magic + e * (i - magic));
        return BitConverter.Int64BitsToDouble(i);
    }

    //private static double OldApproximatePower(float x, float e)
    //{
    //    int i = BitConverter.FloatToInt32Bits(x);
    //    const int magic = 0x5f3759df;
    //    i = (int)(magic + e * (i - magic));
    //    return BitConverter.Int32BitsToFloat(i);
    //}

    /// <summary>Алгоритм - Бинарное возведение в степень (скорость x7.5)</summary>
    /// <param name="x">Возводимое в степень число</param>
    /// <param name="e">Показатель степени  0 ... 134217728</param>
    /// <remarks>
    /// Есть целая степень e, чтобы получить число x в этой степени
    /// нужно возвести это число во все степени 1, 2, 4, … 2n
    /// (в коде этому соответствует x *= x), каждый раз сдвигая биты
    /// e вправо (e >>= 1) пока оно не равно 0 и тогда, когда
    /// последний бит e не равен нулю ((e &amp; 1) != 0), домножать результат
    /// v на полученное x
    /// </remarks>
    public static double BinaryPower(double x, int e)
    {
        var v = 1d;
        while (e != 0)
        {
            if ((e & 1) != 0)
                v *= x;

            x *= x;
            e >>= 1;
        }

        return v;
    }

    /// <summary>Алгоритм - Делящая быстрая степень (скорость х3.5, погрешность &lt;13%)</summary>
    /// <param name="x">Возводимое в степень число</param>
    /// <param name="e">Показатель степени</param>
    /// <remarks>Разиение степени на две части: целую и дробную &lt;1</remarks>
    public static double FastPowerDividing(double x, double e)
    {
        if (x == 1d || e == 0d) return 1d;

        var x_abs = Math.Abs(e);
        var el = Math.Ceiling(x_abs);
        var base_part = OldApproximatePower(x, x_abs / el);
        var result = BinaryPower(base_part, (int)el);

        return e < 0d ? 1d / result : result;
    }

    /// <summary>Алгоритм - Дробная быстрая степень (скорость х4.4, погрешность &lt;0.7)</summary>
    /// <param name="x">Возводимое в степень число</param>
    /// <param name="e">Показатель степени</param>
    /// <remarks>
    /// el = (long)e<br/>
    /// x^e = x^el * x^(e-el)
    /// </remarks>
    public static double FastPowerFractional(double x, double e)
    {
        if (x == 1d || e == 0d) return 1d;

        var abs_e = Math.Abs(e);
        var int_e = (int)abs_e;
        var delta_e = abs_e - int_e;
        var result = OldApproximatePower(x, delta_e) * BinaryPower(x, int_e);

        return e < 0d ? 1d / result : result;
    }

    /// <summary>Алгоритм - Другая аппроксимация (скорость х9, погрешность &lt;1.5)</summary>
    /// <param name="x">Возводимое в степень число</param>
    /// <param name="e">Показатель степени в пределах -10 ... 10</param>
    /// <returns></returns>
    public static double AnotherApproxPower(double x, double e)
    {
        var tmp = (int)(BitConverter.DoubleToInt64Bits(x) >> 32);
        const int magic = 1072632447;
        var tmp2 = (int)(e * (tmp - magic) + magic);
        return BitConverter.Int64BitsToDouble(((long)tmp2) << 32);
    }
}