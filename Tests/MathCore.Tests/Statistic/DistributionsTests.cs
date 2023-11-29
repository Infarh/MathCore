using MathCore.Statistic;

namespace MathCore.Tests.Statistic;

[TestClass]
public class DistributionsTests
{
    private static readonly double[] __TestData =
    [
        -0.439, -0.679, -0.473, -0.951, -1.686, +0.044, -0.121, +0.556,    +2.192, +0.809,
        +0.985, +0.862, +0.916, +0.673, -1.044, +0.069, -0.756, +0.697,    -0.182, -0.644,
        -0.723, -0.517, +0.558, -0.245, +0.09,  +1.262, -0.706, +1.864e-3, +1.108, +0.893
    ];

    [TestMethod, Ignore]
    public void GetPirsonsCriteriaTest()
    {
        var normal_gauss_distribution = Distributions.NormalGauss();
        var pirsons_criteria = __TestData.GetPirsonsCriteria(normal_gauss_distribution, out var freedom_degree, out _, out _);

        var chi2 = MathCore.SpecialFunctions.Distribution.Student.QuantileHi2(0.95, freedom_degree);

        pirsons_criteria.AssertEquals(2.551348617578610423);
        freedom_degree.AssertEquals(2);
    }

    [TestMethod]
    public void CheckDistributionTest()
    {
        var normal_gauss_distribution = Distributions.NormalGauss();
        var check_distribution_result = __TestData.CheckDistribution(normal_gauss_distribution);
        check_distribution_result.AssertTrue();
    }
}