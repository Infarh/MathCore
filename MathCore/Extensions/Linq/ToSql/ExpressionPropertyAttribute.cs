#nullable enable
// ReSharper disable once CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace System.Linq.ToSQL;

public class ExpressionPropertyAttribute(string PropertyName, string? SubPropertyName = null) : Attribute
{
    public string? SubPropertyName { get; private set; } = SubPropertyName;

    public string PropertyName { get; private set; } = PropertyName;
}