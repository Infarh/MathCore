using System;
using System.Collections.Generic;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace MathCore
{
    /// <summary>Класс методов-расширений для массивов комплексных чисел</summary>
    public static class ComplexArrayExtensions
    {
        /// <summary>Метод деконструкции значения массива, позволяющий получить массив действительных и мнимых частей</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Re">Массив действительных частей</param>
        /// <param name="Im">Массив мнимых частей</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Deconstruct([NotNull] this IReadOnlyList<Complex> ComplexArray, [NotNull] out double[] Re, [NotNull] out double[] Im)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Re = new double[ComplexArray.Count];
            Im = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++) (Re[i], Im[i]) = ComplexArray[i];
        }

        /// <summary>Метод расчёта массивов действительных и мнимых частей массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Re">Массив действительных частей комплексных чисел исходного массива</param>
        /// <param name="Im">Массив мнимых частей комплексных чисел исходного массива</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static void ToReIm([NotNull] this IReadOnlyList<Complex> ComplexArray, [NotNull] out double[] Re, [NotNull] out double[] Im)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Re = new double[ComplexArray.Count];
            Im = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                Re[i] = ComplexArray[i].Re;
                Im[i] = ComplexArray[i].Im;
            }
        }

        /// <summary>Метод расчёта массивов действительных и мнимых частей массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static (double[] Re, double[] Im) ToReIm([NotNull] this IReadOnlyList<Complex> ComplexArray)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            var re = new double[ComplexArray.Count];
            var im = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                re[i] = ComplexArray[i].Re;
                im[i] = ComplexArray[i].Im;
            }

            return (re, im);
        }

        /// <summary>Метод расчёта массивов модулей и аргументов массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Abs">Массив модулей комплексных чисел исходного массива</param>
        /// <param name="Arg">Массив аргументов комплексных чисел исходного массива</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static void ToAbsArg([NotNull] this IReadOnlyList<Complex> ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Count];
            Arg = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg;
            }
        }

        /// <summary>Метод расчёта массивов модулей и аргументов массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static (double[] Abs, double[] Arg) ToAbsArg([NotNull] this IReadOnlyList<Complex> ComplexArray)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            var abs = new double[ComplexArray.Count];
            var arg = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                abs[i] = ComplexArray[i].Abs;
                arg[i] = ComplexArray[i].Arg;
            }

            return (abs, arg);
        }

        /// <summary>Метод расчёта массивов модулей и аргументов (в градусах) массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Abs">Массив модулей комплексных чисел исходного массива</param>
        /// <param name="Arg">Массив аргументов комплексных чисел исходного массива в градусах</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static void ToAbsArgDeg([NotNull] this IReadOnlyList<Complex> ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Count];
            Arg = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg * Consts.ToDeg;
            }
        }

        /// <summary>Метод расчёта массивов модулей и аргументов (в градусах) массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static (double[] Abs, double[] Arg) ToAbsArgDeg([NotNull] this IReadOnlyList<Complex> ComplexArray)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            var abs = new double[ComplexArray.Count];
            var arg = new double[ComplexArray.Count];
            for (var i = 0; i < ComplexArray.Count; i++)
            {
                abs[i] = ComplexArray[i].Abs;
                arg[i] = ComplexArray[i].Arg * Consts.ToDeg;
            }

            return (abs, arg);
        }
    }
}