using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MathCore.Exceptions;
using MathCore.Extensions.Expressions;

using static System.Math;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.Statistic.RandomNumbers;

// ReSharper disable once StringLiteralTypo
[Copyright("Александр Самарин - Генераторы непрерывно распределенных случайных величин", url = "http://habrahabr.ru/post/263993/")]
/// <summary>Генератор непрерывно распределённых случайных величин с различными законами распределения</summary>
public class PolyformRandomGenerator
{
    //todo: http://habrahabr.ru/post/265321/
    private readonly Random _RND = new();
    private ulong _LastRND;
    /// <summary>Максимальное значение базового генератора</summary>
    public const ulong RandMax = ulong.MaxValue;
    /// <summary>Инициализирует новый экземпляр генератора</summary>
    public PolyformRandomGenerator() => _LastRND = (ulong)_RND.Next();

    /// <summary>Базовый синхронизированный генератор случайных 32-разрядных значений</summary>
    /// <returns>Случайное значение</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ulong BasicRandGenerator() => (_LastRND << 32) & (_LastRND = (ulong)_RND.Next());

    #region Uniform

    /// <summary>Равномерная функция распределения плотности вероятности</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="a">Нижняя граница</param>
    /// <param name="b">Верхняя граница</param>
    /// <returns>Значение плотности</returns>
    public static double UniformDistribution(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

    /// <summary>Возвращает функцию плотности равномерного распределения</summary>
    /// <param name="a">Нижняя граница</param>
    /// <param name="b">Верхняя граница</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetUniformDistribution(double a, double b)
        => x => x >= a && x <= b ? 1 / (b - a) : 0;
    /// <summary>Возвращает выражение плотности равномерного распределения</summary>
    /// <param name="a">Нижняя граница</param>
    /// <param name="b">Верхняя граница</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetUniformDistributionExpression(double a, double b)
        => x => x >= a && x <= b ? 1 / (b - a) : 0;

    /// <summary>Формирует случайную величину с равномерным распределением в [a;b]</summary>
    /// <param name="a">Нижняя граница</param>
    /// <param name="b">Верхняя граница</param>
    /// <returns>Случайная величина</returns>
    public double Uniform(double a, double b) => a + (double)BasicRandGenerator() / RandMax * (b - a);

    #endregion


    #region Normal

    /// <summary>Функция плотности нормального распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="m">Математическое ожидание</param>
    /// <param name="s">Среднеквадратичное отклонение</param>
    /// <returns>Значение плотности</returns>
    public static double NormalDistribution(double x, double m, double s) => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);
    /// <summary>Возвращает функцию плотности нормального распределения</summary>
    /// <param name="m">Математическое ожидание</param>
    /// <param name="s">Среднеквадратичное отклонение</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetNormalDistribution(double m, double s) => x => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);

    /// <summary>Возвращает выражение плотности нормального распределения</summary>
    /// <param name="m">Математическое ожидание</param>
    /// <param name="s">Среднеквадратичное отклонение</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetNormalDistributionExpression(double m, double s)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var S = s.ToExpression();
        var expr_2 = 2.ToExpression();
        var sqrt2PI = expr_2.Mult(Consts.pi.ToExpression()).Power(0.5);
        var E = ((Func<double, double>)Exp).GetCallExpression(X.Subtract(M).Power(2).Divide(expr_2.Mult(S.Power(2))).Negate());
        var body = 1.ToExpression().Divide(S.Mult(sqrt2PI)).Mult(E);
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    private static double[] __NormalStairWidth = null!;
    private static double[] __NormalStairHeight = null!;
    private static bool __NormalInitialized;
    private static readonly object __NormalInitializationSyncRoot = new();
    private const double __Normal_x1 = 3.6541528853610088;

    /// <summary>area under rectangle</summary>
    private const double __Normal_A = 4.92867323399e-3;

    private static bool InitializeNormal()
    {
        if (__NormalInitialized) return true;
        lock (__NormalInitializationSyncRoot)
        {
            if (__NormalInitialized) return true;
            __NormalStairWidth = new double[257];
            __NormalStairHeight = new double[256];
            // coordinates of the implicit rectangle in base layer
            __NormalStairHeight[0] = Exp(-.5 * __Normal_x1 * __Normal_x1);
            __NormalStairWidth[0] = __Normal_A / __NormalStairHeight[0];
            // implicit value for the top layer
            __NormalStairWidth[256] = 0;
            for (var i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                __NormalStairWidth[i] = Sqrt(-2 * Log(__NormalStairHeight[i - 1]));
                __NormalStairHeight[i] = __NormalStairHeight[i - 1] + __Normal_A / __NormalStairWidth[i];
            }
            return __NormalInitialized = true;
        }
    }

    private double _NormalZigguratZ;

    [MethodImpl(MethodImplOptions.Synchronized)]
    private double NormalZiggurat()
    {
        var iterator = 0;
        do
        {
            var B = BasicRandGenerator();
            var stair_id = (int)(B & 0xff);
            var x = Uniform(0, __NormalStairWidth[stair_id]); // get horizontal coordinate
            if (x < __NormalStairWidth[stair_id + 1])
                return (long)B > 0 ? x : -x;
            if (stair_id == 0) // handle the base layer
            {
                _NormalZigguratZ = -1;
                double y;
                if (_NormalZigguratZ >= 0)
                // we don't have to generate another exponential variable as we already have one
                {
                    y = Exponential(1);
                    _NormalZigguratZ = y - 0.5 * _NormalZigguratZ * _NormalZigguratZ;
                }
                if (_NormalZigguratZ < 0) // if previous generation wasn't successful
                    do
                    {
                        x = Exponential(__Normal_x1);
                        y = Exponential(1);
                        _NormalZigguratZ = y - 0.5 * x * x;
                        // we storage this value as after acceptance it becomes exponentially distributed
                    } while (_NormalZigguratZ <= 0);

                x += __Normal_x1;
                return (long)B > 0 ? x : -x;
            }
            // handle the wedges of other stairs
            if (Uniform(__NormalStairHeight[stair_id - 1], __NormalStairHeight[stair_id]) < Exp(-.5 * x * x))
                return (long)B > 0 ? x : -x;
        } while (++iterator <= 1e9); // one billion should be enough
        throw new CalculationsException();
    }

    /// <summary>Формирует случайную величину с нормальным распределением</summary>
    /// <param name="mu">Математическое ожидание</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <returns>Случайная величина</returns>
    public double Normal(double mu, double sigma) =>
        __NormalInitialized || InitializeNormal()
            ? mu + NormalZiggurat() * sigma
            : throw new CalculationsException();

    #endregion

    #region Exponential

    /// <summary>Функция плотности экспоненциального распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="l">Интенсивность</param>
    /// <param name="_">Неиспользуемый параметр</param>
    /// <returns>Значение плотности</returns>
    public static double ExponentialDistribution(double x, double l, double _) => l * Exp(-l * x);
    /// <summary>Возвращает функцию плотности экспоненциального распределения</summary>
    /// <param name="l">Интенсивность</param>
    /// <param name="_">Неиспользуемый параметр</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetExponentialDistribution(double l, double _) => x => l * Exp(-l * x);

    /// <summary>Возвращает выражение плотности экспоненциального распределения</summary>
    /// <param name="l">Интенсивность</param>
    /// <param name="_">Неиспользуемый параметр</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetExponentialDistributionExpression(double l, double _)
    {
        var x = Expression.Parameter(typeof(double), "x");
        var body = l.ToExpression().Mult(MathExpression.Exp(l.ToExpression().Mult(x).Negate()));
        return Expression.Lambda<Func<double, double>>(body, x);
    }

    private static double[] __ExponentialStairWidth = null!;
    private static double[] __ExponentialStairHeight = null!;
    private static bool __ExponentialInitialized;
    private static readonly object __ExponentialInitializationSyncRoot = new();
    private const double __X1 = 7.69711747013104972;

    /// <summary>area under rectangle</summary>
    private const double __A = 3.9496598225815571993e-3;

    private static bool InitializeExponential()
    {
        if (__ExponentialInitialized) return true;
        lock (__ExponentialInitializationSyncRoot)
        {
            if (__ExponentialInitialized) return true;
            __ExponentialStairWidth = new double[257];
            __ExponentialStairHeight = new double[256];
            // coordinates of the implicit rectangle in base layer
            __ExponentialStairHeight[0] = Exp(-__X1);
            __ExponentialStairWidth[0] = __A / __ExponentialStairHeight[0];
            // implicit value for the top layer
            __ExponentialStairWidth[256] = 0;
            for (var i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                __ExponentialStairWidth[i] = -Log(__ExponentialStairHeight[i - 1]);
                __ExponentialStairHeight[i] = __ExponentialStairHeight[i - 1] + __A / __ExponentialStairWidth[i];
            }
            return __ExponentialInitialized = true;
        }
    }

    private double ExpZiggurat()
    {
        var iter = 0;
        do
        {
            var stair_id = (int)(BasicRandGenerator() & 255);
            var x = Uniform(0, __ExponentialStairWidth[stair_id]); // get horizontal coordinate
            if (x < __ExponentialStairWidth[stair_id + 1])                 // if we are under the upper stair - accept
                return x;
            if (stair_id == 0) // if we catch the tail
                return __X1 + ExpZiggurat();
            if (Uniform(__ExponentialStairHeight[stair_id - 1], __ExponentialStairHeight[stair_id]) < Exp(-x))
                // if we are under the curve - accept
                return x;
            // rejection - go back
        } while (++iter <= 1e9); // one billion should be enough to be sure there is a bug
        throw new CalculationsException();
    }

    /// <summary>Формирует случайную величину с экспоненциальным распределением</summary>
    /// <param name="rate">Интенсивность</param>
    /// <returns>Случайная величина</returns>
    public double Exponential(double rate) => ExpZiggurat() / rate;

    #endregion

    #region Gamma

    /// <summary>Функция плотности гамма-распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="k">Параметр формы</param>
    /// <param name="th">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double GammaDistribution(double x, double k, double th)
        => Pow(x, k - 1) * Exp(-x / th) / (SpecialFunctions.Gamma.G(k) * Pow(th, k));
    /// <summary>Возвращает функцию плотности гамма-распределения</summary>
    /// <param name="k">Параметр формы</param>
    /// <param name="th">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetGammaDistribution(double k, double th) => x => Pow(x, k - 1) * Exp(-x / th) / (SpecialFunctions.Gamma.G(k) * Pow(th, k));

    /// <summary>Возвращает выражение плотности гамма-распределения</summary>
    /// <param name="k">Параметр формы</param>
    /// <param name="th">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetGammaDistributionExpression(double k, double th)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var K = k.ToExpression();
        var TH = th.ToExpression();
        var body = X.Power(K.Subtract(1)).Mult(MathExpression.Exp(X.Divide(TH)).Negate())
           .Divide(MathExpression.F(SpecialFunctions.Gamma.G, K).Mult(TH.Power(K)));
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Случайная величина с гамма-распределением для целочисленного параметра формы (сумма экспоненциальных)</summary>
    /// <param name="k">Параметр формы (целый)</param>
    /// <returns>Случайная величина</returns>
    public double GA1(int k)
    {
        double x = 0;
        for (var i = 0; i < k; ++i)
            x += Exponential(1);
        return x;
    }

    /// <summary>Случайная величина с гамма-распределением для полуцелого параметра формы</summary>
    /// <param name="k">Параметр формы</param>
    /// <returns>Случайная величина</returns>
    public double GA2(double k)
    {
        var x = Normal(0, 1);
        x *= 0.5 * x;
        for (var i = 1; i < k; ++i)
            x += Exponential(1);
        return x;
    }

    /// <summary>Случайная величина с гамма-распределением для параметра формы меньше единицы</summary>
    /// <param name="k">Параметр формы (k &lt; 1)</param>
    /// <returns>Случайная величина</returns>
    public double GS(double k)
    {
        // Assume that k < 1
        var iter = 0;
        do
        {
            // M_E is base of natural logarithm
            var u = Uniform(0, 1 + k / Consts.e);
            var w = Exponential(1);
            double x;
            if (u <= 1)
            {
                x = Pow(u, 1.0 / k);
                if (x <= w)
                    return x;
            }
            else
            {
                x = -Log((1 - u) / k + 1.0 / Consts.e);
                if ((1 - k) * Log(x) <= w)
                    return x;
            }
        } while (++iter < 1e9); // excessive maximum number of rejections
        throw new CalculationsException();
    }

    /// <summary>Случайная величина с гамма-распределением для параметра формы в диапазоне (1;3)</summary>
    /// <param name="k">Параметр формы (1 &lt; k &lt; 3)</param>
    /// <returns>Случайная величина</returns>
    public double GF(double k)
    {
        // Assume that 1 < k < 3
        double e1, e2;
        do
        {
            e1 = Exponential(1);
            e2 = Exponential(1);
        } while (e2 < (k - 1) * (e1 - Log(e1) - 1));
        return k * e1;
    }

    /// <summary>Случайная величина с гамма-распределением для параметра формы больше трёх</summary>
    /// <param name="k">Параметр формы (k &gt; 3)</param>
    /// <returns>Случайная величина</returns>
    private double GO(double k)
    {
        // Assume that k > 3
        var m = k - 1;
        var s2 = Sqrt(8 * k / 3) + k;
        var sqrt_s2 = Sqrt(s2);
        var d = Consts.sqrt_2 * Consts.sqrt_3 * s2;
        var b = d + m;
        var w = s2 / (m - 1);
        var v = (s2 + s2) / (m * Sqrt(k));
        var c = b + Log(sqrt_s2 * d / b) - m - m - 3.7203285;

        var iter = 0;
        do
        {
            var u = Uniform(0, 1);
            double x;
            if (u <= 0.0095722652)
            {
                var e1 = Exponential(1);
                var e2 = Exponential(1);
                x = b * (1 + e1 / d);
                if (m * (x / b - Log(x / m)) + c <= e2)
                    return x;
            }
            else
            {
                double n;
                do
                {
                    n = Normal(0, 1);
                    x = sqrt_s2 * n + m; // ~ Normal(m, s)
                } while (x < 0 || x > b);
                u = Uniform(0, 1);
                var s = 0.5 * n * n;
                if (n > 0)
                {
                    if (u < 1 - w * s)
                        return x;
                }
                else if (u < 1 + s * (v * n - w))
                    return x;
                if (Log(u) < m * Log(x / m) + m - x + s)
                    return x;
            }
        } while (++iter < 1e9);
        throw new CalculationsException();
    }

    #endregion

    #region Cauchy

    /// <summary>Функция плотности распределения Коши</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="x0">Параметр сдвига</param>
    /// <param name="g">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double CauchyDistribution(double x, double x0, double g)
        => g / (Consts.pi * (g * g + (x - x0).Pow(2)));
    /// <summary>Возвращает функцию плотности распределения Коши</summary>
    /// <param name="x0">Параметр сдвига</param>
    /// <param name="g">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetCauchyDistribution(double x0, double g) =>
        x => g / (Consts.pi * (g * g + (x - x0).Pow(2)));

    /// <summary>Возвращает выражение плотности распределения Коши</summary>
    /// <param name="x0">Параметр сдвига</param>
    /// <param name="g">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetCauchyDistributionExpression(double x0, double g)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var X0 = x0.ToExpression();
        var G = g.ToExpression();
        var body = G.Divide(Consts.pi.ToExpression().Mult(G.Power(2).Add(X.Subtract(X0)).Power(2)));
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Случайная величина с распределением Коши</summary>
    /// <param name="x0"></param>
    /// <param name="gamma"></param>
    /// <returns></returns>
    public double Cauchy(double x0, double gamma)
    {
        double x, y;
        do
        {
            x = Uniform(-1, 1);
            y = Uniform(-1, 1);
        } while (x * x + y * y > 1.0 || y.Equals(0d));
        return x0 + gamma * x / y;
    }

    #endregion

    #region Laplace

    /// <summary>Функция плотности распределения Лапласа</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="b">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double LaplaceDistribution(double x, double m, double b)
        => Exp(-Abs(x - m) / b) / (2 * b);
    /// <summary>Возвращает функцию плотности распределения Лапласа</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="b">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetLaplaceDistribution(double m, double b) =>
        x => Exp(-Abs(x - m) / b) / (2 * b);

    /// <summary>Возвращает выражение плотности распределения Лапласа</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="b">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetLaplaceDistributionExpression(double m, double b)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var B = b.ToExpression();
        var body = 2.ToExpression().Mult(B).Inverse().Mult(MathExpression.Exp(X.Subtract(M).Divide(B).Negate()));
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Формирует случайную величину с распределением Лапласа</summary>
    /// <param name="mu">Параметр сдвига</param>
    /// <param name="b">Параметр масштаба</param>
    /// <returns>Случайная величина</returns>
    public double Laplace(double mu, double b) => mu + ((long)BasicRandGenerator() > 0 ? Exponential(1.0 / b) : -Exponential(1.0 / b));

    #endregion

    #region Levy

    /// <summary>Функция плотности распределения Леви</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="c">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double LevyDistribution(double x, double m, double c)
        => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Pow(3)));
    /// <summary>Возвращает функцию плотности распределения Леви</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="c">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetLevyDistribution(double m, double c) =>
        x => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Pow(3)));

    /// <summary>Возвращает выражение плотности распределения Леви</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="c">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetLevyDistributionExpression(double m, double c)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var C = c.ToExpression();
        var body = C.Mult(MathExpression.Exp(C.Divide(M.Subtract(X))))
           .Divide(2.ToExpression().Mult(Consts.pi).Mult(X.Subtract(M).Power(3))).SqrtPower();
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Формирует случайную величину с распределением Леви</summary>
    /// <param name="mu">Параметр сдвига</param>
    /// <param name="c">Параметр масштаба</param>
    /// <returns>Случайная величина</returns>
    public double Levy(double mu, double c)
    {
        var n = Normal(mu, 1.0 / c);
        return mu + 1 / (n * n);
    }

    #endregion

    #region ChiSquared

    /// <summary>Функция плотности распределения хи-квадрат</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="k">Число степеней свободы</param>
    /// <returns>Значение плотности</returns>
    public static double ChiSquaredDistribution(double x, double k)
        => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);
    /// <summary>Возвращает функцию плотности распределения хи-квадрат</summary>
    /// <param name="k">Число степеней свободы</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetChiSquaredDistribution(double k) =>
        x => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);

    /// <summary>Возвращает выражение плотности распределения хи-квадрат</summary>
    /// <param name="k">Число степеней свободы</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetChiSquaredDistributionExpression(double k)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var K05 = k.ToExpression().Divide(2);
        var body = K05.PowerOf(2).Mult(MathExpression.F(SpecialFunctions.Gamma.G, K05)).Inverse()
           .Mult(X.Power(K05.Subtract(1)).Mult(MathExpression.Exp(X.Divide(2).Negate())));
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Формирует случайную величину с распределением хи-квадрат</summary>
    /// <param name="k">Число степеней свободы</param>
    /// <returns>Случайная величина</returns>
    public double ChiSquared(int k)
    {
        // ~ Gamma(k / 2, 2)
        if (k >= 10) // too big parameter
            return GO(0.5 * k);
        var x = (k & 1) != 0 ? GA2(0.5 * k) : GA1(k >> 1);
        return x + x;
    }

    #endregion

    #region LogNormal

    /// <summary>Функция плотности логнормального распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double LogNormalDistribution(double x, double m, double s)
        => Exp(Log(x - m).Pow(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

    /// <summary>Возвращает функцию плотности логнормального распределения</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetLogNormalDistribution(double m, double s) =>
        x => Exp(Log(x - m).Pow(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

    /// <summary>Возвращает выражение плотности логнормального распределения</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetLogNormalDistributionExpression(double m, double s)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var S = s.ToExpression();
        var body = X.Mult(S).Mult(2.ToExpression().Mult(Consts.pi).Sqrt()).Inverse()
           .Mult(
                MathExpression.Exp(
                    MathExpression.Log(X.Subtract(M)).Power(2).Divide(2.ToExpression().Mult(S.Power(2)))).Negate());
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    /// <summary>Формирует случайную величину с логнормальным распределением</summary>
    /// <param name="mu">Параметр сдвига</param>
    /// <param name="sigma">Параметр масштаба</param>
    /// <returns>Случайная величина</returns>
    public double LogNormal(double mu, double sigma) => Exp(Normal(mu, sigma));

    #endregion

    #region Logistic

    /// <summary>Функция плотности логистического распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Значение плотности</returns>
    public static double LogisticDistribution(double x, double m, double s)
        => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Pow(2);

    /// <summary>Возвращает функцию плотности логистического распределения</summary>
    /// <param name="m">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Функция плотности</returns>
    public static Func<double, double> GetLogisticDistribution(double m, double s) =>
        x => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Pow(2);

    /// <summary>Возвращает выражение плотности логистического распределения</summary>
    /// <param name="M">Параметр сдвига</param>
    /// <param name="S">Параметр масштаба</param>
    /// <returns>Выражение плотности</returns>
    public static Expression<Func<double, double>> GetLogisticDistributionExpression(double M, double S)
    {
        var x = Expression.Parameter(typeof(double), "x");
        var m = M.ToExpression();
        var s = S.ToExpression();
        var e = MathExpression.Exp(x.Subtract(m).Divide(s).Negate());
        var body = e.Divide(s.Mult(1.ToExpression().Add(e)).Power(2));
        return Expression.Lambda<Func<double, double>>(body, x);
    }

    /// <summary>Формирует случайную величину с логистическим распределением</summary>
    /// <param name="mu">Параметр сдвига</param>
    /// <param name="s">Параметр масштаба</param>
    /// <returns>Случайная величина</returns>
    public double Logistic(double mu, double s) => mu + s * Log(1.0 / Uniform(0, 1) - 1);

    #endregion

    /// <summary>Формирует случайную величину с распределением Эрланга</summary>
    /// <param name="k">Параметр формы</param>
    /// <param name="l">Параметр интенсивности</param>
    /// <returns>Случайная величина</returns>
    public double Erlang(int k, double l) => GA1(k) / l;

    /// <summary>Формирует случайную величину с распределением Вейбулла</summary>
    /// <param name="l">Параметр масштаба</param>
    /// <param name="k">Параметр формы</param>
    /// <returns>Случайная величина</returns>
    // ReSharper disable once IdentifierTypo
    public double Weibull(double l, double k) => l * Pow(Exponential(1), 1 / k);

    /// <summary>Формирует случайную величину с распределением Рэлея</summary>
    /// <param name="sigma">Параметр масштаба</param>
    /// <returns>Случайная величина</returns>
    public double Rayleigh(double sigma) => sigma * Sqrt(Exponential(0.5));

    /// <summary>Формирует случайную величину с распределением Парето</summary>
    /// <param name="xm">Минимальное значение</param>
    /// <param name="alpha">Параметр формы</param>
    /// <returns>Случайная величина</returns>
    public double Pareto(double xm, double alpha) => xm / Pow(Uniform(0, 1), 1 / alpha);

    /// <summary>Формирует случайную величину с распределением Стьюдента</summary>
    /// <param name="v">Число степеней свободы</param>
    /// <returns>Случайная величина</returns>
    public double StudentT(int v) => v == 1 ? Cauchy(0, 1) : Normal(0, 1) / Sqrt(ChiSquared(v) / v);

    /// <summary>Формирует случайную величину с распределением Фишера-Снедекора</summary>
    /// <param name="d1">Первое число степеней свободы</param>
    /// <param name="d2">Второе число степеней свободы</param>
    /// <returns>Случайная величина</returns>
    // ReSharper disable once IdentifierTypo
    public double FisherSnedecor(int d1, int d2)
    {
        var numerator = d2 * ChiSquared(d1);
        var denominator = d1 * ChiSquared(d2);
        return numerator / denominator;
    }

    /// <summary>Формирует случайную величину с бета-распределением</summary>
    /// <param name="a">Первый параметр формы</param>
    /// <param name="b">Второй параметр формы</param>
    /// <returns>Случайная величина</returns>
    public double Beta(double a, double b)
    {
        var x = GA2(a);
        return x / (x + GA2(b));
    }

}