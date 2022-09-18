#nullable enable
using System.Collections.Generic;
using System.ComponentModel;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Класс методов-расширений для перечислений <see cref="Enum"/></summary>
public static class EnumExtensions
{
    /// <summary>Получить описание поля - значение атрибута <see cref="DescriptionAttribute"/></summary>
    /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
    /// <exception cref="InvalidOperationException">
    /// Если этот член принадлежит типу, который загружается в контекст только для отражения.
    /// Смотрите раздел Как загрузить сборки в контекст только для отражения.
    /// </exception>
    public static TAttribute[]? GetValueAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var attribute_type = typeof(TAttribute);
        var value_type     = value.GetType();
        var field          = value_type.GetField(value.ToString());
        return (TAttribute[])field.GetCustomAttributes(attribute_type, false);
    }

    private static TValue[] GetAttributeValues<TAttribute, TValue>(this Enum value, Func<TAttribute, TValue> Selector)
        where TAttribute : Attribute
    {
        var attributes = value.GetValueAttribute<TAttribute>();
        if (attributes is null || attributes.Length == 0) return Array.Empty<TValue>();
        var result = new TValue[attributes.Length];
        for (var i = 0; i < result.Length; i++)
            result[i] = Selector(attributes[i]);
        return result;
    }

    /// <summary>Получить описание поля - значение атрибута <see cref="DescriptionAttribute"/></summary>
    /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
    public static string? GetDescription(this Enum value) =>
        value.GetAttributeValues<DescriptionAttribute, string>(a => a.Description) is { Length: > 0 } descriptions
            ? string.Join(Environment.NewLine, descriptions)
            : string.Empty;

    public static IEnumerable<string> GetDescriptions(this Enum value) => value.GetAttributeValues<DescriptionAttribute, string>(a => a.Description);

    /// <summary>Получить описание поля - значение атрибута <see cref="DisplayNameAttribute"/></summary>
    /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
    public static string? GetDisplayName(this Enum value) =>
        value.GetAttributeValues<DisplayNameAttribute, string>(a => a.DisplayName) is { Length: > 0 } descriptions
            ? string.Join(Environment.NewLine, descriptions)
            : string.Empty;

    /// <summary>Получить описание поля - значение атрибута <see cref="DisplayNameAttribute"/></summary>
    /// <exception cref="TypeLoadException">Если невозможно загрузить атрибуты типа</exception>
    public static IEnumerable<string> GetDisplayNames(this Enum value) => value.GetAttributeValues<DisplayNameAttribute, string>(a => a.DisplayName);
}