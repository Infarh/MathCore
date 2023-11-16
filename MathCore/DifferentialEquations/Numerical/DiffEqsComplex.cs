namespace MathCore.DifferentialEquations.Numerical;

/// <summary>Система комплексных дифференциальных уравнений</summary>
/// <param name="t">Аргумент</param>
/// <param name="Z">Комплексные значения функции</param>
/// <returns>Комплексные значения производных</returns>
public delegate Complex[] DiffEqsComplex(double t, IReadOnlyList<Complex> Z);