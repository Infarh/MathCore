using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq;

/// <summary>Методы-расширения для работы с XML (XElement, XAttribute) и XPath</summary>
public static class Linq2XmlExtensions
{
    /// <summary>Возвращает значение атрибута элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Значение атрибута или значение по умолчанию</returns>
    public static string? Attribute(this XElement element, XName Name, string? DefaultValue = null) =>
        element.Attribute(Name).ValueOrDefault(DefaultValue);

    /// <summary>Возвращает логическое значение атрибута элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Логическое значение атрибута или значение по умолчанию</returns>
    public static bool AttributeBool(this XElement element, XName Name, bool DefaultValue = false) =>
        element.Attribute(Name).ValueBoolOrDefault(DefaultValue);

    /// <summary>Возвращает целое значение атрибута элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Целое значение атрибута или значение по умолчанию</returns>
    public static int AttributeInt(this XElement element, XName Name, int DefaultValue = 0) =>
        element.Attribute(Name).ValueIntOrDefault(DefaultValue);

    /// <summary>Возвращает целое значение атрибута элемента либо null</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <returns>Целое значение атрибута или null</returns>
    public static int? AttributeIntOrNull(this XElement element, XName Name) =>
        element.Attribute(Name).ValueIntOrNull();

    /// <summary>Возвращает шестнадцатеричное целое значение атрибута элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Шестнадцатеричное целое значение атрибута или значение по умолчанию</returns>
    public static int AttributeIntHex(this XElement element, XName Name, int DefaultValue = 0) =>
        element.Attribute(Name).ValueIntHexOrDefault(DefaultValue);

    /// <summary>Возвращает вещественное значение атрибута элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="DefaultValue">Значение по умолчанию</param>
    /// <returns>Вещественное значение атрибута или значение по умолчанию</returns>
    public static double AttributeDouble(this XElement element, XName Name, double DefaultValue = 0) =>
        element.Attribute(Name).ValueDoubleOrDefault(DefaultValue);

    /// <summary>Возвращает значение атрибута элемента, преобразованное в указанный тип, либо значение по умолчанию</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="element">Элемент XML</param>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение атрибута или значение по умолчанию</returns>
    public static T? AttributeValueOrDefault<T>(this XElement element, XName Name, T? Default = default) =>
        element.Attribute(Name).ValueOrDefault(Default);

    /// <summary>Возвращает значение элемента либо значение по умолчанию</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение элемента или значение по умолчанию</returns>
    public static string? ValueOrDefault(this XElement? element, string? Default = null) => element?.Value ?? Default;

    /// <summary>Возвращает значение элемента, преобразованное в указанный тип, либо значение по умолчанию</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="element">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение элемента или значение по умолчанию</returns>
    /// <exception cref="InvalidOperationException">Невозможно преобразовать строку к типу <typeparamref name="T"/></exception>
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

    /// <summary>Возвращает значение атрибута либо значение по умолчанию</summary>
    /// <param name="element">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение атрибута или значение по умолчанию</returns>
    public static string? ValueOrDefault(this XAttribute? element, string? Default = null) => element?.Value ?? Default;

    /// <summary>Возвращает целое значение элемента либо значение по умолчанию</summary>
    /// <param name="e">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Целое значение или значение по умолчанию</returns>
    public static int ValueIntOrDefault(this XElement? e, int Default = 0) => e is null || !int.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает целое значение атрибута либо значение по умолчанию</summary>
    /// <param name="e">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Целое значение или значение по умолчанию</returns>
    public static int ValueIntOrDefault(this XAttribute? e, int Default = 0) => e is null || !int.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает целое значение атрибута либо null</summary>
    /// <param name="e">Атрибут XML</param>
    /// <returns>Целое значение или null</returns>
    public static int? ValueIntOrNull(this XAttribute? e) => e is null || !int.TryParse(e.Value, out var v) ? null : v;

    /// <summary>Возвращает шестнадцатеричное целое значение элемента либо значение по умолчанию</summary>
    /// <param name="e">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Шестнадцатеричное целое значение или значение по умолчанию</returns>
    public static int ValueIntHexOrDefault(this XElement? e, int Default = 0) => e is null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

    /// <summary>Возвращает шестнадцатеричное целое значение атрибута либо значение по умолчанию</summary>
    /// <param name="e">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Шестнадцатеричное целое значение или значение по умолчанию</returns>
    public static int ValueIntHexOrDefault(this XAttribute? e, int Default = 0) => e is null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

    /// <summary>Возвращает вещественное значение элемента либо значение по умолчанию</summary>
    /// <param name="e">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Вещественное значение или значение по умолчанию</returns>
    public static double ValueDoubleOrDefault(this XElement? e, double Default = 0) => e is null || !double.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает вещественное значение атрибута либо значение по умолчанию</summary>
    /// <param name="e">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Вещественное значение или значение по умолчанию</returns>
    public static double ValueDoubleOrDefault(this XAttribute? e, double Default = 0) => e is null || !double.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает логическое значение элемента либо значение по умолчанию</summary>
    /// <param name="e">Элемент XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Логическое значение или значение по умолчанию</returns>
    public static bool ValueBoolOrDefault(this XElement? e, bool Default = false) => e is null || !bool.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает логическое значение атрибута либо значение по умолчанию</summary>
    /// <param name="e">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Логическое значение или значение по умолчанию</returns>
    public static bool ValueBoolOrDefault(this XAttribute? e, bool Default = false) => e is null || !bool.TryParse(e.Value, out var v) ? Default : v;

    /// <summary>Возвращает значение атрибута, преобразованное в указанный тип, либо значение по умолчанию</summary>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <param name="e">Атрибут XML</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение атрибута или значение по умолчанию</returns>
    /// <exception cref="InvalidOperationException">Невозможно преобразовать строку к типу <typeparamref name="T"/></exception>
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

    /// <summary>Возвращает значение по XPath-выражению</summary>
    /// <param name="root">Корневой элемент</param>
    /// <param name="path">XPath-выражение</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение по XPath или значение по умолчанию</returns>
    public static string? GetXPathValue(this XElement root, string path, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElement(path)?.Value ?? Default;
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElement(path).NotNull().Attribute(a_name, Default);
    }

    /// <summary>Возвращает значение по XPath-выражению с пространством имён</summary>
    /// <param name="root">Корневой элемент</param>
    /// <param name="path">XPath-выражение</param>
    /// <param name="ns">Резолвер пространств имён</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Значение по XPath или значение по умолчанию</returns>
    public static string? GetXPathValue(this XElement root, string path, IXmlNamespaceResolver ns, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElement(path, ns)?.Value ?? Default;
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElement(path, ns).NotNull().Attribute(a_name, Default);
    }

    /// <summary>Возвращает последовательность значений по XPath-выражению</summary>
    /// <param name="root">Корневой элемент</param>
    /// <param name="path">XPath-выражение</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Последовательность значений</returns>
    public static IEnumerable<string?> GetXPathValues(this XElement root, string path, string? Default = null)
    {
        var a_index = path.LastIndexOf('@');
        if (a_index < 0 || path[(a_index + 1)..].IndexOf(']') >= 0)
            return root.XPathSelectElements(path).Select(e => e?.Value ?? Default);
        var a_name = path[(a_index + 1)..];
        path = path.Remove(a_index - 1);
        return root.XPathSelectElements(path).Select(e => e?.Attribute(a_name, Default));
    }

    /// <summary>Возвращает последовательность значений по XPath-выражению с пространством имён</summary>
    /// <param name="root">Корневой элемент</param>
    /// <param name="path">XPath-выражение</param>
    /// <param name="ns">Резолвер пространств имён</param>
    /// <param name="Default">Значение по умолчанию</param>
    /// <returns>Последовательность значений</returns>
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

    /// <summary>Возвращает XPath-выражение, указывающее на объект</summary>
    /// <param name="XObj">XML-объект</param>
    /// <returns>XPath-выражение или null</returns>
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

    /// <summary>Перечисляет все дочерние объекты, включая сам источник, в глубину</summary>
    /// <param name="source">Исходный объект</param>
    /// <returns>Последовательность XML-объектов</returns>
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

    /// <summary>Возвращает целое значение атрибута либо значение по умолчанию</summary>
    /// <param name="attribute">Атрибут XML</param>
    /// <param name="@default">Значение по умолчанию</param>
    /// <returns>Целое значение атрибута или значение по умолчанию</returns>
    public static int Int32OrDefault(this XAttribute? attribute, int @default = 0)
    {
        if (attribute is null) return @default;
        var str = attribute.Value;
        return string.IsNullOrEmpty(str) 
            ? @default 
            : !int.TryParse(str, out var value) ? @default : value;
    }

    /// <summary>Возвращает строковое значение атрибута либо значение по умолчанию</summary>
    /// <param name="attribute">Атрибут XML</param>
    /// <param name="@default">Значение по умолчанию</param>
    /// <returns>Строковое значение атрибута или значение по умолчанию</returns>
    public static string? StringOrDefault(this XAttribute? attribute, string? @default = null) => 
        attribute is { Value: { Length: > 0 } str } 
            ? str 
            : @default;
}