using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    public static class PrimeNumbers
    {
        //[Pure, System.Diagnostics.DST]
        //public static bool IsPrime(int n)
        //{
        //    if(n < 0) n = -n;
        //    if(n % 2 == 0) return n == 2;

        //    var max = (int)Math.Sqrt(n);

        //    for(var i = 3; i <= max; i += 2) if(n % i == 0) return false;
        //    return true;
        //}

        [Copyright("Bodigrim@HabarHabar:\"Еще раз о поиске простых чисел\"", url = "http://habrahabr.ru/blogs/algorithm/133037/")]
        [Pure, DST]
        public static IEnumerable<int> GetNumbersTo(int n)
        {
            var result = new int[n + 1];
            for(var i = 2; i <= n; i++)
                result[i] = i;
            for(var i = 2; i * i <= n; i++)
                if(result[i] == i) for(var j = i * i; j <= n; j += i)
                        result[j] = 0;
            return result.Where(v => v != 0);
        }

        //public static int[] GetNumbersTo(int n)
        //{
        //    var result = new int[n + 1];
        //    for(var k = 1; k <= n; k++) result[k] = k;

        //    for(var k = 1; (2 * k + 1) * (2 * k + 1) <= 2 * n + 1; k++)
        //        // если 2k+1 - простое (т. е. k не вычеркнуто)
        //        if(result[k] == k)
        //            for(var l = 3 * k + 1; l <= n; l += 2 * k + 1)
        //                result[l] = 0;
        //    return result.Where(v => v != 0).ToArray();
        //}

        //public static IEnumerable<int> GetNumbersTo(int n)
        //{
        //    var result = new int[n];

        //    for(int i = 1, k; 3 * i + 1 < n; i++)
        //        for(var j = 1; (k = i + j + 2 * i * j) < n && j <= i; j++)
        //            result[k] = 1;
        //    return result
        //        .Select((v, i) => v == 0 && i != 0 ? 2 * i + 1 : 0)
        //        //.Where(v => v != 0)
        //        .ToArray();
        //}
    }
}