using System;
using MathCore.Annotations;

namespace MathCore.Vectors
{
    /// <summary>Двумерный базис</summary>
    public readonly struct Basis2D
    {
        /// <summary>Базис Эвклидова пространства</summary>
        public static readonly Basis2D Evclid = new Basis2D(
            1, 0,
            0, 1);

        public static Basis2D Rotate(double Angle, bool Positive = true) =>
            new Basis2D(
                Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle),
                (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle));

        public static Basis2D Scale(double kx, double ky) =>
            new Basis2D(
                kx, 0,
                0, ky);

        private readonly double _xx;
        public double xx => _xx;

        private readonly double _xy;
        public double xy => _xy;

        private readonly double _yx;
        public double yx => _yx;

        private readonly double _yy;
        public double yy => _yy;

        public Basis2D(double xx, double xy, double yx, double yy)
        {
            _xx = xx;
            _xy = xy;
            _yx = yx;
            _yy = yy;
        }

        public static implicit operator Matrix(in Basis2D b) => new Matrix(new[,] { { b._xx, b._xy }, { b._yx, b._yy } });

        public static explicit operator Basis2D([NotNull] in Matrix M) => new Basis2D(M[0, 0], M[0, 1], M[1, 0], M[1, 1]);
    }
}