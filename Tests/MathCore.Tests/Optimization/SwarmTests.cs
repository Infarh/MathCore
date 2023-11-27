using MathCore.Optimization.ParticleSwarm;

namespace MathCore.Tests.Optimization;

[TestClass]
public class SwarmTests
{
    private const double __X0 = 5;
    private const double __Y0 = -7;
    private const double __Z0 = 4;
    private const double __V0 = 10;

    private static double F0(double x, double y, double z) => x * x + y * y + z * z;
    private static double F(double x, double y, double z) => F0(x - __X0, y - __Y0, z - __Z0) + __V0;
    private static double FNeg(double x, double y, double z) => -F(x, y, z);

    private static double F(double[] X) => F(X[0], X[1], X[2]);
    private static double FNeg(double[] X) => FNeg(X[0], X[1], X[2]);

    [TestMethod]
    public void Minimize()
    {
        var swarm = new Swarm();
        swarm.Minimize(F, [-10d, -10d, -10d], [10d, 10d, 10d], 1000, out var X, out var V);

        Assert.That.Value(V).IsEqual(__V0);
        CollectionAssert.That.Collection(X).ValuesAreEqualTo(__X0, __Y0, __Z0).WithAccuracy(3e-8);
    }

    [TestMethod]
    public void MinimizeInterval()
    {
        var swarm = new Swarm();
        swarm.Minimize(F, [(-10, 10), (-10, 10), (-10, 10)], 1000, out var X, out var V);

        Assert.That.Value(V).IsEqual(__V0);
        CollectionAssert.That.Collection(X).ValuesAreEqualTo(__X0, __Y0, __Z0).WithAccuracy(3e-8);
    }

    [TestMethod]
    public void Maximize()
    {
        var swarm = new Swarm();
        swarm.Maximize(FNeg, [-10d, -10d, -10d], [10d, 10d, 10d], 1000, out var X, out var V);

        Assert.That.Value(V).IsEqual(-__V0);
        CollectionAssert.That.Collection(X).ValuesAreEqualTo(__X0, __Y0, __Z0).WithAccuracy(3e-8);
    }

    [TestMethod]
    public void MaximizeInterval()
    {
        var swarm = new Swarm();
        swarm.Maximize(FNeg, [(-10, 10), (-10, 10), (-10, 10)], 1000, out var X, out var V);

        Assert.That.Value(V).IsEqual(-__V0);
        CollectionAssert.That.Collection(X).ValuesAreEqualTo(__X0, __Y0, __Z0).WithAccuracy(3e-8);
    }
}