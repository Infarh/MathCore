
using System;
// ReSharper disable InconsistentNaming

namespace MathCore
{
    public partial class Matrix
    {
        public static implicit operator MatrixComplex(Matrix matrix)
        {
            var data = matrix._Data;
            Array.GetLength(data, out var N, out var M);
            var complex_data = new Complex[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) complex_data[i, j] = data[i, j];
            return new MatrixComplex(complex_data);
        }

        public static explicit operator Matrix(MatrixComplex matrix)
        {
            var complex_data = matrix.GetData();
            MatrixComplex.Array.GetLength(complex_data, out var N, out var M);
            var data = new double[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) data[i, j] = complex_data[i, j].Abs;
            return new Matrix(data);
        }

        public static explicit operator MatrixInt(Matrix matrix)
        {
            var data = matrix._Data;
            Array.GetLength(data, out var N, out var M);
            var int_data = new int[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) int_data[i, j] = (int)data[i, j];
            return new MatrixInt(int_data);
        }

        public MatrixInt Round()
        {
            var data = _Data;
            Array.GetLength(data, out var lv_N, out var lv_M);
            var int_data = new int[lv_N, lv_M];
            for (var i = 0; i < lv_N; i++) for (var j = 0; j < lv_M; j++) int_data[i, j] = (int)Math.Round(data[i, j]);
            return new MatrixInt(int_data);
        }

        public MatrixInt Round(int Digits)
        {
            var data = _Data;
            Array.GetLength(data, out var lv_N, out var lv_M);
            var int_data = new int[lv_N, lv_M];
            for (var i = 0; i < lv_N; i++) for (var j = 0; j < lv_M; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits);
            return new MatrixInt(int_data);
        }

        public MatrixInt Round(int Digits, MidpointRounding Rounding)
        {
            var data = _Data;
            Array.GetLength(data, out var lv_N, out var lv_M);
            var int_data = new int[lv_N, lv_M];
            for (var i = 0; i < lv_N; i++) for (var j = 0; j < lv_M; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits, Rounding);
            return new MatrixInt(int_data);
        }

        public static implicit operator Matrix(MatrixInt matrix)
        {
            var int_data = matrix.GetData();
            var N = int_data.GetLength(0);
            var M = int_data.GetLength(1);
            var data = new double[N, M];
            for (var i = 0; i < N; i++) for (var j = 0; j < M; j++) data[i, j] = int_data[i, j];
            return new Matrix(data);
        }
    }
}
