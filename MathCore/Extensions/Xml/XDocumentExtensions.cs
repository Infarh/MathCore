using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Xml.Linq
{
    public static class XDocumentExtensions
    {
        [NotNull]
        public static string ToXml([NotNull] this XDocument document)
        {
            var result = new StringBuilder();
            using var writer = new StringWriter(result);
            document.Save(writer);
            return result.ToString();
        }

        public static void Save([NotNull] this XDocument xml, [NotNull] FileInfo file) => xml.Save(file.FullName);

        [CanBeNull]
        public static XObject XPath([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? null
                : node switch
                {
                    XElement element => (XObject) element,
                    XAttribute attribute => attribute,
                    _ => throw new InvalidOperationException(
                        $"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        [CanBeNull]
        public static void XPathSetValue([NotNull] this XContainer xml, [NotNull] string path, [NotNull] object Value)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");
            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            switch (node)
            {
                default: throw new InvalidOperationException($"Ошибка формата при вычислении пути в файле {path}");
                case XElement element: element.SetValue(Value); break;
                case XAttribute attribute: attribute.SetValue(Value); break;
            }
        }

        public static string XPathString([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? null
                : node switch
                {
                    XElement element => (string) element,
                    XAttribute attribute => (string) attribute,
                    _ => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        public static int? XPathInt32([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? (int?) null
                : node switch
                {
                    XElement element => (int) element,
                    XAttribute attribute => (int) attribute,
                    _ => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        public static double? XPathDouble([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? (double?) null
                : node switch
                {
                    XElement element => (double) element,
                    XAttribute attribute => (double) attribute,
                    _ => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        public static DateTime? XPathDateTime([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? (DateTime?) null
                : node switch
                {
                    XElement element => (DateTime) element,
                    XAttribute attribute => (DateTime) attribute,
                    _ => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        public static bool? XPathBool([NotNull] this XContainer xml, [NotNull] string path)
        {
            if (xml is null) throw new ArgumentNullException(nameof(xml));
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Не задан путь");

            var root = (xml as XDocument)?.Root ?? xml ?? throw new InvalidOperationException("В документе отсутствует корневой элемент");

            var node = ((IEnumerable<object>)root.XPathEvaluate(path)).FirstOrDefault();
            return node is null
                ? (bool?) null
                : node switch
                {
                    XElement element => (bool) element,
                    XAttribute attribute => (bool) attribute,
                    _ => throw new InvalidOperationException($"Непредвиденный тип {node.GetType()} элемента в результате вычисления {path}")
                };
        }

        public static void Save([NotNull] this XElement element, [NotNull] FileInfo file) => element.Save(file.FullName);
    }
}