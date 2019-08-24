using System;
using System.Diagnostics.Contracts;
using MathCore.Annotations;

namespace MathCore.Vectors
{
    public struct Basis3D
    {
        public static readonly Basis3D Evclid = new Basis3D(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public static Basis3D Scale(double kx, double ky, double kz)
        {
            return new Basis3D(
                kx, 0, 0,
                0, ky, 0,
                0, 0, kz);
        }

        public static Basis3D RotateOX(double Angle, bool Positive = true)
        {
            return new Basis3D(
                1, 0, 0,
                0, Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle),
                0, (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle));
        }

        public static Basis3D RotateOY(double Angle, bool Positive = true)
        {
            return new Basis3D(
                Math.Cos(Angle), 0, (Positive ? -1 : 1) * Math.Sin(Angle),
                0, 1, 0,
                (Positive ? 1 : -1) * Math.Sin(Angle), 0, Math.Cos(Angle));
        }

        public static Basis3D RotateOZ(double Angle, bool Positive = true)
        {
            Func<double, double> sin1 = Math.Sin;
            Func<double, double> cos1 = Math.Cos;
            return new Basis3D(
                cos1(Angle), (Positive ? -1 : 1) * sin1(Angle), 0,
                (Positive ? 1 : -1) * sin1(Angle), cos1(Angle), 0,
                0, 0, 1);
        }

        public static Basis3D Rotate(double alpha, double betta, double gamma)
        {
            Func<double, double> sin1 = Math.Sin;
            var sin_a = sin1(alpha);
            Func<double, double> cos1 = Math.Cos;
            var cos_a = cos1(alpha);

            var sin_b = sin1(betta);
            var cos_b = cos1(betta);

            var sin_g = sin1(gamma);
            var cos_g = cos1(gamma);

            return new Basis3D(
                        cos_a * cos_b, cos_g * sin_b * sin_g - sin_a * cos_g, cos_a * sin_b * cos_g + sin_a * sin_g,
                        sin_a * cos_b, sin_a * sin_b * sin_g + cos_a * cos_g, sin_a * sin_b * cos_g - cos_a * sin_g,
                        -sin_b, cos_b * sin_g, cos_b * cos_g);
        }

        public static Basis3D Rotate(in Vector3D v, double thetta)
        {
            var sin_t = ((Func<double, double>)Math.Sin)(thetta);
            var cos_t = ((Func<double, double>)Math.Cos)(thetta);
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

        private double _xx;
        private double _xy;
        private double _xz;
        private double _yx;
        private double _yy;
        private double _yz;
        private double _zx;
        private double _zy;
        private double _zz;

        public double xx { get => _xx; set => _xx = value; }
        public double xy { get => _xy; set => _xy = value; }
        public double xz { get => _xz; set => _xz = value; }

        public double yx { get => _yx; set => _yx = value; }
        public double yy { get => _yy; set => _yy = value; }
        public double yz { get => _yz; set => _yz = value; }

        public double zx { get => _zx; set => _zx = value; }
        public double zy { get => _zy; set => _zy = value; }
        public double zz { get => _zz; set => _zz = value; }

        public Basis3D(double xx, double xy, double xz, double yx, double yy, double yz, double zx, double zy, double zz)
        {
            _xx = xx; _xy = xy; _xz = xz;
            _yx = yx; _yy = yy; _yz = yz;
            _zx = zx; _zy = zy; _zz = zz;
        }

        public static implicit operator Matrix(in Basis3D b)
        {
            Contract.Ensures(Contract.Result<Matrix>().M == 3, "Число столбцов матрицы = 3");
            Contract.Ensures(Contract.Result<Matrix>().N == 3, "Число строк матрицы = 3");

            //var M = new Matrix(3);
            //M[0, 0] = b._xx; M[0, 1] = b._xy; M[0, 2] = b._xz;
            //M[1, 0] = b._yx; M[1, 1] = b._yy; M[1, 2] = b._yz;
            //M[2, 0] = b._zx; M[2, 1] = b._zy; M[2, 2] = b._zz;

            //Contract.Ensures(M.IsSquare);
            //Contract.Ensures(M.N == 3);
            //Contract.Ensures(M.M == 3);
            //return M;

            return new Matrix(new[,]
              {
                  { b._xx, b._xy, b._xz },
                  { b._yx, b._yy, b._yz },
                  { b._zx, b._zy, b._zz }
              });
        }

        public static explicit operator Basis3D([NotNull] Matrix M)
        {
            Contract.Requires(M.M == 3 && M.N == 3, "Матрица должна быть размера 3х3");

            //if(M.M != 3 || M.N != 3)
            //    throw new ArgumentException("Матрица должна быть размера 3х3");
            //Contract.EndContractBlock();

            return new Basis3D(
                M[0, 0], M[0, 1], M[0, 2],
                M[1, 0], M[1, 1], M[1, 2],
                M[2, 0], M[2, 1], M[2, 2]);
        }
    }
}