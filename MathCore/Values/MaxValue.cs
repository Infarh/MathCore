#nullable enable
using System.Globalization;

namespace MathCore.Values;

/// <summary>Максимальное значение</summary>
/// <remarks>Инициализация нового экземпляра <see cref="MaxValue"/></remarks>
/// <param name="StartValue">Начальное значение</param>
public class MaxValue(double StartValue) : IValue<double>, IResettable, IFormattable
{
    public MaxValue() : this(double.NegativeInfinity) { }

    /// <summary>Количество значений</summary>
    private int _Count;

    /// <summary>Количество значений</summary>
    public int Count => _Count;

    /// <summary>Минимальное значение</summary>
    public double Value { get; set; } = StartValue;

    /// <summary>Инициализация нового экземпляра <see cref="MaxValue"/></summary>

    /// <summary>Добавить новое значение</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Максимальное значение из всех добавленных</returns>
    public double Add(double value) => AddValue(value) ? value : Value;

    /// <summary>Добавить новое значение</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Истина, если добавляемое значение является максимальным</returns>
    public bool AddValue(double value)
    {
        if(value <= Value) return false;
        Value = value;
        _Count++;
        return true;
    }

    /// <summary>Сбросить состояние минимального значения</summary>
    public void Reset()
    {
        Value = double.NegativeInfinity;
        _Count = 0;
    }

    /// <inheritdoc />
    public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider FormatProvider) => Value.ToString(format, FormatProvider);

    /// <summary>Возвращает форматированную строку значения </summary>
    /// <param name="FormatString">Формат значения</param>
    /// <returns>Форматированная строка значения</returns>
    public string ToString(string FormatString) => Value.ToString(FormatString);

    /// <summary>Оператор неявного приведения типа <see cref="MaxValue"/> к <see cref="double"/></summary>
    /// <param name="MaxValue">Максимальное значение</param>
    public static implicit operator double(MaxValue MaxValue) => MaxValue.Value;
}