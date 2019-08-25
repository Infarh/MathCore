using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.Vectors;

namespace MathCore
{
    public abstract class TransformationMatrix : Matrix
    {
        protected TransformationMatrix(double[,] Data) : base(Data) { }
    }

    public abstract class Transformation3DMatrix : TransformationMatrix
    {
        protected Transformation3DMatrix(double[,] Data) : base(Data)
        {
            Contract.Requires(Data.GetLength(0) == 3);
            Contract.Requires(Data.GetLength(1) == 3);
            Contract.Ensures(M == 3);
            Contract.Ensures(N == 3);
        }

        [ContractInvariantMethod]
        protected void ObjectInvariant()
        {
            Contract.Invariant(M == 3);
            Contract.Invariant(N == 3);
        }
    }

    public class Roration3DMatrix : Transformation3DMatrix
    {
        public enum RotationAxe { X, Y, Z }

        private static double[,] GetData(double Angle, RotationAxe Axe)
        {
            Contract.Ensures(Contract.Result<double[,]>().GetLength(0) == 3);
            Contract.Ensures(Contract.Result<double[,]>().GetLength(1) == 3);

            var s = Math.Sin(Angle);
            var c = Math.Cos(Angle);

            switch(Axe)
            {
                default: throw new ArgumentOutOfRangeException(nameof(Axe), Axe, null);
                case RotationAxe.X:
                    return new[,]
                    {
                        {1, 0, 0},
                        {0, c, -s},
                        {0, s, c}
                    };
                case RotationAxe.Y:
                    return new[,]
                    {
                        {c, 0, s},
                        {0, 1, 0},
                        {-s, 0, c}
                    };
                case RotationAxe.Z:
                    return new[,]
                    {
                        {c, -s, 0},
                        {s, c, 0},
                        {0, 0, 1}
                    };
            }
        }

        public RotationAxe Axe { get; }

        public Roration3DMatrix(double Angle, RotationAxe Axe) : base(GetData(Angle, Axe)) => this.Axe = Axe;

        public static Vector3D operator ^(Roration3DMatrix M, Vector3D v)
        {
            var m = M.GetData();
            var x = v.X;
            var y = v.Y;
            var z = v.Z;
            return new Vector3D
                (
                    m[0, 0] * x + m[0, 1] * y + m[0, 2] * z,
                    m[1, 0] * x + m[1, 1] * y + m[1, 2] * z,
                    m[2, 0] * x + m[2, 1] * y + m[2, 2] * z
                );
        }
    }

    public class RotationPhiThettaMatrix : Roration3DMatrix
    {
        public RotationPhiThettaMatrix(double Phi, double Thetta) : base(Phi, RotationAxe.Z)
        {
            var s = Math.Sin(Thetta);
            var c = Math.Cos(Thetta);

            var m = GetData();
            m[0, 2] = m[0, 0] * s;
            m[1, 2] = m[1, 0] * s;
            m[2, 2] = c;
            m[0, 0] *= c;
            m[1, 0] *= c;
            m[2, 0] = -s;
        }
    }

    public class RotationThettaPhiMatrix : Roration3DMatrix
    {
        public RotationThettaPhiMatrix(double Phi, double Thetta) : base(Phi, RotationAxe.Z)
        {
            var s = Math.Sin(Thetta);
            var c = Math.Cos(Thetta);

            var m = GetData();
            m[2, 0] = m[0, 0]*-s;
            m[2, 1] = m[0, 1]* s;
            m[0, 2] = s;
            m[2, 2] = c;
            m[0, 0] *= c;
            m[0, 1] *= c;
        }
    }
}
