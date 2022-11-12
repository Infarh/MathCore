using Solver = MathCore.DifferentialEquations.Numerical.Euler;
using static System.Diagnostics.Debug;

namespace MathCore.Tests.DifferentialEquations.Numerical;

[TestClass]
public class Euler
{
    [TestMethod]
    public void Solve()
    {
        var (xx, yy) = Solver.Solve(TestEquation.Eq, 0.2, 10, t0: 0, y0: 40);
        var yy0 = TestEquation.Y(xx);

        var delta = yy0.Zip(yy, (a, b) => a - b).ToArray();

        var err = TestEquation.GetError(yy0, yy);
        WriteLine(err);
    }
}