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
public class PolyformRandomGenerator
{
    //todo: http://habrahabr.ru/post/265321/
    private readonly Random _RND = new();
    private ulong _LastRND;
    public const ulong RandMax = ulong.MaxValue;
    public PolyformRandomGenerator() => _LastRND = (ulong)_RND.Next();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public ulong BasicRandGenerator() => (_LastRND << 32) & (_LastRND = (ulong)_RND.Next());

    #region Uniform

    public static double UniformDistribution(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

    public static Func<double, double> GetUniformDistribution(double a, double b)
        => x => x >= a && x <= b ? 1 / (b - a) : 0;
    public static Expression<Func<double, double>> GetUniformDistributionExpression(double a, double b)
        => x => x >= a && x <= b ? 1 / (b - a) : 0;

    public double Uniform(double a, double b) => a + (double)BasicRandGenerator() / RandMax * (b - a);

    #endregion

    #region Normal

    public static double NormalDistribution(double x, double m, double s) => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);
    public static Func<double, double> GetNormalDistribution(double m, double s) => x => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);

    public static Expression<Func<double, double>> GetNormalDistributionExpression(double m, double s)
    {
        var X       = Expression.Parameter(typeof(double), "x");
        var M       = m.ToExpression();
        var S       = s.ToExpression();
        var expr_2  = 2.ToExpression();
        var sqrt2PI = expr_2.Multiply(Consts.pi.ToExpression()).Power(0.5);
        var E       = ((Func<double, double>)Exp).GetCallExpression(X.Subtract(M).Power(2).Divide(expr_2.Multiply(S.Power(2))).Negate());
        var body    = 1.ToExpression().Divide(S.Multiply(sqrt2PI)).Multiply(E);
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    private static double[] __NormalStairWidth;
    private static double[] __NormalStairHeight;
    private static bool __NormalInitialized;
    private static readonly object __NormalInitializationSyncRoot = new();
    private const double __Normal_x1 = 3.6541528853610088;

    /// <summary>area under rectangle</summary>
    private const double __Normal_A = 4.92867323399e-3;

    private static bool InitializeNormal()
    {
        if(__NormalInitialized) return true;
        lock (__NormalInitializationSyncRoot)
        {
            if(__NormalInitialized) return true;
            __NormalStairWidth  = new double[257];
            __NormalStairHeight = new double[256];
            // coordinates of the implicit rectangle in base layer
            __NormalStairHeight[0] = Exp(-.5 * __Normal_x1 * __Normal_x1);
            __NormalStairWidth[0]  = __Normal_A / __NormalStairHeight[0];
            // implicit value for the top layer
            __NormalStairWidth[256] = 0;
            for(var i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                __NormalStairWidth[i]  = Sqrt(-2 * Log(__NormalStairHeight[i - 1]));
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
            var B        = BasicRandGenerator();
            var stair_id = (int)(B & 0xff);
            var x        = Uniform(0, __NormalStairWidth[stair_id]); // get horizontal coordinate
            if(x < __NormalStairWidth[stair_id + 1])
                return (long)B > 0 ? x : -x;
            if(stair_id == 0) // handle the base layer
            {
                _NormalZigguratZ = -1;
                double y;
                if(_NormalZigguratZ >= 0)
                    // we don't have to generate another exponential variable as we already have one
                {
                    y                = Exponential(1);
                    _NormalZigguratZ = y - 0.5 * _NormalZigguratZ * _NormalZigguratZ;
                }
                if(_NormalZigguratZ < 0) // if previous generation wasn't successful
                    do
                    {
                        x                = Exponential(__Normal_x1);
                        y                = Exponential(1);
                        _NormalZigguratZ = y - 0.5 * x * x;
                        // we storage this value as after acceptance it becomes exponentially distributed
                    } while(_NormalZigguratZ <= 0);

                x += __Normal_x1;
                return (long)B > 0 ? x : -x;
            }
            // handle the wedges of other stairs
            if(Uniform(__NormalStairHeight[stair_id - 1], __NormalStairHeight[stair_id]) < Exp(-.5 * x * x))
                return (long)B > 0 ? x : -x;
        } while(++iterator <= 1e9); // one billion should be enough
        throw new CalculationsException();
    }

    public double Normal(double mu, double sigma) => 
        __NormalInitialized || InitializeNormal()
            ? mu + NormalZiggurat() * sigma 
            : throw new CalculationsException();

    #endregion

    #region Exponential

    public static double ExponentialDistribution(double x, double l, double s) => l * Exp(-l * x);
    public static Func<double, double> GetExponentialDistribution(double l, double s) => x => l * Exp(-l * x);

    public static Expression<Func<double, double>> GetExponentialDistributionExpression(double l, double s)
    {
        var x    = Expression.Parameter(typeof(double), "x");
        var body = l.ToExpression().Multiply(MathExpression.Exp(l.ToExpression().Multiply(x).Negate()));
        return Expression.Lambda<Func<double, double>>(body, x);
    }

    private static double[] __ExponentialStairWidth;
    private static double[] __ExponentialStairHeight;
    private static bool __ExponentialInitialized;
    private static readonly object __ExponentialInitializationSyncRoot = new();
    private const double __X1 = 7.69711747013104972;

    /// <summary>area under rectangle</summary>
    private const double __A = 3.9496598225815571993e-3;

    private static bool InitializeExponential()
    {
        if(__ExponentialInitialized) return true;
        lock (__ExponentialInitializationSyncRoot)
        {
            if(__ExponentialInitialized) return true;
            __ExponentialStairWidth  = new double[257];
            __ExponentialStairHeight = new double[256];
            // coordinates of the implicit rectangle in base layer
            __ExponentialStairHeight[0] = Exp(-__X1);
            __ExponentialStairWidth[0]  = __A / __ExponentialStairHeight[0];
            // implicit value for the top layer
            __ExponentialStairWidth[256] = 0;
            for(var i = 1; i <= 255; ++i)
            {
                // such x_i that f(x_i) = y_{i-1}
                __ExponentialStairWidth[i]  = -Log(__ExponentialStairHeight[i - 1]);
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
            var x        = Uniform(0, __ExponentialStairWidth[stair_id]); // get horizontal coordinate
            if(x < __ExponentialStairWidth[stair_id + 1])                 // if we are under the upper stair - accept
                return x;
            if(stair_id == 0) // if we catch the tail
                return __X1 + ExpZiggurat();
            if(Uniform(__ExponentialStairHeight[stair_id - 1], __ExponentialStairHeight[stair_id]) < Exp(-x))
                // if we are under the curve - accept
                return x;
            // rejection - go back
        } while(++iter <= 1e9); // one billion should be enough to be sure there is a bug
        throw new CalculationsException();
    }

    public double Exponential(double rate) => ExpZiggurat() / rate;

    #endregion

    #region Gamma

    public static double GammaDistribution(double x, double k, double th)
        => Pow(x, k - 1) * Exp(-x / th) / (SpecialFunctions.Gamma.G(k) * Pow(th, k));
    public static Func<double, double> GetGammaDistribution(double k, double th) => x => Pow(x, k - 1) * Exp(-x / th) / (SpecialFunctions.Gamma.G(k) * Pow(th, k));

    public static Expression<Func<double, double>> GetGammaDistributionExpression(double k, double th)
    {
        var X  = Expression.Parameter(typeof(double), "x");
        var K  = k.ToExpression();
        var TH = th.ToExpression();
        var body = X.Power(K.Subtract(1)).Multiply(MathExpression.Exp(X.Divide(TH)).Negate())
           .Divide(MathExpression.F(SpecialFunctions.Gamma.G, K).Multiply(TH.Power(K)));
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    public double GA1(int k)
    {
        double x = 0;
        for(var i = 0; i < k; ++i)
            x += Exponential(1);
        return x;
    }

    public double GA2(double k)
    {
        var x = Normal(0, 1);
        x *= 0.5 * x;
        for(var i = 1; i < k; ++i)
            x += Exponential(1);
        return x;
    }

    public double GS(double k)
    {
        // Assume that k < 1
        var iter = 0;
        do
        {
            // M_E is base of natural logarithm
            var    u = Uniform(0, 1 + k / Consts.e);
            var    w = Exponential(1);
            double x;
            if(u <= 1)
            {
                x = Pow(u, 1.0 / k);
                if(x <= w)
                    return x;
            }
            else
            {
                x = -Log((1 - u) / k + 1.0 / Consts.e);
                if((1 - k) * Log(x) <= w)
                    return x;
            }
        } while(++iter < 1e9); // excessive maximum number of rejections
        throw new CalculationsException();
    }

    public double GF(double k)
    {
        // Assume that 1 < k < 3
        double e1, e2;
        do
        {
            e1 = Exponential(1);
            e2 = Exponential(1);
        } while(e2 < (k - 1) * (e1 - Log(e1) - 1));
        return k * e1;
    }

    private double GO(double k)
    {
        // Assume that k > 3
        var m       = k - 1;
        var s2      = Sqrt(8 * k / 3) + k;
        var sqrt_s2 = Sqrt(s2);
        var d       = Consts.sqrt_2 * Consts.sqrt_3 * s2;
        var b       = d + m;
        var w       = s2 / (m - 1);
        var v       = (s2 + s2) / (m * Sqrt(k));
        var c       = b + Log(sqrt_s2 * d / b) - m - m - 3.7203285;

        var iter = 0;
        do
        {
            var    u = Uniform(0, 1);
            double x;
            if(u <= 0.0095722652)
            {
                var e1 = Exponential(1);
                var e2 = Exponential(1);
                x = b * (1 + e1 / d);
                if(m * (x / b - Log(x / m)) + c <= e2)
                    return x;
            }
            else
            {
                double n;
                do
                {
                    n = Normal(0, 1);
                    x = sqrt_s2 * n + m; // ~ Normal(m, s)
                } while(x < 0 || x > b);
                u = Uniform(0, 1);
                var s = 0.5 * n * n;
                if(n > 0)
                {
                    if(u < 1 - w * s)
                        return x;
                }
                else if(u < 1 + s * (v * n - w))
                    return x;
                if(Log(u) < m * Log(x / m) + m - x + s)
                    return x;
            }
        } while(++iter < 1e9);
        throw new CalculationsException();
    }

    #endregion

    #region Cauchy

    public static double CauchyDistribution(double x, double x0, double g)
        => g / (Consts.pi * (g * g + (x - x0).Pow(2)));
    public static Func<double, double> GetCauchyDistribution(double x0, double g) =>
        x => g / (Consts.pi * (g * g + (x - x0).Pow(2)));

    public static Expression<Func<double, double>> GetCauchyDistributionExpression(double x0, double g)
    {
        var X    = Expression.Parameter(typeof(double), "x");
        var X0   = x0.ToExpression();
        var G    = g.ToExpression();
        var body = G.Divide(Consts.pi.ToExpression().Multiply(G.Power(2).Add(X.Subtract(X0)).Power(2)));
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
        } while(x * x + y * y > 1.0 || y.Equals(0d));
        return x0 + gamma * x / y;
    }

    #endregion

    #region Laplace

    public static double LaplaceDistribution(double x, double m, double b)
        => Exp(-Abs(x - m) / b) / (2 * b);
    public static Func<double, double> GetLaplaceDistribution(double m, double b) =>
        x => Exp(-Abs(x - m) / b) / (2 * b);

    public static Expression<Func<double, double>> GetLaplaceDistributionExpression(double m, double b)
    {
        var X    = Expression.Parameter(typeof(double), "x");
        var M    = m.ToExpression();
        var B    = b.ToExpression();
        var body = 2.ToExpression().Multiply(B)!.Inverse().Multiply(MathExpression.Exp(X.Subtract(M).Divide(B).Negate()));
        return Expression.Lambda<Func<double, double>>(body!, X);
    }

    public double Laplace(double mu, double b) => mu + ((long)BasicRandGenerator() > 0 ? Exponential(1.0 / b) : -Exponential(1.0 / b));

    #endregion

    #region Levy

    public static double LevyDistribution(double x, double m, double c)
        => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Pow(3)));
    public static Func<double, double> GetLevyDistribution(double m, double c) =>
        x => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Pow(3)));

    public static Expression<Func<double, double>> GetLevyDistributionExpression(double m, double c)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var C = c.ToExpression();
        var body = C.Multiply(MathExpression.Exp(C.Divide(M.Subtract(X))))!
           .Divide(2.ToExpression().Multiply(Consts.pi).Multiply(X.Subtract(M).Power(3))).SqrtPower();
        return Expression.Lambda<Func<double, double>>(body, X);
    }

    public double Levy(double mu, double c)
    {
        var n = Normal(mu, 1.0 / c);
        return mu + 1 / (n * n);
    }

    #endregion

    #region ChiSquared

    public static double ChiSquaredDistribution(double x, double k)
        => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);
    public static Func<double, double> GetChiSquaredDistribution(double k) =>
        x => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);

    public static Expression<Func<double, double>> GetChiSquaredDistributionExpression(double k)
    {
        var X   = Expression.Parameter(typeof(double), "x");
        var K05 = k.ToExpression().Divide(2);
        var body = K05.PowerOf(2).Multiply(MathExpression.F(SpecialFunctions.Gamma.G, K05))!.Inverse()
           .Multiply(X.Power(K05.Subtract(1)).Multiply(MathExpression.Exp(X.Divide(2).Negate())));
        return Expression.Lambda<Func<double, double>>(body!, X);
    }

    public double ChiSquared(int k)
    {
        // ~ Gamma(k / 2, 2)
        if(k >= 10) // too big parameter
            return GO(0.5 * k);
        var x = (k & 1) != 0 ? GA2(0.5 * k) : GA1(k >> 1);
        return x + x;
    }

    #endregion

    #region LogNormal

    public static double LogNormalDistribution(double x, double m, double s)
        => Exp(Log(x - m).Pow(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

    public static Func<double, double> GetLogNormalDistribution(double m, double s) =>
        x => Exp(Log(x - m).Pow(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

    public static Expression<Func<double, double>> GetLogNormalDistributionExpression(double m, double s)
    {
        var X = Expression.Parameter(typeof(double), "x");
        var M = m.ToExpression();
        var S = s.ToExpression();
        var body = X.Multiply(S).Multiply(2.ToExpression().Multiply(Consts.pi).Sqrt())!.Inverse()
           .Multiply(
                MathExpression.Exp(
                    MathExpression.Log(X.Subtract(M)).Power(2).Divide(2.ToExpression().Multiply(S.Power(2)))).Negate());
        return Expression.Lambda<Func<double, double>>(body!, X);
    }

    public double LogNormal(double mu, double sigma) => Exp(Normal(mu, sigma));

    #endregion

    #region Logistic

    public static double LogisticDistribution(double x, double m, double s)
        => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Pow(2);

    public static Func<double, double> GetLogisticDistribution(double m, double s) =>
        x => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Pow(2);

    public static Expression<Func<double, double>> GetLogisticDistributionExpression(double M, double S)
    {
        var x    = Expression.Parameter(typeof(double), "x");
        var m    = M.ToExpression();
        var s    = S.ToExpression();
        var e    = MathExpression.Exp(x.Subtract(m).Divide(s).Negate());
        var body = e.Divide(s.Multiply(1.ToExpression().Add(e))!.Power(2));
        return Expression.Lambda<Func<double, double>>(body, x);
    }

    public double Logistic(double mu, double s) => mu + s * Log(1.0 / Uniform(0, 1) - 1);

    #endregion

    public double Erlang(int k, double l) => GA1(k) / l;

    // ReSharper disable once IdentifierTypo
    public double Weibull(double l, double k) => l * Pow(Exponential(1), 1 / k);

    public double Rayleigh(double sigma) => sigma * Sqrt(Exponential(0.5));

    public double Pareto(double xm, double alpha) => xm / Pow(Uniform(0, 1), 1 / alpha);

    public double StudentT(int v) => v == 1 ? Cauchy(0, 1) : Normal(0, 1) / Sqrt(ChiSquared(v) / v);

    // ReSharper disable once IdentifierTypo
    public double FisherSnedecor(int d1, int d2)
    {
        var numerator   = d2 * ChiSquared(d1);
        var denominator = d1 * ChiSquared(d2);
        return numerator / denominator;
    }

    public double Beta(double a, double b)
    {
        var x = GA2(a);
        return x / (x + GA2(b));
    }
}