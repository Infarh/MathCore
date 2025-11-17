#nullable enable
using System.Diagnostics;

// ReSharper disable UnusedMethodReturnValue.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Serialization;

// ReSharper disable once InconsistentNaming
public static class IXmlSerializableAsyncExtensions
{
    /// <summary>Читает XML-данные из указанного XmlReader</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializableAsync</typeparam>
    /// <param name="obj">Объект, в который будут загружены данные</param>
    /// <param name="reader">Источник данных XML</param>
    /// <returns>Объект с загруженными данными</returns>
    [DST]
    public static async Task<T> ReadXmlFromAsync<T>(this T obj, XmlReader reader)
        where T : IXmlSerializableAsync
    {
        await obj.ReadXmlAsync(reader).ConfigureAwait(false);
        return obj;
    }

    /// <summary>Записывает XML-данные в указанный XmlWriter</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializableAsync</typeparam>
    /// <param name="obj">Объект, данные которого будут записаны</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <returns>Объект, данные которого были записаны</returns>
    public static async Task<T> WriteXmlToAsync<T>(this T obj, XmlWriter writer)
        where T : IXmlSerializableAsync
    {
        await obj.WriteXmlAsync(writer).ConfigureAwait(false);
        return obj;
    }

    /// <summary>Записывает XML-данные в указанный XmlWriter с заданным именем элемента</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializableAsync</typeparam>
    /// <param name="obj">Объект, данные которого будут записаны</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <returns>Объект, данные которого были записаны</returns>
    public static async Task<T> WriteXmlToAsync<T>(this T obj, XmlWriter writer, string ElementName)
        where T : IXmlSerializableAsync
    {
        await writer.WriteStartElementAsync(ElementName).ConfigureAwait(false);
        await obj.WriteXmlAsync(writer).ConfigureAwait(false);
        await writer.WriteEndElementAsync().ConfigureAwait(false);
        return obj;
    }

    /// <summary>Записывает коллекцию объектов в XML с заданными именами группы и элементов</summary>
    /// <typeparam name="T">Тип коллекции объектов, реализующих IXmlSerializableAsync</typeparam>
    /// <param name="enumeration">Коллекция объектов</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="GroupName">Имя группы XML</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <param name="attributes">Атрибуты для группы XML</param>
    /// <returns>Коллекция объектов, данные которых были записаны</returns>
    public static async Task<T> WriteXmlToAsync<T>(this T enumeration, XmlWriter writer, string GroupName, string ElementName,
        Dictionary<string, object>? attributes = null)
        where T : IEnumerable<IXmlSerializableAsync>
    {
        await writer.WriteStartElementAsync(GroupName).ConfigureAwait(false);
        if (attributes != null)
            foreach (var (key, value) in attributes.Where(kv => kv.Value != null!))
                await writer.WriteAttributeStringAsync(key, value.ToString()).ConfigureAwait(false);

        foreach (var obj in enumeration)
            await obj.WriteXmlToAsync(writer, ElementName).ConfigureAwait(false);

        await writer.WriteEndElementAsync().ConfigureAwait(false);
        return enumeration;
    }

    /// <summary>Записывает коллекцию объектов в XML с заданным именем элемента</summary>
    /// <typeparam name="T">Тип коллекции объектов, реализующих IXmlSerializableAsync</typeparam>
    /// <param name="enumeration">Коллекция объектов</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <returns>Коллекция объектов, данные которых были записаны</returns>
    public static async Task<T> WriteXmlGroupToAsync<T>(this T enumeration, XmlWriter writer, string ElementName)
        where T : IEnumerable<IXmlSerializableAsync>
    {
        foreach (var obj in enumeration)
            await obj.WriteXmlToAsync(writer, ElementName).ConfigureAwait(false);

        return enumeration;
    }
}

/// <summary>Расширения для работы с IXmlSerializable</summary>
public static class IXmlSerializableExtensions
{
    /// <summary>Читает XML-данные из указанного XmlReader</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializable</typeparam>
    /// <param name="obj">Объект, в который будут загружены данные</param>
    /// <param name="reader">Источник данных XML</param>
    /// <returns>Объект с загруженными данными</returns>
    [DebuggerStepThrough]
    public static T ReadXmlFrom<T>(this T obj, XmlReader reader)
        where T : IXmlSerializable
    {
        obj.ReadXml(reader);
        return obj;
    }

    /// <summary>Записывает XML-данные в указанный XmlWriter</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializable</typeparam>
    /// <param name="obj">Объект, данные которого будут записаны</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <returns>Объект, данные которого были записаны</returns>
    public static T WriteXmlTo<T>(this T obj, XmlWriter writer)
        where T : IXmlSerializable
    {
        obj.WriteXml(writer);
        return obj;
    }

    /// <summary>Записывает XML-данные в указанный XmlWriter с заданным именем элемента</summary>
    /// <typeparam name="T">Тип объекта, реализующего IXmlSerializable</typeparam>
    /// <param name="obj">Объект, данные которого будут записаны</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <returns>Объект, данные которого были записаны</returns>
    public static T WriteXmlTo<T>(this T obj, XmlWriter writer, string ElementName)
        where T : IXmlSerializable
    {
        writer.WriteStartElement(ElementName);
        obj.WriteXml(writer);
        writer.WriteEndElement();
        return obj;
    }

    /// <summary>Записывает коллекцию объектов в XML с заданными именами группы и элементов</summary>
    /// <typeparam name="T">Тип коллекции объектов, реализующих IXmlSerializable</typeparam>
    /// <param name="enumeration">Коллекция объектов</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="GroupName">Имя группы XML</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <param name="attributes">Атрибуты для группы XML</param>
    /// <returns>Коллекция объектов, данные которых были записаны</returns>
    public static T WriteXmlTo<T>(this T enumeration, XmlWriter writer, string GroupName, string ElementName,
        Dictionary<string, object>? attributes = null)
        where T : IEnumerable<IXmlSerializable>
    {
        writer.WriteStartElement(GroupName);

        if (attributes != null)
            foreach (var (key, value) in attributes.Where(kv => kv.Value != null!))
                writer.WriteAttributeString(key, value.ToString());

        foreach (var obj in enumeration)
            obj.WriteXmlTo(writer, ElementName);

        writer.WriteEndElement();
        return enumeration;
    }

    /// <summary>Записывает коллекцию объектов в XML с заданным именем элемента</summary>
    /// <typeparam name="T">Тип коллекции объектов, реализующих IXmlSerializable</typeparam>
    /// <param name="enumeration">Коллекция объектов</param>
    /// <param name="writer">Целевой XmlWriter</param>
    /// <param name="ElementName">Имя элемента XML</param>
    /// <returns>Коллекция объектов, данные которых были записаны</returns>
    public static T WriteXmlGroupTo<T>(this T enumeration, XmlWriter writer, string ElementName)
        where T : IEnumerable<IXmlSerializable>
    {
        foreach (var obj in enumeration)
            obj.WriteXmlTo(writer, ElementName);
        return enumeration;
    }
}

/// <summary>Расширения для XmlReader</summary>
public static class XmlReaderExtensions
{
    /// <summary>Читает содержимое элемента как строку</summary>
    /// <param name="reader">Источник данных XML</param>
    /// <param name="str">Переменная для записи результата</param>
    public static void ReadElementContentTo(this XmlReader reader, out string str) => str = reader.ReadContentAsString();

    /// <summary>Читает содержимое элемента как целое число</summary>
    /// <param name="reader">Источник данных XML</param>
    /// <param name="i">Переменная для записи результата</param>
    public static void ReadElementContentTo(this XmlReader reader, out int i) => i = reader.ReadContentAsInt();

    /// <summary>Читает содержимое элемента как число с плавающей точкой</summary>
    /// <param name="reader">Источник данных XML</param>
    /// <param name="d">Переменная для записи результата</param>
    public static void ReadElementContentTo(this XmlReader reader, out double d) => d = reader.ReadContentAsDouble();

    /// <summary>Читает содержимое элемента как логическое значение</summary>
    /// <param name="reader">Источник данных XML</param>
    /// <param name="b">Переменная для записи результата</param>
    public static void ReadElementContentTo(this XmlReader reader, out bool b) => b = reader.ReadContentAsBoolean();
}