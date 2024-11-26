double[,] A = {
    { 2, 1 },
    { 1, 3 },
    { 4, 2 },
    { 3, 4 }
};

double[] b = [5, 6, 10, 12];

double[] x = Ex.SolveLeastSquares(A, b);

Console.WriteLine("Решение:");
foreach (var xi in x)
{
    Console.WriteLine(xi);
}

Console.WriteLine("End.");
return;

static class Ex
{
    public static double[] SolveLeastSquares(double[,] A, double[] b)
    {
        var m = A.GetLength(0);
        var n = A.GetLength(1);

        var At = Transpose(A);
        var AtA = Multiply(At, A);
        var Atb = Multiply(At, b);

        var x = Solve(AtA, Atb);

        return x;
    }

    private static double[,] Transpose(double[,] matrix)
    {
        var m = matrix.GetLength(0);
        var n = matrix.GetLength(1);
        var transposed = new double[n, m];

        for (var i = 0; i < m; i++)
            for (var j = 0; j < n; j++)
                transposed[j, i] = matrix[i, j];

        return transposed;
    }

    private static double[,] Multiply(double[,] A, double[,] B)
    {
        var m = A.GetLength(0);
        var n = A.GetLength(1);
        var p = B.GetLength(1);
        var result = new double[m, p];

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < p; j++)
            {
                result[i, j] = 0;
                for (var k = 0; k < n; k++)
                    result[i, j] += A[i, k] * B[k, j];
            }
        }

        return result;
    }

    private static double[] Multiply(double[,] A, double[] b)
    {
        var m = A.GetLength(0);
        var n = A.GetLength(1);
        var result = new double[n];

        for (var i = 0; i < n; i++)
        {
            result[i] = 0;
            for (var j = 0; j < m; j++)
                result[i] += A[j, i] * b[j];
        }

        return result;
    }

    private static double[] Solve(double[,] A, double[] b)
    {
        var n = A.GetLength(0);
        var x = new double[n];
        var L = new double[n, n];
        var U = new double[n, n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                double sum = 0;
                for (var k = 0; k < j; k++)
                    sum += L[i, k] * U[k, j];
                L[i, j] = A[i, j] - sum;
            }

            for (var j = i; j < n; j++)
            {
                double sum = 0;
                for (var k = 0; k < i; k++)
                    sum += L[i, k] * U[k, j];
                U[i, j] = (A[i, j] - sum) / L[i, i];
            }
        }

        var y = new double[n];
        for (var i = 0; i < n; i++)
        {
            double sum = 0;
            for (var j = 0; j < i; j++)
                sum += L[i, j] * y[j];
            y[i] = (b[i] - sum) / L[i, i];
        }

        for (var i = n - 1; i >= 0; i--)
        {
            double sum = 0;
            for (var j = i + 1; j < n; j++)
                sum += U[i, j] * x[j];
            x[i] = y[i] - sum;
        }

        return x;
    }
}