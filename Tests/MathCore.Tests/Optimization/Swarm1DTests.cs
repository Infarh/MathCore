using MathCore.Optimization.ParticleSwarm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Optimization
{
    [TestClass]
    public class Swarm1DTests
    {
        private const double __X0 = 3;
        private const double __Y0 = 2;

        private static double Sqr(double x) => x * x;
        private static double F(double x) => Sqr(x - __X0) + __Y0;
        private static double FNegate(double x) => -F(-x);

        [TestMethod]
        public void Minimize()
        {
            var swarm = new Swarm1D();
            swarm.Minimize(
                F,
                MinX: -100,
                MaxX: 100,
                IterationCount: 1000,
                out var X,
                out var Y);
            Assert.That.Value(X).IsEqual(__X0, 2.0e-8);
            Assert.That.Value(Y).IsEqual(__Y0);
        }

        [TestMethod]
        public void MinimizeInterval()
        {
            var swarm = new Swarm1D();
            swarm.Minimize(
                F,
                (-100, 100), 
                IterationCount: 1000,
                out var X,
                out var Y);
            Assert.That.Value(X).IsEqual(__X0, 2e-8);
            Assert.That.Value(Y).IsEqual(__Y0);
        }

        [TestMethod]
        public void Maximize()
        {
            var swarm = new Swarm1D();
            swarm.Maximize(
                FNegate,
                MinX: -5,
                MaxX: 5,
                IterationCount: 10000,
                out var X,
                out var Y);
            Assert.That.Value(X).IsEqual(-__X0, 6e-5);
            Assert.That.Value(Y).IsEqual(-__Y0, 7e-10);
        }

        [TestMethod]
        public void MaximizeInterval()
        {
            var swarm = new Swarm1D();
            swarm.Maximize(
                FNegate,
                (-5, 5),
                IterationCount: 1000,
                out var X,
                out var Y);
            Assert.That.Value(X).IsEqual(-__X0, 2.0e-4);
            Assert.That.Value(Y).IsEqual(-__Y0, 2.0e-8);
        }
    }
}
