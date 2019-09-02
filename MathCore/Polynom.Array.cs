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
        /// <summary>Операции над коэффициентами полинома, в представлении массива значений</summary>
        public static class Array
        {
            /// <summary>Расчитать значение полинома</summary>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <param name="x">Аргумент полинома</param>
            /// <returns>Значение полинома</returns>
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

            /// <summary>Расчитать комплексное значение полинома</summary>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <param name="z">Аргумент полинома</param>
            /// <returns>Комплексное значение полинома</returns>
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

            /// <summary>Расчитать комплексное значение полинома с комплексными коэффициентами</summary>
            /// <param name="Z">Массив комплексных коэффициентов полинома</param>
            /// <param name="z">Комплексный аргумент полинома</param>
            /// <returns>Комплексное значение полинома</returns>
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

            /// <summary>Расчитать значение полинома</summary>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <param name="x">Аргумент полинома</param>
            /// <returns>Значение полинома</returns>
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

            /// <summary>Расчитать комплексное значение полинома</summary>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <param name="z">Аргумент полинома</param>
            /// <returns>Комплексное значение полинома</returns>
            public static Complex GetValue([NotNull] IEnumerable<double> A, Complex z)
            {
                if (A is null)
                    throw new ArgumentNullException(nameof(A));

                if (z.Equals(0d))
                    return A.First();

                var v = new Complex();
                var xx = new Complex(1);
                foreach (var a in A)
                {
                    if (!a.Equals(0d))
                        v += a * xx;
                    xx *= z;
                }

                return v;
            }

            /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
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

            /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при обратных степенях</returns>
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

            /// <summary>Преобразовать массив корней полинома в коэффициенты при степенях</summary>
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

            /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при обратных степенях</returns>

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

            /// <summary>Дифференциал полинома</summary>
            /// <param name="p">Массив коэффициентов полинома</param>
            /// <param name="Order">Порядок дифференцирования</param>
            /// <returns>Массив коэффициентов полинома - дифференциала</returns>
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

            /// <summary>Дифференциал полинома</summary>
            /// <param name="p">Массив комплексных коэффициентов полинома</param>
            /// <param name="Order">Порядок дифференцирования</param>
            /// <returns>Массив комплексных коэффициентов полинома - дифференциала</returns>
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

            /// <summary>Интеграл полинома</summary>
            /// <param name="p">Массив коэффициентов полинома</param>
            /// <param name="C">Константа интегрирования</param>
            /// <returns>Массив коэффициентов полинома - интеграла</returns>
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

            /// <summary>Интеграл полинома</summary>
            /// <param name="p">Массив комплексных коэффициентов полинома</param>
            /// <param name="C">Константа интегрирования</param>
            /// <returns>Массив комплексных коэффициентов полинома - интеграла</returns>
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

            /// <summary>Суммирование полиномов</summary>
            /// <param name="p">Коэффициенты полинома - первого слагаемого</param>
            /// <param name="q">Коэффициенты полинома - первого слагаемого</param>
            /// <returns>Коэффициенты полинома - суммы</returns>
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

            /// <summary>Разность полиномов</summary>
            /// <param name="p">Коэффициенты полинома - первого уменьшаемого</param>
            /// <param name="q">Коэффициенты полинома - первого вычитаемого</param>
            /// <returns>Коэффициенты полинома - разности</returns>
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

            /// <summary>Деление полиномов</summary>
            /// <param name="dividend">Коэффициенты полинома - делимое</param>
            /// <param name="divisor">Коэффициенты полинома - делитель</param>
            /// <param name="quotient">Коэффициенты полинома - остаток от деления</param>
            /// <returns>Коэффициенты полинома - частное</returns>
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

            /// <summary>Умножение полиномов</summary>
            /// <param name="p">Коэффициенты полинома - первый сомножитель</param>
            /// <param name="q">Коэффициенты полинома - второй сомножитель</param>
            /// <returns>Коэффициенты полинома - произведение</returns>
            public static double[] Multiply([NotNull] double[] p, [NotNull] double[] q)
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

            /// <summary>Сложение полинома с вещественным числом</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - суммы</returns>
            [NotNull]
            public static double[] Add([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] + x;
                return result;
            }

            /// <summary>Вычитание вещественного числа из полинома</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - разности</returns>
            [NotNull]
            public static double[] Substract([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] - x;
                return result;
            }

            /// <summary>Вычитание полинома из вещественного числа</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - разности</returns>
            [NotNull]
            public static double[] Substract(double x, [NotNull] double[] p)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = x - p[i];
                return result;
            }

            /// <summary>Отрицание полинома</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <returns>Коэффициенты полинома Q(x) = 0 - P(x)</returns>
            [NotNull]
            public static double[] Negate([NotNull] double[] p)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = -p[i];
                return result;
            }

            /// <summary>Умножение полинома на вещественное число</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - произведения</returns>
            [NotNull]
            public static double[] Multiply([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] * x;
                return result;
            }

            /// <summary>Деление полинома на вещественное число</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - частного</returns>
            [NotNull]
            public static double[] Divade([NotNull] double[] p, double x)
            {
                if (p is null) throw new ArgumentNullException(nameof(p));

                var result = new double[p.Length];
                for (var i = 0; i < result.Length; i++)
                    result[i] = p[i] / x;
                return result;
            }

            /// <summary>Скалярное деление полинома на вещественное число</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - частного</returns>
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
        public readonly struct PolynomDevisionResult
        {
            /// <summary>Частное полиномов</summary>
            public readonly Polynom Result;

            /// <summary>Остаток деления полиновов</summary>
            public readonly Polynom Remainder;

            /// <summary>Полином - делитель</summary>
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

            /// <summary>Значение результата деления полиномов</summary>
            /// <param name="x">Аргумент</param>
            /// <returns>Результат вычисления значения результата деления полиномов</returns>
            public double Value(double x) => Result.Value(x) + Remainder.Value(x) / Divisor.Value(x);

            /// <summary>Получить функцию</summary>
            /// <returns>Функция вычисления значения результата деления полиномов</returns>
            public Func<double, double> GetFunction() => Value;

            public override string ToString() => $"({Result.ToMathString()}) + ({Remainder.ToMathString()}) / ({Divisor.ToMathString()})";

            /// <summary>Оператор неявного преобразования результата деления полиномов в полином результата</summary>
            /// <param name="Result">Результат деления полиномов</param>
            /// <returns>Частное</returns>
            public static implicit operator Polynom(PolynomDevisionResult Result) => Result.Result;
        }

        /// <summary>Случайный полином</summary>
        /// <param name="Power">Степень полинома</param>
        /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
        /// <param name="Da">Дисперсия коэффициентов полинома</param>
        /// <returns>Случайный полином</returns>
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