using Runge_Kutta = MathCore.DifferentialEquations.Numerical.RungeKutta;

namespace MathCore.Tests.DifferentialEquations.Numerical;

[TestClass]
public class RungeKuttaTests
{
    [TestMethod]
    public void Test_Step4_Scalar()
    {
        // dy/dt = y, y(0) = 1
        Func<double, double, double> f = (t, y) => y;
        var result = Runge_Kutta.Step4(f, 0, 0.1, 1);
        // e^0.1 ≈ 1.10517
        Assert.IsTrue(Math.Abs(result - 1.10517) < 0.001);
    }

    [TestMethod]
    public void Test_Solve4_Scalar()
    {
        // dy/dt = -y, y(0) = 1 => y(t) = e^(-t)
        Func<double, double, double> f = (t, y) => -y;
        var (t, y) = Runge_Kutta.Solve4(f, 0.01, 1, y0: 1);
        Assert.AreEqual(101, t.Length);
        Assert.AreEqual(101, y.Length);
        Assert.AreEqual(1, y[0]);
        // y(1) ≈ e^(-1) ≈ 0.3679
        Assert.IsTrue(Math.Abs(y[^1] - 0.3679) < 0.01);
        // Монотонное убывание
        for (var i = 1; i < y.Length; i++)
            Assert.IsTrue(y[i] <= y[i - 1]);
    }

    [TestMethod]
    public void Test_Solve4_Vector2D()
    {
        // dy1/dt = y2, dy2/dt = -y1 (гармонические колебания)
        Func<double, (double y1, double y2), (double y1, double y2)> f = (_, y) => (y.y2, -y.y1);
        var (t, y) = Runge_Kutta.Solve4(f, 0.01, Math.PI / 2, y0: (y1: 1, y2: 0));
        Assert.IsTrue(t.Length > 1);
        Assert.AreEqual(1, y[0].y1);
        Assert.AreEqual(0, y[0].y2);
    }

    [TestMethod]
    public void Test_Step4_Vector2D()
    {
        Func<double, (double y1, double y2), (double y1, double y2)> f = (_, y) => (y.y2, -y.y1);
        var result = Runge_Kutta.Step4(f, 0, 0.1, (y1: 1, y2: 0));
        // y1 ≈ 1 (изменение небольшое), y2 ≈ 0.1
        Assert.IsTrue(Math.Abs(result.y1 - 1) < 0.02);
        Assert.IsTrue(Math.Abs(result.y2) < 0.1);
    }

    [TestMethod]
    public void Test_Solve4_Vector3D()
    {
        Func<double, (double y1, double y2, double y3), (double y1, double y2, double y3)> f = (_, y) => (y.y2, y.y3, 0);
        var (t, y) = Runge_Kutta.Solve4(f, 0.1, 1, y0: (0, 1, 0));
        Assert.AreEqual(11, t.Length);
        Assert.AreEqual(11, y.Length);
        Assert.AreEqual((0, 1, 0), y[0]);
        // y3 должна оставаться 0
        Assert.IsTrue(y.All(p => Math.Abs(p.y3) < 1e-10));
    }

    [TestMethod]
    public void Test_Solve45_Scalar()
    {
        Func<double, double, double> f = (t, y) => -y;
        var (t, y, eps) = Runge_Kutta.Solve45(f, 0.01, 1, y0: 1);
        Assert.IsTrue(t.Length > 0);
        Assert.IsTrue(y.Length == t.Length);
        Assert.IsTrue(eps.Length == t.Length);
        Assert.AreEqual(1, y[0]);
    }

    [TestMethod]
    public void Test_Solve4_MoreAccurateThanEuler()
    {
        // dy/dt = -y, y(0) = 1
        Func<double, double, double> f = (t, y) => -y;
        var dt = 0.01;
        var (t_rk, y_rk) = Runge_Kutta.Solve4(f, dt, 1, y0: 1);
        var (t_euler, y_euler) = MathCore.DifferentialEquations.Numerical.Euler.Solve(f, dt, 1, y0: 1);
        var exact = Math.Exp(-1);
        var err_rk = Math.Abs(y_rk[^1] - exact);
        var err_euler = Math.Abs(y_euler[^1] - exact);
        // Метод Рунге-Кутты 4-го порядка должен быть точнее Эйлера
        Assert.IsTrue(err_rk < err_euler);
    }
}
