#nullable enable
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MathCore.Annotations;
using MathCore.Extensions.Expressions;
// ReSharper disable UnusedMember.Global
// ReSharper disable AnnotateNotNullParameter
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Класс методов-расширений для <see cref="Type"/></summary>
public static class TypeExtensions
{
    /// <summary>Значение данного типа допускают <see langword="null"/></summary>
    /// <param name="type">Проверяемый тип</param>
    /// <returns>Истина, если значение проверяемого типа допускают возможность пустой ссылки</returns>
    public static bool IsCanBeNullRef(this Type type) => type.IsClass || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>Функция, ничего не делающая</summary>
    private static readonly Func<object, object> __NoChangeTypeFunction = o => o;

    #region Приведение типов

    /// <summary>Пул методов приведения типов</summary>
    private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType), Func<object, object>> __CastersDictionary = new();

    /// <summary>Выражение параметра конструируемой функции</summary>
    private static readonly ParameterExpression __ConvParameter = Expression.Parameter(typeof(object), "value");

    /// <summary>Сформировать функцию, осуществляющую приведение типов</summary>
    /// <param name="TargetType">Целевой тип значения</param>
    /// <param name="Source">Объект, тип которого требуется привести</param>
    /// <returns>Функция, осуществляющая приведение типа объекта к целевому типу данных</returns>
    private static Func<object, object> GetCasterFrom<T>(Type TargetType, [CanBeNull] T Source) => TargetType.GetCasterFrom(Source?.GetType() ?? typeof(object));

    /// <summary>Сформировать функцию, осуществляющую приведение типа значения к указанному типу данных</summary>
    /// <param name="SourceType">Тип исходных данных</param>
    /// <param name="TargetType">Тип, к которому требуется выполнить приведение</param>
    /// <returns>Функция, осуществляющая приведение типа значения</returns>
    public static Func<object, object> GetCasterTo(this Type SourceType, Type TargetType) => TargetType.GetCasterFrom(SourceType);

    /// <summary>Сформировать функцию, осуществляющую приведение типа значения из указанного типа данных</summary>
    /// <param name="SourceType">Тип исходных данных</param>
    /// <param name="TargetType">Тип, к которому требуется выполнить приведение</param>
    /// <returns>Функция, осуществляющая приведение типа значения</returns>
    public static Func<object, object> GetCasterFrom(this Type TargetType, Type SourceType) =>
        SourceType == TargetType
            ? __NoChangeTypeFunction
            : __CastersDictionary.GetOrAdd((SourceType, TargetType), types =>
            {
                var (source_type, target_type) = types;
                var object_2_source = Expression.Convert(__ConvParameter, source_type);
                var source_2_target = Expression.Convert(object_2_source, target_type);
                var target_2_object = Expression.Convert(source_2_target, typeof(object));
                return Expression.Lambda<Func<object, object>>(target_2_object, __ConvParameter).Compile();
            });

    /// <summary>Выполнить приведение типа значения объекта</summary>
    /// <param name="type">Целевой тип данных</param>
    /// <param name="obj">Приводимое значение</param>
    /// <returns>Объект-значение, приведённое к целевому типу данных</returns>
    public static object Cast<T>(this Type type, T obj) => GetCasterFrom(type, obj)(obj!);

    #endregion

    /// <summary>Выражение параметра конструируемой функции</summary>
    private static readonly ConcurrentDictionary<(Type SourceType, Type TargetType), Func<object, object>> __ConvertersDictionary = new();

    /// <summary>Сформировать функцию, осуществляющую преобразование типа в указанный тип данных</summary>
    /// <param name="SourceType">Тип исходного значения</param>
    /// <param name="TargetType">Целевой тип данных</param>
    /// <returns>Функция, осуществляющая преобразование исходного значение в значение целевого типа данных</returns>
    public static Func<object, object> GetConverterTo(this Type SourceType, Type TargetType)
        => SourceType == TargetType ? o => o : TargetType.GetConverterFrom(SourceType);

    /// <summary>Сформировать функцию, осуществляющую преобразование типа в указанный тип данных</summary>
    /// <param name="SourceType">Тип исходного значения</param>
    /// <param name="TargetType">Целевой тип данных</param>
    /// <returns>Функция, осуществляющая преобразование исходного значение в значение целевого типа данных</returns>
    public static Func<object, object> GetConverterFrom(this Type TargetType, Type SourceType) =>
        SourceType == TargetType
            ? __NoChangeTypeFunction
            : __ConvertersDictionary.GetOrAdd(
                (SourceType, TargetType),
                tt => tt.SourceType.GetConvertExpression_Object(tt.TargetType).Compile());

    /// <summary>Сформировать выражение, осуществляющее приведение типа к указанному типу данных</summary>
    /// <param name="SourceType">Тип исходного значения</param>
    /// <param name="TargetType">Целевой тип данных</param>
    /// <returns>Выражение, осуществляющее приведение исходного значение к целевому типу данных</returns>
    public static Expression GetCastExpression(this Type SourceType, Type TargetType, ref ParameterExpression parameter)
    {
        if (SourceType == TargetType) return parameter;
        Expression source = parameter;
        if (source is null) source                 = __ConvParameter;
        else if (source.Type != SourceType) source = source.ConvertTo(SourceType);
        return source.ConvertTo(TargetType).ConvertTo(typeof(object));
    }

    /// <summary>Сформировать выражение, осуществляющее преобразование типа данных</summary>
    /// <param name="SourceType">Тип исходного значения</param>
    /// <param name="TargetType">Целевой тип данных</param>
    /// <returns>Выражение, осуществляющее преобразование типа данных</returns>
    /// <exception cref="NotSupportedException">Если преобразование невозможно в виду отсутствия определённых конвертеров типов</exception>
    public static LambdaExpression GetConvertExpression(this Type SourceType, Type TargetType)
    {
        var            converter    = SourceType.GetTypeConverter();
        TypeConverter? converter_to = null;
        if (!converter.CanConvertTo(TargetType) && !(converter_to = TargetType.GetTypeConverter()).CanConvertFrom(SourceType))
            throw new NotSupportedException($"Преобразование из {SourceType} в {TargetType} не поддерживается");
        var parameter_source     = Expression.Parameter(SourceType, "pFrom");
        var source_to_object     = parameter_source.ConvertTo(typeof(object));
        var converter_expression = (converter_to ?? converter).ToExpression();
        var converter_delegate = converter_to is null
            ? (Delegate)(Func<object, Type, object>)converter.ConvertTo
            : (Func<object, object>)converter_to.ConvertFrom;
        var converter_args = converter_to is null
            ? new Expression[] { source_to_object, TargetType.ToExpression() }
            : new Expression[] { source_to_object };
        var expr_conversation = Expression.Call(converter_expression, converter_delegate.Method, converter_args);

        return Expression.Lambda(expr_conversation.ConvertTo(TargetType), parameter_source);
    }

    /// <summary>Сформировать лямбда-выражение для конвертации значения</summary>
    /// <param name="SourceType">Тип исходного значения</param>
    /// <param name="TargetType">Целевой тип данных</param>
    /// <exception cref="NotSupportedException">Если конвертация из <paramref name="SourceType"/> в <paramref name="TargetType"/> не реализована</exception>
    /// <returns>Лямбда-выражение, осуществляющее конвертацию значения</returns>
    public static Expression<Func<object, object>> GetConvertExpression_Object(this Type SourceType, Type TargetType)
    {
        var            converter    = SourceType.GetTypeConverter();
        TypeConverter? converter_to = null;
        if (!converter.CanConvertTo(TargetType) && !(converter_to = TargetType.GetTypeConverter()).CanConvertFrom(SourceType))
            throw new NotSupportedException($"Преобразование из {SourceType} в {TargetType} не поддерживается");
        var parameter_source     = Expression.Parameter(typeof(object), "pFrom");
        var converter_expression = (converter_to ?? converter).ToExpression();
        var converter_delegate = converter_to is null
            ? (Delegate)(Func<object, Type, object>)converter.ConvertTo
            : (Func<object, object>)converter_to.ConvertFrom;
        var converter_args = converter_to is null
            ? new Expression[] { parameter_source, Expression.Constant(TargetType) }
            : new Expression[] { parameter_source };
        var expr_conversation = Expression.Call(converter_expression, converter_delegate.Method, converter_args);

        return Expression.Lambda<Func<object, object>>(Expression.Convert(expr_conversation, typeof(object)), parameter_source);
    }

    /// <summary>Получить конвертер значений для указанного типа данных</summary>
    /// <param name="type">Тип, для которого требуется получить конвертер</param>
    /// <returns>Конвертер указанного типа данных</returns>
    public static TypeConverter GetTypeConverter(this Type type) => TypeDescriptor.GetConverter(type);

    /// <summary>Получить тип по его имени из всех загруженных сборок</summary>
    /// <param name="TypeName">Имя типа</param>
    /// <returns>Тип</returns>
    public static Type? GetType(string TypeName)
    {
        var type_array = AppDomain.CurrentDomain.GetAssemblies().
            SelectMany(a => a.GetTypes()).Where(t => t.Name == TypeName).ToArray();
        return type_array.Length != 0 ? type_array[0] : null;
    }

    /// <summary>Получить все атрибуты типа указанного типа</summary>
    /// <typeparam name="TAttribute">Тип требуемых атрибутов</typeparam>
    /// <param name="T">Тип, атрибуты которого требуется получить</param>
    /// <returns>Массив атрибутов типа указанного типа</returns>
    public static TAttribute[] GetCustomAttributes<TAttribute>(this Type T)
        where TAttribute : Attribute => GetCustomAttributes<TAttribute>(T, false);

    /// <summary>Получить атрибуты типа</summary>
    /// <typeparam name="TAttribute">Тип необходимых атрибутов</typeparam>
    /// <param name="T">Тип, из которого требуется получить атрибуты</param>
    /// <param name="Inherited">Искать атрибуты в базовых классах типа</param>
    /// <returns>Массив найденных атрибутов</returns>
    public static TAttribute[] GetCustomAttributes<TAttribute>(this Type T, bool Inherited)
        where TAttribute : Attribute => T.GetCustomAttributes(typeof(TAttribute), Inherited).OfType<TAttribute>().ToArray();

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <param name="type">Тип создаваемого объекта</param>
    /// <returns>Созданный объект</returns>
    public static object CreateObject(this Type type) => Activator.CreateInstance(type);

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <typeparam name="T">Тип необходимого объекта</typeparam>
    /// <param name="type">Тип создаваемого объекта</param>
    /// <returns>Созданный объект, приведённый к типу <typeparamref name="T"/></returns>
    public static T Create<T>(this Type type) => (T)type.CreateObject();

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <typeparam name="T">Тип необходимого объекта</typeparam>
    /// <param name="type">Тип создаваемого объекта</param>
    /// <param name="Params">Параметры конструктора</param>
    /// <returns>Созданный объект, приведённый к типу <typeparamref name="T"/></returns>
    public static T Create<T>(this Type type, params object[] Params) => (T)type.CreateObject(Params);

    /// <summary>Создать объект типа с помощью конструктора с заданным набором параметров</summary>
    /// <param name="type">Тип создаваемого объекта</param>
    /// <param name="Params">Параметры конструктора</param>
    /// <returns>Созданный объект</returns>
    public static object CreateObject(this Type type, params object[] Params) =>
        Activator.CreateInstance(type, Params);

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <param name="type">Тип создаваемого объекта</param>
    /// <param name="Flags">Флаги, определяющие метод поиска конструктора</param>
    /// <param name="binder">Объект, определяющий способ поиска конструктора</param>
    /// <param name="Params">Параметры конструктора</param>
    /// <returns>Созданный объект</returns>
    public static object CreateObject(this Type type, BindingFlags Flags, Binder binder, params object[] Params) =>
        Activator.CreateInstance(type, Flags, binder, Params);

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <param name="Params">Параметры конструктора</param>
    /// <returns>Созданный объект, приведённый к типу <typeparamref name="T"/></returns>
    public static T Create<T>(params object[] Params) => (T)CreateObject(typeof(T), Params);

    /// <summary>Создать объект типа с помощью конструктора по умолчанию</summary>
    /// <typeparam name="T">Тип необходимого объекта</typeparam>
    /// <param name="Flags">Флаги, определяющие метод поиска конструктора</param>
    /// <param name="binder">Объект, определяющий способ поиска конструктора</param>
    /// <param name="Params">Параметры конструктора</param>
    /// <returns>Созданный объект, приведённый к типу <typeparamref name="T"/></returns>
    public static T Create<T>(BindingFlags Flags, Binder binder, params object[] Params) =>
        (T)CreateObject(typeof(T), Flags, binder, Params);

    /// <summary>Добавить конвертер к типу</summary>
    /// <param name="type">Тип, к которому требуется добавить конвертер</param>
    /// <param name="ConverterType">Тип конвертера, который надо добавить к описанию типа</param>
    public static void AddConverter(this Type type, Type ConverterType) =>
        TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(ConverterType));

    /// <summary>Добавить конвертер к типу</summary>
    /// <param name="type">Тип, к которому требуется добавить конвертер</param>
    /// <param name="ConverterTypes">Типы конвертеров, которые требуется добавить к описанию типа</param>
    public static void AddConverter(this Type type, params Type[] ConverterTypes) =>
        TypeDescriptor.AddAttributes(type, ConverterTypes.Select(t => new TypeConverterAttribute(t)).Cast<Attribute>().ToArray());

    /// <summary>Получить объект-описатель типа <see cref="TypeDescriptionProvider"/></summary>
    /// <param name="type">Исследуемый тип</param>
    /// <returns>Объект, определяющий информацию о типе - <see cref="TypeDescriptionProvider"/></returns>
    static TypeDescriptionProvider GetProvider(this Type type) => TypeDescriptor.GetProvider(type);

    /// <summary>Добавить описатель типа - <see cref="TypeDescriptionProvider"/></summary>
    /// <param name="type">Тип, к которому требуется добавить описатель</param>
    /// <param name="provider">Добавляемый описатель типа <see cref="TypeDescriptionProvider"/></param>
    public static void AddProvider(this Type type, TypeDescriptionProvider provider) => TypeDescriptor.AddProvider(provider, type);
}