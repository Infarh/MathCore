using System;
using System.ComponentModel;
using System.Xml.Serialization;
using MathCore.Annotations;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors
{
    /// <summary>Двумерный вектор</summary>
    [Serializable]
    [TypeConverter(typeof(Vector2DConverter))]
    public readonly struct Vector2D : IEquatable<Vector2D>, ICloneable<Vector2D>
    {
        /// <summary>Координата X</summary>
        private readonly double _X;

        /// <summary>Координата Y</summary>
        private readonly double _Y;

        /// <summary>Координата X</summary>
        [XmlAttribute]
        public double X => _X;

        /// <summary>Координата Y</summary>
        [XmlAttribute]
        public double Y => _Y;

        /// <summary>Радиус (длина) вектора</summary>
        [XmlIgnore]
        public double R => Math.Sqrt(_X * _X + _Y * _Y);

        /// <summary>Угол к оси X в радианах</summary>
        [XmlIgnore]
        public double Angle =>
            _X.Equals(0)
                ? _Y.Equals(0)
                    ? 0
                    : Math.Sign(_Y) * Consts.pi05
                : _Y.Equals(0)
                    ? Math.Sign(_X) > 0
                        ? 0
                        : Consts.pi
                    : Math.Atan2(_Y, _X);

        /// <summary>Инициализация двумерного вектора</summary>
        /// <param name="X">Координата X</param>
        /// <param name="Y">Координата Y</param>
        public Vector2D(double X, double Y)
        {
            _X = X;
            _Y = Y;
        }

        /// <summary>Инициализация вектора по по комплексному числу</summary>
        /// <param name="Z">Комплексное число X + iY</param>
        private Vector2D(in Complex Z)
        {
            _X = Z.Re;
            _Y = Z.Im;
        }

        /// <summary>Представление вектора в базисе</summary>
        /// <param name="b">Базис</param>
        /// <returns>Вектор в базисе</returns>
        public Vector2D InBasis(in Basis2D b) => new Vector2D(b.xx * _X + b.xy * _Y, b.yx * _X + b.yy * _Y);

        /// <inheritdoc />
        [NotNull]
        public override string ToString() => $"({X};{Y})";

        /// <inheritdoc />
        public Vector2D Clone() => new Vector2D(_X, _Y);

        /// <inheritdoc />
        object ICloneable.Clone() => Clone();

        /// <inheritdoc />
        public bool Equals(Vector2D other) => _X.Equals(other._X) && _Y.Equals(other._Y);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Vector2D v && Equals(v);

        /// <inheritdoc />
        public override int GetHashCode() { unchecked { return (_X.GetHashCode() * 0x18d) ^ _Y.GetHashCode(); } }

        #region Операторы

        /// <summary>Деконструктор вектора на его составляющие</summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        public void Deconstruct(out double x, out double y) { x = _X; y = _Y; }

        /// <summary>Оператор равенства</summary>
        public static bool operator ==(in Vector2D a, in Vector2D b) => a.Equals(b);

        /// <summary>Оператор неравенства</summary>
        public static bool operator !=(in Vector2D a, in Vector2D b) => !(a == b);

        /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
        public static explicit operator Complex(in Vector2D P) => new Complex(P._X, P._Y);

        /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
        public static explicit operator Vector2D(in Complex Z) => new Vector2D(Z);

        /// <summary>Оператор неявного преобразования типа <see cref="double"/> в <see cref="Vector2D"/> (вектор вдоль оси OX)</summary>
        public static implicit operator Vector2D(double x) => new Vector2D(x, 0);

        /// <summary>Унарный оператор +</summary>
        /// <returns>Вектор без изменения</returns>
        public static Vector2D operator +(in Vector2D a) => a;

        /// <summary>Унарный оператор отрицания</summary>
        /// <returns>Вектор, координаты которого имеют противоположенный знак</returns>
        public static Vector2D operator -(in Vector2D a) => new Vector2D(-a._X, -a._Y);

        public static Vector2D operator +(in Vector2D a, in Vector2D b) => new Vector2D(a._X + b._X, a._Y + b._Y);

        public static Vector2D operator -(in Vector2D a, in Vector2D b) => new Vector2D(a._X - b._X, a._Y - b._Y);

        public static double operator *(in Vector2D a, in Vector2D b) => a._X * b._X + a._Y * b._Y;

        public static Vector2D operator +(in Vector2D a, double b) => new Vector2D(a._X + b, a._Y + b);

        public static Vector2D operator -(in Vector2D a, double b) => new Vector2D(a._X - b, a._Y - b);

        public static Vector2D operator *(Vector2D a, double b) => new Vector2D(a._X * b, a._Y * b);

        public static Vector2D operator /(in Vector2D a, double b) => new Vector2D(a._X / b, a._Y / b);

        public static Vector2D operator +(double a, in Vector2D b) => new Vector2D(a + b._X, a + b._Y);
        public static Vector2D operator -(double a, in Vector2D b) => new Vector2D(a - b._X, a - b._Y);

        public static Vector2D operator *(double a, in Vector2D b) => new Vector2D(a * b._X, a * b._Y);

        public static Vector2D operator /(double a, in Vector2D b) => new Vector2D(a / b._X, a / b._Y);

        /// <summary>Оператор проекции в вектора на вектор</summary>
        /// <param name="a">Проецируемый вектор</param>
        /// <param name="b">Вектор проекции</param>
        /// <returns>Проекция первого вектора на второй</returns>
        public static double operator ^(in Vector2D a, in Vector2D b) => a * b / (a.R * b.R);

        public static bool operator |(in Vector2D a, in Vector2D b) => (a ^ b).Equals(1d);

        public static bool operator &(in Vector2D a, in Vector2D b) => (a ^ b).Equals(0d);

        public static double operator %(in Vector2D a, in Vector2D b) => a * b / b.R;

        public static Vector2D operator %(in Vector2D a, in Basis2D b) => a.InBasis(b);

        public static Vector2D operator +(in Vector2D a, float b) => new Vector2D(a._X + b, a._Y + b);

        public static Vector2D operator -(in Vector2D a, float b) => new Vector2D(a._X - b, a._Y - b);

        public static Vector2D operator *(in Vector2D a, float b) => new Vector2D(a._X * b, a._Y * b);

        public static Vector2D operator /(in Vector2D a, float b) => new Vector2D(a._X / b, a._Y / b);

        public static Vector2D operator +(float a, in Vector2D b) => new Vector2D(a + b._X, a + b._Y);

        public static Vector2D operator -(float a, in Vector2D b) => new Vector2D(a - b._X, a - b._Y);

        public static Vector2D operator *(float a, in Vector2D b) => new Vector2D(a * b._X, a * b._Y);

        public static Vector2D operator /(float a, in Vector2D b) => new Vector2D(a / b._X, a / b._Y);

        public static Vector2D operator +(in Vector2D a, int b) => new Vector2D(a._X + b, a._Y + b);

        public static Vector2D operator -(in Vector2D a, int b) => new Vector2D(a._X - b, a._Y - b);

        public static Vector2D operator *(in Vector2D a, int b) => new Vector2D(a._X * b, a._Y * b);

        public static Vector2D operator /(in Vector2D a, int b) => new Vector2D(a._X / b, a._Y / b);

        public static Vector2D operator +(int a, in Vector2D b) => new Vector2D(a + b._X, a + b._Y);
        public static Vector2D operator -(int a, in Vector2D b) => new Vector2D(a - b._X, a - b._Y);

        public static Vector2D operator *(int a, in Vector2D b) => new Vector2D(a * b._X, a * b._Y);

        public static Vector2D operator /(int a, in Vector2D b) => new Vector2D(a / b._X, a / b._Y);

        #endregion
    }
}