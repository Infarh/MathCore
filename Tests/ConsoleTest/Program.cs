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
        var dx = 1e12;
        const double pi = Math.PI;
        var last_x = 0d;

        do
        {
            Console.Title = Math.Log10(dx).ToString();
            for (var (x, x0) = (last_x, -1d) ; 1 + 1 / (pi * x) > 1 && x != x0; x0 = x, x += dx)
            {
                Console.WriteLine(x);
                last_x = x;
            }

            dx /= 10;
        }
        while (dx > 0);


        var values = new int[5];
        foreach (var array_item in new ArrayItems<int>(values))
        {
            array_item.Value = array_item.Index;
        }



        //HistogramTest.Run();
        //HistogramTest.RunIteration();

        Console.ReadLine();
    }
}

public interface ITest
{
    static abstract ITest operator +(ITest a, ITest b);
}