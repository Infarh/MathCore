#nullable enable
using System;

// ReSharper disable UnusedType.Global

namespace MathCore;

public class LambdaToStringObjectIndicator<T>
{
    private readonly Func<T, string> _Converter;

    public T Value { get; }

    public LambdaToStringObjectIndicator(T t, Func<T, string> Converter)
    {
        _Converter = Converter;
        Value      = t;
    }

    /// <inheritdoc />
    public override string ToString() => _Converter(Value);

    public static implicit operator T(LambdaToStringObjectIndicator<T> Indicator) => Indicator.Value;

    public static explicit operator string(LambdaToStringObjectIndicator<T> Indicator) => Indicator.ToString();
}