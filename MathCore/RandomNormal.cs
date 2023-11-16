#nullable enable
using static System.Math;

namespace MathCore;

/// <summary>Генератор случайных чисел с нормальным распределением</summary>
public class RandomNormal : Random
{
    private double _Sigma = 1;
    private double _Mu;

    /// <summary>Среднеквадратичное отклонение</summary>
    public double Sigma { get => _Sigma; set => _Sigma = value; }

    /// <summary>Математическое ожидание</summary>
    public double Mu { get => _Mu; set => _Mu = value; }

    public RandomNormal() { }

    public RandomNormal(int Seed) : base(Seed) { }

    protected override double Sample()
    {
        var r2 = 0d;
        var x  = base.Sample() * 2 - 1;
        for (var y = base.Sample() * 2 - 1; r2 is > 1 or 0d; y = x, x = base.Sample() * 2 - 1) 
            r2 = x * x + y * y;

        return Sqrt(-2 * Log(r2) / r2) * x * _Sigma + _Mu;
    }
}