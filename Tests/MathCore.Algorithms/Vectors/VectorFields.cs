#nullable enable
using System.Runtime.InteropServices;

using MathCore.Vectors;
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleOther

// ReSharper disable InconsistentNaming

namespace MathCore.Algorithms.Vectors;

public static class VectorFields
{


    public static void Test()
    {
        const double f0 = 3; // GHz;
        const double w = 2 * Math.PI * f0;
        const double c = 0.3;
        const double lambda = c / f0; // m
        const double k = 2 * Math.PI / lambda;


    }
}

public static class FieldComponentEx
{
    public static FieldComponent? dFdx(this FieldComponent? F, double dx) => F is null
    ? null
    : (in Vector3D r, double t) =>
    {
        var dx2 = dx / 2;
        return (F(r with { X = r.X - dx2 }, t) - F(r with { X = r.X + dx2 }, t)) / dx;
    };

    public static FieldComponent? dFdy(this FieldComponent? F, double dy) => F is null
    ? null
    : (in Vector3D r, double t) =>
    {
        var dy2 = dy / 2;
        return (F(r with { Y = r.Y - dy2 }, t) - F(r with { Y = r.Y + dy2 }, t)) / dy;
    };

    public static FieldComponent? dFdz(this FieldComponent? F, double dz) => F is null
    ? null
    : (in Vector3D r, double t) =>
    {
        var dz2 = dz / 2;
        return (F(r with { Z = r.Z - dz2 }, t) - F(r with { Z = r.Z + dz2 }, t)) / dz;
    };

    public static FieldComponent? Add(this FieldComponent? X, FieldComponent? Y) => (X, Y) switch
    {
        (null, null) => null,
        (null, _) => (in Vector3D r, double t) => Y(r, t),
        (_, null) => X,
        _ => (in Vector3D r, double t) => X(r, t) + Y(r, t),
    };

    public static FieldComponent? Subtract(this FieldComponent? X, FieldComponent? Y) => (X, Y) switch
    {
        (null, null) => null,
        (null, _) => (in Vector3D r, double t) => -Y(r, t),
        (_, null) => X,
        _ => (in Vector3D r, double t) => X(r, t) - Y(r, t),
    };
}

public delegate double FieldComponent(in Vector3D r, double t);

public readonly record struct Field
{
    public static FieldComponent Zero { get; } = (in Vector3D r, double t) => 0;
    public static FieldComponent One { get; } = (in Vector3D r, double t) => 1;

    public static Field Grad(FieldComponent f, double dr) => new()
    {
        X = f.dFdx(dr),
        Y = f.dFdy(dr),
        Z = f.dFdz(dr),
    };

    public static FieldComponent? Div(in Field F, double dr) => F.X.dFdx(dr).Add(F.Y.dFdy(dr)).Add(F.Z.dFdz(dr));

    public static Field Rot(in Field F, double dr) => new()
    {
        X = F.Z.dFdy(dr).Subtract(F.Y.dFdz(dr)),
        Y = F.X.dFdz(dr).Subtract(F.Z.dFdx(dr)),
        Z = F.Y.dFdx(dr).Subtract(F.X.dFdy(dr)),
    };

    public FieldComponent? X { get; init; }
    public FieldComponent? Y { get; init; }
    public FieldComponent? Z { get; init; }

    public Vector3D Value(in Vector3D r, double t)
    {
        var x = X is { } fx ? fx(r, t) : 0;
        var y = Y is { } fy ? fy(r, t) : 0;
        var z = Z is { } fz ? fz(r, t) : 0;

        return new(x, y, z);
    }

    public double ValueX(in Vector3D r, double t) => X is { } fx ? fx(r, t) : 0;
    public double ValueY(in Vector3D r, double t) => Y is { } fy ? fy(r, t) : 0;
    public double ValueZ(in Vector3D r, double t) => Z is { } fz ? fz(r, t) : 0;

    public FieldComponent? Div(double dr) => Div(this, dr);

    public Field Rot(double dr) => Rot(this, dr);
}