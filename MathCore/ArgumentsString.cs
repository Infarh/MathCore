#nullable enable
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MathCore;

/// <summary>Аргументы командной строки</summary>
public class ArgumentsString
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Строки значений аргументов</summary>
    private readonly string[]? _Arguments;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Число аргументов</summary>
    public int Count => _Arguments?.Length ?? 0;

    /// <summary>Получение аргумента по индексу</summary>
    /// <param name="index">Индекс аргумента</param>
    /// <returns>Значение аргумента по указанному индексу</returns>
    public ref readonly string this[int index] => ref _Arguments[index];

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Инициализация нового экземпляра <see cref="ArgumentsString"/></summary>
    /// <param name="Arguments">Массив значений аргументов</param>
    public ArgumentsString(string[] Arguments) => _Arguments = Arguments;

    /* ------------------------------------------------------------------------------------------ */

    /// <inheritdoc />
    public override string ToString() => _Arguments is { Length: > 0 } args 
        ? string.Concat(args) 
        : string.Empty;

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Оператор неявного приведения типа массива строк к <see cref="ArgumentsString"/></summary>
    /// <param name="Arguments">Массив строковых значений аргументов</param>
    /// <returns>Экземпляр <see cref="ArgumentsString"/></returns>
    public static implicit operator ArgumentsString(string[] Arguments) => new(Arguments);

    /// <summary>Оператор неявного приведения <see cref="ArgumentsString"/> к типу массива строк</summary>
    /// <param name="Arguments">Экземпляр <see cref="ArgumentsString"/></param>
    /// <returns>Массив строковых значений аргументов</returns>
    public static implicit operator string[](ArgumentsString Arguments) => Arguments._Arguments;

    /// <summary>Оператор неявного приведения <see cref="ArgumentsString"/> к строке</summary>
    /// <param name="Argument">Экземпляр <see cref="ArgumentsString"/></param>
    /// <returns>Строковое представление <see cref="ArgumentsString"/></returns>
    public static explicit operator string(ArgumentsString Argument) => Argument.ToString();

    /* ------------------------------------------------------------------------------------------ */
}