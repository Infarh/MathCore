using System.Collections.Concurrent;
using System.Linq.Expressions;

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
        HistogramTest.Run();
        HistogramTest.RunIteration();

        Console.ReadLine();
    }
}
