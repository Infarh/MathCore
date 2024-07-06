namespace MathCore.Vectors;

// https://ru.wikipedia.org/wiki/Алгоритм_Рамера_—_Дугласа_—_Пекера
// https://habr.com/ru/articles/448618/
public static class RamerDouglasPeucker
{
    public static Vector2D[] Smooth(IReadOnlyList<Vector2D> points, double eps)
    {
        var eps2 = eps * eps;

        var len = new double[points.Count];
        (len[0], len[^1]) = (eps, eps);

        var last_point = points.Count - 1;
        var (start, end) = (0, last_point);

        while (start < end)
        {
            var segment = (points[start], points[end]);
            var max = eps2;
            var max_index = 0;
            for (var (i, i1) = (start + 1, end - 1); i <= i1; i++)
            {
                var p = points[i];
                var l = Length2ToLine(p, segment);
                if (l < max) continue;

                max_index = i;
                max = l;
            }

            if (max_index == 0)
                (start, end) = (end, last_point);
            else
            {
                end = max_index;
                len[end] = max;
            }
        }

        var result = new List<Vector2D>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];
    }

    private static double Length2ToLine(Vector2D p0, (Vector2D p1, Vector2D p2) segment)
    {
        var (x0, y0) = p0;
        var ((x1, y1), (x2, y2)) = segment;

        var dx = x2 - x1;
        var dy = y2 - y1;

        var d = dy * x0 - dx * y0 + y1 * x2 - y2 * x1;
        return d * d / (dx * dx + dy * dy);
    }
}
