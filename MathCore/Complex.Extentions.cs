using System;
using MathCore.Annotations;

namespace MathCore
{
    /// <summary>Методы-расширения комплексных чисел</summary>
    public static class ComplexExtensions
    {
        /* - Массивы ---------------------------------------------------------------------------------- */

        #region Массивы

        /// <summary>Преобразование массива комплексных чисел в масив действительных</summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Массив действительных чисел</returns>
        [CanBeNull]
        public static double[] ToRe([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length];

            for(var i = 0; i < c.Length; i++) result[i] = c[i].Re;

            return result;
        }

        /// <summary>Массив комплексных чисел в массив значений мнимых чисел</summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Массив значений комплексных мнимых чисел</returns>
        [CanBeNull]
        public static double[] ToIm([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length];

            for(var i = 0; i < c.Length; i++) result[i] = c[i].Im;

            return result;
        }

        /// <summary>Массив комплексных чисел в массив модулей</summary>
        /// <param name="c">Массив сомплексных чисел</param>
        /// <returns>Массив модулей комплексных чисел</returns>
        [CanBeNull]
        public static double[] ToAbs([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length];

            for(var i = 0; i < c.Length; i++) result[i] = c[i].Abs;

            return result;
        }

        /// <summary>Массив сомплексных чисел в массив аргументов</summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Массив аргументов комплексных чисел</returns>
        [CanBeNull]
        public static double[] ToArg([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length];

            for(var i = 0; i < c.Length; i++) result[i] = c[i].Arg;

            return result;
        }

        [CanBeNull]
        public static double[] ToArgDeg([CanBeNull] this Complex[] c)
        {
            if (c is null) return null;

            var result = new double[c.Length];

            for (var i = 0; i < c.Length; i++) result[i] = c[i].Arg * Consts.ToDeg;

            return result;
        }

        /// <summary>
        /// Массив комплексных чисел в двумерный массив действительных и мнимых частей, где
        /// Re = V[i,0]
        /// Im = V[i,1]
        /// </summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Двумерный массив вещественных и мнимых частей</returns>
        [CanBeNull]
        public static double[,] ToReImArray([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length, 2];
            for(var i = 0; i < c.Length; i++)
            {
                result[i, 0] = c[i].Re;
                result[i, 1] = c[i].Im;
            }
            return result;
        }

        /// <summary>
        /// Массив комплексных чисел в двумерный массив модулей и аргументов частей, где
        /// Abs = V[i,0]
        /// Arg = V[i,1]
        /// </summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Двумерный массив Модулей и аргументов</returns>
        [CanBeNull]
        public static double[,] ToAbsArgArray([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new double[c.Length, 2];
            for(var i = 0; i < c.Length; i++)
            {
                result[i, 0] = c[i].Abs;
                result[i, 1] = c[i].Arg;
            }
            return result;
        }

        /// <summary>Преобразовать массив действительных в массив комплексных чисел</summary>
        /// <param name="Re">Массив действительных чисел</param>
        /// <returns>Массив комплексных чисел</returns>
        [CanBeNull]
        public static Complex[] ToComplex([CanBeNull] this double[] Re)
        {
            if(Re is null) return null;

            var result = new Complex[Re.Length];

            for(var i = 0; i < Re.Length; i++)
                result[i] = Re[i];

            return result;
        }

        /// <summary>Преобразовать двумерный массив действительных в массив комплексных чисел</summary>
        /// <param name="Values">Двумерный массив действительных чисел, где Re = V[i,0], Im = V[i,1]</param>
        /// <returns>Массив комплексных чисел</returns>
        [CanBeNull]
        public static Complex[] ToComplex([CanBeNull] this double[,] Values)
        {
            if(Values is null) return null;

            if(Values.GetLength(1) != 2)
                throw new ArgumentException("Операция возможна для массива с размерностью [N,2]");
            var result = new Complex[Values.GetLength(0)];
            for(var i = 0; i < Values.GetLength(0); i++)
                result[i] = new Complex(Values[i, 0], Values[i, 1]);
            return result;
        }

        /// <summary>Преобразование в массив модулей</summary>
        /// <param name="c">Массив комплексных чисел</param>
        /// <returns>Массив модулей комплексных чисел</returns>
        [CanBeNull]
        public static Complex[] GetAbs([CanBeNull] this Complex[] c)
        {
            if(c is null) return null;

            var result = new Complex[c.Length];
            for(var i = 0; i < c.Length; i++)
                result[i] = c[i].Abs;
            return result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */
    }
}