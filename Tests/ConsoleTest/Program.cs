using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;

using MathCore;

// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable DoubleEquals

namespace ConsoleTest;

internal static class Program
{
    private static readonly ConcurrentDictionary<(Type, string), Delegate> __Getters = new();
    public static T GetValue<T>(object source, string PropertyName)
    {
        var getter = (Func<object, T>)__Getters.GetOrAdd(
            (typeof(T), PropertyName), v =>
            {
                var (type, property_name) = v;
                var parameter = Expression.Parameter(typeof(object), "p");
                var cast = Expression.TypeAs(parameter, type);
                var property = Expression.Property(cast, property_name);
                var expression = Expression.Lambda<Func<object, T>>(property, parameter);
                return expression.Compile();
            });
        return getter(source);
    }

    private static void Main()
    {
        var str = "Hello world _    +123,321   _===";

        var ptr = new StringPtr(str);
        var index = ptr.IndexOf('_');

        var substr = ptr.Substring(index + 1);
        index = substr.IndexOf('_');
        substr = substr.Substring(0, index);

        var parsed = substr.TryParseDouble(out var int_value);


        HistogramTest.Run();
        HistogramTest.RunIteration();

        Console.ReadLine();
    }
}
