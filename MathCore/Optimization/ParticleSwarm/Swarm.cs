namespace MathCore.Optimization.ParticleSwarm;

/// <summary>Оптимизация многомерных функций методом роя частиц</summary>
/// <remarks>
/// Реализует алгоритм оптимизации роем частиц (PSO) для решения задач минимизации и максимизации функций произвольной размерности.
/// Подходит для функций трёх и более переменных. Для одномерных и двумерных задач рекомендуется использовать специализированные классы Swarm1D и Swarm2D.
/// </remarks>
public class Swarm(int ParticleCount = 100)
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
            < 0 => throw new ArgumentOutOfRangeException(nameof(value), value, nameof(Inertia) + " величина должна быть > 0"),
            >= 1 => throw new ArgumentOutOfRangeException(nameof(value), value, nameof(Inertia) + " величина должна быть < 1"),
            _ => value
        };
    }

    /// <summary>Коэффициент локального веса (по умолчанию 1.49445)</summary>
    private double _LocalWeight = 1.49445; // cognitive/local weight

    /// <summary>Получает или устанавливает коэффициент локального веса</summary>
    /// <value>
    /// Коэффициент стремления каждой частицы к своему собственному лучшему решению.
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

    private class Particle(double[] X, double Value)
    {
        private double _BestValue = Value;
        public readonly double[] BestX = (double[])X.Clone();
        public double Value = Value;
        public readonly double[] X = X;

        private void SetBest()
        {
            _BestValue = Value;
            X.CopyTo(BestX, 0);
        }

        public void SetMin(Func<double[], double> F)
        {
            Value = F(X);
            if (Value < _BestValue)
                SetBest();
        }

        public void SetMax(Func<double[], double> F)
        {
            Value = F(X);
            if (Value > _BestValue)
                SetBest();
        }
    }

    private static readonly Random __Random = new();

    /// <summary>Находит минимум функции MANY переменных методом роя частиц</summary>
    /// <param name="F">Целевая функция для минимизации</param>
    /// <param name="MinX">Минимальные значения границ для каждой переменной</param>
    /// <param name="MaxX">Максимальные значения границ для каждой переменной</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример минимизации функции Сферы: f(x) = x₁² + x₂² + x₃²</para>
    /// <code><![CDATA[
    /// var swarm = new Swarm(ParticleCount: 50);
    /// swarm.Inertia = 0.7;
    /// swarm.LocalWeight = 1.49445;
    /// swarm.GlobalWeight = 1.49445;
    /// 
    /// Func<double[], double> sphereFunction = x => x.Sum(v => v * v);
    /// 
    /// swarm.Minimize(
    ///     F: sphereFunction,
    ///     MinX: [-5, -5, -5],
    ///     MaxX: [5, 5, 5],
    ///     IterationCount: 100,
    ///     out var bestX,
    ///     out var bestValue
    /// );
    /// 
    /// Console.WriteLine($"Минимум найден в точке: ({string.Join(", ", bestX)})");
    /// Console.WriteLine($"Значение функции: {bestValue:F6}");
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double[], double> F,
        in double[] MinX,
        in double[] MaxX,
        int IterationCount,
        out double[] X,
        out double Value) =>
        Minimize(F, MinX.Zip(MaxX, (min, max) => new Interval(min, max)).ToArray(), IterationCount, out X, out Value);

    /// <summary>Находит минимум функции многих переменных методом роя частиц с использованием интервалов</summary>
    /// <param name="F">Целевая функция для минимизации</param>
    /// <param name="IntervalX">Интервалы поиска для каждой переменной</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <remarks>
    /// Этот метод предпочтительнее использовать для точного задания доменов поиска через Interval
    /// </remarks>
    /// <example>
    /// <para>Пример минимизации функции Растригина: f(x) = 10n + Σ(xᵢ² - 10cos(2πxᵢ))</para>
    /// <code><![CDATA[
    /// var swarm = new Swarm(100);
    /// swarm.Inertia = 0.729;
    /// 
    /// Func<double[], double> rastrigin = x => 
    ///     10 * x.Length + x.Sum(v => v * v - 10 * Math.Cos(2 * Math.PI * v));
    /// 
    /// var intervals = new[]
    /// {
    ///     new Interval(-5.12, 5.12),
    ///     new Interval(-5.12, 5.12)
    /// };
    /// 
    /// swarm.Minimize(
    ///     F: rastrigin,
    ///     IntervalX: intervals,
    ///     IterationCount: 200,
    ///     out var optimalX,
    ///     out var minValue
    /// );
    /// 
    /// Console.WriteLine($"Оптимум найден в точке: ({string.Join(", ", optimalX.Select(v => v.ToString("F4")))})");
    /// Console.WriteLine($"Минимальное значение: {minValue:F4}");
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double[], double> F,
        in Interval[] IntervalX,
        in int IterationCount,
        out double[] X,
        out double Value)
    {
        var dimensions = IntervalX.Length;

        var swarm = new Particle[ParticleCount];
        for (var i = 0; i < ParticleCount; i++)
        {
            var xx = __Random.NextUniform(IntervalX);
            swarm[i] = new(xx, F(xx));
        }
        X = new double[dimensions];

        var start = swarm.GetMin(p => p.Value);
        start!.X.CopyTo(X, 0);
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                for (var j = 0; j < dimensions; j++)
                {
                    var x = p.X[j];
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var v = _Inertia * x + _LocalWeight * r1 * (p.BestX[j] - x) + _GlobalWeight * r2 * (X[j] - x);
                    p.X[j] = IntervalX[j].Normalize(x + v);
                }
                p.SetMin(F);

                if (p.Value >= Value) continue;

                p.X.CopyTo(X, 0);
                Value = p.Value;
            }
    }

    /// <summary>Находит максимум функции MANY переменных методом роя частиц</summary>
    /// <param name="F">Целевая функция для максимизации</param>
    /// <param name="MinX">Минимальные значения границ для каждой переменной</param>
    /// <param name="MaxX">Максимальные значения границ для каждой переменной</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример максимизации функции Акли</para>
    /// <code><![CDATA[
    /// var swarm = new Swarm(ParticleCount: 75);
    /// swarm.Inertia = 0.75;
    /// 
    /// Func<double[], double> ackley = x =>
    /// {
    ///     var a = 20.0;
    ///     var b = 0.2;
    ///     var c = 2 * Math.PI;
    ///     var d = x.Length;
    ///     var sum1 = x.Sum(v => v * v);
    ///     var sum2 = x.Sum(v => Math.Cos(c * v));
    ///     return -a * Math.Exp(-b * Math.Sqrt(sum1 / d)) - Math.Exp(sum2 / d) + a + Math.E;
    /// };
    /// 
    /// swarm.Maximize(
    ///     F: x => -ackley(x), // инвертируем для максимизации
    ///     MinX: [-32, -32],
    ///     MaxX: [32, 32],
    ///     IterationCount: 250,
    ///     out var bestX,
    ///     out var bestValue
    /// );
    /// ]]></code>
    /// </example>
    public void Maximize(
        in Func<double[], double> F,
        in double[] MinX,
        in double[] MaxX,
        in int IterationCount,
        out double[] X,
        out double Value) =>
        Maximize(F, MinX.Zip(MaxX, (min, max) => new Interval(min, max)).ToArray(), IterationCount, out X, out Value);

    /// <summary>Находит максимум функции многих переменных методом роя частиц с использованием интервалов</summary>
    /// <param name="F">Целевая функция для максимизации</param>
    /// <param name="IntervalX">Интервалы поиска для каждой переменной</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Найденная точка максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    public void Maximize(
        in Func<double[], double> F,
        in Interval[] IntervalX,
        int IterationCount,
        out double[] X,
        out double Value)
    {
        var dimensions = IntervalX.Length;

        var swarm = new Particle[ParticleCount];
        for (var i = 0; i < ParticleCount; i++)
        {
            var xx = __Random.NextUniform(IntervalX);
            swarm[i] = new(xx, F(xx));
        }
        X = new double[dimensions];

        var start = swarm.GetMax(p => p.Value);
        start!.X.CopyTo(X, 0);
        Value = start.Value;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                for (var j = 0; j < dimensions; j++)
                {
                    var x = p.X[j];
                    var r1 = __Random.NextDouble();
                    var r2 = __Random.NextDouble();

                    var v = _Inertia * x + _LocalWeight * r1 * (p.BestX[j] - x) + _GlobalWeight * r2 * (X[j] - x);
                    p.X[j] = IntervalX[j].Normalize(x + v);
                }
                p.SetMax(F);

                if (p.Value <= Value) continue;
                p.X.CopyTo(X, 0);
                Value = p.Value;
            }
    }
}