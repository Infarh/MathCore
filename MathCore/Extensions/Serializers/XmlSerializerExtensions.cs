using System.Collections.Concurrent;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Xml.Serialization;

/// <summary>Класс методов-расширений для XML-сериализаторов</summary>
public static class XmlSerializerExtensions
{
    /// <summary>Словарь типов - сериализаторов</summary>
    private static readonly ConcurrentDictionary<Type, XmlSerializer> __XmlSerializersPool = new();

    /// <summary>Получить XML-сериализатор по указанному типу</summary>
    /// <param name="type">Тип, для которого требуется получить сериализатор</param>
    /// <returns>Экземпляр XML-сериализатора для указанного типа</returns>
    [DebuggerStepThrough] 
    public static XmlSerializer GetXmlSerializer(this Type type) => __XmlSerializersPool.GetOrAdd(type, t => new(t));

    /// <summary>Получить XML-сериализатор по указанному типу</summary>
    /// <typeparam name="T">Тип, для которого требуется получить сериализатор</typeparam>
    /// <returns>Экземпляр XML-сериализатора для типа T</returns>
    [DebuggerStepThrough] 
    public static XmlSerializer GetXmlSerializer<T>() => __XmlSerializersPool.GetOrAdd(typeof(T), t => new(t));

    /// <summary>Проверить возможность десериализации строки</summary>
    /// <param name="serializer">Сериализатор, используемый для проверки</param>
    /// <param name="str">Строка, содержащая XML-данные</param>
    /// <returns>Истина, если десериализация возможна</returns>
    [DebuggerStepThrough]
    public static bool CanDeserialize(this XmlSerializer serializer, string str) => serializer.CanDeserialize(XmlReader.Create(str));

    /// <summary>Десериализовать объект из строки</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="serializer">Сериализатор, используемый для десериализации</param>
    /// <param name="str">Строка, содержащая XML-данные</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this XmlSerializer serializer, string str) => (T)serializer.Deserialize(XmlReader.Create(str));

    /// <summary>Десериализовать объект из потока</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="serializer">Сериализатор, используемый для десериализации</param>
    /// <param name="data">Поток с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this XmlSerializer serializer, Stream data) => (T)serializer.Deserialize(data);

    /// <summary>Десериализовать объект из потока по типу</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="data">Поток с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, Stream data) => type.GetXmlSerializer().Deserialize<T>(data);

    /// <summary>Десериализовать и инициализировать объект из потока</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="data">Поток с XML-данными</param>
    /// <param name="Initialize">Флаг необходимости инициализации объекта</param>
    /// <returns>Десериализованный и инициализированный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, Stream data, bool Initialize)
        where T : IInitializable
    {
        var obj = type.GetXmlSerializer().Deserialize<T>(data);
        if(Initialize) obj.Initialize();
        return obj;
    }

    /// <summary>Десериализовать объект из XmlReader</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="serializer">Сериализатор, используемый для десериализации</param>
    /// <param name="reader">XmlReader с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this XmlSerializer serializer, XmlReader reader) => (T)serializer.Deserialize(reader);

    /// <summary>Десериализовать объект из XmlReader по типу</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="reader">XmlReader с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, XmlReader reader) => type.GetXmlSerializer().Deserialize<T>(reader);

    /// <summary>Десериализовать и инициализировать объект из XmlReader</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="reader">XmlReader с XML-данными</param>
    /// <param name="Initialize">Флаг необходимости инициализации объекта</param>
    /// <returns>Десериализованный и инициализированный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, XmlReader reader, bool Initialize)
        where T : IInitializable
    {
        var obj = type.GetXmlSerializer().Deserialize<T>(reader);
        if(Initialize) obj.Initialize();
        return obj;
    }

    /// <summary>Десериализовать объект из TextReader</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="serializer">Сериализатор, используемый для десериализации</param>
    /// <param name="reader">TextReader с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this XmlSerializer serializer, TextReader reader) => (T)serializer.Deserialize(reader);

    /// <summary>Десериализовать объект из TextReader по типу</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="reader">TextReader с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, TextReader reader) => type.GetXmlSerializer().Deserialize<T>(reader);

    /// <summary>Десериализовать и инициализировать объект из TextReader</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="reader">TextReader с XML-данными</param>
    /// <param name="Initialize">Флаг необходимости инициализации объекта</param>
    /// <returns>Десериализованный и инициализированный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, TextReader reader, bool Initialize)
        where T : IInitializable
    {
        var obj = type.GetXmlSerializer().Deserialize<T>(reader);
        if(Initialize) obj.Initialize();
        return obj;
    }

    /// <summary>Десериализовать объект из файла</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="file">Файл с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, FileInfo file) => 
        file.OpenText().DisposeAfter(type, (reader, t) => t.Deserialize<T>(reader)) ;

    /// <summary>Десериализовать и инициализировать объект из файла</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="file">Файл с XML-данными</param>
    /// <param name="Initialize">Флаг необходимости инициализации объекта</param>
    /// <returns>Десериализованный и инициализированный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, FileInfo file, bool Initialize)
        where T : IInitializable => 
        file.OpenRead().DisposeAfter(Initialize, type, (reader, init, t) => t.Deserialize<T>(reader, init));

    /// <summary>Десериализовать объект из файла по имени</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="file">Имя файла с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, string file) => type.Deserialize<T>(new FileInfo(file));

    /// <summary>Десериализовать и инициализировать объект из файла по имени</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="type">Тип, для которого выполняется десериализация</param>
    /// <param name="file">Имя файла с XML-данными</param>
    /// <param name="Initialize">Флаг необходимости инициализации объекта</param>
    /// <returns>Десериализованный и инициализированный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Type type, string file, bool Initialize) where T : IInitializable => type.Deserialize<T>(new FileInfo(file), Initialize);

    /// <summary>Десериализовать объект из потока</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="stream">Поток с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    [DebuggerStepThrough]
    public static T Deserialize<T>(this Stream stream) => (T)GetXmlSerializer<T>().Deserialize(stream);

    /// <summary>Десериализовать объект из файла</summary>
    /// <typeparam name="T">Тип десериализуемого объекта</typeparam>
    /// <param name="file">Файл с XML-данными</param>
    /// <returns>Десериализованный объект типа T</returns>
    public static T Deserialize<T>(this FileInfo file) => file.OpenRead().DisposeAfter(reader => reader.Deserialize<T>());
}