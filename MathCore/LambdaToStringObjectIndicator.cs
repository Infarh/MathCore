#nullable enable
// ReSharper disable UnusedType.Global

namespace MathCore;

public class LambdaToStringObjectIndicator<T>(T t, Func<T, string> Converter)
{
    private readonly Func<T, string> _Converter = Converter;

    public T Value { get; } = t;

    /// <inheritdoc />
    public override string ToString() => _Converter(Value);

    public static implicit operator T(LambdaToStringObjectIndicator<T> Indicator) => Indicator.Value;

    public static explicit operator string(LambdaToStringObjectIndicator<T> Indicator) => Indicator.ToString();
}