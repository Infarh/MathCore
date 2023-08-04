namespace MathCore.Interpolation;

/// <summary>Интерфейс интерполяции</summary>
public interface IInterpolator
{
    double this[double x] { get; }

    /// <summary>Получить значение</summary>
    /// <param name="x">Переменная</param>
    /// <returns>Значение</returns>
    double Value(double x);

    Func<double, double> GetFunction();
}