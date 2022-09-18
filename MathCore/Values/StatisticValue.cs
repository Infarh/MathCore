using System;
using System.Collections.Generic;

namespace MathCore.Values
{
    /// <summary>Статистическая информация о значении</summary>
    public class StatisticValue : IResettable
    {
        /// <summary>Среднее значение</summary>
        private readonly AverageValue _Average;

        /// <summary>Среднее значение квадрата величины</summary>
        private readonly AverageValue _Average2;

        /// <summary>Диапазон значений</summary>
        private readonly MinMaxValue _MinMax;

        /// <summary>Математическое ожидание величины</summary>
        public double M => _Average.Value;

        /// <summary>Дисперсия</summary>
        public double D => _Average2.Value - M.Pow2();

        /// <summary>Интервал значений, в который попадает величина</summary>
        public Interval MinMax => _MinMax;

        /// <summary>Размер выборки</summary>
        public int Count { get; private set; }

        /// <summary>Инициализация нового экземпляра <see cref="StatisticValue"/></summary>
        /// <param name="Length">Требуемый размер выборки</param>
        public StatisticValue([MinValue(1)] int Length = 100)
        {
            _Average = new AverageValue(Length);
            _Average2 = new AverageValue(Length);
            _MinMax = new MinMaxValue();
        }

        /// <summary>Добавить значение к оценке статистики</summary>
        /// <param name="x">Добавляемое значение</param>
        public void AddValue(double x)
        {
            _Average.AddValue(x);
            _Average2.AddValue(x * x);
            _MinMax.AddValue(x);
            Count++;
        }

        /// <summary>Добавить перечисление объектов к оценке статистики</summary>
        /// <param name="collection">Добавляемая последовательность значений</param>
        public void AddEnumerable(IEnumerable<double> collection)
        {
            switch (collection)
            {
                case double[] values:
                    foreach (var value in values)
                        AddValue(value);
                    break;

                case List<double> values:
                    foreach (var value in values)
                        AddValue(value);
                    break;

                case IList<double> values:
                    foreach (var value in values)
                        AddValue(value);
                    break;

                default:
                    foreach (var value in collection)
                        AddValue(value);
                    break;
            }
        }

        /// <summary>Сбросить состояние оценки</summary>
        public void Reset()
        {
            _Average.Reset();
            _Average2.Reset();
            _MinMax.Reset();
            Count = 0;
        }

        /// <inheritdoc />
        public override string ToString() => $"{M.RoundAdaptive(2)}(±{D.RoundAdaptive(2)})[{MinMax.Min.RoundAdaptive(2)}:{MinMax.Max.RoundAdaptive(2)}]";
    }
}