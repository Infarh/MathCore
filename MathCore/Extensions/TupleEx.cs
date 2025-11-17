namespace MathCore.Extensions;

public static class TupleEx
{
    /// <summary>Возвращает кортеж с минимальным и максимальным значением</summary>
    /// <param name="value">Кортеж значений</param>
    /// <typeparam name="T">Тип элементов кортежа</typeparam>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (T min, T max) MinMax<T>(this (T, T) value) where T : IComparable<T> =>
        Comparer<T>.Default.Compare(value.Item1, value.Item2) <= 0
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для целых чисел</summary>
    /// <param name="value">Кортеж целых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (int min, int max) MinMax(this (int, int) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для целых беззнаковых чисел</summary>
    /// <param name="value">Кортеж целых беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (uint min, uint max) MinMax(this (uint, uint) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для коротких целых чисел</summary>
    /// <param name="value">Кортеж коротких целых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (short min, short max) MinMax(this (short, short) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для коротких беззнаковых чисел</summary>
    /// <param name="value">Кортеж коротких беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (ushort min, ushort max) MinMax(this (ushort, ushort) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для длинных целых чисел</summary>
    /// <param name="value">Кортеж длинных целых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (long min, long max) MinMax(this (long, long) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для длинных беззнаковых чисел</summary>
    /// <param name="value">Кортеж длинных беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (ulong min, ulong max) MinMax(this (ulong, ulong) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для байтов</summary>
    /// <param name="value">Кортеж байтов</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (byte min, byte max) MinMax(this (byte, byte) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для знаковых байтов</summary>
    /// <param name="value">Кортеж знаковых байтов</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (sbyte min, sbyte max) MinMax(this (sbyte, sbyte) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для чисел с плавающей запятой двойной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой двойной точности</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (double min, double max) MinMax(this (double, double) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для чисел с плавающей запятой одинарной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой одинарной точности</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (float min, float max) MinMax(this (float, float) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с минимальным и максимальным значением для десятичных чисел</summary>
    /// <param name="value">Кортеж десятичных чисел</param>
    /// <returns>Кортеж с минимальным и максимальным значением</returns>
    public static (decimal min, decimal max) MinMax(this (decimal, decimal) value) =>
        value.Item1 <= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для целых чисел</summary>
    /// <param name="value">Кортеж целых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (int min, int len) MinMaxToMinLength(this (int, int) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для целых беззнаковых чисел</summary>
    /// <param name="value">Кортеж целых беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (uint min, uint len) MinMaxToMinLength(this (uint, uint) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для коротких целых чисел</summary>
    /// <param name="value">Кортеж коротких целых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (short min, short len) MinMaxToMinLength(this (short, short) value)
    {
        var (min, max) = value.MinMax();
        return (min, (short)(max - min));
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для коротких беззнаковых чисел</summary>
    /// <param name="value">Кортеж коротких беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (ushort min, ushort len) MinMaxToMinLength(this (ushort, ushort) value)
    {
        var (min, max) = value.MinMax();
        return (min, (ushort)(max - min));
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для байтов</summary>
    /// <param name="value">Кортеж байтов</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (byte min, byte len) MinMaxToMinLength(this (byte, byte) value)
    {
        var (min, max) = value.MinMax();
        return (min, (byte)(max - min));
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для знаковых байтов</summary>
    /// <param name="value">Кортеж знаковых байтов</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (sbyte min, sbyte len) MinMaxToMinLength(this (sbyte, sbyte) value)
    {
        var (min, max) = value.MinMax();
        return (min, (sbyte)(max - min));
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для длинных целых чисел</summary>
    /// <param name="value">Кортеж длинных целых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (long min, long len) MinMaxToMinLength(this (long, long) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для длинных беззнаковых чисел</summary>
    /// <param name="value">Кортеж длинных беззнаковых чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (ulong min, ulong len) MinMaxToMinLength(this (ulong, ulong) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для чисел с плавающей запятой двойной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой двойной точности</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (double min, double len) MinMaxToMinLength(this (double, double) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для чисел с плавающей запятой одинарной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой одинарной точности</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (float min, float len) MinMaxToMinLength(this (float, float) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Преобразует кортеж в минимальное значение и длину диапазона для десятичных чисел</summary>
    /// <param name="value">Кортеж десятичных чисел</param>
    /// <returns>Кортеж с минимальным значением и длиной диапазона</returns>
    public static (decimal min, decimal len) MinMaxToMinLength(this (decimal, decimal) value)
    {
        var (min, max) = value.MinMax();
        return (min, max - min);
    }

    /// <summary>Возвращает кортеж с максимальным и минимальным значением</summary>
    /// <param name="value">Кортеж значений</param>
    /// <typeparam name="T">Тип элементов кортежа</typeparam>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (T min, T max) MaxMin<T>(this (T, T) value) where T : IComparable<T> =>
        Comparer<T>.Default.Compare(value.Item1, value.Item2) >= 0
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для целых чисел</summary>
    /// <param name="value">Кортеж целых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (int min, int max) MaxMin(this (int, int) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для целых беззнаковых чисел</summary>
    /// <param name="value">Кортеж целых беззнаковых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (uint min, uint max) MaxMin(this (uint, uint) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для коротких целых чисел</summary>
    /// <param name="value">Кортеж коротких целых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (short min, short max) MaxMin(this (short, short) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для коротких беззнаковых чисел</summary>
    /// <param name="value">Кортеж коротких беззнаковых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (ushort min, ushort max) MaxMin(this (ushort, ushort) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для длинных целых чисел</summary>
    /// <param name="value">Кортеж длинных целых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (long min, long max) MaxMin(this (long, long) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для длинных беззнаковых чисел</summary>
    /// <param name="value">Кортеж длинных беззнаковых чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (ulong min, ulong max) MaxMin(this (ulong, ulong) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для байтов</summary>
    /// <param name="value">Кортеж байтов</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (byte min, byte max) MaxMin(this (byte, byte) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для знаковых байтов</summary>
    /// <param name="value">Кортеж знаковых байтов</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (sbyte min, sbyte max) MaxMin(this (sbyte, sbyte) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для чисел с плавающей запятой двойной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой двойной точности</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (double min, double max) MaxMin(this (double, double) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для чисел с плавающей запятой одинарной точности</summary>
    /// <param name="value">Кортеж чисел с плавающей запятой одинарной точности</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (float min, float max) MaxMin(this (float, float) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);

    /// <summary>Возвращает кортеж с максимальным и минимальным значением для десятичных чисел</summary>
    /// <param name="value">Кортеж десятичных чисел</param>
    /// <returns>Кортеж с максимальным и минимальным значением</returns>
    public static (decimal min, decimal max) MaxMin(this (decimal, decimal) value) =>
        value.Item1 >= value.Item2
            ? value
            : (value.Item2, value.Item1);
}
