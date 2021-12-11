using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using MathCore;

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest
{
    internal class TP : Polynom
    {
        public int Test()
        {
            return AAA;
        }
    }

    internal static class Program
    {
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

        private static void Main()
        {
            var t0 = 637713575720000000;
            var time = DateTime.FromBinary(t0);
            var start_time = new DateTime(2010, 1, 1, 0, 0, 0, 0);
            var delta = time - start_time;
            var ticks_delta = delta.Ticks;
            var int_ticks = (int)(ticks_delta / 10000000);

            var values = new List<string>();
            foreach (var value in CSVParseTest.ParseCSVLine(CSVParseTest.SourceStr))
                values.Add(value);

            var size = 1000;
            var arr_a = GC.AllocateUninitializedArray<double>(size).Initialize(i => Math.Sin(4 * Math.PI / size * i));
            var arr_b = GC.AllocateUninitializedArray<double>(size).Initialize(i => Math.Cos(4 * Math.PI / size * i));

            var correlations = new List<(int offset, double value)>(size * 2);
            for (var offset = -size; offset <= size; offset++)
            {
                correlations.Add((offset, Multiply(arr_a, arr_b, offset)));
            }

            var fff = SpecialFunctions.Fibonacci(7);

            var ff = EnumFib().ElementAt(8);

            var fib = new int[10];
            for (var m = 0; m < fib.Length; m++)
            {
                fib[m] = Fib(m);
            }

            Array.Clear(fib, 0, fib.Length);
            Fib(fib);

            var rnd = new Random();

            for (var ii = 0; ii < 1000; ii++)
            {
                var X = Enumerable.Range(1, 100).Select(i => rnd.Next(1, 1000)).ToArray();
                QuickSort(X);
                for (var j = 1; j < X.Length; j++)
                    if (X[j] < X[j - 1])
                        throw new InvalidOperationException();
            }


            const int count = 1000;
            var items = Enumerable.Range(1, count).ToArray();

            var r2 = items.Shuffle(rnd).ToArray();

            ulong n = 68;
            ulong k = 34;

            var r = SpecialFunctions.BinomialCoefficientBigInt(n, k);

            if (k > n / 2) k = n - k;

            var b = SpecialFunctions.BinomialCoefficient(n, k);
            //var b = SpecialFunctions.BCR((int)n, (int)k);
            //var vv = SpecialFunctions.BCRCache.OrderBy(v => v.Key).ThenBy(v => v.Value).ToArray();
            // n! / (k!*(n-k)!)


            var nn = 1ul;
            var kk = 1ul;
            var i = 0ul;
            while (i < k)
            {
                var n0 = n - i;
                i++;
                var k0 = i;

                if (!Fraction.Simplify(ref nn, ref k0) || k0 > 1) kk *= k0;
                Fraction.Simplify(ref n0, ref kk);

                nn *= n0;
            }

            Console.WriteLine(nn);
            Console.WriteLine();

            //Console.WriteLine();
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
}