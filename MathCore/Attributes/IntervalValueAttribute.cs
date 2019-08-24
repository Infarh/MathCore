// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Допустимый интервал значений</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class IntervalValueAttribute : Attribute
    {
        /// <summary>Минимальное значение</summary>
        public double Min { get => Interval.Min; set => Interval = Interval.SetMin(value); }

        /// <summary>Максимальное значение</summary>
        public double Max { get => Interval.Max; set => Interval = Interval.SetMax(value); }

        /// <summary>Допустимый интервал значений</summary>
        public MathCore.Interval Interval { set; get; }

        /// <summary>Инициализация нового экземпляра <see cref="IntervalValueAttribute"/></summary>
        public IntervalValueAttribute() => Interval = new MathCore.Interval(double.NegativeInfinity, double.PositiveInfinity);

        /// <summary>Инициализация нового экземпляра <see cref="IntervalValueAttribute"/></summary>
        /// <param name="Interval">Допустимый интервал значений</param>
        public IntervalValueAttribute(MathCore.Interval Interval) => this.Interval = Interval;

        /// <summary>Инициализация нового экземпляра <see cref="IntervalValueAttribute"/></summary>
        /// <param name="Min">Минимально допустимое значение</param>
        /// <param name="Max">Максимально допустимое значение</param>
        public IntervalValueAttribute(double Min, double Max = double.PositiveInfinity) => Interval = new MathCore.Interval(Min, Max);
    }
}