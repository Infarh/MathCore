#nullable enable
using System.Text;
using System.Xml.XPath;

using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq;

[PublicAPI]
public static class XDocumentExtensions
{
    /// <summary>Преобразует XDocument в строку XML</summary>
    /// <param name="document">Документ XML</param>
    /// <returns>Строка, представляющая XML-документ</returns>
    public static string ToXml(this XDocument document)
    {
        var result = new StringBuilder();
        using var writer = new StringWriter(result);
        document.Save(writer);
        return result.ToString();
    }

    /// <summary>Сохраняет XDocument в файл</summary>
    /// <param name="xml">Документ XML</param>
    /// <param name="file">Файл для сохранения</param>
    public static void Save(this XDocument xml, FileInfo file) => xml.Save(file.FullName);

    //public static XObject? XPath(this XContainer xml, string path) =>
    //    path is not { Length: > 0 }
    //        ? throw new ArgumentException("Не задан путь")
    //        : xml is not XDocument { Root: { } root }
    //            ? throw new InvalidOperationException("В документе отсутствует корневой элемент")
    //            : ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault() switch
    //            {
    //                null                 => null,
    //                XElement element     => element,
    //                XAttribute attribute => attribute,
    //                { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
    //            };

    /// <summary>Получает корневой элемент из контейнера XML</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <returns>Корневой элемент контейнера</returns>
    private static XElement GetRoot(this XContainer xml) => xml switch
    {
        XElement element             => element,
        XDocument { Root: { } root } => root,
        _                            => throw new InvalidOperationException("В документе отсутствует корневой элемент")
    };

    /// <summary>Выполняет XPath-запрос и возвращает первый найденный объект</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Найденный объект или null</returns>
    public static XObject? XPath(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => element,
            XAttribute attribute => attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Устанавливает значение для элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <param name="Value">Новое значение</param>
    public static void XPathSetValue(this XContainer xml, string path, object Value)
    {
        if (path is not { Length: > 0 }) throw new ArgumentException("Не задан путь");
        switch (((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault())
        {
            default: throw new InvalidOperationException($"Ошибка формата при вычислении пути в файле {path}");
            case XElement element: element.SetValue(Value); break;
            case XAttribute attribute: attribute.SetValue(Value); break;
        }
    }

    /// <summary>Возвращает строковое значение элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Строковое значение или null</returns>
    public static string? XPathString(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (string)element,
            XAttribute attribute => (string)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Возвращает целочисленное значение элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Целочисленное значение или null</returns>
    public static int? XPathInt32(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (int)element,
            XAttribute attribute => (int)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Возвращает значение с плавающей точкой элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Значение с плавающей точкой или null</returns>
    public static double? XPathDouble(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (double)element,
            XAttribute attribute => (double)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Возвращает значение даты и времени элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Значение даты и времени или null</returns>
    public static DateTime? XPathDateTime(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (DateTime)element,
            XAttribute attribute => (DateTime)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Возвращает логическое значение элемента или атрибута, найденного по XPath-запросу</summary>
    /// <param name="xml">Контейнер XML</param>
    /// <param name="path">XPath-запрос</param>
    /// <returns>Логическое значение или null</returns>
    public static bool? XPathBool(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (bool)element,
            XAttribute attribute => (bool)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    /// <summary>Сохраняет XElement в файл</summary>
    /// <param name="element">Элемент XML</param>
    /// <param name="file">Файл для сохранения</param>
    public static void Save(this XElement element, FileInfo file) => element.Save(file.FullName);
}