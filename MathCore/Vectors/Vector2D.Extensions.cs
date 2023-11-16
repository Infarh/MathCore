#nullable enable
namespace MathCore.Vectors;

public static class Vector2DExtensions
{
    public static Vector2D Average(this IEnumerable<Vector2D> vectors)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var count = 0;
        foreach (var (x, y) in vectors)
        {
            x_sum += x;
            y_sum += y;
            count++;
        }

        return count == 0 ? Vector2D.NaN : new(x_sum / count, y_sum / count);
    }

    public static Vector2D Average<T>(this IEnumerable<T> items, Func<T, Vector2D> VectorSelector)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var count = 0;
        foreach (var (x, y) in items.Select(VectorSelector))
        {
            x_sum += x;
            y_sum += y;
            count++;
        }

        return count == 0 ? Vector2D.NaN : new(x_sum / count, y_sum / count);
    }

    public static Vector2D Sum(this IEnumerable<Vector2D> vectors)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        foreach (var (x, y) in vectors)
        {
            x_sum += x;
            y_sum += y;
        }

        return new(x_sum, y_sum);
    }

    public static Vector2D Sum<T>(this IEnumerable<T> items, Func<T, Vector2D> VectorSelector)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        foreach (var (x, y) in items.Select(VectorSelector))
        {
            x_sum += x;
            y_sum += y;
        }

        return new(x_sum, y_sum);
    }
}