#nullable enable
// ReSharper disable UnusedMember.Global

namespace MathCore.Values;

/// <summary>Объект, отслеживающий минимальное и максимальное значение входящей величины</summary>
public class MinMaxValue(double Min, double Max) : IResettable, IFormattable
{
    public MinMaxValue() : this(double.PositiveInfinity, double.NegativeInfinity) { }

    /// <summary>Минимальное значение</summary>
    private double _Min = Min;

    /// <summary>Максимальное значение</summary>
    private double _Max = Max;

    /// <summary>Минимальное значение</summary>
    public double Min => _Min;

    /// <summary>Максимальное значение</summary>
    public double Max => _Max;

    /// <summary>Интервал значений</summary>
    public Interval Interval => new(_Min, _Max, true);

    public MinMaxValue(IEnumerable<double> values) : this() => values.Foreach(SetValue);

    public void SetValue(double x)
    {
        if(x < _Min) _Min = x;
        if(x > _Max) _Max = x;
    }

    public double AddValue(double x)
    {
        SetValue(x);
        return x;
    }

    /// <inheritdoc />
    public void Reset() => (_Min, _Max) = (double.PositiveInfinity, double.NegativeInfinity);

    /// <inheritdoc />
    public override string ToString() => $"Min:{_Min}; Max:{_Max}";

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider FormatProvider) => 
        $"Min:{_Min.ToString(format, FormatProvider)}; Max:{_Max.ToString(format, FormatProvider)}";

    public string ToString(string format) => $"Min:{_Min.ToString(format)}; Max:{_Max.ToString(format)}";

    public static implicit operator Interval(MinMaxValue value) => value.Interval;
}