using MathCore.Vectors;

namespace MathCore.DifferentialEquations.Numerical;

/// <summary>Система двухмерных векторных дифференциальных уравнений</summary>
/// <param name="t">Аргумент</param>
/// <param name="V2">Двухмерные векторные значения функции</param>
/// <returns>Двухмерные векторные значения производных</returns>
public delegate Vector2D[] DiffEqsVector2D(double t, Vector2D[] V2);