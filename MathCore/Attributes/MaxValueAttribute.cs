// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System;

/// <summary>Максимально допустимое значение</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class MaxValueAttribute : Attribute
{
    /// <summary>Максимально допустимое значение</summary>
    public double Value { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="MaxValueAttribute"/></summary>
    public MaxValueAttribute() { }

    /// <summary>Инициализация нового экземпляра <see cref="MaxValueAttribute"/></summary>
    /// <param name="Value">Максимально допустимое значение</param>
    public MaxValueAttribute(double Value) => this.Value = Value;
}