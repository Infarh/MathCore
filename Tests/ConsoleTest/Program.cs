using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

using MathCore;
using MathCore.CSV;
using MathCore.Statistic;
using MathCore.Statistic.RandomNumbers;

using Microsoft.Data.Analysis;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest
{
    internal static class Program
    {
        private static RecursionResult<BigInteger> Factorial(int n, BigInteger product) => 
            n < 2 
                ? TailRecursion.Return(product) 
                : TailRecursion.Next(() => Factorial(n - 1, n * product));

        private static void Main()
        {
            var ff = new FileInfo(@"C:\Users\shmac\src\ГСС\RRJ\Data\Airports\Airports\Data\airports.csv");

            var first = ff.OpenCSV().WithHeader().GetHeader();


            //var date_time = DateTime.Now;
            //var time = date_time.TimeOfDay;
            //var day = date_time.Date;

            //var result = TailRecursion.Execute(() => Factorial(8, 1));
            //var v = (long)result;
            //var s = result.ToString();
            //var len = s.Length;

            var a_ch = 'a';
            var n_ch = 'n';

            var a_code = (int)a_ch;
            var n_code = (int)n_ch;

            var result = (char)(a_ch + n_ch);
            Console.WriteLine(result);
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