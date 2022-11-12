#nullable enable

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore;

public partial class Matrix
{
    public partial class Array
    {
        public static double[,] CrateZTransformMatrixArray(int Order)
        {
            if (Order < 1) throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок должен быть больше 0");

            var result = new double[Order, Order];
            result[0, 0] = 1;

            for (var j = 1; j < Order; j++)
            {
                result[0, j] = 1;

                for (var i = 1; i < Order; i++)
                    result[i, j] = result[i, j - 1] + result[i - 1, j - 1];

                for (var k = 0; k < j; k++)
                {
                    var last = result[0, k];
                    for (var i = 1; i < Order; i++)
                    {
                        var tmp = result[i, k];
                        result[i, k] -= last;
                        last     =  tmp;
                    }
                }
            }

            return result;
        }
    }

    public static Matrix GetZTransformMatrix(int Order)
    {
        if (Order < 1) throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок должен быть больше 0");

        var matrix = Array.CrateZTransformMatrixArray(Order);
        return new(matrix);
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="w"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out double[] w, out Matrix V)
    {
        Array.SVD(_Data, out var u, out w, out var v);
        U = new Matrix(u);
        V = new Matrix(v);
    }

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="S"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix U, out Matrix S, out Matrix V)
    {
        SVD(out U, out double[] w, out V);
        S = CreateDiagonal(w);
    }
}