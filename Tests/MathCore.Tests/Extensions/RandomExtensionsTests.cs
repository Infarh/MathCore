using MathCore.Statistic;

namespace MathCore.Tests.Extensions;

[TestClass]
public class RandomExtensionsTests
{
    [Owner("Tester")]
    [TestMethod/*Iterative(100)*/, Ignore/*, Timeout(TestTimeout.Infinite)*/, Description("Тест нормального распределения")]
    public void NextNormal_SingleValue_Sigma1Mu0()
    {
        const double sigma = 1;
        const double mu    = 0;

        const int count  = 100000;
        var       values = new double[count];

        //var seed = (int) (DateTime.Now.Ticks % int.MaxValue);
        const int seed = 1395601201;
        var       rnd  = new Random(seed);
        for (var i = 0; i < count; i++) 
            values[i] = rnd.NextNormal(sigma, mu);

        var pirsons_criteria = Distributions.GetPirsonsCriteria(
            values, 
            Distributions.NormalGauss(sigma, mu), 
            out var freedom_degree);

        var quantile = MathCore.SpecialFunctions.Distribution.Student.QuantileHi2Approximation(0.80, freedom_degree);

        Assert.That.Value(pirsons_criteria).LessThan(quantile, $"seed:{seed}");
    }

    [TestMethod, Ignore]
    public void NextNormal_SingleValue_Sigma3Mu5()
    {
        const double sigma = 3;
        const double mu    = 5;

        const int count  = 10000;
        var       values = new double[count];

        var rnd = new Random();
        for (var i = 0; i < count; i++)
            values[i] = rnd.NextNormal(sigma, mu);

        var pirsons_criteria = Distributions.GetPirsonsCriteria(
            values,
            Distributions.NormalGauss(sigma, mu),
            out var freedom_degree);

        var quantile = MathCore.SpecialFunctions.Distribution.Student.QuantileHi2Approximation(0.95, freedom_degree);

        Assert.That.Value(pirsons_criteria).LessThan(quantile);
    }
}