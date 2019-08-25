using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore
{
    public partial class Polynom
    {
        public static class Array
        {
            [Pure]
            public static double GetValue([NotNull] double[] A, double x)
            {
                var length = A.Length;
                if (length == 0)
                    return double.NaN;
                var y = A[length - 1];
                for (var n = length - 2; n >= 0; n--)
                    y = y * x + A[n];
                return y;
            }

            [Pure, DST]
            public static Complex GetValue([NotNull] double[] A, Complex z)
            {
                var length = A.Length;
                if (length == 0)
                    return Complex.NaN;
                Complex y = A[length - 1];
                for (var n = length - 2; n >= 0; n--)
                    y = y * z + A[n];
                return y;
            }

            [Pure, DST]
            public static Complex GetValue([NotNull] Complex[] Z, Complex z)
            {
                var length = Z.Length;
                if (length == 0) return 0;
                var y = Z[length - 1];
                for (var i = 1; i < length; i++)
                    y = y * z + Z[length - 1 - i];
                return y;
            }

            public static double GetValue([NotNull] IEnumerable<double> A, double x)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));

                if (x.Equals(0d))
                    return A.First();

                var v = 0d;
                var xx = 1d;
                foreach (var a in A)
                {
                    if (!a.Equals(0d))
                        v += a * xx;
                    xx *= x;
                }

                return v;
            }

            public static Complex GetValue([NotNull] IEnumerable<double> A, Complex x)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));

                if (x.Equals(0d))
                    return A.First();

                var v = new Complex();
                var xx = new Complex(1);
                foreach (var a in A)
                {
                    if (!a.Equals(0d))
                        v += a * xx;
                    xx *= x;
                }

                return v;
            }

            /// <summary>Преобразовать массив корней полинома в коэффициенты прои степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при степенях</returns>
            [NotNull]
            public static double[] GetCoefficients([NotNull] params double[] Root)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));
                if (Root.Length == 0)
                    throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                if (Root.Length == 1)
                    return new[] { -Root[0], 1 };

                var N = Root.Length + 1;
                var a = new double[N];

                a[0] = -Root[0];
                a[1] = 1;

                for (var k = 2; k < N; k++)
                {
                    a[k] = a[k - 1];
                    for (var i = k - 1; i > 0; i--)
                        a[i] = a[i - 1] - a[i] * Root[k - 1];
                    a[0] = -a[0] * Root[k - 1];
                }

                return a;
            }

            [NotNull]
            public static double[] GetCoefficientsInverted([NotNull] params double[] Root)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));
                if (Root.Length == 0)
                    throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                if (Root.Length == 1)
                    return new[] { 1, -Root[0] };

                var N = Root.Length + 1;
                var a = new double[N];

                a[N - 1] = -Root[0];
                a[N - 1 - 1] = 1;

                for (var k = 2; k < N; k++)
                {
                    a[N - 1 - k] = a[N - 1 - k + 1];
                    for (var i = k - 1; i > 0; i--)
                        a[N - 1 - i] = a[N - 1 - (i - 1)] - a[N - 1 - i] * Root[k - 1];
                    a[N - 1] = -a[N - 1] * Root[k - 1];
                }

                return a;
            }

            /// <summary>Преобразовать массив корней полинома в коэффициенты прои степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при степенях</returns>
            [NotNull]
            public static Complex[] GetCoefficients([NotNull] params Complex[] Root)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));
                if (Root.Length == 0)
                    throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                if (Root.Length == 1)
                    return new[] { -Root[0], 1 };

                var N = Root.Length + 1;
                var a = new Complex[N];

                a[0] = -Root[0];
                a[1] = 1;

                for (var n = 2; n < N; n++)
                {
                    a[n] = a[n - 1];
                    for (var i = n - 1; i > 0; i--)
                        a[i] = a[i - 1] - a[i] * Root[n - 1];
                    a[0] = -a[0] * Root[n - 1];
                }

                return a;
            }

            [NotNull]
            public static Complex[] GetCoefficientsInverted([NotNull] params Complex[] Root)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));
                if (Root.Length == 0)
                    throw new ArgumentException("Длина массива корней полинома должна быть больше 0", nameof(Root));
                if (Root.Length == 1)
                    return new[] { 1, -Root[0] };

                var N = Root.Length + 1;
                var a = new Complex[N];

                a[N - 1] = -Root[0];
                a[N - 1 - 1] = 1;

                for (var k = 2; k < N; k++)
                {
                    a[N - 1 - k] = a[N - 1 - k + 1];
                    for (var i = k - 1; i > 0; i--)
                        a[N - 1 - i] = a[N - 1 - (i - 1)] - a[N - 1 - i] * Root[k - 1];
                    a[N - 1] = -a[N - 1] * Root[k - 1];
                }

                return a;
            }

            #region Интегрирование/дифференцирование

            [NotNull]
            public static double[] GetDifferential([NotNull] double[] p, int Order = 1)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (Order < 0)
                    throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок дифференциала не может быть меньше 0");
                if (Order == 0)
                    return (double[])p.Clone();

                while (Order-- > 0 && p.Length > 0)
                {
                    var q = new double[p.Length - 1];
                    for (var i = 0; i < q.Length; i++)
                        q[i] = p[i + 1] * (i + 1);
                    p = q;
                }
                return p;
            }

            [NotNull]
            public static Complex[] GetDifferential([NotNull] Complex[] p, int Order = 1)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (Order < 0)
                    throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок дифференциала не может быть меньше 0");
                if (Order == 0)
                    return (Complex[])p.Clone();

                while (Order-- > 0 && p.Length > 0)
                {
                    var q = new Complex[p.Length - 1];
                    for (var i = 0; i < q.Length; i++)
                        q[i] = p[i + 1] * (i + 1);
                    p = q;
                }
                return p;
            }

            [NotNull]
            public static double[] GetIntegral([NotNull] double[] p, double C = 0)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length + 1];
                result[0] = C;
                for (var i = 1; i < result.Length; i++)
                    result[i] = p[i - 1] / i;
                return result;
            }

            [NotNull]
            public static Complex[] GetIntegral([NotNull] Complex[] p, Complex C = default)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));

                var result = new Complex[p.Length + 1];
                result[0] = C;
                for (var i = 1; i < result.Length; i++)
                    result[i] = p[i - 1] / i;
                return result;
            }

            #endregion

            #region Операторы

            public static double[] Sum([NotNull] double[] p, [NotNull] double[] q)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (q is null)
                    throw new ArgumentNullException(nameof(q));
                Contract.Ensures(Contract.Result<double[]>() != null);
                Contract.EndContractBlock();

                var length_p = p.Length;
                var length_q = q.Length;
                if (length_p > length_q)
                {
                    var result = (double[])p.Clone();
                    for (var i = 0; i < length_q; i++)
                        result[i] = p[i] + q[i];
                    return result;
                }
                else
                {
                    var result = (double[])q.Clone();
                    for (var i = 0; i < length_p; i++)
                        result[i] = p[i] + q[i];
                    return result;
                }
            }

            public static double[] Substract([NotNull] double[] p, [NotNull] double[] q)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (q is null)
                    throw new ArgumentNullException(nameof(q));
                Contract.Ensures(Contract.Result<double[]>() != null);

                var length_p = p.Length;
                var length_q = q.Length;
                if (length_p > length_q)
                {
                    var result = (double[])p.Clone();
                    for (var i = 0; i < length_q; i++)
                        result[i] -= q[i];
                    return result;
                }
                else
                {
                    var result = (double[])q.Clone();
                    for (var i = 0; i < length_p; i++)
                        result[i] = p[i] - q[i];
                    for (var i = length_p; i < length_q; i++)
                        result[i] = -q[i];

                    return result;
                }
            }

            [Copyright("", url = "http://losev-al.blogspot.ru/2012/09/blog-post_14.htm")]
            public static void Devide([NotNull] double[] dividend, [NotNull] double[] divisor, [NotNull] out double[] quotient, [NotNull] out double[] remainder)
            {
                Contract.Requires(!dividend[dividend.Length - 1].Equals(0), "Старший член многочлена делимого не может быть 0");
                Contract.Requires(!divisor[divisor.Length - 1].Equals(0), "Старший член многочлена делителя не может быть 0");
                if (dividend[dividend.Length - 1].Equals(0)) throw new ArithmeticException("Старший член многочлена делимого не может быть 0");
                if (divisor[divisor.Length - 1].Equals(0)) throw new ArithmeticException("Старший член многочлена делителя не может быть 0");

                remainder = (double[])dividend.Clone();
                quotient = new double[remainder.Length - divisor.Length + 1];
                for (var i = 0; i < quotient.Length; i++)
                {
                    var k = remainder[remainder.Length - i - 1] / divisor[divisor.Length - 1];
                    quotient[quotient.Length - i - 1] = k;
                    for (var j = 0; j < divisor.Length; j++)
                        remainder[remainder.Length - i - j - 1] -= k * divisor[divisor.Length - j - 1];
                }
            }

            public static double[] Multiply(double[] p, double[] q)
            {
                var length = p.Length + q.Length;
                var a = new double[length + 1];
                for (var i = 0; i < p.Length; i++)
                    for (var j = 0; j < q.Length; j++)
                        a[i + j] += p[i] * q[j];

                var zerros_count = 0;
                for (var i = a.Length - 1; i >= 0; i--)
                    if (a[i].Equals(0d))
                        zerros_count++;
                    else
                        break;
                if (zerros_count == 0) return a;
                if (zerros_count == a.Length) return new double[0];
                System.Array.Resize(ref a, a.Length - zerros_count);
                return a;
            }

            [NotNull]
            public static double[] Add([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] + x;
                return result;
            }

            [NotNull]
            public static double[] Substract([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] - x;
                return result;
            }

            [NotNull]
            public static double[] Substract(double x, [NotNull] double[] p)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = x - p[i];
                return result;
            }

            [NotNull]
            public static double[] Negate([NotNull] double[] p)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = -p[i];
                return result;
            }

            [NotNull]
            public static double[] Multiply([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] * x;
                return result;
            }

            [NotNull]
            public static double[] Divade([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] / x;
                return result;
            }

            [NotNull]
            public static double[] DivadeScalar(double x, [NotNull] double[] p)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = x / p[i];
                return result;
            }

            #endregion
        }

        /// <summary>Результат деления полиномов</summary>
        public struct PolynomDevisionResult
        {
            /// <summary>Частное полиномов</summary>
            public readonly Polynom Result;

            /// <summary>Остаток деления полиновов</summary>
            public readonly Polynom Remainder;

            public readonly Polynom Divisor;

            /// <summary>Инициализация результата деления полиномов</summary>
            /// <param name="Divisor"></param>
            /// <param name="Result">Частоное</param>
            /// <param name="Remainder">Остаток от деления</param>
            public PolynomDevisionResult(Polynom Result, Polynom Remainder, Polynom Divisor)
            {
                this.Result = Result;
                this.Remainder = Remainder;
                this.Divisor = Divisor;
            }

            public double Value(double x) => Result.Value(x) + Remainder.Value(x) / Divisor.Value(x);

            public Func<double, double> GetFunction() => Value;

            public override string ToString() => $"({Result.ToMathString()}) + ({Remainder.ToMathString()}) / ({Divisor.ToMathString()})";

            /// <summary>Оператор неявного преобразования результата деления полиномов в полином результата</summary>
            /// <param name="Result">Результат деления полиномов</param>
            /// <returns>Частное</returns>
            public static implicit operator Polynom(PolynomDevisionResult Result) => Result.Result;
        }

        [NotNull]
        public static Polynom Random(int Power = 3, double Ma = 0, double Da = 1)
        {
            var rnd = new Random();
            var a = new double[Power];
            for (var i = 0; i < Power; i++)
                a[i] = (rnd.NextDouble() - .5) * Da + Ma;
            return new Polynom(a);
        }

        [NotNull] public static Polynom RandomPower(int MaxPower = 5, double Ma = 0, double Da = 1) => Random(new Random().Next(MaxPower), Ma, Da);

        [NotNull]
        public static Polynom RandomWithIntCoefficients(int Power, int Ma = 0, int Da = 10)
        {
            var rnd = new Random();
            var a = new double[Power];
            for (var i = 0; i < Power; i++)
                a[i] = Math.Round(rnd.Next(Da) - .5 * Da + Ma);
            return new Polynom(a);
        }

        [NotNull] public static Polynom RandomWithIntCoefficients_P(int MaxPower = 5, int Ma = 0, int Da = 10) => RandomWithIntCoefficients(new Random().Next(MaxPower), Ma, Da);
        [NotNull] public static Polynom PolynomCoefficients([NotNull] params double[] Coefficients) => new Polynom(Coefficients.FilterNullValuesFromEnd());
    }
}