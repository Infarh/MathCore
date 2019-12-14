using System;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors
{
    public readonly struct Basis3D
    {
        public static readonly Basis3D Evclid = new Basis3D(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public static Basis3D Scale(double kx, double ky, double kz) =>
            new Basis3D(
                kx, 0, 0,
                0, ky, 0,
                0, 0, kz);

        public static Basis3D RotateOX(double Angle, bool Positive = true) =>
            new Basis3D(
                1, 0, 0,
                0, Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle),
                0, (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle));

        public static Basis3D RotateOY(double Angle, bool Positive = true) =>
            new Basis3D(
                Math.Cos(Angle), 0, (Positive ? -1 : 1) * Math.Sin(Angle),
                0, 1, 0,
                (Positive ? 1 : -1) * Math.Sin(Angle), 0, Math.Cos(Angle));

        public static Basis3D RotateOZ(double Angle, bool Positive = true) =>
            new Basis3D(
                Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle), 0,
                (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle), 0,
                0, 0, 1);

        public static Basis3D Rotate(double alpha, double betta, double gamma)
        {
            var sin_a = Math.Sin(alpha);
            var cos_a = Math.Cos(alpha);

            var sin_b = Math.Sin(betta);
            var cos_b = Math.Cos(betta);

            var sin_g = Math.Sin(gamma);
            var cos_g = Math.Cos(gamma);

            return new Basis3D(
                        cos_a * cos_b, cos_g * sin_b * sin_g - sin_a * cos_g, cos_a * sin_b * cos_g + sin_a * sin_g,
                        sin_a * cos_b, sin_a * sin_b * sin_g + cos_a * cos_g, sin_a * sin_b * cos_g - cos_a * sin_g,
                        -sin_b, cos_b * sin_g, cos_b * cos_g);
        }

        public static Basis3D Rotate(in Vector3D v, double theta)
        {
            var sin_t = Math.Sin(theta);
            var cos_t = Math.Cos(theta);
            var cos_t1 = 1 - cos_t;

            var x = v.X;
            var y = v.Y;
            var z = v.Z;

            var xy = x * y;
            var xz = x * z;
            var yz = y * z;

            var sin_tx = sin_t * x;
            var sin_ty = sin_t * y;
            var sin_tz = sin_t * z;

            return new Basis3D(
                cos_t + cos_t1 * x * x, cos_t1 * xy - sin_tz, cos_t1 * xz + sin_ty,
                cos_t1 * xy + sin_tz, cos_t + cos_t1 * y * y, cos_t1 * yz - sin_tx,
                cos_t1 * xz - sin_ty, cos_t1 * yz + sin_tx, cos_t + cos_t1 * z * z);
        }

        private readonly double _xx;
        private readonly double _xy;
        private readonly double _xz;
        private readonly double _yx;
        private readonly double _yy;
        private readonly double _yz;
        private readonly double _zx;
        private readonly double _zy;
        private readonly double _zz;

        public double xx => _xx;
        public double xy => _xy;
        public double xz => _xz;

        public double yx => _yx;
        public double yy => _yy;
        public double yz => _yz;

        public double zx => _zx;
        public double zy => _zy;
        public double zz => _zz;

        public Basis3D(double xx, double xy, double xz, double yx, double yy, double yz, double zx, double zy, double zz)
        {
            _xx = xx; _xy = xy; _xz = xz;
            _yx = yx; _yy = yy; _yz = yz;
            _zx = zx; _zy = zy; _zz = zz;
        }

        [NotNull]
        public static implicit operator Matrix(in Basis3D b) =>
            new Matrix(new[,]
            {
                { b._xx, b._xy, b._xz },
                { b._yx, b._yy, b._yz },
                { b._zx, b._zy, b._zz }
            });

        public static explicit operator Basis3D([NotNull] Matrix M) =>
            new Basis3D(
                M[0, 0], M[0, 1], M[0, 2],
                M[1, 0], M[1, 1], M[1, 2],
                M[2, 0], M[2, 1], M[2, 2]);
    }
}