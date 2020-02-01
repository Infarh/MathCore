using System;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace MathCore.Vectors
{
    /// <summary>Базис трёхмерного пространства</summary>
    public readonly struct Basis3D : IEquatable<Basis3D>
    {
        /// <summary>Базис Евклидова пространства</summary>
        public static readonly Basis3D Euclid = new Basis3D(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        /// <summary>Базис масштабирования по осям</summary>
        /// <param name="kx">Коэффициент масштабирования вдоль оси OX</param>
        /// <param name="ky">Коэффициент масштабирования вдоль оси OY</param>
        /// <param name="kz">Коэффициент масштабирования вдоль оси OZ</param>
        /// <returns>Базис масштабирования вдоль основных осей</returns>
        public static Basis3D Scale(double kx, double ky, double kz) =>
            new Basis3D(
                kx, 0, 0,
                0, ky, 0,
                0, 0, kz);

        /// <summary>Базис поворота вокруг оси OX</summary>
        /// <param name="Angle">Угол поворота</param>
        /// <param name="Positive">Направление - по часовой стрелке</param>
        /// <returns>Базис поворота вокруг оси OX</returns>
        public static Basis3D RotateOX(double Angle, bool Positive = true) =>
            new Basis3D(
                1, 0, 0,
                0, Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle),
                0, (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle));

        /// <summary>Базис поворота вокруг оси OY</summary>
        /// <param name="Angle">Угол поворота</param>
        /// <param name="Positive">Направление - по часовой стрелке</param>
        /// <returns>Базис поворота вокруг оси OY</returns>
        public static Basis3D RotateOY(double Angle, bool Positive = true) =>
            new Basis3D(
                Math.Cos(Angle), 0, (Positive ? -1 : 1) * Math.Sin(Angle),
                0, 1, 0,
                (Positive ? 1 : -1) * Math.Sin(Angle), 0, Math.Cos(Angle));

        /// <summary>Базис поворота вокруг оси OZ</summary>
        /// <param name="Angle">Угол поворота</param>
        /// <param name="Positive">Направление - по часовой стрелке</param>
        /// <returns>Базис поворота вокруг оси OZ</returns>
        public static Basis3D RotateOZ(double Angle, bool Positive = true) =>
            new Basis3D(
                Math.Cos(Angle), (Positive ? -1 : 1) * Math.Sin(Angle), 0,
                (Positive ? 1 : -1) * Math.Sin(Angle), Math.Cos(Angle), 0,
                0, 0, 1);

        /// <summary>Базис поворота на углы Эйлера</summary>
        /// <param name="alpha">Угол поворота вокруг оси OX</param>
        /// <param name="beta">Угол поворота вокруг оси OY</param>
        /// <param name="gamma">Угол поворота вокруг оси OZ</param>
        /// <returns></returns>
        public static Basis3D Rotate(double alpha, double beta, double gamma)
        {
            var sin_a = Math.Sin(alpha);
            var cos_a = Math.Cos(alpha);

            var sin_b = Math.Sin(beta);
            var cos_b = Math.Cos(beta);

            var sin_g = Math.Sin(gamma);
            var cos_g = Math.Cos(gamma);

            return new Basis3D(
                        cos_a * cos_b, cos_g * sin_b * sin_g - sin_a * cos_g, cos_a * sin_b * cos_g + sin_a * sin_g,
                        sin_a * cos_b, sin_a * sin_b * sin_g + cos_a * cos_g, sin_a * sin_b * cos_g - cos_a * sin_g,
                        -sin_b, cos_b * sin_g, cos_b * cos_g);
        }

        /// <summary>Базис сдвига и поворота на заданный угол</summary>
        /// <param name="v">Вектор сдвига</param>
        /// <param name="theta">Угол места поворота</param>
        /// <returns>Базис сдвига и поворота</returns>
        public static Basis3D Rotate(in Vector3D v, double theta)
        {
            var sin_t = Math.Sin(theta);
            var cos_t = Math.Cos(theta);
            var cos_t1 = 1 - cos_t;

            var (x, y, z) = v;

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

#pragma warning disable IDE1006 // Стили именования

        /// <summary>Элемент [0,0]</summary>
        public double xx => _xx;
        
        /// <summary>Элемент [0,1]</summary>
        public double xy => _xy;
        
        /// <summary>Элемент [0,2]</summary>
        public double xz => _xz;

        
        /// <summary>Элемент [1,0]</summary>
        public double yx => _yx;
        
        /// <summary>Элемент [1,1]</summary>
        public double yy => _yy;
        
        /// <summary>Элемент [1,2]</summary>
        public double yz => _yz;

        
        /// <summary>Элемент [2,0]</summary>
        public double zx => _zx;
        
        /// <summary>Элемент [2,1]</summary>
        public double zy => _zy;
        
        /// <summary>Элемент [2,2]</summary>
        public double zz => _zz;

#pragma warning restore IDE1006 // Стили именования

        /// <summary>Инициализация нового экземпляра <see cref="Basis3D"/></summary>
        /// <param name="xx">Элемент X[0,0]</param>
        /// <param name="xy">Элемент X[0,1]</param>
        /// <param name="xz">Элемент X[0,2]</param>
        /// <param name="yx">Элемент X[1,0]</param>
        /// <param name="yy">Элемент X[1,1]</param>
        /// <param name="yz">Элемент X[1,2]</param>
        /// <param name="zx">Элемент X[2,0]</param>
        /// <param name="zy">Элемент X[2,1]</param>
        /// <param name="zz">Элемент X[2,2]</param>
        public Basis3D(double xx, double xy, double xz, double yx, double yy, double yz, double zx, double zy, double zz)
        {
            _xx = xx; _xy = xy; _xz = xz;
            _yx = yx; _yy = yy; _yz = yz;
            _zx = zx; _zy = zy; _zz = zz;
        }

        /// <summary>Оператор неявного преобразования базиса в матрицу 3х3</summary>
        [NotNull]
        public static implicit operator Matrix(in Basis3D b) =>
            new Matrix(new[,]
            {
                { b._xx, b._xy, b._xz },
                { b._yx, b._yy, b._yz },
                { b._zx, b._zy, b._zz }
            });

        /// <summary>Оператор явного преобразования матрицы 2х2 в двумерный базис</summary>
        public static explicit operator Basis3D([NotNull] Matrix M) =>
            new Basis3D(
                M[0, 0], M[0, 1], M[0, 2],
                M[1, 0], M[1, 1], M[1, 2],
                M[2, 0], M[2, 1], M[2, 2]);

        /// <inheritdoc />
        public bool Equals(Basis3D other) => _xx.Equals(other._xx) && _xy.Equals(other._xy) && _xz.Equals(other._xz) && _yx.Equals(other._yx) && _yy.Equals(other._yy) && _yz.Equals(other._yz) && _zx.Equals(other._zx) && _zy.Equals(other._zy) && _zz.Equals(other._zz);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Basis3D other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code = _xx.GetHashCode();
                hash_code = (hash_code * 397) ^ _xy.GetHashCode();
                hash_code = (hash_code * 397) ^ _xz.GetHashCode();
                hash_code = (hash_code * 397) ^ _yx.GetHashCode();
                hash_code = (hash_code * 397) ^ _yy.GetHashCode();
                hash_code = (hash_code * 397) ^ _yz.GetHashCode();
                hash_code = (hash_code * 397) ^ _zx.GetHashCode();
                hash_code = (hash_code * 397) ^ _zy.GetHashCode();
                hash_code = (hash_code * 397) ^ _zz.GetHashCode();
                return hash_code;
            }
        }

        /// <summary>Оператор равенства двух базисов</summary>
        public static bool operator ==(Basis3D left, Basis3D right) => left.Equals(right);

        /// <summary>Оператор неравенства двух базисов</summary>
        public static bool operator !=(Basis3D left, Basis3D right) => !left.Equals(right);
    }
}