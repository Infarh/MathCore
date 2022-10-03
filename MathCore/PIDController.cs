namespace MathCore;

/// <summary>Пропорционально-интегрально-дифференциальный регулятор</summary>
public class PIDController
{
    /// <summary>Интеграл ошибки</summary>
    public double IntegralError { get; private set; }

    /// <summary>Ошибка предыдущего шага управления</summary>
    public double Error { get; private set; }

    /// <summary>Коэффициент пропорционального регулирования</summary>
    public double P { get; set; }

    /// <summary>Коэффициент интегрального регулирования</summary>
    public double I { get; set; }

    /// <summary>Коэффициент дифференциального регулирования</summary>
    public double D { get; set; }

    /// <summary>Инициализация нового PID-регулятора</summary>
    /// <param name="P">Коэффициент пропорционального регулирования</param>
    /// <param name="I">Коэффициент интегрального регулирования</param>
    /// <param name="D">Коэффициент дифференциального регулирования</param>
    public PIDController(double P, double I, double D)
    {
        this.P = P;
        this.I = I;
        this.D = D;
    }

    /// <summary>Обработка очередного значения</summary>
    /// <param name="Input">Текущее значение</param>
    /// <param name="Target">Требуемое значение</param>
    /// <param name="dt">Интервал времени, прошедший после предыдущего измерения</param>
    /// <returns>Регулирующий сигнал</returns>
    public double Process(double Input, double Target, double dt)
    {
        var error          = Target - Input;
        var error_integral = IntegralError + error * dt;
        var delta          = (error - Error) / dt;
        Error              = error;
        IntegralError      = error_integral;
        return error * P + error_integral * I + delta * D;
    }
}