namespace MathCore;

public class MatrixN
{
    public static MatrixN CreateFromArray(Array array)
    {
        if (array is null) throw new ArgumentNullException(nameof(array));

        var dimensions   = new int[array.Rank];
        var values_count = 1;
        for (var i = 0; i < dimensions.Length; i++)
        {
            dimensions[i] =  array.GetLength(i);
            values_count  *= dimensions[i];
        }

        var values = new double[values_count];

        var index = new int[dimensions.Length];

        for (var i = 0; i < values_count; i++)
        {
            var value = Convert.ToDouble(array.GetValue(index));
            values[i] = value;

            for (var n = index.Length - 1; n >= 0; n--)
            {
                index[n]++;
                if (index[n] < dimensions[n]) break;
                index[n] = 0;
            }
        }

        return new MatrixN(dimensions, values);
    }

    private readonly int[] _Dimensions;
    private readonly double[] _Values;

    public int DimensionsCount => _Dimensions.Length;

    public IReadOnlyList<int> Dimensions => _Dimensions;

    public ref double this[params int[] Index]
    {
        get
        {
            var indexes_count = Index.Length;
            if (indexes_count != _Dimensions.Length)
                throw new ArgumentException($"Число измерений индекса {indexes_count} не соответствует числу измерений массива {_Dimensions.Length}");

            var index = Index[0];
            if (index >= _Dimensions[0])
                throw new ArgumentOutOfRangeException($"Индекс[0] превышает размерность массивах[0] {_Dimensions[0]}");

            for (var i = 1; i < indexes_count; i++)
            {
                if (Index[i] >= _Dimensions[i])
                    throw new ArgumentOutOfRangeException($"Индекс[{i}] превышает размерность массивах[{i}] {_Dimensions[i]}");

                index *= _Dimensions[i - 1];
                index += Index[i];
            }
            return ref _Values[index];
        }
    }

    private MatrixN(int[] Dimensions, double[] Values)
    {
        _Dimensions = Dimensions;
        _Values     = Values;
    }

    public MatrixN(params int[] Dimensions)
    {
        if (Dimensions is null) throw new ArgumentNullException(nameof(Dimensions));
        if (Dimensions.Length == 0) throw new ArgumentException("Число измерений не может быть равно 0");

        _Dimensions = Dimensions;
        var values_count = Dimensions[0];
        for (var i = 1; i < Dimensions.Length; i++)
        {
            if (Dimensions[i] <= 0)
                throw new ArgumentOutOfRangeException($"Размерность [{i}]:{Dimensions[i]} меньше, либо равно 0");
            values_count *= Dimensions[i];
        }
        _Values = new double[values_count];
    }

    public override string ToString() => $"Matrix[{string.Join("x", _Dimensions)}]";

    private static void ThrowIfDimensionsNotEquals(MatrixN A, MatrixN B)
    {
        var a_dimensions = A._Dimensions;
        var b_dimensions = B._Dimensions;
        if (a_dimensions.Length != b_dimensions.Length)
            throw new ArgumentException($"Число размерностей A({a_dimensions.Length}) не равно числу размерностей B({b_dimensions.Length})");
        for (var i = 0; i < a_dimensions.Length; i++)
            if (a_dimensions[i] != b_dimensions[i])
                throw new ArgumentException($"Размерность A{{{i}}}:{a_dimensions[i]} не равна размерности B{{{i}}}:{b_dimensions[i]}");
    }

    public static MatrixN operator +(MatrixN A, MatrixN B)
    {
        ThrowIfDimensionsNotEquals(A, B);
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] + B._Values[i];
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator -(MatrixN A, MatrixN B)
    {
        ThrowIfDimensionsNotEquals(A, B);
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] - B._Values[i];
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator +(MatrixN A, double B)
    {
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] + B;
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator -(MatrixN A, double B)
    {
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] - B;
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator *(MatrixN A, double B)
    {
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] * B;
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator /(MatrixN A, double B)
    {
        var new_values = new double[A._Values.Length];
        for (int i = 0, count = A._Values.Length; i < count; i++)
            new_values[i] = A._Values[i] / B;
        return new MatrixN((int[])A._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator +(double A, MatrixN B)
    {
        var new_values = new double[B._Values.Length];
        for (int i = 0, count = B._Values.Length; i < count; i++)
            new_values[i] = A + B._Values[i];
        return new MatrixN((int[])B._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator -(double A, MatrixN B)
    {
        var new_values = new double[B._Values.Length];
        for (int i = 0, count = B._Values.Length; i < count; i++)
            new_values[i] = A + B._Values[i];
        return new MatrixN((int[])B._Dimensions.Clone(), new_values);
    }

    public static MatrixN operator *(double A, MatrixN B)
    {
        var new_values = new double[B._Values.Length];
        for (int i = 0, count = B._Values.Length; i < count; i++)
            new_values[i] = A + B._Values[i];
        return new MatrixN((int[])B._Dimensions.Clone(), new_values);
    }
}