using System.Diagnostics.Contracts;
using System.Linq;
using MathCore;
using MathCore.Annotations;
using MathCore.Interpolation;
using MathCore.Statistic;
using MathCore.Values;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable CompareOfFloatsByEqualityOperator

// ReSharper disable once CheckNamespace
namespace System
{
    public static class DoubleArrayExtentions
    {
        [DST]
        public static void Add([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            for(var i = 0; i < array.Length; i++)
                array[i] += value;
        }

        [DST]
        public static void Add([NotNull] this double[] array, [NotNull] double[] values)
        {
            Contract.Requires(array != null);
            Contract.Requires(values != null);
            for(var i = 0; i < array.Length && i < values.Length; i++)
                array[i] += values[i];
        }

        [DST]
        public static double Average([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(array.Length > 0 || Contract.Result<double>() == 0);
            Contract.Ensures(array.Length == 0 || Contract.Result<double>() == array.Sum() / array.Length);
            return array.Length == 0 ? 0 : array.Sum() / array.Length;
        }

        [DST, NotNull]
        public static double[] Average([NotNull] this double[][] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == array[0].Length);
            var length_0 = array[0].Length;
            var result = new double[length_0];
            for(var i = 0; i < length_0; i++)
            {
                result[i] = 0;
                for(var n = 0; n < array.Length; n++)
                    result[i] += array[n][i];
                result[i] /= length_0;
            }
            return result;
        }

        [DST, NotNull]
        public static double[] Average([NotNull] this double[,] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[,]>() != null);
            var length_1 = array.GetLength(1);
            var result = new double[length_1];
            for(var i = 0; i < length_1; i++)
            {
                result[i] = 0;
                for(var n = 0; n < array.Length; n++)
                    result[i] += array[n, i];
                result[i] /= length_1;
            }
            return result;
        }

        [DST]
        public static double Dispersion([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(array.Length > 0 || Contract.Result<double>() == 0);
            Contract.Ensures(Contract.Result<double>() >= 0);
            var m2 = 0.0;
            var m = 0.0;
            var length = array.Length;
            for(var i = 0; i < length; i++)
            {
                var v = array[i];
                m2 += v * v;
                m += v;
            }
            return (m2 - m) / length;
        }

        [DST, NotNull]
        public static double[] Dispersion([NotNull] this double[][] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<double[]>(), d => d >= 0));
            var i_length = array[0].Length;
            var m2 = new double[i_length];
            var m = new double[i_length];
            for(var i = 0; i < i_length; i++)
            {
                m2[i] = 0.0;
                var j_length = array.Length;
                for(var j = 0; j < j_length; j++)
                {
                    var val = array[j][i];
                    m2[i] += val * val;
                    m[i] += val;
                }
                m[i] -= m2[i];
                m2[i] /= i_length;
            }
            return m2;
        }

        [DST, NotNull]
        public static double[] Dispersion([NotNull] this double[,] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[,]>() != null);
            var i_length = array.GetLength(1);
            var lv_NumArray = new double[i_length];
            var lv_NumArray2 = new double[i_length];
            for(var i = 0; i < i_length; i++)
            {
                lv_NumArray[i] = 0.0;
                var j_length = array.Length;
                for(var j = 0; j < j_length; j++)
                {
                    var num5 = array[j, i];
                    lv_NumArray[i] += num5 * num5;
                    lv_NumArray2[i] += num5;
                }
                lv_NumArray[i] -= lv_NumArray2[i];
                lv_NumArray[i] /= i_length;
            }
            return lv_NumArray;
        }

        [DST]
        public static double Dispersion_Power([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(array.Length > 0 || Contract.Result<double>() == 0);
            Contract.Ensures(Contract.Result<double>() >= 0);
            var result = 0.0;
            var length = array.Length;
            for(var i = 0; i < length; i++)
            {
                var val = array[i];
                result += val * val;
            }
            return result / length;
        }

        [DST, NotNull]
        public static double[] Dispersion_Power([NotNull] this double[][] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<double[]>(), d => d >= 0));
            var i_length = array[0].Length;
            var lv_NumArray = new double[i_length];
            for(var i = 0; i < i_length; i++)
            {
                lv_NumArray[i] = 0.0;
                var j_length = array.Length;
                for(var j = 0; j < j_length; j++)
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
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<double[]>(), d => d >= 0));
            var i_length = array.GetLength(1);
            var result = new double[i_length];
            for(var i = 0; i < i_length; i++)
            {
                result[i] = 0.0;
                var j_length = array.Length;
                for(var j = 0; j < j_length; j++)
                {
                    var val = array[j, i];
                    result[i] += val * val;
                }
                result[i] /= i_length;
            }
            return result;
        }

        [DST, NotNull]
        public static double[] Divade([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            Contract.Requires(value != 0);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(ReferenceEquals(array, Contract.Result<double[]>()));
            for(var i = 0; i < array.Length; i++)
                array[i] /= value;
            return array;
        }

        [DST, NotNull]
        public static double[] GetAKF([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            return array.GetConvolution(array.GetReversed());
        }

        [DST, NotNull]
        public static double[] GetConvolution([NotNull] this double[] s, [NotNull] double[] h)
        {
            Contract.Requires(s != null);
            Contract.Requires(h != null);
            Contract.Ensures(Contract.Result<double[]>().Length == s.Length + h.Length - 1);
            var k = new double[s.Length + h.Length - 1];
            for(var i = 0; i < s.Length; i++)
                for(var j = 0; j < h.Length; j++)
                    k[i + j] += s[i] * h[j];
            return k;
        }

        [DST, NotNull]
        public static CubicSpline GetCubicSpline([NotNull] this double[] Y, [NotNull] double[] X)
        {
            Contract.Requires(Y != null);
            Contract.Requires(X != null);
            Contract.Ensures(Contract.Result<CubicSpline>() != null);
            return new CubicSpline(X, Y);
        }

        [DST, NotNull]
        public static CubicSpline GetCubicSpline([NotNull] this double[] Y, double dx, double x0 = 0.0)
        {
            Contract.Requires(Y != null);
            Contract.Requires(dx > 0);
            Contract.Ensures(Contract.Result<CubicSpline>() != null);
            return new double[Y.Length].Initialize(dx, x0, (i, Dx, X0) => i * Dx + X0).GetCubicSpline(Y);
        }

        [DST, NotNull]
        public static double[] GetDivaded([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            Contract.Requires(value != 0);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == array.Length);
            return new double[array.Length].Initialize(array, value, (i, a, v) => a[i] / v);
        }

        [DST, NotNull]
        public static Histogram GetHistogram([NotNull] this double[] X, int IntervalsCount)
        {
            Contract.Requires(X != null);
            Contract.Requires(IntervalsCount > 0);
            Contract.Ensures(Contract.Result<Histogram>() != null);
            return new Histogram(X, IntervalsCount);
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, double dx)
        {
            Contract.Requires(Y != null);
            Contract.Requires(dx > 0);
            var s = 0.0;
            for(var i = 1; i < Y.Length; i++)
                s += Y[i] + Y[i - 1];
            return 0.5 * s * dx;
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, [NotNull] double[] X)
        {
            Contract.Requires(X != null);
            Contract.Requires(Y != null);
            Contract.Requires(X.Length == Y.Length);
            if(X.Length != Y.Length)
                throw new ArgumentException("Длина массива аргумента не соответствует длине массива функции.", nameof(X));
            var s = 0.0;
            for(var i = 1; i < Y.Length; i++)
                s += 0.5 * (Y[i] + Y[i - 1]) * (X[i] - X[i - 1]);
            return s;
        }

        [DST]
        public static double GetIntegral([NotNull] this double[] Y, [NotNull] Func<double, double, double> Core, [NotNull] double[] X)
        {
            Contract.Requires(Y != null);
            Contract.Requires(Core != null);
            Contract.Requires(X != null);
            Contract.Requires(X.Length == Y.Length);
            if(X.Length != Y.Length)
                throw new ArgumentException("Длина массива аргумента не соответствует длине массива функции.", nameof(X));
            var s = 0.0;
            var core_old = Core(X[0], Y[0]);
            for(var i = 1; i < Y.Length; i++)
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
            Contract.Requires(Y != null);
            Contract.Requires(Core != null);
            Contract.Requires(dx > 0);
            var result = 0.0;
            var core_value_old = Core(x0, Y[0]);
            var x = x0;
            for(var i = 1; i < Y.Length; i++)
            {
                var core_value_new = Core(x, Y[i]);
                result += core_value_new + core_value_old;
                core_value_old = core_value_new;
                x += dx;
            }
            return 0.5 * result * dx;
        }

        [DST, NotNull]
        public static MNK GetMNKInterp([NotNull] this double[] Y, int m, [NotNull] double[] X)
        {
            Contract.Requires(Y != null);
            Contract.Requires(X != null);
            Contract.Ensures(Contract.Result<MNK>() != null);
            return new MNK(X, Y, m);
        }

        [DST, NotNull]
        public static MNK GetMNKInterp([NotNull] this double[] Y, int m, double dx, double x0 = 0.0)
        {
            Contract.Requires(Y != null);
            Contract.Requires(dx > 0);
            Contract.Ensures(Contract.Result<MNK>() != null);
            return Y.GetMNKInterp(m, new double[Y.Length].Initialize(dx, x0, (i, Dx, X0) => i * Dx + X0));
        }

        [DST, NotNull]
        public static double[] GetMultiplyed([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == array.Length);
            return new double[array.Length].Initialize(array, value, (i, a, v) => a[i] * v);
        }

        [DST, NotNull]
        public static double[] GetNormalized([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == array.Length);
            var max = array.Max();
            return new double[array.Length].Initialize(array, max, (i, a, m) => a[i] / m);
        }

        [DST, NotNull]
        public static double[] GetSubstract([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            return new double[array.Length].Initialize(array, value, (i, a, v) => a[i] - v);
        }

        [DST, NotNull]
        public static double[] GetSumm([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(Contract.Result<double[]>().Length == array.Length);
            return new double[array.Length].Initialize(array, value, (i, a, v) => a[i] + v);
        }

        [DST]
        public static double Max([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.ForAll(array, v => v <= Contract.Result<double>()));
            Contract.Ensures(array.Length > 0 || double.IsNegativeInfinity(Contract.Result<double>()));
            Contract.Ensures(array.Length == 0 || Contract.Result<double>() == array.Max());
            var max = new MaxValue();
            for (var i = 0; i < array.Length; i++)
                max.AddValue(array[i]);
            return max;
        }

        [DST]
        public static double Max([NotNull] this double[] array, out int MaxPos)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.ForAll(array, v => v <= Contract.Result<double>()));
            Contract.Ensures(array.Length > 0 || double.IsNegativeInfinity(Contract.Result<double>()));
            Contract.Ensures(array.Length == 0 || Contract.Result<double>() == array.Max());
            Contract.Ensures(array.Length > 0 || Contract.ValueAtReturn(out MaxPos) == -1);
            Contract.Ensures(array.Length == 0 || Contract.ValueAtReturn(out MaxPos) >= 0 && Contract.ValueAtReturn(out MaxPos) < array.Length);
            Contract.Ensures(array.Length == 0 || array[Contract.ValueAtReturn(out MaxPos)] == Contract.Result<double>());
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
            Contract.Requires(array != null);
            Contract.Ensures(Contract.ForAll(array, v => v >= Contract.Result<double>()));
            Contract.Ensures(array.Length > 0 || double.IsPositiveInfinity(Contract.Result<double>()));
            Contract.Ensures(array.Length == 0 || Contract.Result<double>() == array.Min());
            var min = new MinValue();
            for (var i = 0; i < array.Length; i++)
                min.AddValue(array[i]);
            return min;
        }

        [DST]
        public static double Min([NotNull] this double[] array, out int MinPos)
        {
            Contract.Requires(array != null);
            Contract.Ensures(Contract.ForAll(array, v => v <= Contract.Result<double>()));
            Contract.Ensures(array.Length > 0 || double.IsPositiveInfinity(Contract.Result<double>()));
            Contract.Ensures(array.Length == 0 || Contract.Result<double>() == array.Min());
            Contract.Ensures(array.Length > 0 || Contract.ValueAtReturn(out MinPos) == -1);
            Contract.Ensures(array.Length == 0 || Contract.ValueAtReturn(out MinPos) >= 0 && Contract.ValueAtReturn(out MinPos) < array.Length);
            Contract.Ensures(array.Length == 0 || array[Contract.ValueAtReturn(out MinPos)] == Contract.Result<double>());
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
            Contract.Requires(array != null);
            for(var i = 0; i < array.Length; i++)
                array[i] *= value;
        }

        [DST]
        public static void Normalize([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            array.Divade(array.Max());
        }

        [DST]
        public static void Substract([NotNull] this double[] array, double value)
        {
            Contract.Requires(array != null);
            for(var i = 0; i < array.Length; i++)
                array[i] -= value;
        }

        public static int GetMaxIndex([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(array.Length > 0 || Contract.Result<int>() == -1);
            Contract.Ensures(array.Length == 0 || Contract.Result<int>() >= 0 && Contract.Result<int>() < array.Length);
            Contract.Ensures(array.Length == 0 || array[Contract.Result<int>()] == array.Max());
            var max = double.NegativeInfinity;
            var max_index = -1;

            for(var i = 0; i < array.Length; i++)
                if(array[i] > max)
                {
                    max = array[i];
                    max_index = i;
                }

            return max_index;
        }

        public static int GetMinIndex([NotNull] this double[] array)
        {
            Contract.Requires(array != null);
            Contract.Ensures(array.Length > 0 || Contract.Result<int>() == -1);
            Contract.Ensures(array.Length == 0 || Contract.Result<int>() >= 0 && Contract.Result<int>() < array.Length);
            Contract.Ensures(array.Length == 0 || array[Contract.Result<int>()] == array.Min());
            var min = double.PositiveInfinity;
            var min_index = -1;

            for(var i = 0; i < array.Length; i++)
                if(array[i] < min)
                {
                    min = array[i];
                    min_index = i;
                }

            return min_index;
        }

        [NotNull]
        public static double[] WaveletDirectTransform([NotNull] this double[] source)
        {
            Contract.Requires(source != null);
            Contract.Requires(source.Length > 0);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(source.Length != 1 || ReferenceEquals(source, Contract.Result<double[]>()));
            Contract.Ensures(Contract.Result<double[]>().Length == source.Length);

            var length = source.Length;
            switch(length)
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
                    for(var i = 0; i < length / 2; i++)
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
            Contract.Requires(source != null);
            Contract.Requires(source.Length > 0);
            Contract.Ensures(Contract.Result<double[]>() != null);
            Contract.Ensures(source.Length != 1 || ReferenceEquals(source, Contract.Result<double[]>()));
            Contract.Ensures(Contract.Result<double[]>().Length == source.Length);

            var length = source.Length;
            switch(length)
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

                    for(var i = 0; i < length / 2; i++)
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