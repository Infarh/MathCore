using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    /// <summary>Алгоритмы для Простых чисел</summary>
    public static class PrimeNumbers
    {
        public static int ExpandPrime(int OldSize)
        {
            var min = 2 * OldSize;
            return (uint)min > 2146435069U && 2146435069 > OldSize ? 2146435069 : GetClosestUp(min);
        }

        private static readonly int[] __First72PrimeNumbers = new int[72]
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 
            353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
            4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229,
            30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
            968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899,
            4166287, 4999559, 5999471, 7199369
        };

        public static int GetClosestUp(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Значение должно быть больше нуля");

            for (var i = 0; i < __First72PrimeNumbers.Length; i++)
            {
                var value = __First72PrimeNumbers[i];
                if (value >= n)
                    return value;
            }

            for (var i = n | 1; i < int.MaxValue; i += 2)
                if (IsPrime(i) && (i - 1) % 101 != 0)
                    return i;

            return n;
        }

        public static bool IsPrime(int n)
        {
            if ((n & 1) == 0)
                return n == 2;

            var max = (int)Math.Sqrt(n);
            for (var i = 3; i <= max; i += 2)
                if (n % i == 0)
                    return false;

            return true;
        }

        //[System.Diagnostics.DST]
        //public static bool IsPrime(int n)
        //{
        //    if(n < 0) n = -n;
        //    if(n % 2 == 0) return n == 2;

        //    var max = (int)Math.Sqrt(n);

        //    for(var i = 3; i <= max; i += 2) if(n % i == 0) return false;
        //    return true;
        //}

        /// <summary>Поиск простых чисел с использованием алгоритма "Решето Эратосфен"</summary>
        /// <param name="n">Предельное значение до которого (включительно) осуществляется поиск</param>
        /// <returns>Перечисление найденных простых чисел</returns>
        [Copyright("Bodigrim@HabarHabar:\"Еще раз о поиске простых чисел\"", url = "http://habrahabr.ru/blogs/algorithm/133037/"), DST, NotNull]
        public static IEnumerable<int> GetNumbersTo(int n)
        {
            var result = new int[n + 1];
            for(var i = 2; i <= n; i++)
                result[i] = i;
            for(var i = 2; i * i <= n; i++)
                if(result[i] == i) 
                    for(var j = i * i; j <= n; j += i)
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

        ///<remarks>
        /// <code>
        ///     long N = 100_000_000_000L;
        ///     var wheel = new Wheel235(N);
        ///     wheel.Save("./primes.dat");
        ///
        ///     ...
        /// 
        ///     var start = DateTime.UtcNow;
        ///     var loaded = new Wheel235("./primes.dat");
        ///     var end = DateTime.UtcNow;
        ///     Console.WriteLine($"Database of prime numbers up to {loaded.Length:N0} loaded from file to memory  in {end - start}");
        ///     var number = 98_000_000_093L;
        ///     Console.WriteLine($"{number:N0} is {(loaded.IsPrime(number) ? "prime" : "not prime")}!");
        /// </code>
        /// </remarks>
        [Copyright("Oleg T lightln2@HabarHabar:\"База данных простых чисел до ста миллиардов на коленке\"", url = "https://habr.com/ru/post/504598/")]
        private class Wheel235
        {
            private class LongArray
            {
                private const long __MaxChunkLength = 2_000_000_000L;
                private readonly byte[] _FirstBuffer;
                private readonly byte[] _SecondBuffer;

                public LongArray(long length)
                {
                    _FirstBuffer = new byte[Math.Min(__MaxChunkLength, length)];
                    if (length > __MaxChunkLength) _SecondBuffer = new byte[length - __MaxChunkLength];
                }

                public LongArray(string FileName)
                {
                    using var file = System.IO.File.OpenRead(FileName);
                    var length = file.Length;
                    _FirstBuffer = new byte[Math.Min(__MaxChunkLength, length)];
                    if (length > __MaxChunkLength) _SecondBuffer = new byte[length - __MaxChunkLength];
                    file.Read(_FirstBuffer, 0, _FirstBuffer.Length);
                    if (_SecondBuffer != null) file.Read(_SecondBuffer, 0, _SecondBuffer.Length);
                }

                public long Length => _FirstBuffer.LongLength + (_SecondBuffer?.LongLength ?? 0);

                public byte this[long index]
                {
                    get => index < __MaxChunkLength ? _FirstBuffer[index] : _SecondBuffer[index - __MaxChunkLength];
                    set
                    {
                        if (index < __MaxChunkLength) 
                            _FirstBuffer[index] = value;
                        else 
                            _SecondBuffer[index - __MaxChunkLength] = value;
                    }
                }

                public void Save(string FileName)
                {
                    using var file = System.IO.File.OpenWrite(FileName);
                    file.Write(_FirstBuffer, 0, _FirstBuffer.Length);
                    if (_SecondBuffer != null)
                        file.Write(_SecondBuffer, 0, _SecondBuffer.Length);
                }
            }

            private static readonly int[] __BitToIndex = { 1, 7, 11, 13, 17, 19, 23, 29 };

            private static readonly int[] __IndexToBit = {
                -1, 0,
                -1, -1, -1, -1, -1, 1,
                -1, -1, -1, 2,
                -1, 3,
                -1, -1, -1, 4,
                -1, 5,
                -1, -1, -1, 6,
                -1, -1, -1, -1, -1, 7,
            };

            private readonly LongArray _Data;

            public Wheel235(long length)
            {
                // ensure length divides by 30
                length = (length + 29) / 30 * 30;
                _Data = new LongArray(length / 30);
                for (long i = 0; i < _Data.Length; i++) 
                    _Data[i] = byte.MaxValue;

                for (long i = 7; i * i < Length; i++)
                {
                    if (!IsPrime(i)) continue;
                    for (var d = i * i; d < Length; d += i) 
                        ClearPrime(d);
                }
            }

            public Wheel235(string file) => _Data = new LongArray(file);

            public void Save(string file) => _Data.Save(file);

            public long Length => _Data.Length * 30L;

            public bool IsPrime(long n)
            {
                if (n >= Length) throw new ArgumentException("Number too big");
                if (n <= 5) return n == 2 || n == 3 || n == 5;
                var bit = __IndexToBit[n % 30];
                return bit >= 0 && (_Data[n / 30] & (1 << bit)) != 0;
            }

            private void ClearPrime(long n)
            {
                var bit = __IndexToBit[n % 30];
                if (bit < 0) 
                    return;
                _Data[n / 30] &= (byte)~(1 << bit);
            }

        }
    }
}