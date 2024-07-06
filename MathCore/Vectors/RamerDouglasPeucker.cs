using System.Drawing;

using Tuple2Pd = (double x, double y);
using Tuple2Pf = (float x, float y);
using Tuple3Pd = (double x, double y, double z);
using Tuple3Pf = (float x, float y, float z);

namespace MathCore.Vectors;

// https://ru.wikipedia.org/wiki/Алгоритм_Рамера_—_Дугласа_—_Пекера
// https://habr.com/ru/articles/448618/
public static class RamerDouglasPeucker
{
    public static PointF[] Smooth(IReadOnlyList<PointF> points, double eps)
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

        var result = new List<PointF>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static double Length2ToLine(PointF p0, (PointF p1, PointF p2) segment)
        {
            var (x0, y0) = p0;
            var ((x1, y1), (x2, y2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;

            var d = dy * x0 - dx * y0 + y1 * x2 - y2 * x1;
            return d * d / (dx * dx + dy * dy);
        }
    }

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

        static double Length2ToLine(Vector2D p0, (Vector2D p1, Vector2D p2) segment)
        {
            var (x0, y0) = p0;
            var ((x1, y1), (x2, y2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;

            var d = dy * x0 - dx * y0 + y1 * x2 - y2 * x1;
            return d * d / (dx * dx + dy * dy);
        }
    }

    public static Tuple2Pd[] Smooth(IReadOnlyList<Tuple2Pd> points, double eps)
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

        var result = new List<Tuple2Pd>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static double Length2ToLine(Tuple2Pd p0, (Tuple2Pd p1, Tuple2Pd p2) segment)
        {
            var (x0, y0) = p0;
            var ((x1, y1), (x2, y2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;

            var d = dy * x0 - dx * y0 + y1 * x2 - y2 * x1;
            return d * d / (dx * dx + dy * dy);
        }
    }

    public static Tuple2Pf[] Smooth(IReadOnlyList<Tuple2Pf> points, double eps)
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

        var result = new List<Tuple2Pf>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static float Length2ToLine(Tuple2Pf p0, (Tuple2Pf p1, Tuple2Pf p2) segment)
        {
            var (x0, y0) = p0;
            var ((x1, y1), (x2, y2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;

            var d = dy * x0 - dx * y0 + y1 * x2 - y2 * x1;
            return d * d / (dx * dx + dy * dy);
        }
    }

    public static Vector3D[] Smooth(IReadOnlyList<Vector3D> points, double eps)
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

        var result = new List<Vector3D>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static double Length2ToLine(Vector3D p0, (Vector3D p1, Vector3D p2) segment)
        {
            var (x0, y0, z0) = p0;
            var ((x1, y1, z1), (x2, y2, z2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;
            var dz = z2 - z1;

            var dx0 = x1 - x0;
            var dy0 = y1 - y0;
            var dz0 = z1 - z0;

            var mx = dy0 * dz - dz0 * dy;
            var my = dz0 * dx - dx0 * dz;
            var mz = dx0 * dy - dy0 * dx;

            var s1 = mx * mx + my * my + mz * mz;
            var s2 = dx * dx + dy * dy + dz * dz;

            return s1 / s2;
        }
    }

    public static Tuple3Pd[] Smooth(IReadOnlyList<Tuple3Pd> points, double eps)
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

        var result = new List<Tuple3Pd>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static double Length2ToLine(Tuple3Pd p0, (Tuple3Pd p1, Tuple3Pd p2) segment)
        {
            var (x0, y0, z0) = p0;
            var ((x1, y1, z1), (x2, y2, z2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;
            var dz = z2 - z1;

            var dx0 = x1 - x0;
            var dy0 = y1 - y0;
            var dz0 = z1 - z0;

            var mx = dy0 * dz - dz0 * dy;
            var my = dz0 * dx - dx0 * dz;
            var mz = dx0 * dy - dy0 * dx;

            var s1 = mx * mx + my * my + mz * mz;
            var s2 = dx * dx + dy * dy + dz * dz;

            return s1 / s2;
        }
    }

    public static Tuple3Pf[] Smooth(IReadOnlyList<Tuple3Pf> points, double eps)
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

        var result = new List<Tuple3Pf>(points.Count / 2);
        for (var i = 0; i <= last_point; i++)
            if (len[i] > 0)
                result.Add(points[i]);

        return [.. result];

        static double Length2ToLine(Tuple3Pf p0, (Tuple3Pf p1, Tuple3Pf p2) segment)
        {
            var (x0, y0, z0) = p0;
            var ((x1, y1, z1), (x2, y2, z2)) = segment;

            var dx = x2 - x1;
            var dy = y2 - y1;
            var dz = z2 - z1;

            var dx0 = x1 - x0;
            var dy0 = y1 - y0;
            var dz0 = z1 - z0;

            var mx = dy0 * dz - dz0 * dy;
            var my = dz0 * dx - dx0 * dz;
            var mz = dx0 * dy - dy0 * dx;

            var s1 = mx * mx + my * my + mz * mz;
            var s2 = dx * dx + dy * dy + dz * dz;

            return s1 / s2;
        }
    }
}
