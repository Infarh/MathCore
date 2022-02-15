using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

using ConsoleTest.Extensions;

using MathCore;
using MathCore.Statistic;

using OxyPlot.Axes;
using OxyPlot.Series;

using OxyPlot;
using OxyPlot.Annotations;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest;

internal static class Program
{
    private static readonly ConcurrentDictionary<(Type, string), Delegate> __Getters = new();
    public static T GetValue<T>(object source, string PropertyName)
    {
        var getter = (Func<object, T>)__Getters.GetOrAdd(
            (typeof(T), PropertyName), v =>
            {
                var (type, property_name) = v;
                var parameter = Expression.Parameter(typeof(object), "p");
                var cast = Expression.TypeAs(parameter, type);
                var property = Expression.Property(cast, property_name);
                var expression = Expression.Lambda<Func<object, T>>(property, parameter);
                return expression.Compile();
            });
        return getter(source);
    }

    private static RecursionResult<BigInteger> Factorial(int n, BigInteger product) =>
        n < 2
            ? TailRecursion.Return(product)
            : TailRecursion.Next(() => Factorial(n - 1, n * product));

    public static void QuickSort(int[] A) => QuickSort(A, 0, A.Length - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Swap(ref int A, ref int B)
    {
        var tmp = A;
        A = B;
        B = tmp;
    }

    // [1, 14, 5, 7, 0, 8, 6]
    public static void QuickSort(int[] A, int low, int high)
    {
        var i = low;  // i -  левая граница интервала сортировки
        var j = high; // j - правая граница интервала сортировки
        // Захватываем элемент из середины интервала
        var x = A[(low + high) / 2];  // x - опорный элемент посредине между low и high
        do
        {
            // Сдвигаем левую границу вправо до тех пор, пока элемент на левой границе < x
            while (A[i] < x) i++;  // поиск элемента для переноса в старшую часть
            // Сдвигаем правую границу влево до тех пор, пока элемент на левой границе > x
            while (A[j] > x) j--;  // поиск элемента для переноса в младшую часть

            if (i > j) break;

            // обмен элементов местами:
            var tmp = A[i];
            A[i] = A[j];
            A[j] = tmp;

            // переход к следующим элементам:
            i++; // Двигаем левую  границу вправо
            j--; // Двигаем правую границу влево
        } while (i < j); // Делаем всё это до тех пор, пока границы не пересекутся

        if (low < j) QuickSort(A, low, j);
        if (i < high) QuickSort(A, i, high);
    }

    public static int Fib(int n)
    {
        if (n == 0) return 1;
        if (n == 1) return 1;
        return Fib(n - 1) + Fib(n - 2);
    }

    public static void Fib(int[] F)
    {
        for (var i = 0; i < F.Length; i++)
            if (i == 0)
                F[i] = 1;
            else if (i == 1)
                F[i] = 1;
            else F[i] =
                F[i - 1] + F[i - 2];
    }

    public static IEnumerable<int> EnumFib()
    {
        yield return 1;
        yield return 1;
        var last1 = 1;
        var last2 = 1;
        while (true)
        {
            var fib = last1 + last2;
            yield return fib;
            last1 = last2;
            last2 = fib;
        }
    }

    private static double Multiply(double[] a, double[] b, int BOffset)
    {
        var result = 0d;
        var count = 0;
        var a_len = a.Length;
        var b_len = b.Length;

        for (var i = 0; i < b_len; i++)
            if (i - BOffset is >= 0 and var a_index && a_index < a_len)
            {
                count++;
                result += a[a_index] * b[i];
            }

        return count > 0 ? result / count : double.NaN;
    }

    private static void TestHist()
    {
        var rnd = new Random(5);

        const double D = 5;
        const double m = 3;
        const int count = 100000;
        var values = rnd.NextNormal(count, D, m);
        var gauss = Distributions.NormalGauss(D, m);
        var gauss0 = Distributions.NormalGauss(D, m + 0.1);

        const int intervals_count = 17;
        var histogram = new Histogram(values, intervals_count);

        var pirson = histogram.GetPirsonsCriteria(gauss);
        var pirson0 = histogram.GetPirsonsCriteria(gauss0);

        var q1 = SpecialFunctions.Distribution.Student.QuantileHi2Approximation(0.95, intervals_count - 2);
        var q2 = SpecialFunctions.Distribution.Student.QuantileHi2(0.95, intervals_count - 2);
        var interval = histogram.Interval;
        const int function_points_count = 1000;
        var model = new PlotModel
        {
            Background = OxyColors.White,
            Series =
            {
                new HistogramSeries
                {
                    FillColor = OxyColors.Blue,
                    StrokeColor = OxyColors.DarkBlue,
                    StrokeThickness = 1,
                    ItemsSource = histogram,
                    Mapping = o =>
                    {
                        var ((min, max), n, value, normal_value) = (Histogram.HistogramValue)o;
                        return new HistogramItem(min, max, value, 0);
                    },
                },
                new FunctionSeries(gauss, interval.Min, interval.Max, interval.Length / function_points_count)
                {
                    Color = OxyColors.Red
                },
            },
            Annotations =
            {
                new LineAnnotation
                {
                    X = histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.DarkRed,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Solid,
                    Text = "μ"
                },
                new LineAnnotation
                {
                    X = histogram.StandardDeviation + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "σ"
                },
                new LineAnnotation
                {
                    X = -histogram.StandardDeviation + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "σ"
                },
                new LineAnnotation
                {
                    X = histogram.StandardDeviation * 3 + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "3σ"
                },
                new LineAnnotation
                {
                    X = -histogram.StandardDeviation * 3 + histogram.Mean,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Red,
                    StrokeThickness = 2,
                    LineStyle = LineStyle.Dash,
                    Text = "3σ"
                },
            }

        };


        var result = histogram.CheckDistribution(gauss);

        model.ToPNG("image.png").ShowInExplorer();
    }

    private static void Main()
    {
        TestHist();


        //string[] lines =
        //{
        //    "123.",
        //    "..\"Hello,.World!,.QWE\".",
        //    ".Value.",
        //    "\"123,23",
        //};

        //var line = string.Join(',', lines);
        //var line_array = line.ToCharArray().AsMemory();
        //var values = CSVParseTest.ParseLine(line_array).ToList();

        //var segment_start = new MemorySegment<char>(values[1]);
        //var segment_end = segment_start.Append(values[3]);

        //var pp = new Pipe();

        //Writer(pp.Writer);

        //static void Writer(PipeWriter writer)
        //{

        //}

        //var seq = new ReadOnlySequence<char>(segment_start, 0, segment_end, segment_end.Memory.Length);

        //var sss = seq.ToString();

        //Debug.WriteLine(sss);

        //var size = 1000;
        //var arr_a = GC.AllocateUninitializedArray<double>(size).Initialize(i => Math.Sin(4 * Math.PI / size * i));
        //var arr_b = GC.AllocateUninitializedArray<double>(size).Initialize(i => Math.Cos(4 * Math.PI / size * i));

        //var correlations = new List<(int offset, double value)>(size * 2);
        //for (var offset = -size; offset <= size; offset++)
        //{
        //    correlations.Add((offset, Multiply(arr_a, arr_b, offset)));
        //}

        //var fff = SpecialFunctions.Fibonacci(7);

        //var ff = EnumFib().ElementAt(8);

        //var fib = new int[10];
        //for (var m = 0; m < fib.Length; m++)
        //{
        //    fib[m] = Fib(m);
        //}

        //Array.Clear(fib, 0, fib.Length);
        //Fib(fib);

        //var rnd = new Random();

        //for (var ii = 0; ii < 1000; ii++)
        //{
        //    var X = Enumerable.Range(1, 100).Select(i => rnd.Next(1, 1000)).ToArray();
        //    QuickSort(X);
        //    for (var j = 1; j < X.Length; j++)
        //        if (X[j] < X[j - 1])
        //            throw new InvalidOperationException();
        //}


        //const int count = 1000;
        //var items = Enumerable.Range(1, count).ToArray();

        //var r2 = items.Shuffle(rnd).ToArray();

        //ulong n = 68;
        //ulong k = 34;

        //var r = SpecialFunctions.BinomialCoefficientBigInt(n, k);

        //if (k > n / 2) k = n - k;

        //var b = SpecialFunctions.BinomialCoefficient(n, k);
        ////var b = SpecialFunctions.BCR((int)n, (int)k);
        ////var vv = SpecialFunctions.BCRCache.OrderBy(v => v.Key).ThenBy(v => v.Value).ToArray();
        //// n! / (k!*(n-k)!)


        //var nn = 1ul;
        //var kk = 1ul;
        //var i = 0ul;
        //while (i < k)
        //{
        //    var n0 = n - i;
        //    i++;
        //    var k0 = i;

        //    if (!Fraction.Simplify(ref nn, ref k0) || k0 > 1) kk *= k0;
        //    Fraction.Simplify(ref n0, ref kk);

        //    nn *= n0;
        //}

        //Console.WriteLine(nn);
        //Console.WriteLine();

        ////Console.WriteLine();
    }
}

public static class TailRecursion
{
    public static T Execute<T>(Func<RecursionResult<T>> func)
    {
        while (true)
        {
            var recursion_result = func();
            if (recursion_result.IsFinalResult)
                return recursion_result.Result;
            func = recursion_result.NextStep;
        }
    }

    public static RecursionResult<T> Return<T>(T result) => new(true, result, null);

    public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> NextStep) => new(false, default, NextStep);
}

public class RecursionResult<T>
{
    private readonly bool _IsFinalResult;
    private readonly T _Result;
    private readonly Func<RecursionResult<T>> _NextStep;
    internal RecursionResult(bool IsFinalResult, T result, Func<RecursionResult<T>> NextStep)
    {
        _IsFinalResult = IsFinalResult;
        _Result = result;
        _NextStep = NextStep;
    }

    public bool IsFinalResult => _IsFinalResult;
    public T Result => _Result;
    public Func<RecursionResult<T>> NextStep => _NextStep;
}