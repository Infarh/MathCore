﻿#nullable enable

using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Xml;

[PublicAPI]
public static class XmlReaderExtensions
{
    public static XmlReader EnsureElement(this XmlReader reader, bool SkipCurrent = false)
    {
        if (SkipCurrent && reader.NodeType == XmlNodeType.Element)
            reader.Read();

        while (reader.NodeType != XmlNodeType.Element)
            reader.Read();

        return reader;
    }

    public static IEnumerable<(string Name, string Value)> EnumerateAttributeValues(this XmlReader reader)
    {
        if (reader.NodeType != XmlNodeType.Element)
            throw new InvalidOperationException($"Процесс чтения находится не на элементе Xml. Тип текущего элемента {reader.NodeType}");

        if (reader.MoveToFirstAttribute())
            do
            {
                var name = reader.Name;
                var value = reader.Value;
                yield return (name, value);
            }
            while (reader.MoveToNextAttribute());
    }

    [DST]
    public static void Read(this XmlReader reader, Action<XmlReader>? read) => read?.Invoke(reader);

    public static bool TryGetAttribute(this XmlReader reader, string name, out string? value) => (value = reader.GetAttribute(name)) != null;

    public static double? GetAttributeDouble(this XmlReader reader, string name, double? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDouble(str);
    }

    public static int? GetAttributeInt(this XmlReader reader, string name, int? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToInt32(str);
    }

    public static uint? GetAttributeUInt(this XmlReader reader, string name, uint? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToUInt32(str);
    }

    public static bool? GetAttributeBool(this XmlReader reader, string name, bool? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToBoolean(str);
    }

    public static char? GetAttributeChar(this XmlReader reader, string name, char? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToChar(str);
    }

    public static decimal? GetAttributeDecimal(this XmlReader reader, string name, decimal? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDecimal(str);
    }

    public static DateTime? GetAttributeDateTime(this XmlReader reader, string name, string format, DateTime? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDateTime(str, format);
    }

    public static TimeSpan? GetAttributeTimeSpan(this XmlReader reader, string name, TimeSpan? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToTimeSpan(str);
    }

    public static async Task<T> ReadElementContentAsTypeAsync<T>(this XmlReader reader) => (T)await reader.ReadContentAsAsync(typeof(T), null).ConfigureAwait(false);

    public static Task<int> ReadElementContentAsIntAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<int>();

    public static Task<double> ReadElementContentAsDoubleAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<double>();

    public static Task<bool> ReadElementContentAsBooleanAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<bool>();
}