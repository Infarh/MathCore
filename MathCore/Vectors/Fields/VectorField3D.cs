#nullable enable
using System;

namespace MathCore.Vectors.Fields;

/// <summary>Трёхмерное векторное поле</summary>
/// <param name="r">Координата в пространстве</param>
/// <returns>Значение вектора в точке с указанными координатами</returns>
public delegate Vector3D VectorField3D(Vector3D r);

/// <summary>Методы-расширения над <see cref="VectorField3D"/></summary>
public static class VectorField3DExtensions
{
    public static VectorField3D GetVectorField(Func<double, double> fx, Func<double, double> fy, Func<double, double> fz) => r => new Vector3D(fx(r.X), fy(r.Y), fz(r.Z));
}