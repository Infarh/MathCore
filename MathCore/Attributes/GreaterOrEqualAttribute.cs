#nullable enable
// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace System;

/// <summary>Значение должно быть больше, либо равно</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class GreaterOrEqualAttribute : Attribute
{
    /// <summary>Пороговое значение</summary>
    private readonly object _Value = null!;

    /// <summary>Пороговое значение</summary>
    public object Value
    {
        get => _Value;
        init
        {
            if(value is not IComparable) throw new ArgumentException("Значение должно поддерживать интерфейс IComparable", nameof(value));
            _Value = value;
        }
    }

    /// <summary>Инициализация нового экземпляра <see cref="GreaterOrEqualAttribute"/></summary>
    public GreaterOrEqualAttribute() { }

    /// <summary>Инициализация нового экземпляра <see cref="GreaterOrEqualAttribute"/></summary>
    /// <param name="Value">Пороговое значение</param>
    public GreaterOrEqualAttribute(object Value)
    {
        if(Value is not IComparable) throw new ArgumentException("Значение должно поддерживать интерфейс IComparable", nameof(Value));
        this.Value = Value;
    }

    /// <inheritdoc />
    public override string ToString() => $"value >= {Value}";
}