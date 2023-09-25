#nullable enable
using System.Collections;

using MathCore;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Методы-расширения для генератора случайных чисел</summary>
public static class RandomExtensions
{
    public static byte[] NextBytes(this Random random, int Count)
    {
        if (Count < 0) throw new ArgumentOutOfRangeException(nameof(Count), Count, "Размер должен быть больше нуля");
        if (Count == 0) return Array.Empty<byte>();

        var bytes = new byte[Count];
        random.NextBytes(bytes);

        return bytes;
    }

    /// <summary>Создать генератор случных элементов</summary>
    /// <typeparam name="T">Тип элементов списка</typeparam>
    /// <param name="Random">Датчик случайных чисел</param>
    /// <param name="Items">Список элементов, на основе которого надо создать генератор</param>
    /// <returns>Генератор случайного значения из элементов списка</returns>
    public static Randomizer<T> GetRandomizer<T>(this Random Random, IList<T> Items) => new(Items, Random);

    /// <summary>Создать генератор случных элементов</summary>
    /// <typeparam name="T">Тип элементов списка</typeparam>
    /// <param name="Random">Датчик случайных чисел</param>
    /// <param name="Items">Элементы, на основе которых надо создать генератор</param>
    /// <returns>Генератор случайного значения из элементов списка</returns>
    public static Randomizer<T> GetRandomizer<T>(this Random Random, params T[] Items) => new(Items, Random);

    /// <summary>Случайный элемент из указанного набора вариантов (ссылка на элемент)</summary>
    /// <typeparam name="T">Тип элементов</typeparam>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="items">Массив вариантов</param>
    /// <returns>Ссылка на случайный элемент массива</returns>
    public static ref T NextItem<T>(this Random rnd, params T[] items) => ref items[rnd.Next(items.Length)];

    /// <summary>Случайных элемент из списка</summary>
    /// <typeparam name="T">Тип элементов для выбора</typeparam>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="items">Список элементов для выбора</param>
    /// <returns>Случайный элемент из списка</returns>
    public static T NextItem<T>(this Random rnd, IList<T> items) => items[rnd.Next(items.Count)];

    /// <summary>Массив случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="D">Дисперсия</param>
    /// <param name="M">Математическое ожидание</param>
    /// <returns>Массив случайных чисел с равномерным распределением</returns>
    public static double[] NextUniform(this Random rnd, int Count, double D = 1, double M = 0)
    {
        var result = new double[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.NextUniform(D, M);
        return result;
    }

    /// <summary>Перечисление случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <param name="D">Дисперсия</param>
    /// <param name="M">Математическое ожидание</param>
    /// <returns>Перечисление случайных чисел с равномерным распределением</returns>
    public static IEnumerable<double> NextUniformEnum(this Random rnd, int Count, double D = 1, double M = 0)
    {
        for (var i = 0; Count < 0 || i < Count; i++)
            yield return rnd.NextUniform(D, M);
    }

    /// <summary>Массив случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="Interval">Интервал</param>
    /// <returns>Массив случайных чисел с равномерным распределением</returns>
    public static double[] NextUniform(this Random rnd, int Count, Interval Interval)
    {
        var D = Interval.Length;
        var M = Interval.Middle;
        var result = new double[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.NextUniform(D, M);
        return result;
    }

    /// <summary>Перечисление случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <param name="Interval">Интервал</param>
    /// <returns>Перечисление случайных чисел с равномерным распределением</returns>
    public static IEnumerable<double> NextUniformEnum(this Random rnd, int Count, Interval Interval)
    {
        var D = Interval.Length;
        var M = Interval.Middle;
        for (var i = 0; Count < 0 || i < Count; i++)
            yield return rnd.NextUniform(D, M);
    }

    /// <summary>Массив случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Intervals">Интервал</param>
    /// <returns>Массив случайных чисел с равномерным распределением</returns>
    public static double[] NextUniform(this Random rnd, params Interval[] Intervals)
    {
        var count = Intervals.Length;
        var result = new double[count];
        for (var i = 0; i < count; i++)
            result[i] = rnd.NextUniform(Intervals[i].Length, Intervals[i].Middle);
        return result;
    }

    /// <summary>Массив случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="Min">Минимум</param>
    /// <param name="Max">Максимум</param>
    /// <returns>Массив случайных чисел с равномерным распределением</returns>
    public static double[] NextUniformInterval(this Random rnd, int Count, double Min, double Max)
    {
        var D = Math.Abs(Max - Min);
        var M = (Max + Min) / 2;
        var result = new double[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.NextUniform(D, M);
        return result;
    }

    /// <summary>Перечисление случайных чисел с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <param name="Min">Минимум</param>
    /// <param name="Max">Максимум</param>
    /// <returns>Перечисление случайных чисел с равномерным распределением</returns>
    public static IEnumerable<double> NextUniformIntervalEnum(this Random rnd, int Count, double Min, double Max)
    {
        var D = Math.Abs(Max - Min);
        var M = (Max + Min) / 2;
        for (var i = 0; Count < 0 || i < Count; i++)
            yield return rnd.NextUniform(D, M);
    }

    /// <summary>Массив целых неотрицательных случайных чисел</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <returns>Массив целых неотрицательных случайных чисел</returns>
    public static int[] NextValues(this Random rnd, int Count)
    {
        var result = new int[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.Next();
        return result;
    }

    /// <summary>Перечисление целых неотрицательных случайных чисел</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <returns>перечисление целых неотрицательных случайных чисел</returns>
    public static IEnumerable<int> NextValuesEnum(this Random rnd, int Count)
    {
        for (var i = 0; i < Count; i++)
            yield return rnd.Next();
    }

    /// <summary>Массив целых неотрицательных случайных чисел ограниченный сверху (верхний предел не входит)</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="Max">Максимум (не входит)</param>
    /// <returns>Массив целых неотрицательных случайных чисел (верхний предел не входит)</returns>
    public static int[] NextValues(this Random rnd, int Count, int Max)
    {
        var result = new int[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.Next(Max);
        return result;
    }

    /// <summary>Перечисление целых неотрицательных случайных чисел ограниченный сверху (верхний предел не входит)</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <param name="Max">Максимум (не входит)</param>
    /// <returns>Перечисление целых неотрицательных случайных чисел (верхний предел не входит)</returns>
    public static IEnumerable<int> NextValuesEnum(this Random rnd, int Count, int Max)
    {
        for (var i = 0; i < Count; i++)
            yield return rnd.Next(Max);
    }

    /// <summary>Массив целых неотрицательных случайных чисел в заданном интервале (верхний предел не входит)</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="Min">Минимум</param>
    /// <param name="Max">Максимум (не входит)</param>
    /// <returns>Массив целых неотрицательных случайных чисел в заданном интервале (верхний предел не входит)</returns>
    public static int[] NextValues(this Random rnd, int Count, int Min, int Max)
    {
        var result = new int[Count];
        for (var i = 0; i < Count; i++)
            result[i] = rnd.Next(Min, Max);
        return result;
    }

    /// <summary>Перечисление целых неотрицательных случайных чисел в заданном интервале (верхний предел не входит)</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер перечисления (если меньше 0, то бесконечное)</param>
    /// <param name="Min">Минимум</param>
    /// <param name="Max">Максимум (не входит)</param>
    /// <returns>Перечисление целых неотрицательных случайных чисел в заданном интервале (верхний предел не входит)</returns>
    public static IEnumerable<int> NextValuesEnum(this Random rnd, int Count, int Min, int Max)
    {
        for (var i = 0; Count < 0 || i < Count; i++)
            yield return rnd.Next(Min, Max);
    }

    /// <summary>Массив случайных чисел с нормальным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Массив случайных чисел с нормальным распределением</returns>
    public static double[] NextNormal(this Random rnd, int Count, double sigma = 1, double mu = 0) => rnd.FillNormal(new double[Count], sigma, mu);

    /// <summary>Перечисление случайных чисел с нормальным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="Count">Размер массива</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Перечисление случайных чисел с нормальным распределением</returns>
    public static IEnumerable<double> NextNormalEnum(this Random rnd, int Count, double sigma = 1, double mu = 0)
    {
        for (var i = 0; i < Count; i++)
            yield return rnd.NextNormal(sigma, mu);
    }

    //public static double NextNormal(this Random rnd, double D = 1, double M = 0) => Math.Tan(Math.PI * (rnd.NextDouble() - 0.5)) * D + M;

    /// <summary>Случайное число с нормальным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Случайное число с нормальным распределением</returns>
    [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
    public static double NextNormal(this Random rnd, double sigma = 1, double mu = 0)
    {
        var u1     = 1 - rnd.NextDouble(); //uniform(0,1] random doubles
        var u2     = 1 - rnd.NextDouble();
        var normal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2); //random normal(0,1)
        return mu + sigma * normal;                                                 //random normal(m,D^2)
    }

    /// <summary>Случайное число с равномерным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Случайное число в равномерным распределением</returns>
    public static double NextUniform(this Random rnd, double sigma = 1, double mu = 0) => (rnd.NextDouble() - 0.5) * sigma + mu;

    /// <summary>Случайное число с треугольным распределением</summary>
    /// <remarks>http://en.wikipedia.org/wiki/Triangular_distribution</remarks>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="min">Минимум</param>
    /// <param name="max">Максимум</param>
    /// <param name="mode">Медиана</param>
    /// <returns>Случайное число с треугольным распределением</returns>
    [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
    public static double NextTriangular(this Random rnd, double min, double max, double mode)
    {
        var u = rnd.NextDouble();

        return u < (mode - min) / (max - min)
            ? min + Math.Sqrt(u * (max - min) * (mode - min))
            : max - Math.Sqrt((1 - u) * (max - min) * (max - mode));
    }

    /// <summary>Заполнить массив случайными числами с равномерным распределением в интервале [0, 1)</summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="array">Заполняемый массив</param>
    public static double[] FillUniform(this Random rnd, double[] array)
    {
        for (var i = 0; i < array.Length; i++)
            array[i] = rnd.NextDouble();
        return array;
    }

    /// <summary>Заполнить массив случайными числами с равномерным распределением в интервале [-D/2, D/2)</summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="array">Заполняемый массив</param>
    /// <param name="sigma">Среднеквадратичное отклонение значений</param>
    public static double[] FillUniform(this Random rnd, double[] array, double sigma)
    {
        for (var i = 0; i < array.Length; i++)
            array[i] = rnd.NextDouble(sigma);
        return array;
    }

    /// <summary>Заполнить массив случайными числами с равномерным распределением в интервале [-D/2 + M, D/2 + M)</summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="array">Заполняемый массив</param>
    /// <param name="sigma">Среднеквадратичное отклонение значений</param>
    /// <param name="mu">Математическое ожидание</param>
    public static double[] FillUniform(this Random rnd, double[] array, double sigma, double mu)
    {
        for (var i = 0; i < array.Length; i++)
            array[i] = rnd.NextDouble(sigma, mu);
        return array;
    }

    public static double[] FillNormal(this Random rnd, double[] array, double sigma = 1, double mu = 0)
    {
        for (var i = 0; i < array.Length; i++)
            array[i] = rnd.NextNormal(sigma, mu);
        return array;
    }

    /// <summary>Случайное значение <see langword="true"/>/<see langword="false"/></summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    public static bool NextBoolean(this Random rnd) => rnd.Next(2) > 1;

    /// <summary>Случайное число с равномерным распределением в интервале [-D/2, D/2)</summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="sigma">Среднеквадратичное отклонение значений</param>
    /// <returns>Случайное число в интервале  [-D/2, D/2)</returns>
    public static double NextDouble(this Random rnd, double sigma) => (rnd.NextDouble() - 0.5) * sigma;

    /// <summary>Случайное число с равномерным распределением в интервале [-sigma/2 + mu, sigma/2 + mu)</summary>
    /// <param name="rnd">Генератор случайных чисел</param>
    /// <param name="sigma">Среднеквадратичное отклонение значений</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <returns>Случайное число в интервале  [-sigma/2 + mu, sigma/2 + mu)</returns>
    public static double NextDouble(this Random rnd, double sigma, double mu) => (rnd.NextDouble() - 0.5) * sigma + mu;

    public static double NextDoubleInterval(this Random rnd, in Interval Interval) => rnd.NextDouble(Interval.Length, Interval.Middle);

    public static double NextDoubleInterval(this Random rnd, double Min, double Max) => rnd.NextDouble(Max - Min, 0.5 * (Max + Min));

    /// <summary>Shuffles a list in O(n) time by using the Fisher-Yates/Knuth algorithm</summary>
    /// <param name="rnd"></param>
    /// <param name = "list"></param>
    [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
    public static void Mix(this Random rnd, IList list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var j = rnd.Next(0, i + 1);

            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    /// <summary>
    /// Returns n unique random numbers in the range [1, n], inclusive. 
    /// This is equivalent to getting the first n numbers of some random permutation of the sequential numbers from 1 to max. 
    /// Runs in O(k^2) time.
    /// </summary>
    /// <param name="rnd"></param>
    /// <param name="n">Maximum number possible.</param>
    /// <param name="k">How many numbers to return.</param>
    /// <returns></returns>
    [Copyright("Superbest@bitbucket.org", url = "https://bitbucket.org/Superbest/superbest-random")]
    public static int[] Permutation(this Random rnd, int n, int k)
    {
        var result = new List<int>();
        var sorted = new SortedSet<int>();

        for (var i = 0; i < k; i++)
        {
            var r = rnd.Next(1, n + 1 - i);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var q in sorted)
                if (r >= q) r++;

            result.Add(r);
            sorted.Add(r);
        }

        return result.ToArray();
    }

    public static T Next<T>(this Random rnd, params T[] items) => items[rnd.Next(items.Length)];

    public static T Next<T>(this Random rnd, IReadOnlyList<T> list) => list[rnd.Next(list.Count)];

    /// <summary>Случайных элемент из перечисленных вариантов</summary>
    /// <typeparam name="T">Тип вариантов выбора</typeparam>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="count">Количество результатов выбора</param>
    /// <param name="variants">Перечисление вариантов выбора</param>
    /// <returns>Последовательность случайных вариантов</returns>
    public static IEnumerable<T?> Next<T>(this Random rnd, int count, params T?[] variants)
    {
        if (rnd is null) throw new ArgumentNullException(nameof(rnd));
        if (variants is null) throw new ArgumentNullException(nameof(variants));

        var variants_count = variants.Length;
        for (var i = 0; i < count; i++)
            yield return variants[rnd.Next(0, variants_count)];
    }

    /// <summary>Последовательность случайных целых чисел в указанном интервале</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="min">Нижняя граница интервала (входит)</param>
    /// <param name="max">Верхняя граница интервала (не входит)</param>
    /// <param name="count">Размер выборки (если меньше 0), то бесконечная последовательность</param>
    /// <returns>Последовательность случайных целых чисел в указанном интервале</returns>
    public static IEnumerable<int> SequenceInt(this Random rnd, int min, int max, int count = -1)
    {
        if (rnd is null) throw new ArgumentNullException(nameof(rnd));

        if (count == 0) yield break;
        if (count < 0) while (true) yield return rnd.Next(min, max);
        for (var i = 0; i < count; i++)
            yield return rnd.Next(min, max);
    }

    /// <summary>Последовательность случайных вещественных чисел с равномерным распределением в интервале (0,1)</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="count">Размер выборки (если меньше 0), то бесконечная последовательность</param>
    /// <returns>Последовательность случайных вещественных чисел в интервале (0,1)</returns>
    public static IEnumerable<double> SequenceDouble(this Random rnd, int count = -1)
    {
        if (rnd is null) throw new ArgumentNullException(nameof(rnd));

        if (count == 0) yield break;
        if (count < 0) while (true) yield return rnd.NextDouble();
        for (var i = 0; i < count; i++)
            yield return rnd.NextDouble();
    }

    /// <summary>Последовательность случайных вещественных чисел с нормальным распределением</summary>
    /// <param name="rnd">Датчик случайных чисел</param>
    /// <param name="mu">Математическое ожидание</param>
    /// <param name="sigma">Среднеквадратичное отклонение</param>
    /// <param name="count">Размер выборки (если меньше 0), то бесконечная последовательность</param>
    /// <returns>Последовательность случайных вещественных чисел</returns>
    public static IEnumerable<double> SequenceNormal(this Random rnd, double sigma = 1, double mu = 0, int count = -1)
    {
        if (rnd is null) throw new ArgumentNullException(nameof(rnd));

        if (count == 0) yield break;
        if (count < 0) while (true) yield return rnd.NextNormal(sigma, mu);
        for (var i = 0; i < count; i++)
            yield return rnd.NextNormal(sigma, mu);
    }
}