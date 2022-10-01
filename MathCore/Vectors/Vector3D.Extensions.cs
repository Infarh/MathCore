#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathCore.Vectors;

public static class Vector3DExtensions
{
    public static Vector3D Average(this IEnumerable<Vector3D> vectors)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var z_sum = 0d;
        var count = 0;
        foreach (var (x, y, z) in vectors)
        {
            x_sum += x;
            y_sum += y;
            z_sum += z;
            count++;
        }

        return count == 0 ? Vector3D.NaN : new(x_sum / count, y_sum / count, z_sum / count);
    }

    public static Vector3D Average<T>(this IEnumerable<T> items, Func<T, Vector3D> VectorSelector)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var z_sum = 0d;
        var count = 0;
        foreach (var (x, y, z) in items.Select(VectorSelector))
        {
            x_sum += x;
            y_sum += y;
            z_sum += z;
            count++;
        }

        return count == 0 ? Vector3D.NaN : new(x_sum / count, y_sum / count, z_sum / count);
    }

    public static Vector3D Sum(this IEnumerable<Vector3D> vectors)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var z_sum = 0d;
        foreach (var (x, y, z) in vectors)
        {
            x_sum += x;
            y_sum += y;
            z_sum += z;
        }

        return new(x_sum, y_sum, z_sum);
    }

    public static Vector3D Sum<T>(this IEnumerable<T> items, Func<T, Vector3D> VectorSelector)
    {
        var x_sum = 0d;
        var y_sum = 0d;
        var z_sum = 0d;
        foreach (var (x, y, z) in items.Select(VectorSelector))
        {
            x_sum += x;
            y_sum += y;
            z_sum += z;
        }

        return new(x_sum, y_sum, z_sum);
    }
}