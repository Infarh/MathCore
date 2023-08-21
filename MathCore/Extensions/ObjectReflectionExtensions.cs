#nullable enable
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

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
        if(!obj.TryGetPropertyValue(PropertyName, out var value))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит свойства {PropertyName} доступного для чтения");
        return value;
    }

    public static bool TryGetPropertyValue(this object obj, string PropertyName, out object? value)
    {
        var type = obj.NotNull().GetType();
        if (__PropertyGetters.GetOrAdd((type, PropertyName), GetPublicPropertyGetter) is not { } getter)
        {
            value = null;
            return false;
        }

        value = getter(obj);
        return true;
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
        if(!obj.TryGetPropertyValue(PropertyName, out TValue value))
            throw new InvalidOperationException($"Тип {typeof(T)} не содержит свойства {PropertyName} доступного для чтения");
        return value;
    }

    public static bool TryGetPropertyValue<T, TValue>(this T obj, string PropertyName, out TValue? value)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        if (__TypedGetters.GetOrAdd((type, PropertyName), GetPublicPropertyGetter<T, TValue>) is not Func<T, TValue> getter)
        {
            value = default;
            return false;
        }    

        value = getter(obj);
        return true;
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
        if(!obj.TrySetPropertyValue(PropertyName, Value))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит свойства {PropertyName} доступного для записи");
    }

    public static bool TrySetPropertyValue(this object obj, string PropertyName, object? Value)
    {
        var type = obj.NotNull().GetType();

        if (__PropertySetters.GetOrAdd((type, PropertyName), GetPublicPropertySetter) is not { } setter)
            return false;

        setter(obj, Value);
        return true;
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
        if(!obj.TrySetPropertyValue(PropertyName, Value))
            throw new InvalidOperationException($"Тип {typeof(T)} не содержит свойства {PropertyName} доступного для записи");
    }

    public static bool TrySetPropertyValue<T, TValue>(this T obj, string PropertyName, TValue? Value)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();

        if (__TypedPropertySetters.GetOrAdd((type, PropertyName), GetPublicPropertySetter<T, TValue>) is not Action<T, TValue?> setter)
            return false;

        setter(obj, Value);
        return true;
    }

    public static IDictionary<string, object?> GetPropertyValues<T>(this T obj) => new PropertyValuesController<T>(obj);

    private class PropertyValuesController<T>(T obj) : IDictionary<string, object?>
    {
        private readonly Lazy<HashSet<string>> _PropertyNames = new(() => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => p.Name).GetHashSet());

        public object? this[string key]
        {
            get
            {
                if (!_PropertyNames.Value.Contains(key))
                    throw new InvalidOperationException($"В объекте типа {typeof(T)} свойство {key} отсутствует");

                if(obj.TryGetPropertyValue(key, out var value)) 
                    return value;

                throw new InvalidOperationException($"Свойство {typeof(T)}.{key} не доступно для чтения");
            }
            set
            {
                if (!_PropertyNames.Value.Contains(key))
                    throw new InvalidOperationException($"В объекте типа {typeof(T)} свойство {key} отсутствует");

                if (!obj.TrySetPropertyValue(key, value))
                    throw new InvalidOperationException($"Свойство {typeof(T)}.{key} не доступно для записи");
            }
        }

        public ICollection<string> Keys => _PropertyNames.Value;

        public ICollection<object?> Values => Keys.ToList(p => this[p]).AsReadOnly();

        public int Count => Keys.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value) => this[key] = value;

        public void Add(KeyValuePair<string, object?> item) => Add(item.Key, item.Value);

        public void Clear() => throw new NotSupportedException();

        public bool Contains(KeyValuePair<string, object?> item) => Equals(this[item.Key], item.Value);
        
        public bool ContainsKey(string key) => _PropertyNames.Value.Contains(key);

        public void CopyTo(KeyValuePair<string, object?>[] array, int Index)
        {
            var properties = _PropertyNames.Value;
            if (array.Length - Index < properties.Count)
                throw new InvalidOperationException("Недостаточная длина массива");

            var i = 0;
            foreach(var property in properties)
                array[i++] = new(property, this[property]);
        }

        public bool Remove(string key) => throw new NotSupportedException();

        public bool Remove(KeyValuePair<string, object?> item) => throw new NotSupportedException();

        public bool TryGetValue(string key, out object value) => obj.TryGetPropertyValue(key, out value);

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            foreach (var property in Keys)
                yield return new(property, this[property]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
