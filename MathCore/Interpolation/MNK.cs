using System;
using MathCore.Annotations;
using static MathCore.Matrix.Array;
using static MathCore.Matrix.Array.Operator;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace MathCore
{
    /// <summary>Метод наименьших квадратов</summary>
    public class MNK
    {
        /// <summary>Коэффициенты аппроксимирующего полинома</summary>
        private double[] _A;
        /// <summary>Максимальный показатель степени многочлена аппроксимации</summary>
        private readonly int _M;
        /// <summary>Значение абсцисс точек данных</summary>
        private readonly double[] _XData;
        /// <summary>Значение ординат точек данных</summary>
        private readonly double[] _YData;

        /// <summary>Аппроксиматор методом наименьших квадратов</summary>
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
        public double Approximate(double x) => Polynom.Array.GetValue(x, _A); //F(_A, x);

        [NotNull]
        public Func<double, double> GetApproximation() => x => Polynom.Array.GetValue(x, _A);

        private static double[,] CreateMatrix(double[] XData, int MaxPower)
        {
            var N = XData.Length;
            var result = new double[N, MaxPower];
            for (var i = 0; i < N; i++)
            {
                result[i, 0] = 1;
                var x = XData[i];
                var v = x;
                for (var power = 1; power < MaxPower; power++)
                {
                    result[i, power] = v;
                    v *= x;
                }
            }

            return result;
        }

        private void Initialize()
        {
            // A = (M^T * M)^-1 * M^T * Y
            var matrix = CreateMatrix(_XData, _M);
            var transpose = Transpose(matrix);
            _A = MultiplyCol(Multiply(Inverse(Multiply(transpose, matrix)), transpose), _YData);

            //// Создаём матрицу из степеней области определения
            //var matrix = new Matrix(_XData.Length, _M, (i, j) => Math.Pow(_XData[i], j));
            //var transpose = matrix.GetTranspose(); // Транспонируем матрицу
            //var matrix3 = (transpose * matrix).GetInverse(out _) * transpose * _YData;
            //_A = new double[matrix3.N];
            //for (var i = 0; i < _A.Length; i++)
            //    _A[i] = matrix3[i, 0];
        }

        [NotNull] public static implicit operator Func<double, double>([NotNull] MNK mnk) => mnk.GetApproximation();
    }
}