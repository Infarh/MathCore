using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathCore.Statistic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Statistic
{
    [TestClass]
    public class DistributionsTests
    {
        private static readonly double[] __TestData =
        {
            -0.439, -0.679, -0.473, -0.951, -1.686, 0.044, -0.121, 0.556,     2.192,  0.809,
            0.985,  0.862,  0.916,  0.673, -1.044, 0.069, -0.756, 0.697,    -0.182, -0.644,
            -0.723, -0.517,  0.558, -0.245,  0.09,  1.262, -0.706, 1.864e-3,  1.108,  0.893
        };

        [TestMethod]
        public void GetPirsonsCriteriaTest()
        {
            var pirsons_criteria = Distributions.GetPirsonsCriteria(__TestData, Distributions.NormalGauss(), out var freedom_degree);

            Assert.That.Value(pirsons_criteria).IsEqual(3.2763111794634741);
            Assert.That.Value(freedom_degree).IsEqual(2);
        }

        [TestMethod]
        public void CheckDistributionTest() => Assert.IsTrue(__TestData.CheckDistribution(Distributions.NormalGauss()));
    }
}
