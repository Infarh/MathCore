using System;
using System.Globalization;

namespace MathCore.Values
{
    /// <summary>Минимальное значение</summary>
    public class MinValue : IValue<double>, IResetable, IFormattable
    {
        /// <summary>Минимальное значение</summary>
        public double Value { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="MinValue"/></summary>
        public MinValue() => Value = double.PositiveInfinity;

        /// <summary>Инициализация нового экземпляра <see cref="MinValue"/></summary>
        /// <param name="StartValue">Начальное значение</param>
        public MinValue(double StartValue) => Value = StartValue;

        /// <summary>Добавить новое значение</summary>
        /// <param name="value">Добавляемое значение</param>
        /// <returns>Истина, если добавляемое значение является минимальным</returns>
        public bool AddValue(double value)
        {
            if(value >= Value) return false;
            Value = value;
            return true;
        }

        /// <summary>Сбросить состояние минимального значения</summary>
        public void Reset() => Value = double.PositiveInfinity;

        /// <inheritdoc />
        public override string ToString() => Value.ToString(CultureInfo.CurrentCulture);

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

        /// <summary>Возвращает форматированную строку значения </summary>
        /// <param name="FormatString">Формат значения</param>
        /// <returns>Форматированная строка значения</returns>
        public string ToString(string FormatString) => Value.ToString(FormatString);

        /// <summary>Оператор неявного приведения типа <see cref="MinValue"/> к <see cref="double"/></summary>
        /// <param name="MinValue">Минимальное значение</param>
        public static implicit operator double(MinValue MinValue) => MinValue.Value;
    }
}