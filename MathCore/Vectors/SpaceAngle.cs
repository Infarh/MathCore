using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MathCore.Annotations;
using MathCore.Extentions.Expressions;
using static System.Math;
// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors
{
    /// <summary>Пространственный угол</summary>
    [Serializable]
    [DebuggerDisplay("Thetta = {Thetta}; Phi = {Phi}; {AngleType}")]
    [TypeConverter(typeof(SpaceAngleConverter))]
    public struct SpaceAngle : IEquatable<SpaceAngle>, ICloneable
    {
        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Константа преобразования угла в радианах в градусы</summary>
        private const double c_ToDeg = Consts.Geometry.ToDeg;

        /// <summary>Константа преобразования угла в градусах в радианы</summary>
        private const double c_ToRad = Consts.Geometry.ToRad;

        /// <summary>Число Пи</summary>
        public const double pi = Consts.pi;

        /* -------------------------------------------------------------------------------------------- */

        public static double NormaliseAngle(double angle, in AngleType type = AngleType.Rad)
        {
            var max = type == AngleType.Rad ? Consts.pi2 : 360;
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

        [DebuggerStepThrough]
        public static SpaceAngle Value(double thetta, double phi, in AngleType type = AngleType.Rad) => new SpaceAngle(thetta, phi, type);

        [DebuggerStepThrough]
        public static SpaceAngle Direction(in Vector3D r) => r.Angle;

        [DebuggerStepThrough]
        public static SpaceAngle Direction(double x, double y, double z = 0)
            => new SpaceAngle(Atan2(Sqrt(x * x + y * y), z), Atan2(y, x));

        public static readonly SpaceAngle i = new SpaceAngle(Thetta: Consts.pi05, Phi: 0);
        public static readonly SpaceAngle i_negative = new SpaceAngle(Thetta: Consts.pi05, Phi: Consts.pi);

        public static readonly SpaceAngle j = new SpaceAngle(Thetta: Consts.pi05, Phi: Consts.pi05);
        public static readonly SpaceAngle j_negative = new SpaceAngle(Thetta: Consts.pi05, Phi: -Consts.pi05);

        public static readonly SpaceAngle k = new SpaceAngle(Thetta: 0, Phi: 0);
        public static readonly SpaceAngle k_negative = new SpaceAngle(Thetta: Consts.pi, Phi: 0);


        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Тип угла</summary>
        private readonly AngleType _AngleType;

        /// <summary>Угол места</summary>
        private readonly double _Thetta;

        /// <summary>Угол азимута</summary>
        private readonly double _Phi;

        /* -------------------------------------------------------------------------------------------- */

        /// <summary>Угол места</summary>
        public double Thetta => _Thetta;

        /// <summary>Азимутальный угол в плоскости XOY</summary>
        public double Phi => _Phi;

        public double ThettaRad => _AngleType == AngleType.Rad ? _Thetta : _Thetta * Consts.Geometry.ToRad;
        public double ThettaDeg => _AngleType == AngleType.Deg ? _Thetta : _Thetta * Consts.Geometry.ToDeg;
        public double PhiRad => _AngleType == AngleType.Rad ? _Phi : _Phi * Consts.Geometry.ToRad;
        public double PhiDeg => _AngleType == AngleType.Deg ? _Phi : _Phi * Consts.Geometry.ToDeg;

        public bool IsZerro => _Thetta.Equals(0d) && _Phi.Equals(0d);

        /// <summary>Комплексное число, характеризующее действительной частью направляющий косинус <see cref="Thetta"/>, мнимой частью - направляющий синус</summary>
        public Complex ComplexCosThetta => _AngleType == AngleType.Rad ? Complex.Exp(_Thetta) : Complex.Exp(_Thetta * c_ToRad);

        /// <summary>Комплексное число, характеризующее действительной частью направляющий косинус <see cref="Phi"/>, мнимой частью - направляющий синус</summary>
        public Complex ComplexCosPhi => _AngleType == AngleType.Rad ? Complex.Exp(_Phi) : Complex.Exp(_Phi * c_ToRad);

        /// <summary>Тип угла</summary>
        public AngleType AngleType => _AngleType;

        /// <summary>Представление угла в градусах</summary>
        /// <exception cref="NotSupportedException" accessor="get">Неизвестный тип угла</exception>
        public SpaceAngle InDeg
        {
            get
            {
                switch(_AngleType)
                {
                    default: throw new NotSupportedException("Неизвестный тип угла");
                    case AngleType.Deg:
                        return this; //new SpaceAngle(_Thetta, _Phi, AngleType.Deg);
                    case AngleType.Rad:
                        return new SpaceAngle(_Thetta * c_ToDeg, _Phi * c_ToDeg, AngleType.Deg);
                }
            }
        }

        /// <summary>Представлкние угла в радианах</summary>
        /// <exception cref="NotSupportedException" accessor="get">Неизвестный тип угла</exception>
        public SpaceAngle InRad
        {
            get
            {
                switch(_AngleType)
                {
                    default: throw new NotSupportedException("Неизвестный тип угла");
                    case AngleType.Deg:
                        return new SpaceAngle(_Thetta * c_ToRad, _Phi * c_ToRad, AngleType.Rad);
                    case AngleType.Rad:
                        return this; //new SpaceAngle(_Thetta, _Phi, AngleType.Rad);
                }
            }
        }

        public Vector3D DirectionalVector => new Vector3D(this);

        /* -------------------------------------------------------------------------------------------- */

        public SpaceAngle(double Phi)
        {
            _Phi = Phi;
            _Thetta = 0;
            _AngleType = AngleType.Rad;
        }

        public SpaceAngle(double Phi, in AngleType AngleType)
        {
            _Phi = Phi;
            _Thetta = 0;
            _AngleType = AngleType;
        }

        /// <summary>Пространственный угол в радианах</summary>
        /// <param name="Thetta">Угол места [рад]</param>
        /// <param name="Phi">Азимутальный угол [рад]</param>
        [DebuggerStepThrough]
        public SpaceAngle(double Thetta, double Phi)
        {
            _Phi = Phi;
            _Thetta = Thetta;
            _AngleType = AngleType.Rad;
        }

        [DebuggerStepThrough]
        public SpaceAngle(double Thetta, double Phi, in AngleType AngleType) : this(Thetta, Phi) { _AngleType = AngleType; }

        [DebuggerStepThrough]
        public SpaceAngle(in SpaceAngle Angle) : this(Angle._Thetta, Angle._Phi, Angle._AngleType) { }

        /// <exception cref="NotSupportedException">Если AngleType != Deg || Rad - Неизвестный тип угла</exception>
        [DebuggerStepThrough]
        public SpaceAngle(in SpaceAngle Angle, in AngleType AngleType)
            : this(Angle._Thetta, Angle._Phi, AngleType)
        {
            switch(_AngleType)
            {
                default: throw new NotSupportedException("Неизвестный тип угла");
                case AngleType.Deg:
                    switch(Angle._AngleType)
                    {
                        default: throw new NotSupportedException("Неизвестный тип угла");
                        case AngleType.Deg: break;
                        case AngleType.Rad:
                            _Thetta *= c_ToDeg;
                            _Phi *= c_ToDeg;
                            break;
                    }
                    break;
                case AngleType.Rad:
                    switch(Angle._AngleType)
                    {
                        default: throw new NotSupportedException("Неизвестный тип угла");
                        case AngleType.Deg:
                            _Thetta *= c_ToRad;
                            _Phi *= c_ToRad;
                            break;
                        case AngleType.Rad: break;
                    }
                    break;
            }
        }

        /// <summary>Повернуть угол в сферической системе координат</summary>
        /// <param name="thetta">Угол места поворота локальной системы кординат</param>
        /// <param name="phi">Угол азимута поворота локальной системы координат</param>
        /// <param name="type">Тип значений угловых величин</param>
        /// <returns>Угол в повёрнутой локальной системе координат</returns>
        public SpaceAngle Rotate_PhiThetta(double thetta, double phi, in AngleType type = AngleType.Rad)
            => Rotate_PhiThetta(new SpaceAngle(thetta, phi, type));

        /// <summary>Повернуть угол в сферической системе координат</summary>
        /// <param name="angle">Пространственный угол поворота локаьлной системы координат</param>
        /// <returns>Угол в повёрнутой локальной системе координат</returns>
        public SpaceAngle Rotate_PhiThetta(in SpaceAngle angle)
        {
            var ph0 = angle.PhiRad;
            var th0 = angle.ThettaRad;
            if(ph0.Equals(0d) && th0.Equals(0d)) return this;

            var ph = PhiRad;
            var th = ThettaRad;

            var s_ph0 = Sin(ph0);
            var c_ph0 = Cos(ph0);
            var s_th0 = Sin(th0);
            var c_th0 = Cos(th0);

            var s_ph = Sin(ph);
            var c_ph = Cos(ph);
            var s_th = Sin(th);
            var c_th = Cos(th);

            var x = c_ph * s_th;
            var y = s_ph * s_th;
            var z = c_th;

            //Поворот по оси Z -> phi
            var x0 = x * c_ph0 - y * s_ph0;
            var y0 = x * s_ph0 + y * c_ph0;
            var z0 = z;

            //Поворот по оси Y -> thetta
            var x1 = x0 * c_th0 + z0 * s_th0;
            var y1 = y0;
            var z1 = -x0 * s_th0 + z0 * c_th0;

            //Поворот по X
            //var x1 = x0;
            //var y1 = y0 * c_th0 - z0 * s_th0;
            //var z1 = y0 * s_th0 + z0 * c_th0;

            var ph1 = Atan2(y1, x1);
            var th1 = Atan2(Sqrt(x1 * x1 + y1 * y1), z1);
            return AngleType == AngleType.Rad ? new SpaceAngle(th1, ph1) : new SpaceAngle(th1, ph1).InDeg;
        }

        [NotNull]
        public Func<SpaceAngle, SpaceAngle> GetRotator_PhiThetta() => GetRotator_PhiThetta(this);

        [NotNull]
        public static Func<SpaceAngle, SpaceAngle> GetRotator_PhiThetta(in SpaceAngle angle)
        {
            var ph0 = angle.PhiRad;
            var th0 = angle.ThettaRad;
            if(ph0.Equals(0d) && th0.Equals(0d)) return a => a;

            var s_ph0 = Sin(ph0);
            var c_ph0 = Cos(ph0);
            var s_th0 = Sin(th0);
            var c_th0 = Cos(th0);

            return r =>
            {
                var ph = r.PhiRad;
                var th = r.ThettaRad;

                var s_ph = Sin(ph);
                var c_ph = Cos(ph);
                var s_th = Sin(th);
                var c_th = Cos(th);

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
                return r.AngleType == AngleType.Rad ? new SpaceAngle(th1, ph1) : new SpaceAngle(th1, ph1).InDeg;
            };
        }


        public Expression GetRotator_PhiThetta_Expression(Expression r)
        {
            var a = this;
            var ph0 = a.PhiRad;
            var th0 = a.ThettaRad;
            if(ph0.Equals(0d) && th0.Equals(0d)) return r;

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
                th.Assign(r.GetProperty(nameof(ThettaRad))),
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

        [NotNull]
        public Func<SpaceAngle, T> Rotate_PhiThetta<T>(Func<SpaceAngle, T> f)
        {
            var r = GetRotator_PhiThetta();
            return a => f(r(a));
        }

        [DebuggerStepThrough]
        public SpaceAngle In(in AngleType type)
        {
            if(_AngleType == type) return this;
            switch(type)
            {
                case AngleType.Rad:
                    return InRad;
                case AngleType.Deg:
                    return InDeg;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /* -------------------------------------------------------------------------------------------- */

        public void Deconstruct(out double th, out double phi)
        {
            th = _Thetta;
            phi = _Phi;
        }

        [DebuggerStepThrough]
        public object Clone() => new SpaceAngle(this, AngleType);

        [DebuggerStepThrough]
        public override string ToString() => $"(Thetta:{_Thetta}; Phi:{_Phi}):{_AngleType}";

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            if(_AngleType == AngleType.Deg) return InRad.GetHashCode();
            unchecked { return (_Thetta.GetHashCode() * 397) ^ _Phi.GetHashCode(); }
        }

        [DebuggerStepThrough]
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is SpaceAngle a && Equals(a);

        /* -------------------------------------------------------------------------------------------- */



        /* -------------------------------------------------------------------------------------------- */

        public static SpaceAngle operator +(in SpaceAngle x, SpaceAngle y)
        {
            var (y_th, y_ph) = y.In(x._AngleType);
            return new SpaceAngle(x._Thetta + y_th, x._Phi + y_ph, x._AngleType);
        }

        public static SpaceAngle operator -(in SpaceAngle x, in SpaceAngle y)
        {
            var (y_th, y_ph) = y.In(x._AngleType);
            return new SpaceAngle(x._Thetta - y_th, x._Phi - y_ph, x._AngleType);
        }

        public static SpaceAngle operator -(in SpaceAngle a) => new SpaceAngle(-a._Thetta, -a._Phi, a._AngleType);

        public static SpaceAngle operator /(in SpaceAngle a, double x) => new SpaceAngle(a.Thetta / x, a.Phi / x, a.AngleType);

        public static bool operator ==(in SpaceAngle A, in SpaceAngle B) => A.Equals(B);

        public static bool operator !=(in SpaceAngle A, in SpaceAngle B) => !(A == B);

        public static implicit operator Vector3D(in SpaceAngle a) => new Vector3D(a);

        public static explicit operator SpaceAngle(in Vector3D v) => v.Angle;

        /// <summary>Оператор поворота функции на пространственный угол</summary>
        /// <param name="f">Вещественная пространственная функция</param>
        /// <param name="a">Пространственный угол поворота</param>
        /// <returns>Вещественная функция, аргумент которой повёрнут на указанный пространственный угол</returns>
        [NotNull]
        public static Func<SpaceAngle, double> operator ^(Func<SpaceAngle, double> f, in SpaceAngle a) => a.Rotate_PhiThetta(f);

        /// <summary>Оператор поворота функции на пространственный угол</summary>
        /// <param name="f">Комплексная пространственная функция</param>
        /// <param name="a">Пространственный угол поворота</param>
        /// <returns>Комплексная функция, аргумент которой повёрнут на указанный пространственный угол</returns>
        [NotNull]
        public static Func<SpaceAngle, Complex> operator ^(Func<SpaceAngle, Complex> f, in SpaceAngle a) => a.Rotate_PhiThetta(f);

        /* -------------------------------------------------------------------------------------------- */

        #region IEquatable<SpaceAngle> Members

        /// <summary>Точность сравнения (по умолчанию 10^-13)</summary>
        public static double ComparisonsAccuracy { get; set; } = 1e-13;

        public bool Equals(SpaceAngle other) => Equals(other, ComparisonsAccuracy);

        public bool Equals(SpaceAngle other, double eps)
        {
            if(other._AngleType != _AngleType)
                other = other.In(_AngleType);
            var max = _AngleType == AngleType.Rad ? Consts.pi2 : 360;
            return
                Abs(_Thetta % max + (_Thetta < 0 ? max : 0) - (other._Thetta % max + (other._Thetta < 0 ? max : 0))) % max < eps &&
                Abs(_Phi % max + (_Phi < 0 ? max : 0) - (other._Phi % max + (other._Phi < 0 ? max : 0))) % max < eps;
        }

        bool IEquatable<SpaceAngle>.Equals(SpaceAngle other) => Equals(other);

        #endregion
    }
}
