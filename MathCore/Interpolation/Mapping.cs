using MathCore.Annotations;
using MathCore.Vectors;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Interpolation;

/// <summary>Линейный интерполятор вещественного значения</summary>
public class Mapping : Interpolator, IInterpolator
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Линейная интерполяция</summary>
    /// <param name="x">Аргумент интерполируемого значения</param>
    /// <param name="x1">Аргумент первого значения</param>
    /// <param name="y1">Первое значение</param>
    /// <param name="x2">Аргумент второго значения</param>
    /// <param name="y2">Второе значение</param>
    /// <returns></returns>
    public static double GetValue(double x, double x1, double y1, double x2, double y2) => Interpolation.Linear.Interpolate(x, x1, y1, x2, y2);

    /* ------------------------------------------------------------------------------------------ */

    public double X1 { get; set; }

    public double Y1 { get; set; }

    public double X2 { get; set; }

    public double Y2 { get; set; }

    public Vector2D P1 { get => new(X1, Y1); set => (X1, Y1) = value; }

    public Vector2D P2 { get => new(X2, Y2); set => (X2, Y2) = value; }

    public double this[double x] => Value(x);

    /* ------------------------------------------------------------------------------------------ */

    public Mapping() { }

    public Mapping(double X1, double Y1, double X2, double Y2)
    {
        this.X1 = X1;
        this.X2 = X2;
        this.Y1 = Y1;
        this.Y2 = Y2;
    }

    public Mapping(Vector2D P1, Vector2D P2)
    {
        this.P1 = P1;
        this.P2 = P2;
    }

    /* ------------------------------------------------------------------------------------------ */

    public double Value(double x) => Interpolation.Linear.Interpolate(x, X1, Y1, X2, Y2);

    [NotNull] public Func<double, double> GetFunction() => Value;

    /* ------------------------------------------------------------------------------------------ */

    [NotNull] public static implicit operator Func<double, double>([NotNull] Mapping Interpolator) => Interpolator.Value;

    /* ------------------------------------------------------------------------------------------ */
}