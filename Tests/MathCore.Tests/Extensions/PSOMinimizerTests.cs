using System;
using MathCore.Functions.PSO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Extensions
{
    [TestClass]
    public class PSOMinimizerTests : UnitTest
    {
        private const int SamplesCount = 100;

        private readonly Random _RND = new Random();
        private double RND(double M = 0, double D = 1) => (_RND.NextDouble() - 0.5) * D + M;

        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private static double Parabola(double x) => x * x;


        [TestMethod, Priority(0)]
        public void FunctionMinimizerTest()
        {
            const double Dx = 10.0;
            for (var i = 0; i < SamplesCount; i++)
            {
                Func<double, double> f = Parabola;
                var x0 = RND(Dx, Dx);
                var y0 = RND(Dx, Dx);
                f = f.ArgumentShift(x0);
                f = f.Add(y0);

                f.Minimize(x0 - Dx / 2, x0 + Dx / 2, 100, out var x, out var y);

                Assert.That.Value(x).IsEqual(x0, 0.5, "x != x0");
                Assert.That.Value(y).IsEqual(y0, 0.2, "y != y0");
            }
        }

        [TestMethod, Priority(0)]
        public void FunctionMaximizerTest()
        {
            const double Dx = 10.0;
            for (var i = 0; i < SamplesCount; i++)
            {
                Func<double, double> f = Parabola;
                var x0 = RND(Dx, Dx);
                var y0 = RND(Dx, Dx);
                f = f.Reverse();
                f = f.ArgumentShift(x0);
                f = f.Add(y0);

                f.Maximize(x0 - Dx / 2, x0 + Dx / 2, 100, out var x, out var y);

                const double eps = 0.5;
                Assert.That.Value(x).IsEqual(x0, eps, "x != x0");
                Assert.That.Value(y).IsEqual(y0, eps, "y != y0");
            }
        }
    }
}
