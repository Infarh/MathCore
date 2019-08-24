using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using MathCore.Annotations;

namespace MathCore
{
    public partial class Polynom
    {
        /// <summary>Явное приведение типов полинома к делегату функции преобразования double->double</summary>
        /// <param name="P">Полином</param>
        /// <returns>Делегат функции преобразования</returns>
        [Pure, DebuggerStepThrough, NotNull]
        public static implicit operator Func<double, double>([NotNull] Polynom P)
        {
            Contract.Ensures(Contract.Result<Func<double, double>>() != null);
            return P.Value;
        }

        [Pure, DebuggerStepThrough, NotNull]
        public static implicit operator Func<Complex, Complex>([NotNull] Polynom P)
        {
            Contract.Ensures(Contract.Result<Func<Complex, Complex>>() != null);
            return P.Value;
        }

        /// <summary>Оператор сложения двух полиномов</summary>
        /// <param name="P">Первое слагаемое</param>
        /// <param name="Q">Второе слагаемое</param>
        /// <returns>Сумма полиномов</returns>
        [DebuggerStepThrough, Pure, NotNull]
        public static Polynom operator +([NotNull] Polynom P, [NotNull] Polynom Q)
        {
            Contract.Requires(P != null);
            Contract.Requires(Q != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);
            Contract.Ensures(Contract.Result<Polynom>().Power == Math.Max(P.Power, Q.Power));

            return new Polynom(Array.Sum(P._a, Q._a));
        }

        /// <summary>
        /// Оператор отрицания полинома (изменяет знак всех коэффициентов на обратной). Эквивалентно домножению полинома на -1
        /// </summary>
        /// <param name="P">Отрицаемый полином</param>
        /// <returns>Полином Q = -P</returns>
        [NotNull]
        public static Polynom operator -([NotNull] Polynom P)
        {
            Contract.Requires(P != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);

            return new Polynom(Array.Negate(P._a));
        }

        /// <summary>Оператор вычетания полинома Q из полинома P</summary>
        /// <param name="P">Уменьшаемое</param>
        /// <param name="Q">Вычитаемое</param>
        /// <returns>Разность</returns>
        public static Polynom operator -([NotNull] Polynom P, [NotNull] Polynom Q)
        {
            Contract.Requires(P != null);
            Contract.Requires(Q != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);
            Contract.Ensures(Contract.Result<Polynom>().Power == Math.Max(P.Power, Q.Power));

            return new Polynom(Array.Substract(P._a, Q._a));
        }

        [NotNull]
        public static Polynom operator *([NotNull] Polynom P, [NotNull] Polynom Q)
        {
            Contract.Requires(P != null);
            Contract.Requires(Q != null);
            Contract.Ensures(Contract.Result<Polynom>() != null);

            return new Polynom(Array.Multiply(P._a, Q._a));
        }

        /// <summary>Оператор деления двух полиномов</summary>
        /// <param name="p">Полином делимого</param>
        /// <param name="q">Полином делителя</param>
        /// <returns>Результат деления полиномов, включающий частное и остаток от деления</returns>
        public static PolynomDevisionResult operator /([NotNull] Polynom p, [NotNull] Polynom q) =>
            new PolynomDevisionResult(p.DivideTo(q, out var remainder), remainder, q);

        [NotNull]
        public static Polynom operator *([NotNull] Polynom P, double q) => new Polynom(Array.Multiply(P._a, q));

        [NotNull]
        public static Polynom operator *(double p, [NotNull] Polynom Q) => new Polynom(Array.Multiply(Q._a, p));

        [NotNull]
        public static Polynom operator /([NotNull] Polynom P, double q) => new Polynom(Array.Divade(P._a, q));

        /// <summary>Оператор неявного преведения типа полинома в массив вещественных значений коэффициентов</summary>
        /// <param name="p">Полином</param>
        /// <returns>Массив значений коэффициентов</returns>
        [DebuggerStepThrough, NotNull]
        public static explicit operator double[] ([NotNull] Polynom p) => p.Coefficients;

        [DebuggerStepThrough, NotNull]
        public static implicit operator Polynom(double a) => new Polynom(a);

        [DebuggerStepThrough, NotNull]
        public static implicit operator Polynom(float a) => new Polynom(a);

        [DebuggerStepThrough, NotNull]
        public static implicit operator Polynom(int a) => new Polynom(a);

        [DebuggerStepThrough, NotNull]
        public static implicit operator Polynom(short a) => new Polynom(a);
    }
}