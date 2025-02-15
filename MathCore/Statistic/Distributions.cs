﻿#nullable enable
namespace MathCore.Statistic;

/// <summary>Распределения</summary>
public static partial class Distributions
{
    /// <summary>
    ///     Гамма-распределение.
    ///     f(x) = (x^(k-1) * e^(-x/theta)) / (theta^k * G(k))
    /// </summary>
    /// <param name="k">Параметр k</param>
    /// <param name="theta">Параметр theta</param>
    /// <returns>Функция Гамма-распределения</returns>
    public static Func<double, double> Gamma(double k, double theta) =>
        x => x >= 0
            ? Math.Pow(x, k - 1) * Math.Exp(-x / theta) / Math.Pow(theta, k) /
            SpecialFunctions.Gamma.G(k)
            : 0;

    /// <summary>
    /// Гамма-распределение.
    /// f(x) = (x^(k-1) * e^(-x/theta)) / (theta^k * G(k))
    /// </summary>
    /// <param name="k">Параметр k</param>
    /// <param name="theta">Параметр theta</param>
    /// <param name="x">Аргумент</param>
    /// <returns>Значение плотности вероятности</returns>
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

    /// <summary>
    /// Генерирует функцию плотности вероятности нормального распределения Гаусса.
    /// </summary>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Функция плотности вероятности нормального распределения</returns>
    public static Func<double, double> NormalGauss(double sigma = 1, double mu = 0) =>
        x =>
        {
            var xx = (x - mu) / sigma;
            return Math.Exp(-0.5 * xx * xx) / (sigma * Consts.sqrt_pi2);
        };

    /// <summary>Нормальное распределение (гауссово).</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Значение плотности вероятности</returns>
    public static double NormalGauss(double x, double sigma, double mu)
    {
        var xx = (x - mu) / sigma;
        return Math.Exp(-0.5 * xx * xx) / (sigma * Consts.sqrt_pi2);
    }

    /// <summary>Функция распределения нормального распределения</summary>
    /// <param name="x">Аргумент</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Значение функции распределения нормального распределения в заданной точке</returns>
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

    /// <summary>
    ///     Вычисление критерия Пирсона для сравнения выборки с заданным распределением
    /// </summary>
    /// <param name="Samples">Выборка</param>
    /// <param name="Distribution">Заданное распределение</param>
    /// <param name="FreedomDegree">Степени свободы</param>
    /// <param name="Average">Среднее значение выборки</param>
    /// <param name="RestoredVariance">Восстановленное СКО</param>
    /// <returns>Критерий Пирсона</returns>
    public static double GetPirsonsCriteria(
        this IReadOnlyCollection<double> Samples, 
        Func<double, double> Distribution, 
        out int FreedomDegree,
        out double Average,
        out double RestoredVariance)
    {
        var samples_count = Samples.NotNull().Count;
        if (samples_count == 0) throw new ArgumentException("Массы выборки пуст", nameof(Samples));

        Samples.GetMinMax(out var min, out var max);
        var interval = max - min;

        var dx             = interval / (1 + 3.32 * Math.Log10(samples_count)); // interval / (1 + Math.Log(count, 2));
        var segments_count = (int)Math.Round(interval / dx);
        var histogram_dx   = interval / segments_count;

        var histogram = new int[segments_count];

        foreach (var sample in Samples)
        {
            var i = Math.Min(histogram.Length - 1, (int)Math.Floor((sample - min) / dx));
            histogram[i]++;
        }

        Average = 0d;

        for (var i = 0; i < segments_count; i++)
            Average += (min + (i + 0.5) * dx) * histogram[i];

        Average /= samples_count;

        // Скорректированное среднее
        var variance_sum = 0d;

        for (var i = 0; i < segments_count; i++)
        {
            var x1 = Average - (min + (i + 0.5) * dx);
            variance_sum += x1 * x1 * histogram[i];
        }

        //var variance = variance_sum / samples_count;
        RestoredVariance = variance_sum / (samples_count - 1);

        // Восстановленное СКО
        var restored_sko = Math.Sqrt(RestoredVariance);

        FreedomDegree = segments_count - 3;

        var criteria = 0d;

        for (var i = 0; i < segments_count; i++)
        {
            var n_actual = histogram[i];

            var x = min + (i + 0.5) * dx;
            var p_expected = histogram_dx * Distribution(x);
            var n_expected = p_expected * samples_count;

            var delta = n_actual - n_expected;
            var delta_relative = delta * delta / n_expected;

            criteria += delta_relative;
        }

        return criteria;
    }

    /// <summary>Проверяет, является ли выборка реализацией данного нормального распределения</summary>
    /// <param name="Samples">Выборка</param>
    /// <param name="Distribution">Нормальное распределение</param>
    /// <param name="TrustLevel">Уровень доверия (0.95 - 95%)</param>
    /// <returns>true, если выборка является реализацией данного нормального распределения, false - если нет</returns>
    /// <remarks>
    ///     Метод использует критерий Пирсона.
    ///     Расчёт критерия Пирсона выполняется функцией <see cref="GetPirsonsCriteria"/>
    ///     Теоретическое значение критерия Пирсона берётся из таблицы квантилей распределения Стьюдента
    /// </remarks>
    public static bool CheckDistribution(this double[] Samples, Func<double, double> Distribution, double TrustLevel = 0.95)
    {
        var pirsons_criteria = GetPirsonsCriteria(Samples, Distribution, out var number_of_degrees_of_freedom, out _, out _);
        var hi_theoretical = SpecialFunctions.Distribution.Student.QuantileHi2(TrustLevel, number_of_degrees_of_freedom);

        return pirsons_criteria < hi_theoretical;
    }
}