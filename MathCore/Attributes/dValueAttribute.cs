// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System
{
    /// <summary>Допустимая величина изменения значения</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class dValueAttribute : Attribute
    {
        /// <summary>Допустимая величина изменения значения</summary>
        public double dV { get; set; }

        /// <summary>Инициализация нового экземпляра <see cref="dValueAttribute"/></summary>
        public dValueAttribute() { }

        /// <summary>Инициализация нового экземпляра <see cref="dValueAttribute"/></summary>
        /// <param name="dV">Допустимая величина изменения значения</param>
        public dValueAttribute(double dV) => this.dV = dV;
    }
}