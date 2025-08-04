#nullable enable
using MathCore.Values;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Статические методы для вычисления скользящего и обычного среднего для последовательностей чисел</summary>
public static class NumericalEnumerableExtensions
{
    /// <summary>Вычисляет среднее значение по блокам заданной длины для последовательности double</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер блока для усреднения</param>
    /// <returns>Последовательность средних значений</returns>
    public static IEnumerable<double> Average(this IEnumerable<double> collection, int length)
    {
        var aggregator = new double[length];
        var i = 0;
        foreach (var value in collection)
        {
            aggregator[i++] = value;
            if (i < length) continue;
            var sum = 0d;
            for (var j = 0; j < length; j++)
                sum += aggregator[j];
            yield return sum / length;
            i = 0;
        }

        if (i > 0)
        {
            var sum = 0d;
            for (var j = 0; j < i; j++)
                sum += aggregator[j];
            yield return sum / i;
        }
    }

    /// <summary>Вычисляет скользящее среднее для последовательности double</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер окна скользящего среднего</param>
    /// <returns>Последовательность скользящих средних</returns>
    public static IEnumerable<double> RollingAverage(this IEnumerable<double> collection, int length)
    {
        var average = new AverageValue(length);
        return collection.Select(value => average.AddValue(value));
    }

    /// <summary>Вычисляет среднее значение по блокам заданной длины для последовательности float</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер блока для усреднения</param>
    /// <returns>Последовательность средних значений</returns>
    public static IEnumerable<float> Average(this IEnumerable<float> collection, int length)
    {
        var aggregator = new double[length];
        var i = 0;
        foreach (var value in collection)
        {
            aggregator[i++] = value;
            if (i < length) continue;
            var sum = 0d;
            for (var j = 0; j < length; j++)
                sum += aggregator[j];
            yield return (float)(sum / length);
            i = 0;
        }

        if (i > 0)
        {
            var sum = 0d;
            for (var j = 0; j < i; j++)
                sum += aggregator[j];
            yield return (float)(sum / i);
        }
    }

    /// <summary>Вычисляет скользящее среднее для последовательности float</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер окна скользящего среднего</param>
    /// <returns>Последовательность скользящих средних</returns>
    public static IEnumerable<float> RollingAverage(this IEnumerable<float> collection, int length)
    {
        var average = new AverageValue(length);
        return collection.Select(value => (float)average.AddValue(value));
    }

    /// <summary>Вычисляет среднее значение по блокам заданной длины для последовательности int</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер блока для усреднения</param>
    /// <returns>Последовательность средних значений</returns>
    public static IEnumerable<double> Average(this IEnumerable<int> collection, int length)
    {
        var aggregator = new double[length];
        var i = 0;
        foreach (var value in collection)
        {
            aggregator[i++] = value;
            if (i < length) continue;
            var sum = 0d;
            for (var j = 0; j < length; j++)
                sum += aggregator[j];
            yield return sum / length;
            i = 0;
        }

        if (i > 0)
        {
            var sum = 0d;
            for (var j = 0; j < i; j++)
                sum += aggregator[j];
            yield return sum / i;
        }
    }

    /// <summary>Вычисляет скользящее среднее для последовательности int</summary>
    /// <param name="collection">Исходная последовательность</param>
    /// <param name="length">Размер окна скользящего среднего</param>
    /// <returns>Последовательность скользящих средних</returns>
    public static IEnumerable<double> RollingAverage(this IEnumerable<int> collection, int length)
    {
        var average = new AverageValue(length);
        return collection.Select(value => average.AddValue(value));
    }
}