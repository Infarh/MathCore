#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.Xml;

/// <summary>Настраиваемый сериализатор объектов в XML</summary>
public abstract class LambdaXmlSerializer
{
    /// <summary>Создать новый сериализатор объектов</summary>
    /// <typeparam name="T">Тип сериализуемого класса</typeparam>
    /// <param name="RootName">Имя корневого элемента (если не указано, то будет использовано имя класса)</param>
    /// <returns>Сериализатор</returns>
    public static LambdaXmlSerializer<T> Create<T>(string? RootName = null) => new(RootName ?? typeof(T).Name);
}

/// <summary>Настраиваемый сериализатор объектов в XML</summary>
/// <typeparam name="T">Тип сериализуемого класса</typeparam>
public class LambdaXmlSerializer<T> : LambdaXmlSerializer
{
    /// <summary>Название элемента по умолчанию</summary>
    // ReSharper disable once StaticMemberInGenericType
    private static string __EmptyName = "item";

    /// <summary>Название элемента по умолчанию</summary>
    public static string EmptyName { get => __EmptyName; set => __EmptyName = value; }

    /// <summary>Название элемента</summary>
    private readonly string? _ElementName;

    /// <summary>Список методов формирования атрибутов элемента</summary>
    private readonly List<Func<T, object>> _Attributes = new();

    /// <summary>Список методов формирования дочерних элементов</summary>
    private readonly List<Func<T, object>> _Elements = new();

    /// <summary>Инициализация нового настраиваемого сериализатора</summary>
    /// <param name="ElementName">Название корневого элемента</param>
    public LambdaXmlSerializer(string? ElementName = null) => _ElementName = ElementName;

    /// <summary>Выполнение процесса сериализации</summary>
    /// <param name="value">Сериализуемый объект</param>
    /// <returns>xml-представление сериализуемого объекта</returns>
    public XElement Serialize(T value) => new(_ElementName ?? __EmptyName, Content(value).ToArray());

    /// <summary>Выполнение процесса сериализации</summary>
    /// <param name="Name">Название корневого элемента</param>
    /// <param name="value">Сериализуемый объект</param>
    /// <returns>xml-представление сериализуемого объекта</returns>
    public XElement Serialize(string? Name, T value) => new(Name ?? _ElementName ?? __EmptyName, Content(value).ToArray());

    /// <summary>Формирование содержимого элемента</summary>
    /// <remarks>Выполнение списков методов вычисления значений атрибутов, затем - дочерних элементов</remarks>
    /// <param name="value">Сериализуемый объект</param>
    /// <returns>Перечисление атрибутов и дочерних элементов, вкладываемых в корневой элемент</returns>
    private IEnumerable<object> Content(T value) => _Attributes.Select(a => a(value)).Concat(_Elements.Select(e => e(value)));

    /// <summary>Добавление конфигурации атрибута</summary>
    /// <typeparam name="TValue">ТИп значения атрибута</typeparam>
    /// <param name="Name">Имя атрибута</param>
    /// <param name="Selector">Метод определения значения атрибута</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attribute<TValue>(string? Name, Func<T, TValue> Selector)
    {
        _Attributes.Add(v => new XAttribute(Name ?? __EmptyName, Selector(v)));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, string> NameSelector,
        Func<TItem, TValue> ValueSelector)
    {
        _Attributes.Add(items => Selector(items).ToArray(item => new XAttribute(NameSelector(item), ValueSelector(item))));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута</param>
    /// <param name="NeedToSerialize">Метод, определяющий - требуется ли выполнять сериализацию конкретного значения в атрибут?</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, string> NameSelector,
        Func<TItem, bool> NeedToSerialize,
        Func<TItem, TValue> ValueSelector)
    {
        _Attributes.Add(items => Selector(items).Where(NeedToSerialize).ToArray(item => new XAttribute(NameSelector(item), ValueSelector(item))));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута в том числе по порядковому номеру</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, int, string> NameSelector,
        Func<TItem, TValue> ValueSelector)
    {
        _Attributes.Add(items => Selector(items).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item))));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута в том числе по порядковому номеру</param>
    /// <param name="NeedToSerialize">Метод, определяющий - требуется ли выполнять сериализацию конкретного значения в атрибут?</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, int, string> NameSelector,
        Func<TItem, bool> NeedToSerialize,
        Func<TItem, TValue> ValueSelector)
    {
        _Attributes.Add(items => Selector(items).Where(NeedToSerialize).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item))));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута в том числе</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута по порядковому номеру</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, string> NameSelector,
        Func<TItem, int, TValue> ValueSelector)
    {
        _Attributes.Add(element => Selector(element).ToArray((item, i) => new XAttribute(NameSelector(item), ValueSelector(item, i))));
        return this;
    }

    /// <summary>Добавление конфигурации атрибутов</summary>
    /// <typeparam name="TItem">Тип данных сериализуемого элемента</typeparam>
    /// <typeparam name="TValue">Тип значения атрибута</typeparam>
    /// <param name="Selector">Метод определения набора данных, который должен быть упакован в атрибуты</param>
    /// <param name="NameSelector">Метод определения имени каждого конкретного атрибута в том числе</param>
    /// <param name="NeedToSerialize">Метод, определяющий - требуется ли выполнять сериализацию конкретного значения в атрибут?</param>
    /// <param name="ValueSelector">Метод определения значения каждого конкретного атрибута по порядковому номеру</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Attributes<TItem, TValue>(
        Func<T, IEnumerable<TItem>> Selector,
        Func<TItem, int, string> NameSelector,
        Func<TItem, bool> NeedToSerialize,
        Func<TItem, int, TValue> ValueSelector)
    {
        _Attributes.Add(element => Selector(element).Where(NeedToSerialize).ToArray((item, i) => new XAttribute(NameSelector(item, i), ValueSelector(item, i))));
        return this;
    }

    /// <summary>Добавление конфигурации элемента</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="Selector">Метод определения значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Element<TValue>(
        string? Name,
        Func<T, TValue> Selector)
    {
        _Elements.Add(v => new XElement(Name ?? __EmptyName, Selector(v)));
        return this;
    }

    /// <summary>Добавление конфигурации элемента</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="Selector">Метод определения значения элемента</param>
    /// <param name="NeedToSerialize">Метод, определяющий - нужно ли выполнять сериализацию данного элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Element<TValue>(
        string? Name,
        Func<T, TValue> Selector,
        Func<TValue, bool> NeedToSerialize)
    {
        _Elements.Add(item =>
        {
            var value = Selector(item);
            return NeedToSerialize(value) ? new XElement(Name ?? __EmptyName, value) : null;
        });
        return this;
    }

    /// <summary>Добавление конфигурации элемента</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="Selector">Метод определения значения элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Element<TValue>(
        string? Name,
        Func<T, TValue> Selector,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>(Name);
        Configurator(serializer);
        _Elements.Add(value => serializer.Serialize(Selector(value)));
        return this;
    }

    /// <summary>Добавление конфигурации элемента</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="Selector">Метод определения значения элемента</param>
    /// <param name="NeedToSerialize">Метод, определяющий - нужно ли выполнять сериализацию данного элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Element<TValue>(
        string? Name,
        Func<T, TValue> Selector,
        Func<TValue, bool> NeedToSerialize,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>(Name);
        Configurator(serializer);
        _Elements.Add(item =>
        {
            var value = Selector(item);
            return NeedToSerialize(value) ? serializer.Serialize(value) : null;
        });
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        string? Name,
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>(Name);
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).ToArray(serializer.Serialize));
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="Name">Имя элемента</param>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="NeedToSerialize">Метод, определяющий - нужно ли выполнять сериализацию данного элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        string? Name,
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Func<TValue, bool> NeedToSerialize,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>(Name);
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray(serializer.Serialize));
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="NameSelector">Метод определения имени элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Func<TValue, string> NameSelector,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>();
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).ToArray(v => serializer.Serialize(NameSelector(v), v)));
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="NameSelector">Метод определения имени элемента</param>
    /// <param name="NeedToSerialize">Метод, определяющий - нужно ли выполнять сериализацию данного элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Func<TValue, string> NameSelector,
        Func<TValue, bool> NeedToSerialize,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>();
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray(v => serializer.Serialize(NameSelector(v), v)));
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="NameSelector">Метод определения имени элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Func<TValue, int, string> NameSelector,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>();
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).ToArray((v, i) => serializer.Serialize(NameSelector(v, i), v)));
        return this;
    }

    /// <summary>Добавление конфигурации набора элементов</summary>
    /// <typeparam name="TValue">Тип данных значения элемента</typeparam>
    /// <param name="ElementsSelector">Метод определения набора значений элементов для сериализации</param>
    /// <param name="NameSelector">Метод определения имени элемента</param>
    /// <param name="NeedToSerialize">Метод, определяющий - нужно ли выполнять сериализацию данного элемента</param>
    /// <param name="Configurator">Конфигурация сериализатора значения элемента</param>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Elements<TValue>(
        Func<T, IEnumerable<TValue>> ElementsSelector,
        Func<TValue, int, string> NameSelector,
        Func<TValue, bool> NeedToSerialize,
        Action<LambdaXmlSerializer<TValue>> Configurator)
    {
        var serializer = new LambdaXmlSerializer<TValue>();
        Configurator(serializer);
        _Elements.Add(value => ElementsSelector(value).Where(NeedToSerialize).ToArray((v, i) => serializer.Serialize(NameSelector(v, i), v)));
        return this;
    }

    /// <summary>Добавление конфигурации, устанавливающий необходимость включения значения сериализуемого объекта</summary>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Value()
    {
        _Elements.Add(v => v);
        return this;
    }

    /// <summary>Добавление конфигурации, устанавливающий необходимость включения указанного значения</summary>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Value<TValue>(TValue Value)
    {
        _Elements.Add(_ => Value);
        return this;
    }

    /// <summary>Добавление конфигурации, устанавливающий необходимость включения значения, определяемого на основе сериализуемого объекта</summary>
    /// <returns>Исходный сериализатор</returns>
    public LambdaXmlSerializer<T> Value<TValue>(Func<T, TValue> Selector)
    {
        _Elements.Add(v => Selector(v));
        return this;
    }
}