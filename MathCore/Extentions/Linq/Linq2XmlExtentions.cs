using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using MathCore.Annotations;

namespace System.Xml.Linq
{
    public static class Linq2XmlExtentions
    {
        public static string Attribute(this XElement element, XName Name, string DefaultValue = null)
            => element.Attribute(Name).ValueOrDefault(DefaultValue);

        public static bool AttributeBool(this XElement element, XName Name, bool DefaultValue = false)
            => element.Attribute(Name).ValueBoolOrDefault(DefaultValue);

        public static int AttributeInt(this XElement element, XName Name, int DefaultValue = 0)
            => element.Attribute(Name).ValueIntOrDefault(DefaultValue);

        public static int? AttributeIntOrNull(this XElement element, XName Name)
            => element.Attribute(Name).ValueIntOrNull();

        public static int AttributeIntHex(this XElement element, XName Name, int DefaultValue = 0)
            => element.Attribute(Name).ValueIntHexOrDefault(DefaultValue);

        public static double AttributeDouble(this XElement element, XName Name, double DefaultValue = 0)
            => element.Attribute(Name).ValueDoubleOrDefault(DefaultValue);

        public static T AttributeValueOrDefault<T>(this XElement element, XName Name, T Default = default) =>
            element.Attribute(Name).ValueOrDefault(Default);

        public static string ValueOrDefault(this XElement element, string Default = null) => element?.Value ?? Default;

        public static T ValueOrDefault<T>(this XElement element, T Default = default)
        {
            var str = element.ValueOrDefault();
            if(str is T) return (T)(object)str;
            if(str == null) return Default;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if(!converter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException($"Невозможно преобразовать тип {typeof(string)} к типу {typeof(T)}");
            return (T)converter.ConvertFrom(str);
        }

        public static string ValueOrDefault(this XAttribute element, string Default = null) => element?.Value ?? Default;

        public static int ValueIntOrDefault(this XElement e, int Default = 0) => e == null || !int.TryParse(e.Value, out var v) ? Default : v;

        public static int ValueIntOrDefault(this XAttribute e, int Default = 0) => e == null || !int.TryParse(e.Value, out var v) ? Default : v;

        public static int? ValueIntOrNull(this XAttribute e) => e == null || !int.TryParse(e.Value, out var v) ? (int?)null : v;

        public static int ValueIntHexOrDefault(this XElement e, int Default = 0) => e == null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

        public static int ValueIntHexOrDefault(this XAttribute e, int Default = 0) => e == null || !int.TryParse(e.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var v) ? Default : v;

        public static double ValueDoubleOrDefault(this XElement e, double Default = 0) => e == null || !double.TryParse(e.Value, out var v) ? Default : v;

        public static double ValueDoubleOrDefault(this XAttribute e, double Default = 0) => e == null || !double.TryParse(e.Value, out var v) ? Default : v;

        public static bool ValueBoolOrDefault(this XElement e, bool Default = false) => e == null || !bool.TryParse(e.Value, out var v) ? Default : v;

        public static bool ValueBoolOrDefault(this XAttribute e, bool Default = false) => e == null || !bool.TryParse(e.Value, out var v) ? Default : v;

        public static T ValueOrDefault<T>(this XAttribute e, T Default = default)
        {
            if(e == null) return Default;
            var str = e.Value;
            if(str is T) return (T)(object)str;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if(!converter.CanConvertFrom(typeof(string)))
                throw new InvalidOperationException($"Невозможно преобразовать тип {typeof(string)} к типу {typeof(T)}");
            return (T)converter.ConvertFrom(str);
        }

        public static string GetXPathValue(this XElement root, string path, string Default = null)
        {
            var a_index = path.LastIndexOf('@');
            if(a_index < 0 || path.Substring(a_index + 1).IndexOf(']') >= 0)
                return root.XPathSelectElement(path)?.Value ?? Default;
            var a_name = path.Substring(a_index + 1);
            path = path.Remove(a_index - 1);
            return root.XPathSelectElement(path).Attribute(a_name, Default);
        }

        public static string GetXPathValue(this XElement root, string path, IXmlNamespaceResolver ns, string Default = null)
        {
            var a_index = path.LastIndexOf('@');
            if(a_index < 0 || path.Substring(a_index + 1).IndexOf(']') >= 0)
                return root.XPathSelectElement(path, ns)?.Value ?? Default;
            var a_name = path.Substring(a_index + 1);
            path = path.Remove(a_index - 1);
            return root.XPathSelectElement(path, ns).Attribute(a_name, Default);
        }

        public static IEnumerable<string> GetXPathValues(this XElement root, string path, string Default = null)
        {
            var a_index = path.LastIndexOf('@');
            if(a_index < 0 || path.Substring(a_index + 1).IndexOf(']') >= 0)
                return root.XPathSelectElements(path).Select(e => e?.Value ?? Default);
            var a_name = path.Substring(a_index + 1);
            path = path.Remove(a_index - 1);
            return root.XPathSelectElements(path).Select(e => e?.Attribute(a_name, Default));
        }

        [NotNull]
        public static IEnumerable<string> GetXPathValues(
            [NotNull] this XElement root, 
            [NotNull] string path, 
            IXmlNamespaceResolver ns, 
            [CanBeNull] string Default = null)
        {
            var a_index = path.LastIndexOf('@');
            if(a_index < 0 || path.Substring(a_index + 1).IndexOf(']') >= 0)
                return root.XPathSelectElements(path, ns).Select(e => e?.Value ?? Default);
            var a_name = path.Substring(a_index + 1);
            path = path.Remove(a_index - 1);
            return root.XPathSelectElements(path, ns).Select(e => e?.Attribute(a_name, Default));
        }

        [NotNull]
        private static string GetQName([NotNull] XElement element)
        {
            var prefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            return element.Name.Namespace == XNamespace.None || prefix is null
                ? element.Name.LocalName
                : $"{prefix}:{element.Name.LocalName}";
        }

        [NotNull]
        private static string GetQName([NotNull] XAttribute attribute)
        {
            var prefix = attribute.Parent?.GetPrefixOfNamespace(attribute.Name.Namespace);
            return attribute.Name.Namespace == XNamespace.None || prefix is null
                ? attribute.Name.ToString()
                : $"{prefix}:{attribute.Name.LocalName}";
        }

        [NotNull] private static string NameWithPredicate([NotNull] XElement element) =>
            element.Parent != null && element.Parent.Elements(element.Name).Count() != 1
                ? $"{GetQName(element)}[{element.ElementsBeforeSelf(element.Name).Count() + 1}]"
                : GetQName(element);

        [NotNull] private static string StrCat<T>([NotNull] this IEnumerable<T> source, [CanBeNull] string separator) => 
            source.Aggregate(new StringBuilder(), (S, i) => S.Append(i.ToString()).Append(separator), S => S.ToString());

        [CanBeNull]
        public static string GetXPath([NotNull] this XObject xobj)
        {
            if(xobj.Parent == null)
                switch (xobj)
                {
                    case XDocument _:
                        return ".";
                    case XElement element:
                        return $"/{NameWithPredicate(element)}";
                    case XText _:
                        return null;
                    case XComment comment:
                        return
                            $"/{(comment.Document?.Nodes().OfType<XComment>().Count() != 1 ? $"comment()[{comment.NodesBeforeSelf().OfType<XComment>().Count() + 1}]" : "comment()")}";
                    default:
                        return xobj is XProcessingInstruction processing_instruction
                            ? $"/{(processing_instruction.Document?.Nodes().OfType<XProcessingInstruction>().Count() != 1 ? $"processing-instruction()[{processing_instruction.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1}]" : "processing-instruction()")}"
                            : null;
                }

            switch (xobj)
            {
                case XElement element:
                    return $"/{element.Ancestors().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{NameWithPredicate(element)}";
                case XAttribute attribute:
                    return $"/{attribute.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}@{GetQName(attribute)}";
                case XComment comment:
                    return $"/{comment.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(comment.Parent?.Nodes().OfType<XComment>().Count() != 1 ? $"comment()[{comment.NodesBeforeSelf().OfType<XComment>().Count() + 1}]" : "comment()")}";
                case XCData data:
                    return $"/{data.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(data.Parent?.Nodes().OfType<XText>().Count() != 1 ? $"text()[{data.NodesBeforeSelf().OfType<XText>().Count() + 1}]" : "text()")}";
                case XText text:
                    return $"/{text.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(text.Parent?.Nodes().OfType<XText>().Count() != 1 ? $"text()[{text.NodesBeforeSelf().OfType<XText>().Count() + 1}]" : "text()")}";
                default:
                    return xobj is XProcessingInstruction pi ? $"/{pi.Parent?.AncestorsAndSelf().InDocumentOrder().Select(NameWithPredicate).StrCat("/")}{(pi.Parent?.Nodes().OfType<XProcessingInstruction>().Count() != 1 ? $"processing-instruction()[{pi.NodesBeforeSelf().OfType<XProcessingInstruction>().Count() + 1}]" : "processing-instruction()")}" : null;
            }
        }

        [NotNull]
        public static IEnumerable<XObject> DescendantXObjects(this XObject source)
        {
            yield return source;
            if(source is XElement element)
                foreach(var attribute in element.Attributes().Where(a => !a.IsNamespaceDeclaration))
                    yield return attribute;
            if(!(source is XContainer container)) yield break;
            foreach(var s in container.Nodes().SelectMany(child => child.DescendantXObjects()))
                yield return s;
        }

        public static int Int32OrDefault([CanBeNull] this XAttribute attribute, int @default = 0)
        {
            if (@attribute == null) return @default;
            var str = attribute.Value;
            if (string.IsNullOrEmpty(str)) return @default;
            return !int.TryParse(str, out var value) ? @default : value;
        }

        [CanBeNull]
        public static string StringOrDefault([CanBeNull] this XAttribute attribute, [CanBeNull] string @default = null)
        {
            if (@attribute == null) return @default;
            var str = attribute?.Value;
            return string.IsNullOrEmpty(str) ? @default : str;
        }
    }
}
