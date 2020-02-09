using System;
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
        public static void Deconstruct([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Re, [NotNull] out double[] Im)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Re = new double[ComplexArray.Length];
            Im = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++) (Re[i], Im[i]) = ComplexArray[i];
        }

        /// <summary>Метод расчёта массивов модулей и аргументов массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Abs">Массив модулей комплексных чисел исходного массива</param>
        /// <param name="Arg">Массив аргументов комплексных чисел исходного массива</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static void ToAbsArg([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Length];
            Arg = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg;
            }
        }

        /// <summary>Метод расчёта массивов модулей и аргументов (в градусах) массива комплексных чисел</summary>
        /// <param name="ComplexArray">Массив комплексных чисел</param>
        /// <param name="Abs">Массив модулей комплексных чисел исходного массива</param>
        /// <param name="Arg">Массив аргументов комплексных чисел исходного массива в градусах</param>
        /// <exception cref="ArgumentNullException">Если ссылка на исходный массив комплексных чисел пуста</exception>
        public static void ToAbsArgDeg([NotNull] this Complex[] ComplexArray, [NotNull] out double[] Abs, [NotNull] out double[] Arg)
        {
            if (ComplexArray is null) throw new ArgumentNullException(nameof(ComplexArray));

            Abs = new double[ComplexArray.Length];
            Arg = new double[ComplexArray.Length];
            for (var i = 0; i < ComplexArray.Length; i++)
            {
                Abs[i] = ComplexArray[i].Abs;
                Arg[i] = ComplexArray[i].Arg * Consts.ToDeg;
            }
        }
    }
}