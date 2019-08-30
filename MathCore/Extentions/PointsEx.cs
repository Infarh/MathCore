using MathCore;
using MathCore.Vectors;

namespace System.Drawing
{
    public static class PointsEx
    {
        public static Point ValueToScreen(this Vector2D v, Size FieldSize, Interval X, Interval Y)
        {
            var h = FieldSize.Height;
            return new Point
                (
                    (int)((v.X - X.Min) * FieldSize.Width / X.Length),
                    h - (int)Math.Round((v.Y - Y.Min) * h / Y.Length)
                );
        }

        public static Point ValueToScreen(this PointF v, Size FieldSize, Interval X, Interval Y)
        {
            var h = FieldSize.Height;
            return new Point
                (
                    (int)((v.X - X.Min) * FieldSize.Width / X.Length),
                    h - (int)Math.Round((v.Y - Y.Min) * h / Y.Length)
                );
        }

        public static PointF ScreenToValue(this Point p, Size FieldSize, Interval X, Interval Y)
        {
            var h = FieldSize.Height;
            return new PointF
                (
                    (float)(p.X * X.Length / FieldSize.Width + X.Min),
                    (float)((h - p.Y) * Y.Length / h + Y.Min)
                );
        }
    }
}
