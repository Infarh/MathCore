#nullable enable
namespace MathCore;

/// <summary>Матрица на линейном массиве</summary>
public class MatrixArray
{
    /// <summary>Создать квадратную нулевую матрицу</summary>
    /// <param name="N">Размер матрицы</param>
    /// <returns>Квадратная матрица, составленная из нулей</returns>
    public static MatrixArray CreateZeros(int N) => CreateZeros(N, N);

    /// <summary>Создать нулевую матрицу</summary>
    /// <param name="N">Число строк матрицы</param>
    /// <param name="M">Число столбцов матрицы</param>
    /// <returns>Прямоугольная матрица</returns>
    public static MatrixArray CreateZeros(int N, int M) => new(N, M);

    /// <summary>Создать единичную (квадратную матрицу)</summary>
    /// <param name="N">Размер матрицы</param>
    /// <returns>Единичная матрица</returns>
    public static MatrixArray CreateUnitary(int N)
    {
        var result = new double[N * N];
        for (var i = 0; i < N; i++)
            result[i * N + i] = 1;
        return new(N, N, result);
    }

    /// <summary>Создать квадратную матрицу, заполненную единицами</summary>
    /// <param name="N">Размер матрицы</param>
    /// <returns>Квадратная матрица, заполненная единицами</returns>
    public static MatrixArray CreateOnes(int N) => CreateOnes(N, N);

    /// <summary>Создать матрицу, заполенную единицами</summary>
    /// <param name="N">Число строк матрицы</param>
    /// <param name="M">Число столбцов матрицы</param>
    /// <returns>Прямоугольная матрица, заполненная единицами</returns>
    public static MatrixArray CreateOnes(int N, int M)
    {
        var result = new MatrixArray(N, M);
        var data   = result._Values;
        for (int i = 0, count = data.Length; i < count; i++)
            data[i] = 1;
        return result;
    }

    /// <summary>Число строк матрицы</summary>
    private readonly int _N;

    /// <summary>Число столбцов матрицы</summary>
    private readonly int _M;

    /// <summary>Массив значений матрицы</summary>
    protected readonly double[] _Values;

    /// <summary>Число строк матрицы</summary>
    public int N => _N;

    /// <summary>Число столбцов матрицы</summary>
    public int M => _M;

    /// <summary>Элемент матрицы</summary>
    /// <param name="i">Номер строки</param>
    /// <param name="j">Номер столбца</param>
    /// <returns></returns>
    public ref double this[int i, int j] => ref _Values[i * _M + j];

    /// <summary>Матрица на основе одномерного массива</summary>
    /// <param name="N">Число строк</param>
    /// <param name="M">Число столбцов</param>
    public MatrixArray(int N, int M) : this(N, M, new double[N * M]) { }

    /// <summary>Матрица</summary>
    /// <param name="N">Число строк</param>
    /// <param name="M">Число столбцов</param>
    /// <param name="Values">Массив значений ячеек матрицы</param>
    /// <param name="Clone">Слонировать массив?</param>
    public MatrixArray(int N, int M, double[] Values, bool Clone = false)
    {
        if (Values is null) throw new ArgumentNullException(nameof(Values));
        if (N <= 0) throw new ArgumentOutOfRangeException(nameof(N), N, "Число строк должно быть больше 0");
        if (M <= 0) throw new ArgumentOutOfRangeException(nameof(M), M, "Число столбцов должно быть больше 0");
        if (Values.Length != N * M) throw new ArgumentException($"Размер массива значений {Values.Length} не равен заявленной размерности N:{N} * M:{M} = {N * M} элементов");

        _N      = N;
        _M      = M;
        _Values = Clone ? (double[])Values.Clone() : Values;
    }

    /// <summary>Создать транспонированную матрицу</summary>
    /// <returns>Транспонированная матрица</returns>
    public MatrixArray GetTransposed()
    {
        var values = new double[_Values.Length];
        for(var i = 0; i < _N; i++)
            for (var j = 0; j < _M; j++)
                values[j * _N + i] = _Values[i * _M + j];
        return new(_M, _N, values);
    }

    public static MatrixArray operator +(MatrixArray A, MatrixArray B)
    {
        if (A is null) throw new ArgumentNullException(nameof(A));
        if (B is null) throw new ArgumentNullException(nameof(B));
        if (A.N != B.N) throw new InvalidOperationException("Нельзя выполнить сложение матриц с разным числом строк");
        if (A.M != B.M) throw new InvalidOperationException("Нельзя выполнить сложение матриц с разным числом столбцов");

        var a      = A._Values;
        var b      = B._Values;
        var count  = a.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = a[i] + b[i];

        return new MatrixArray(A.N, A.M, result);
    }

    public static MatrixArray operator -(MatrixArray A, MatrixArray B)
    {
        if (A is null) throw new ArgumentNullException(nameof(A));
        if (B is null) throw new ArgumentNullException(nameof(B));
        if (A.N != B.N) throw new InvalidOperationException("Нельзя выполнить сложение матриц с разным числом строк");
        if (A.M != B.M) throw new InvalidOperationException("Нельзя выполнить сложение матриц с разным числом столбцов");

        var a      = A._Values;
        var b      = B._Values;
        var count  = a.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = a[i] - b[i];

        return new MatrixArray(A.N, A.M, result);
    }

    public static MatrixArray operator +(MatrixArray A, double B)
    {
        if (A is null) throw new ArgumentNullException(nameof(A));

        var a      = A._Values;
        var count  = a.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = a[i] + B;

        return new MatrixArray(A.N, A.M, result);
    }

    public static MatrixArray operator -(MatrixArray A, double B)
    {
        if (A is null) throw new ArgumentNullException(nameof(A));

        var a      = A._Values;
        var count  = a.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = a[i] - B;

        return new MatrixArray(A.N, A.M, result);
    }

    public static MatrixArray operator +(double A, MatrixArray B)
    {
        if (B is null) throw new ArgumentNullException(nameof(B));

        var b      = B._Values;
        var count  = b.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = A + b[i];

        return new MatrixArray(B.N, B.M, result);
    }

    public static MatrixArray operator -(double A, MatrixArray B)
    {
        if (B is null) throw new ArgumentNullException(nameof(B));

        var b      = B._Values;
        var count  = b.Length;
        var result = new double[count];

        for (var i = 0; i < count; i++)
            result[i] = A - b[i];

        return new MatrixArray(B.N, B.M, result);
    }

    /// <summary>Преобразовать матрицу на линейном массиве в матрицу на двумерном массиве</summary>
    /// <returns>Матрица на двумерном массиве</returns>
    public Matrix ToMatrix() => Matrix.FomMatrixArray(this);

    public static implicit operator Matrix(MatrixArray matrix) => Matrix.FomMatrixArray(matrix);
}