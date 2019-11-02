﻿using System;
using System.Collections.Generic;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace MathCore
{
    /// <summary>Метод наименьших квадратов</summary>
    public class MNK
    {
        /// <summary>Коэффициенты апроксимирующего полинома</summary>
        private double[] _A;
        /// <summary>Максимальный показатель степени многочления апроксимации</summary>
        private readonly int _M;
        /// <summary>Значение абсцисс точек данных</summary>
        private readonly double[] _XData;
        /// <summary>Значение ординат точек данных</summary>
        private readonly double[] _YData;

        /// <summary>Апроксиматор методом наименьших квадратов</summary>
        /// <param name="X">Массив аргументов</param>
        /// <param name="Y">Массив значений</param>
        /// <param name="m">Степень полинома интерполяции</param>
        public MNK(double[] X, double[] Y, int m)
        {
            _XData = X;
            _YData = Y;
            _M = m;
            Initialize();
        }

        // ReSharper disable once UnusedMember.Global
        public double Approximate(double x) => Polynom.Array.GetValue(_A, x); //F(_A, x);

        private static double F([NotNull] IReadOnlyList<double> A, double x)
        {
            var d = x;
            var result = A[0];
            for (var i = 1; i < A.Count; i++)
            {
                result += A[i] * d;
                d *= x;
            }
            return result;
        }

        [NotNull]
        public Func<double, double> GetApproximation()
        {
            var a = _A;
            return x => F(a, x);
        }

        private void Initialize()
        {
            // Создаём матрицу из степеней области определения
            var matrix = new Matrix(_XData.Length, _M, (i, j) => Math.Pow(_XData[i], j));
            var transponse = matrix.GetTransponse(); // Транспонируем матрицу
            // Y = (M^T * M)^-1 * M^T * Y
            var matrix3 = (transponse * matrix).GetInverse(out _) * transponse * _YData;
            _A = new double[matrix3.N];
            for (var i = 0; i < _A.Length; i++)
                _A[i] = matrix3[i, 0];
        }

        [NotNull] public static implicit operator Func<double, double>([NotNull] MNK mnk) => mnk.GetApproximation();
    }
}