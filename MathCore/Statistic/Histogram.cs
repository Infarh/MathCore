using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathCore.Annotations;
using MathCore.Values;
// ReSharper disable ArgumentsStyleOther
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleNamedExpression

namespace MathCore.Statistic
{
    /// <summary>Гистограмма</summary>
    public sealed class Histogram : IEnumerable<ValuedInterval<double>>
    {
        private readonly Interval[] _Intervals;

        private readonly Interval _Interval;

        private readonly double _Normalizer;

        private readonly double[] _Values;

        private readonly double[] _Percent;

        public double Max => _Interval.Max;

        public double Min => _Interval.Min;

        public int N { get; }

        public IReadOnlyList<double> Argument { get; }

        public IReadOnlyList<double> Values => _Values;

        public IReadOnlyList<double> Percent => _Percent;

        public ValuedInterval<double> this[int i]
        {
            get
            {
                var (min, max) = _Intervals[i];
                return new(min, max, _Values[i]);
            }
        }

        public Histogram(IEnumerable<double> X, int IntervalsCount)
        {
            var min_max = new MinMaxValue();
            X = X.ForeachLazy(min_max.SetValue);

            var x_values = X.ToArray();
            N = x_values.Length;

            _Interval = min_max.Interval;

            var dx = min_max.Interval.Length / IntervalsCount;
            var min = min_max.Min;

            //_Intervals = new Interval[IntervalsCount].Initialize(dx, Min, (i, d, min) => new Interval(min + i * d, true, min + (i + 1) * d, false));
            var intervals = new Interval[IntervalsCount];
            _Intervals = intervals;
            var argument = new double[IntervalsCount];
            Argument = argument;
            for (var i = 0; i < IntervalsCount; i++)
            {
                var interval_min = min + i * dx;
                var interval_max = interval_min + dx;
                intervals[i] = new Interval(
                    Min: interval_min,
                    MinInclude: true,
                    Max: interval_max,
                    MaxInclude: i == IntervalsCount - 1);
                argument[i] = (interval_min + interval_max) / 2;
            }

            var values = new double[IntervalsCount];
            _Values = values;

            foreach (var value in x_values)
                for (var i = 0; i < intervals.Length; i++)
                    if (intervals[i].Check(value))
                        values[i]++;


            var percents = new double[values.Length];
            _Percent = percents;
            for (var i = 0; i < percents.Length; i++) 
                percents[i] = values[i] / N;

            var normalizer = values.GetIntegral(argument);
            _Normalizer = normalizer;
            values.Divide(normalizer);
        }

        public bool CheckDistribution(Func<double, double> F, double p, double alpha = 0.05)
        {
            // ReSharper disable once IdentifierTypo
            //var p_teor = new double[_Values.Length].Initialize(_Intervals, F, (i, intervals, f) =>
            //{
            //    var interval = intervals[i];
            //    return f.GetIntegralValue(interval.Min, interval.Max, interval.Length / 20);
            //});

            var p_teor = new double[_Values.Length];
            for (var i = 0; i < p_teor.Length; i++)
            {
                var interval = _Intervals[i];
                p_teor[i] = F.GetIntegralValue(interval.Min, interval.Max, interval.Length / 20);
            }

            //var stat = _Values
            //    .GetMultiplied(_Normalizer / N)
            //    .Select((t, i) => p_teor[i] - t)
            //    .Select((delta, i) => delta * delta / p_teor[i])
            //    .Sum();
            var stat = 0d;
            var q = _Normalizer / N;
            for (var i = 0; i < _Values.Length; i++)
            {
                //var t = _Values[i] * _Normalizer / N;
                //var delta = p_teor[i] - _Values[i] * q;
                stat += (p_teor[i] - _Values[i] * q).Pow2() / p_teor[i];
            }

            var quantile = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(alpha, 2);
            return stat < quantile;
        }

        [NotNull]
        public KeyValuePair<Interval, double>[] GetValues()
        {
            var values = new KeyValuePair<Interval, double>[_Values.Length];
            for (var i = 0; i < values.Length; i++) 
                values[i] = new KeyValuePair<Interval, double>(_Intervals[i], _Values[i]);
            return values;
        }

        [NotNull]
        public KeyValuePair<Interval, double>[] GetPercents() =>
            new KeyValuePair<Interval, double>[_Percent.Length].Initialize(_Intervals, _Percent,
                (i, intervals, percent) => new KeyValuePair<Interval, double>(intervals[i], percent[i]));

        [NotNull]
        private IEnumerable<ValuedInterval<double>> GetEnumerable()
        {
            for (var i = 0; i < _Intervals.Length; i++) 
                yield return new ValuedInterval<double>(_Intervals[i], _Values[i]);
        }

        public IEnumerator<ValuedInterval<double>> GetEnumerator() => GetEnumerable().GetEnumerator();

        public override string ToString() => string.Join(" ", GetEnumerable());

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