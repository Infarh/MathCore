// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Сведения об авторских правах на участок кода</summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class CopyrightAttribute : Attribute
{
    /// <summary>Авторские права</summary>
    public string Copyright { set; get; }

    /// <summary>Ссылка на источник</summary>
#pragma warning disable IDE1006 // Стили именования
    public string url { get; set; }
#pragma warning restore IDE1006 // Стили именования

    /// <summary>Инициализация нового экземпляра <see cref="CopyrightAttribute"/></summary>
    /// <param name="Copyright">Авторские права</param>
    public CopyrightAttribute(string Copyright) => this.Copyright = Copyright;

    /// <inheritdoc />
    [NotNull]
    public override string ToString() => $"Copyright: {Copyright}";
}