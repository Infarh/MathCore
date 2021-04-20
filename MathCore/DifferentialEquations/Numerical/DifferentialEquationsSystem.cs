using System.Collections.Generic;

namespace MathCore.DifferentialEquations.Numerical
{
    /// <summary> Система дифференциальных уравнений</summary>
    /// <param name="t">Аргумент</param>
    /// <param name="X">Значения функции</param>
    /// <returns>Значения производных</returns>
    public delegate double[] DifferentialEquationsSystem(double t, IReadOnlyList<double> X);
}