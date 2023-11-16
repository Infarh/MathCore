#nullable enable
using static System.Math;
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AnnotateNotNullTypeMember

// ReSharper disable UnusedMember.Global

namespace MathCore.Vectors;

/// <summary>Двумерный базис</summary>
/// <remarks>Инициализация нового экземпляра <see cref="Basis2D"/></remarks>
/// <param name="xx">Элемент X[0,0]</param>
/// <param name="xy">Элемент X[0,1]</param>
/// <param name="yx">Элемент X[1,0]</param>
/// <param name="yy">Элемент X[1,1]</param>
public readonly struct Basis2D(double xx, double xy, double yx, double yy) : IEquatable<Basis2D>
{
    /// <summary>Базис Евклидова пространства</summary>
    public static readonly Basis2D Euclid = new(
        1, 0,
        0, 1);

    /// <summary>Базис поворота вектора на заданный угол</summary>
    /// <param name="Angle">Угол поворота пространства</param>
    /// <param name="Positive">Направление поворота по часовой стрелке</param>
    /// <returns>Базис, осуществляющий поворот вектора в пространстве на указанный угол</returns>
    public static Basis2D Rotate(double Angle, bool Positive = true) =>
        new(
            Cos(Angle), (Positive ? -1 : 1) * Sin(Angle),
            (Positive ? 1 : -1) * Sin(Angle), Cos(Angle));

    /// <summary>Базис масштабирования вектора по осям</summary>
    /// <param name="kx">Коэффициент масштабирования вдоль оси OX</param>
    /// <param name="ky">Коэффициент масштабирования вдоль оси OY</param>
    /// <returns>Базис, осуществляющий масштабирования вектора по осям</returns>
    public static Basis2D Scale(double kx, double ky) =>
        new(
            kx, 0,
            0, ky);

#pragma warning disable IDE1006 // Стили именования

    /// <summary>Элемент X[0,0]</summary>
    private readonly double _xx = xx;

    /// <summary>Элемент X[0,0]</summary>
    public double xx { get => _xx; init => _xx = value; }

    /// <summary>Элемент X[0,1]</summary>
    private readonly double _xy = xy;

    /// <summary>Элемент X[0,1]</summary>
    public double xy { get=> _xy; init=> _xy = value; }

    /// <summary>Элемент X[1,0]</summary>
    private readonly double _yx = yx;

    /// <summary>Элемент X[1,0]</summary>
    public double yx { get => _yx; init => _yx = value; }

    /// <summary>Элемент X[1,1]</summary>
    private readonly double _yy = yy;

    /// <summary>Элемент X[1,1]</summary>
    public double yy { get => _yy; init => _yy = value; }

    /// <summary>Оператор неявного преобразования базиса в матрицу 2х2</summary>
    public static implicit operator Matrix(in Basis2D b) => new(new[,] { { b._xx, b._xy }, { b._yx, b._yy } });

    /// <summary>Оператор явного преобразования матрицы 2х2 в двумерный базис</summary>
    public static explicit operator Basis2D(in Matrix M) => new(M[0, 0], M[0, 1], M[1, 0], M[1, 1]);

    /// <inheritdoc />
    public bool Equals(Basis2D other) => _xx.Equals(other._xx) && _xy.Equals(other._xy) && _yx.Equals(other._yx) && _yy.Equals(other._yy);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Basis2D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash_code = _xx.GetHashCode();
            hash_code = (hash_code * 397) ^ _xy.GetHashCode();
            hash_code = (hash_code * 397) ^ _yx.GetHashCode();
            hash_code = (hash_code * 397) ^ _yy.GetHashCode();
            return hash_code;
        }
    }

    /// <summary>Оператор равенства двух базисов</summary>
    public static bool operator ==(Basis2D left, Basis2D right) => left.Equals(right);

    /// <summary>Оператор неравенства двух базисов</summary>
    public static bool operator !=(Basis2D left, Basis2D right) => !left.Equals(right);
}