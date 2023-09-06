#nullable enable
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using MathCore.Extensions.Expressions;

namespace MathCore.Extensions;
public static class ObjectReflectionFieldsExtensions
{
    private const BindingFlags __NonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly ConcurrentDictionary<(Type, string), Func<object, object?>?> __FieldGetters = new();

    private static Func<object, object?>? GetPublicFieldGetter((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name) is not { FieldType: { IsValueType: var is_value_type } } field_info)
            return null;

        var obj_parameter = "obj".ParameterOf<object>();
        Expression body = obj_parameter
            .ConvertTo(type)
            .GetField(field_info);

        if (is_value_type)
            body = body.ConvertTo<object>();

        var function = body
            .CreateLambda<Func<object, object?>>(obj_parameter)
            .Compile();

        return function;
    }

    private static Func<object, object?>? GetPrivateFieldGetter((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name, __NonPublic) is not { FieldType: { IsValueType: var is_value_type } } field_info)
            return null;

        var obj_parameter = "obj".ParameterOf<object>();
        Expression body = obj_parameter
            .ConvertTo(type)
            .GetField(field_info);

        if (is_value_type)
            body = body.ConvertTo<object>();

        var function = body
            .CreateLambda<Func<object, object?>>(obj_parameter)
            .Compile();

        return function;
    }

    public static object? GetFieldValue(this object obj, string FieldName)
    {
        if(!obj.TryGetFieldValue(FieldName, out var value))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит поля {FieldName}")
            {
                Data = 
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(FieldName), FieldName },
                },
            };

        return value;
    }

    public static object? GetFieldValue(this object obj, string FieldName, bool NonPublic)
    {
        if (!obj.TryGetFieldValue(FieldName, NonPublic, out var value))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит поля {FieldName}")
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(FieldName), FieldName },
                    { nameof(NonPublic), NonPublic },
                },
            };

        return value;
    }

    public static bool TryGetFieldValue(this object obj, string FieldName, out object? value)
    {
        var type = obj.NotNull().GetType();
        if (__FieldGetters.GetOrAdd((type, FieldName), GetPublicFieldGetter) is not { } getter)
        {
            value = null;
            return false;
        }

        value = getter(obj);
        return true;
    }

    public static bool TryGetFieldValue(this object obj, string FieldName, bool NonPublic, out object? value)
    {
        if(!NonPublic)
            return obj.TryGetFieldValue(FieldName, out value);

        var type = obj.NotNull().GetType();
        if (__FieldGetters.GetOrAdd((type, FieldName), GetPrivateFieldGetter) is not { } getter)
        {
            value = null;
            return false;
        }

        value = getter(obj);
        return true;
    }

    public static TValue? GetFieldValue<TValue>(this object obj, string FieldName) => (TValue?)obj.GetFieldValue(FieldName);

    private static Delegate? GetPublicFieldGetter<T, TValue>((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name) is not { } field_info)
            return null;

        var obj_parameter = "obj".ParameterOf(type);
        var expr = obj_parameter
            .GetField(field_info)
            .CreateLambda<Func<T, TValue?>>(obj_parameter);

        var function = expr.Compile();

        return function;
    }

    private static Delegate? GetPrivateFieldGetter<T, TValue>((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name, __NonPublic) is not { } field_info)
            return null;

        var obj_parameter = "obj".ParameterOf(type);
        var expr = obj_parameter
            .GetField(field_info)
            .CreateLambda<Func<T, TValue?>>(obj_parameter);

        var function = expr.Compile();

        return function;
    }

    private static readonly ConcurrentDictionary<(Type, string), Delegate?> __TypedGetters = new();

    public static TValue? GetFieldValue<T, TValue>(this T obj, string FieldName)
    {
        if(!obj.TryGetFieldValue(FieldName, out TValue value))
            throw new InvalidOperationException($"Тип {typeof(T)} не содержит поля {FieldName}")
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(T), typeof(T) },
                    { nameof(TValue), typeof(TValue) },
                    { nameof(FieldName), FieldName },
                },
            };

        return value;
    }

    public static TValue? GetFieldValue<T, TValue>(this T obj, string FieldName, bool NonPublic)
    {
        if(!obj.TryGetFieldValue(FieldName, NonPublic, out TValue value))
            throw new InvalidOperationException($"Тип {typeof(T)} не содержит поля {FieldName}")
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(T), typeof(T) },
                    { nameof(TValue), typeof(TValue) },
                    { nameof(FieldName), FieldName },
                    { nameof(NonPublic), true },
                },
            };

        return value;
    }

    public static bool TryGetFieldValue<T, TValue>(this T obj, string FieldName, out TValue? value)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        if (__TypedGetters.GetOrAdd((type, FieldName), GetPublicFieldGetter<T, TValue>) is not Func<T, TValue> getter)
        {
            value = default;
            return false;
        }    

        value = getter(obj);
        return true;
    }

    public static bool TryGetFieldValue<T, TValue>(this T obj, string FieldName, bool NonPublic, out TValue? value)
    {
        if(!NonPublic)
            return obj.TryGetFieldValue(FieldName, out value);

        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        if (__TypedGetters.GetOrAdd((type, FieldName), GetPublicFieldGetter<T, TValue>) is not Func<T, TValue> getter)
        {
            value = default;
            return false;
        }    

        value = getter(obj);
        return true;
    }

    private static readonly ConcurrentDictionary<(Type, string), Action<object, object?>?> __FieldSetters = new();

    private static Action<object, object?>? GetPublicFieldSetter((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name) is not { IsInitOnly: false, SetMethod: var set_method })
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

    private static Action<object, object?>? GetPrivateFieldSetter((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name, __NonPublic) is not { IsInitOnly: false, SetMethod: var set_method })
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

    public static void SetFieldValue(this object obj, string FieldName, object? Value)
    {
        if(!obj.TrySetFieldValue(FieldName, Value))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит поля {FieldName} доступного для записи") 
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(FieldName), FieldName },
                },
            };
    }

    public static void SetFieldValue(this object obj, string FieldName, object? Value, bool NonPublic)
    {
        if(!obj.TrySetFieldValue(FieldName, Value, NonPublic))
            throw new InvalidOperationException($"Тип {obj.GetType()} не содержит поля {FieldName} доступного для записи") 
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(FieldName), FieldName },
                    { nameof(NonPublic), NonPublic },
                },
            };
    }

    public static bool TrySetFieldValue(this object obj, string FieldName, object? Value)
    {
        var type = obj.NotNull().GetType();

        if (__FieldSetters.GetOrAdd((type, FieldName), GetPublicFieldSetter) is not { } setter)
            return false;

        setter(obj, Value);
        return true;
    }

    public static bool TrySetFieldValue(this object obj, string FieldName, object? Value, bool NonPublic)
    {
        if(!NonPublic)
            return TrySetFieldValue(obj, FieldName, Value);

        var type = obj.NotNull().GetType();

        if (__FieldSetters.GetOrAdd((type, FieldName), GetPrivateFieldSetter) is not { } setter)
            return false;

        setter(obj, Value);
        return true;
    }

    private static readonly ConcurrentDictionary<(Type, string), Delegate?> __TypedFieldSetters = new();

    private static Delegate? GetPublicFieldSetter<T, TValue>((Type type, string FieldName) field)
    {
        var (type, field_name) = field;

        if (type.GetField(field_name) is not { IsInitOnly: false, SetMethod: var set_method })
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

    public static void SetFieldValue<T, TValue>(this T obj, string FieldName, TValue? Value)
    {
        if(!obj.TrySetFieldValue(FieldName, Value))
            throw new InvalidOperationException($"Тип {typeof(T)} не содержит поля {FieldName} доступного для записи")
            {
                Data =
                {
                    { nameof(obj), obj.GetType() },
                    { nameof(T), typeof(T) },
                    { nameof(TValue), typeof(TValue) },
                    { nameof(FieldName), FieldName },
                },
            }; ;
    }

    public static bool TrySetFieldValue<T, TValue>(this T obj, string FieldName, TValue? Value)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();

        if (__TypedFieldSetters.GetOrAdd((type, FieldName), GetPublicFieldSetter<T, TValue>) is not Action<T, TValue?> setter)
            return false;

        setter(obj, Value);
        return true;
    }

    public static IDictionary<string, object?> GetFieldValues<T>(this T obj) => new FieldValuesController<T>(obj);

    private class FieldValuesController<T>(T obj) : IDictionary<string, object?>
    {
        private readonly Lazy<HashSet<string>> _FieldNames = new(() => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => p.Name).GetHashSet());

        public object? this[string key]
        {
            get
            {
                if (!_FieldNames.Value.Contains(key))
                    throw new InvalidOperationException($"В объекте типа {typeof(T)} свойство {key} отсутствует");

                if(obj.TryGetFieldValue(key, out var value)) 
                    return value;

                throw new InvalidOperationException($"Поле {typeof(T)}.{key} не доступно для чтения");
            }
            set
            {
                if (!_FieldNames.Value.Contains(key))
                    throw new InvalidOperationException($"В объекте типа {typeof(T)} свойство {key} отсутствует");

                if (!obj.TrySetFieldValue(key, value))
                    throw new InvalidOperationException($"Поле {typeof(T)}.{key} не доступно для записи");
            }
        }

        public ICollection<string> Keys => _FieldNames.Value;

        public ICollection<object?> Values => Keys.ToList(p => this[p]).AsReadOnly();

        public int Count => Keys.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value) => this[key] = value;

        public void Add(KeyValuePair<string, object?> item) => Add(item.Key, item.Value);

        public void Clear() => throw new NotSupportedException();

        public bool Contains(KeyValuePair<string, object?> item) => Equals(this[item.Key], item.Value);
        
        public bool ContainsKey(string key) => _FieldNames.Value.Contains(key);

        public void CopyTo(KeyValuePair<string, object?>[] array, int Index)
        {
            var properties = _FieldNames.Value;
            if (array.Length - Index < properties.Count)
                throw new InvalidOperationException("Недостаточная длина массива");

            var i = 0;
            foreach(var field in properties)
                array[i++] = new(field, this[field]);
        }

        public bool Remove(string key) => throw new NotSupportedException();

        public bool Remove(KeyValuePair<string, object?> item) => throw new NotSupportedException();

        public bool TryGetValue(string key, out object value) => obj.TryGetFieldValue(key, out value);

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            foreach (var field in Keys)
                yield return new(field, this[field]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
