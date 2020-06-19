using System.Linq;
using MathCore;
using MathCore.Annotations;
using MathCore.Interpolation;
using MathCore.Statistic;
using MathCore.Values;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>Методы-расширения для массивов вещественных чисел</summary>
    public static class DoubleArrayExtensions
    {
        /// <summary>Прибавить значение ко всем элементам массива</summary>
        /// <param name="array">Массив вещественных чисел</param>
        /// <param name="value">Прибавляемое ко всем элементам значение</param>
        [DST]
        public static void Add([NotNull] this double[] array, double value)
        {
            var array_length = array.Length;
            for (var i = 0; i < array_length; i++)
                array[i] += value;
        }

        /// <summary>Поэлементно сложить два массива (для минимального числа совпадающих элементов)</summary>
        /// <param name="array">Массив - первое слагаемое</param>
        /// <param name="values">Массив - второе слагаемое</param>
        [DST]
        public static void Add([NotNull] this double[] array, [NotNull] double[] values)
        {
            var array_length = array.Length;
            var values_length = values.Length;
            for (var i = 0; i < array_length && i < values_length; i++)
                array[i] += values[i];
        }

        /// <summary>Усреднить значения всех массивов</summary>
        /// <param name="array">Усредняемые массивы</param>
        /// <returns>Массив средних значений</returns>
        [DST, NotNull]
        public static double[] Average([NotNull] this double[][] array) => array.ToArray(a => a?.Average() ?? double.NaN);

        [DST, NotNull]
        public static double[] AverageByRows([NotNull] this double[,] array)
        {
            var rows_count = array.GetLength(0);
            var cols_count = array.GetLength(1);
            var col_average = new double[cols_count];
            for (var row = 0; row < cols_count; row++)
            {
                var sum = 0d;
                for (var row_cell = 0; row_cell < rows_count; row_cell++)
                    sum += array[row_cell, row];
                col_average[row] = sum / rows_count;
            }
            return col_average;
        }

        [DST, NotNull]
        public static double[] AverageByCols([NotNull] this double[,] array)
        {
            var rows_count = array.GetLength(0);
            var cols_count = array.GetLength(1);
            var row_average = new double[rows_count];
            for (var col = 0; col < rows_count; col++)
            {
                var sum = 0d;
                for (var col_cell = 0; col_cell < cols_count; col_cell++)
                    sum += array[col, col_cell];
                row_average[col] = sum / rows_count;
            }
            return row_average;
        }

        /// <summary>Рассчитать дисперсию массива как M{X^2} - M{X}^2</summary>
        /// <param name="array">Массив, дисперсию элементов которого требуется рассчитать</param>
        /// <returns>Если длина 0, то NaN, если длина 1, то 0, иначе - дисперсия элементов массива</returns>
        public static double Dispersion([NotNull] this double[] array)
        {
            var length = array.Length;
            switch (length)
            {
                case 0: return double.NaN;
                case 1: return 0;
                default:
                    var average = 0d;
                    var average2 = 0d;
                    for (var i = 0; i < length; i++)
                    {
                        var x = array[i];
                        average += x;
                        average2 += x * x;
                    }

                    return (average2 - average * average / length) / length;
            }
        }

        [DST, NotNull]
        public static double[] Dispersion([NotNull] this double[][] array)
        {
            var length = array.Length;
            var result = new double[length];
            for (var i = 0; i < length; i++)
                result[i] = array[i].Dispersion();
            return result;
        }

        [DST, NotNull]
        public static double[] Dispersion([NotNull] this double[,] array)
        {
            var cols = array.GetLength(1);
            var rows = array.GetLength(0);

            var average = new double[cols];
            var average2 = new double[cols];
            for (var i = 0; i < cols; i++)
            {
                average[i] = 0.0;
                for (var j = 0; j < rows; j++)
                {
                    var x = array[j, i];
                    average[i] += x * x;
                    average2[i] += x;
                }
                average[i] -= average2[i];
                average[i] /= cols;
            }
            return average;
        }

        [DST]
        public static double Dispersion_Power([NotNull] this double[] array)
        {
            var result = 0.0;
            var length = array.Length;
            for (var i = 0; i < length; i++)
            {
                var val = array[i];
                result += val * val;
            }
            return result / length;
        }

        [DST, NotNull]
        public static double[] Dispersion_Power([NotNull] this double[][] array)
        {
            var i_length = array[0].Length;
            var lv_NumArray = new double[i_length];
            for (var i = 0; i < i_length; i++)
            {
                lv_NumArray[i] = 0.0;
                var j_length = array.Length;
                for (var j = 0; j < j_length; j++)
                {
                    var val = array[j][i];
                    lv_NumArray[i] += val * val;
                }
                lv_NumArray[i] /= i_length;
            }
            return lv_NumArray;
        }

        [DST, NotNull]
        public static double[] Dispersion_Power([NotNull] this double[,] array)
        {
            var i_length = array.GetLength(1);
            var result = new double[i_length];
            for (var i = 0; i < i_length; i++)
            {
                result[i] = 0.0;
                var j_length = array.Length;
                for (var j = 0; j < j_length; j++)
                {
                    var val = array[j, i];
                    result[i] += val * val;
                }
                result[i] /= i_length;
            }
            return result;
        }

        [DST, NotNull]
        public static double[] Divide([NotNull] this double[] array, double value)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] /= value;
            return array;
        }

        [DST, NotNull]
        public static double[] GetAKF([NotNull] this double[] array) => array.GetConvolution(array.GetReversed());

        [DST, NotNull]
        public static double[] GetConvolution([NotNull] this double[] s, [NotNull] double[] h)
        {
            var k = new double[s.Length + h.Length - 1];
            for (var i = 0; i < s.Length; i++)
                for (var j = 0; j < h.Length; j++)
                    k[i + j] += s[i] * h[j];
            return k;
        }

        [DST, NotNull]
        public static CubicSpline GetCubicSpline([NotNull] this double[] Y, [NotNull] double[] X) => new CubicSpline(X, Y);

        [DST, NotNull]
        public static CubicSpline GetCubicSpline([NotNull] this double[] Y, double dx, double x0 = 0.0) => new double[Y.Length].Initialize(dx, x0, (i, Dx, X0) => i * Dx + X0).GetCubicSpline(Y);

        [DST, NotNull]
        public static double[] GetDivided([NotNull] this double[] array, double value) => new double[array.Length].Initialize(array, value, (i, a, v) => a[i] / v);

        [DST, NotNull]
        public static Histogram GetHistogram([NotNull] this double[] X, int IntervalsCount) => new Histogram(X, IntervalsCount);

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, double dx)
        {
            var s = 0.0;
            for (var i = 1; i < Y.Length; i++)
                s += Y[i] + Y[i - 1];
            return 0.5 * s * dx;
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, [NotNull] double[] X)
        {
            if (X.Length != Y.Length)
                throw new ArgumentException("Длина массива аргумента не соответствует длине массива функции.", nameof(X));
            var s = 0.0;
            for (var i = 1; i < Y.Length; i++)
                s += 0.5 * (Y[i] + Y[i - 1]) * (X[i] - X[i - 1]);
            return s;
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, [NotNull] Func<double, double, double> Core, [NotNull] double[] X)
        {
            if (X.Length != Y.Length)
                throw new ArgumentException("Длина массива аргумента не соответствует длине массива функции.", nameof(X));
            var s = 0.0;
            var core_old = Core(X[0], Y[0]);
            for (var i = 1; i < Y.Length; i++)
            {
                var dx = X[i] - X[i - 1];
                var core = Core(X[i], Y[i]);
                s += (core + core_old) * dx;
                core_old = core;
            }
            return 0.5 * s;
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, [NotNull] Func<double, double, double> Core, double dx, double x0 = 0.0)
        {
            var result = 0.0;
            var core_value_old = Core(x0, Y[0]);
            var x = x0;
            for (var i = 1; i < Y.Length; i++)
            {
                var core_value_new = Core(x, Y[i]);
                result += core_value_new + core_value_old;
                core_value_old = core_value_new;
                x += dx;
            }
            return 0.5 * result * dx;
        }

        [DST, NotNull]
        public static MNK GetMNKInterp([NotNull] this double[] Y, int m, [NotNull] double[] X) => new MNK(X, Y, m);

        [DST, NotNull]
        public static MNK GetMNKInterp([NotNull] this double[] Y, int m, double dx, double x0 = 0.0) => Y.GetMNKInterp(m, new double[Y.Length].Initialize(dx, x0, (i, Dx, X0) => i * Dx + X0));

        [DST, NotNull]
        public static double[] GetMultiplied([NotNull] this double[] array, double value) => new double[array.Length].Initialize(array, value, (i, a, v) => a[i] * v);

        [DST, NotNull]
        public static double[] GetNormalized([NotNull] this double[] array)
        {
            var max = array.Max();
            return new double[array.Length].Initialize(array, max, (i, a, m) => a[i] / m);
        }

        [DST, NotNull]
        public static double[] GetSubtract([NotNull] this double[] array, double value) => new double[array.Length].Initialize(array, value, (i, a, v) => a[i] - v);

        [DST, NotNull]
        public static double[] GetSum([NotNull] this double[] array, double value) => new double[array.Length].Initialize(array, value, (i, a, v) => a[i] + v);

        [DST]
        public static double Max([NotNull] this double[] array)
        {
            var max = new MaxValue();
            for (var i = 0; i < array.Length; i++)
                max.AddValue(array[i]);
            return max;
        }

        [DST]
        public static double Max([NotNull] this double[] array, out int MaxPos)
        {
            var max = new MaxValue();
            MaxPos = -1;
            for (var i = 0; i < array.Length; i++)
                if (max.AddValue(array[i]))
                    MaxPos = i;
            return max;
        }

        [DST]
        public static double Min([NotNull] this double[] array)
        {
            var min = new MinValue();
            for (var i = 0; i < array.Length; i++)
                min.AddValue(array[i]);
            return min;
        }

        [DST]
        public static double Min([NotNull] this double[] array, out int MinPos)
        {
            var min = new MinValue();
            MinPos = -1;
            for (var i = 0; i < array.Length; i++)
                if (min.AddValue(array[i]))
                    MinPos = i;
            return min;
        }

        [DST]
        public static void Multiply([NotNull] this double[] array, double value)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] *= value;
        }

        [DST]
        public static void Normalize([NotNull] this double[] array) => array.Divide(array.Max());

        [DST]
        public static void subtract([NotNull] this double[] array, double value)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] -= value;
        }

        public static int GetMaxIndex([NotNull] this double[] array)
        {
            var max = double.NegativeInfinity;
            var max_index = -1;

            for (var i = 0; i < array.Length; i++)
                if (array[i] > max)
                {
                    max = array[i];
                    max_index = i;
                }

            return max_index;
        }

        public static int GetMinIndex([NotNull] this double[] array)
        {
            var min = double.PositiveInfinity;
            var min_index = -1;

            for (var i = 0; i < array.Length; i++)
                if (array[i] < min)
                {
                    min = array[i];
                    min_index = i;
                }

            return min_index;
        }

        [NotNull]
        public static double[] WaveletDirectTransform([NotNull] this double[] source)
        {
            var length = source.Length;
            switch (length)
            {
                case 1:
                    return source;
                case 2:
                    return new[]
                    {
                        (source[0] - source[1]) / 2.0,
                        (source[0] + source[1]) / 2.0
                    };
                default:
                    var r = new double[length];
                    var t = new double[length / 2];
                    //var result = new List<double>(length);
                    //var temp = new List<double>(length / 2);
                    for (var i = 0; i < length / 2; i++)
                    {
                        r[i] = (source[i * 2] - source[i * 2 + 1]) / 2.0;
                        t[i] = (source[i * 2] + source[i * 2 + 1]) / 2.0;
                        //result.Add((source[i] - source[i + 1]) / 2.0);
                        //temp.Add((source[i] + source[i + 1]) / 2.0);
                    }

                    Array.Copy(t.WaveletDirectTransform(), 0, r, length / 2, length / 2);
                    //result.AddRange(temp.ToArray().WaveletDirectTransform());

                    //return result.ToArray();
                    return r;
            }
        }

        [NotNull]
        public static double[] WaveletInverseTransform([NotNull] this double[] source)
        {
            var length = source.Length;
            switch (length)
            {
                case 1:
                    return source;
                case 2:
                    return new[]
                    {
                        source[1] + source[0],
                        source[1] - source[0]
                    };
                default:
                    var r = new double[length];
                    var t = new double[length / 2];

                    Array.Copy(source, 0, t, 0, length / 2);

                    t = t.WaveletInverseTransform();

                    //var result = new List<double>(length);
                    //var temp = new List<double>(length / 2);

                    //for(var i = length / 2; i < length; i++)
                    //    temp.Add(source[i]);

                    //var second = temp.ToArray().WaveletInverseTransform();

                    for (var i = 0; i < length / 2; i++)
                    {
                        r[i * 2] = t[i] + source[i * 2];
                        r[i * 2 + 1] = t[i] + source[i * 2];
                        //result.Add(second[i] + source[i]);
                        //result.Add(second[i] - source[i]);
                    }

                    //return result.ToArray();
                    return r;
            }
        }
    }
}