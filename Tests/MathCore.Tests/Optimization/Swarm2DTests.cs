using MathCore.Optimization.ParticleSwarm;

namespace MathCore.Tests.Optimization
{
    [TestClass]
    public class Swarm2DTests
    {
        private const double __X0 = 5;
        private const double __Y0 = -7;
        private const double __Z0 = 10;

        private static double F0(double x, double y) => x * x + y * y;
        private static double F(double x, double y) => F0(x - __X0, y - __Y0) + __Z0;
        private static double FNeg(double x, double y) => -F(x, y);

        [TestMethod]
        public void Minimize()
        {
            var swarm = new Swarm2D();
            swarm.Minimize(
                F,
                MinX: -10, MaxX: 10,
                MinY: -10, MaxY: 10,
                IterationCount: 1000,
                out var X, out var Y,
                out var Z);
            Assert.That.Value(X).IsEqual(__X0, 3.0e-8);
            Assert.That.Value(Y).IsEqual(__Y0, 3.0e-8);
            Assert.That.Value(Z).IsEqual(__Z0);
        }

        [TestMethod]
        public void MinimizeInterval()
        {
            var swarm = new Swarm2D();
            swarm.Minimize(
                F,
                (-10, 10),
                (-10, 10),
                IterationCount: 1000,
                out var X, out var Y,
                out var Z);
            Assert.That.Value(X).IsEqual(__X0, 3.0e-8);
            Assert.That.Value(Y).IsEqual(__Y0, 3.0e-8);
            Assert.That.Value(Z).IsEqual(__Z0);
        }

        [TestMethod]
        public void Maximize()
        {
            var swarm = new Swarm2D();
            swarm.Maximize(
                FNeg,
                MinX: -10, MaxX: 10,
                MinY: -10, MaxY: 10,
                IterationCount: 1000,
                out var X, out var Y,
                out var Z);
            Assert.That.Value(X).IsEqual(__X0, 3.0e-8);
            Assert.That.Value(Y).IsEqual(__Y0, 3.0e-8);
            Assert.That.Value(Z).IsEqual(-__Z0);
        }

        [TestMethod]
        public void MaximizeInterval()
        {
            var swarm = new Swarm2D();
            swarm.Maximize(
                FNeg,
                (-10, 10),
                (-10, 10),
                IterationCount: 1000,
                out var X, out var Y,
                out var Z);
            Assert.That.Value(X).IsEqual(__X0, 3.0e-8);
            Assert.That.Value(Y).IsEqual(__Y0, 3.0e-8);
            Assert.That.Value(Z).IsEqual(-__Z0);
        }
    }
}
