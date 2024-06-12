#nullable enable
using System.Runtime.CompilerServices;

namespace MathCore.Data;
public static class Validate
{
    public static T IsValid<T>(this T value, Func<T, bool> Validator, string? ErrorMessage = null) =>
        Validator(value)
            ? value
            : throw new ArgumentException(ErrorMessage ?? $"Параметр имеет недопустимое значение {value?.ToString() ?? "<null>"}", nameof(value));

    public static T IsValidMin<T>(this T value, T Min, string? ErrorMessage = null, [CallerArgumentExpression(nameof(value))] string? CallerExpression = null) where T : IComparable<T> =>
        value.CompareTo(Min) < 0
            ? throw new ArgumentOutOfRangeException(nameof(value), value,
                    ErrorMessage ?? $"{CallerExpression}(={value}) должно быть больше {Min}")
                .WithData(nameof(CallerExpression), CallerExpression!)
                .WithData(nameof(Min), Min)
            : value;

    public static T IsValidMax<T>(this T value, T Max, string? ErrorMessage = null, [CallerArgumentExpression(nameof(value))] string? CallerExpression = null) where T : IComparable<T> =>
        value.CompareTo(Max) > 0
            ? throw new ArgumentOutOfRangeException(nameof(value), value,
                    ErrorMessage ?? $"{CallerExpression}(={value}) должно быть меньше {Max}")
                .WithData(nameof(CallerExpression), CallerExpression!)
                .WithData(nameof(Max), Max)
            : value;

    public static T IsValidRange<T>(
        this T value,
        Interval<T> Interval,
        string? ErrorMessage = null,
        [CallerArgumentExpression(nameof(value))] string? CallerExpression = null)
        where T : IComparable<T>
    {
        var not_valid_min = value.CompareTo(Interval.Min) < 0;
        var not_valid_max = value.CompareTo(Interval.Max) > 0;

        var error_message = ErrorMessage ?? (not_valid_min, not_valid_max) switch
        {
            (true, true) => $"Значение {CallerExpression}(={value}) вышло за пределы интервала {Interval}",
            (true, false) => $"Значение {CallerExpression}(={value}) должно быть больше {Interval.Min}",
            (false, true) => $"Значение {CallerExpression}(={value}) должно быть меньше {Interval.Max}",
            _ => null
        };

        if (error_message is not null)
            throw new ArgumentOutOfRangeException(nameof(value), value, error_message)
                .WithData(nameof(CallerExpression), CallerExpression!)
                .WithData(nameof(Interval<T>.Min), Interval.Min)
                .WithData(nameof(Interval<T>.Max), Interval.Max)
                ;

        return value;
    }
}
