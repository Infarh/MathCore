#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;

using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq;

[PublicAPI]
public static class XDocumentExtensions
{
    public static string ToXml(this XDocument document)
    {
        var result = new StringBuilder();
        using var writer = new StringWriter(result);
        document.Save(writer);
        return result.ToString();
    }

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

    private static XElement GetRoot(this XContainer xml) => xml switch
    {
        XElement element             => element,
        XDocument { Root: { } root } => root,
        _                            => throw new InvalidOperationException("В документе отсутствует корневой элемент")
    };

    public static XObject? XPath(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => element,
            XAttribute attribute => attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

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

    public static string? XPathString(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (string)element,
            XAttribute attribute => (string)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    public static int? XPathInt32(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (int)element,
            XAttribute attribute => (int)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    public static double? XPathDouble(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (double)element,
            XAttribute attribute => (double)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    public static DateTime? XPathDateTime(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (DateTime)element,
            XAttribute attribute => (DateTime)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    public static bool? XPathBool(this XContainer xml, string path) => path is not { Length: > 0 }
        ? throw new ArgumentException("Не задан путь")
        : ((IEnumerable<object>)xml.GetRoot().XPathEvaluate(path)).FirstOrDefault() switch
        {
            null                 => null,
            XElement element     => (bool)element,
            XAttribute attribute => (bool)attribute,
            { } node             => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
        };

    public static void Save(this XElement element, FileInfo file) => element.Save(file.FullName);
}