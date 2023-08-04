#nullable enable
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq;

public static class Linq2XmlExtensions
{
    public static string? Attribute(this XElement element, XName Name, string? DefaultValue = null) =>
        element.Attribute(Name).ValueOrDefault(DefaultValue);

    public static bool AttributeBool(this XElement element, XName Name, bool DefaultValue = false) =>
        element.Attribute(Name).ValueBoolOrDefault(DefaultValue);

    public static int AttributeInt(this XElement element, XName Name, int DefaultValue = 0) =>
        element.Attribute(Name).ValueIntOrDefault(DefaultValue);

    public static int? AttributeIntOrNull(this XElement element, XName Name) =>
        element.Attribute(Name).ValueIntOrNull();

    public static int AttributeIntHex(this XElement element, XName Name, int DefaultValue = 0) =>
        element.Attribute(Name).ValueIntHexOrDefault(DefaultValue);

    public static double AttributeDouble(this XElement element, XName Name, double DefaultValue = 0) =>
        element.Attribute(Name).ValueDoubleOrDefault(DefaultValue);

    public static T? AttributeValueOrDefault<T>(this XElement element, XName Name, T? Default = default) =>
        element.Attribute(Name).ValueOrDefault(Default);

    public static string? ValueOrDefault(this XElement? element, string? Default = null) => element?.Value ?? Default;

    public static T? ValueOrDefault<T>(this XElement element, T? Default = default)
    {
        var str = element.ValueOrDefault();
        switch (str)
        {
            case T:    return (T)(object)str;
            case null: return Default;
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));
        return !converter.CanConvertFrom(typeof(string))
            ? throw new InvalidOperationException($"Невозможно преобразовать тип {typeof(string)} к типу {typeof(T)}")
            : (T?)converter.ConvertFrom(str);
    }

    public static string? ValueOrDefault(this XAttribute? element, string? Default = null) => element?.Value ?? Default;

    public static int ValueIntOrDefault(this XElement? e, int Default = 0) => e is null || !int.TryParse(e.Value, out var v) ? Default : v;

    public static int ValueIntOrDefault(this XAttribute? e, int Default = 0) => e is null || !int.TryParse(e.Value, out var v) ? Default : v;

    public static int? ValueIntOrNull(this XAttribute? e) => e is null || !int.TryParse(e.Value, out var v) ? null : v;

    public static int ValueIntHexOrDefault(this XElement? e, int Default = 0) => e is null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

    public static int ValueIntHexOrDefault(this XAttribute? e, int Default = 0) => e is null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

    public static double ValueDoubleOrDefault(this XElement? e, double Default = 0) => e is null || !double.TryParse(e.Value, out var v) ? Default : v;

    public static double ValueDoubleOrDefault(this XAttribute? e, double Default = 0) => e is null || !double.TryParse(e.Value, out var v) ? Default : v;

    public static bool ValueBoolOrDefault(this XElement? e, bool Default = false) => e is null || !bool.TryParse(e.Value, out var v) ? Default : v;

    public static bool ValueBoolOrDefault(this XAttribute? e, bool Default = false) => e is null || !bool.TryParse(e.Value, out var v) ? Default : v;

    public static T? ValueOrDefault<T>(this XAttribute? e, T? Default = default)
    {
        if (e is null) return Default;
        var str = e.Value;
        if (str is T) return (T)(object)str;
        var converter = TypeDescriptor.GetConverter(typeof(T));
        return !converter.CanConvertFrom(typeof(string))
            ? throw new InvalidOperationException($"Невозможно преобразовать тип {typeof(string)} к типу {typeof(T)}")
            : (T?)converter.ConvertFrom(str);
    }

    public static string? GetXPathValue(this XElement root, string path, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElement(path)?.Value ?? Default;
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElement(path).NotNull().Attribute(a_name, Default);
    }

    public static string? GetXPathValue(this XElement root, string path, IXmlNamespaceResolver ns, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElement(path, ns)?.Value ?? Default;
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElement(path, ns).NotNull().Attribute(a_name, Default);
    }

    public static IEnumerable<string?> GetXPathValues(this XElement root, string path, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElements(path).Select(e => e?.Value ?? Default);
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElements(path).Select(e => e?.Attribute(a_name, Default));
    }

    public static IEnumerable<string?> GetXPathValues(
        this XElement root,
        string path,
        IXmlNamespaceResolver ns,
        string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElements(path, ns).Select(e => e?.Value ?? Default);
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElements(path, ns).Select(e => e?.Attribute(a_name, Default));
    }

    private static string GetQName(XElement element)
    {
        var prefix = element.GetPrefixOfNamespace(element.Name.Namespace);
        return element.Name.Namespace == XNamespace.None || prefix is null
            ? element.Name.LocalName
            : $"{prefix}:{element.Name.LocalName}";
    }

    private static string GetQName(XAttribute attribute)
    {
        var prefix = attribute.Parent?.GetPrefixOfNamespace(attribute.Name.Namespace);
        return attribute.Name.Namespace == XNamespace.None || prefix is null
            ? attribute.Name.ToString()
            : $"{prefix}:{attribute.Name.LocalName}";
    }

    private static string NameWithPredicate(XElement element) =>
        element.Parent != null && element.Parent.Elements(element.Name).Count() != 1
            ? $"{GetQName(element)}[{element.ElementsBeforeSelf(element.Name).Count() + 1}]"
            : GetQName(element);

    private static string StrCat<T>(this IEnumerable<T> source, string? separator) =>
        source.Aggregate(new StringBuilder(), (S, i) => S.Append(i).Append(separator), S => S.ToString());

    public static string? GetXPath(this XObject XObj) => XObj.Parent is null ? GetXPathNoParent(XObj) : GetXPathParent(XObj);

    private static string? GetXPathNoParent(XObject XObj) => XObj switch
    {
        XDocument => ".",
        XElement element => $"/{NameWithPredicate(element)}",
        XText => null,
        XComment comment => $"/{(comment.Document?.Nodes().OfType<XComment>().Count() != 1 ? $"comment()[{comment.NodesBeforeSelf().OfType<XComment>().Count() + 1}]" : "comment()")}",
        XProcessingInstruction instruction => $"/{(instruction.Document?.Nodes().OfType<XProcessingInstruction>().Count() != 1 ? $"processing-instruction()[{instruction.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1}]" : "processing-instruction()")}",
        _ => null
    };

    private static string? GetXPathParent(XObject XObj) => XObj switch
    {
        XElement element => $"/{element.Ancestors().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{NameWithPredicate(element)}",
        XAttribute attribute => $"/{attribute.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}@{GetQName(attribute)}",
        XComment comment => $"/{comment.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(comment.Parent?.Nodes().OfType<XComment>().Count() != 1 ? $"comment()[{comment.NodesBeforeSelf().OfType<XComment>().Count() + 1}]" : "comment()")}",
        XCData data => $"/{data.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(data.Parent?.Nodes().OfType<XText>().Count() != 1 ? $"text()[{data.NodesBeforeSelf().OfType<XText>().Count() + 1}]" : "text()")}",
        XText text => $"/{text.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(text.Parent?.Nodes().OfType<XText>().Count() != 1 ? $"text()[{text.NodesBeforeSelf().OfType<XText>().Count() + 1}]" : "text()")}",
        XProcessingInstruction instruction => $"/{instruction.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(instruction.Parent?.Nodes().OfType<XProcessingInstruction>().Count() != 1 ? $"processing-instruction()[{instruction.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1}]" : "processing-instruction()")}",
        _ => null
    };

    public static IEnumerable<XObject> DescendantXObjects(this XObject source)
    {
        yield return source;

        if (source is XElement element)
            foreach (var attribute in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
                yield return attribute;

        if (source is not XContainer container) 
            yield break;

        foreach (var s in container.Nodes().SelectMany(child => child.DescendantXObjects()))
            yield return s;
    }

    public static int Int32OrDefault(this XAttribute? attribute, int @default = 0)
    {
        if (attribute is null) return @default;
        var str = attribute.Value;
        return string.IsNullOrEmpty(str) 
            ? @default 
            : !int.TryParse(str, out var value) ? @default : value;
    }

    public static string? StringOrDefault(this XAttribute? attribute, string? @default = null) => 
        attribute is { Value: { Length: > 0 } str } 
            ? str 
            : @default;
}