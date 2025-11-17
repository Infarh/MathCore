#nullable enable
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml;

[PublicAPI]
public static class XmlWriterExtensions
{
    /// <summary>Асинхронно записывает элемент с указанным именем и значением</summary>
    /// <param name="writer">XML-писатель</param>
    /// <param name="name">Имя элемента</param>
    /// <param name="value">Значение элемента</param>
    /// <returns>Задача, представляющая асинхронную операцию записи</returns>
    public static Task WriteElementStringAsync(this XmlWriter writer, string name, string value) => writer.WriteElementStringAsync(null, name, null, value);

    /// <summary>Асинхронно записывает начальный элемент с указанным именем</summary>
    /// <param name="writer">XML-писатель</param>
    /// <param name="name">Имя элемента</param>
    /// <returns>Задача, представляющая асинхронную операцию записи</returns>
    public static Task WriteStartElementAsync(this XmlWriter writer, string name) => writer.WriteStartElementAsync(null, name, null);

    /// <summary>Асинхронно записывает атрибут с указанным именем и значением</summary>
    /// <param name="writer">XML-писатель</param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="value">Значение атрибута</param>
    /// <returns>Задача, представляющая асинхронную операцию записи</returns>
    public static Task WriteAttributeString(this XmlWriter writer, string name, string value) => writer.WriteAttributeStringAsync(null, name, null, value);

    /// <summary>Асинхронно записывает атрибут с указанным именем и значением</summary>
    /// <param name="writer">XML-писатель</param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="value">Значение атрибута</param>
    /// <returns>Задача, представляющая асинхронную операцию записи</returns>
    public static Task WriteAttributeStringAsync(this XmlWriter writer, string name, string value) => writer.WriteAttributeStringAsync(null, name, null, value);
}