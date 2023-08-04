#nullable enable
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml;

[PublicAPI]
public static class XmlWriterExtensions
{
    public static Task WriteElementStringAsync(this XmlWriter writer, string name, string value) => writer.WriteElementStringAsync(null, name, null, value);

    public static Task WriteStartElementAsync(this XmlWriter writer, string name) => writer.WriteStartElementAsync(null, name, null);

    public static Task WriteAttributeString(this XmlWriter writer, string name, string value) => writer.WriteAttributeStringAsync(null, name, null, value);

    public static Task WriteAttributeStringAsync(this XmlWriter writer, string name, string value) => writer.WriteAttributeStringAsync(null, name, null, value);
}