using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MathCore.Exceptions;
using MathCore.Extentions.Expressions;
using static System.Math;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.Statistic.RandomNumbers
{
    [Copyright("Александр Самарин - Генераторы непрерывно распределенных случайных величин", url = "http://habrahabr.ru/post/263993/")]
    public class PolyformRandomGenertor
    {
        //todo: http://habrahabr.ru/post/265321/
        private readonly Random _RND = new Random();
        private ulong _LastRND;
        public const ulong RAND_MAX = ulong.MaxValue;
        public PolyformRandomGenertor() => _LastRND = (ulong)_RND.Next();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ulong BasicRandGenerator() => (_LastRND << 32) & (_LastRND = (ulong)_RND.Next());

        #region Uniform

        public static double UniformDistribution(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

        public static Func<double, double> GetUniformDestribution(double a, double b)
            => x => x >= a && x <= b ? 1 / (b - a) : 0;
        public static Expression<Func<double, double>> GetUniformDestributionExpression(double a, double b)
            => x => x >= a && x <= b ? 1 / (b - a) : 0;

        public double Uniform(double a, double b) => a + (double)BasicRandGenerator() / RAND_MAX * (b - a);

        #endregion


        #region Normal

        public static double NormalDistribution(double x, double m, double s) => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);
        public static Func<double, double> GetNormalDestribution(double m, double s) => x => 1 / s / Sqrt(Consts.pi2) * Exp(-(x - m) * (x - m) / 2 / s / s);

        public static Expression<Func<double, double>> GetNormalDestributionExpression(double m, double s)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var M = m.ToExpression();
            var S = s.ToExpression();
            var expr_2 = 2.ToExpression();
            var sqrt2PI = expr_2.Multiply(Consts.pi.ToExpression()).Power(0.5);
            var E = ((Func<double, double>)Exp).GetCallExpression(X.Subtract(M).Power(2).Divide(expr_2.Multiply(S.Power(2))).Negate());
            var body = 1.ToExpression().Divide(S.Multiply(sqrt2PI)).Multiply(E);
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        private static double[] __Normal_StairWidth;
        private static double[] __Normal_StairHeight;
        private static bool __Normal_Initialized;
        private static readonly object __Normal_Initialization_SyncRoot = new object();
        private const double c_Normal_x1 = 3.6541528853610088;

        /// <summary>area under rectangle</summary>
        private const double c_Normal_A = 4.92867323399e-3;

        private static bool InitializeNormal()
        {
            if(__Normal_Initialized) return true;
            lock (__Normal_Initialization_SyncRoot)
            {
                if(__Normal_Initialized) return true;
                __Normal_StairWidth = new double[257];
                __Normal_StairHeight = new double[256];
                // coordinates of the implicit rectangle in base layer
                __Normal_StairHeight[0] = Exp(-.5 * c_Normal_x1 * c_Normal_x1);
                __Normal_StairWidth[0] = c_Normal_A / __Normal_StairHeight[0];
                // implicit value for the top layer
                __Normal_StairWidth[256] = 0;
                for(var i = 1; i <= 255; ++i)
                {
                    // such x_i that f(x_i) = y_{i-1}
                    __Normal_StairWidth[i] = Sqrt(-2 * Log(__Normal_StairHeight[i - 1]));
                    __Normal_StairHeight[i] = __Normal_StairHeight[i - 1] + c_Normal_A / __Normal_StairWidth[i];
                }
                return __Normal_Initialized = true;
            }
        }

        private double _NormalZiggurat_z;

        [MethodImpl(MethodImplOptions.Synchronized)]
        private double NormalZiggurat()
        {
            var iter = 0;
            do
            {
                var B = BasicRandGenerator();
                var stairId = (int)(B & 0xff);
                var x = Uniform(0, __Normal_StairWidth[stairId]); // get horizontal coordinate
                if(x < __Normal_StairWidth[stairId + 1])
                    return (long)B > 0 ? x : -x;
                if(stairId == 0) // handle the base layer
                {
                    _NormalZiggurat_z = -1;
                    double y;
                    if(_NormalZiggurat_z >= 0)
                    // we don't have to generate another exponential variable as we already have one
                    {
                        y = Exponential(1);
                        _NormalZiggurat_z = y - 0.5 * _NormalZiggurat_z * _NormalZiggurat_z;
                    }
                    if(_NormalZiggurat_z < 0) // if previous generation wasn't successful
                    {
                        do
                        {
                            x = Exponential(c_Normal_x1);
                            y = Exponential(1);
                            _NormalZiggurat_z = y - 0.5 * x * x;
                            // we storage this value as after acceptance it becomes exponentially distributed
                        } while(_NormalZiggurat_z <= 0);
                    }
                    x += c_Normal_x1;
                    return (long)B > 0 ? x : -x;
                }
                // handle the wedges of other stairs
                if(Uniform(__Normal_StairHeight[stairId - 1], __Normal_StairHeight[stairId]) < Exp(-.5 * x * x))
                    return (long)B > 0 ? x : -x;
            } while(++iter <= 1e9); // one billion should be enough
            throw new CalculationsException();
        }

        public double Normal(double mu, double sigma)
        {
            if(__Normal_Initialized || InitializeNormal()) return mu + NormalZiggurat() * sigma;
            throw new CalculationsException();
        }

        #endregion

        #region Exponential

        public static double ExponentialDistribution(double x, double l, double s) => l * Exp(-l * x);
        public static Func<double, double> GetExponentialDestribution(double l, double s) => x => l * Exp(-l * x);

        public static Expression<Func<double, double>> GetExponentialDestributionExpression(double l, double s)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var body = l.ToExpression().Multiply(MathExpression.Exp(l.ToExpression().Multiply(X).Negate()));
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        private static double[] __Exponential_StairWidth;
        private static double[] __Exponential_StairHeight;
        private static bool __Exponential_Initialized;
        private static readonly object __Exponential_Initialization_SyncRoot = new object();
        private const double x1 = 7.69711747013104972;

        /// <summary>area under rectangle</summary>
        private const double A = 3.9496598225815571993e-3;

        private static bool InitializeExponential()
        {
            if(__Exponential_Initialized) return true;
            lock (__Exponential_Initialization_SyncRoot)
            {
                if(__Exponential_Initialized) return true;
                __Exponential_StairWidth = new double[257];
                __Exponential_StairHeight = new double[256];
                // coordinates of the implicit rectangle in base layer
                __Exponential_StairHeight[0] = Exp(-x1);
                __Exponential_StairWidth[0] = A / __Exponential_StairHeight[0];
                // implicit value for the top layer
                __Exponential_StairWidth[256] = 0;
                for(var i = 1; i <= 255; ++i)
                {
                    // such x_i that f(x_i) = y_{i-1}
                    __Exponential_StairWidth[i] = -Log(__Exponential_StairHeight[i - 1]);
                    __Exponential_StairHeight[i] = __Exponential_StairHeight[i - 1] + A / __Exponential_StairWidth[i];
                }
                return __Exponential_Initialized = true;
            }
        }

        private double ExpZiggurat()
        {
            var iter = 0;
            do
            {
                var stairId = (int)(BasicRandGenerator() & 255);
                var x = Uniform(0, __Exponential_StairWidth[stairId]); // get horizontal coordinate
                if(x < __Exponential_StairWidth[stairId + 1]) // if we are under the upper stair - accept
                    return x;
                if(stairId == 0) // if we catch the tail
                    return x1 + ExpZiggurat();
                if(Uniform(__Exponential_StairHeight[stairId - 1], __Exponential_StairHeight[stairId]) < Exp(-x))
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
        public static Func<double, double> GetGammaDestribution(double k, double th) => x => Pow(x, k - 1) * Exp(-x / th) / (SpecialFunctions.Gamma.G(k) * Pow(th, k));

        public static Expression<Func<double, double>> GetGammaDestributionExpression(double k, double th)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var K = k.ToExpression();
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
            double x;
            var iter = 0;
            do
            {
                // M_E is base of natural logarithm
                var U = Uniform(0, 1 + k / Consts.e);
                var W = Exponential(1);
                if(U <= 1)
                {
                    x = Pow(U, 1.0 / k);
                    if(x <= W)
                        return x;
                }
                else
                {
                    x = -Log((1 - U) / k + 1.0 / Consts.e);
                    if((1 - k) * Log(x) <= W)
                        return x;
                }
            } while(++iter < 1e9); // excessive maximum number of rejections
            throw new CalculationsException();
        }

        public double GF(double k)
        {
            // Assume that 1 < k < 3
            double E1, E2;
            do
            {
                E1 = Exponential(1);
                E2 = Exponential(1);
            } while(E2 < (k - 1) * (E1 - Log(E1) - 1));
            return k * E1;
        }

        private double GO(double k)
        {
            // Assume that k > 3
            var m = k - 1;
            var s_2 = Sqrt(8.0 * k / 3) + k;
            var s = Sqrt(s_2);
            var d = Consts.sqrt_2 * Consts.sqrt_3 * s_2;
            var b = d + m;
            var w = s_2 / (m - 1);
            var v = (s_2 + s_2) / (m * Sqrt(k));
            var c = b + Log(s * d / b) - m - m - 3.7203285;

            double x;
            var iter = 0;
            do
            {
                var U = Uniform(0, 1);
                if(U <= 0.0095722652)
                {
                    var E1 = Exponential(1);
                    var E2 = Exponential(1);
                    x = b * (1 + E1 / d);
                    if(m * (x / b - Log(x / m)) + c <= E2)
                        return x;
                }
                else
                {
                    double N;
                    do
                    {
                        N = Normal(0, 1);
                        x = s * N + m; // ~ Normal(m, s)
                    } while(x < 0 || x > b);
                    U = Uniform(0, 1);
                    var S = 0.5 * N * N;
                    if(N > 0)
                    {
                        if(U < 1 - w * S)
                            return x;
                    }
                    else if(U < 1 + S * (v * N - w))
                        return x;
                    if(Log(U) < m * Log(x / m) + m - x + S)
                        return x;
                }
            } while(++iter < 1e9);
            throw new CalculationsException();
        }

        #endregion

        #region Cauchy

        public static double CauchyDistribution(double x, double x0, double g)
           => g / (Consts.pi * (g * g + (x - x0).Power(2)));
        public static Func<double, double> GetCauchyDestribution(double x0, double g) =>
            x => g / (Consts.pi * (g * g + (x - x0).Power(2)));

        public static Expression<Func<double, double>> GetCauchyDestributionExpression(double x0, double g)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var X0 = x0.ToExpression();
            var G = g.ToExpression();
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
        public static Func<double, double> GetLaplaceDestribution(double m, double b) =>
            x => Exp(-Abs(x - m) / b) / (2 * b);

        public static Expression<Func<double, double>> GetLaplaceDestributionExpression(double m, double b)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var M = m.ToExpression();
            var B = b.ToExpression();
            var body = 2.ToExpression().Multiply(B).Inverse().Multiply(MathExpression.Exp(X.Subtract(M).Divide(B).Negate()));
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        public double Laplace(double mu, double b) => mu + ((long)BasicRandGenerator() > 0 ? Exponential(1.0 / b) : -Exponential(1.0 / b));

        #endregion

        #region Levy

        public static double LevyDistribution(double x, double m, double c)
            => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Power(3)));
        public static Func<double, double> GetLevyDestribution(double m, double c) =>
            x => Sqrt(c * Exp(c / (m - x)) / (Consts.pi2 * (x - m).Power(3)));

        public static Expression<Func<double, double>> GetLevyDestributionExpression(double m, double c)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var M = m.ToExpression();
            var C = c.ToExpression();
            var body = C.Multiply(MathExpression.Exp(C.Divide(M.Subtract(X))))
                .Divide(2.ToExpression().Multiply(Consts.pi).Multiply(X.Subtract(M).Power(3))).SqrtPower();
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        public double Levy(double mu, double c)
        {
            var n = Normal(mu, 1.0 / c);
            return mu + 1.0 / (n * n);
        }

        #endregion

        #region ChiSquared

        public static double ChiSquaredDistribution(double x, double k)
            => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);
        public static Func<double, double> GetChiSquaredDestribution(double k) =>
            x => Pow(x, k / 2 - 1) * Exp(-x / 2) / Pow(2, k / 2) / SpecialFunctions.Gamma.G(k / 2);

        public static Expression<Func<double, double>> GetChiSquaredDestributionExpression(double k)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var K05 = k.ToExpression().Divide(2);
            var body = K05.PowerOf(2).Multiply(MathExpression.F(SpecialFunctions.Gamma.G, K05)).Inverse()
                .Multiply(X.Power(K05.Subtract(1)).Multiply(MathExpression.Exp(X.Divide(2).Negate())));
            return Expression.Lambda<Func<double, double>>(body, X);
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
            => Exp(Log(x - m).Power(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

        public static Func<double, double> GetLogNormalDestribution(double m, double s) =>
            x => Exp(Log(x - m).Power(2) / (2 * s * s)) / (x * s * Consts.sqrt_pi2);

        public static Expression<Func<double, double>> GetLogNormalDestributionExpression(double m, double s)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var M = m.ToExpression();
            var S = s.ToExpression();
            var body = X.Multiply(S).Multiply(2.ToExpression().Multiply(Consts.pi).Sqrt()).Inverse()
                .Multiply(
                    MathExpression.Exp(
                        MathExpression.Log(X.Subtract(M)).Power(2).Divide(2.ToExpression().Multiply(S.Power(2)))).Negate());
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        public double LogNormal(double mu, double sigma) => Exp(Normal(mu, sigma));

        #endregion

        #region Logistic

        public static double LogisticDistribution(double x, double m, double s)
            => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Power(2);

        public static Func<double, double> GetLogisticDestribution(double m, double s) =>
            x => Exp(-(x - m) / s) / s / (1 + Exp(-(x - m) / s)).Power(2);

        public static Expression<Func<double, double>> GetLogisticDestributionExpression(double m, double s)
        {
            var X = Expression.Parameter(typeof(double), "x");
            var M = m.ToExpression();
            var S = s.ToExpression();
            var E = MathExpression.Exp(X.Subtract(M).Divide(S).Negate());
            var body = E.Divide(S.Multiply(1.ToExpression().Add(E)).Power(2));
            return Expression.Lambda<Func<double, double>>(body, X);
        }

        public double Logistic(double mu, double s) => mu + s * Log(1.0 / Uniform(0, 1) - 1);

        #endregion

        public double Erlang(int k, double l) => GA1(k) / l;

        public double Weibull(double l, double k) => l * Pow(Exponential(1), 1.0 / k);

        public double Rayleigh(double sigma) => sigma * Sqrt(Exponential(0.5));

        public double Pareto(double xm, double alpha) => xm / Pow(Uniform(0, 1), 1.0 / alpha);

        public double StudentT(int v) => v == 1 ? Cauchy(0, 1) : Normal(0, 1) / Sqrt(ChiSquared(v) / v);

        public double FisherSnedecor(int d1, int d2)
        {
            var numerator = d2 * ChiSquared(d1);
            var denominator = d1 * ChiSquared(d2);
            return numerator / denominator;
        }

        public double Beta(double a, double b)
        {
            var x = GA2(a);
            return x / (x + GA2(b));
        }

    }
}