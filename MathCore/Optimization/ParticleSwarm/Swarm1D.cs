namespace MathCore.Optimization.ParticleSwarm;

/// <summary>Оптимизация одномерных функций методом роя частиц</summary>
/// <remarks>
/// Специализированная реализация алгоритма PSO для функций одного переменного. 
/// Оптимизирована для производительности благодаря использованию скалярных операций вместо массивов.
/// Рекомендуется для одномерных задач вместо универсального класса Swarm.
/// </remarks>
public class Swarm1D(int ParticleCount = 100)
{
    /// <summary>Вес инерции</summary>
    private double _Inertia;

    /// <summary>Получает или устанавливает вес инерции (допустимый диапазон: 0 &lt; значение &lt; 1)</summary>
    /// <value>
    /// Вес инерции, влияющий на степень использования предыдущей скорости частицы.
    /// Рекомендуемые значения: 0.4-0.9. Большие значения улучшают глобальный поиск, малые - локальный
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Выбрасывается, если значение &lt;= 0 или &gt;= 1
    /// </exception>
    public double Inertia
    {
        get => _Inertia;
        set => _Inertia = value switch
        {
            < 0 => throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(Inertia)} величина должна быть > 0"),
            >= 1 => throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(Inertia)} величина должна быть < 1"),
            _ => value
        };
    }

    /// <summary>Коэффициент локального веса (по умолчанию 1.49445)</summary>
    private double _LocalWeight = 1.49445; // cognitive/local weight

    /// <summary>Получает или устанавливает коэффициент локального веса</summary>
    /// <value>
    /// Коэффициент стремления частицы к своему собственному лучшему решению.
    /// Рекомендуемые значения: 1.4-2.0. По умолчанию 1.49445
    /// </value>
    public double LocalWeight { get => _LocalWeight; set => _LocalWeight = value; }

    /// <summary>Коэффициент глобального веса (по умолчанию 1.49445)</summary>
    private double _GlobalWeight = 1.49445; // social/global weight

    /// <summary>Получает или устанавливает коэффициент глобального веса</summary>
    /// <value>
    /// Коэффициент стремления всех частиц к лучшему решению, найденному всем роем.
    /// Рекомендуемые значения: 1.4-2.0. По умолчанию 1.49445
    /// </value>
    public double GlobalWeight { get => _GlobalWeight; set => _GlobalWeight = value; }

    private class Particle1D
    {
        private double _BestValue;
        public double BestX;
        public double Value;
        public double X;

        public Particle1D(double X, double Value)
        {
            this.X = X;
            this.Value = Value;
            SetBest();
        }

        private void SetBest()
        {
            BestX = X;
            _BestValue = Value;
        }

        public void SetMin(Func<double, double> F)
        {
            Value = F(X);
            if (Value < _BestValue)
                SetBest();
        }

        public void SetMax(Func<double, double> F)
        {
            Value = F(X);
            if (Value < _BestValue)
                SetBest();
        }
    }

    private static readonly Random __Random = new();

    private double Get(in Interval interval, double px, double best, double x)
    {
        var v = _Inertia * px + _LocalWeight * __Random.NextDouble() * (best - px) + _GlobalWeight * __Random.NextDouble() * (x - px);
        return interval.Normalize(px + v);
    }

    /// <summary>Находит минимум функции одной переменной методом роя частиц</summary>
    /// <param name="F">Целевая функция для минимизации</param>
    /// <param name="MinX">Минимальное значение границы поиска</param>
    /// <param name="MaxX">Максимальное значение границы поиска</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример минимизации квадратичной функции: f(x) = x² - 4x + 5</para>
    /// <code><![CDATA[
    /// var swarm1d = new Swarm1D(ParticleCount: 30);
    /// swarm1d.Inertia = 0.7;
    /// swarm1d.LocalWeight = 1.49445;
    /// swarm1d.GlobalWeight = 1.49445;
    /// 
    /// Func<double, double> quadraticFunction = x => x * x - 4 * x + 5;
    /// 
    /// swarm1d.Minimize(
    ///     F: quadraticFunction,
    ///     MinX: -10,
    ///     MaxX: 10,
    ///     IterationCount: 50,
    ///     out var bestX,
    ///     out var bestValue
    /// );
    /// 
    /// Console.WriteLine($"Минимум в точке x = {bestX:F6}");
    /// Console.WriteLine($"Значение функции = {bestValue:F6}");
    /// // Ожидаемый результат: x ≈ 2, f(x) ≈ 1
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double, double> F,
        in double MinX,
        in double MaxX,
        in int IterationCount,
        out double X,
        out double Value) =>
        Minimize(F, new(MinX, MaxX), IterationCount, out X, out Value);

    /// <summary>Находит минимум функции одной переменной методом роя частиц с использованием интервала</summary>
    /// <param name="F">Целевая функция для минимизации</param>
    /// <param name="IntervalX">Интервал поиска</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <remarks>
    /// Этот метод предпочтительнее использовать для точного задания домена поиска
    /// </remarks>
    /// <example>
    /// <para>Пример минимизации синусоидальной функции: f(x) = sin(x)</para>
    /// <code><![CDATA[
    /// var swarm = new Swarm1D(50);
    /// swarm.Inertia = 0.75;
    /// 
    /// Func<double, double> sineFunction = x => Math.Sin(x);
    /// 
    /// swarm.Minimize(
    ///     F: sineFunction,
    ///     IntervalX: new Interval(-Math.PI, Math.PI),
    ///     IterationCount: 100,
    ///     out var optimalX,
    ///     out var minValue
    /// );
    /// 
    /// Console.WriteLine($"Минимум функции sin(x) в точке x = {optimalX:F6}");
    /// Console.WriteLine($"Значение функции = {minValue:F6}");
    /// // Ожидаемый результат: x ≈ -π/2, f(x) ≈ -1
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double, double> F,
        in Interval IntervalX,
        int IterationCount,
        out double X,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var min_x = IntervalX.Min;

        var swarm = new Particle1D[ParticleCount];
        for (var i = 0; i < ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            swarm[i] = new(x, F(x));
        }

        var start = swarm.GetMin(p => p.Value);
        X = start!.X;
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.SetMin(F);

                if (p.Value >= Value) continue;
                X = p.X;
                Value = p.Value;
            }
    }

    /// <summary>Находит максимум функции одной переменной методом роя частиц</summary>
    /// <param name="F">Целевая функция для максимизации</param>
    /// <param name="MinX">Минимальное значение границы поиска</param>
    /// <param name="MaxX">Максимальное значение границы поиска</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример максимизации функции: f(x) = -x² + 4x</para>
    /// <code><![CDATA[
    /// var swarm1d = new Swarm1D(ParticleCount: 40);
    /// swarm1d.Inertia = 0.8;
    /// swarm1d.LocalWeight = 1.5;
    /// swarm1d.GlobalWeight = 1.5;
    /// 
    /// Func<double, double> parabolaFunction = x => -x * x + 4 * x;
    /// 
    /// swarm1d.Maximize(
    ///     F: parabolaFunction,
    ///     MinX: -10,
    ///     MaxX: 10,
    ///     IterationCount: 80,
    ///     out var bestX,
    ///     out var bestValue
    /// );
    /// 
    /// Console.WriteLine($"Максимум в точке x = {bestX:F6}"); // ≈ 2
    /// Console.WriteLine($"Значение функции = {bestValue:F6}"); // ≈ 4
    /// ]]></code>
    /// </example>
    public void Maximize(
        in Func<double, double> F,
        in double MinX,
        in double MaxX,
        in int IterationCount,
        out double X,
        out double Value) =>
        Maximize(F, new(MinX, MaxX), IterationCount, out X, out Value);

    /// <summary>Находит максимум функции одной переменной методом роя частиц с использованием интервала</summary>
    /// <param name="F">Целевая функция для максимизации</param>
    /// <param name="IntervalX">Интервал поиска</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    public void Maximize(
        in Func<double, double> F,
        in Interval IntervalX,
        in int IterationCount,
        out double X,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var min_x = IntervalX.Min;

        var swarm = new Particle1D[ParticleCount];
        for (var i = 0; i < ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            swarm[i] = new(x, F(x));
        }

        var start = swarm.GetMin(p => p.Value);
        X = start!.X;
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.SetMax(F);

                if (p.Value <= Value) continue;
                X = p.X;
                Value = p.Value;
            }
    }
}