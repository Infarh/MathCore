#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore
{
    public partial class Polynom
    {
        /// <summary>Операции над коэффициентами полинома, в представлении массива значений</summary>
        public static class Array
        {
            /// <summary>Рассчитать значение полинома</summary>
            /// <param name="x">Аргумент полинома</param>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <returns>Значение полинома</returns>
            public static double GetValue(double x, params double[] A)
            {
                var length = A.Length;
                if (length == 0) return double.NaN;
                var y = A[length - 1];
                for (var n = length - 2; n >= 0; n--)
                    y = y * x + A[n];
                return y;
            }

            /// <summary>Рассчитать комплексное значение полинома</summary>
            /// <param name="z">Аргумент полинома</param>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <returns>Комплексное значение полинома</returns>
            [DST]
            public static Complex GetValue(Complex z, params double[] A)
            {
                var length = A.Length;
                if (length == 0)
                    return Complex.NaN;
                Complex y = A[length - 1];
                for (var n = length - 2; n >= 0; n--)
                    y = y * z + A[n];
                return y;
            }

            /// <summary>Рассчитать комплексное значение полинома с комплексными коэффициентами</summary>
            /// <param name="z">Комплексный аргумент полинома</param>
            /// <param name="Z">Массив комплексных коэффициентов полинома</param>
            /// <returns>Комплексное значение полинома</returns>
            [DST]
            public static Complex GetValue(Complex z, params Complex[] Z)
            {
                var length = Z.Length;
                if (length == 0) return 0;
                var y = Z[length - 1];
                for (var i = 1; i < length; i++)
                    y = y * z + Z[length - 1 - i];
                return y;
            }

            /// <summary>Рассчитать значение полинома</summary>
            /// <param name="x">Аргумент полинома</param>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <returns>Значение полинома</returns>
            public static double GetValue(double x, IEnumerable<double> A)
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

            /// <summary>Рассчитать комплексное значение полинома</summary>
            /// <param name="z">Аргумент полинома</param>
            /// <param name="A">Массив коэффициентов полинома</param>
            /// <returns>Комплексное значение полинома</returns>
            public static Complex GetValue(Complex z, IEnumerable<double> A)
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
            public static double[] GetCoefficients(params double[] Root)
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
                    a[0] *= -Root[k - 1];
                }

                return a;
            }

            /// <summary>Преобразовать последовательность корней полинома в коэффициенты при степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при степенях</returns>
            public static double[] GetCoefficients(IEnumerable<double> Root)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));

                using var root = Root.GetEnumerator();
                if (!root.MoveNext())
                    throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

                var a = new List<double> { -root.Current, 1 };

                while (root.MoveNext())
                {
                    var r = root.Current;
                    for (var i = a.Count - 1; i > 0; i--)
                        a[i] = a[i - 1] - a[i] * r;
                    a[0] *= -r;
                    a.Add(1);
                }

                return a.ToArray();
            }

            public static void GetCoefficients(IEnumerable<double> Root, IList<double> Coefficients)
            {
                if (Root is null)
                    throw new ArgumentNullException(nameof(Root));

                if (Coefficients.IsReadOnly)
                    throw new ArgumentException("Список коэффициентов невозможно изменить", nameof(CommandLineArgs))
                    {
                        Data =
                        {
                            { "CoefficientsType", Coefficients.GetType() }
                        }
                    };

                using var root = Root.GetEnumerator();
                if (!root.MoveNext())
                    throw new ArgumentException("Длина перечисления корней полинома должна быть больше 0", nameof(Root));

                switch (Coefficients.Count)
                {
                    default:
                        Coefficients.Clear();
                        goto case 0;
                    case 0:
                        Coefficients.Add(-root.Current);
                        Coefficients.Add(1);
                        break;
                    case 2:
                        Coefficients[0] = -root.Current;
                        Coefficients[1] = 1;
                        break;
                }

                if (Coefficients.Count > 0)
                    Coefficients.Clear();

                Coefficients.Add(-root.Current);
                Coefficients.Add(1);

                while (root.MoveNext())
                {

                    var r = root.Current;
                    for (var i = Coefficients.Count - 1; i > 0; i--)
                        Coefficients[i] = Coefficients[i - 1] - Coefficients[i] * r;
                    Coefficients[0] *= -r;

                    Coefficients.Add(1);
                }
            }

            /// <summary>Преобразовать массив корней полинома в коэффициенты при обратных степенях</summary>
            /// <param name="Root">Корни полинома</param>
            /// <returns>Коэффициенты при обратных степенях</returns>

            public static double[] GetCoefficientsInverted(params double[] Root)
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

            public static Complex[] GetCoefficients(params Complex[] Root)
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
            public static Complex[] GetCoefficientsInverted(params Complex[] Root)
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

            public static double[] GetDifferential(double[] p, int Order = 1)
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

            public static Complex[] GetDifferential(Complex[] p, int Order = 1)
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

            public static double[] GetIntegral(double[] p, double C = 0)
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

            public static Complex[] GetIntegral(Complex[] p, Complex C = default)
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

            public static double[] Sum(double[] p, double[] q)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (q is null)
                    throw new ArgumentNullException(nameof(q));

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

            public static double[] Subtract(double[] p, double[] q)
            {
                if (p is null)
                    throw new ArgumentNullException(nameof(p));
                if (q is null)
                    throw new ArgumentNullException(nameof(q));

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
            /// <param name="quotient">Коэффициенты полинома - частного</param>
            /// <param name="remainder">Коэффициенты полинома - остаток от деления</param>
            /// <returns>Коэффициенты полинома - частное</returns>
            // ReSharper disable once EmptyString
            [Copyright("", url = "http://losev-al.blogspot.ru/2012/09/blog-post_14.htm")]
            public static void Divide(double[] dividend, double[] divisor, out double[] quotient, out double[] remainder)
            {
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
            public static double[] Multiply(double[] p, double[] q)
            {
                var length = p.Length + q.Length;
                var a = new double[length + 1];
                for (var i = 0; i < p.Length; i++)
                    for (var j = 0; j < q.Length; j++)
                        a[i + j] += p[i] * q[j];

                var zeros_count = 0;
                for (var i = a.Length - 1; i >= 0; i--)
                    if (a[i].Equals(0d))
                        zeros_count++;
                    else
                        break;
                if (zeros_count == 0) return a;
                if (zeros_count == a.Length) return System.Array.Empty<double>();
                System.Array.Resize(ref a, a.Length - zeros_count);
                return a;
            }

            /// <summary>Сложение полинома с вещественным числом</summary>
            /// <param name="p">Коэффициенты полинома</param>
            /// <param name="x">Вещественное число</param>
            /// <returns>Коэффициенты полинома - суммы</returns>

            public static double[] Add(double[] p, double x)
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

            public static double[] Subtract(double[] p, double x)
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

            public static double[] Subtract(double x, double[] p)
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

            public static double[] Negate(double[] p)
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

            public static double[] Multiply(double[] p, double x)
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

            public static double[] Divide(double[] p, double x)
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

            public static double[] DivideScalar(double x, double[] p)
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
        public readonly struct PolynomDivisionResult
        {
            /// <summary>Частное полиномов</summary>
            public readonly Polynom Result;

            /// <summary>Остаток деления полиномов</summary>
            public readonly Polynom Remainder;

            /// <summary>Полином - делитель</summary>
            public readonly Polynom Divisor;

            /// <summary>Инициализация результата деления полиномов</summary>
            /// <param name="Divisor"></param>
            /// <param name="Result">Частное</param>
            /// <param name="Remainder">Остаток от деления</param>
            public PolynomDivisionResult(Polynom Result, Polynom Remainder, Polynom Divisor)
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

            /// <inheritdoc />
            public override string ToString() => $"({Result.ToMathString()}) + ({Remainder.ToMathString()}) / ({Divisor.ToMathString()})";

            /// <summary>Оператор неявного преобразования результата деления полиномов в полином результата</summary>
            /// <param name="Result">Результат деления полиномов</param>
            /// <returns>Частное</returns>
            public static implicit operator Polynom(PolynomDivisionResult Result) => Result.Result;
        }

        /// <summary>Случайный полином</summary>
        /// <param name="Power">Степень полинома</param>
        /// <param name="Ma">Математическое ожидание коэффициентов полинома</param>
        /// <param name="Da">Дисперсия коэффициентов полинома</param>
        /// <returns>Случайный полином</returns>

        public static Polynom Random(int Power = 3, double Ma = 0, double Da = 1)
        {
            var rnd = new Random();
            var a = new double[Power];
            for (var i = 0; i < Power; i++)
                a[i] = (rnd.NextDouble() - .5) * Da + Ma;
            return new Polynom(a);
        }

        public static Polynom RandomPower(int MaxPower = 5, double Ma = 0, double Da = 1) => Random(new Random().Next(MaxPower), Ma, Da);


        public static Polynom RandomWithIntCoefficients(int Power, int Ma = 0, int Da = 10)
        {
            var rnd = new Random();
            var a = new double[Power];
            for (var i = 0; i < Power; i++)
                a[i] = Math.Round(rnd.Next(Da) - .5 * Da + Ma);
            return new Polynom(a);
        }

        public static Polynom RandomWithIntCoefficients_P(int MaxPower = 5, int Ma = 0, int Da = 10) => RandomWithIntCoefficients(new Random().Next(MaxPower), Ma, Da);

        public static Polynom PolynomCoefficients(params double[] Coefficients) => new(Coefficients.FilterNullValuesFromEnd());
    }
}