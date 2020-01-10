using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Statistic
{
    /// <summary>Распределения</summary>
    public static partial class Distributions
    {
        [NotNull]
        public static Func<double, double> Gamma(double k, double theta) =>
            x => x >= 0
                ? Math.Pow(x, k - 1) * Math.Exp(-x / theta) / Math.Pow(theta, k) /
                  SpecialFunctions.Gamma.G(k)
                : 0;

        public static double Gamma(double k, double theta, double x) =>
            x >= 0
                ? Math.Pow(x, k - 1) * Math.Exp(-x / theta) / Math.Pow(theta, k) / SpecialFunctions.Gamma.G(k)
                : 0;

        [NotNull] public static Func<double, double> Hi2(int k) => Gamma(k / 2d, 2);

        /// <summary>Распределение Хи-квадрат</summary>
        /// <param name="k">Число степеней свободы</param>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение плотности вероятности</returns>
        public static double Hi2(int k, double x) => Gamma(k / 2d, 2, x);

        [NotNull]
        public static Func<double, double> NormalGauss(double sigma = 1, double mu = 0) =>
            x =>
            {
                x -= mu;
                return Math.Exp(-(x * x) / (2 * sigma * sigma)) / (Consts.sqrt_pi2 * sigma);
            };

        public static double NormalGauss(double x, double sigma, double mu)
        {
            sigma *= 2 * sigma;
            x -= mu;
            x *= x;
            return Math.Exp(-x / sigma) / (Consts.pi * sigma);
        }

        /// <summary>Равномерное распределение</summary>
        /// <param name="a">Минимальное значение</param>
        /// <param name="b">Максимальное значение</param>
        /// <returns>Функция равномерного распределения в заданном интервале</returns>
        [NotNull] public static Func<double, double> Uniform(double a, double b) => x => x >= a && x <= b ? 1 / (b - a) : 0;

        /// <summary>Равномерное распределение</summary>
        /// <param name="x">Аргумент</param>
        /// <param name="a">Минимальное значение</param>
        /// <param name="b">Максимальное значение</param>
        /// <returns>Значение равномерного распределения в заданной точке</returns>
        public static double Uniform(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

        [NotNull] public static Func<double, double> Triangular(double a, double b) => x => Triangular(x, a, b);

        public static double Triangular(double x, double a, double b) => x >= a && x <= b ? 2 / (b - a) - 2 / ((b - a) * (b - a)) * Math.Abs(a + b - 2 * x) : 0;

        /// <summary>Распределение Рэлея http://ru.wikipedia.org/wiki/Распределение_Рэлея</summary>
        /// <param name="sigma">Параметр масштаба</param>
        /// <returns>Функция распределения для заданного масштаба</returns>
        [NotNull] public static Func<double, double> Rayleigh(double sigma) => x => Rayleigh(x, sigma);

        /// <summary>Распределение Рэлея http://ru.wikipedia.org/wiki/Распределение_Рэлея</summary>
        /// <param name="x">Аргумент</param>
        /// <param name="sigma">Параметр масштаба</param>
        /// <returns>Значение распределения</returns>
        public static double Rayleigh(double x, double sigma)
        {
            var s = x / sigma / sigma;
            return s * Math.Exp(-s * x / 2);
        }

        [NotNull]
        private static Func<double, double> HistogramKey(double Max, double Min, double dx) =>
            x => (Math.Floor((Math.Min(x, Max - dx / 2) - Min) / dx) + 0.5) * dx + Min;

        public static double GetPirsonsCriteria([NotNull] double[] Samples, [NotNull] Func<double, double> f, out int FreedomDegree)
        {
            if (Samples is null) throw new ArgumentNullException(nameof(Samples));

            var count = Samples.Length;
            if (count == 0) throw new ArgumentException("Массы выборки пуст", nameof(Samples));
            Samples.GetMinMax(out var min, out var max);
            var interval = max - min;

            var dx = interval / (1 + 3.32 * Math.Log10(count));
            var segments_count = Math.Floor(interval / dx);
            var histogram_dx = interval / segments_count;

            var histogram = Samples
               .GroupBy(HistogramKey(max, min, histogram_dx))
               .ToDictionary(g => g.Key, g => g.Count())
               .OrderBy(x => x.Key)
               .ToArray();

            var xn = histogram.Select(v => v.Key * v.Value);

            var sample_average = xn.Sum() / count; // Среднее по совокупности

            // Скорректированное среднее
            var restored_variance = histogram.Sum(v =>
            {
                var (key, value) = v;
                var x = sample_average - key;
                return x * x * value;
            }) / (count - 1);

            // Восстановленное СКО
            var restored_sko = Math.Sqrt(restored_variance);

            FreedomDegree = histogram.Length - 3;
            return histogram.Sum(h =>
            {
                var (key, value) = h;
                var u = (key - sample_average) / restored_sko;
                var n0 = count * histogram_dx * f(u) / restored_sko;
                var d = value - n0;
                return d * d / n0;
            });
        }

        public static bool CheckDistribution([NotNull] this double[] Samples, [NotNull] Func<double, double> f, double TrustLevel = 0.95)
        {
            var pirsons_criteria = GetPirsonsCriteria(Samples, f, out var number_of_degrees_of_freedom);
             var hi_theoretical = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(TrustLevel, number_of_degrees_of_freedom);

            return pirsons_criteria < hi_theoretical;
        }
    }
}