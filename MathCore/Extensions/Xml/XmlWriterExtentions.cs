using System.Threading.Tasks;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml
{
    [PublicAPI]
    public static class XmlWriterExtentions
    {
        public static Task WriteElementStringAsync([NotNull] this XmlWriter writer, [NotNull] string name, string value) => writer.WriteElementStringAsync(null, name, null, value);

        public static Task WriteStartElementAsync([NotNull] this XmlWriter writer, [NotNull] string name) => writer.WriteStartElementAsync(null, name, null);

        public static Task WriteAttributeString([NotNull] this XmlWriter writer, [NotNull] string name, [NotNull] string value) => writer.WriteAttributeStringAsync(null, name, null, value);
    }
}