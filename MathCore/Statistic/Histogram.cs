﻿#nullable enable
using System.Collections;
using System.Globalization;
using System.Text;

using MathCore.Values;

using static MathCore.Statistic.Histogram;
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore.Statistic;

/// <summary>Гистограмма</summary>
public sealed class Histogram : IEnumerable<HistogramValue>
{
    /// <summary>Значение гистограммы</summary>
    public readonly struct HistogramValue
    {
        /// <summary>Интервал значений</summary>
        public Interval Interval { get; init; }

        /// <summary>Частота</summary>
        public double Value { get; init; }

        /// <summary>Нормированная частота</summary>
        public double NormalValue { get; init; }

        public int IntegralCount { get; init; }

        public double IntegralValue { get; init; }

        public int Count { get; init; }

        public override string ToString() => $"{Interval}:{Value}({Count}):{NormalValue}";

        public string ToString(string Format) => $"{Interval.ToString(Format)}:{Value.ToString(Format)}({Count}):{NormalValue.ToString(Format)}";

        public void Deconstruct(out Interval Interval, out int Count) => (Interval, Count) = (this.Interval, this.Count);
        public void Deconstruct(out Interval Interval, out int Count, out double Value) => (Interval, Count, Value) = (this.Interval, this.Count, this.Value);
        public void Deconstruct(out Interval Interval, out int Count, out double Value, out double NormalValue) => (Interval, Count, Value, NormalValue) = (this.Interval, this.Count, this.Value, this.NormalValue);
    }

    private readonly int _IntervalsCount;

    //private readonly Interval[] _Intervals;

    private readonly Interval _Interval;

    private readonly double _dx;

    private readonly double _Normalizer;

    private readonly int[] _Counts;

    private readonly double[] _Frequencies;

    private readonly double _Mean;

    private readonly double _Variance;

    public int IntervalsCount => _IntervalsCount;

    public Interval Interval => _Interval;

    public double dx => _dx;

    public int TotalValuesCount { get; }

    public double Mean => _Mean;

    public double Variance => _Variance;

    public double StdDev => _Variance.Sqrt();

    public double StdErr => StdDev / Math.Sqrt(TotalValuesCount);

    public IReadOnlyList<double> Frequencies => _Frequencies;

    public HistogramValue this[int i]
    {
        get
        {
            if (i < 0 || i >= _IntervalsCount)
                throw new ArgumentOutOfRangeException(nameof(i), i, "Номер интервала вне диапазона [0..IntervalsCount-1]")
                {
                    Data =
                    {
                        { nameof(IntervalsCount), _IntervalsCount }
                    }
                };

            var min = i * dx + _Interval.Min;
            return new()
            {
                Interval = new(min, true, min + dx, i == _IntervalsCount - 1),
                Value = _Frequencies[i],
                NormalValue = _Frequencies[i] / _Normalizer,
                Count = _Counts[i],
                IntegralValue = double.NaN,
                IntegralCount = 0,
            };
        }
    }

    public Histogram(IReadOnlyCollection<double> X) : this(X, (int)Math.Floor(1 + Math.Log(X.Count, 2))) { }

    /// <summary>Инициализация гистограммы</summary>
    /// <param name="X">Перечисление значений, для которых будет вычисляться гистограмма.</param>
    /// <param name="IntervalsCount">Количество интервалов, на которые будет разбита гистограмма.</param>
    public Histogram(IEnumerable<double> X, int IntervalsCount)
    {
        _IntervalsCount = IntervalsCount;

        var min_max = new MinMaxValue();
        var average_value = new AverageValue();

        if (X is double[] x_values)
            foreach (var x in x_values)
            {
                min_max.AddValue(x);
                average_value.AddValue(x);
            }
        else
        {
            var list = new List<double>();

            foreach (var x in X)
            {
                min_max.AddValue(x);
                average_value.AddValue(x);
                list.Add(x);
            }

            x_values = [.. list];
        }

        var total_values_count = x_values.Length;
        TotalValuesCount = total_values_count;

        _Interval = min_max.Interval;
        (_Mean, _Variance) = average_value;

        var dx = min_max.Interval.Length / IntervalsCount;
        _dx = dx;
        var min = min_max.Min;

        var values = new int[IntervalsCount];
        _Counts = values;

        foreach (var value in x_values)
        {
            var index = Math.Min((int)((value - min) / dx), IntervalsCount - 1);
            values[index]++;
        }

        var frequencies = new double[IntervalsCount];
        _Frequencies = frequencies;
        for (var i = 0; i < IntervalsCount; i++)
            frequencies[i] = (double)values[i] / total_values_count;

        _Normalizer = frequencies.GetIntegral(dx);
    }

    /// <summary>Вычислить статистику Пирсона</summary>
    /// <param name="Distribution">Распределение</param>
    /// <returns>Статистика Пирсона</returns>
    public double GetPirsonsCriteria(Func<double, double> Distribution)
    {
        var x0 = _Interval.Min;
        var dx = _dx;

        var stat = 0d;
        for (var i = 0; i < _IntervalsCount; i++)
        {
            var min = x0 + i * dx;
            var max = min + dx;

            var p = Distribution.GetIntegralValue_AdaptiveTrapRecursive(min, max);
            var f = _Frequencies[i];
            var dp = p - f;
            stat += dp * dp / p;
        }

        return stat * TotalValuesCount;
    }

    /// <summary>Проверяет, является ли выборка реализацией данного нормального распределения</summary>
    /// <param name="Distribution">Нормальное распределение</param>
    /// <param name="alpha">Уровень доверия (0.95 - 95%)</param>
    /// <returns>true, если выборка является реализацией данного нормального распределения, false - если нет</returns>
    public bool CheckDistribution(Func<double, double> Distribution, double alpha = 0.05)
    {
        var stat = GetPirsonsCriteria(Distribution);
        var quantile = SpecialFunctions.Distribution.Student.QuantileHi2(alpha, _IntervalsCount - 3);
        return stat < quantile;
    }

    /// <summary>Получить перечисление значений гистограммы</summary>
    /// <returns>Перечисление значений гистограммы</returns>
    /// <remarks>
    ///     Метод возвращает перечисление с информацией о каждом интервале гистограммы:
    ///     <list type="bullet">
    ///         <item><description>Интервал</description></item>
    ///         <item><description>Значение</description></item>
    ///         <item><description>Нормализованное значение</description></item>
    ///         <item><description>Количество значений в интервале</description></item>
    ///         <item><description>Суммарное значение</description></item>
    ///         <item><description>Количество значений в интервале</description></item>
    ///     </list>
    /// </remarks>
    private IEnumerable<HistogramValue> GetEnumerable()
    {
        var x0 = _Interval.Min;
        var dx = _dx;
        var integral_value = 0d;
        var integral_count = 0;
        for (var i = 0; i < _IntervalsCount; i++)
        {
            var frequency = _Frequencies[i];
            var count = _Counts[i];
            var min = x0 + i * dx;
            integral_value += frequency;
            integral_count += count;
            yield return new()
            {
                Interval = new(min, true, min + dx, i == _IntervalsCount - 1),
                Value = frequency,
                NormalValue = frequency / _Normalizer,
                Count = count,
                IntegralValue = integral_value,
                IntegralCount = integral_count,
            };
        }
    }

    public IEnumerator<HistogramValue> GetEnumerator() => GetEnumerable().GetEnumerator();

    public override string ToString() => string.Join(" ", GetEnumerable());

    /// <summary>Выводит текстовое представление гистограммы в указанный поток</summary>
    /// <param name="Writer">Текстовый поток для вывода данных</param>
    /// <param name="Width">Ширина вывода (по умолчанию 50, не менее 40)</param>
    /// <param name="PrintInfo">Флаг, указывающий выводить ли дополнительную информацию о гистограмме</param>
    /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если ширина меньше 40</exception>
    public void Print(TextWriter Writer, int Width = 50, bool PrintInfo = true)
    {
        if (Width < 40) throw new ArgumentOutOfRangeException(nameof(Width), Width, "Ширина не должна быть меньше 40");

        var xx_str = new string[_IntervalsCount];
        var dx = _dx;
        var (min, max) = _Interval;
        var invariant_culture = CultureInfo.InvariantCulture;
        for (var i = 0; i < _IntervalsCount; i++)
            xx_str[i] = ((i + 0.5) * dx + min).ToString(" 0.00000000000000;-0.00000000000000", invariant_culture);

        var max_xx_str_length = xx_str.Max(s => s.Length);
        var offset_str = new string(' ', max_xx_str_length);

        Writer.WriteLine($"dx:{dx.ToString(invariant_culture)}");
        Writer.WriteLine($"min x:{min.ToString(invariant_culture)}");
        Writer.WriteLine("{0}┌{1}►", offset_str, new string('─', Width - 2 - max_xx_str_length));

        var counts_max_digits_count = (int)Math.Log10(_Counts.Max());
        var bar_max_width = Width - 3 - counts_max_digits_count - max_xx_str_length;
        for (var i = 0; i < _IntervalsCount; i++)
        {
            Writer.Write("{0}│", xx_str[i].PadRight(max_xx_str_length));

            var bar_width_int = (int)(bar_max_width * _Counts[i] / (double)_Counts.Max() - 1);
            for (var j = 0; j < bar_width_int; j++)
                Writer.Write('█');

            Writer.WriteLine("▌{0}", _Counts[i]);
        }

        Writer.WriteLine("{0}▼", offset_str);
        Writer.WriteLine("{0}x", offset_str);

        if (!PrintInfo)
        {
            Writer.WriteLine($"max x : {max.ToString(invariant_culture)}");
            return;
        }

        var sgm = StdDev;
        var mu_sgm = new Interval(_Mean - sgm, _Mean + sgm);
        var mu_3sgm = new Interval(_Mean - 3 * sgm, _Mean + 3 * sgm);

        Writer.WriteLine($"           min x : {min.ToString(invariant_culture)}");
        Writer.WriteLine($"           max x : {max.ToString(invariant_culture)}");
        Writer.WriteLine($" Intervals count : {_IntervalsCount}");
        Writer.WriteLine($"    Values count : {TotalValuesCount}");
        Writer.WriteLine($"               μ : {_Mean.ToString(invariant_culture)}");
        Writer.WriteLine($"               D : {_Variance.ToString(invariant_culture)}");
        Writer.WriteLine($"               σ : {sgm.ToString(invariant_culture)}");
        Writer.WriteLine($"             err : {StdErr.ToString(invariant_culture)}");
        Writer.WriteLine($"            μ±σ  : {mu_sgm.ToString(invariant_culture)}");
        Writer.WriteLine($"            μ±3σ : {mu_3sgm.ToString(invariant_culture)}");

    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public string ToString(string Format)
    {
        var result = new StringBuilder();
        foreach (var interval in GetEnumerable())
            result.Append(interval.ToString(Format)).Append(" ");

        if (result.Length > 0)
            result.Length--;
        return result.ToString();
    }

    public string ToString(string IntervalFormat, string ValueFormat)
    {
        var result = new StringBuilder();
        foreach (var (interval, _, value) in GetEnumerable())
            result
               .Append(interval.ToString(IntervalFormat))
               .Append(value.ToString(ValueFormat))
               .Append(" ");

        if (result.Length > 0)
            result.Length--;
        return result.ToString();
    }
}