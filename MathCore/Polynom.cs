using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MathCore.Annotations;

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


        /* -------------------------------------------------------------------------------------------- */

        [NotNull] public static Polynom FromCoefficients([NotNull] params double[] a) => new Polynom(a);

        [NotNull] public static Polynom FromRoots([NotNull] IEnumerable<double> roots) => FromRoots(roots.ToArray());

        /// <summary>Получить полином из корней полинома</summary>
        /// <param name="Root">Корни полинома</param>
        /// <returns>Полином с указанными корнями</returns>
        [NotNull]
        public static Polynom FromRoots([NotNull] params double[] Root)
        {
            Contract.Requires(Root != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);

            return new Polynom(Array.GetCoefficients(Root));
        }

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
        public double[] Coefficients
        {
            [Pure, DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<double[]>() != null);
                Contract.Ensures(Contract.Result<double[]>().Length > 0);
                return _a;
            }
        }

        /// <summary>Степень полинома = число коэффициентов - 1</summary>
        public int Power
        {
            [Pure, DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() == Length - 1);
                return _a.Length - 1;
            }
        }

        /// <summary>Длина полинома - число коэффициентов</summary>
        public int Length
        {
            [Pure, DebuggerStepThrough]
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);
                Contract.Ensures(Contract.Result<int>() == _a.Length);
                return _a.Length;
            }
        }

        ///<summary>
        /// Коэффициент при степени <paramref name="n"/>, где <paramref name="n"/> принадлежит [0; <see cref="Power"/>]
        /// <see cref="Power"/> = <see cref="Length"/> - 1
        /// </summary>
        ///<param name="n">Степень a[0]+a[1]*x+a[2]*x^2+...<b>+a[<paramref name="n"/>]*x^<paramref name="n"/>+</b>...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
        public double this[int n]
        {
            [Pure, DebuggerStepThrough]
            get
            {
                Contract.Requires(n >= 0);
                Contract.Requires(n < Coefficients.Length);
                return _a[n];
            }
            [DebuggerStepThrough]
            set
            {
                Contract.Requires(n >= 0);
                Contract.Requires(n < Coefficients.Length);
                _a[n] = value;
            }
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Полином степени N, нулевой элемент массива a[0] при младшей степени x^0</summary>
        /// <param name="a">a[0]+a[1]*x+a[2]*x^2+...+a[N-1]*x^(N-1)+a[N-1]*x^(N-1)</param>
        [DebuggerStepThrough]
        public Polynom([NotNull] params double[] a)
        {
            Contract.Requires(a != null);
            Contract.Requires(a.Length > 0);
            Contract.Ensures(_a != null);
            Contract.Ensures(_a.Length > 0);
            Contract.Ensures(Power >= 0);
            _a = a ?? throw new ArgumentNullException(nameof(a));
        }

        [DebuggerStepThrough]
        public Polynom([NotNull] IEnumerable<double> a)
            : this(a.ToArray())
        {
            Contract.Requires(a != null);
            Contract.Requires(a.Any());
            Contract.Ensures(_a != null);
            Contract.Ensures(_a.Length > 0);
            Contract.Ensures(Power >= 0);
        }

        [DebuggerStepThrough]
        public Polynom([NotNull] IEnumerable<int> a)
            : this(a.Select(v => (double)v))
        {
            Contract.Requires(a != null);
            Contract.Requires(a.Any());
            Contract.Ensures(_a != null);
            Contract.Ensures(_a.Length > 0);
            Contract.Ensures(Power >= 0);
        }

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Получить значение полинома</summary>
        /// <param name="x">Переменная</param>
        /// <returns>Значение полинома в точке x</returns>
        [Pure, DebuggerStepThrough]
        public double Value(double x) => Array.GetValue(_a, x);

        [Pure, DebuggerStepThrough]
        public Complex Value(Complex z) => Array.GetValue(_a, z);

        [NotNull] public Func<double, double> GetFunction() => Value;

        [NotNull] public Func<Complex, Complex> GetComplexFunction() => Value;

        [NotNull]
        public Polynom DivideTo([NotNull] Polynom Divisor, [NotNull] out Polynom Remainder)
        {
            if (Divisor is null) throw new ArgumentNullException(nameof(Divisor));

            Array.Devide(_a, Divisor._a, out var result, out var remainder);
            var n = remainder.Length;
            while (n >= 1 && remainder[n - 1].Equals(0)) n--;
            System.Array.Resize(ref remainder, n);
            Remainder = new Polynom(remainder);
            return new Polynom(result);
        }

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
                    result.Length > 0 && a > 0 ? "+" : "",
                    a.Equals(1) ? "" : (a.Equals(-1) ? "-" : a.ToString(CultureInfo.CurrentCulture)),
                    n > 0 ? "x" : "",
                    n > 1 ? "^" : "",
                    n > 1 ? n.ToString() : "");
            }
            if (length > 0 && !_a[0].Equals(0))
                result.AppendFormat("{0}{1}",
                    _a[0] < 0 ? "" : "+",
                    _a[0]);


            return result.Length == 0 ? "0" : result.ToString();
        }

        [NotNull]
        public Polynom GetDifferential(int Order = 1)
        {
            Contract.Requires(Order > 0);

            //var result = this;
            //while (Order-- > 0 && result.Length > 0)
            //    result = new Polynom(result.Select((a, i) => new { a, i }).Skip(1).Select(v => v.a * v.i));
            //return result;
            return new Polynom(Array.GetDifferential(_a, Order));
        }

        [NotNull] public Polynom GetIntegral(double C = 0) => new Polynom(Array.GetIntegral(_a, C));

        [NotNull]
        public Polynom GetInversed()
        {
            Array.Devide(new[] { 1d }, _a, out var result, out _);
            return new Polynom(result);
        }

        [NotNull]
        public Polynom ScalePolynom(double c)
        {
            var result = Clone();
            var cc = 1d;
            for (int i = 1, length = result.Length; i < length; i++)
                result[i] = result[i] * (cc *= c);
            return result;
        }

        [NotNull]
        public Polynom Substite([NotNull] Polynom P)
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

        [NotNull] public Polynom Clone() => new Polynom((double[])_a.Clone());
        object ICloneable.Clone() => Clone();

        [Pure, DebuggerStepThrough]
        public override bool Equals(object obj) => Equals(obj as Polynom);

        #endregion

        [Pure, DebuggerStepThrough]
        public override int GetHashCode() => _a.Select((i, a) => i.GetHashCode() ^ a.GetHashCode()).Aggregate(0x285da41, (S, s) => S ^ s);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<double>)this).GetEnumerator();

        IEnumerator<double> IEnumerable<double>.GetEnumerator() => ((IEnumerable<double>)_a).GetEnumerator();

        public override string ToString() =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                    a < 0 || i == 0 ? "" : "+",
                    a,
                    i == 0 ? "" : $"*x{(i == 1 ? "" : "^" + i)}")).ToString();

        public string ToString(string Format) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? "" : "+",
                a.ToString(Format),
                i == 0 ? "" : $"*x{(i == 1 ? "" : "^" + i)}")).ToString();

        public string ToString(IFormatProvider provider) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? "" : "+",
                a.ToString(provider),
                i == 0 ? "" : $"*x{(i == 1 ? "" : "^" + i)}")).ToString();

        public string ToString(string Format, IFormatProvider provider) =>
            _a.Aggregate(new StringBuilder(), (S, a, i) => S.AppendFormat("{0}{1}{2}",
                a < 0 || i == 0 ? "" : "+",
                a.ToString(Format, provider),
                i == 0 ? "" : $"*x{(i == 1 ? "" : "^" + i)}")).ToString();
    }
}