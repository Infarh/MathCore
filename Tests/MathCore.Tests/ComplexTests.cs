using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace MathCore.Tests
{
    [TestClass]
    public class ComplexTests
    {
        /* ------------------------------------------------------------------------------------------ */

        // ReSharper disable once InconsistentNaming
        private const double pi = Math.PI;

        private static Random __RndGenerator;

        private static double Random => __RndGenerator.NextDouble() * 20 - 10;

        private static double RandomPositive => __RndGenerator.NextDouble() * 10;

        private static double RandomRad => 2 * Math.PI * __RndGenerator.NextDouble();

        private static Complex Rnd => new Complex(Random, Random);

        //private static int GetRNDInt(int Min = 5, int Max = 15) => __RndGenerator.Next(Min, Max);

        //private static double GetRNDDouble(double Min = -20, double Max = 20)
        //{
        //    var delta = Max - Min;
        //    return delta * __RndGenerator.NextDouble() - Min;
        //}

        //[NotNull]
        //private static double[] GetRandomVector(int Length = 0) => new double[Length == 0 ? GetRNDInt() : Length].Initialize(i => GetRNDDouble());

        /* ------------------------------------------------------------------------------------------ */

        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //

        ////Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup]
        //public static void MyClassCleanup()
        //{
        //}

        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize() => __RndGenerator = new Random();

        //Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup() => __RndGenerator = null;

        #endregion

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Тест экспоненциальной формы записи числа</summary>
        [TestMethod, Priority(1), Description("Тест экспоненциальной формы записи числа")]
        public void ExpTest()
        {
            Complex z;
            //var delta = new double[10000];
            for (var i = 0; i < 10000; i++)
            {
                z = Complex.Exp(1, RandomRad);
                Assert.AreEqual(1, z.Abs, Complex.Epsilon, "|exp(j*phi)| = |{0}| не равно 1", z.Abs);
                //delta[i] = Math.Abs(delta[i] - 1);
            }
            ////1.11022302e-16
            //Assert.Inconclusive(delta.Max().ToString());

            z = Complex.Exp(1, Math.PI / 2);
            Assert.AreEqual(1, z.Im, "exp(j*pi/2).Im не равно 1");

            z = Complex.Exp(1, 3 * Math.PI / 2);
            Assert.AreEqual(-1, z.Im, "exp(j*3*pi/2).Im не равно -1");

            z = Complex.Exp(1, 3 * Math.PI / 2);
            Assert.AreEqual(0, z.Re, Complex.Epsilon, "exp(j*3*pi/2).Re не равно 0");

            z = Complex.Exp(0, 0);
            Assert.AreEqual(0, (double)z, Complex.Epsilon, "0*exp(j*0).Im не равно 0");
        }
        /// <summary>Тест экспоненциальной формы записи числа для всей области определения аргумента</summary>
        [TestMethod, Priority(1), Description("Тест экспоненциальной формы записи числа для всей области определения аргумента")]
        public void ExpAroundTest()
        {
            const double dPhi = Consts.pi2 / 3600;
            for (var arg = 0.0; arg < Consts.pi2; arg += dPhi)
            {
                var z = Complex.Exp(arg);
                var Arg = z.Arg;
                Assert.AreEqual(Arg, z.Arg, Complex.Epsilon,
                    "exp(arg)=exp(j·{0}) -> arg(z)=arg({1})={2} != {0}=arg", arg, z, arg);
            }
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Тест конструктора комплексного числа</summary>
        [TestMethod, Priority(8), Timeout(100), Description("Тест конструктора комплексного числа")]
        public void ComplexConstructorTest()
        {
            var Re = Random;
            var Im = Random;
            var z = new Complex(Re, Im);
            Assert.AreEqual(Re, z.Re,
                "Ожидаемое значение {0} действительной части комплексного числа {1} установлено некорректно.", Re, z);
            Assert.AreEqual(Im, z.Im,
                "Ожидаемое значение {0} мнимой части комплексного числа {1} установлено некорректно.", Im, z);
        }

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Тест метода создания массива комплексных чисел</summary>
        [TestMethod, Priority(8), Timeout(1000), Description("Тест метода создания массива комплексных чисел")]
        public void CreateArrayTest()
        {
            var N = __RndGenerator.Next(100, 500);
            var Re = new double[N];
            var Im = new double[N];
            var expected = new Complex[N];
            for (var i = 0; i < N; i++)
            {
                Re[i] = Random;
                Im[i] = Random;
                expected[i] = new Complex(Re[i], Im[i]);
            }
            CollectionAssert.AreEqual(expected, Complex.CreateArray(Re, Im), "Массив не соответствует ожидаемому");
        }

        [TestMethod]
        public void EqualsTest()
        {
            var target = new Complex(5, 7);
            Assert.IsTrue(target.Equals(new Complex(5, 7)));
            Assert.IsFalse(target.Equals(Rnd));
            target = 7;
            Assert.IsTrue(target.Equals(7));
            Assert.IsTrue(target.Equals(7.0));
            Assert.IsFalse(target.Equals(Random));
            Assert.IsFalse(target.Equals(new object()));
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod]
        public void EqualsWithComplexTest()
        {
            var target = new Complex(1, 3);
            Assert.IsTrue(target.Equals(new Complex(1, 3)));
            Assert.IsFalse(target.Equals(Rnd));
        }

        /// <summary>
        ///A test for System.IEquatable<MathCore.Complex/>.Equals
        ///</summary>
        [TestMethod]
        [DeploymentItem("MathCore.dll")]
        public void EqualsIEquatableTest()
        {
            var Re = Random;
            var Im = Random;
            IEquatable<Complex> target = new Complex(Re, Im);
            Assert.IsTrue(target.Equals(new Complex(Re, Im)));
            Assert.IsFalse(target.Equals(Rnd));
        }

        /// <summary>A test for Exp</summary>
        [TestMethod]
        public void Exp_RandomCount_Test()
        {
            var N = __RndGenerator.Next(100, 500);
            for (var i = 0; i < N; i++)
            {
                var arg = RandomRad;
                if (arg > Math.PI)
                    arg -= 2 * Math.PI;
                var z = Complex.Exp(RandomPositive, arg);
                Assert.AreEqual(arg, z.Arg, Complex.Epsilon, "Ошибка в тесте {3}/{4}: Аргумент {0} = {1}pi не соответствует {2}pi",
                    z, z.Arg / Math.PI, arg / Math.PI, i + 1, N);
            }
        }


        /// <summary>A test for GetHashCode</summary>
        [TestMethod]
        public void GetHashCodeTest()
        {
            var x = Rnd;
            var y = Rnd;
            Assert.AreEqual(x.GetHashCode(), x.GetHashCode(),
                            "Хэш-коды двух одинаковых комплексных чисел не равны");
            Assert.AreNotEqual(x.GetHashCode(), y.GetHashCode(),
                               "Хэш-коды двух разных комплексных чисел равны");
        }

        /// <summary>
        ///A test for Mod
        ///</summary>
        [TestMethod]
        public void ModTest()
        {
            Assert.AreEqual(new Complex(1, 1), Complex.Mod(1, 1), "Mod.(1,1) не равно 1+i");
            Assert.AreEqual(new Complex(1), Complex.Mod(1), "Mod.(1) не равно 1");
            Assert.AreEqual(new Complex(-4.6, 3.7), Complex.Mod(-4.6, 3.7), "Mod.(-4.6,3.7) не равно -4.6+i3.7");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("3+2i", new Complex(3, 2).ToString());
            Assert.AreEqual("-3+2i", new Complex(-3, 2).ToString());
            Assert.AreEqual("3-2i", new Complex(3, -2).ToString());
            Assert.AreEqual("2i", new Complex(0, 2).ToString());
            Assert.AreEqual("2", new Complex(2).ToString());
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OperatorAdditionComplexArrayToComplexTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new Complex[N].Initialize(i => Rnd);
            var Y = Rnd;
            var expected = new Complex[N].Initialize(i => new Complex(X[i].Re + Y.Re, X[i].Im + Y.Im));
            (X + Y).Foreach((a, i) => Assert.AreEqual(expected[i], a));
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod, Description("Сумма комплексного числа с действительным")]
        public void OperatorAdditionComplexToDoubleTest()
        {
            Assert.AreEqual(6, new Complex(1) + 5, "(1+0i) + 5 не равно 6");
            Assert.AreEqual(new Complex(5, 1), new Complex(0, 1) + 5, "(0+1i) + 5 не равно 5+1i");
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OperatorAdditionDoubleArrayToComplexTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new double[N].Initialize(i => Random);
            var Y = Rnd;
            var expected = new Complex[N].Initialize(i => new Complex(X[i] + Y.Re, Y.Im));
            (X + Y).Foreach((a, i) => Assert.AreEqual(expected[i], a));
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OperatorAdditionComplexToComplexTest()
        {
            Assert.AreEqual(new Complex(1, 1), new Complex(1) + new Complex(0, 1), "1 + i != (1+1i)");
            Assert.AreEqual(new Complex(5, 7), new Complex(1, 2) + new Complex(4, 5), "(1+2i) + (4+5i) != (5+7i)");
            Assert.AreEqual(new Complex(0, 7), new Complex(5, 2) + new Complex(-5, 5), "(5+2i) + (-5+5i) != 7i");
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod]
        public void OperatorAdditionDoubleToComplexTest()
        {
            var a = 5;
            var b = new Complex(0, 7);
            var c = a + b;
            var expected_c = new Complex(5, 7);
            Assert.That.Value(c).IsEqual(expected_c, "5 + 7i != (5+7i)");
            Assert.AreEqual(0, (double)(new Complex(4, -12.33) - (-6.2 + new Complex(10.2, -12.33))),
                            1e-15, "-6.2 + (10.2-12.33i) != (4-12.33i)");
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OperatorDivisionDoubleToComplexTest()
        {
            var X = Random;
            Complex Y;
            do
                Y = Rnd;
            while (Y.Abs.Equals(0));
            var q = Y.Re * Y.Re + Y.Im * Y.Im;
            var (re, im) = X / Y;
            Assert.AreEqual(X * Y.Re / q, re, 1e-15);
            Assert.AreEqual(-X * Y.Im / q, im, 1e-15);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OperatorDivisionComplexToComplexTest()
        {
            var X = Rnd;
            Complex Y;
            do
                Y = Rnd;
            while (Y.Abs.Equals(0));
            var q = Y.Re * Y.Re + Y.Im * Y.Im;
            var (re, im) = X / Y;
            Assert.That.Value(re).IsEqual((X.Re * Y.Re + X.Im * Y.Im) / q, 2e-15);
            Assert.That.Value(im).IsEqual((X.Im * Y.Re - X.Re * Y.Im) / q, 2e-15);
        }

        /// <summary> test for op_Division</summary>
        [TestMethod]
        public void OperatorDivisionComplexArrayToComplexesTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new Complex[N].Initialize(i => Rnd);
            Complex Y;
            do
                Y = Rnd;
            while (Y.Abs.Equals(0));
            var expected = new Complex[N].Initialize(i =>
            {
                var q = Y.Re * Y.Re + Y.Im * Y.Im;
                return new Complex((X[i].Re * Y.Re + X[i].Im * Y.Im) / q, (X[i].Im * Y.Re - X[i].Re * Y.Im) / q);
            });
            (X / Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 1e-14);
                Assert.AreEqual(expected[i].Im, im, 1e-14);
            });
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OperatorDivisionDoubleArrayToComplexTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new double[N].Initialize(i => Random);
            Complex Y;
            do
                Y = Rnd;
            while (Y.Abs.Equals(0));
            var expected = new Complex[N].Initialize(i =>
            {
                var q = Y.Re * Y.Re + Y.Im * Y.Im;
                return new Complex(X[i] * Y.Re / q, -X[i] * Y.Im / q);
            });
            (X / Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 5e-15);
                Assert.AreEqual(expected[i].Im, im, 5e-15);
            });
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod]
        public void OperatorDivisionComplexToDoubleTest()
        {
            var X = Rnd;
            double Y;
            do
                Y = Random;
            while (Y.Equals(0));
            var (re, im) = X / Y;
            Assert.AreEqual(X.Re / Y, re, 1e-15);
            Assert.AreEqual(X.Im / Y, im, 1e-15);
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod]
        public void OperatorEqualityComplexToComplexTest()
        {
            var a = new Complex(1, 3);
            var b = new Complex(1, 3);
            Assert.AreEqual(true, a == b, "1+3i != 1+3i");
            Assert.AreNotEqual(true, a == new Complex(-5, 2), "1+3i == -5+2i");
        }

        /// <summary>
        ///A test for op_ExclusiveOr
        ///</summary>
        [TestMethod]
        public void OperatorPowerComplexToComplexTest()
        {
            var X = Rnd;
            var Y = Rnd;
            var r = X.Abs;
            var arg = X.Arg;
            var R = Math.Pow(r, Y.Re) * Math.Pow(Math.E, -Y.Im * arg);
            var Arg = Y.Re * arg + Y.Im * Math.Log(r);
            var (expected_re, _) = Complex.Exp(R, Arg);
            var (actual_re, _) = X ^ Y;
            Assert.AreEqual(expected_re, actual_re, 1e-15);
            Assert.AreEqual(expected_re, actual_re, 1e-15);
        }

        /// <summary>
        ///A test for op_ExclusiveOr
        ///</summary>
        [TestMethod]
        public void OperatorPowerComplexToDoubleTest()
        {
            var Z = Rnd;
            var X = Random;
            var (actual_re, actual_im) = Z ^ X;
            var (expected_re, expected_im) = Complex.Exp(Math.Pow(Z.Abs, X), Z.Arg * X);
            Assert.AreEqual(expected_re, actual_re, 1e-15);
            Assert.AreEqual(expected_im, actual_im, 1e-15);
        }

        /// <summary>
        ///A test for op_ExclusiveOr
        ///</summary>
        [TestMethod]
        public void OperatorPowerDoubleToComplexTest()
        {
            for (var i = 0; i < 100; i++)
            {
                double X;
                do
                    X = Random;
                while (X.Equals(0d));
                var Z = Rnd;
                Complex expected;
                if (X >= 0)
                    expected = Complex.Exp(Math.Pow(X, Z.Re), Z.Im * Math.Log(X));
                else
                {
                    var ln_x = Math.Log(Math.Abs(X));
                    expected = Complex.Exp(Z.Re * ln_x - pi * Z.Im, pi * Z.Re + ln_x * Z.Im);
                }
                var actual = X ^ Z;
                Assert.AreEqual(expected.Re, actual.Re, 1e-15, $"expected:({expected}) != actual({actual}) | x={X}, z={Z}");
                Assert.AreEqual(expected.Im, actual.Im, 1e-15, $"expected:({expected}) != actual({actual}) | x={X}, z={Z}");

            }
        }

        /// <summary>
        ///A test for op_Explicit
        ///</summary>
        [TestMethod]
        public void OperatorExplicitComplexToDoubleTest()
        {
            Assert.AreEqual(Math.Sqrt(5 * 5 + 8 * 8), (double)new Complex(5, 8), 1e-15, "(double)(5+8i) != Abs(5+8i)");
            Assert.AreEqual(5, (double)new Complex(5), 1e-15, "(double)(5+0i) != 5");
            Assert.AreEqual(8, (double)new Complex(0, 8), 1e-15, "(double)(0+8i) != 8");
        }

        /// <summary>
        ///A test for op_Implicit
        ///</summary>
        [TestMethod]
        public void OperatorImplicitDoubleToComplexTest()
        {
            Assert.AreEqual(new Complex(5), 5.0, "5 != (5+0i)");
            Complex x = 4;
            Assert.AreEqual(0, x.Im, 1e-15, "Im(4) != 0");
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod]
        public void OperatorInequalityComplexToComplexTest()
        {
            var X = new Complex(5, 8);
            var Y = new Complex(7, -2);
            Assert.AreEqual(true, X != Y, "(5+8i) == (7-2i)");
            Assert.AreEqual(true, X != new Complex(9), "(5+8i) == (9+0i)");
            Assert.AreEqual(false, X != new Complex(5, 8), "(5+8i) != (5+8i)");
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OperatorMultiplyComplexArrayToComplexTest()
        {
            var N = __RndGenerator.Next(5, 15);
            var X = new Complex[N].Initialize(i => Rnd);
            var Y = Rnd;
            var expected = X.Select(x => x * Y).ToArray();
            (X * Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 1e-15);
                Assert.AreEqual(expected[i].Im, im, 1e-15);
            });
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OperatorMultiplyDoubleToComplexTest()
        {
            var X = Random;
            var Y = Rnd;
            var (re, im) = X * Y;
            Assert.AreEqual(X * Y.Re, re, 1e-15);
            Assert.AreEqual(X * Y.Im, im, 1e-15);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OperatorMultiplyComplexToDoubleTest()
        {
            var X = Rnd;
            var Y = Random;
            var (re, im) = X * Y;
            Assert.AreEqual(X.Re * Y, re, 1e-15);
            Assert.AreEqual(X.Im * Y, im, 1e-15);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OperatorMultiplyDoubleArrayToComplexTest()
        {
            var N = __RndGenerator.Next(5, 15);
            var X = new double[N].Initialize(i => Random);
            var Y = Rnd;
            var expected = X.Select(x => x * Y).ToArray();
            (X * Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 1e-15);
                Assert.AreEqual(expected[i].Im, im, 1e-15);
            });
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod]
        public void OperatorMultiplyComplexToComplexTest()
        {
            var X = Rnd;
            var Y = Rnd;
            var (re, im) = X * Y;
            Assert.AreEqual(X.Re * Y.Re - X.Im * Y.Im, re);
            Assert.AreEqual(X.Re * Y.Im + X.Im * Y.Re, im);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OperatorSubtractionDoubleToComplexTest()
        {
            var X = Random;
            var Y = Rnd;
            var (expected_re, expected_im) = new Complex(X - Y.Re, -Y.Im);
            var (actual_re, actual_im) = X - Y;
            Assert.AreEqual(expected_re, actual_re, 1e-15);
            Assert.AreEqual(expected_im, actual_im, 1e-15);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OperatorSubtractionDoubleArrayToComplexTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new double[N].Initialize(i => Random);
            var Y = Rnd;
            var expected = new Complex[N].Initialize(i => X[i] - Y);
            (X - Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 1e-15);
                Assert.AreEqual(expected[i].Im, im, 1e-15);
            });
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod]
        public void OperatorSubtractionComplexArrayToComplexTest()
        {
            var N = __RndGenerator.Next(10) + 5;
            var X = new Complex[N].Initialize(i => Rnd);
            var Y = Rnd;
            var expected = X.Select(x => x - Y).ToArray();
            (X - Y).Foreach((z, i) =>
            {
                var (re, im) = z;
                Assert.AreEqual(expected[i].Re, re, 1e-15);
                Assert.AreEqual(expected[i].Im, im, 1e-15);
            });
        }

        /// <summary>A test for op_Subtraction</summary>
        [TestMethod]
        public void OperatorSubtractionComplexToDoubleTest()
        {
            var X = Rnd;
            var Y = Random;
            var (expected_re, expected_im) = new Complex(X.Re - Y, X.Im);
            var (actual_re, actual_im) = X - Y;
            Assert.AreEqual(expected_re, actual_re, 1e-15);
            Assert.AreEqual(expected_im, actual_im, 1e-15);
        }

        /// <summary>A test for op_Subtraction</summary>
        [TestMethod]
        public void OperatorSubtractionComplexToComplexTest()
        {
            var X = Rnd;
            var Y = Rnd;
            var (expected_re, expected_im) = new Complex(X.Re - Y.Re, X.Im - Y.Im);
            var (actual_re, actual_im) = X - Y;
            Assert.AreEqual(expected_re, actual_re, 1e-15);
            Assert.AreEqual(expected_im, actual_im, 1e-15);
        }

        /// <summary>A test for Abs</summary>
        [TestMethod]
        public void AbsTest()
        {
            var target = Rnd;
            var expected = Math.Sqrt(target.Re * target.Re + target.Im * target.Im);
            var actual = target.Abs;
            Assert.AreEqual(expected, actual, 1e-14, "Разница меду ожидаемым и полученным значением составила {0}({1:p}) ", expected - actual, Math.Abs(expected - actual) / expected);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetArg(Complex z) =>
            Math.Abs(z.Re) < double.Epsilon
                ? Math.Abs(z.Im) < double.Epsilon ? 0 : z.Im > 0 ? Math.PI / 2 : -Math.PI / 2
                : Math.Abs(z.Im) < double.Epsilon ? z.Re > 0 ? 0 : Math.PI : Math.Atan(z.Im / z.Re);

        /// <summary>A test for Arg</summary>
        [TestMethod]
        public void ArgTest()
        {
            var target = new Complex();
            Assert.AreEqual(GetArg(target) / pi, target.Arg / pi, 1e-15);
            target = new Complex(RandomPositive);
            Assert.AreEqual(GetArg(target) / pi, target.Arg / pi, 1e-15);
            target = new Complex(-RandomPositive);
            Assert.AreEqual(GetArg(target) / pi, target.Arg / pi, 1e-15);
            target = new Complex(0, RandomPositive);
            Assert.AreEqual(GetArg(target) / pi, target.Arg / pi, 1e-15);
            target = new Complex(0, -RandomPositive);
            Assert.AreEqual(GetArg(target) / pi, target.Arg / pi, 1e-15);
        }

        /// <summary>A test for ComplexConjugate</summary>
        [TestMethod]
        public void ComplexConjugateTest()
        {
            Assert.AreEqual(new Complex(5, -8), new Complex(5, 8).ComplexConjugate, "(5+8i)* != (5-8i)");
            Assert.AreEqual(new Complex(0, -8), new Complex(0, 8).ComplexConjugate, "8i* != -8i");
            Assert.AreEqual(new Complex(10), new Complex(10).ComplexConjugate, "(10+0i)* != (10-0i)");
        }

        /// <summary>
        ///A test for Im
        ///</summary>
        [TestMethod]
        public void ImTest()
        {
            Assert.AreEqual(7, new Complex(3, 7).Im, "Im(3+7i) != 7");
            Assert.AreEqual(0, new Complex(3).Im, "Im(3+0i) != 0");
            Assert.AreEqual(-10, new Complex(0, -10).Im, "Im(-10i) != 10");
        }

        /// <summary>
        ///A test for GetPower
        ///</summary>
        [TestMethod]
        public void PowerTest() => Assert.AreEqual(5 * 5 + 7 * 7, new Complex(5, 7).Power, "(5+7i)*(5+7i)^* != 5^2 + 7^2");

        /// <summary>
        ///A test for Re
        ///</summary>
        [TestMethod]
        public void ReTest()
        {
            var Re = Random;
            var Im = Random;
            var target = new Complex(Re, Im);
            Assert.AreEqual(Re, target.Re);
        }

        /// <summary>Тестирование статического свойства класса комплексных чисел "Мнимая единица"</summary>
        [TestMethod, Priority(1), Description("Тестирование статического свойства класса комплексных чисел \"Мнимая единица\"")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Стиль", "IDE1006:Стили именования", Justification = "<Ожидание>")]
        public void iTest()
        {
            Assert.AreEqual(1, Complex.i.Im, "Мнимая часть комплексного числа 0+i не равно 1");
            Assert.AreEqual(-1, (Complex.i ^ 2).Re, "Квадрат мнимой единицы не равен -1");
            Assert.AreEqual(Math.PI / 2, Complex.i.Arg, 1e-15, "Аргумент мнимой единицы не равен pi/2");
            Assert.AreEqual(1, Complex.i.Abs, 1e-15, "Модуль мнимой единицы не равен 1");
        }

        [TestMethod]
        public void Asin_Test()
        {
            const double im = 1.9652382426275;
            var z = new Complex(0, im);
            var (actual_asin_re, actual_asin_im) = Complex.Trigonometry.Asin(z);

            const double expected_asin_im = 1.427980580692356;
            Assert.That.Value(actual_asin_im).IsEqual(expected_asin_im, 2.23e-16);
            Assert.That.Value(actual_asin_re).IsEqual(0);
        }

        [TestMethod]
        public void Acos_Test()
        {
            const double im = 1.9652382426275;
            var z = new Complex(0, im);
            var (actual_acos_re, actual_acos_im) = Complex.Trigonometry.Acos(z);

            const double expected_acos_re = 1.570796326794897;
            const double expected_asin_im = -1.427980580692356;
            Assert.That.Value(actual_acos_re).IsEqual(expected_acos_re, 4.45e-16);
            Assert.That.Value(actual_acos_im).IsEqual(expected_asin_im, 2.23e-16);
        }
    }
}