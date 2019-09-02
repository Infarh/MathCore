using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Serialization
{
    /// <summary>Класс методов-расширений для XML-сериализаторов</summary>
    public static class XmlSerializerExtentions
    {
        /// <summary>Словарь типов - сериализаторов</summary>
        [NotNull] private static readonly ConcurrentDictionary<Type, XmlSerializer> __XmlSerializersPool = new ConcurrentDictionary<Type, XmlSerializer>();

        /// <summary>Получить XML-сериализатор по указанному типу</summary>
        /// <param name="type">Тип XML-сериализатора</param>
        /// <returns>XML-сериализатор</returns>
        [DebuggerStepThrough, NotNull] 
        public static XmlSerializer GetXmlSerializer([NotNull] this Type type) => __XmlSerializersPool.GetOrAdd(type, t => new XmlSerializer(t));

        /// <summary>Получить XML-сериализатор по указанному типу</summary>
        /// <param name="type">Тип XML-сериализатора</param>
        /// <returns>XML-сериализатор</returns>
        [DebuggerStepThrough, NotNull] 
        public static XmlSerializer GetXmlSerializer<T>() => __XmlSerializersPool.GetOrAdd(typeof(T), t => new XmlSerializer(t));

        [DebuggerStepThrough]
        public static bool CanDeserialize([NotNull] this XmlSerializer serializer, [NotNull] string str) => serializer.CanDeserialize(XmlReader.Create(str));

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this XmlSerializer serializer, [NotNull] string str) => (T)serializer.Deserialize(XmlReader.Create(str));

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this XmlSerializer serializer, [NotNull] Stream data) => (T)serializer.Deserialize(data);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] Stream data) => type.GetXmlSerializer().Deserialize<T>(data);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] Stream data, bool Initialize)
            where T : IInitializable
        {
            var obj = type.GetXmlSerializer().Deserialize<T>(data);
            if(Initialize) (obj as IInitializable)?.Initialize();
            return obj;
        }

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this XmlSerializer serializer, [NotNull] XmlReader reader) => (T)serializer.Deserialize(reader);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] XmlReader reader) => type.GetXmlSerializer().Deserialize<T>(reader);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] XmlReader reader, bool Initialize)
            where T : IInitializable
        {
            var obj = type.GetXmlSerializer().Deserialize<T>(reader);
            if(Initialize) (obj as IInitializable)?.Initialize();
            return obj;
        }

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this XmlSerializer serializer, TextReader reader) => (T)serializer.Deserialize(reader);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, TextReader reader) => type.GetXmlSerializer().Deserialize<T>(reader);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, TextReader reader, bool Initialize)
            where T : IInitializable
        {
            var obj = type.GetXmlSerializer().Deserialize<T>(reader);
            if(Initialize) (obj as IInitializable)?.Initialize();
            return obj;
        }

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] FileInfo file) => 
            file.OpenText().DisposeAfter(type, (reader, t) => t.Deserialize<T>(reader)) ;

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] FileInfo file, bool Initialize)
            where T : IInitializable => 
            file.OpenRead().DisposeAfter(Initialize, type, (reader, init, t) => t.Deserialize<T>(reader, init));

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] string file) => type.Deserialize<T>(new FileInfo(file));

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Type type, [NotNull] string file, bool Initialize) where T : IInitializable => type.Deserialize<T>(new FileInfo(file), Initialize);

        [DebuggerStepThrough]
        public static T Deserialize<T>([NotNull] this Stream stream) => (T)GetXmlSerializer<T>().Deserialize(stream);

        public static T Deserialize<T>([NotNull] this FileInfo file) => file.OpenRead().DisposeAfter(reader => reader.Deserialize<T>());
    }
}
