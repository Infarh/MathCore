namespace MathCore.Optimization.ParticleSwarm;

/// <summary>Оптимизация двумерных функций методом роя частиц</summary>
/// <remarks>
/// Специализированная реализация алгоритма PSO для функций двух переменных.
/// Оптимизирована для производительности и удобства использования благодаря разделению координат X и Y.
/// Рекомендуется для двумерных задач вместо универсального класса Swarm.
/// </remarks>
public class Swarm2D(int ParticleCount = 100)
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

    /// <summary>Частица двумерного пространства</summary>
    private class Particle2D
    {
        private double _BestValue;
        public double BestX;
        public double BestY;
        public double Value;
        public double X;
        public double Y;

        public Particle2D(double X, double Y, double Value)
        {
            this.X = X;
            this.Y = Y;
            this.Value = Value;
            SetBest();
        }

        private void SetBest()
        {
            _BestValue = Value;
            BestX = X;
            BestY = Y;
        }

        public void SetMin(Func<double, double, double> F)
        {
            Value = F(X, Y);
            if (Value < _BestValue)
                SetBest();
        }

        public void SetMax(Func<double, double, double> F)
        {
            Value = F(X, Y);
            if (Value > _BestValue)
                SetBest();
        }

        public void Deconstruct(out double X, out double Y, out double Value) => (X, Y, Value) = (this.X, this.Y, this.Value);
    }

    private static readonly Random __Random = new();

    /// <summary>Размер роя частиц</summary>
    private readonly int _ParticleCount = ParticleCount;

    private double Get(in Interval Interval, double X, double CurrentBestX, double GlobalBestX)
    {
        var dx_local = _LocalWeight * __Random.NextDouble() * (CurrentBestX - X);
        var dx_global = _GlobalWeight * __Random.NextDouble() * (GlobalBestX - X);
        var v = _Inertia * X + dx_local + dx_global;
        return Interval.Normalize(X + v);
    }

    /// <summary>Находит минимум функции двух переменных методом роя частиц</summary>
    /// <param name="F">Целевая функция двух переменных для минимизации</param>
    /// <param name="MinX">Минимальное значение границы поиска по оси X</param>
    /// <param name="MaxX">Максимальное значение границы поиска по оси X</param>
    /// <param name="MinY">Минимальное значение границы поиска по оси Y</param>
    /// <param name="MaxY">Максимальное значение границы поиска по оси Y</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Координата X найденной точки минимума</param>
    /// <param name="Y">Координата Y найденной точки минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример минимизации функции Бута: f(x,y) = (x + 2y - 7)² + (2x + y - 5)²</para>
    /// <code><![CDATA[
    /// var swarm2d = new Swarm2D(ParticleCount: 50);
    /// swarm2d.Inertia = 0.72;
    /// swarm2d.LocalWeight = 1.49445;
    /// swarm2d.GlobalWeight = 1.49445;
    /// 
    /// Func<double, double, double> boothFunction = (x, y) =>
    /// {
    ///     var a = x + 2 * y - 7;
    ///     var b = 2 * x + y - 5;
    ///     return a * a + b * b;
    /// };
    /// 
    /// swarm2d.Minimize(
    ///     F: boothFunction,
    ///     MinX: -10,
    ///     MaxX: 10,
    ///     MinY: -10,
    ///     MaxY: 10,
    ///     IterationCount: 150,
    ///     out var optimalX,
    ///     out var optimalY,
    ///     out var minValue
    /// );
    /// 
    /// Console.WriteLine($"Оптимум: x = {optimalX:F4}, y = {optimalY:F4}");
    /// Console.WriteLine($"Минимальное значение: {minValue:F4}");
    /// // Ожидаемый результат: x ≈ 1, y ≈ 3, f ≈ 0
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double, double, double> F,
        in double MinX,
        in double MaxX,
        in double MinY,
        in double MaxY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value) =>
        Minimize(F, new(MinX, MaxX), new(MinY, MaxY), IterationCount, out X, out Y, out Value);

    /// <summary>Находит минимум функции двух переменных методом роя частиц с использованием интервалов</summary>
    /// <param name="F">Целевая функция двух переменных для минимизации</param>
    /// <param name="IntervalX">Интервал поиска по оси X</param>
    /// <param name="IntervalY">Интервал поиска по оси Y</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Координата X найденной точки минимума</param>
    /// <param name="Y">Координата Y найденной точки минимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <remarks>
    /// Этот метод предпочтительнее использовать для точного задания доменов поиска по каждой оси
    /// </remarks>
    /// <example>
    /// <para>Пример минимизации функции Бизероу</para>
    /// <code><![CDATA[
    /// var swarm = new Swarm2D(80);
    /// swarm.Inertia = 0.729;
    /// 
    /// Func<double, double, double> beale = (x, y) =>
    /// {
    ///     var a = 1.5 - x + x * y;
    ///     var b = 2.25 - x + x * y * y;
    ///     var c = 2.625 - x + x * y * y * y;
    ///     return a * a + b * b + c * c;
    /// };
    /// 
    /// swarm.Minimize(
    ///     F: beale,
    ///     IntervalX: new Interval(-4.5, 4.5),
    ///     IntervalY: new Interval(-4.5, 4.5),
    ///     IterationCount: 200,
    ///     out var optX,
    ///     out var optY,
    ///     out var fValue
    /// );
    /// 
    /// Console.WriteLine($"Найденный оптимум: ({optX:F4}, {optY:F4})");
    /// Console.WriteLine($"Значение функции: {fValue:F4}");
    /// ]]></code>
    /// </example>
    public void Minimize(
        in Func<double, double, double> F,
        in Interval IntervalX,
        in Interval IntervalY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var delta_y = IntervalY.Length;
        var min_x = IntervalX.Min;
        var min_y = IntervalY.Min;

        var swarm = new Particle2D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            var y = __Random.NextDouble() * delta_y + min_y;
            swarm[i] = new(x, y, F(x, y));
        }

        (X, Y, Value) = swarm.GetMin(p => p.Value)!;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.Y = Get(IntervalY, p.Y, p.BestY, Y);
                p.SetMin(F);

                if (p.Value >= Value) continue;

                (X, Y, Value) = p;
            }
    }

    /// <summary>Находит максимум функции двух переменных методом роя частиц</summary>
    /// <param name="F">Целевая функция двух переменных для максимизации</param>
    /// <param name="MinX">Минимальное значение границы поиска по оси X</param>
    /// <param name="MaxX">Максимальное значение границы поиска по оси X</param>
    /// <param name="MinY">Минимальное значение границы поиска по оси Y</param>
    /// <param name="MaxY">Максимальное значение границы поиска по оси Y</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Координата X найденной точки максимума</param>
    /// <param name="Y">Координата Y найденной точки максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример максимизации функции Химмельблау: f(x,y) = (x² + y - 11)² + (x + y² - 7)²</para>
    /// <code><![CDATA[
    /// var swarm2d = new Swarm2D(ParticleCount: 100);
    /// swarm2d.Inertia = 0.75;
    /// swarm2d.LocalWeight = 1.5;
    /// swarm2d.GlobalWeight = 1.5;
    /// 
    /// Func<double, double, double> himmelblau = (x, y) =>
    /// {
    ///     var a = x * x + y - 11;
    ///     var b = x + y * y - 7;
    ///     return -(a * a + b * b); // инвертируем для максимизации
    /// };
    /// 
    /// swarm2d.Maximize(
    ///     F: himmelblau,
    ///     MinX: -5,
    ///     MaxX: 5,
    ///     MinY: -5,
    ///     MaxY: 5,
    ///     IterationCount: 300,
    ///     out var bestX,
    ///     out var bestY,
    ///     out var maxValue
    /// );
    /// 
    /// Console.WriteLine($"Найденный оптимум: ({bestX:F4}, {bestY:F4})");
    /// Console.WriteLine($"Значение функции: {maxValue:F4}");
    /// ]]></code>
    /// </example>
    public void Maximize(
        in Func<double, double, double> F,
        in double MinX,
        in double MaxX,
        in double MinY,
        in double MaxY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value) =>
        Maximize(
            F: F,
            IntervalX: new(Min: MinX, Max: MaxX),
            IntervalY: new(Min: MinY, Max: MaxY),
            IterationCount: IterationCount,
            X: out X,
            Y: out Y,
            Value: out Value);

    /// <summary>Находит максимум функции двух переменных методом роя частиц с использованием интервалов</summary>
    /// <param name="F">Целевая функция двух переменных для максимизации</param>
    /// <param name="IntervalX">Интервал поиска по оси X</param>
    /// <param name="IntervalY">Интервал поиска по оси Y</param>
    /// <param name="IterationCount">Количество итераций алгоритма</param>
    /// <param name="X">Координата X найденной точки максимума</param>
    /// <param name="Y">Координата Y найденной точки максимума</param>
    /// <param name="Value">Значение функции в найденной точке</param>
    /// <example>
    /// <para>Пример максимизации функции Розенброка: минимизируем f(x,y) = (1-x)² + 100(y-x²)²</para>
    /// <code><![CDATA[
    /// var swarm2d = new Swarm2D(150);
    /// swarm2d.Inertia = 0.729;
    /// swarm2d.LocalWeight = 1.49445;
    /// swarm2d.GlobalWeight = 1.49445;
    /// 
    /// Func<double, double, double> rosenbrock = (x, y) =>
    /// {
    ///     var a = 1 - x;
    ///     var b = y - x * x;
    ///     return -(a * a + 100 * b * b); // инвертируем
    /// };
    /// 
    /// swarm2d.Maximize(
    ///     F: rosenbrock,
    ///     IntervalX: new Interval(-2, 2),
    ///     IntervalY: new Interval(-1, 3),
    ///     IterationCount: 500,
    ///     out var optX,
    ///     out var optY,
    ///     out var maxValue
    /// );
    /// 
    /// Console.WriteLine($"Найденный оптимум: ({optX:F4}, {optY:F4})");
    /// Console.WriteLine($"Значение функции: {maxValue:F4}");
    /// // Ожидаемое приблизительно: (1, 1)
    /// ]]></code>
    /// </example>
    public void Maximize(
        in Func<double, double, double> F,
        in Interval IntervalX,
        in Interval IntervalY,
        in int IterationCount,
        out double X,
        out double Y,
        out double Value)
    {
        var delta_x = IntervalX.Length;
        var delta_y = IntervalY.Length;
        var min_x = IntervalX.Min;
        var min_y = IntervalY.Min;

        var swarm = new Particle2D[_ParticleCount];
        for (var i = 0; i < _ParticleCount; i++)
        {
            var x = __Random.NextDouble() * delta_x + min_x;
            var y = __Random.NextDouble() * delta_y + min_y;
            swarm[i] = new(x, y, F(x, y));
        }

        (X, Y, Value) = swarm.GetMax(p => p.Value)!;

        for (var i = 0; i < IterationCount; i++)
            foreach (var p in swarm)
            {
                p.X = Get(IntervalX, p.X, p.BestX, X);
                p.Y = Get(IntervalY, p.Y, p.BestY, Y);
                p.SetMax(F);

                if (p.Value <= Value) continue;

                (X, Y, Value) = p;
            }
    }
}