using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MathCore.Annotations;
using MathCore.Values;

using static MathCore.Statistic.Histogram;
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.Statistic
{
    /// <summary>Гистограмма</summary>
    public sealed class Histogram : IEnumerable<HistogramValue>
    {
        public readonly struct HistogramValue
        {
            public Interval Interval { get; init; }

            public double Value { get; init; }

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

        public int IntervalsCount => _IntervalsCount;

        public Interval Interval => _Interval;

        public double dx => _dx;

        public int TotalValuesCount { get; }

        //public IReadOnlyList<double> Argument { get; }

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
                    Interval = new Interval(min, true, min + dx, i == _IntervalsCount - 1),
                    Value = _Frequencies[i],
                    NormalValue = _Frequencies[i] / _Normalizer,
                    Count = _Counts[i],
                    IntegralValue = double.NaN,
                    IntegralCount = 0,
                };
            }
        }

        public Histogram(IEnumerable<double> X, int IntervalsCount)
        {
            _IntervalsCount = IntervalsCount;

            var min_max = new MinMaxValue();
            X = X.ForeachLazy(min_max.SetValue);

            var x_values = X.ToArray();
            var total_values_count = x_values.Length;
            TotalValuesCount = total_values_count;

            _Interval = min_max.Interval;

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

        //public bool CheckDistribution(Func<double, double> F, double p, double alpha = 0.05)
        //{
        //    // ReSharper disable once IdentifierTypo
        //    //var p_teor = new double[_Values.Length].Initialize(_Intervals, F, (i, intervals, f) =>
        //    //{
        //    //    var interval = intervals[i];
        //    //    return f.GetIntegralValue(interval.Min, interval.Max, interval.Length / 20);
        //    //});

        //    var p_teor = new double[_Values.Length];
        //    for (var i = 0; i < p_teor.Length; i++)
        //    {
        //        var interval = _Intervals[i];
        //        p_teor[i] = F.GetIntegralValue(interval.Min, interval.Max, interval.Length / 20);
        //    }

        //    //var stat = _Values
        //    //    .GetMultiplied(_Normalizer / N)
        //    //    .Select((t, i) => p_teor[i] - t)
        //    //    .Select((delta, i) => delta * delta / p_teor[i])
        //    //    .Sum();
        //    var stat = 0d;
        //    var q = _Normalizer / TotalValuesCount;
        //    for (var i = 0; i < _Values.Length; i++)
        //    {
        //        //var t = _Values[i] * _Normalizer / N;
        //        //var delta = p_teor[i] - _Values[i] * q;
        //        stat += (p_teor[i] - _Values[i] * q).Pow2() / p_teor[i];
        //    }

        //    var quantile = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(alpha, 2);
        //    return stat < quantile;
        //}

        [NotNull]
        private IEnumerable<HistogramValue> GetEnumerable()
        {
            var x0 = _Interval.Min;
            var dx = _dx;
            var integral_value = 0d;
            var integral_count = 0;
            for (var i = 0; i < _IntervalsCount; i++)
            {
                //yield return new ValuedInterval<double>(_Intervals[i], _Values[i]);
                var frequency = _Frequencies[i];
                var count = _Counts[i];
                var min = x0 + i * dx;
                integral_value += frequency;
                integral_count += count;
                yield return new HistogramValue
                {
                    Interval = new Interval(min, true, min + dx, i == _IntervalsCount - 1),
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

        public void Print(TextWriter Writer)
        {

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
            foreach (var v in GetEnumerable())
                result
                   .Append(v.Interval.ToString(IntervalFormat))
                   .Append(v.Value.ToString(ValueFormat))
                   .Append(" ");

            if (result.Length > 0)
                result.Length--;
            return result.ToString();
        }
    }
}