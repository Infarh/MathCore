#nullable enable
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Методы-расширения для логических значений</summary>
public static class BoolExtensions
{
    /// <summary>
    /// Вычислить поэлементную операцию И со всеми элементами перечисления.
    /// Все элементы перечисления будут перебраны в обязательном порядке
    /// </summary>
    /// <param name="items">Перечисление логических значений</param>
    /// <returns>Результат вычисления логического И для всех значений перечисления</returns>
    public static bool And(this IEnumerable<bool> items)
    {
        switch (items)
        {
            case null:
                throw new ArgumentNullException(nameof(items));

            case bool[] array:
                foreach (var item in array)
                    if (!item) return false;
                return true;

            case List<bool> list:
                foreach (var item in list)
                    if (!item) return false;
                return true;

            case IList<bool> list:
                foreach (var item in list)
                    if (!item) return false;
                return true;

            default:
                return items.Aggregate(true, (r, v) => r && v);
        }
    }

    /// <summary>
    /// Вычислить поэлементную операцию (ленивое) И со всеми элементами перечисления.
    /// Перебор элементов перечисления производится до появления первого значения <see langword="false"/>
    /// </summary>
    /// <param name="items">Перечисление логических значений</param>
    /// <returns>Результат вычисления логического И для значений перечисления</returns>
    public static bool AndLazy(this IEnumerable<bool> items)
    {
        switch (items)
        {
            case null:
                throw new ArgumentNullException(nameof(items));

            case bool[] array:
                foreach (var item in array)
                    if (!item) return false;
                return true;

            case List<bool> list:
                foreach (var item in list)
                    if (!item) return false;
                return true;

            case IList<bool> list:
                foreach (var item in list)
                    if (!item) return false;
                return true;

            default:
                return items.Any(item => !item);
        }
    }

    /// <summary>
    /// Вычислить поэлементную операцию ИЛИ со всеми элементами перечисления.
    /// Все элементы перечисления будут перебраны в обязательном порядке
    /// </summary>
    /// <param name="items">Перечисление логических значений</param>
    /// <returns>Результат вычисления логического ИЛИ для всех значений перечисления</returns>
    public static bool Or(this IEnumerable<bool> items)
    {
        switch (items)
        {
            case null:
                throw new ArgumentNullException(nameof(items));

            case bool[] array:
                foreach (var item in array)
                    if (item) return true;
                return false;

            case List<bool> list:
                foreach (var item in list)
                    if (item) return true;
                return false;

            case IList<bool> list:
                foreach (var item in list)
                    if (!item) return true;
                return false;

            default:
                return items.Aggregate(false, (r, v) => r || v);
        }
    }

    /// <summary>
    /// Вычислить поэлементную операцию (ленивое) ИЛИ со всеми элементами перечисления.
    /// Перебор элементов перечисления производится до появления первого значения <see langword="false"/>
    /// </summary>
    /// <param name="items">Перечисление логических значений</param>
    /// <returns>Результат вычисления логического ИЛИ для значений перечисления</returns>
    public static bool OrLazy(this IEnumerable<bool> items)
    {
        switch (items)
        {
            case null:
                throw new ArgumentNullException(nameof(items));

            case bool[] array:
                foreach (var item in array)
                    if (item) return true;
                return false;

            case List<bool> list:
                foreach (var item in list)
                    if (item) return true;
                return false;

            case IList<bool> list:
                foreach (var item in list)
                    if (!item) return true;
                return false;

            default:
                return items.Any(item => item);
        }
    }
}