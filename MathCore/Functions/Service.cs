using System;
using System.Drawing;
using MathCore.Vectors;

namespace MathCore.Functions;

public static class Service
{
    public static class Calculator
    {
        public static Func<double, double> GetParabola(Point p1, Point p2, Point p3) => GetParabola(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        public static Func<double, double> GetParabola(PointF p1, PointF p2, PointF p3) => GetParabola(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        public static Func<double, double> GetParabola(Vector2D p1, Vector2D p2, Vector2D p3) => GetParabola(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        public static Func<double, double> GetParabola(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3)
        {
            var a = (y3 - (x3 * (y2 - y1) + x2 * y1 - x1 * y2) / (x2 - x1)) / (x3 * (x3 - x1 - x2) + x1 * x2);
            var b = (y2 - y1) / (x2 - x1) - a * (x1 + x2);
            var c = (x2 * y1 - x1 * y2) / (x2 - x1) + a * x1 * x2;
            return x => a * x * x + b * x + c;
        }
    }
}