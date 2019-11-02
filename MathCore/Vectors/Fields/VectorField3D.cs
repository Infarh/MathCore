using System;
using MathCore.Annotations;

namespace MathCore.Vectors.Fields
{
    public delegate Vector3D VectorField3D(Vector3D r);

    public static class VectorField3DExtensions
    {
        [NotNull] public static VectorField3D GetVectorField(Func<double, double> fx, Func<double, double> fy, Func<double, double> fz) => r => new Vector3D(fx(r.X), fy(r.Y), fz(r.Z));
    }
}