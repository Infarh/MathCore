namespace MathCore
{
    public partial class Matrix
    {
        /// <summary>Преобразовать в матрицу на линейном массиве</summary>
        /// <returns>Матрица на линейном массиве</returns>
        public MatrixArray ToMatrixArray()
        {
            var (n, m) = (N, M);
            var result = new MatrixArray(n, m);
            for(var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                    result[i, j] = _Data[i, j];
            return result;
        }

        public static implicit operator MatrixArray(Matrix matrix) => matrix.ToMatrixArray();

        /// <summary>Создать на основе матрицы на линейном массиве</summary>
        /// <param name="matrix">Матрица на линейном массиве</param>
        /// <returns>Матрица на двумерном массиве</returns>
        public static Matrix FomMatrixArray(MatrixArray matrix)
        {
            var (n,m) = (matrix.N, matrix.M);
            var result = new double[n, m];
            for(var i = 0; i < n; i++)
                for (var j = 0; j < m; j++)
                    result[i, j] = matrix[i, j];
            return new Matrix(result);
        }
    }
}
