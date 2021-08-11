using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathCore.Annotations;
using MathCore.Values;

namespace MathCore.Statistic
{
    /// <summary>Гистограмма</summary>
    public sealed class Histogram : IEnumerable<ValuedInterval<double>>
    {
        public readonly double[] Argument;
        private readonly Interval[] _Intervals;
        private readonly Interval _Interval;
        private readonly double _Normalizer;
        public double Max => _Interval.Max;
        public double Min => _Interval.Min;
        public readonly int N;
        public readonly double[] Values;
        public readonly double[] Percent;

        public ValuedInterval<double> this[int i] => new(_Intervals[i].Min, _Interval.Max, Values[i]);

        public Histogram(IEnumerable<double> X, int IntervalsCount)
        {
            var min_max = new MinMaxValue();
            X = X.ForeachLazy(min_max.SetValue);


            var x_values = X.ToArray();
            N = x_values.Length;

            _Interval = min_max.Interval;

            var dx = (Max - Min) / IntervalsCount;

            _Intervals = new Interval[IntervalsCount].Initialize(dx, Min, (i, d, min) => new Interval(min + i * d, true, min + (i + 1) * d, false));
            _Intervals[IntervalsCount - 1] = _Intervals[IntervalsCount - 1].IncludeMax(true);

            Argument = _Intervals.Select(I => I.Middle).ToArray();
            Values = new double[IntervalsCount];

            void AddValue(double x) => _Intervals.Foreach(Values, x, (interval, i, values, xx) => { if (interval.Check(xx)) values[i]++; });

            x_values.Foreach(AddValue);
            Percent = new double[Values.Length].Initialize(Values, N, (i, values, n) => values[i] / n);
            Values.Divide(_Normalizer = Values.GetIntegral(Argument));
        }

        public bool CheckDistribution(Func<double, double> F, double p, double alpha = 0.05)
        {
            // ReSharper disable once IdentifierTypo
            var p_teor = new double[Values.Length].Initialize(_Intervals, F, (i, intervals, f) =>
            {
                var interval = intervals[i];
                return f.GetIntegralValue(interval.Min, interval.Max, interval.Length / 20);
            });

            var stat = Values
                .GetMultiplied(_Normalizer / N)
                .Select((t, i) => p_teor[i] - t)
                .Select((delta, i) => delta * delta / p_teor[i])
                .Sum();
            var quantile = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(alpha, 2);
            Console.WriteLine(stat < quantile);
            Console.ReadLine();
            return stat < quantile;
        }

        [NotNull] public KeyValuePair<Interval, double>[] GetValues() => 
            new KeyValuePair<Interval, double>[Values.Length].Initialize(_Intervals, Values,
                (i, intervals, values) => new KeyValuePair<Interval, double>(intervals[i], values[i]));

        [NotNull] public KeyValuePair<Interval, double>[] GetPercents() => 
            new KeyValuePair<Interval, double>[Percent.Length].Initialize(_Intervals, Percent,
                (i, intervals, percent) => new KeyValuePair<Interval, double>(intervals[i], percent[i]));

        [NotNull]
        private IEnumerable<ValuedInterval<double>> GetIEnumerable()
        {
            for(var i = 0; i < _Intervals.Length; i++)
                yield return this[i];
        }

        public IEnumerator<ValuedInterval<double>> GetEnumerator() => GetIEnumerable().GetEnumerator();

        public override string ToString() => GetValues()
            .Aggregate(
                new StringBuilder(),
                (sb, v) => sb.AppendFormat("{0}:{1} ", v.Key, v.Value),
                sb => sb.ToString().RemoveFromEnd(1));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public string ToString(string Format) => GetValues()
            .Aggregate(
                new StringBuilder(),
                (sb, v) => sb.AppendFormat("{0}:{1} ", v.Key, v.Value.ToString(Format)),
                sb => sb.ToString().RemoveFromEnd(1));

        public string ToString(string IntervalFormat, string ValueFormat) => GetValues()
            .Aggregate(
                new StringBuilder(),
                (sb, v) => sb.AppendFormat("{0}:{1} ", v.Key.ToString(IntervalFormat), v.Value.ToString(ValueFormat)),
                sb => sb.ToString().RemoveFromEnd(1));
    }
}