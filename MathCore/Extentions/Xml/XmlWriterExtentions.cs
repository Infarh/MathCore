using System.Threading.Tasks;

namespace System.Xml
{
    public static class XmlWriterExtentions
    {
        public static Task WriteElementStringAsync(this XmlWriter writer, string name, string value) => writer.WriteElementStringAsync(null, name, null, value);
    }
}