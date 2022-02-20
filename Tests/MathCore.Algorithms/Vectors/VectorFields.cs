#nullable enable
using MathCore.Vectors;
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleOther

using Field = System.Func<MathCore.Vectors.Vector3D, double, MathCore.Vectors.Vector3D>;
// ReSharper disable InconsistentNaming

namespace MathCore.Algorithms.Vectors;

public static class VectorFields
{
    public static Vector3D dx(this Field field, Vector3D r, double t, double d) =>
        (field(new(r.X + d / 2, r.Y, r.Z), t) - field(new(r.X - d / 2, r.Y, r.Z), t)) / d;
    public static Vector3D dy(this Field field, Vector3D r, double t, double d) =>
        (field(new(r.X, r.Y + d / 2, r.Z), t) - field(new(r.X, r.Y - d / 2, r.Z), t)) / d;
    public static Vector3D dz(this Field field, Vector3D r, double t, double d) =>
        (field(new(r.X, r.Y, r.Z + d / 2), t) - field(new(r.X, r.Y, r.Z - d / 2), t)) / d;

    public static Func<Vector3D, double, Vector3D> Grad(this Field field, double dr) =>
        (r, t) =>
        {
            return new(field.dx(r, t, dr), field.dy(r, t, dr), field.dz(r, t, dr));
        };

    public static Func<Vector3D, double, double> Div(this Field field, double dr) =>
        (r, t) =>
        {
            var (x, y, z) = r;
            var dr2 = dr / 2;
            var dFdx = (field(new(x + dr2, y, z), t) - field(new(x - dr2, y, z), t)) / dr;
            var dFdy = (field(new(x, y + dr2, z), t) - field(new(x, y - dr2, z), t)) / dr;
            var dFdz = (field(new(x, y, z + dr2), t) - field(new(x, y, z - dr2), t)) / dr;
            return dFdx + dFdy + dFdz;
        };

    public static Func<Vector3D, double, Vector3D> Rot(this Field field, double dr) =>
        (r, t) =>
        {
            var (x, y, z) = r;
            var dr2 = dr / 2;
            var dFdx = (field(new(x + dr2, y, z), t) - field(new(x - dr2, y, z), t)) / dr;
            var dFdy = (field(new(x, y + dr2, z), t) - field(new(x, y - dr2, z), t)) / dr;
            var dFdz = (field(new(x, y, z + dr2), t) - field(new(x, y, z - dr2), t)) / dr;
            return dFdx + dFdy + dFdz;
        };

    public static void Test()
    {
        const double f0 = 3; // GHz;
        const double w = 2 * Math.PI * f0;
        const double c = 0.3;
        const double lambda = c / f0; // m
        const double k = 2 * Math.PI / lambda;

        Field E = (r, t) => new(
            X: 0,
            Y: Math.Sin(k * r.X + w * t),
            Z: 0);
    }
}