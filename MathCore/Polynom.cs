using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>
    /// Полином степени N-1
    ///  a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)
    /// где N - число элементов массива коэффициентов
    /// Нулевой элемент массива при нулевой степени члена полинома 
    /// </summary>
    [Serializable]
    //[DebuggerDisplay("GetPower = {GetPower}")]
    public partial class Polynom : ICloneable<Polynom>, IEquatable<Polynom>, IEnumerable<double>, IFormattable
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Создание нового полинома из массива его коэффициентов</summary>
        /// <param name="a">Массив коэффициентов полинома</param>
        /// <returns>Полином, созданный на основе массива его коэффициентов</returns>
        [NotNull] public static Polynom FromCoefficients([NotNull] params double[] a) => new(a);

        /// <summary>Создание нового полинома на основе его корней</summary>
        /// <param name="roots">Корни полинома</param>
        /// <returns>Полином, собранный из массива корней</returns>
        [NotNull] public static Polynom FromRoots([NotNull] IEnumerable<double> roots) => FromRoots(roots.ToArray());

        /// <summary>Получить полином из корней полинома</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Полином с указанными корнями</returns>
        [NotNull]
        public static Polynom FromRoots([NotNull] params double[] Root) => new(Array.GetCoefficients(Root));

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Коэффициенты при степенях</summary>
        /// <remarks>a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</remarks>
        [NotNull] private readonly double[] _a;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>
        /// Коэффициенты при степенях
        ///   a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)
        /// </summary>
        [NotNull, XmlArray(ElementName = "a")]
        public double[] Coefficients => _a;

        /// <summary>Степень полинома = число коэффициентов - 1</summary>
        public int Power => _a.Length - 1;

        /// <summary>Длина полинома - число коэффициентов</summary>
        public int Length => _a.Length;

        ///<summary>
        /// Коэффициент при степени <paramref name="n"/>, где <paramref name="n"/> принадлежит [0; <see cref="Power"/>]
        /// <see cref="Power"/> = <see cref="Length"/> - 1
        /// </summary>
        ///<param name="n">Степень a[0]+a[1]*x+a[2]*x^2+...<b>+a[<paramref name="n"/>]*x^<paramref name="n"/>+</b>...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
        public ref double this[int n] { [DST] get => ref _a[n]; }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Полином степени N, нулевой элемент массива a[0] при младшей степени x^0</summary>
        /// <param name="a">a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
        [DST]
        public Polynom([NotNull] params double[] a) => _a = a ?? throw new ArgumentNullException(nameof(a));

        /// <inheritdoc />
        [DST]
        public Polynom([NotNull] IEnumerable<double> a) : this(a.ToArray()) { }

        /// <inheritdoc />
        [DST]
        public Polynom([NotNull] IEnumerable<int> a) : this(a.Select(v => (double)v)) { }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить значение полинома</summary>
        /// <param name="x">Аргумент</param>
        /// <returns>Значение полинома в точке x</returns>
        [DST]
        public double Value(double x) => Array.GetValue(x, _a);

        /// <summary>Вычислить комплексное значение полинома</summary>
        /// <param name="z">Комплексный аргумент</param>
        /// <returns>Комплексное значение полинома в точке</returns>
        [DST]
        public Complex Value(Complex z) => Array.GetValue(z, _a);

        /// <summary>Получить функцию полинома</summary>
        /// <returns>Функция полинома</returns>
        [NotNull] public Func<double, double> GetFunction() => Value;

        /// <summary>Получить комплексную функцию полинома</summary>
        /// <returns>Комплексная функция полинома</returns>
        [NotNull] public Func<Complex, Complex> GetComplexFunction() => Value;

        /// <summary>Выполнить операцию деления полинома на полином</summary>
        /// <param name="Divisor">Полином - делимое</param>
        /// <param name="Remainder">Полином - делитель</param>
        /// <returns>Полином - частное</returns>
        [NotNull]
        public Polynom DivideTo([NotNull] Polynom Divisor, [NotNull] out Polynom Remainder)
        {
            if (Divisor is null) throw new ArgumentNullException(nameof(Divisor));

            Array.Divide(_a, Divisor._a, out var result, out var remainder);
            var n = remainder.Length;
            while (n >= 1 && remainder[n - 1].Equals(0)) n--;
            System.Array.Resize(ref remainder, n);
            Remainder = new Polynom(remainder);
            return new Polynom(result);
        }

        /// <summary>Представить полином в виде математической записи в степенной форме</summary>
        /// <returns>Строковое представление полинома в степенной форме</returns>
        [NotNull]
        public string ToMathString()
        {
            var result = new StringBuilder();
            var length = _a.Length;
            for (var n = _a.Length - 1; n > 0; n--)
            {
                var a = _a[n];
                if (a.Equals(0)) continue;
                result.AppendFormat("{0}{1}{2}{3}{4}",
                    result.Length > 0 && a > 0 ? "+" : string.Empty,
                    a.Equals(1) ? string.Empty : (a.Equals(-1) ? "-" : a.ToString(CultureInfo.CurrentCulture)),
                    n > 0 ? "x" : string.Empty,
                    n > 1 ? "^" : string.Empty,
                    n > 1 ? n.ToString() : string.Empty);
            }
            if (length > 0 && !_a[0].Equals(0))
                result.AppendFormat("{0}{1}",
                    _a[0] < 0 ? string.Empty : "+",
                    _a[0]);


            return result.Length == 0 ? "0" : result.ToString();
        }

        /// <summary>Дифференцирование полинома</summary>
        /// <param name="Order">Порядок дифференциала</param>
        /// <returns>Полином - результат дифференцирования</returns>
        [NotNull]
        public Polynom GetDifferential(int Order = 1) => new(Array.GetDifferential(_a, Order));

        /// <summary>Интегрирование полинома</summary>
        /// <param name="C">Константа интегрирования</param>
        /// <returns>Полином - результат интегрирования полинома</returns>
        [NotNull] public Polynom GetIntegral(double C = 0) => new(Array.GetIntegral(_a, C));

        /// <summary>Вычислить обратный полином</summary>
        /// <returns>Полином, являющийся обратным к текущим</returns>
        [NotNull]
        public Polynom GetInversed()
        {
            Array.Divide(new[] { 1d }, _a, out var result, out _);
            return new Polynom(result);
        }

        /// <summary>Масштабирование полинома Q(x) = P(x * c)</summary>
        /// <param name="c">Коэффициент масштабирования полинома</param>
        /// <returns>Отмасштабированный полином</returns>
        [NotNull]
        public Polynom ScalePolynom(double c)
        {
            var result = Clone();
            var a = result._a;
            var cc = 1d;
            for (int i = 1, length = a.Length; i < length; i++)
                a[i] = a[i] * (cc *= c);
            return result;
        }

        /// <summary>Подстановка полинома x' = P(x) в полином Q(x')=Q(P(x))</summary>
        /// <param name="P">Полином - подстановка</param>
        /// <returns>Полином - результат подстановки</returns>
        [NotNull]
        public Polynom Substitute([NotNull] Polynom P)
        {
            var p = P.Clone();
            var result = new Polynom(_a[0]);
            var i = 1;
            for (; i < _a.Length - 1; i++)
            {
                result += _a[i] * p;
                p *= P;
            }
            if (i < _a.Length) result += _a[i] * p;
            return result;
        }

        /* -------------------------------------------------------------------------------------------- */

        #region Implementation of IEquatable<Polynom>

        /// <inheritdoc />
        public bool Equals(Polynom other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other) || ReferenceEquals(_a, other._a)) return true;

            var a = _a;
            var b = other._a;
            var length = a.Length;

            if (length != b.Length) return false;
            for (var i = 0; i < length; i++)
                if (!_a[i].Equals(other._a[i]))
                    return false;
            return true;
        }

        /// <inheritdoc />
        [NotNull] public Polynom Clone() => new((double[])_a.Clone());

        /// <inheritdoc />
        object ICloneable.Clone() => Clone();

        /// <inheritdoc />
        [DST]
        public override bool Equals(object obj) => Equals(obj as Polynom);

        #endregion

        /// <inheritdoc />
        [DST]
        public override int GetHashCode() => _a.Select((i, a) => i.GetHashCode() ^ a.GetHashCode()).Aggregate(0x285da41, (S, s) => S ^ s);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<double>)this).GetEnumerator();

        /// <inheritdoc />
        IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)_a).GetEnumerator();

        /// <inheritdoc />
        [NotNull]
        public override string ToString() =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                    a < 0 || i == 0 ? string.Empty : "+",
                    a,
                    i == 0 ? string.Empty : $"*x{(i == 1 ? string.Empty : "^" + i)}")).ToString();

        /// <summary>Строковое представление полинома с форматированием</summary>
        /// <param name="Format">Строка форматирования</param>
        /// <returns>Форматированное представление полинома</returns>
        [NotNull]
        public string ToString(string Format) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? string.Empty : "+",
                a.ToString(Format),
                i == 0 ? string.Empty : $"*x{(i == 1 ? string.Empty : "^" + i)}")).ToString();

        [NotNull]
        public string ToString(IFormatProvider provider) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? string.Empty : "+",
                a.ToString(provider),
                i == 0 ? string.Empty : $"*x{(i == 1 ? string.Empty : "^" + i)}")).ToString();

        public string ToString(string Format, IFormatProvider provider) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? string.Empty : "+",
                a.ToString(Format, provider),
                i == 0 ? string.Empty : $"*x{(i == 1 ? string.Empty : "^" + i)}")).ToString();
    }
}