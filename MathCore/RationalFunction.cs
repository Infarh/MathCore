#nullable enable
namespace MathCore;

/// <summary>Рациональная функция - отношение полиномов</summary>
public class RationalFunction(double[] P, double[] Q) : ICloneable
{
    /// <summary>Массив коэффициентов числителя</summary>
    private readonly double[] _P = P.NotNull();

    /// <summary>Массив коэффициентов знаменателя</summary>
    private readonly double[] _Q = Q.NotNull();

    /// <summary>полином числителя</summary>
    public Polynom P => _PolynomP ??= new(_P);
    private Polynom? _PolynomP;

    /// <summary>Полином знаменателя</summary>
    public Polynom Q => _PolynomQ ??= new(_Q);
    private Polynom? _PolynomQ;

    public RationalFunction(Polynom P, Polynom Q) : this(P.Coefficients, Q.Coefficients) { }

    /// <summary>Вычислить значение рациональной функции</summary>
    /// <param name="x">Аргумент рациональной функции</param>
    /// <returns>Значение рациональной функции</returns>
    /// <exception cref="DivideByZeroException">При <paramref name="x"/> - корне знаменателя</exception>
    public double Value(double x)
    {
        var p = Polynom.Array.GetValue(x, _P);
        var q = Polynom.Array.GetValue(x, _Q);
        return p / q;
    }

    /// <summary>Вычислить значение рациональной функции</summary>
    /// <param name="z">Комплексный аргумент рациональной функции</param>
    /// <returns>Комплексное значение рациональной функции</returns>
    /// <exception cref="DivideByZeroException">При <paramref name="z"/> - корне знаменателя</exception>
    public Complex Value(Complex z)
    {

        var p = Polynom.Array.GetValue(z, _P);
        var q = Polynom.Array.GetValue(z, _Q);
        return p / q;
    }



    public (Polynom Quotient, RationalFunction Remainder) Simplify()
    {
        Polynom.Array.Divide(_P, _Q, out var quotient, out var remainder);
        return (new(quotient), new(remainder, _P));
    }

    public RationalFunction GetInverse() => new(_Q.CloneObject(), _P.CloneObject());

    public object Clone() => new RationalFunction(_P.CloneObject(), _Q.CloneObject());

    public static RationalFunction operator +(RationalFunction x, RationalFunction y)
    {
        var x_p = Polynom.Array.Multiply(x._P, y._Q);
        var y_p = Polynom.Array.Multiply(y._P, x._Q);
        var p = Polynom.Array.Sum(x_p, y_p);
        var q = Polynom.Array.Multiply(x._Q, y._Q);
        return new(p, q);
    }


    public static RationalFunction operator -(RationalFunction x, RationalFunction y)
    {
        var x_p = Polynom.Array.Multiply(x._P, y._Q);
        var y_p = Polynom.Array.Multiply(y._P, x._Q);
        var p = Polynom.Array.Subtract(x_p, y_p);
        var q = Polynom.Array.Multiply(x._Q, y._Q);
        return new(p, q);
    }

    public static RationalFunction operator +(RationalFunction x, double y)
    {
        var k = Polynom.Array.Multiply(x._Q, y);
        var p = Polynom.Array.Sum(x._P, k);
        return new(p, x._Q.CloneObject());
    }

    public static RationalFunction operator -(RationalFunction x, double y)
    {
        var k = Polynom.Array.Multiply(x._Q, y);
        var p = Polynom.Array.Subtract(x._P, k);
        return new(p, x._Q.CloneObject());
    }

    public static RationalFunction operator +(double x, RationalFunction y)
    {
        var k = Polynom.Array.Multiply(y._Q, x);
        var p = Polynom.Array.Sum(y._P, k);
        return new(p, y._Q.CloneObject());
    }

    public static RationalFunction operator -(double x, RationalFunction y)
    {
        var k = Polynom.Array.Multiply(y._Q, x);
        var p = Polynom.Array.Subtract(k, y._P);
        return new(p, y._Q.CloneObject());
    }

    public static RationalFunction operator *(RationalFunction x, double y)
    {
        var p = Polynom.Array.Multiply(x._Q, y);
        return new(p, x._Q.CloneObject());
    }

    public static RationalFunction operator /(RationalFunction x, double y)
    {
        var q = Polynom.Array.Multiply(x._Q, y);
        return new(x._P.CloneObject(), q);
    }

    public static RationalFunction operator *(double x, RationalFunction y) => y * x;

    public static RationalFunction operator /(double x, RationalFunction y)
    {
        var p = Polynom.Array.Multiply(y._Q, x);
        return new(p, y._P.CloneObject());
    }
}
