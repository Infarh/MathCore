using MathCore.Vectors;

namespace MathCore.DifferentialEquations.Numerical
{
    /// <summary>Система Трёхмерных векторных дифференциальных уравнений</summary>
    /// <param name="t">Аргумент</param>
    /// <param name="V3">Трёхмерные векторные значения функции</param>
    /// <returns>Трёхмерные векторные значения производных</returns>
    public delegate Vector3D[] DiffEqsVector3D(double t, Vector3D[] V3);
}