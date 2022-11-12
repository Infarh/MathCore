using System;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
namespace MathCore.Attributes;

/// <summary>Имя элемента</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class ElementNameAttribute : Attribute
{
    /// <summary>Имя элемента</summary>
    [NotNull]
    public string Name { get; }

    /// <summary>Инициализация нового экземпляра <see cref="ElementNameAttribute"/></summary>
    /// <param name="Name">Имя элемента</param>
    public ElementNameAttribute([NotNull] string Name) => this.Name = Name;

    /// <inheritdoc />
    public override string ToString() => Name ?? throw new FormatException();

    /// <inheritdoc />
    public override int GetHashCode() => Name.GetHashCode() ^ typeof(ElementNameAttribute).GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is ElementNameAttribute A ? A.Name == Name : base.Equals(obj);

    /// <summary>Оператор неявного приведения типа <see cref="ElementNameAttribute"/> к <see cref="string"/></summary>
    /// <param name="A">Атрибут имени</param>
    [NotNull]
    public static implicit operator string([NotNull] ElementNameAttribute A) => A.Name;
}