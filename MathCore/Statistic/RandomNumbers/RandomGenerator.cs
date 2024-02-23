using MathCore.Values;
// ReSharper disable UnusedMember.Global

namespace MathCore.Statistic.RandomNumbers;

/// <summary>Генератор случайных чисел</summary>
[Serializable]
public abstract class RandomGenerator(Random rnd = null) : IValueRead<double>
{
    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Дисперсия</summary>
    protected double _Sigma = 1;
    /// <summary>Математическое ожидание</summary>
    protected double _Mu;

    protected readonly Random _Random = rnd ?? new();

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Дисперсия</summary>
    public double Sigma { get => _Sigma; set => _Sigma = value; }

    /// <summary>Математическое ожидание</summary>
    public double Mu { get => _Mu; set => _Mu = value; }

    /// <summary>Случайное значение</summary>
    public double Value => GetValue();

    /* ------------------------------------------------------------------------------------------ */

    protected RandomGenerator(double sigma, Random rnd = null) : this(rnd) => _Sigma = sigma;

    protected RandomGenerator(double sigma, double mu, Random rnd = null) : this(sigma, rnd) => _Mu = mu;

    /* ------------------------------------------------------------------------------------------ */

    public abstract double Distribution(double x);

    /// <summary>Новое случайное число</summary><returns>Случайное число</returns>
    protected double GetValue() => GetValue(_Sigma, _Mu);

    public double GetValue(double sigma, double m) => GetNextValue() * sigma + m;

    protected abstract double GetNextValue();

    public double[] GetValues(int count)
    {
        var values = new double[count];
        for (var i = 0; i < count; i++)
            values[i] = GetValue();
        return values;
    }

    public void FillValues(double[] values)
    {
        for (var i = 0; i < values.Length; i++)
            values[i] = GetValue();
    }

    public void FillValues(double[] values, int index, int count)
    {
        for (var i = index; i - index < count && i < values.Length; i++)
            values[i] = GetValue();
    }

    public IEnumerable<double> EnumValues()
    {
        while (true) yield return GetValue();
    }

    public IEnumerable<double> EnumValues(int count) => EnumValues().Take(count);

    /* ------------------------------------------------------------------------------------------ */

    public static implicit operator double(RandomGenerator rnd) => rnd.GetValue();

    /* ------------------------------------------------------------------------------------------ */
}