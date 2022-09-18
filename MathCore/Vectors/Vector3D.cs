using System;
using System.ComponentModel;
using System.Xml.Serialization;
using MathCore.Annotations;
using static System.Math;
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MathCore.Vectors;

/// <summary>Трёхмерный вектор</summary>
[TypeConverter(typeof(Vector3DConverter))]
public readonly partial struct Vector3D : 
    ICloneable<Vector3D>, 
    IFormattable, 
    IEquatable<Vector3D>, 
    IEquatable<(double X, double Y, double Z)>
{

    public static readonly Vector3D Zero = new();

    public static readonly Vector3D NaN = new(double.NaN, double.NaN, double.NaN);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Создать вектор по сферической системе координат</summary>
    /// <param name="Theta">Угол места</param>
    /// <param name="Phi">Азимут</param>
    /// <param name="R">Длина вектора</param>
    /// <returns>Трёхмерный вектор</returns>
    [DST]
    public static Vector3D ThetaPhiRadius(double Theta, double Phi, double R = 1) => new(R, new SpaceAngle(Theta, Phi));

    /// <summary>Преобразовать координаты декартовой системы в вектор</summary>
    /// <param name="X">Координата X</param>
    /// <param name="Y">Координата Y</param>
    /// <param name="Z">Координата Z</param>
    /// <returns>Вектор с заданными координатами</returns>
    [DST]
    public static Vector3D XYZ(double X, double Y, double Z) => new(X, Y, Z);

    /// <summary>Вектор со случайными координатами (с равномерным распределением)</summary>
    /// <param name="min">Минимальное значение</param>
    /// <param name="max">Максимальное значение</param>
    /// <param name="rnd">Генератор случайных чисел (если не задан, то будет создан новый)</param>
    /// <returns>Вектор со случайными значениями координат из указанного диапазона</returns>
    public static Vector3D Random(double min = -100, double max = 100, Random rnd = null)
    {
        rnd ??= new Random();
        var d = Abs(max - min);
        var m = (max + min) * .5;
        return new Vector3D(rnd.NextDouble(d, m), rnd.NextDouble(d, m), rnd.NextDouble(d, m));
    }

    /* -------------------------------------------------------------------------------------------- */
    // ReSharper disable InconsistentNaming

    /// <summary>Базисный вектор k</summary>
    public static readonly Vector3D k = new(0, 0, 1);

    /// <summary>Вектор нулевой длины в начале координат</summary>
    public static readonly Vector3D Empty = new();

    /// <summary>Единичный базисный вектор</summary>
    public static readonly Vector3D BasisUnitVector = new(1, 1, 1);

    /// <summary>Базисный вектор i</summary>
    public static readonly Vector3D i = new(1, 0, 0);

    /// <summary>Базисный вектор j</summary>
    public static readonly Vector3D j = new(0, 1, 0);

    // ReSharper restore InconsistentNaming
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
    public double X { get => _X; init => _X = value; }

    /// <summary>Длина по оси Y</summary>
    [XmlAttribute]
    public double Y { get => _Y; init => _Y = value; }

    /// <summary>Длина по оси Z</summary>
    [XmlAttribute]
    public double Z { get => _Z; init => _Z = value; }

    /// <summary>Длина вектора</summary>
    [XmlIgnore]
    public double R => Sqrt(_X * _X + _Y * _Y + _Z * _Z);

    /// <summary>Угол проекции в плоскости XOY</summary>
    public double AngleXOY => Abs(_X) < double.Epsilon
        ? Abs(_Y) < double.Epsilon   // X == 0
            ? 0                      //  Y == 0 => 0
            : Sign(_Y) * Consts.pi05 //  Y != 0 => pi/2 * sign(Y)
        : Abs(_Y) < double.Epsilon   // X != 0
            ? Sign(_X) > 0
                ? 0
                : Consts.pi
            : Atan2(_Y, _X);

    /// <summary>Угол проекции в плоскости XOZ</summary>
    public double AngleXOZ => Abs(_X) < double.Epsilon
        ? Abs(_Z) < double.Epsilon   // X == 0
            ? 0                      //  Z == 0 => 0
            : Sign(_Z) * Consts.pi05 //  Z != 0 => pi/2 * sign(Z)
        : Abs(_Z) < double.Epsilon   // X != 0
            ? Sign(_X) > 0
                ? 0
                : Consts.pi
            : Atan2(_Z, _X);

    /// <summary>Угол проекции в плоскости YOZ</summary>
    public double AngleYOZ => Abs(_Y) < double.Epsilon
        ? Abs(_Z) < double.Epsilon   // Y == 0
            ? 0                      //  Z == 0 => 0
            : Sign(_Z) * Consts.pi05 //  Z != 0 => pi/2 * sign(Y)
        : Abs(_Z) < double.Epsilon   // Y != 0
            ? Sign(_Y) > 0
                ? 0
                : Consts.pi
            : Atan2(_Z, _Y);

    /// <summary>Азимутальный угол</summary>
    [XmlIgnore]
    public double Phi => AngleXOY;

    /// <summary>Угол места</summary>
    [XmlIgnore]
    public double Theta => Atan2(R_XOY, _Z);

    /// <summary>Пространственный угол</summary>
    [XmlIgnore]
    public SpaceAngle Angle => new(Theta, Phi);

    /// <summary>Двумерный вектор - проекция в плоскости XOY</summary>
    public Vector2D VectorXOY => new(_X, _Y);

    /// <summary>Двумерный вектор - проекция в плоскости XOZ (X->X; Z->Y)</summary>
    public Vector2D VectorXOZ => new(_X, _Z);

    /// <summary>Двумерный вектор - проекция в плоскости YOZ (Y->X; Z->Y)</summary>
    public Vector2D VectorYOZ => new(_Y, _Z);

    /// <summary>Длина в плоскости XOY</summary>
    public double R_XOY => Sqrt(_X * _X + _Y * _Y);

    /// <summary>Длина в плоскости XOZ</summary>
    public double R_XOZ => Sqrt(_X * _X + _Z * _Z);

    /// <summary>Длина в плоскости YOZ</summary>
    public double R_YOZ => Sqrt(_Y * _Y + _Z * _Z);

    /// <summary>Длина вектора</summary>
    public Vector3D Abs => new(Abs(_X), Abs(_Y), Abs(_Z));

    /// <summary>Вектор знаков координат текущего вектора</summary>
    public Vector3D Sign => new(Sign(_X), Sign(_Y), Sign(_Z));

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового вектора, расположенного вдоль оси OX</summary>
    /// <param name="X">Координата вдоль оси OX</param>
    [DST] public Vector3D(double X) { _X = X; _Y = 0; _Z = 0; }

    /// <summary>Инициализация нового вектора, расположенного в плоскости XOY</summary>
    /// <param name="X">Координата вдоль оси OX</param>
    /// <param name="Y">Координата вдоль оси OY</param>
    [DST] public Vector3D(double X, double Y) { _X = X; _Y = Y; _Z = 0; }

    /// <summary>Инициализация нового вектора, заданного своими координатами</summary>
    /// <param name="X">Координата вдоль оси OX</param>
    /// <param name="Y">Координата вдоль оси OY</param>
    /// <param name="Z">Координата вдоль оси OZ</param>
    [DST] public Vector3D(double X, double Y, double Z) { _X = X; _Y = Y; _Z = Z; }

    /// <summary>Единичный вектор, заданный двумя углами в сферической системе координат</summary>
    /// <param name="Angle">Пространственный угол сферической системы координат</param>
    public Vector3D(SpaceAngle Angle) : this(1, Angle) { }

    /// <summary>Вектор, заданный двумя углами и радиусом в сферической системе координат</summary>
    /// <param name="R">Радиус вектора</param>
    /// <param name="Angle">Пространственный угол сферической системы координат</param>
    public Vector3D(double R, SpaceAngle Angle)
    {
        double theta;
        double phi;
        if (Angle.AngleType == AngleType.Deg)
        {
            theta = Angle.InRad.Theta;
            phi   = Angle.InRad.Phi;
        }
        else
        {
            theta = Angle.Theta;
            phi   = Angle.Phi;
        }

        _Z = R * Cos(theta);
        var r = R * Sin(theta);
        _X = r * Cos(phi);
        _Y = r * Sin(phi);
    }

    /// <summary>Конструктор копирования</summary>
    /// <param name="V">Вектор-прототип</param>
    private Vector3D(in Vector3D V) { _X = V._X; _Y = V._Y; _Z = V._Z; }

    /// <summary>Представление вектора в базисе</summary>
    /// <param name="b">Новый базис вектора</param>
    /// <returns>Вектор в указанном базисе</returns>
    public Vector3D InBasis(in Basis3D b) => new(
        b.xx * _X + b.xy * _Y + b.xz * _Z,
        b.yx * _X + b.yy * _Y + b.yz * _Z,
        b.zx * _X + b.zy * _Y + b.zz * _Z);

    /// <summary>Инкрементация координат вектора</summary>
    /// <param name="dx">Величина приращения координаты X</param>
    /// <param name="dy">Величина приращения координаты Y</param>
    /// <param name="dz">Величина приращения координаты Z</param>
    /// <returns>Вектор с новыми координатами</returns>
    public Vector3D Inc(double dx, double dy, double dz) => new(_X + dx, _Y + dy, _Z + dz);

    /// <summary>Инкрементировать координату X</summary>
    /// <param name="dx">Величина приращения координаты X</param>
    /// <returns>Вектор с обновлённой координатой X</returns>
    public Vector3D IncX(double dx) => new(_X + dx, _Y, _Z);

    /// <summary>Инкрементировать координату Y</summary>
    /// <param name="dy">Величина приращения координаты Y</param>
    /// <returns>Вектор с обновлённой координатой Y</returns>
    public Vector3D IncY(double dy) => new(_X, _Y + dy, _Z);

    /// <summary>Инкрементировать координату Z</summary>
    /// <param name="dz">Величина приращения координаты Z</param>
    /// <returns>Вектор с обновлённой координатой Z</returns>
    public Vector3D IncZ(double dz) => new(_X, _Y, _Z + dz);

    /// <summary>Декрементация координат вектора</summary>
    /// <param name="dx">Величина приращения координаты X</param>
    /// <param name="dy">Величина приращения координаты Y</param>
    /// <param name="dz">Величина приращения координаты Z</param>
    /// <returns>Вектор с новыми координатами</returns>
    public Vector3D Dec(double dx, double dy, double dz) => new(_X - dx, _Y - dy, _Z - dz);

    /// <summary>Декрементировать координату X</summary>
    /// <param name="dx">Величина приращения координаты X</param>
    /// <returns>Вектор с обновлённой координатой X</returns>
    public Vector3D DecX(double dx) => new(_X - dx, _Y, _Z);

    /// <summary>Декрементировать координату Y</summary>
    /// <param name="dy">Величина приращения координаты Y</param>
    /// <returns>Вектор с обновлённой координатой Y</returns>
    public Vector3D DecY(double dy) => new(_X, _Y - dy, _Z);

    /// <summary>Декрементировать координату Z</summary>
    /// <param name="dz">Величина приращения координаты Z</param>
    /// <returns>Вектор с обновлённой координатой Z</returns>
    public Vector3D DecZ(double dz) => new(_X, _Y, _Z - dz);

    /// <summary>Выполнить масштабирование вектора по координатам</summary>
    /// <param name="kx">Коэффициент масштабирования координаты X</param>
    /// <param name="ky">Коэффициент масштабирования координаты Y</param>
    /// <param name="kz">Коэффициент масштабирования координаты Z</param>
    /// <returns>Вектор, координаты которого умножены на соответствующие значения</returns>
    public Vector3D Scale(double kx, double ky, double kz) => new(_X * kx, _Y * ky, _Z * kz);

    /// <summary>Выполнить масштабирование вектора по оси OX</summary>
    /// <param name="kx">Коэффициент масштабирования координаты X</param>
    /// <returns>Вектор, координаты которого умножены на соответствующие значения</returns>
    public Vector3D ScaleX(double kx) => new(_X * kx, _Y, _Z);

    /// <summary>Выполнить масштабирование вектора по оси OY</summary>
    /// <param name="ky">Коэффициент масштабирования координаты Y</param>
    /// <returns>Вектор, координаты которого умножены на соответствующие значения</returns>
    public Vector3D ScaleY(double ky) => new(_X, _Y * ky, _Z);

    /// <summary>Выполнить масштабирование вектора по оси OZ</summary>
    /// <param name="kz">Коэффициент масштабирования координаты Z</param>
    /// <returns>Вектор, координаты которого умножены на соответствующие значения</returns>
    public Vector3D ScaleZ(double kz) => new(_X, _Y, _Z * kz);

    /* -------------------------------------------------------------------------------------------- */

    /// <inheritdoc />
    [DST, NotNull]
    public override string ToString() => $"({_X};{_Y};{_Z})";

    /// <inheritdoc />
    [DST]
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

    /// <summary>Создает новый объект, который является копией текущего экземпляра</summary>
    /// <returns>Новый объект, являющийся копией этого экземпляра</returns>
    [DST]
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    [DST]
    public Vector3D Clone() => new(this);

    /// <inheritdoc />
    [DST]
    public override bool Equals(object obj) => obj is Vector3D vector_3d && Equals(vector_3d);

    /// <summary>Преобразование в строку с форматированием</summary>
    /// <param name="Format">Строка формата</param>
    /// <returns>Форматированное строковое представление</returns>
    [DST, NotNull]
    public string ToString(string Format) => $"({_X.ToString(Format)};{_Y.ToString(Format)};{_Z.ToString(Format)})";

    /// <summary>Преобразование в строку с форматированием</summary>
    /// <param name="Format">Строка формата</param>
    /// <param name="Provider">Провайдер форматирования данных</param>
    /// <returns>Форматированное строковое представление</returns>
    [DST]
    public string ToString(string Format, IFormatProvider Provider) => $"({_X.ToString(Format, Provider)};{_Y.ToString(Format, Provider)};{_Z.ToString(Format, Provider)})";

    /// <summary>Деконструктор вектора на значения его координат</summary>
    public void Deconstruct(out double x, out double y, out double z) { x = _X; y = _Y; z = _Z; }

    /* -------------------------------------------------------------------------------------------- */

    #region IEquatable<Vector3D> Members

    /// <summary>Точность сравнения (по умолчанию 10^-16)</summary>
    public static double ComparisonsAccuracy { get; set; } = 1e-16;

    /// <inheritdoc />
    public bool Equals(Vector3D other)
    {
        var eps = ComparisonsAccuracy;
        return Abs(other._X - _X) < eps
            && Abs(other._Y - _Y) < eps
            && Abs(other._Z - _Z) < eps;
    }

    /// <inheritdoc />
    public bool Equals((double X, double Y, double Z) other)
    {
        var eps = ComparisonsAccuracy;
        return Abs(other.X - _X) < eps 
            && Abs(other.Y - _Y) < eps
            && Abs(other.Z - _Z) < eps;
    }

    #endregion

    /* -------------------------------------------------------------------------------------------- */
}