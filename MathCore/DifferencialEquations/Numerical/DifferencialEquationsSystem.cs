
namespace MathCore.DifferencialEquations.Numerical
{
    /// <summary> Система дифференциальных уравнений</summary>
    /// <param name="X">Аргумент</param>
    /// <param name="Y">Значения функции</param>
    /// <returns>Значения производных</returns>
    public delegate double[] DifferencialEquationsSystem(double[] X, double[] Y);
}
