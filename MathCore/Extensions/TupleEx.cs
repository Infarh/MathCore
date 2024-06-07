namespace MathCore.Extensions;

public static class TupleEx
{
    public static (T, T) MinMax<T>(this (T, T) value) where T : IComparable<T> => 
        Comparer<T>.Default.Compare(value.Item1, value.Item2) >= 0 
            ? value 
            : (value.Item2, value.Item1);

    public static (int, int) MinMax(this (int, int) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (uint, uint) MinMax(this (uint, uint) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (short, short) MinMax(this (short, short) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (ushort, ushort) MinMax(this (ushort, ushort) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (long, long) MinMax(this (long, long) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (ulong, ulong) MinMax(this (ulong, ulong) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (byte, byte) MinMax(this (byte, byte) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (sbyte, sbyte) MinMax(this (sbyte, sbyte) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (double, double) MinMax(this (double, double) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);

    public static (float, float) MinMax(this (float, float) value) =>
        value.Item1 >= value.Item2
            ? value 
            : (value.Item2, value.Item1);
}
