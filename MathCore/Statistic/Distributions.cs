using System;

namespace MathCore.Statistic
{
    public static partial class Distributions
    {
        public static Func<double, double> Gamma(double k, double thetta)
        {
            return x => x >= 0
                            ? Math.Pow(x, k - 1) * Math.Exp(-x / thetta) / Math.Pow(thetta, k) /
                                SpecialFunctions.Gamma.G(k)
                            : 0;
        }

        public static double Gamma(double k, double thetta, double x)
        {
            return x >= 0
                       ? Math.Pow(x, k - 1) * Math.Exp(-x / thetta) / Math.Pow(thetta, k) / SpecialFunctions.Gamma.G(k)
                       : 0;
        }

        public static Func<double, double> Hi2(int n) => Gamma(n / 2d, 2);

        public static double Hi2(int n, double x) => Gamma(n / 2d, 2, x);

        public static Func<double, double> NormalGaus(double sigma = 1, double mu = 0)
        {
            sigma *= 2 * sigma;
            return x =>
                       {
                           x -= mu;
                           return Math.Exp(-(x * x) / sigma) / (Consts.pi * sigma);
                       };
        }

        public static double NormalGaus(double x, double sigma, double mu)
        {
            sigma *= 2 * sigma;
            x -= mu;
            x *= x;
            return Math.Exp(-x / sigma) / (Consts.pi * sigma);
        }

        public static Func<double, double> Uniform(double a, double b) => x => x >= a && x <= b ? 1 / (b - a) : 0;

        public static double Uniform(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

        public static Func<double, double> Triangular(double a, double b) => x => Triangular(x, a, b);

        public static double Triangular(double x, double a, double b) => x >= a && x <= b ? 2 / (b - a) - 2 / ((b - a) * (b - a)) * Math.Abs(a + b - 2 * x) : 0;

        public static Func<double, double> Rayleigh(double sigma) => x => Rayleigh(x, sigma);
        public static double Rayleigh(double x, double sigma)
        {
            var s = x / sigma / sigma;
            return s * Math.Exp(-s * x / 2);
        }
    }
}