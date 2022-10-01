#nullable enable
using System;
using System.ComponentModel;
using System.Xml.Serialization;

using static System.Math;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors;

/// <summary>Двумерный вектор</summary>
[Serializable]
[TypeConverter(typeof(Vector2DConverter))]
public readonly struct Vector2D : 
    ICloneable<Vector2D>, 
    IEquatable<Vector2D>,
    IEquatable<(double X, double Y)>,
    IEquatable<(int X, double Y)>,
    IEquatable<(double X, int Y)>,
    IEquatable<(int X, int Y)>
{
    public static readonly Vector2D Zero = new();

    public static readonly Vector2D NaN = new(double.NaN, double.NaN);

    /// <summary>Координата X</summary>
    private readonly double _X;

    /// <summary>Координата Y</summary>
    private readonly double _Y;

    /// <summary>Координата X</summary>
    [XmlAttribute]
    public double X { get => _X; init => _X = value; }

    /// <summary>Координата Y</summary>
    [XmlAttribute]
    public double Y { get => _Y; init => _Y = value; }

    /// <summary>Радиус (длина) вектора</summary>
    [XmlIgnore]
    public double R => Numeric.Radius(_X, _Y);

    /// <summary>Угол к оси X в радианах</summary>
    [XmlIgnore]
    public double Angle => Numeric.Angle(_X, _Y);

    /// <summary>Инициализация двумерного вектора</summary>
    /// <param name="X">Координата X</param>
    /// <param name="Y">Координата Y</param>
    public Vector2D(double X, double Y) => (_X, _Y) = (X, Y);

    /// <summary>Инициализация вектора по по комплексному числу</summary>
    /// <param name="Z">Комплексное число X + iY</param>
    private Vector2D(Complex Z) => (_X, _Y) = Z;

    /// <summary>Представление вектора в базисе</summary>
    /// <param name="b">Базис</param>
    /// <returns>Вектор в базисе</returns>
    public Vector2D InBasis(Basis2D b) => new(b.xx * _X + b.xy * _Y, b.yx * _X + b.yy * _Y);

    /// <inheritdoc />
    public override string ToString() => $"({X};{Y})";

    /// <inheritdoc />
    public Vector2D Clone() => new(_X, _Y);

    /// <inheritdoc />
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    public bool Equals(Vector2D other) => _X == other._X && _Y == other._Y;

    /// <inheritdoc />
    public bool Equals((double X, double Y) other) => _X == other.X && _Y == other.Y;

    /// <inheritdoc />
    public bool Equals((int X, double Y) other) => _X == other.X && _Y == other.Y;

    /// <inheritdoc />
    public bool Equals((double X, int Y) other) => _X == other.X && _Y == other.Y;

    /// <inheritdoc />
    public bool Equals((int X, int Y) other) => _X == other.X && _Y == other.Y;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector2D v && Equals(v);

    /// <inheritdoc />
    public override int GetHashCode() => unchecked((_X.GetHashCode() * 0x18d) ^ _Y.GetHashCode());

    #region Операторы

    /// <summary>Деконструктор вектора на его составляющие</summary>
    /// <param name="x">Координата X</param><param name="y">Координата Y</param>
    public void Deconstruct(out double x, out double y) => (x, y) = (_X, _Y);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==(Vector2D a, Vector2D b) => a.Equals(b);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==((double X, double Y) a, Vector2D b) => b.Equals(a);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==((int X, double Y) a, Vector2D b) => b.Equals(a);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==((double X, int Y) a, Vector2D b) => b.Equals(a);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==((int X, int Y) a, Vector2D b) => b.Equals(a);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==(Vector2D a, (double X, double Y) b) => a.Equals(b);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==(Vector2D a, (int X, double Y) b) => a.Equals(b);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==(Vector2D a, (double X, int Y) b) => a.Equals(b);

    /// <summary>Оператор равенства</summary>
    public static bool operator ==(Vector2D a, (int X, int Y) b) => a.Equals(b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=(Vector2D a, Vector2D b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=((double X, double Y) a, Vector2D b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=((int X, double Y) a, Vector2D b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=((double X, int Y) a, Vector2D b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=((int X, int Y) a, Vector2D b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=(Vector2D a, (double X, double Y) b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=(Vector2D a, (int X, double Y) b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=(Vector2D a, (double X, int Y) b) => !(a == b);

    /// <summary>Оператор неравенства</summary>
    public static bool operator !=(Vector2D a, (int X, int Y) b) => !(a == b);

    /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
    public static explicit operator Complex(Vector2D P) => new(P._X, P._Y);

    /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
    public static explicit operator (double X, double Y)(Vector2D P) => (P._X, P._Y);

    /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
    public static explicit operator (int X, double Y)(Vector2D P) => ((int)P._X, P._Y);

    /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
    public static explicit operator (double X, int Y)(Vector2D P) => (P._X, (int)P._Y);

    /// <summary>Оператор явного преобразования типа <see cref="Vector2D"/> в <see cref="Complex"/></summary>
    public static explicit operator (int X, int Y)(Vector2D P) => ((int)P._X, (int)P._Y);

    /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
    public static explicit operator Vector2D(Complex Z) => new(Z);

    /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
    public static explicit operator Vector2D((double X, double Y) Z) => new(Z);

    /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
    public static explicit operator Vector2D((int X, double Y) Z) => new(Z);

    /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
    public static explicit operator Vector2D((double X, int Y) Z) => new(Z);

    /// <summary>Оператор явного преобразования типа <see cref="Complex"/> в <see cref="Vector2D"/></summary>
    public static explicit operator Vector2D((int X, int Y) Z) => new(Z);

    /// <summary>Оператор неявного преобразования типа <see cref="double"/> в <see cref="Vector2D"/> (вектор вдоль оси OX)</summary>
    public static implicit operator Vector2D(double x) => new(x, 0);

    /// <summary>Унарный оператор +</summary>
    /// <returns>Вектор без изменения</returns>
    public static Vector2D operator +(Vector2D a) => a;

    /// <summary>Унарный оператор отрицания</summary>
    /// <returns>Вектор, координаты которого имеют противоположенный знак</returns>
    public static Vector2D operator -(Vector2D a) => new(-a._X, -a._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a._X + b._X, a._Y + b._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +((double X, double Y) a, Vector2D b) => new(a.X + b._X, a.Y + b._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +((int X, double Y) a, Vector2D b) => new(a.X + b._X, a.Y + b._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +((double X, int Y) a, Vector2D b) => new(a.X + b._X, a.Y + b._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +((int X, int Y) a, Vector2D b) => new(a.X + b._X, a.Y + b._Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +(Vector2D a, (double X, double Y) b) => new(a._X + b.X, a._Y + b.Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +(Vector2D a, (int X, double Y) b) => new(a._X + b.X, a._Y + b.Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +(Vector2D a, (double X, int Y) b) => new(a._X + b.X, a._Y + b.Y);

    /// <summary>Оператор вычисления суммы двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из суммы координат слагаемых</returns>
    public static Vector2D operator +(Vector2D a, (int X, int Y) b) => new(a._X + b.X, a._Y + b.Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a._X - b._X, a._Y - b._Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -((double X, double Y) a, Vector2D b) => new(a.X - b._X, a.Y - b._Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -((int X, double Y) a, Vector2D b) => new(a.X - b._X, a.Y - b._Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -((double X, int Y) a, Vector2D b) => new(a.X - b._X, a.Y - b._Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -((int X, int Y) a, Vector2D b) => new(a.X - b._X, a.Y - b._Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -(Vector2D a, (double X, double Y) b) => new(a._X - b.X, a._Y - b.Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -(Vector2D a, (int X, double Y) b) => new(a._X - b.X, a._Y - b.Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -(Vector2D a, (double X, int Y) b) => new(a._X - b.X, a._Y - b.Y);

    /// <summary>Оператор вычисления разности двух векторов</summary>
    /// <returns>Вектор, координаты которого составлены из разности координат слагаемых</returns>
    public static Vector2D operator -(Vector2D a, (int X, int Y) b) => new(a._X - b.X, a._Y - b.Y);

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *(Vector2D a, Vector2D b) => a._X * b._X + a._Y * b._Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *((double X, double Y) a, Vector2D b) => a.X * b._X + a.Y * b._Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *((int X, double Y) a, Vector2D b) => a.X * b._X + a.Y * b._Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *((double X, int Y) a, Vector2D b) => a.X * b._X + a.Y * b._Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *((int X, int Y) a, Vector2D b) => a.X * b._X + a.Y * b._Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *(Vector2D a, (double X, double Y) b) => a._X * b.X + a._Y * b.Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *(Vector2D a, (int X, double Y) b) => a._X * b.X + a._Y * b.Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *(Vector2D a, (double X, int Y) b) => a._X * b.X + a._Y * b.Y;

    /// <summary>Оператор вычисления скалярного произведения двух векторов</summary>
    /// <returns>Вещественное число, равное суме попарных произведений координат векторов</returns>
    public static double operator *(Vector2D a, (int X, int Y) b) => a._X * b.X + a._Y * b.Y;

    /// <summary>Оператор вычисления суммы вектора и вещественного числа</summary>
    /// <returns>Вектор, координаты которого увеличены на значение скалярного слагаемого</returns>
    public static Vector2D operator +(Vector2D a, double b) => new(a._X + b, a._Y + b);

    /// <summary>Оператор вычисления суммы вектора и вещественного числа</summary>
    /// <returns>Вектор, координаты которого увеличены на значение скалярного слагаемого</returns>
    public static Vector2D operator +(double a, Vector2D b) => new(a + b._X, a + b._Y);

    /// <summary>Оператор вычитания скалярного значения из вектора</summary>
    /// <param name="a">Уменьшаемое - вектор</param>
    /// <param name="b">Вычитаемое - скаляр</param>
    /// <returns>Вектор, координаты которого уменьшены на значение скаляра</returns>
    public static Vector2D operator -(Vector2D a, double b) => new(a._X - b, a._Y - b);

    /// <summary>Оператор вычитания вектора из скалярного значения</summary>
    /// <param name="b">Уменьшаемое - скаляр</param>
    /// <param name="a">вычитаемое - вектор</param>
    /// <returns>Вектор, координаты которого вычтены из скалярного значения</returns>
    public static Vector2D operator -(double a, Vector2D b) => new(a - b._X, a - b._Y);

    /// <summary>Оператор умножения вектора на число</summary>
    /// <returns>Вектор, координаты которого умножены на скалярное значение</returns>
    public static Vector2D operator *(Vector2D a, double b) => new(a._X * b, a._Y * b);

    /// <summary>Оператор умножения вектора на число</summary>
    /// <returns>Вектор, координаты которого умножены на скалярное значение</returns>
    public static Vector2D operator *(double a, Vector2D b) => new(a * b._X, a * b._Y);

    /// <summary>Оператор деления вектора на число</summary>
    /// <returns>Вектор, координаты которого разделены на скалярное значение</returns>
    public static Vector2D operator /(Vector2D a, double b) => new(a._X / b, a._Y / b);

    /// <summary>Оператор деления числа на вектор</summary>
    /// <returns>Вектор, координаты которого образованы делением координат числа на координаты исходного вектора</returns>
    public static Vector2D operator /(double a, Vector2D b) => new(a / b._X, a / b._Y);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^(Vector2D a, Vector2D b) => a * b / (a.R * b.R);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^((double X, double Y) a, Vector2D b) => a * b / (Sqrt(a.X * a.X + a.Y * a.Y) * b.R);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^((int X, double Y) a, Vector2D b) => a * b / (Sqrt(a.X * a.X + a.Y * a.Y) * b.R);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^((double X, int Y) a, Vector2D b) => a * b / (Sqrt(a.X * a.X + a.Y * a.Y) * b.R);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^((int X, int Y) a, Vector2D b) => a * b / (Sqrt(a.X * a.X + a.Y * a.Y) * b.R);

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^(Vector2D a, (double X, double Y) b) => a * b / (a.R * Sqrt(b.X * b.X + b.Y * b.Y));

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^(Vector2D a, (int X, double Y) b) => a * b / (a.R * Sqrt(b.X * b.X + b.Y * b.Y));

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^(Vector2D a, (double X, int Y) b) => a * b / (a.R * Sqrt(b.X * b.X + b.Y * b.Y));

    /// <summary>Оператор проекции в вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param><param name="b">Вектор проекции</param>
    /// <returns>Проекция первого вектора на второй</returns>
    public static double operator ^(Vector2D a, (int X, int Y) b) => a * b / (a.R * Sqrt(b.X * b.X + b.Y * b.Y));

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |(Vector2D a, Vector2D b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |((double X, double Y) a, Vector2D b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |((int X, double Y) a, Vector2D b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |((double X, int Y) a, Vector2D b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |((int X, int Y) a, Vector2D b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |(Vector2D a, (double X, double Y) b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |(Vector2D a, (int X, double Y) b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |(Vector2D a, (double X, int Y) b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на параллельность двух векторов</summary>
    /// <returns>Истина, если вектора параллельны</returns>
    public static bool operator |(Vector2D a, (int X, int Y) b) => (a ^ b) == 1d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &(Vector2D a, Vector2D b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &((double X, double Y) a, Vector2D b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &((int X, double Y) a, Vector2D b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &((double X, int Y) a, Vector2D b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &((int X, int Y) a, Vector2D b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &(Vector2D a, (double X, double Y) b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &(Vector2D a, (int X, double Y) b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &(Vector2D a, (double X, int Y) b) => (a ^ b) == 0d;

    /// <summary>Оператор проверки на перпендикулярность двух векторов</summary>
    /// <returns>Истина, если вектора перпендикулярны</returns>
    public static bool operator &(Vector2D a, (int X, int Y) b) => (a ^ b) == 0d;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %(Vector2D a, Vector2D b) => a * b / b.R;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %((double X, double Y) a, Vector2D b) => a * b / b.R;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %((int X, double Y) a, Vector2D b) => a * b / b.R;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %((double X, int Y) a, Vector2D b) => a * b / b.R;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %((int X, int Y) a, Vector2D b) => a * b / b.R;

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %(Vector2D a, (double X, double Y) b) => a * b / Sqrt(b.X * b.X + b.Y * b.Y);

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %(Vector2D a, (int X, double Y) b) => a * b / Sqrt(b.X * b.X + b.Y * b.Y);

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %(Vector2D a, (double X, int Y) b) => a * b / Sqrt(b.X * b.X + b.Y * b.Y);

    /// <summary>Оператор вычисления проекции вектора на вектор</summary>
    /// <param name="a">Проецируемый вектор</param>
    /// <param name="b">Вектор, на который осуществляется проекция</param>
    /// <returns>Длина проекции</returns>
    public static double operator %(Vector2D a, (int X, int Y) b) => a * b / Sqrt(b.X * b.X + b.Y * b.Y);

    /// <summary>Оператор представление вектора в базисе</summary>
    /// <param name="a">Вектор, представляемый в базисе</param>
    /// <param name="b">Базис, в котором надо представить вектор</param>
    /// <returns>Вектор, представленный в базисе</returns>
    public static Vector2D operator %(Vector2D a, Basis2D b) => a.InBasis(b);

    /// <summary>Оператор суммы вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого увеличены на значение числа одинарной точности</returns>
    public static Vector2D operator +(Vector2D a, float b) => new(a._X + b, a._Y + b);

    /// <summary>Оператор разности вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого уменьшены на значение числа одинарной точности</returns>
    public static Vector2D operator -(Vector2D a, float b) => new(a._X - b, a._Y - b);

    /// <summary>Оператор произведения вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого умножены на значение числа одинарной точности</returns>
    public static Vector2D operator *(Vector2D a, float b) => new(a._X * b, a._Y * b);

    /// <summary>Оператор деления вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого поделены на значение числа одинарной точности</returns>
    public static Vector2D operator /(Vector2D a, float b) => new(a._X / b, a._Y / b);

    /// <summary>Оператор суммы вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого увеличены на значение числа одинарной точности</returns>
    public static Vector2D operator +(float a, Vector2D b) => new(a + b._X, a + b._Y);

    /// <summary>Оператор вещественного числа одинарной точности и разности вектора</summary>
    /// <returns>Вектор, координаты которого равны разности числа одинарной точности и координат исходного вектора</returns>
    public static Vector2D operator -(float a, Vector2D b) => new(a - b._X, a - b._Y);

    // <summary>Оператор произведения вектора и вещественного числа одинарной точности</summary>
    /// <returns>Вектор, координаты которого умножены на значение числа одинарной точности</returns>
    public static Vector2D operator *(float a, Vector2D b) => new(a * b._X, a * b._Y);

    /// <summary>Оператор деления вещественного числа одинарной точности и вектора</summary>
    /// <returns>Вектор, координаты которого равны отношению числа одинарной точности и координат исходного вектора</returns>
    public static Vector2D operator /(float a, Vector2D b) => new(a / b._X, a / b._Y);

    /// <summary>Оператор суммы вектора и вещественного целого числа</summary>
    /// <returns>Вектор, координаты которого увеличены на значение целого числа</returns>
    public static Vector2D operator +(Vector2D a, int b) => new(a._X + b, a._Y + b);

    /// <summary>Оператор разности вектора и вещественного целого числа</summary>
    /// <returns>Вектор, координаты которого уменьшены на значение целого числа</returns>
    public static Vector2D operator -(Vector2D a, int b) => new(a._X - b, a._Y - b);

    /// <summary>Оператор произведения вектора и целого числа</summary>
    /// <returns>Вектор, координаты которого умножены на значение целого числа</returns>
    public static Vector2D operator *(Vector2D a, int b) => new(a._X * b, a._Y * b);

    /// <summary>Оператор деления вектора и целого числа</summary>
    /// <returns>Вектор, координаты которого поделены на значение целого числа</returns>
    public static Vector2D operator /(Vector2D a, int b) => new(a._X / b, a._Y / b);

    /// <summary>Оператор произведения вектора и целого числа</summary>
    /// <returns>Вектор, координаты которого умножены на значение целого числа</returns>
    public static Vector2D operator +(int a, Vector2D b) => new(a + b._X, a + b._Y);

    /// <summary>Оператор целого числа и разности вектора</summary>
    /// <returns>Вектор, координаты которого равны разности целого числа и координат исходного вектора</returns>
    public static Vector2D operator -(int a, Vector2D b) => new(a - b._X, a - b._Y);

    /// <summary>Оператор произведения вектора и целого числа</summary>
    /// <returns>Вектор, координаты которого умножены на значение целого числа</returns>
    public static Vector2D operator *(int a, Vector2D b) => new(a * b._X, a * b._Y);

    /// <summary>Оператор деления целого числа и вектора</summary>
    /// <returns>Вектор, координаты которого равны отношению целого числа и координат исходного вектора</returns>
    public static Vector2D operator /(int a, Vector2D b) => new(a / b._X, a / b._Y);

    #endregion
}