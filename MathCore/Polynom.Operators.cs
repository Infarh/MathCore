using System;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;
using System.Diagnostics.Contracts;
using MathCore.Annotations;

namespace MathCore
{
    public partial class Polynom
    {
        /// <summary>Явное приведение типов полинома к делегату функции преобразования double->double</summary>
        /// <param name="P">Полином</param>
        /// <returns>Делегат функции преобразования</returns>
        [Pure, DST, NotNull]
        public static implicit operator Func<double, double>([NotNull] Polynom P) => P.GetFunction();

        /// <summary>Явное приведение типов полинома к делегату комплексной функции преобразования Complex->Complex</summary>
        /// <param name="P">Полином</param>
        /// <returns>Делегат комплексной функции преобразования</returns>
        [Pure, DST, NotNull]
        public static implicit operator Func<Complex, Complex>([NotNull] Polynom P) => P.GetComplexFunction();

        /// <summary>Оператор сложения двух полиномов</summary>
        /// <param name="P">Первое слагаемое</param>
        /// <param name="Q">Второе слагаемое</param>
        /// <returns>Сумма полиномов</returns>
        [DST, Pure, NotNull]
        public static Polynom operator +([NotNull] Polynom P, [NotNull] Polynom Q) => new Polynom(Array.Sum(P._a, Q._a));

        /// <summary>
        /// Оператор отрицания полинома (изменяет знак всех коэффициентов на обратной). Эквивалентно домножению полинома на -1
        /// </summary>
        /// <param name="P">Отрицаемый полином</param>
        /// <returns>Полином Q = -P</returns>
        [NotNull]
        public static Polynom operator -([NotNull] Polynom P) => new Polynom(Array.Negate(P._a));

        /// <summary>Оператор вычетания полинома Q из полинома P</summary>
        /// <param name="P">Уменьшаемое</param>
        /// <param name="Q">Вычитаемое</param>
        /// <returns>Разность</returns>
        [NotNull] public static Polynom operator -([NotNull] Polynom P, [NotNull] Polynom Q) => new Polynom(Array.Substract(P._a, Q._a));

        [NotNull]
        public static Polynom operator *([NotNull] Polynom P, [NotNull] Polynom Q) => new Polynom(Array.Multiply(P._a, Q._a));

        /// <summary>Оператор деления двух полиномов</summary>
        /// <param name="p">Полином делимого</param>
        /// <param name="q">Полином делителя</param>
        /// <returns>Результат деления полиномов, включающий частное и остаток от деления</returns>
        public static PolynomDevisionResult operator /([NotNull] Polynom p, [NotNull] Polynom q) =>
            new PolynomDevisionResult(p.DivideTo(q, out var remainder), remainder, q);

        /// <summary>Умножение полинома на вещественное число</summary>
        /// <param name="P">Полином</param>
        /// <param name="q">Вещественное число</param>
        /// <returns>Полином - результат умножения исходного полинома на вещественное число</returns>
        [NotNull]
        public static Polynom operator *([NotNull] Polynom P, double q) => new Polynom(Array.Multiply(P._a, q));

        /// <summary>Умножение полинома на вещественное число</summary>
        /// <param name="p">Вещественное число</param>
        /// <param name="Q">Полином</param>
        /// <returns>Полином - результат умножения исходного полинома на вещественное число</returns>
        [NotNull]
        public static Polynom operator *(double p, [NotNull] Polynom Q) => new Polynom(Array.Multiply(Q._a, p));

        /// <summary>Деление полинома на вещественное число</summary>
        /// <param name="P">Полином</param>
        /// <param name="q">Вещественное число</param>
        /// <returns>Полином - результат деления исходного полинома на вещественное число</returns>
        [NotNull]
        public static Polynom operator /([NotNull] Polynom P, double q) => new Polynom(Array.Divade(P._a, q));

        /// <summary>Оператор неявного преведения типа полинома в массив вещественных значений коэффициентов</summary>
        /// <param name="p">Полином</param>
        /// <returns>Массив значений коэффициентов</returns>
        [DST, NotNull]
        public static explicit operator double[] ([NotNull] Polynom p) => p.Coefficients;

        /// <summary>Оператор приведения типа вещественного числа к типу полинома</summary>
        /// <param name="a">Вещественное число</param>
        [DST, NotNull]
        public static implicit operator Polynom(double a) => new Polynom(a);

        /// <summary>Оператор приведения типа вещественного числа к типу полинома</summary>
        /// <param name="a">Вещественное число</param>
        [DST, NotNull]
        public static implicit operator Polynom(float a) => new Polynom(a);

        /// <summary>Оператор приведения типа целого числа к типу полинома</summary>
        /// <param name="a">Целое число</param>
        [DST, NotNull]
        public static implicit operator Polynom(int a) => new Polynom(a);

        /// <summary>Оператор приведения типа целого числа к типу полинома</summary>
        /// <param name="a">Целое число</param>
        [DST, NotNull]
        public static implicit operator Polynom(short a) => new Polynom(a);
    }
}