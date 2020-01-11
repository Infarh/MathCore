using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class PolynomArrayTests
    {

        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize]
        //public static void MyClassInitialize(TestContext testContext) { }

        //[ClassCleanup]
        //public static void MyClassCleanup() { }

        //[TestInitialize]
        //public void MyTestInitialize() { }

        //[TestCleanup]
        //public void MyTestCleanup() { }

        #endregion


        [TestMethod]
        public void GetValue_Test()
        {
            double[] a = { 3, 5, 7 };

            Assert.AreEqual(3, Polynom.Array.GetValue(0, a));
            Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(1, a));
            Assert.AreEqual(41, Polynom.Array.GetValue(2, a));
            Assert.AreEqual(81, Polynom.Array.GetValue(3, a));

            Assert.AreEqual(double.NaN, Polynom.Array.GetValue(10, new double[0]));
        }

        [TestMethod]
        public void GetValue_Enumerable_Test()
        {
            var a = Enumerable.Range(0, 3).Select(x => x * 2 + 3d);

            Assert.AreEqual(3, Polynom.Array.GetValue(0, a));
            Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(1, a));
            Assert.AreEqual(41, Polynom.Array.GetValue(2, a));
            Assert.AreEqual(81, Polynom.Array.GetValue(3, a));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetValue_Enumerable_Exceptions_Test() => Polynom.Array.GetValue(0, (IEnumerable<double>)null);


        [TestMethod]
        public void GetComplexValue_Test()
        {
            double[] a = { 3, 5, 7 };

            Assert.AreEqual(3, Polynom.Array.GetValue(new Complex(), a));
            Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(new Complex(1), a));
            Assert.AreEqual(41, Polynom.Array.GetValue(new Complex(2), a));
            Assert.AreEqual(81, Polynom.Array.GetValue(new Complex(3), a));

            Assert.AreEqual(Complex.NaN, Polynom.Array.GetValue(new Complex(), new double[0]));
        }

        [TestMethod]
        public void GetValue_Complex_Enumerable_Test()
        {
            var a = Enumerable.Range(0, 3).Select(x => x * 2 + 3d);

            Assert.AreEqual(3, Polynom.Array.GetValue(new Complex(), a));
            Assert.AreEqual(a.Sum(), Polynom.Array.GetValue(new Complex(1), a));
            Assert.AreEqual(41, Polynom.Array.GetValue(new Complex(2), a));
            Assert.AreEqual(81, Polynom.Array.GetValue(new Complex(3), a));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void GetValue_Complex_Enumerable_Exceptions_Test() => Polynom.Array.GetValue(new Complex(), (IEnumerable<double>)null);

        [TestMethod]
        public void GetCoefficients_Test()
        {
            double[] roots = { 3, 5, 7 };

            var coefficients = Polynom.Array.GetCoefficients(roots);

            double GetValue(double[] X0, double x) => X0.Select(x0 => x - x0).Aggregate(1d, (P, p) => P * p);
            void Check(double x) => Assert.AreEqual(GetValue(roots, x), Polynom.Array.GetValue(x, coefficients));
            Check(23);
            Check(17);
            Check(0);
            Check(-17);
            Check(-23);

            coefficients = Polynom.Array.GetCoefficients(5);
            CollectionAssert.AreEqual(new[] { -5d, 1 }, coefficients);
        }

        [TestMethod]
        public void GetCoefficients_Exceptions_Test()
        {
            Exception exception = null;
            try
            {
                double[] roots = null;
                Polynom.Array.GetCoefficients(roots);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
            Assert.AreEqual("Root", ((ArgumentNullException)exception).ParamName);

            exception = null;
            try
            {
                var roots = new double[0];
                Polynom.Array.GetCoefficients(roots);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual("Root", ((ArgumentException)exception).ParamName);
        }

        [TestMethod]
        public void GetCoefficientsInverted_Test()
        {
            double[] roots = { 3, 5, 7 };

            var coefficients = Polynom.Array.GetCoefficients(roots);                   // -105 71 -15 1
            var coefficients_inverted = Polynom.Array.GetCoefficientsInverted(roots);
            Array.Reverse(coefficients);
            CollectionAssert.AreEqual(coefficients, coefficients_inverted);

            coefficients = Polynom.Array.GetCoefficientsInverted(5);
            CollectionAssert.AreEqual(new[] { 1, -5d }, coefficients);
        }

        [TestMethod]
        public void GetCoefficientsInverted_Exceptions_Test()
        {
            Exception exception = null;
            try
            {
                double[] roots = null;
                Polynom.Array.GetCoefficientsInverted(roots);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
            Assert.AreEqual("Root", ((ArgumentNullException)exception).ParamName);

            exception = null;
            try
            {
                var roots = new double[0];
                Polynom.Array.GetCoefficientsInverted(roots);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(ArgumentException));
            Assert.AreEqual("Root", ((ArgumentException)exception).ParamName);
        }

        [TestMethod]
        public void GetCoefficients_Complex_Test()
        {
            Complex[] roots = { 3, 5, 7 };

            var coefficients = Polynom.Array.GetCoefficients(roots);

            Complex GetValue(Complex[] X0, Complex x) => X0.Select(x0 => x - x0).Aggregate(new Complex(1), (P, p) => P * p);
            void Check(Complex x) => Assert.AreEqual(GetValue(roots, x), Polynom.Array.GetValue(x, coefficients));
            Check(23);
            Check(17);
            Check(0);
            Check(-17);
            Check(-23);

            coefficients = Polynom.Array.GetCoefficients(new Complex(5));
            CollectionAssert.AreEqual(new[] { new Complex(-5), 1 }, coefficients);
        }

        [TestMethod]
        public void GetCoefficients_Complex_Exceptions_Test()
        {
            Assert.That.Method(Polynom.Array.GetCoefficients, (Complex[])null)
               .Throw<ArgumentNullException>()
               .Where(e => e.ParamName).IsEqual("Root");
           
            Assert.That.Method(Polynom.Array.GetCoefficients, new Complex[0])
               .Throw<ArgumentException>()
               .Where(e => e.ParamName).IsEqual("Root");
        }

        [TestMethod]
        public void GetCoefficientsInverted_Complex_Test()
        {
            Complex[] roots = { 3, 5, 7 };

            var coefficients = Polynom.Array.GetCoefficients(roots);                   // -105 71 -15 1
            var coefficients_inverted = Polynom.Array.GetCoefficientsInverted(roots);
            Array.Reverse(coefficients);
            CollectionAssert.AreEqual(coefficients, coefficients_inverted);

            coefficients = Polynom.Array.GetCoefficientsInverted(new Complex(5));
            CollectionAssert.That.Collection(coefficients).IsEqualTo(new[] { 1, new Complex(-5) });
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void GetCoefficientsInverted_Complex_Exceptions_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Polynom.Array.GetCoefficientsInverted((Complex[])null));
            Assert.ThrowsException<ArgumentException>(() => Polynom.Array.GetCoefficientsInverted(new Complex[0]));
        }

        [TestMethod]
        public void GetDifferential_Test()
        {
            double[] a = { 3, 5, 7 };

            var actual_differential = Polynom.Array.GetDifferential(a);

            double[] expected_differential = { 5, 14 };
            CollectionAssert.AreEqual(expected_differential, actual_differential);

            a = new double[] { 3, 5, 7, 9, 12 };
            actual_differential = Polynom.Array.GetDifferential(a, 3);
            expected_differential = new double[] { 54, 288 };
            CollectionAssert.AreEqual(expected_differential, actual_differential);

            actual_differential = Polynom.Array.GetDifferential(a, 0);
            CollectionAssert.AreEqual(a, actual_differential);
            Assert.IsFalse(ReferenceEquals(a, actual_differential));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void GetDifferential_ArgumentNullException_Test() => Polynom.Array.GetDifferential((double[])null);

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetDifferential_ArgumentOutOfRangeException_Test() => Polynom.Array.GetDifferential(new double[5], -1);

        [TestMethod]
        public void GetDifferential_Complex_Test()
        {
            Complex[] a = { 3, 5, 7 };

            var actual_differential = Polynom.Array.GetDifferential(a);

            Complex[] expected_differential = { 5, 14 };
            CollectionAssert.AreEqual(expected_differential, actual_differential);

            a = new Complex[] { 3, 5, 7, 9, 12 };
            actual_differential = Polynom.Array.GetDifferential(a, 3);
            expected_differential = new Complex[] { 54, 288 };
            CollectionAssert.AreEqual(expected_differential, actual_differential);

            actual_differential = Polynom.Array.GetDifferential(a, 0);
            CollectionAssert.AreEqual(a, actual_differential);
            Assert.IsFalse(ReferenceEquals(a, actual_differential));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void GetDifferential_Complex_ArgumentNullException_Test() => Polynom.Array.GetDifferential((Complex[])null);

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetDifferential_Complex_ArgumentOutOfRangeException_Test() => Polynom.Array.GetDifferential(new Complex[5], -1);

        [TestMethod]
        public void GetIntegral_Test()
        {
            double[] a = { 18, 30, 42 };
            const int c = 17;

            var integral = Polynom.Array.GetIntegral(a, c);
            double[] expected_integral = { c, 18, 15, 14 };

            CollectionAssert.AreEqual(expected_integral, integral);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void GetIntegral_Exceptions_Test() => Polynom.Array.GetIntegral((double[])null);

        [TestMethod]
        public void GetIntegral_Complex_Test()
        {
            Complex[] a = { 18, 30, 42 };
            Complex c = 17;

            var integral = Polynom.Array.GetIntegral(a, c);
            Complex[] expected_integral = { c, 18, 15, 14 };

            CollectionAssert.AreEqual(expected_integral, integral);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void GetIntegral_Complex_Exceptions_Test() => Polynom.Array.GetIntegral((Complex[])null);


        [TestMethod]
        public void Sum_Test()
        {
            double[] p = { 3, 5, 7 };
            double[] q = { 1, 2, 3, 4, 5 };
            double[] expected_sum = { 4, 7, 10, 4, 5 };

            var actual_sum = Polynom.Array.Sum(p, q);
            CollectionAssert.AreEqual(expected_sum, actual_sum);

            actual_sum = Polynom.Array.Sum(q, p);
            CollectionAssert.AreEqual(expected_sum, actual_sum);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void Sum_ArgumentNullException_p_Test() => Polynom.Array.Sum(null, new double[5]);

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void Sum_ArgumentNullException_q_Test() => Polynom.Array.Sum(new double[5], null);

        [TestMethod]
        public void subtract_Test()
        {
            double[] p = { 3, 5, 7 };
            double[] q = { 1, 2, 3, 4, 5 };
            double[] expected_subtract = { 2, 3, 4, -4, -5 };

            var actual_subtract = Polynom.Array.Subtract(p, q);
            CollectionAssert.That.Collection(actual_subtract).IsEqualTo(expected_subtract);

            expected_subtract = new double[] { -2, -3, -4, 4, 5 };
            actual_subtract = Polynom.Array.Subtract(q, p);
            CollectionAssert.That.Collection(actual_subtract).IsEqualTo(expected_subtract);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void subtract_ArgumentNullException_p_Test() => Polynom.Array.Subtract(null, new double[5]);

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void subtract_ArgumentNullException_q_Test() => Polynom.Array.Subtract(new double[5], null);
    }
}