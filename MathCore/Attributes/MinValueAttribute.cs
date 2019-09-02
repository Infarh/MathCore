// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Минимально допустимое значение</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class MinValueAttribute : Attribute
    {
        /// <summary>Минимально допустимое значение</summary>
        public double Value { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="MinValueAttribute"/></summary>
        public MinValueAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="MinValueAttribute"/></summary>
        /// <param name="Value">Минимально допустимое значение</param>
        public MinValueAttribute(double Value) => this.Value = Value;
    }
}