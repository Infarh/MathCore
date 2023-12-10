using System.Numerics;

namespace Benchmarks.Vectorization;

readonly ref struct DoubleVector(double[] array)
{
    public static DoubleVector Create(params double[] array) => new(array);

    public double[] Items => array;

    public int Length => array.Length;

    public double Sum()
    {
        var sum = 0d;
        if (!Vector.IsHardwareAccelerated)
        {
            foreach (var x in array)
                sum += x;

            return sum;
        }

        var window_size = Vector<double>.Count;
        var ones = Vector<double>.One;

        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            sum += Vector.Dot(new(array, i), ones);

        for (var i = vector_part_length; i < array.Length; i++)
            sum += array[i];

        return sum;
    }

    public DoubleVector Correlation(DoubleVector h)
    {

        return default;
    }

    public DoubleVector Add(double x)
    {
        var result = new double[Length];

        if (!Vector.IsHardwareAccelerated)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] = array[i] + x;
        }

        var window_size = Vector<double>.Count;
        var vector_x = new Vector<double>(x);
        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            Vector.Add(new(array, i), vector_x).CopyTo(result, i);

        for (var i = vector_part_length; i < array.Length; i++)
            result[i] = array[i] + x;
        
        return result;
    }

    public DoubleVector Add(DoubleVector other)
    {
        if (Length != other.Length) throw new InvalidOperationException("Размеры векторов не совпадают");

        var other_array = other.Items;
        var result = new double[Length];

        if (!Vector.IsHardwareAccelerated)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] = array[i] + other_array[i];
        }

        var window_size = Vector<double>.Count;
        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            Vector.Add<double>(new(array, i), new(other, i)).CopyTo(result, i);

        for (var i = vector_part_length; i < array.Length; i++)
            result[i] = array[i] + other_array[i];
        
        return result;
    }

    public DoubleVector Subtract(double x)
    {
        var result = new double[Length];

        if (!Vector.IsHardwareAccelerated)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] = array[i] - x;
        }

        var window_size = Vector<double>.Count;
        var vector_x = new Vector<double>(x);
        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            Vector.Subtract(new(array, i), vector_x).CopyTo(result, i);

        for (var i = vector_part_length; i < array.Length; i++)
            result[i] = array[i] - x;

        return result;
    }

    public DoubleVector SubtractFrom(double x)
    {
        var result = new double[Length];

        if (!Vector.IsHardwareAccelerated)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] = x - array[i];
        }

        var window_size = Vector<double>.Count;
        var vector_x = new Vector<double>(x);
        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            Vector.Subtract(vector_x, new(array, i)).CopyTo(result, i);

        for (var i = vector_part_length; i < array.Length; i++)
            result[i] = x - array[i];

        return result;
    }

    public DoubleVector Subtract(DoubleVector other)
    {
        if (Length != other.Length) throw new InvalidOperationException("Размеры векторов не совпадают");

        var other_array = other.Items;
        var result = new double[Length];

        if (!Vector.IsHardwareAccelerated)
        {
            for (var i = 0; i < result.Length; i++)
                result[i] = array[i] - other_array[i];
        }

        var window_size = Vector<double>.Count;
        var vector_part_length = array.Length - array.Length % window_size;
        for (var i = 0; i < vector_part_length; i += window_size)
            Vector.Subtract<double>(new(array, i), new(other, i)).CopyTo(result, i);

        for (var i = vector_part_length; i < array.Length; i++)
            result[i] = array[i] - other_array[i];
        
        return result;
    }


    public static implicit operator DoubleVector(double[] array) => new(array);

    public static implicit operator double[](DoubleVector vector) => vector.Items;

    public static DoubleVector operator +(DoubleVector a, double b) => a.Add(b);
    public static DoubleVector operator +(double a, DoubleVector b) => b.Add(a);
    public static DoubleVector operator -(DoubleVector a, double b) => a.Subtract(b);

    public static DoubleVector operator +(DoubleVector a, DoubleVector b) => a.Add(b);
    public static DoubleVector operator -(DoubleVector a, DoubleVector b) => a.Subtract(b);
}