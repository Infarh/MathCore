
using System;

namespace MathCore.Vectors.Feelds
{
    public delegate Vector3D VectorFeeld3D(Vector3D r);

    public static class VectorFeeld3DExtentions
    {
        public static VectorFeeld3D GetVectorFeeld(Func<double, double> fx, Func<double, double> fy, Func<double, double> fz)
        {
            return r => new Vector3D(fx(r.X), fy(r.Y), fz(r.Z));
        }
    }
}
