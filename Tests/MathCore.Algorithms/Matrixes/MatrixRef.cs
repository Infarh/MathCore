using System.Text;

namespace MathCore.Algorithms.Matrixes;

public readonly ref struct MatrixRef
{
    private readonly double[,] _Matrix;

    private readonly bool _Transposed;

    public readonly ref double this[int n, int m]
    {
        get
        {
            if (_Transposed)
                return ref _Matrix[m, n];

            return ref _Matrix[n, m];
        }
    }

    public int N => _Matrix.GetLength(_Transposed ? 1 : 0);

    public int M => _Matrix.GetLength(_Transposed ? 0 : 1);

    public MatrixRef(double[,] Matrix, bool Transposed = false)
    {
        _Matrix = Matrix;
        _Transposed = Transposed;
    }

    public MatrixRef Transpose() => new(_Matrix, !_Transposed);

    public override string ToString()
    {
        var result = new StringBuilder("{");

        var (rows, cols) = (this.N, this.M);
        for (var n = 0; n < rows; n++)
        {
            result.Append("{ ");
            for (var m = 0; m < cols; m++)
                result.Append(this[n, m]).Append(", ");
            result.Length -= 2;
            result.AppendLine(" },");
            result.Append(' ');
        }

        result.Length -= Environment.NewLine.Length + 2;
        result.Append("}");
        return result.ToString();
    }

    public static implicit operator MatrixRef(double[,] M) => new(M);

    public static MatrixRef operator +(MatrixRef A, MatrixRef B)
    {
        var (rows, cols) = (A.N, A.M);
        if (rows != B.N) throw new InvalidOperationException("Число строк не совпадает");
        if (cols != B.M) throw new InvalidOperationException("Число столбцов не совпадает");

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A[n, m] + B[n, m];

        return result;
    }

    public static MatrixRef operator -(MatrixRef A, MatrixRef B)
    {
        var (rows, cols) = (A.N, A.M);
        if (rows != B.N) throw new InvalidOperationException("Число строк не совпадает");
        if (cols != B.M) throw new InvalidOperationException("Число столбцов не совпадает");

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A[n, m] - B[n, m];

        return result;
    }

    public static MatrixRef operator +(MatrixRef A, double B)
    {
        var (rows, cols) = (A.N, A.M);

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A[n, m] + B;

        return result;
    }

    public static MatrixRef operator +(double A, MatrixRef B)
    {
        var (rows, cols) = (B.N, B.M);

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A + B[n, m];

        return result;
    }

    public static MatrixRef operator -(MatrixRef A, double B)
    {
        var (rows, cols) = (A.N, A.M);

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A[n, m] - B;

        return result;
    }

    public static MatrixRef operator -(double A, MatrixRef B)
    {
        var (rows, cols) = (B.N, B.M);

        var result = new double[rows, cols];

        for (var n = 0; n < rows; n++)
            for (var m = 0; m < cols; m++)
                result[n, m] = A - B[n, m];

        return result;
    }

    public static MatrixRef operator *(MatrixRef A, MatrixRef B)
    {
        var (rows_a, cols_a) = (A.N, A.M);
        var (rows_b, cols_b) = (B.N, B.M);



        var result = new double[rows_a, cols_b];

        for (var n = 0; n < rows_a; n++)
            for (var m = 0; m < cols_a; m++)
                result[n, m] = A[n, m] + B[n, m];

        return result;
    }
}
