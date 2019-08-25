using System;
using MathCore.Vectors;

namespace MathCore.Interpolation
{
    public class Maping : IInterpolator
    {
        /* ------------------------------------------------------------------------------------------ */

        public static double GetValue(double x, double x0, double y0, double x1, double y1) => (y1 - y0) / (x1 - x0) * (x - x0) + y0;

        /* ------------------------------------------------------------------------------------------ */

        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }

        public Vector2D P1 { get => new Vector2D(X0, Y0);
            set { X0 = value.X; Y0 = value.Y; } }
        public Vector2D P2 { get => new Vector2D(X1, Y1);
            set { X1 = value.X; Y1 = value.Y; } }

        public double this[double x] => Value(x);

        /* ------------------------------------------------------------------------------------------ */

        public Maping() { }

        public Maping(double x0, double y0, double x1, double y1)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
        }

        public Maping(Vector2D p1, Vector2D p2) : this(p1.X, p1.Y, p2.X, p2.Y) { }

        /* ------------------------------------------------------------------------------------------ */

        public double Value(double x) => GetValue(x, X0, Y0, X1, Y1);

        public Func<double, double> GetFunction() => Value;

        /* ------------------------------------------------------------------------------------------ */

        public static implicit operator Func<double, double>(Maping Interpolator) => Interpolator.Value;

        /* ------------------------------------------------------------------------------------------ */
    }
}
