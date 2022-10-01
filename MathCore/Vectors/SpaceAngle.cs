#nullable enable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

using MathCore.Extensions.Expressions;

using static System.Math;

using static MathCore.Consts;

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace MathCore.Vectors;

/// <summary>Пространственный угол</summary>
[Serializable]
[DebuggerDisplay("Theta = {Theta}; Phi = {Phi}; {AngleType}")]
[TypeConverter(typeof(SpaceAngleConverter))]
public readonly struct SpaceAngle : IEquatable<SpaceAngle>, ICloneable
{
    public static readonly SpaceAngle Zero = new();

    public static readonly SpaceAngle NaN = new(double.NaN, double.NaN);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Константа преобразования угла в радианах в градусы</summary>
    private const double __ToDeg = Geometry.ToDeg;

    /// <summary>Константа преобразования угла в градусах в радианы</summary>
    private const double __ToRad = Geometry.ToRad;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Нормализация угла</summary>
    /// <param name="angle">Номализуемый угол</param>
    /// <param name="type">Тип значения угла (градусы/радианы)</param>
    /// <returns>Угол, представленный в интервале от 0 до 360° (0 до 2π)</returns>
    public static double NormalizeAngle(double angle, in AngleType type = AngleType.Rad)
    {
        var max = type == AngleType.Rad ? pi2 : 360;
        return angle % max + (angle < 0 ? max : 0);
    }

    /// <summary>Случайный пространственный угол</summary>
    /// <param name="Min">Минимальное значение угла</param>
    /// <param name="Max">Максимальное значение угла</param>
    /// <returns>Случайный пространственный угол</returns>
    public static SpaceAngle Random(double Min = -pi, double Max = pi)
    {
        var rnd = new Random();
        // ReSharper disable once EventExceptionNotDocumented
        return new SpaceAngle(
            Abs(Max - Min) * (rnd.NextDouble() - .5) + (Max + Min) * .5,
            Abs(Max - Min) * (rnd.NextDouble() - .5) + (Max + Min) * .5);
    }

    /// <summary>Создать новый пространственный угол на основе значений углов места и азимута</summary>
    /// <param name="theta">Угол места</param>
    /// <param name="phi">Угол азимута</param>
    /// <param name="type">Тип значения угла</param>
    /// <returns>Пространственный угол</returns>
    [DST]
    public static SpaceAngle Value(double theta, double phi, in AngleType type = AngleType.Rad) => new(theta, phi, type);

    /// <summary>Создать угол на основе направляющего вектора</summary>
    /// <param name="r">Вектор направления</param>
    /// <returns>Угол вектора направления</returns>
    [DST]
    public static SpaceAngle Direction(Vector3D r) => r.Angle;

    /// <summary>Создать пространственный угол по заданным координатам вектора направления</summary>
    /// <returns>Угол вектора направления</returns>
    [DST]
    public static SpaceAngle Direction(double x, double y, double z = 0)
        => new(Atan2(Sqrt(x * x + y * y), z), Atan2(y, x));

    // ReSharper disable InconsistentNaming

    /// <summary>Орта оси OX</summary>
    public static readonly SpaceAngle i = new(Theta: pi05, Phi: 0);
    /// <summary>Отрицательная орта оси OX</summary>
    public static readonly SpaceAngle i_negative = new(Theta: pi05, Phi: pi);

    /// <summary>Орта оси OY</summary>
    public static readonly SpaceAngle j = new(Theta: pi05, Phi: pi05);
    /// <summary>Отрицательная орта оси OY</summary>
    public static readonly SpaceAngle j_negative = new(Theta: pi05, Phi: -pi05);

    /// <summary>Орта оси OZ</summary>
    public static readonly SpaceAngle k = new(Theta: 0, Phi: 0);
    /// <summary>Отрицательная орта оси OZ</summary>
    public static readonly SpaceAngle k_negative = new(Theta: pi, Phi: 0);

    // ReSharper restore InconsistentNaming

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Тип угла</summary>
    private readonly AngleType _AngleType;

    /// <summary>Угол места</summary>
    private readonly double _Theta;

    /// <summary>Угол азимута</summary>
    private readonly double _Phi;

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Угол места</summary>
    public double Theta => _Theta;

    /// <summary>Азимутальный угол в плоскости XOY</summary>
    public double Phi => _Phi;

    /// <summary>Угол места в радианах</summary>
    public double ThetaRad => _AngleType == AngleType.Rad ? _Theta : _Theta * Geometry.ToRad;

    /// <summary>Угол места в градусах</summary>
    public double ThetaDeg => _AngleType == AngleType.Deg ? _Theta : _Theta * Geometry.ToDeg;

    /// <summary>Угол азимута в радианах</summary>
    public double PhiRad => _AngleType == AngleType.Rad ? _Phi : _Phi * Geometry.ToRad;

    /// <summary>Угол азимута в градусах</summary>
    public double PhiDeg => _AngleType == AngleType.Deg ? _Phi : _Phi * Geometry.ToDeg;

    public (double Sin, double Cos) SinCosTheta => _AngleType == AngleType.Rad
        ? (Sin(_Theta), Cos(_Theta))
        : (Sin(_Theta * ToRad), Cos(_Theta * ToRad));

    public (double Sin, double Cos) SinCosPhi => _AngleType == AngleType.Rad
        ? (Sin(_Phi), Cos(_Phi))
        : (Sin(_Phi * ToRad), Cos(_Phi * ToRad));

    public ((double SinTh, double CosTh), (double SinPh, double CosPh)) SinCos => _AngleType == AngleType.Rad 
        ? ((Sin(_Theta), Cos(_Theta)), (Sin(_Phi), Cos(_Phi))) 
        : ((Sin(_Theta * ToRad), Cos(_Theta * ToRad)), (Sin(_Phi * ToRad), Cos(_Phi * ToRad)));

    /// <summary>Являются ли значения угла места и азимута = 0?</summary>
    public bool IsZero => _Theta.Equals(0d) && _Phi.Equals(0d);

    /// <summary>Комплексное число, характеризующее действительной частью направляющий косинус <see cref="Theta"/>, мнимой частью - направляющий синус</summary>
    public Complex ComplexCosTheta => _AngleType == AngleType.Rad 
        ? Complex.Exp(_Theta) 
        : Complex.Exp(_Theta * __ToRad);

    /// <summary>Комплексное число, характеризующее действительной частью направляющий косинус <see cref="Phi"/>, мнимой частью - направляющий синус</summary>
    public Complex ComplexCosPhi => _AngleType == AngleType.Rad 
        ? Complex.Exp(_Phi) 
        : Complex.Exp(_Phi * __ToRad);

    /// <summary>Тип угла</summary>
    public AngleType AngleType => _AngleType;

    /// <summary>Представление угла в градусах</summary>
    /// <exception cref="NotSupportedException" accessor="get">Неизвестный тип угла</exception>
    public SpaceAngle InDeg => _AngleType switch
    {
        AngleType.Deg => this,
        AngleType.Rad => new(_Theta * __ToDeg, _Phi * __ToDeg, AngleType.Deg),
        _ => throw new ArgumentOutOfRangeException(nameof(AngleType), _AngleType, "Неизвестный тип угла")
    };

    /// <summary>Представление угла в радианах</summary>
    /// <exception cref="NotSupportedException" accessor="get">Неизвестный тип угла</exception>
    public SpaceAngle InRad => _AngleType switch
    {
        AngleType.Deg => new(_Theta * __ToRad, _Phi * __ToRad, AngleType.Rad),
        AngleType.Rad => this,
        _ => throw new ArgumentOutOfRangeException(nameof(AngleType), _AngleType, "Неизвестный тип угла")
    };

    /// <summary>Направляющий вектор</summary>
    public Vector3D DirectionalVector => new(this);

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового экземпляра <see cref="SpaceAngle"/></summary>
    /// <param name="Phi">Угол азимута</param>
    public SpaceAngle(double Phi)
    {
        _Phi = Phi;
        _Theta = 0;
        _AngleType = AngleType.Rad;
    }

    /// <summary>Инициализация нового экземпляра <see cref="SpaceAngle"/></summary>
    /// <param name="Phi">Угол азимута</param>
    /// <param name="AngleType">Тип значения угла</param>
    public SpaceAngle(double Phi, in AngleType AngleType)
    {
        _Phi = Phi;
        _Theta = 0;
        _AngleType = AngleType;
    }

    /// <summary>Пространственный угол в радианах</summary>
    /// <param name="Theta">Угол места [рад]</param>
    /// <param name="Phi">Азимутальный угол [рад]</param>
    [DST]
    public SpaceAngle(double Theta, double Phi)
    {
        _Phi = Phi;
        _Theta = Theta;
        _AngleType = AngleType.Rad;
    }

    /// <summary>Инициализация нового экземпляра <see cref="SpaceAngle"/></summary>
    /// <param name="Theta">Угол места</param>
    /// <param name="Phi">Угол азимута</param>
    /// <param name="AngleType">Тип значения угла</param>
    [DST]
    public SpaceAngle(double Theta, double Phi, in AngleType AngleType) : this(Theta, Phi) => _AngleType = AngleType;

    /// <summary>Копирование нового экземпляра <see cref="SpaceAngle"/> из прототипа</summary>
    /// <param name="Angle">Исходное значение угла</param>
    [DST]
    public SpaceAngle(SpaceAngle Angle) : this(Angle._Theta, Angle._Phi, Angle._AngleType) { }

    /// <summary>Инициализация нового экземпляра <see cref="SpaceAngle"/></summary>
    /// <param name="Angle">Исходное значение угла</param>
    /// <param name="AngleType">Тип желаемого значения угла</param>
    /// <exception cref="NotSupportedException">Если AngleType != Deg || Rad - Неизвестный тип угла</exception>
    [DST]
    public SpaceAngle(SpaceAngle Angle, in AngleType AngleType)
        : this(Angle._Theta, Angle._Phi, AngleType)
    {
        switch (_AngleType)
        {
            default: throw new ArgumentOutOfRangeException(nameof(AngleType), _AngleType, "Неизвестный тип угла");
            case AngleType.Deg:
                switch (Angle._AngleType)
                {
                    default: throw new ArgumentOutOfRangeException(nameof(AngleType), Angle._AngleType, "Неизвестный тип угла");
                    case AngleType.Deg: break;
                    case AngleType.Rad:
                        _Theta *= __ToDeg;
                        _Phi *= __ToDeg;
                        break;
                }
                break;
            case AngleType.Rad:
                switch (Angle._AngleType)
                {
                    default: throw new ArgumentOutOfRangeException(nameof(AngleType), Angle._AngleType, "Неизвестный тип угла");
                    case AngleType.Deg:
                        _Theta *= __ToRad;
                        _Phi *= __ToRad;
                        break;
                    case AngleType.Rad: break;
                }
                break;
        }
    }

    /// <summary>Повернуть угол в сферической системе координат</summary>
    /// <param name="theta">Угол места поворота локальной системы координат</param>
    /// <param name="phi">Угол азимута поворота локальной системы координат</param>
    /// <param name="type">Тип значений угловых величин</param>
    /// <returns>Угол в повёрнутой локальной системе координат</returns>
    public SpaceAngle RotatePhiTheta(double theta, double phi, in AngleType type = AngleType.Rad)
        => RotatePhiTheta(new SpaceAngle(theta, phi, type));

    /// <summary>Повернуть угол в сферической системе координат</summary>
    /// <param name="angle">Пространственный угол поворота локальной системы координат</param>
    /// <returns>Угол в повёрнутой локальной системе координат</returns>
    public SpaceAngle RotatePhiTheta(SpaceAngle angle)
    {
        var ph0 = angle.PhiRad;
        var th0 = angle.ThetaRad;
        if (ph0 == 0 && th0 == 0) return this;

        var ph = PhiRad;
        var th = ThetaRad;

        var (s_ph0, c_ph0) = Complex.SinCos(ph0);
        var (s_th0, c_th0) = Complex.SinCos(th0);

        var (s_ph, c_ph) = Complex.SinCos(ph);
        var (s_th, c_th) = Complex.SinCos(th);

        var x = c_ph * s_th;
        var y = s_ph * s_th;
        var z = c_th;

        //Поворот по оси Z -> phi
        var x0 = x * c_ph0 - y * s_ph0;
        var y0 = x * s_ph0 + y * c_ph0;
        var z0 = z;

        //Поворот по оси Y -> theta
        var x1 = x0 * c_th0 + z0 * s_th0;
        var y1 = y0;
        var z1 = -x0 * s_th0 + z0 * c_th0;

        //Поворот по X
        //var x1 = x0;
        //var y1 = y0 * c_th0 - z0 * s_th0;
        //var z1 = y0 * s_th0 + z0 * c_th0;

        var ph1 = Atan2(y1, x1);
        var th1 = Atan2(Sqrt(x1 * x1 + y1 * y1), z1);
        return AngleType == AngleType.Rad
            ? new(th1, ph1)
            : new(th1 * __ToDeg, ph1 * __ToDeg, AngleType.Deg);
    }

    /// <summary>Получить функцию, осуществляющую поворот пространственного угла на текущий пространственный угол</summary>
    /// <returns>Функция, осуществляющая поворот пространственного угла на заданный угол</returns>
    public Func<SpaceAngle, SpaceAngle> GetRotatorPhiTheta() => GetRotatorPhiTheta(this);

    /// <summary>Получить функцию, осуществляющую поворот пространственного угла на заданный пространственный угол</summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Func<SpaceAngle, SpaceAngle> GetRotatorPhiTheta(SpaceAngle angle)
    {
        var ph0 = angle.PhiRad;
        var th0 = angle.ThetaRad;
        if (ph0 == 0 && th0 == 0) return a => a;

        var (s_ph0, c_ph0) = Complex.SinCos(ph0);
        var (s_th0, c_th0) = Complex.SinCos(th0);

        return r =>
        {
            var ph = r.PhiRad;
            var th = r.ThetaRad;

            var (s_ph, c_ph) = Complex.SinCos(ph);
            var (s_th, c_th) = Complex.SinCos(th);

            var x = c_ph * s_th;
            var y = s_ph * s_th;
            var z = c_th;

            #region Другой вид поворота

            //Поворот по оси Z
            //var x0 = x*c_ph0 - y*s_ph0;
            //var y0 = x*s_ph0 + y*c_ph0;
            //var z0 = z;

            //Поворот по оси Y
            //var x1 = x0 * c_th0 + z0 * s_th0;
            //var y1 = y0;
            //var z1 = -x0 * s_th0 + z0 * c_th0;

            //Поворот по X
            //var x1 = x0;
            //var y1 = y0 * c_th0 - z0 * s_th0;
            //var z1 = y0 * s_th0 + z0 * c_th0;

            #endregion

            //Поворот по оси Z
            var x0 = x * c_ph0 - y * s_ph0;

            //Поворот по оси Y
            var x1 = x0 * c_th0 + z * s_th0;
            var y1 = x * s_ph0 + y * c_ph0;
            var z1 = -x0 * s_th0 + z * c_th0;

            var ph1 = Atan2(y1, x1);
            var th1 = Atan2(Sqrt(x1 * x1 + y1 * y1), z1);
            return r.AngleType == AngleType.Rad
                ? new SpaceAngle(th1, ph1)
                : new SpaceAngle(th1 * __ToDeg, ph1 * __ToDeg, AngleType.Deg);
        };
    }


    /// <summary>Получить выражение, осуществляющее поворот угла на текущий пространственный угол</summary>
    /// <param name="r">Выражение, предоставляющее пространственный угол, который требуется повернуть</param>
    /// <returns>Выражение, обеспечивающее поворот пространственного угла на текущий угол</returns>
    public Expression GetRotatorPhiThetaExpression(Expression r)
    {
        if (r is null) throw new ArgumentNullException(nameof(r));
        if (r.Type != typeof(SpaceAngle))
            throw new ArgumentException(
                $"Тип результата выражения {r.Type} не является типом {typeof(SpaceAngle)}",
                nameof(r));

        var a = this;
        var ph0 = a.PhiRad;
        var th0 = a.ThetaRad;
        if (ph0.Equals(0d) && th0.Equals(0d)) return r;

        var th0_ex = th0.ToExpression();
        var ph0_ex = ph0.ToExpression();

        var sin = (Func<double, double>)Sin;
        var cos = (Func<double, double>)Cos;
        var atan = (Func<double, double, double>)Atan2;
        var sqrt = (Func<double, double>)Sqrt;

        var s_ph0 = sin.GetCallExpression(ph0_ex);
        var c_ph0 = cos.GetCallExpression(ph0_ex);
        var s_th0 = sin.GetCallExpression(th0_ex);
        var c_th0 = cos.GetCallExpression(th0_ex);

        ParameterExpression th, ph, ph1, th1;
        ParameterExpression s_th, s_ph, c_th, c_ph;
        ParameterExpression x, y, z, x0, x1, y1, z1;
        return Expression.Block
        (
            new[]
            {
                th = nameof(th).ParameterOf(typeof(double)),
                ph = nameof(ph).ParameterOf(typeof(double)),
                s_th = nameof(s_th).ParameterOf(typeof(double)),
                c_th = nameof(c_th).ParameterOf(typeof(double)),
                s_ph = nameof(s_ph).ParameterOf(typeof(double)),
                c_ph = nameof(c_ph).ParameterOf(typeof(double)),
                x = nameof(x).ParameterOf(typeof(double)),
                y = nameof(y).ParameterOf(typeof(double)),
                z = nameof(z).ParameterOf(typeof(double)),
                x0 = nameof(x0).ParameterOf(typeof(double)),
                x1 = nameof(x1).ParameterOf(typeof(double)),
                y1 = nameof(y1).ParameterOf(typeof(double)),
                z1 = nameof(z1).ParameterOf(typeof(double)),
                ph1 = nameof(ph1).ParameterOf(typeof(double)),
                th1 = nameof(th1).ParameterOf(typeof(double))
            },
            th.Assign(r.GetProperty(nameof(ThetaRad))),
            ph.Assign(r.GetProperty(nameof(PhiRad))),
            s_th.Assign(sin.GetCallExpression(th)),
            c_th.Assign(cos.GetCallExpression(th)),
            s_ph.Assign(sin.GetCallExpression(ph)),
            c_ph.Assign(cos.GetCallExpression(ph)),
            x.Assign(c_ph.Multiply(s_th)),
            y.Assign(s_ph.Multiply(s_th)),
            z.Assign(c_th),
            x0.Assign(x.Multiply(c_ph0).Subtract(y.Multiply(s_ph0))),
            x1.Assign(x0.Multiply(c_th0).Add(z.Multiply(s_th0))),
            y1.Assign(x.Multiply(s_ph0).Add(y.Multiply(c_ph0))),
            z1.Assign(z.Multiply(c_th0).Subtract(x0.Multiply(s_th0))),
            ph1.Assign(atan.GetCallExpression(y1, x1)),
            th1.Assign(atan.GetCallExpression(sqrt.GetCallExpression(x1.Multiply(x1).Add(y1.Multiply(y1))), z1)),
            r.GetProperty(nameof(AngleType)).IsEqual(AngleType.Rad.ToExpression())
               .Condition
                (
                    typeof(SpaceAngle).ToNewExpression(th1, ph1),
                    typeof(SpaceAngle).ToNewExpression(th1, ph1).GetProperty(nameof(InDeg))
                )
        );
    }

    /// <summary>Повернуть функцию от пространственного угла на текущий угол</summary>
    /// <param name="f">Поворачиваемая функция</param>
    /// <typeparam name="T">Тип значения функции</typeparam>
    /// <returns>Функция, аргумент которой повёрнут на текущий угол</returns>
    public Func<SpaceAngle, T> RotatePhiTheta<T>(Func<SpaceAngle, T> f)
    {
        var r = GetRotatorPhiTheta();
        return a => f(r(a));
    }

    /// <summary>Представить угол в указанном типе значения</summary>
    /// <param name="type">Тип требуемого значения угла</param>
    /// <returns></returns>
    [DST]
    public SpaceAngle In(AngleType type) =>
        _AngleType == type
            ? this
            : type switch
            {
                AngleType.Rad => InRad,
                AngleType.Deg => InDeg,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

    public double ProjectionTo(double X, double Y, double Z)
    {
        double sin_th, cos_th, sin_ph, cos_ph;
        if (_AngleType == AngleType.Rad)
        {
            sin_th = Sin(_Theta);
            cos_th = Cos(_Theta);
            sin_ph = Sin(_Phi);
            cos_ph = Cos(_Phi);
        }
        else
        {
            sin_th = Sin(_Theta * ToRad);
            cos_th = Cos(_Theta * ToRad);
            sin_ph = Sin(_Phi * ToRad);
            cos_ph = Cos(_Phi * ToRad);
        }

        return (X * cos_ph + Y * sin_ph) * sin_th + Z * cos_th;
    }

    /* -------------------------------------------------------------------------------------------- */

    /// <summary>Деконструктор значений угла</summary>
    /// <param name="theta">Угол места</param>
    /// <param name="phi">Угол азимута</param>
    public void Deconstruct(out double theta, out double phi) { theta = _Theta; phi = _Phi; }

    /// <inheritdoc />
    [DST]
    public object Clone() => new SpaceAngle(this, AngleType);

    /// <inheritdoc />
    [DST] public override string ToString() => $"(Theta:{_Theta}; Phi:{_Phi}):{_AngleType}";

    /// <inheritdoc />
    [DST]
    public override int GetHashCode() =>
        _AngleType == AngleType.Deg
            ? InRad.GetHashCode()
            : unchecked((_Theta.GetHashCode() * 397) ^ _Phi.GetHashCode());

    /// <inheritdoc />
    [DST]
    public override bool Equals(object? obj) => obj is SpaceAngle a && Equals(a);

    /* -------------------------------------------------------------------------------------------- */



    /* -------------------------------------------------------------------------------------------- */

    public static SpaceAngle operator +(SpaceAngle x, SpaceAngle y)
    {
        var (y_th, y_ph) = y.In(x._AngleType);
        return new SpaceAngle(x._Theta + y_th, x._Phi + y_ph, x._AngleType);
    }

    public static SpaceAngle operator -(SpaceAngle x, SpaceAngle y)
    {
        var (y_th, y_ph) = y.In(x._AngleType);
        return new SpaceAngle(x._Theta - y_th, x._Phi - y_ph, x._AngleType);
    }

    /// <summary>Оператор отрицания значения пространственного угла</summary>
    /// <param name="a">Исходный пространственный угол</param>
    /// <returns>Пространственный угол у которого значения угла места и азимута имеют обратный знак по отношению к исходному значению</returns>
    public static SpaceAngle operator -(SpaceAngle a) => new(-a._Theta, -a._Phi, a._AngleType);

    public static SpaceAngle operator /(SpaceAngle a, double x) => new(a.Theta / x, a.Phi / x, a.AngleType);

    public static bool operator ==(SpaceAngle A, SpaceAngle B) => A.Equals(B);

    public static bool operator !=(SpaceAngle A, SpaceAngle B) => !(A == B);

    public static implicit operator Vector3D(SpaceAngle a) => new(a);

    public static explicit operator SpaceAngle(Vector3D v) => v.Angle;

    /// <summary>Оператор поворота функции на пространственный угол</summary>
    /// <param name="f">Вещественная пространственная функция</param>
    /// <param name="a">Пространственный угол поворота</param>
    /// <returns>Вещественная функция, аргумент которой повёрнут на указанный пространственный угол</returns>
    public static Func<SpaceAngle, double> operator ^(Func<SpaceAngle, double> f, SpaceAngle a) => a.RotatePhiTheta(f);

    /// <summary>Оператор поворота функции на пространственный угол</summary>
    /// <param name="f">Комплексная пространственная функция</param>
    /// <param name="a">Пространственный угол поворота</param>
    /// <returns>Комплексная функция, аргумент которой повёрнут на указанный пространственный угол</returns>
    public static Func<SpaceAngle, Complex> operator ^(Func<SpaceAngle, Complex> f, SpaceAngle a) => a.RotatePhiTheta(f);

    /* -------------------------------------------------------------------------------------------- */

    #region IEquatable<SpaceAngle> Members

    /// <summary>Точность сравнения (по умолчанию 10^-13)</summary>
    public static double ComparisonsAccuracy { get; set; } = 1e-13;

    public bool Equals(SpaceAngle other) => Equals(other, ComparisonsAccuracy);

    public bool Equals(SpaceAngle other, double eps)
    {
        if (other._AngleType != _AngleType)
            other = other.In(_AngleType);
        var max = _AngleType == AngleType.Rad ? pi2 : 360;
        return
            Abs(_Theta % max + (_Theta < 0 ? max : 0) - (other._Theta % max + (other._Theta < 0 ? max : 0))) % max < eps &&
            Abs(_Phi % max + (_Phi < 0 ? max : 0) - (other._Phi % max + (other._Phi < 0 ? max : 0))) % max < eps;
    }

    bool IEquatable<SpaceAngle>.Equals(SpaceAngle other) => Equals(other);

    #endregion
}