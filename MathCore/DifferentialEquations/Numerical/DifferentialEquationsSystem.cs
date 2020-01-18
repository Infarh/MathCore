namespace MathCore.DifferentialEquations.Numerical
{
    /// <summary> Система дифференциальных уравнений</summary>
    /// <param name="X">Аргумент</param>
    /// <param name="Y">Значения функции</param>
    /// <returns>Значения производных</returns>
    public delegate double[] DifferentialEquationsSystem(double[] X, double[] Y);
}