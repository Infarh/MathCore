// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace System;

/// <summary>Влияние на</summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class AffectsTheAttribute : Attribute
{
    /// <summary>Элемент, на который оказывается влияние</summary>
    public string Name { get; set; }

    /// <summary>Инициализация нового экземпляра <see cref="AffectsTheAttribute"/></summary>
    public AffectsTheAttribute() { }

    /// <summary>Инициализация нового экземпляра <see cref="AffectsTheAttribute"/></summary>
    /// <param name="Name">Имя элемента, на что помеченный элемент оказывает влияние</param>
    public AffectsTheAttribute(string Name) => this.Name = Name;
}