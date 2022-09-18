using System.Collections;
using System.Reflection;
using System.Text;

namespace MathCore.Tests.Infrastructure;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class TestDataAttribute : Attribute, ITestDataSource
{
    public string MethodSourceName { get; set; }

    public string DisplayName { get; set; }

    public string Prefix { get; set; }

    public IEnumerable<object[]> GetData(MethodInfo TestMethod)
    {
        if (MethodSourceName is not { Length: > 0 } method_name)
            throw new InvalidOperationException("Не указан метод-источник данных");

        var type = TestMethod.DeclaringType ?? throw new InvalidOperationException("Не удалось определить класс модульных тестов");

        var source_method = type.GetMethod(MethodSourceName, BindingFlags.Static | BindingFlags.Public)
            ?? type.GetMethod(MethodSourceName, BindingFlags.Static | BindingFlags.NonPublic);

        if (source_method is null)
            throw new InvalidOperationException("Не удалось найти статический метод-источник данных в классе модульного теста");

        if (source_method.ReturnType.GetInterface(typeof(IEnumerable).FullName!) is null)
        {
            //throw new InvalidOperationException("Метод-источник данных возвращает значение, не поддерживающее перечисление");
        }

        var test_data = (IEnumerable)source_method.Invoke(null, null)
            ?? throw new InvalidOperationException("Не удалось получить результат вызова метода-источника данных");

        var test_method_parameters = TestMethod.GetParameters();

        foreach (var data in test_data)
        {
            if (data is null) continue;

            var data_type = data.GetType();
            var value_tuple_fields = data_type.FullName!.StartsWith("System.ValueTuple")
                ? data_type.GetFields()
                : null;

            var result = new object[test_method_parameters.Length];
            for (var i = 0; i < result.Length; i++)
            {
                var parameter_name = test_method_parameters[i].Name!;

                if (value_tuple_fields is not null)
                    result[i] = value_tuple_fields[i].GetValue(data);
                else if (data is IDictionary dictionary)
                    result[i] = dictionary[parameter_name];
                else if (data_type.GetProperty(parameter_name) is { } property)
                    result[i] = property.GetValue(data);
                else if (data_type.GetField(parameter_name) is { } field)
                    result[i] = field.GetValue(data);
            }

            yield return result;
        }
    }

    public string GetDisplayName(MethodInfo TestMethod, object[] data)
    {
        if (!string.IsNullOrWhiteSpace(DisplayName)) return DisplayName;
        if (data is null) return null;

        var result = new StringBuilder(Prefix ?? TestMethod.Name);
        result.Append("(");
        var parameters = TestMethod.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            result.Append(parameters[i].Name);
            result.Append(": ");
            var value = data[i];
            if (value is IEnumerable e)
            {
                result.Append("{ ");
                var any = false;
                foreach (var e_value in e)
                {
                    any = true;
                    result.Append(e_value);
                    result.Append(", ");
                }

                if (any)
                    result.Length -= 2;
                result.Append(" }");
            }
            else
                result.Append(value);

            result.Append(", ");
        }

        if (parameters.Length > 0)
            result.Length -= 2;
        result.Append(")");


        return result.ToString();
    }
}