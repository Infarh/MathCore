using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MathCore.Vectors
{
    /// <summary>Трёхмерный вектор</summary>
    [TypeConverter(typeof(Vector3DConverter))]
    public partial struct Vector3D : IEquatable<Vector3D>, ICloneable<Vector3D>, IFormattable
    {
        /* -------------------------------------------------------------------------------------------- */

        [DebuggerStepThrough]
        public static Vector3D RThettaPhi(double R, double Thetta, double Phi) => new Vector3D(R, new SpaceAngle(Thetta, Phi));

        [DebuggerStepThrough]
        public static Vector3D XYZ(double X, double Y, double Z) => new Vector3D(X, Y, Z);

        public static Vector3D Random(double min = -100, double max = 100)
        {
            var rnd = new Random();
            Func<double> RND = () => Math.Abs(max - min) * (rnd.NextDouble() - .5) + (max + min) * .5;
            return new Vector3D(RND(), RND(), RND());
        }

        /* -------------------------------------------------------------------------------------------- */

        public static readonly Vector3D Empty = new Vector3D();

        /// <summary>Единичный базисный вектор</summary>
        public static readonly Vector3D BasysUnitVector = new Vector3D(1, 1, 1);

        /// <summary>Базисный вектор i</summary>
        public static readonly Vector3D i = new Vector3D(1, 0, 0);

        /// <summary>Базисный вектор j</summary>
        public static readonly Vector3D j = new Vector3D(0, 1, 0);

        /// <summary>Базисный вектор k</summary>
        public static readonly Vector3D k = new Vector3D(0, 0, 1);

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Длина по оси X</summary>
        private readonly double _X;

        /// <summary>Длина по оси Y</summary>
        private readonly double _Y;

        /// <summary>Длина по оси Z</summary>
        private readonly double _Z;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Длина по оси X</summary>
        [XmlAttribute]
        public double X => _X;

        /// <summary>Длина по оси Y</summary>
        [XmlAttribute]
        public double Y => _Y;

        /// <summary>Длина по оси Z</summary>
        [XmlAttribute]
        public double Z => _Z;

        /// <summary>Длина вектора</summary>
        [XmlIgnore]
        public double R => Math.Sqrt(_X * _X + _Y * _Y + _Z * _Z);

        /// <summary>Угол проекции в плоскости XOY</summary>
        public double AngleXOY => Math.Abs(_X) < double.Epsilon
                    ? (Math.Abs(_Y) < double.Epsilon       // X == 0
                                ? 0                                 //  Y == 0 => 0
                                : Math.Sign(_Y) * Consts.pi05)     //  Y != 0 => pi/2 * sign(Y)
                    : (Math.Abs(_Y) < double.Epsilon       // X != 0
                                ? (Math.Sign(_X) > 0
                                            ? 0
                                            : Consts.pi)
                                : Math.Atan2(_Y, _X));

        /// <summary>Угол проекции в плоскости XOZ</summary>
        public double AngleXOZ => Math.Abs(_X) < double.Epsilon
                    ? (Math.Abs(_Z) < double.Epsilon       // X == 0
                                ? 0                                 //  Z == 0 => 0
                                : Math.Sign(_Z) * Consts.pi05)     //  Z != 0 => pi/2 * sign(Z)
                    : (Math.Abs(_Z) < double.Epsilon       // X != 0
                                ? (Math.Sign(_X) > 0
                                            ? 0
                                            : Consts.pi)
                                : Math.Atan2(_Z, _X));

        /// <summary>Угол проекции в плоскости YOZ</summary>
        public double AngleYOZ => Math.Abs(_Y) < double.Epsilon
                    ? (Math.Abs(_Z) < double.Epsilon       // Y == 0
                                ? 0                                 //  Z == 0 => 0
                                : Math.Sign(_Z) * Consts.pi05)     //  Z != 0 => pi/2 * sign(Y)
                    : (Math.Abs(_Z) < double.Epsilon       // Y != 0
                                ? (Math.Sign(_Y) > 0
                                            ? 0
                                            : Consts.pi)
                                : Math.Atan2(_Z, _Y));

        /// <summary>Азимутальный угол</summary>
        [XmlIgnore]
        public double Phi => AngleXOY;

        /// <summary>Угол места</summary>
        [XmlIgnore]
        public double Thetta => Math.Atan2(R_XOY, _Z);

        /// <summary>Пространственный угол</summary>
        [XmlIgnore]
        public SpaceAngle Angle => new SpaceAngle(Thetta, Phi);

        /// <summary>Двумерный вектор - проекция в плоскости XOY</summary>
        public Vector2D VectorXOY => new Vector2D(_X, _Y);

        /// <summary>Двумерный вектор - проекция в плоскости XOZ (X->X; Z->Y)</summary>
        public Vector2D VectorXOZ => new Vector2D(_X, _Z);

        /// <summary>Двумерный вектор - проекция в плоскости YOZ (Y->X; Z->Y)</summary>
        public Vector2D VectorYOZ => new Vector2D(_Y, _Z);

        /// <summary>Длина в плоскости XOY</summary>
        public double R_XOY => Math.Sqrt(_X * _X + _Y * _Y);

        /// <summary>Длина в плоскости XOZ</summary>
        public double R_XOZ => Math.Sqrt(_X * _X + _Z * _Z);

        /// <summary>Длина в плоскости YOZ</summary>
        public double R_YOZ => Math.Sqrt(_Y * _Y + _Z * _Z);

        public Vector3D Abs => new Vector3D(Math.Abs(_X), Math.Abs(_Y), Math.Abs(_Z));

        public Vector3D Sign => new Vector3D(Math.Sign(_X), Math.Sign(_Y), Math.Sign(_Z));

        /* -------------------------------------------------------------------------------------------- */

        [DebuggerStepThrough]
        public Vector3D(double X) { _X = X; _Y = 0; _Z = 0; }


        [DebuggerStepThrough]
        public Vector3D(double X, double Y) { _X = X; _Y = Y; _Z = 0; }

        [DebuggerStepThrough]
        public Vector3D(double X, double Y, double Z) { _X = X; _Y = Y; _Z = Z; }

        //[System.Diagnostics.DebuggerStepThrough]
        public Vector3D(in SpaceAngle Angle) : this(1, Angle) { }


        //[System.Diagnostics.DebuggerStepThrough]
        public Vector3D(double R, in SpaceAngle Angle)
        {
            double thetta;
            double phi;
            if (Angle.AngleType == AngleType.Deg)
            {
                thetta = Angle.InRad.Thetta;
                phi = Angle.InRad.Phi;
            }
            else
            {
                thetta = Angle.Thetta;
                phi = Angle.Phi;
            }

            _Z = R * Math.Cos(thetta);
            var r = R * Math.Sin(thetta);
            _X = r * Math.Cos(phi);
            _Y = r * Math.Sin(phi);
        }
        private Vector3D(in Vector3D V) : this(V._X, V._Y, V._Z) { }

        //[System.Diagnostics.DebuggerStepThrough]
        public Vector3D InBasis(in Basis3D b) => new Vector3D(
            b.xx * _X + b.xy * _Y + b.xz * _Z,
            b.yx * _X + b.yy * _Y + b.yz * _Z,
            b.zx * _X + b.zy * _Y + b.zz * _Z);

        public Vector3D Inc(double dx, double dy, double dz) => new Vector3D(_X + dx, _Y + dy, _Z + dz);

        public Vector3D Inc_X(double dx) => new Vector3D(_X + dx, _Y, _Z);
        public Vector3D Inc_Y(double dy) => new Vector3D(_X, _Y + dy, _Z);
        public Vector3D Inc_Z(double dz) => new Vector3D(_X, _Y, _Z + dz);

        public Vector3D Dec(double dx, double dy, double dz) => new Vector3D(_X - dx, _Y - dy, _Z - dz);

        public Vector3D Dec_X(double dx) => new Vector3D(_X - dx, _Y, _Z);
        public Vector3D Dec_Y(double dy) => new Vector3D(_X, _Y - dy, _Z);
        public Vector3D Dec_Z(double dz) => new Vector3D(_X, _Y, _Z - dz);


        public Vector3D Scale_X(double kx) => new Vector3D(_X * kx, _Y, _Z);
        public Vector3D Scale_Y(double ky) => new Vector3D(_X, _Y * ky, _Z);
        public Vector3D Scale_Z(double kz) => new Vector3D(_X, _Y, _Z * kz);

        public Vector3D Scale(double kx, double ky, double kz) => new Vector3D(_X * kx, _Y * ky, _Z * kz);

        /* -------------------------------------------------------------------------------------------- */

        [DebuggerStepThrough]
        public override string ToString() => $"({_X};{_Y};{_Z})";

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            unchecked
            {
                var result = _X.GetHashCode();
                result = (result * 397) ^ _Y.GetHashCode();
                result = (result * 397) ^ _Z.GetHashCode();
                return result;
            }
        }

        /// <summary>Создает новый объект, который является копией текущего экземпляра.</summary>
        /// <returns>Новый объект, являющийся копией этого экземпляра.</returns><filterpriority>2</filterpriority>
        [DebuggerStepThrough]
        object ICloneable.Clone() => Clone();

        [DebuggerStepThrough]
        public Vector3D Clone() => new Vector3D(this);

        [DebuggerStepThrough]
        public override bool Equals(object obj) => obj is Vector3D && Equals((Vector3D)obj);

        [DebuggerStepThrough]
        public string ToString(string Format) => $"({_X.ToString(Format)};{_Y.ToString(Format)};{_Z.ToString(Format)})";

        [DebuggerStepThrough]
        public string ToString(string Format, IFormatProvider Provider) => $"({_X.ToString(Format, Provider)};{_Y.ToString(Format, Provider)};{_Z.ToString(Format, Provider)})";

        public void Deconstruct(double x, double y, double z)
        {
            x = _X;
            y = _Y;
            z = _Z;
        }

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<Vector3D> Members

        /// <summary>Точность сравнения (по умолчанию 10^-16)</summary>
        public static double ComparisonsAccuracy { get; set; } = 1e-16;

        //[System.Diagnostics.DebuggerStepThrough]
        public bool Equals(Vector3D other)
        {
            var eps = ComparisonsAccuracy;
            return Math.Abs(other._X - _X) < eps
                   && Math.Abs(other._Y - _Y) < eps
                   && Math.Abs(other._Z - _Z) < eps;
        }

        #endregion

        /* -------------------------------------------------------------------------------------------- */
    }
}
