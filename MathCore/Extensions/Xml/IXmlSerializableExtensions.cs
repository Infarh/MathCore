using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml.Serialization
{
    public static class IXmlSerializableAsyncExtensions
    {
        [DebuggerStepThrough]
        [ItemNotNull]
        public static async Task<T> ReadXmlFromAsync<T>([NotNull] this T obj, XmlReader reader)
            where T : IXmlSerializableAsync
        {
            await obj.ReadXmlAsync(reader).ConfigureAwait(false);
            return obj;
        }

        [ItemNotNull]
        public static async Task<T> WriteXmlToAsync<T>([NotNull] this T obj, XmlWriter writer)
            where T : IXmlSerializableAsync
        {
            await obj.WriteXmlAsync(writer).ConfigureAwait(false);
            return obj;
        }

        [ItemNotNull]
        public static async Task<T> WriteXmlToAsync<T>([NotNull] this T obj, [NotNull] XmlWriter writer, [NotNull] string ElementName)
            where T : IXmlSerializableAsync
        {
            writer.WriteStartElement(ElementName);
            await obj.WriteXmlAsync(writer).ConfigureAwait(false);
            writer.WriteEndElement();
            return obj;
        }

        [ItemNotNull]
        public static async Task<T> WriteXmlToAsync<T>([NotNull] this T enumeration, [NotNull] XmlWriter writer, [NotNull] string GroupName, string ElementName,
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

        [ItemNotNull]
        public static async Task<T> WriteXmlGroupToAsync<T>([NotNull] this T enumeration, XmlWriter writer, string ElementName)
            where T : IEnumerable<IXmlSerializableAsync>
        {
            foreach(var obj in enumeration)
                await obj.WriteXmlToAsync(writer, ElementName).ConfigureAwait(false);
            return enumeration;
        }
    }

    public static class IXmlSerializableExtensions
    {
        [DebuggerStepThrough]
        [NotNull]
        public static T ReadXmlFrom<T>([NotNull] this T obj, [NotNull] XmlReader reader)
            where T : IXmlSerializable
        {
            obj.ReadXml(reader);
            return obj;
        }

        [NotNull]
        public static T WriteXmlTo<T>([NotNull] this T obj, [NotNull] XmlWriter writer)
            where T : IXmlSerializable
        {
            obj.WriteXml(writer);
            return obj;
        }

        [NotNull]
        public static T WriteXmlTo<T>([NotNull] this T obj, [NotNull] XmlWriter writer, [NotNull] string ElementName)
            where T : IXmlSerializable
        {
            writer.WriteStartElement(ElementName);
            obj.WriteXml(writer);
            writer.WriteEndElement();
            return obj;
        }

        [NotNull]
        public static T WriteXmlTo<T>([NotNull] this T enumeration, [NotNull] XmlWriter writer, [NotNull] string GroupName, string ElementName,
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

        [NotNull]
        public static T WriteXmlGroupTo<T>([NotNull] this T enumeration, XmlWriter writer, string ElementName)
            where T : IEnumerable<IXmlSerializable>
        {
            foreach(var obj in enumeration)
                obj.WriteXmlTo(writer, ElementName);
            return enumeration;
        }
    }

    public static class XmlReaderExtensions
    {
        public static void ReadElementContentTo([NotNull] this XmlReader reader, out string str) => str = reader.ReadContentAsString();
        public static void ReadElementContentTo([NotNull] this XmlReader reader, out int i) => i = reader.ReadContentAsInt();
        public static void ReadElementContentTo([NotNull] this XmlReader reader, out double d) => d = reader.ReadContentAsDouble();
        public static void ReadElementContentTo([NotNull] this XmlReader reader, out bool b) => b = reader.ReadContentAsBoolean();
    }
}