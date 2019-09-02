using System;
using MathCore.Values;
// ReSharper disable UnusedMember.Global

namespace MathCore.Statistic.RandomNumbers
{
    /// <summary>Генератор случайных чисел</summary>
    [Serializable]
    public abstract class RandomGenerator : IValueRead<double>
    {
        /// <summary>Датчик случайных чисел с равномерным распределением</summary>
        protected static readonly LazyValue<Random> SystemRandomGenerator = new LazyValue<Random>(() => new Random(DateTime.Now.TimeOfDay.Milliseconds));

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Дисперсия</summary>
        protected double _Sigma = 1;
        /// <summary>Математическое ожидание</summary>
        protected double _Mu;

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Дисперсия</summary>
        public double Sigma { get => _Sigma; set => _Sigma = value; }

        /// <summary>Математическое ожидание</summary>
        public double Mu { get => _Mu; set => _Mu = value; }

        /// <summary>Случайное значение</summary>
        public double Value => GetValue();

        /* ------------------------------------------------------------------------------------------ */

        protected RandomGenerator() { }

        protected RandomGenerator(double sigma) => _Sigma = sigma;

        protected RandomGenerator(double sigma, double mu) { _Sigma = sigma; _Mu = mu; }

        /* ------------------------------------------------------------------------------------------ */

        public abstract double Distribution(double x);

        /// <summary>Новое случайное число</summary><returns>Случайное число</returns>
        protected double GetValue() => GetValue(_Sigma, _Mu);

        public double GetValue(double sigma, double m) => GetNextValue() * sigma + m;

        protected abstract double GetNextValue();

        /* ------------------------------------------------------------------------------------------ */

        public static implicit operator double(RandomGenerator rnd) => rnd.GetValue();

        /* ------------------------------------------------------------------------------------------ */
    }
}