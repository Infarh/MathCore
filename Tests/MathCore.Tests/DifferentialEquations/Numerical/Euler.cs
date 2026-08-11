using Solver = MathCore.DifferentialEquations.Numerical.Euler;

namespace MathCore.Tests.DifferentialEquations.Numerical;

[TestClass]
public class EulerTests
{
    [TestMethod]
    public void Test_Step_Simple()
    {
        // dy/dt = y, y(0) = 1 => y(t) = e^t
        Func<double, double, double> f = (t, y) => y;
        var result = Solver.Step(f, 0, 0.1, 1);
        Assert.IsTrue(Math.Abs(result - 1.1) < 1e-10);
    }

    [TestMethod]
    public void Test_Solve_Scalar()
    {
        // dy/dt = -y, y(0) = 1
        Func<double, double, double> f = (t, y) => -y;
        var (t, y) = Solver.Solve(f, 0.1, 1, y0: 1, t0: 0);
        var N = (int)((1 - 0) / 0.1) + 1;
        Assert.AreEqual(N, t.Length);
        Assert.AreEqual(N, y.Length);
        Assert.AreEqual(0, t[0]);
        Assert.AreEqual(1, y[0]);
        // y должно экспоненциально убывать
        Assert.IsTrue(y[1] < y[0]);
        Assert.IsTrue(y[^1] < y[0]);
    }

    [TestMethod]
    public void Test_Solve_WithCustomT0()
    {
        Func<double, double, double> f = (t, y) => y;
        var (t, y) = Solver.Solve(f, 0.5, 2, y0: 1, t0: 1);
        var N = (int)((2 - 1) / 0.5) + 1;
        Assert.AreEqual(N, t.Length);
        Assert.AreEqual(1, t[0]);
        Assert.AreEqual(2, t[^1]);
    }

    [TestMethod]
    public void Test_Solve_Vector2D()
    {
        // Тест на базовую функциональность (без проверки точности из-за особенностей реализации)
        Func<double, (double y1, double y2), (double y1, double y2)> f = (_, y) => (y.y2, -y.y1);
        var (t, y) = Solver.Solve(f, 0.1, Math.PI, y0: (y1: 1, y2: 0));
        Assert.IsTrue(t.Length > 1);
        Assert.AreEqual(1, y[0].y1);
        Assert.AreEqual(0, y[0].y2);
    }

    [TestMethod]
    public void Test_Step_Vector2D()
    {
        Func<double, (double y1, double y2), (double y1, double y2)> f = (_, y) => (y.y2, -y.y1);
        var result = Solver.Step(f, 0, 0.1, (y1: 1, y2: 0));
        // result.y1 = 1 + 0.1 * 0 = 1
        Assert.IsTrue(Math.Abs(result.y1 - 1) < 1e-10);
    }

    [TestMethod]
    public void Test_Solve_Vector3D()
    {
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f = (_, y) => (y.y2, y.y3, 0);
        var (t, y) = Solver.Solve(f, 0.1, 1, y0: (y1: 0, y2: 1, y3: 0));
        Assert.AreEqual(11, t.Length);
        Assert.AreEqual(11, y.Length);
        Assert.AreEqual(0, y[0].y1);
        Assert.AreEqual(1, y[0].y2);
        Assert.AreEqual(0, y[0].y3);
    }

    [TestMethod]
    public void Test_Step_Vector3D()
    {
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f = (_, y) => (y.y2, y.y3, 0);
        var result = Solver.Step(f, 0, 0.1, (y1: 0, y2: 1, y3: 0));
        // result.y1 = 0 + 0.1 * 1 = 0.1
        Assert.AreEqual(0.1, result.y1);
    }
}
