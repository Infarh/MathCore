﻿#nullable enable
namespace MathCore;

/// <summary>Алгоритмы для Простых чисел</summary>
public static class PrimeNumbers
{
    /// <summary>Расширение списка простых чисел до нужного размера</summary>
    /// <param name="OldSize">Текущий размер списка</param>
    /// <returns>Новый размер - ближайшее большее простое число</returns>
    /// <remarks>max = 2146435069</remarks>
    public static int ExpandPrime(int OldSize)
    {
        var min = 2 * OldSize;
        return (uint)min > 2146435069U && 2146435069 > OldSize ? 2146435069 : GetClosestUp(min);
    }

    private static readonly int[] __First72PrimeNumbers =
    [
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293,
        353, 431, 521, 631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
        4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023, 25229,
        30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
        187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
        968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899,
        4166287, 4999559, 5999471, 7199369
    ];

    /// <summary>Возвращает ближайшее большее простое число</summary>
    /// <param name="n">Значение</param>
    /// <returns>Ближайшее большее простое число</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если <paramref name="n"/> меньше нуля</exception>
    public static int GetClosestUp(int n)
    {
        if (n < 0)
            throw new ArgumentOutOfRangeException(nameof(n), "Значение должно быть больше нуля");

        foreach (var value in __First72PrimeNumbers)
            if (value >= n)
                return value;

        for (var i = n | 1; i < int.MaxValue; i += 2)
            if (IsPrime(i) && (i - 1) % 101 != 0)
                return i;

        return n;
    }

    /// <summary>Проверка - является ли число простым?</summary>
    /// <param name="n">Проверяемое число</param>
    /// <returns>Истина, если число простое</returns>
    /// <remarks>Возвращает false, если n &lt;= 1</remarks>
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
    [Copyright("Bodigrim@HabarHabar:\"Еще раз о поиске простых чисел\"", url = "http://habrahabr.ru/blogs/algorithm/133037/"), DST]
    public static IEnumerable<int> GetNumbersTo(int n)
    {
        var result = new int[n + 1];
        for (var i = 2; i <= n; i++)
            result[i] = i;
        for (var i = 2; i * i <= n; i++)
            if (result[i] == i)
                for (var j = i * i; j <= n; j += i)
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
            private readonly byte[]? _SecondBuffer;

            /// <summary>Инициализирует новый экземпляр класса LongArray, создавая буферы для хранения данных определенной длины</summary>
            /// <param name="length">Длина массива, который необходимо создать</param>
            public LongArray(long length)
            {
                _FirstBuffer = new byte[Math.Min(__MaxChunkLength, length)];
                if (length > __MaxChunkLength) _SecondBuffer = new byte[length - __MaxChunkLength];
            }

            /// <summary>Конструктор, который загружает массив из файла</summary>
            /// <param name="FileName">Имя файла, из которого загружается массив</param>
            /// <exception cref="InvalidOperationException">Если в файле недостаточно данных</exception>
            public LongArray(string FileName)
            {
                using var file = System.IO.File.OpenRead(FileName);
                var length = file.Length;
                _FirstBuffer = new byte[Math.Min(__MaxChunkLength, length)];
                if (length > __MaxChunkLength) _SecondBuffer = new byte[length - __MaxChunkLength];

                if (file.Read(_FirstBuffer, 0, _FirstBuffer.Length) != _FirstBuffer.Length)
                    throw new InvalidOperationException("Недостаточно данных в файле");

                if (_SecondBuffer != null)
                    if (file.Read(_SecondBuffer, 0, _SecondBuffer.Length) != _SecondBuffer.Length)
                        throw new InvalidOperationException("Недостаточно данных для чтения второго буфера");
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

            /// <summary>Сохраняет массив в файл</summary>
            /// <param name="FileName">Имя файла, в который будет сохранен массив</param>
            public void Save(string FileName)
            {
                using var file = System.IO.File.OpenWrite(FileName);
                file.Write(_FirstBuffer, 0, _FirstBuffer.Length);
                if (_SecondBuffer != null)
                    file.Write(_SecondBuffer, 0, _SecondBuffer.Length);
            }
        }

        //private static readonly int[] __BitToIndex = { 1, 7, 11, 13, 17, 19, 23, 29 };

        private static readonly int[] __IndexToBit = [
            -1, 0,
            -1, -1, -1, -1, -1, 1,
            -1, -1, -1, 2,
            -1, 3,
            -1, -1, -1, 4,
            -1, 5,
            -1, -1, -1, 6,
            -1, -1, -1, -1, -1, 7,
        ];

        private readonly LongArray _Data;

        public Wheel235(long length)
        {
            // ensure length divides by 30
            length = (length + 29) / 30 * 30;
            _Data = new(length / 30);
            for (long i = 0; i < _Data.Length; i++)
                _Data[i] = byte.MaxValue;

            for (long i = 7; i * i < Length; i++)
            {
                if (!IsPrime(i)) continue;
                for (var d = i * i; d < Length; d += i)
                    ClearPrime(d);
            }
        }

        public Wheel235(string file) => _Data = new(file);

        public void Save(string file) => _Data.Save(file);

        public long Length => _Data.Length * 30L;

        /// <summary>Проверяет, является ли заданное число простым</summary>
        /// <param name="n">Число, которое нужно проверить.</param>
        /// <returns>True, если число простое, false - в противном случае.</returns>
        /// <exception cref="ArgumentException">Если <paramref name="n"/> больше, чем длина колеса.</exception>
        public bool IsPrime(long n)
        {
            if (n >= Length) throw new ArgumentException("Number too big");
            if (n <= 5) return n is 2 or 3 or 5;
            var bit = __IndexToBit[n % 30];
            return bit >= 0 && (_Data[n / 30] & (1 << bit)) != 0;
        }

        /// <summary>Удаляет простое число из колеса</summary>
        /// <param name="n">Простое число, которое нужно удалить.</param>
        /// <remarks>Метод ничего не делает, если <paramref name="n"/> - не простое число.</remarks>
        private void ClearPrime(long n)
        {
            var bit = __IndexToBit[n % 30];
            if (bit < 0)
                return;
            _Data[n / 30] &= (byte)~(1 << bit);
        }
    }
}