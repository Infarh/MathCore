
using System;
using System.Diagnostics.Contracts;
using MathCore.Annotations;

namespace MathCore.Vectors
{
    public struct Basis2D
    {
        public static readonly Basis2D Evclid = new Basis2D(
            1, 0,
            0, 1);

        public static Basis2D Rotate(double Angle, bool Positive = true)
        {
            return new Basis2D(
                Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle),
                (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle));
        }

        public static Basis2D Scale(double kx, double ky)
        {
            return new Basis2D(
                kx, 0,
                0, ky);
        }

        private double _xx;
        public double xx { get => _xx; set => _xx = value; }

        private double _xy; 
        public double xy { get => _xy; set => _xy = value; }

        private double _yx;
        public double yx { get => _yx; set => _yx = value; }

        private double _yy;
        public double yy { get => _yy; set => _yy = value; }

        public Basis2D(double xx, double xy, double yx, double yy)
        {
            _xx = xx;
            _xy = xy;
            _yx = yx;
            _yy = yy;
        }

        public static implicit operator Matrix(in Basis2D b)
        {
            Contract.Ensures(Contract.Result<Matrix>().M == 2, "Число столбцов матрицы = 2");
            Contract.Ensures(Contract.Result<Matrix>().N == 2, "Число строк матрицы = 2");

            return new Matrix(new[,] { { b._xx, b._xy }, { b._yx, b._yy } });
        }

        public static explicit operator Basis2D([NotNull] in Matrix M)
        {
            Contract.Requires(M.M == 2 && M.N == 2, "Матрица должна быть размера 2х2");

            //if(M.M != 2 || M.N != 2)
            //    throw new ArgumentException("Матрица должна быть размера 2х2");
            //Contract.EndContractBlock();


            return new Basis2D(
                M[0, 0], M[0, 1],
                M[1, 0], M[1, 1]);
        }
    }
}
