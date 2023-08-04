#nullable enable


// ReSharper disable once CheckNamespace
namespace MathCore;

/// <summary>Класс методов-расширений для перечисления комплексных чисел</summary>
public static class ComplexEnumerableExtensions
{
    /// <summary>Получить перечисление действительных частей комплексных чисел</summary>
    /// <param name="items">Перечисление комплексных чисел</param>
    /// <returns>Перечисление действительных частей комплексных чисел</returns>
    public static IEnumerable<double> ToRe(this IEnumerable<Complex> items) => items.Select(z => z.Re);

    /// <summary>Получить перечисление мнимых частей комплексных чисел</summary>
    /// <param name="items">Перечисление комплексных чисел</param>
    /// <returns>Перечисление мнимых частей комплексных чисел</returns>
    public static IEnumerable<double> ToIm(this IEnumerable<Complex> items) => items.Select(z => z.Im);

    /// <summary>Получить перечисление модулей комплексных чисел</summary>
    /// <param name="items">Перечисление комплексных чисел</param>
    /// <returns>Перечисление модулей комплексных чисел</returns>
    public static IEnumerable<double> ToAbs(this IEnumerable<Complex> items) => items.Select(z => z.Abs);

    /// <summary>Получить перечисление аргументов комплексных чисел</summary>
    /// <param name="items">Перечисление комплексных чисел</param>
    /// <returns>Перечисление аргументов комплексных чисел</returns>
    public static IEnumerable<double> ToArg(this IEnumerable<Complex> items) => items.Select(z => z.Arg);

    /// <summary>Получить перечисление аргументов (в градусах) комплексных чисел</summary>
    /// <param name="items">Перечисление комплексных чисел</param>
    /// <returns>Перечисление аргументов (в градусах) комплексных чисел</returns>
    public static IEnumerable<double> ToArgDeg(this IEnumerable<Complex> items) => items.Select(z => z.Arg * Consts.ToDeg);

    /// <summary>Сумма комплексных чисел последовательности</summary>
    /// <param name="items">Перечисление комплексных чисел для сложения</param>
    /// <returns>Сумма всех комплексных чисел в последовательности</returns>
    public static Complex Sum(this IEnumerable<Complex> items) => items.Aggregate(Complex.Zero, (Z, z) => Z + z);

    /// <summary>Произведение комплексных чисел последовательности</summary>
    /// <param name="items">Перечисление комплексных чисел для перемножения</param>
    /// <returns>Произведение всех комплексных чисел в последовательности</returns>
    public static Complex Mul(this IEnumerable<Complex> items) => items.Aggregate(Complex.Real, (Z, z) => Z * z);
}