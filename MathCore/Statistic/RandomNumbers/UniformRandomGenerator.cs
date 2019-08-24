// ReSharper disable UnusedMember.Global
namespace MathCore.Statistic.RandomNumbers
{
    /// <summary>Генератор случайных чисел с равномерным распределением</summary>
    public class UniformRandomGenerator : RandomGenerator
    {
        public UniformRandomGenerator() { }
        public UniformRandomGenerator(double Sigma) : base(Sigma) { }
        public UniformRandomGenerator(double Sigma, double mu) : base(Sigma, mu) { }

        public override double Distribution(double x)
        {
            var sigma2 = sigma / 2;
            return Distributions.Uniform(x, mu - sigma2, mu + sigma2);
        }

        protected override double GetNextValue() => SystemRandomGenerator.Value.NextDouble() - 0.5;
    }
}
