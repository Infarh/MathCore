using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathCore.Values;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Values
{
    [TestClass]
    public class VarianceValueTests
    {
        [TestMethod]
        public void SamplesVariance()
        {
            var rnd = new Random();
            const int samples_count = 10000;
            var samples = new double[samples_count];
            const int expected_sigma = 5;
            const int expected_variance = expected_sigma * expected_sigma;
            rnd.FillNormal(samples, sigma: expected_sigma);

            const int window_length = 1000;
            var variance = new VarianceValue(window_length);// { Value = expected_sigma * expected_sigma };

            var results = samples.Select(variance.AddValue).ToArray();

            var last = results[(int)(samples_count * 0.75)..];
            var avg = last.Average();
            var avg2 = avg.Sqr();
            var std_dev = last.Average(x => x.Sqr() - avg2);

            Assert.That.Value(variance)
               .Where(v => v.Average).CheckEquals(0, 0.45)
               .Where(v => v.StdDev).CheckEquals(expected_sigma, 0.4);

            Assert.That.Value(avg).IsEqual(expected_variance, 2.3)
                   .And.Value(std_dev).IsEqual(0, 2.2);
        }
    }
}
