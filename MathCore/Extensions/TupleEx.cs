namespace MathCore.Extensions;

public static class TupleEx
{
    public static (T min, T max) MinMax<T>(this (T, T) value) where T : IComparable<T> => 
        Comparer<T>.Default.Compare(value.Item1, value.Item2) <= 0 
            ? value 
            : (value.Item2, value.Item1);

    public static (int min, int max) MinMax(this (int, int) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (uint min, uint max) MinMax(this (uint, uint) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (short min, short max) MinMax(this (short, short) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (ushort min, ushort max) MinMax(this (ushort, ushort) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (long min, long max) MinMax(this (long, long) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (ulong min, ulong max) MinMax(this (ulong, ulong) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (byte min, byte max) MinMax(this (byte, byte) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (sbyte min, sbyte max) MinMax(this (sbyte, sbyte) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (double min, double max) MinMax(this (double, double) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (float min, float max) MinMax(this (float, float) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (decimal min, decimal max) MinMax(this (decimal, decimal) value) =>
        value.Item1 <= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (int min, int len) MinMaxToMinLength(this (int, int) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (uint min, uint len) MinMaxToMinLength(this (uint, uint) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (short min, short len) MinMaxToMinLength(this (short, short) value)
    {
        var (min, max) = value.MinMax();
        return (min, (short)(max - min));
    }

    public static (ushort min, ushort len) MinMaxToMinLength(this (ushort, ushort) value)
    {
        var (min, max) = value.MinMax();
        return (min, (ushort)(max - min));
    }

    public static (byte min, byte len) MinMaxToMinLength(this (byte, byte) value)
    {
        var (min, max) = value.MinMax();
        return (min, (byte)(max - min));
    }

    public static (sbyte min, sbyte len) MinMaxToMinLength(this (sbyte, sbyte) value)
    {
        var (min, max) = value.MinMax();
        return (min, (sbyte)(max - min));
    }

    public static (long min, long len) MinMaxToMinLength(this (long, long) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (ulong min, ulong len) MinMaxToMinLength(this (ulong, ulong) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (double min, double len) MinMaxToMinLength(this (double, double) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (float min, float len) MinMaxToMinLength(this (float, float) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (decimal min, decimal len) MinMaxToMinLength(this (decimal, decimal) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    public static (T min, T max) MaxMin<T>(this (T, T) value) where T : IComparable<T> =>
        Comparer<T>.Default.Compare(value.Item1, value.Item2) >= 0
            ? value
            : (value.Item2, value.Item1);

    public static (int min, int max) MaxMin(this (int, int) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (uint min, uint max) MaxMin(this (uint, uint) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (short min, short max) MaxMin(this (short, short) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (ushort min, ushort max) MaxMin(this (ushort, ushort) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (long min, long max) MaxMin(this (long, long) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (ulong min, ulong max) MaxMin(this (ulong, ulong) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (byte min, byte max) MaxMin(this (byte, byte) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (sbyte min, sbyte max) MaxMin(this (sbyte, sbyte) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (double min, double max) MaxMin(this (double, double) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (float min, float max) MaxMin(this (float, float) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    public static (decimal min, decimal max) MaxMin(this (decimal, decimal) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);
}
