using MathCore;
using MathCore.Vectors;

// ReSharper disable once CheckNamespace
namespace System.Drawing
{
    public static class PointsEx
    {
        public static Point ValueToScreen(this Vector2D Value, Size FieldSize, Interval X, Interval Y)
        {
            var (w, h) = FieldSize;
            var (value_x, value_y) = Value;
            return ScreenPoint(X, Y, w, h, value_x, value_y);
        }

        private static Point ScreenPoint(Interval X, Interval Y, int Width, int Height, double ValueX, double ValueY) =>
            new(
                (int)((ValueX - X.Min) * Width / X.Length),
                Height - (int)Math.Round((ValueY - Y.Min) * Height / Y.Length)
            );

        public static Point ValueToScreen(this PointF Value, Size FieldSize, Interval X, Interval Y)
        {
            var (w, h) = FieldSize;
            var (value_x, value_y) = Value;
            return ScreenPoint(X, Y, w, h, value_x, value_y);
        }

        public static PointF ScreenToValue(this Point Value, Size FieldSize, Interval X, Interval Y)
        {
            var (w, h) = FieldSize;
            var (value_x, value_y) = Value;
            return ScreenPoint(X, Y, w, h, value_x, value_y);
        }

        public static void Deconstruct(this Point point, out int x, out int y)
        {
            x = point.X;
            y = point.Y;
        } 

        public static void Deconstruct(this PointF point, out float x, out float y)
        {
            x = point.X;
            y = point.Y;
        }

        public static double LengthTo(this Point start, Point end)
        {
            var x = start.X - end.X;
            var y = start.Y - end.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public static Point Add(this Point start, Point end) => new(start.X + end.X, start.Y + end.Y);

        public static Point Subtract(this Point start, Point end) => new(start.X - end.X, start.Y - end.Y);
    }
}