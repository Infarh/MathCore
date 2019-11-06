using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MathCore.SpecialFunctions.Distribution.Student;

namespace MathCore.Tests.SpecialFunctions.Distribution
{
    [TestClass]
    public class StudentTests
    {
        [TestMethod]
        public void QuantileHi2Test()
        {
            var P = Enumerable.Range(1, 19).Select(i => i * 5 / 100d).ToArray();
            var K = Enumerable.Range(1, 10).ToArray();

            var hi_with_p_greater_than_05 = QuantileHi2(0.95, 8);
            var hi_with_p_less_than_05 = QuantileHi2(0.05, 8);

           Assert.That.Value(hi_with_p_greater_than_05).IsEqual(15.506278896843497);
           Assert.That.Value(hi_with_p_greater_than_05).IsEqual(15.507313055865437, 1.035e-3);

           Assert.That.Value(hi_with_p_less_than_05).IsEqual(2.7356377218788865);
           Assert.That.Value(hi_with_p_less_than_05).IsEqual(2.732636793499664, 3.01e-3);


            //var M = new double[P.Length, K.Length];
            //for (var p = 0; p < P.Length; p++)
            //    for (var k = 0; k < K.Length; k++)
            //        M[p, k] = QuantileHi2(P[p], K[k]);

            //double[,] M0 =
            //{
            //   { 3.932e-3, 0.103, 0.352, 0.711, 1.145, 1.635,  2.167,  2.733,  3.325,  3.94,   },
            //   { 0.016,    0.211, 0.584, 1.064, 1.61,  2.204,  2.833,  3.49,   4.168,  4.865,  },
            //   { 0.036,    0.325, 0.798, 1.366, 1.994, 2.661,  3.358,  4.078,  4.817,  5.57,   },
            //   { 0.064,    0.446, 1.005, 1.649, 2.343, 3.07,   3.822,  4.594,  5.38,   6.179,  },
            //   { 0.102,    0.575, 1.213, 1.923, 2.675, 3.455,  4.255,  5.071,  5.899,  6.737,  },
            //   { 0.148,    0.713, 1.424, 2.195, 3,     3.828,  4.671,  5.527,  6.393,  7.267,  },
            //   { 0.206,    0.862, 1.642, 2.47,  3.325, 4.197,  5.082,  5.975,  6.876,  7.783,  },
            //   { 0.275,    1.022, 1.869, 2.753, 3.655, 4.57,   5.493,  6.423,  7.357,  8.295,  },
            //   { 0.357,    1.196, 2.109, 3.047, 3.996, 4.952,  5.913,  6.877,  7.843,  8.812,  },
            //   { 0.455,    1.386, 2.366, 3.357, 4.351, 5.348,  6.346,  7.344,  8.343,  9.342,  },
            //   { 0.571,    1.597, 2.643, 3.687, 4.728, 5.765,  6.8,    7.833,  8.863,  9.892,  },
            //   { 0.708,    1.833, 2.946, 4.045, 5.132, 6.211,  7.283,  8.351,  9.414,  10.473, },
            //   { 0.873,    2.1,   3.283, 4.438, 5.573, 6.695,  7.806,  8.909,  10.006, 11.097, },
            //   { 1.074,    2.408, 3.665, 4.878, 6.064, 7.231,  8.383,  9.524,  10.656, 11.781, },
            //   { 1.323,    2.773, 4.108, 5.385, 6.626, 7.841,  9.037,  10.219, 11.389, 12.549, },
            //   { 1.642,    3.219, 4.642, 5.989, 7.289, 8.558,  9.803,  11.03,  12.242, 13.442, },
            //   { 2.072,    3.794, 5.317, 6.745, 8.115, 9.446,  10.748, 12.027, 13.288, 14.534, },
            //   { 2.706,    4.605, 6.251, 7.779, 9.236, 10.645, 12.017, 13.362, 14.684, 15.987, },
            //   { 3.841,    5.991, 7.815, 9.488, 11.07, 12.592, 14.067, 15.507, 16.919, 18.307  },
            //};
        }
    }
}
