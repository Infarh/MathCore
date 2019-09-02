using System;
using MathCore.Annotations;

// ReSharper disable InconsistentNaming
namespace MathCore
{
    public partial class Matrix
    {
        [NotNull]
        public static implicit operator MatrixComplex([NotNull] Matrix matrix)
        {
            var data = matrix._Data;
            var (n, m) = data;
            var complex_data = new Complex[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) complex_data[i, j] = data[i, j];
            return new MatrixComplex(complex_data);
        }

        [NotNull]
        public static explicit operator Matrix([NotNull] MatrixComplex matrix)
        {
            var complex_data = matrix.GetData();
            var (n, m) = complex_data;
            var data = new double[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) data[i, j] = complex_data[i, j].Abs;
            return new Matrix(data);
        }

        /// <summary>Оператор явного приведения вещественной матрицы к целочисленной</summary>
        /// <param name="matrix">Вещественная матрица</param>
        [NotNull]
        public static explicit operator MatrixInt([NotNull] Matrix matrix)
        {
            var data = matrix._Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)data[i, j];
            return new MatrixInt(int_data);
        }

        [NotNull]
        public MatrixInt Ceiling()
        {
            var data = _Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Ceiling(data[i, j]);
            return new MatrixInt(int_data);
        } 

        [NotNull]
        public MatrixInt Floor()
        {
            var data = _Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Floor(data[i, j]);
            return new MatrixInt(int_data);
        }

        [NotNull]
        public MatrixInt Round()
        {
            var data = _Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j]);
            return new MatrixInt(int_data);
        }

        [NotNull]
        public MatrixInt Round(int Digits)
        {
            var data = _Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits);
            return new MatrixInt(int_data);
        }

        [NotNull]
        public MatrixInt Round(int Digits, MidpointRounding Rounding)
        {
            var data = _Data;
            var (n, m) = data;
            var int_data = new int[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits, Rounding);
            return new MatrixInt(int_data);
        }

        [NotNull]
        public static implicit operator Matrix([NotNull] MatrixInt matrix)
        {
            var int_data = matrix.GetData();
            var (n, m) = int_data;
            var data = new double[n, m];
            for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) data[i, j] = int_data[i, j];
            return new Matrix(data);
        }
    }
}