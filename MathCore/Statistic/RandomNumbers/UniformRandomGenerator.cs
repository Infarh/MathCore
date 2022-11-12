// ReSharper disable UnusedMember.Global

using System;

namespace MathCore.Statistic.RandomNumbers;

/// <summary>Генератор случайных чисел с равномерным распределением</summary>
public class UniformRandomGenerator : RandomGenerator
{
    public UniformRandomGenerator(Random rnd = null) : base(rnd) { }
    public UniformRandomGenerator(double Sigma, Random rnd = null) : base(Sigma, rnd) { }
    public UniformRandomGenerator(double Sigma, double mu, Random rnd = null) : base(Sigma, mu, rnd) { }

    public override double Distribution(double x)
    {
        var sigma2 = Sigma / 2;
        return Distributions.Uniform(x, Mu - sigma2, Mu + sigma2);
    }

    protected override double GetNextValue() => _Random.NextDouble() - 0.5;
}