using System;
using System.Collections.Generic;
using System.Linq;

using MathCore;
using MathCore.Statistic;
using MathCore.Statistic.RandomNumbers;

using Microsoft.Data.Analysis;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest
{
    internal static class EnumerableEx
    {
        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> Items, int count)
        {
            var items = new T[count];
            var index = 0;
            foreach (var item in Items)
            {
                if (index >= count)
                    yield return items[(index - count) % count];

                items[index % count] = item;
                index++;
            }
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var random = new Random(100);

            const double sigma = 5.0;
            const double mu = 3.0;
            const int count = 1000;

            var rnd = new NormalRandomGenerator(sigma, mu, random);

            var values = rnd.GetValues(count);

            var mu_actual = values.Average();
            var mu2_actual = mu_actual.Pow2();
            var sigma_actual = values.Average(x => x.Pow2() - mu2_actual).Sqrt();
            var (min, max) = values.GetMinMax();

            var hist = values.Hystogram(1).ToArray();

            var check = hist
               .ToArray(h =>
                {
                    var x = h.x.Center();
                    var f = Distributions.NormalGauss(x, sigma, mu);
                    return $"x:{x} h:{h.value} f:{f:f4} d:{h.value - f:f4} d2:{(h.value - f).Pow2():f4} d2r:{(h.value - f).Pow2() / f:f4}";
                });

            //var dd = Distributions.NormalDestribution(0);

            var d2r_s = 0d;
            var O = 0;
            Console.WriteLine("   x       h         f         d        d2r     O    E  (O-E)  (O-E)^2/E");
            Console.WriteLine("------------------------------------------------------------------------");
            foreach (var (interval, value) in hist)
            {
                var x = interval.Center();
                var f = Distributions.NormalGauss(x, sigma, mu);
                var d = value - f;
                var d2 = d * d;
                var d2r = d2 / f;
                d2r_s += d2r;
                var o = (int)(count * value);
                O += o;
                var e = (int)Math.Round(count * f);
                var oe = o - e;
                var oe2e = oe.Pow2() / (double)e;
                Console.WriteLine("{0,5:f1}   {1,-6:0.####}   {2,-6:0.####}   {3,7:0.####}   {4,-8:0.######}   {5,2}   {6,2}   {7,3}   {8,-8:0.######}",
                    x, value, f, d, d2r, o, e, oe, oe2e);
            }
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("                                    {0:0.######}   {1}", d2r_s, O);

            var distrib = Distributions.NormalGauss(sigma, mu);

            var hi2 = count * hist
               .Select(h =>
                {
                    var (min, max) = h.x;
                    var p = distrib.GetIntegralValue_Adaptive(min, max, Eps: 1e-9);
                    return (h.value - p).Pow2() / p;
                })
               .Sum();
            var hi21 = count * hist
               .Select(h =>
                {
                    var (min, max) = h.x;
                    var p = Distributions.NormalDestribution(max, sigma, mu) - Distributions.NormalDestribution(min, sigma, mu);
                    return (h.value - p).Pow2() / p;
                })
               .Sum();

            var q = SpecialFunctions.Distribution.Student.QuantileHi2(0.95, hist.Length - 2);

            var mu_err_abs = mu_actual - mu;
            var sigma_err_abs = sigma_actual - sigma;

            var mu_err_rel = mu_err_abs.Abs() / mu;
            var sigma_err_rel = sigma_err_abs.Abs() / sigma;

            Console.WriteLine();
            Console.WriteLine("   mu:{0,8:f3}   d:{1,7:f3} r:{2,-6:0.####} p:{2:p}", mu_actual, mu_err_abs, mu_err_rel);
            Console.WriteLine("sigma:{0,8:f3}   d:{1,7:f3} r:{2,-6:0.####} p:{2:p}", sigma_actual, sigma_err_abs, sigma_err_rel);
            Console.WriteLine("  min:{0,8:f3} max:{1,7:f3}", min, max);
            Console.WriteLine("delta:{0,8:f3}", max - min);
        }

        private static IReadOnlyList<((double min, double max) x, int count)> GetClasses(
            this IEnumerable<double> values,
            double dx) =>
            values
               .GroupBy(x => Math.Round(x / dx) * dx)
               .OrderBy(h => h.Key)
               .Select(h => ((h.Key - dx / 2, h.Key + dx / 2), h.Count()))
               .ToArray();

        private static IReadOnlyList<((double min, double max) x, int count)> OptimizeClasses(
            this IReadOnlyList<((double min, double max) x, int count)> Classes,
            int CountTreshold = 50)
        {
            var classes_count = Classes.Count;
            var result = new List<((double min, double max) x, int count)>(classes_count);

            var last_class = Classes[0];
            for (var i = 1; i < classes_count; i++)
            {
                var current_class = Classes[i];

                if (last_class.count >= CountTreshold)
                {
                    result.Add(last_class);
                    last_class = current_class;
                    continue;
                }

                last_class = ((last_class.x.min, current_class.x.max), last_class.count + current_class.count);
            }

            if (last_class.count >= CountTreshold)
                result.Add(last_class);
            else
                result[^1] = result[^1].Add(last_class);

            result.TrimExcess();
            return result;
        }

        private static ((double min, double max) x, int count) Add(
            this ((double min, double max) x, int count) h1,
            ((double min, double max) x, int count) h2) =>
            ((h1.x.min, h2.x.max), h1.count + h2.count);

        private static IEnumerable<((double min, double max) x, double value)> Hystogram(
            this IEnumerable<double> values,
            double dx,
            int CountThreshold = 5)
        {
            var classes = values.GetClasses(dx).OptimizeClasses(CountThreshold);
            var total_count = classes.Sum(h => h.count);
            return classes.Select(h => (h.x, (double)h.count / total_count));
        }

        private static double Delta(this (double min, double max) x) => x.max - x.min;

        private static double Center(this (double min, double max) x) => (x.max + x.min) / 2;

        private static (double center, double delta) CenterDelta(this (double min, double max) x) =>
            (x.Center(), x.Delta());

        private static void PrimitiveDataFrameColumnTest()
        {
            var times = new PrimitiveDataFrameColumn<DateTime>("Date");
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(2)));
            times.Append(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
            times.Append(DateTime.Now);

            var ints = new PrimitiveDataFrameColumn<int>("ints", 3);
            var strs = new StringDataFrameColumn("strings", 3);

            var data = new DataFrame(times, ints, strs) { [0, 1] = 10 };
        }
    }
}