using Microsoft.VisualStudio.TestTools.UnitTesting;
using EllipticJacobi = MathCore.SpecialFunctions.EllipticJacobi;

namespace MathCore.Tests
{
    [TestClass]
    public class SpecialFunctionsEllipticJacobiTests
    {
        [TestMethod]
        public void FullEllipticIntegral_Iterative_Test()
        {
            const double k = 0.93;
            const double expected_K = 2.437457556775147;

            var actual_K = EllipticJacobi.FullEllipticIntegral(k);

            Assert.That.Value(actual_K).IsEqual(expected_K, 4.45e-16);
        }

        [TestMethod]
        public void FullEllipticIntegral_Recursive_Test()
        {
            const double k = 0.93;
            const double expected_K = 2.437457556775147;

            var actual_K = EllipticJacobi.FullEllipticIntegral_Recursive(k);

            Assert.That.Value(actual_K).IsEqual(expected_K, 4.45e-16);
        }

        [TestMethod]
        public void sn_Iterative_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_sn = 9.179189425955885e-1;

            var z = u * EllipticJacobi.FullEllipticIntegral(k);

            var actual_sn = EllipticJacobi.sn_iterative(z, k);

            Assert.That.Value(actual_sn).IsEqual(expected_sn, 1.12e-16);
        }

        [TestMethod]
        public void sn_uk_Iterative_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_sn = 9.179189425955885e-1;

            var actual_sn = EllipticJacobi.sn_uk(u, k);

            Assert.That.Value(actual_sn).IsEqual(expected_sn, 1.12e-16);
        }

        [TestMethod]
        public void sn_uk_Recursive_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_sn = 9.179189425955885e-1;

            var actual_sn = EllipticJacobi.sn_uk_recursive(u, k);

            Assert.That.Value(actual_sn).IsEqual(expected_sn, 1.12e-16);
        }

        [TestMethod]
        public void cd_Iterative_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_cd = 7.618094237515579e-1;

            var z = u * EllipticJacobi.FullEllipticIntegral(k);

            var actual_cd = EllipticJacobi.cd_iterative(z, k);

            Assert.That.Value(actual_cd).IsEqual(expected_cd, 1.12e-16);
        }

        [TestMethod]
        public void cd_uk_Iterative_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_cd = 7.618094237515579e-1;

            var actual_cd = EllipticJacobi.cd_uk(u, k);

            Assert.That.Value(actual_cd).IsEqual(expected_cd, 1.12e-16);

            const double u2 = 0.2;
            const double k2 = 0.766760202993181;
            const double expected_cd2 = 0.968346873207978;
            actual_cd = EllipticJacobi.cd_uk(u2, k2);
            Assert.That.Value(actual_cd).IsEqual(expected_cd2, 2.23e-16);
        }

        [TestMethod]
        public void cd_uk_recursive_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_cd = 7.618094237515579e-1;

            var actual_cd = EllipticJacobi.cd_uk_recursive(u, k);

            Assert.That.Value(actual_cd).IsEqual(expected_cd, 1.12e-16);
        }

        [TestMethod]
        public void sn_inverse_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_sn = 9.179189425955885e-1;

            var sn = EllipticJacobi.sn_uk(u, k);

            Assert.That.Value(sn).IsEqual(expected_sn, 1.12e-16);

            var actual_sn_inversed = EllipticJacobi.sn_inverse(sn, k);

            Assert.That.Value(actual_sn_inversed).IsEqual(u, 2.23e-16);
        }

        [TestMethod]
        public void sn_inverse_recursive_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_sn = 9.179189425955885e-1;

            var sn = EllipticJacobi.sn_uk(u, k);

            Assert.That.Value(sn).IsEqual(expected_sn, 1.12e-16);

            var actual_sn_inversed = EllipticJacobi.sn_inverse_recursive(sn, k);

            Assert.That.Value(actual_sn_inversed).IsEqual(u, 2.23e-16);
        }

        [TestMethod]
        public void cd_inverse_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_cd = 7.618094237515579e-1;
            var cd = EllipticJacobi.cd_uk(u, k);
            Assert.That.Value(cd).IsEqual(expected_cd, 1.12e-16);

            const double expected_cd_inverse = u;

            var actual_cd_inversed = EllipticJacobi.cd_inverse(cd, k);
            Assert.That.Value(actual_cd_inversed).IsEqual(expected_cd_inverse, 1.12e-16);
        }

        [TestMethod]
        public void cd_inverse_recursive_Test()
        {
            const double u = 0.6;
            const double k = 0.93;
            const double expected_cd = 7.618094237515579e-1;
            var cd = EllipticJacobi.cd_uk(u, k);
            Assert.That.Value(cd).IsEqual(expected_cd, 1.12e-16);

            const double expected_cd_inverse = u;

            var actual_cd_inversed = EllipticJacobi.cd_inverse_recursive(cd, k);
            Assert.That.Value(actual_cd_inversed).IsEqual(expected_cd_inverse, 1.12e-16);
        }
    }
}