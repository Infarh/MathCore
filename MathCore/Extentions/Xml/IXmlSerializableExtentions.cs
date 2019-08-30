using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathCore.Annotations;

namespace System.Xml.Serialization
{
    public static class IXmlSerializableAsyncExtentions
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
            writer.WriteStartElement(ElementName);
            await obj.WriteXmlAsync(writer).ConfigureAwait(false);
            writer.WriteEndElement();
            return obj;
        }

        public static async Task<T> WriteXmlToAsync<T>(this T enumeration, XmlWriter writer, string GroupName, string ElementName,
            [CanBeNull] Dictionary<string, object> attributes = null)
            where T : IEnumerable<IXmlSerializableAsync>
        {
            writer.WriteStartElement(GroupName);
            if(attributes != null)
                foreach(var (key, value) in attributes.Where(kv => kv.Value != null))
                    writer.WriteAttributeString(key, value.ToString());
            foreach(var obj in enumeration)
                await obj.WriteXmlToAsync(writer, ElementName).ConfigureAwait(false);
            writer.WriteEndElement();
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

    public static class IXmlSerializableExtentions
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
            [CanBeNull] Dictionary<string, object> attributes = null)
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

    public static class XmlReaderExtentions
    {
        public static void ReadElementContentTo(this XmlReader reader, out string str) => str = reader.ReadContentAsString();
        public static void ReadElementContentTo(this XmlReader reader, out int i) => i = reader.ReadContentAsInt();
        public static void ReadElementContentTo(this XmlReader reader, out double d) => d = reader.ReadContentAsDouble();
        public static void ReadElementContentTo(this XmlReader reader, out bool b) => b = reader.ReadContentAsBoolean();
    }
}