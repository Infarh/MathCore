#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Serialization;

// ReSharper disable once InconsistentNaming
public static class IXmlSerializableAsyncExtensions
{
    [DebuggerStepThrough]
    public static async Task<T> ReadXmlFromAsync<T>(this T obj, XmlReader reader)
        where T : IXmlSerializableAsync
    {
        await obj.ReadXmlAsync(reader).ConfigureAwait(false);
        return obj;
    }

    public static async Task<T> WriteXmlToAsync<T>(this T obj, XmlWriter writer)
        where T : IXmlSerializableAsync
    {
        await obj.WriteXmlAsync(writer).ConfigureAwait(false);
        return obj;
    }

    public static async Task<T> WriteXmlToAsync<T>(this T obj, XmlWriter writer, string ElementName)
        where T : IXmlSerializableAsync
    {
        await writer.WriteStartElementAsync(ElementName).ConfigureAwait(false);
        await obj.WriteXmlAsync(writer);
        await writer.WriteEndElementAsync();
        return obj;
    }

    public static async Task<T> WriteXmlToAsync<T>(this T enumeration, XmlWriter writer, string GroupName, string ElementName,
        Dictionary<string, object>? attributes = null)
        where T : IEnumerable<IXmlSerializableAsync>
    {
        await writer.WriteStartElementAsync(GroupName).ConfigureAwait(false);
        if(attributes != null)
            foreach(var (key, value) in attributes.Where(kv => kv.Value != null))
                await writer.WriteAttributeStringAsync(key, value.ToString());

        foreach(var obj in enumeration)
            await obj.WriteXmlToAsync(writer, ElementName);

        await writer.WriteEndElementAsync();
        return enumeration;
    }

    public static async Task<T> WriteXmlGroupToAsync<T>(this T enumeration, XmlWriter writer, string ElementName)
        where T : IEnumerable<IXmlSerializableAsync>
    {
        foreach(var obj in enumeration)
            await obj.WriteXmlToAsync(writer, ElementName).ConfigureAwait(false);

        return enumeration;
    }
}

// ReSharper disable once InconsistentNaming
public static class IXmlSerializableExtensions
{
    [DebuggerStepThrough]
    public static T ReadXmlFrom<T>(this T obj, XmlReader reader)
        where T : IXmlSerializable
    {
        obj.ReadXml(reader);
        return obj;
    }

    public static T WriteXmlTo<T>(this T obj, XmlWriter writer)
        where T : IXmlSerializable
    {
        obj.WriteXml(writer);
        return obj;
    }

    public static T WriteXmlTo<T>(this T obj, XmlWriter writer, string ElementName)
        where T : IXmlSerializable
    {
        writer.WriteStartElement(ElementName);
        obj.WriteXml(writer);
        writer.WriteEndElement();
        return obj;
    }

    public static T WriteXmlTo<T>(this T enumeration, XmlWriter writer, string GroupName, string ElementName,
        Dictionary<string, object>? attributes = null)
        where T : IEnumerable<IXmlSerializable>
    {
        writer.WriteStartElement(GroupName);

        if(attributes != null)
            foreach(var (key, value) in attributes.Where(kv => kv.Value != null))
                writer.WriteAttributeString(key, value.ToString());

        foreach(var obj in enumeration)
            obj.WriteXmlTo(writer, ElementName);

        writer.WriteEndElement();
        return enumeration;
    }

    public static T WriteXmlGroupTo<T>(this T enumeration, XmlWriter writer, string ElementName)
        where T : IEnumerable<IXmlSerializable>
    {
        foreach(var obj in enumeration)
            obj.WriteXmlTo(writer, ElementName);
        return enumeration;
    }
}

public static class XmlReaderExtensions
{
    public static void ReadElementContentTo(this XmlReader reader, out string str) => str = reader.ReadContentAsString();
    public static void ReadElementContentTo(this XmlReader reader, out int i) => i = reader.ReadContentAsInt();
    public static void ReadElementContentTo(this XmlReader reader, out double d) => d = reader.ReadContentAsDouble();
    public static void ReadElementContentTo(this XmlReader reader, out bool b) => b = reader.ReadContentAsBoolean();
}