#if NET5_0_OR_GREATER

#nullable enable
namespace MathCore;

public partial class Matrix<T>
{
    public partial class Array
    {
        public static T[,] CrateZTransformMatrixArray(int Order)
        {
            if (Order < 1) throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок должен быть больше 0");

            var result = new T[Order, Order];
            result[0, 0] = T.One;

            for (var j = 1; j < Order; j++)
            {
                result[0, j] = T.One;

                for (var i = 1; i < Order; i++)
                    result[i, j] = result[i, j - 1] + result[i - 1, j - 1];

                for (var k = 0; k < j; k++)
                    for (var (i, last) = (1, result[0, k]); i < Order; i++)
                        (result[i, k], last) = (result[i, k] - last, result[i, k]);
            }

            return result;
        }
    }

    public static Matrix<T> GetZTransformMatrix(int Order)
    {
        if (Order < 1) throw new ArgumentOutOfRangeException(nameof(Order), Order, "Порядок должен быть больше 0");

        var matrix = Array.CrateZTransformMatrixArray(Order);
        return new(matrix);
    }

    /// <summary>Метод прогонки</summary>
    /// <param name="Down">Нижняя диагональ</param>
    /// <param name="Middle">Главная диагональ</param>
    /// <param name="Up">Верхняя диагональ</param>
    /// <param name="RightPart">Правая часть системы уравнений</param>
    /// <param name="Result">Вектор результата</param>
    public static void TridiagonalAlgorithm(T[] Down, T[] Middle, T[] Up, T[] RightPart, ref T[]? Result)
    {
        Result ??= new T[Middle.Length];
        TridiagonalAlgorithm(Down, Middle, Up, RightPart, Result);
    }

    /// <summary>Метод прогонки</summary>
    /// <param name="Down">Нижняя диагональ</param>
    /// <param name="Middle">Главная диагональ</param>
    /// <param name="Up">Верхняя диагональ</param>
    /// <param name="RightPart">Правая часть системы уравнений</param>
    public static T[] TridiagonalAlgorithm(T[] Down, T[] Middle, T[] Up, T[] RightPart)
    {
        T[] result = null!;
        TridiagonalAlgorithm(Down, Middle, Up, RightPart, ref result);
        return result;
    }

    /// <summary>Метод прогонки</summary>
    /// <param name="Down">Нижняя диагональ</param>
    /// <param name="Middle">Главная диагональ</param>
    /// <param name="Up">Верхняя диагональ</param>
    /// <param name="RightPart">Правая часть системы уравнений</param>
#if !NET8_0_OR_GREATER
    public static void TridiagonalAlgorithm(T[] Down, T[] Middle, T[] Up, T[] RightPart, T[] Result)
    {
        if (Down.Length != Middle.Length - 1)
            throw new ArgumentException(
                "Длина элементов вектора нижней диагонали не равна длине элементов главной диагонали - 1",
                nameof(Down));
        if (Down.Length != Up.Length)
            throw new ArgumentException("Размеры векторов нижней и верхней диагонали не совпадают", nameof(Middle));

        if (RightPart.Length != Middle.Length)
            throw new ArgumentException(
                "Размер вектора значений не равно числу строк (размеру вектора элементов главной диагонали) матрицы",
                nameof(RightPart));

        var n = Down.Length;

        RightPart.CopyTo(Result, 0);

        var up = (T[])Up.Clone();

        up[0] /= Middle[0];
        Result[0] = RightPart[0] / Middle[0];

        for (var i = 1; i < n; i++)
        {
            up[i] /= Middle[i] - Down[i - 1] * up[i - 1];
            Result[i] = (Result[i] - Down[i - 1] * Result[i - 1]) / (Middle[i] - Down[i - 1] * up[i - 1]);
        }

        Result[n] = (Result[n] - Down[n - 1] * Result[n - 1]) / (Middle[n] - Down[n - 1] * up[n - 1]);

        while(n-- > 0)
            Result[n] -= up[n] * Result[n + 1];
    }
#else
    public static void TridiagonalAlgorithm(T[] Down, T[] Middle, T[] Up, T[] RightPart, T[] Result)
    {
        if (Down.Length != Middle.Length - 1)
            throw new ArgumentException(
                "Длина элементов вектора нижней диагонали не равна длине элементов главной диагонали - 1",
                nameof(Down));
        if (Down.Length != Up.Length)
            throw new ArgumentException("Размеры векторов нижней и верхней диагонали не совпадают", nameof(Middle));

        if (RightPart.Length != Middle.Length)
            throw new ArgumentException(
                "Размер вектора значений не равно числу строк (размеру вектора элементов главной диагонали) матрицы",
                nameof(RightPart));

        if (Result.NotNull().Length != Middle.Length)
            throw new ArgumentException(
                "Размер вектора результата не равен числу строк (длина вектора элементов главной диагонали) матрицы",
                nameof(Result));

        var n = Down.Length;

        RightPart.AsSpan().CopyTo(Result);
        var pool_array = System.Buffers.ArrayPool<T>.Shared.Rent(n);
        var up = pool_array.AsSpan(0, n);

        try
        {
            Up.AsSpan().CopyTo(up);

            up[0] /= Middle[0];
            Result[0] = RightPart[0] / Middle[0];

            for (var i = 1; i < n; i++)
            {
                up[i] /= Middle[i] - Down[i - 1] * up[i - 1];
                Result[i] = (Result[i] - Down[i - 1] * Result[i - 1]) / (Middle[i] - Down[i - 1] * up[i - 1]);
            }

            Result[n] = (Result[n] - Down[n - 1] * Result[n - 1]) / (Middle[n] - Down[n - 1] * up[n - 1]);

            while (n-- > 0)
                Result[n] -= up[n] * Result[n + 1];
        }
        finally
        {
            System.Buffers.ArrayPool<T>.Shared.Return(pool_array);
        }
    }
#endif

    /// <summary>SVD-разложение матрицы</summary>
    /// <param name="U"></param>
    /// <param name="w"></param>
    /// <param name="V"></param>
    public void SVD(out Matrix<T> U, out T[] w, out Matrix<T> V)
    {
        Array.SVD(_Data, out var u, out w, out var v);
        U = new(u);
        V = new(v);
    }

    /// <summary>Вычисляет сингулярное разложение (SVD) текущей матрицы.</summary>
    /// <param name="U">Левые сингулярные векторы матрицы.</param>
    /// <param name="S">Сингулярные значения матрицы.</param>
    /// <param name="V">Правые сингулярные векторы матрицы.</param>
    public void SVD(out Matrix<T> U, out Matrix<T> S, out Matrix<T> V)
    {
        SVD(out U, out T[] w, out V);
        S = CreateDiagonal(w);
    }
}


#endif