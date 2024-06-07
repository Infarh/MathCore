#nullable enable
using System.Globalization;

namespace MathCore.Values;

/// <summary>Минимальное значение</summary>
/// <remarks>Инициализация нового экземпляра <see cref="MinValue"/></remarks>
/// <param name="StartValue">Начальное значение</param>
public class MinValue(double StartValue) : IValue<double>, IResettable, IFormattable
{
    /// <summary>Инициализация нового экземпляра <see cref="MinValue"/></summary>
    public MinValue() : this(double.PositiveInfinity) { }

    /// <summary>Количество значений</summary>
    private int _Count;

    /// <summary>Количество значений</summary>
    public int Count => _Count;

    /// <summary>Минимальное значение</summary>
    public double Value { get; set; } = StartValue;

    /// <summary>Добавить новое значение</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Минимальный элемент из всех добавленных</returns>
    public double Add(double value) => AddValue(value) ? value : Value;

    /// <summary>Добавить новое значение</summary>
    /// <param name="value">Добавляемое значение</param>
    /// <returns>Истина, если добавляемое значение является минимальным</returns>
    public bool AddValue(double value)
    {
        if(value >= Value) return false;
        Value = value;
        _Count++;
        return true;
    }

    /// <summary>Сбросить состояние минимального значения</summary>
    public void Reset()
    {
        Value = double.PositiveInfinity;
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

    /// <summary>Оператор неявного приведения типа <see cref="MinValue"/> к <see cref="double"/></summary>
    /// <param name="MinValue">Минимальное значение</param>
    public static implicit operator double(MinValue MinValue) => MinValue.Value;
}