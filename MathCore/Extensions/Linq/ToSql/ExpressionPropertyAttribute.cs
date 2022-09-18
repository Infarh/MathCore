#nullable enable
// ReSharper disable once CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace System.Linq.ToSQL;

public class ExpressionPropertyAttribute : Attribute
{
    public string? SubPropertyName { get; private set; }

    public string PropertyName { get; private set; }

    public ExpressionPropertyAttribute(string PropertyName, string? SubPropertyName = null)
    {
        this.PropertyName    = PropertyName;
        this.SubPropertyName = SubPropertyName;
    }
}