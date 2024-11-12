using MathCore.Annotations;

// ReSharper disable InconsistentNaming
namespace MathCore;

public partial class Matrix
{
    [NotNull]
    public static implicit operator MatrixComplex([NotNull] Matrix matrix)
    {
        var data = matrix._Data;
        var (n, m) = data;
        var complex_data = new Complex[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) complex_data[i, j] = data[i, j];
        return new(complex_data);
    }

    [NotNull]
    public static explicit operator Matrix([NotNull] MatrixComplex matrix)
    {
        var complex_data = matrix.GetData();
        var (n, m) = complex_data;
        var data = new double[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) data[i, j] = complex_data[i, j].Abs;
        return new(data);
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
        return new(int_data);
    }

    /// <summary>Округление элементов матрицы до ближайшего большего целого</summary>
    /// <returns>Матрица с округленными элементами</returns>
    [NotNull]
    public MatrixInt Ceiling()
    {
        var data = _Data;
        var (n, m) = data;
        var int_data = new int[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Ceiling(data[i, j]);
        return new(int_data);
    } 

    /// <summary>Округление элементов матрицы до ближайшего меньшего целого</summary>
    /// <returns>Матрица с округленными элементами</returns>
    [NotNull]
    public MatrixInt Floor()
    {
        var data = _Data;
        var (n, m) = data;
        var int_data = new int[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Floor(data[i, j]);
        return new(int_data);
    }

    /// <summary>Округление элементов матрицы до ближайшего целого</summary>
    /// <returns>Матрица с округленными элементами</returns>
    [NotNull]
    public MatrixInt Round()
    {
        var data = _Data;
        var (n, m) = data;
        var int_data = new int[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j]);
        return new(int_data);
    }

    /// <summary>Округление элементов матрицы до указанного количества знаков после запятой</summary>
    /// <param name="Digits">Количество знаков после запятой</param>
    /// <returns>Матрица с округленными элементами</returns>
    [NotNull]
    public MatrixInt Round(int Digits)
    {
        var data = _Data;
        var (n, m) = data;
        var int_data = new int[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits);
        return new(int_data);
    }

    /// <summary>Округление элементов матрицы до указанного количества знаков после запятой</summary>
    /// <param name="Digits">Количество знаков после запятой</param>
    /// <param name="Rounding">Стиль округления</param>
    /// <returns>Матрица с округленными элементами</returns>
    [NotNull]
    public MatrixInt Round(int Digits, MidpointRounding Rounding)
    {
        var data = _Data;
        var (n, m) = data;
        var int_data = new int[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) int_data[i, j] = (int)Math.Round(data[i, j], Digits, Rounding);
        return new(int_data);
    }

    [NotNull]
    public static implicit operator Matrix([NotNull] MatrixInt matrix)
    {
        var int_data = matrix.GetData();
        var (n, m) = int_data;
        var data = new double[n, m];
        for (var i = 0; i < n; i++) for (var j = 0; j < m; j++) data[i, j] = int_data[i, j];
        return new(data);
    }
}