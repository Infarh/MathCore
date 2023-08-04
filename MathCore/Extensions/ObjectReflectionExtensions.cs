#nullable enable
using System.Collections.Concurrent;
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

namespace MathCore.Extensions;
public static class ObjectReflectionExtensions
{
    private static readonly ConcurrentDictionary<(Type, string), Func<object, object?>?> __PropertyGetters = new();

    private static Func<object, object?>? GetPublicPropertyGetter((Type type, string PropertyName) property)
    {
        var (type, property_name) = property;

        if (type.GetProperty(property_name) is not { CanRead: true, PropertyType: { IsValueType: var is_value_type } } property_info)
            return null;

        var obj_parameter = "obj".ParameterOf<object>();
        Expression body = obj_parameter
            .ConvertTo(type)
            .GetProperty(property_info);

        if (is_value_type)
            body = body.ConvertTo<object>();

        var function = body
            .CreateLambda<Func<object, object?>>(obj_parameter)
            .Compile();

        return function;
    }

    public static object? GetPropertyValue(this object obj, string PropertyName)
    {
        var type = obj.NotNull().GetType();
        if (__PropertyGetters.GetOrAdd((type, PropertyName), GetPublicPropertyGetter) is not { } getter)
            throw new InvalidOperationException($"Тип {type} не содержит свойства {PropertyName} доступного для чтения");

        var value = getter(obj);
        return value;
    }

    public static T? GetPropertyValue<T>(this object obj, string PropertyName) => (T?)obj.GetPropertyValue(PropertyName);

    private static Delegate? GetPublicPropertyGetter<T, TValue>((Type type, string PropertyName) property)
    {
        var (type, property_name) = property;

        if (type.GetProperty(property_name) is not { CanRead: true } property_info)
            return null;

        var obj_parameter = "obj".ParameterOf(type);
        var expr = obj_parameter
            .GetProperty(property_info)
            .CreateLambda<Func<T, TValue?>>(obj_parameter);

        var function = expr.Compile();

        return function;
    }

    private static readonly ConcurrentDictionary<(Type, string), Delegate?> __TypedGetters = new();

    public static TValue? GetPropertyValue<T, TValue>(this T obj, string PropertyName)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        if (__TypedGetters.GetOrAdd((type, PropertyName), GetPublicPropertyGetter<T, TValue>) is not Func<T, TValue> getter)
            throw new InvalidOperationException($"Тип {type} не содержит свойства {PropertyName} доступного для чтения");

        var value = getter(obj);
        return value;
    }

    private static readonly ConcurrentDictionary<(Type, string), Action<object, object?>?> __PropertySetters = new();

    private static Action<object, object?>? GetPublicPropertySetter((Type type, string PropertyName) property)
    {
        var (type, property_name) = property;

        if (type.GetProperty(property_name) is not { CanWrite: true, SetMethod: var set_method })
            return null;

        var parameter = "obj".ParameterOf<object>();
        var value_parameter = "value".ParameterOf<object>();

        var call_expr = set_method.GetCallExpression(
            parameter.ConvertTo(type), 
            value_parameter.ConvertTo(set_method.GetParameters()[0].ParameterType));

        var action = call_expr.
            CreateLambda<Action<object, object?>>(parameter, value_parameter)
            .Compile();

        return action;
    }

    public static void SetPropertyValue(this object obj, string PropertyName, object? Value)
    {
        var type = obj.NotNull().GetType();

        if (__PropertySetters.GetOrAdd((type, PropertyName), GetPublicPropertySetter) is not { } setter)
            throw new InvalidOperationException($"Тип {type} не содержит свойства {PropertyName} доступного для записи");

        setter(obj, Value);
    }

    private static readonly ConcurrentDictionary<(Type, string), Delegate?> __TypedPropertySetters = new();

    private static Delegate? GetPublicPropertySetter<T, TValue>((Type type, string PropertyName) property)
    {
        var (type, property_name) = property;

        if (type.GetProperty(property_name) is not { CanWrite: true, SetMethod: var set_method })
            return null;

        var parameter = "obj".ParameterOf<T>();
        var value_parameter = "value".ParameterOf<TValue>();

        Expression instance_expr = type == typeof(T) ? parameter : parameter.ConvertTo(type);
        var call_expr = set_method.GetCallExpression(instance_expr, value_parameter);

        var action = call_expr
            .CreateLambda<Action<T, TValue?>>(parameter, value_parameter)
            .Compile();

        return action;
    }

    public static void SetPropertyValue<T, TValue>(this T obj, string PropertyName, TValue? Value)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();

        if (__TypedPropertySetters.GetOrAdd((type, PropertyName), GetPublicPropertySetter<T, TValue>) is not Action<T, TValue?> setter)
            throw new InvalidOperationException($"Тип {type} не содержит свойства {PropertyName} доступного для записи");

        setter(obj, Value);
    }
}
