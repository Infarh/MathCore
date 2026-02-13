
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global
// ReSharper disable once CheckNamespace
namespace System.Xml;

[PublicAPI]
public static class XmlReaderExtensions
{
    /// <summary>Убеждается, что текущий узел является элементом, при необходимости пропуская текущий узел</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="SkipCurrent">Флаг, указывающий, нужно ли пропустить текущий узел</param>
    /// <returns>Объект <see cref="XmlReader"/>, установленный на элемент</returns>
    public static XmlReader EnsureElement(this XmlReader reader, bool SkipCurrent = false)
    {
        if (SkipCurrent && reader.NodeType == XmlNodeType.Element)
            reader.Read();

        while (reader.NodeType != XmlNodeType.Element)
            reader.Read();

        return reader;
    }

    /// <summary>Перечисляет атрибуты текущего элемента и их значения</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <returns>Перечисление пар имя-значение атрибутов</returns>
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

    /// <summary>Выполняет действие чтения с использованием переданного делегата</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="read">Делегат действия чтения</param>
    [DST]
    public static void Read(this XmlReader reader, Action<XmlReader>? read) => read?.Invoke(reader);

    /// <summary>Пытается получить значение атрибута с указанным именем</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="value">Значение атрибута, если он найден</param>
    /// <returns>Истина, если атрибут найден, иначе ложь</returns>
    public static bool TryGetAttribute(this XmlReader reader, string name, out string? value) => (value = reader.GetAttribute(name)) != null;

    /// <summary>Получает значение атрибута как число с плавающей запятой</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="double"/></returns>
    public static double? GetAttributeDouble(this XmlReader reader, string name, double? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDouble(str);
    }

    /// <summary>Получает значение атрибута как целое число</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="int"/></returns>
    public static int? GetAttributeInt(this XmlReader reader, string name, int? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToInt32(str);
    }

    /// <summary>Получает значение атрибута как беззнаковое целое число</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="uint"/></returns>
    public static uint? GetAttributeUInt(this XmlReader reader, string name, uint? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToUInt32(str);
    }

    /// <summary>Получает значение атрибута как логическое значение</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="bool"/></returns>
    public static bool? GetAttributeBool(this XmlReader reader, string name, bool? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToBoolean(str);
    }

    /// <summary>Получает значение атрибута как символ</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="char"/></returns>
    public static char? GetAttributeChar(this XmlReader reader, string name, char? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToChar(str);
    }

    /// <summary>Получает значение атрибута как десятичное число</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="decimal"/></returns>
    public static decimal? GetAttributeDecimal(this XmlReader reader, string name, decimal? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDecimal(str);
    }

    /// <summary>Получает значение атрибута как дату и время</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="format">Формат даты и времени</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="DateTime"/></returns>
    public static DateTime? GetAttributeDateTime(this XmlReader reader, string name, string format, DateTime? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToDateTime(str, format);
    }

    /// <summary>Получает значение атрибута как временной интервал</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <param name="name">Имя атрибута</param>
    /// <param name="Default">Значение по умолчанию, если атрибут отсутствует</param>
    /// <returns>Значение атрибута как <see cref="TimeSpan"/></returns>
    public static TimeSpan? GetAttributeTimeSpan(this XmlReader reader, string name, TimeSpan? Default = null)
    {
        var str = reader.GetAttribute(name);
        return str is null ? Default : XmlConvert.ToTimeSpan(str);
    }

    /// <summary>Асинхронно читает содержимое элемента как указанный тип</summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <returns>Значение содержимого элемента как указанный тип</returns>
    public static async Task<T> ReadElementContentAsTypeAsync<T>(this XmlReader reader) => (T)await reader.ReadContentAsAsync(typeof(T), null).ConfigureAwait(false);

    /// <summary>Асинхронно читает содержимое элемента как целое число</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <returns>Значение содержимого элемента как <see cref="int"/></returns>
    public static Task<int> ReadElementContentAsIntAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<int>();

    /// <summary>Асинхронно читает содержимое элемента как число с плавающей запятой</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <returns>Значение содержимого элемента как <see cref="double"/></returns>
    public static Task<double> ReadElementContentAsDoubleAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<double>();

    /// <summary>Асинхронно читает содержимое элемента как логическое значение</summary>
    /// <param name="reader">Читающий объект <see cref="XmlReader"/></param>
    /// <returns>Значение содержимого элемента как <see cref="bool"/></returns>
    public static Task<bool> ReadElementContentAsBooleanAsync(this XmlReader reader) => reader.ReadElementContentAsTypeAsync<bool>();
}