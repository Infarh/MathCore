using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace MathCore.Values
{
    /// <summary>Объект, отслеживающий минимальное и максимальное значение входящей величины</summary>
    public class MinMaxValue : IResetable, IFormattable
    {
        /// <summary>Минимальное значение</summary>
        private double _Min;
        /// <summary>Максимальное значение</summary>
        private double _Max;

        /// <summary>Минимальное значение</summary>
        public double Min => _Min;

        /// <summary>Максимальное значение</summary>
        public double Max => _Max;

        /// <summary>Интервал значений</summary>
        public Interval Interval => new Interval(_Min, _Max, true);

        public MinMaxValue()
        {
            _Min = double.PositiveInfinity;
            _Max = double.NegativeInfinity;
        }

        public MinMaxValue(double Min, double Max)
        {
            _Min = Min;
            _Max = Max;
        }

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
        public void Reset()
        {
            _Min = double.PositiveInfinity;
            _Max = double.NegativeInfinity;
        }

        /// <inheritdoc />
        public override string ToString() => $"Min:{_Min}; Max:{_Max}";

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider) => 
            $"Min:{_Min.ToString(format, formatProvider)}; Max:{_Max.ToString(format, formatProvider)}";

        public string ToString(string format) => $"Min:{_Min.ToString(format)}; Max:{_Max.ToString(format)}";

        public static implicit operator Interval(MinMaxValue value) => value.Interval;
    }
}