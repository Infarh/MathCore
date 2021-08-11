#nullable enable
using System;
using System.Linq;

using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore
{
    /// <summary>Методы-расширения комплексных чисел</summary>
    public static class ComplexExtensions
    {
        /* - Массивы ---------------------------------------------------------------------------------- */

        #region Массивы

        public static Complex[]? Add(this Complex[]? ZZ, double x)
        {
            if (ZZ is null) return null;
            var result = new Complex[ZZ.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = ZZ[i] + x;
            return result;
        }

        public static Complex[]? Substrat(this Complex[]? ZZ, double x)
        {
            if (ZZ is null) return null;
            var result = new Complex[ZZ.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = ZZ[i] - x;
            return result;
        }

        public static Complex[]? Multiply(this Complex[]? ZZ, double x)
        {
            if (ZZ is null) return null;
            var result = new Complex[ZZ.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = ZZ[i] * x;
            return result;
        }

        public static Complex[]? Divide(this Complex[]? ZZ, double x)
        {
            if (ZZ is null) return null;
            var result = new Complex[ZZ.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = ZZ[i] / x;
            return result;
        }

        /// <summary>Преобразование массива комплексных чисел в массив действительных</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив действительных чисел</returns>
        public static double[]? ToRe(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length];

            for(var i = 0; i < ZZ.Length; i++)
                result[i] = ZZ[i].Re;

            return result;
        }

        /// <summary>Массив комплексных чисел в массив значений мнимых чисел</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив значений комплексных мнимых чисел</returns>
        public static double[]? ToIm(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length];

            for(var i = 0; i < ZZ.Length; i++) 
                result[i] = ZZ[i].Im;

            return result;
        }

        /// <summary>Массив комплексных чисел в массив модулей</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив модулей комплексных чисел</returns>
        public static double[]? ToAbs(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length];

            for(var i = 0; i < ZZ.Length; i++) 
                result[i] = ZZ[i].Abs;

            return result;
        }

        /// <summary>Массив комплексных чисел в массив аргументов</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив аргументов комплексных чисел</returns>
        public static double[]? ToArg(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length];

            for(var i = 0; i < ZZ.Length; i++) 
                result[i] = ZZ[i].Arg;

            return result;
        }

        /// <summary>Преобразование массива комплексных чисел в массив значений аргумента каждого из них в градусах</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив аргументов в градусах</returns>
        public static double[]? ToArgDeg(this Complex[]? ZZ)
        {
            if (ZZ is null) return null;

            var result = new double[ZZ.Length];

            for (var i = 0; i < ZZ.Length; i++) 
                result[i] = ZZ[i].Arg * Consts.ToDeg;

            return result;
        }

        /// <summary>
        /// Массив комплексных чисел в двумерный массив действительных и мнимых частей, где
        /// Re = V[i,0] Im = V[i,1]
        /// </summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Двумерный массив вещественных и мнимых частей</returns>
        public static double[,]? ToReImArray(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length, 2];

            for(var i = 0; i < ZZ.Length; i++)
            {
                result[i, 0] = ZZ[i].Re;
                result[i, 1] = ZZ[i].Im;
            }

            return result;
        }

        /// <summary>Массив комплексных чисел в массив кортежей действительных и мнимых частей</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив кортежей вещественных и мнимых частей</returns>
        public static (double Re, double Im)[]? ToReImTuple(this Complex[]? ZZ)
        {
            if (ZZ is null) return null;

            var result = new (double Re, double Im)[ZZ.Length];

            for (var i = 0; i < ZZ.Length; i++) 
                result[i] = ZZ[i];

            return result;
        }

        /// <summary>
        /// Массив комплексных чисел в двумерный массив модулей и аргументов частей, где
        /// Abs = V[i,0] Arg = V[i,1]
        /// </summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Двумерный массив модулей и аргументов</returns>
        public static double[,]? ToAbsArgArray(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new double[ZZ.Length, 2];

            for(var i = 0; i < ZZ.Length; i++)
            {
                result[i, 0] = ZZ[i].Abs;
                result[i, 1] = ZZ[i].Arg;
            }
            return result;
        }

        /// <summary>Массив комплексных чисел в массив кортежей модулей и аргументов</summary>
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив кортежей модулей и аргументов</returns>
        public static (double Abs, double Arg)[]? ToAbsArgTuple(this Complex[]? ZZ)
        {
            if (ZZ is null) return null;

            var result = new (double Abs, double Arg)[ZZ.Length];

            for (var i = 0; i < ZZ.Length; i++) 
                result[i] = (ZZ[i].Abs, ZZ[i].Arg);

            return result;
        }

        /// <summary>Преобразовать массив действительных в массив комплексных чисел</summary>
        /// <param name="Re">Массив действительных чисел</param>
        /// <returns>Массив комплексных чисел</returns>
        public static Complex[]? ToComplex(this double[]? Re)
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
        public static Complex[]? ToComplex(this double[,]? Values)
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
        /// <param name="ZZ">Массив комплексных чисел</param>
        /// <returns>Массив модулей комплексных чисел</returns>
        public static Complex[]? GetAbs(this Complex[]? ZZ)
        {
            if(ZZ is null) return null;

            var result = new Complex[ZZ.Length];

            for(var i = 0; i < ZZ.Length; i++)
                result[i] = ZZ[i].Abs;

            return result;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */
    }
}