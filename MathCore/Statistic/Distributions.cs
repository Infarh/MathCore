#nullable enable
using MathCore.Annotations;

namespace MathCore.Statistic;

/// <summary>Распределения</summary>
public static partial class Distributions
{
    public static Func<double, double> Gamma(double k, double theta) =>
        x => x >= 0
            ? Math.Pow(x, k - 1) * Math.Exp(-x / theta) / Math.Pow(theta, k) /
            SpecialFunctions.Gamma.G(k)
            : 0;

    public static double Gamma(double k, double theta, double x) =>
        x >= 0
            ? Math.Pow(x, k - 1) * Math.Exp(-x / theta) / Math.Pow(theta, k) / SpecialFunctions.Gamma.G(k)
            : 0;

    public static Func<double, double> Hi2(int k) => Gamma(k / 2d, 2);

    /// <summary>Распределение Хи-квадрат</summary>
    /// <param name="k">Число степеней свободы</param>
    /// <param name="x">Аргумент</param>
    /// <returns>Значение плотности вероятности</returns>
    public static double Hi2(int k, double x) => Gamma(k / 2d, 2, x);

    public static Func<double, double> NormalGauss(double sigma = 1, double mu = 0) =>
        x =>
        {
            var xx = (x - mu) / sigma;
            return Math.Exp(-0.5 * xx * xx) / (sigma * Consts.sqrt_pi2);
        };

    public static double NormalGauss(double x, double sigma, double mu)
    {
        var xx = (x - mu) / sigma;
        return Math.Exp(-0.5 * xx * xx) / (sigma * Consts.sqrt_pi2);
    }

    public static double NormalDistribution(double x, double sigma = 1, double mu = 0)
    {
        var z = (x - mu) / (sigma * Consts.sqrt_2);
        return 0.5 * (1 + SpecialFunctions.Erf.Value(z));
    }


    /// <summary>Равномерное распределение</summary>
    /// <param name="a">Минимальное значение</param>
    /// <param name="b">Максимальное значение</param>
    /// <returns>Функция равномерного распределения в заданном интервале</returns>
    public static Func<double, double> Uniform(double a, double b) => x => x >= a && x <= b ? 1 / (b - a) : 0;

    /// <summary>Равномерное распределение</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="a">Минимальное значение</param>
    /// <param name="b">Максимальное значение</param>
    /// <returns>Значение равномерного распределения в заданной точке</returns>
    public static double Uniform(double x, double a, double b) => x >= a && x <= b ? 1 / (b - a) : 0;

    public static Func<double, double> Triangular(double a, double b) => x => Triangular(x, a, b);

    /// <summary>Триугольное распределение</summary>
    public static double Triangular(double x, double a, double b) => x >= a && x <= b ? 2 / (b - a) - 2 / ((b - a) * (b - a)) * Math.Abs(a + b - 2 * x) : 0;

    /// <summary>Распределение Рэлея http://ru.wikipedia.org/wiki/Распределение_Рэлея</summary>
    /// <param name="sigma">Параметр масштаба</param>
    /// <returns>Функция распределения для заданного масштаба</returns>
    public static Func<double, double> Rayleigh(double sigma) => x => Rayleigh(x, sigma);

    /// <summary>Распределение Рэлея http://ru.wikipedia.org/wiki/Распределение_Рэлея</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="sigma">Параметр масштаба</param>
    /// <returns>Значение распределения</returns>
    public static double Rayleigh(double x, double sigma)
    {
        var s = x / sigma / sigma;
        return s * Math.Exp(-s * x / 2);
    }

    //
    //private static Func<double, double> HistogramKey(double Max, double Min, double dx) =>
    //    x => (Math.Floor((Math.Min(x, Max - dx / 2) - Min) / dx) + 0.5) * dx + Min;

    public static double GetPirsonsCriteria(this IReadOnlyCollection<double> Samples, Func<double, double> Distribution, out int FreedomDegree)
    {
        if (Samples is null) throw new ArgumentNullException(nameof(Samples));

        var samples_count = Samples.Count;
        if (samples_count == 0) throw new ArgumentException("Массы выборки пуст", nameof(Samples));

        Samples.GetMinMax(out var min, out var max);
        var interval = max - min;

        var dx = interval / (1 + 3.32 * Math.Log10(samples_count)); // interval / (1 + Math.Log(count, 2));
        var segments_count = (int)Math.Floor(interval / dx);
        var histogram_dx = interval / segments_count;

        var counts = new int[segments_count];

        foreach (var sample in Samples)
        {
            var i = Math.Min(counts.Length - 1, (int)Math.Floor((sample - min) / dx));
            counts[i]++;
        }

        //var histogram = Samples
        //   .GroupBy(HistogramKey(max, min, histogram_dx))
        //   .OrderBy(x => x.Key)
        //   .Select(g => (x: g.Key, count: g.Count(), values: g.ToArray()))
        //   .ToArray();


        var sample_average = 0d;

        for (var i = 0; i < segments_count; i++)
            sample_average += (i + 0.5) * dx * counts[i];

        sample_average /= samples_count;

        // Скорректированное среднее
        var varance = 0d;

        for (var i = 0; i < segments_count; i++)
        {
            var x1 = sample_average - (i + 0.5) * dx;
            varance += x1 * x1 * counts[i];
        }

        var restored_variance = varance / (samples_count - 1);

        // Восстановленное СКО
        var restored_sko = Math.Sqrt(restored_variance);

        FreedomDegree = segments_count - 3;

        var criteria = 0d;

        for (var i = 0; i < segments_count; i++)
        {
            var x = (i + 0.5) * dx;
            var u = (x - sample_average) / restored_sko;
            var n0 = samples_count * histogram_dx * Distribution(u) / restored_sko;
            var d = counts[i] - n0;
            criteria += d * d / n0;
        }

        return criteria;
    }

    public static bool CheckDistribution(this double[] Samples, Func<double, double> Distribution, double TrustLevel = 0.95)
    {
        var pirsons_criteria = GetPirsonsCriteria(Samples, Distribution, out var number_of_degrees_of_freedom);
        var hi_theoretical = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(TrustLevel, number_of_degrees_of_freedom);

        return pirsons_criteria < hi_theoretical;
    }
}