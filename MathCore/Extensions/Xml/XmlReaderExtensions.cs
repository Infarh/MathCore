using System.Threading.Tasks;

using MathCore.Annotations;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Xml
{
    public static class XmlReaderExtensions
    {
        [DST]
        public static void Read([NotNull] this XmlReader reader, [CanBeNull] Action<XmlReader> read) => read?.Invoke(reader);

        public static bool TryGetAttribute([NotNull] this XmlReader reader, string name, out string value) => (value = reader.GetAttribute(name)) != null;

        public static double? GetAttributeDouble([NotNull] this XmlReader reader, string name, double? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToDouble(str);
        }

        public static int? GetAttributeInt([NotNull] this XmlReader reader, string name, int? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToInt32(str);
        }

        public static uint? GetAttributeUInt([NotNull] this XmlReader reader, string name, uint? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToUInt32(str);
        }

        public static bool? GetAttributeBool([NotNull] this XmlReader reader, string name, bool? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToBoolean(str);
        }

        public static char? GetAttributeChar([NotNull] this XmlReader reader, string name, char? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToChar(str);
        }

        public static decimal? GetAttributeDecimal([NotNull] this XmlReader reader, string name, decimal? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToDecimal(str);
        }

        public static DateTime? GetAttributeDateTime([NotNull] this XmlReader reader, string name, string format, DateTime? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToDateTime(str, format);
        }

        public static TimeSpan? GetAttributeTimeSpan([NotNull] this XmlReader reader, string name, TimeSpan? Default = null)
        {
            var str = reader.GetAttribute(name);
            return str is null ? Default : XmlConvert.ToTimeSpan(str);
        }


        public static async Task<T> ReadElementContentAsTypeAsync<T>([NotNull] this XmlReader reader) => (T)await reader.ReadContentAsAsync(typeof(T), null).ConfigureAwait(false);
        public static Task<int> ReadElementContentAsIntAsync([NotNull] this XmlReader reader) => reader.ReadElementContentAsTypeAsync<int>();
        public static Task<double> ReadElementContentAsDoubleAsync([NotNull] this XmlReader reader) => reader.ReadElementContentAsTypeAsync<double>();
        public static Task<bool> ReadElementContentAsBooleanAsync([NotNull] this XmlReader reader) => reader.ReadElementContentAsTypeAsync<bool>();

    }
}