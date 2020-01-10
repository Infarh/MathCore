using System;
using MathCore.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class Vector3DTests : UnitTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //[ClassInitialize] public static void MyClassInitialize(TestContext testContext) { }
        //[ClassCleanup] public static void MyClassCleanup() { }
        //[TestInitialize] public void MyTestInitialize() { }
        //[TestCleanup] public void MyTestCleanup() { }

        #endregion

        private static readonly Func<double, double> sin = Math.Sin;
        private static readonly Func<double, double> cos = Math.Cos;

        private static void TestVectorsComponents(double x, double y, double z, Vector3D r, double delta = double.Epsilon)
        {
            Assert.AreEqual(x, r.X, delta, "\r\nКомпонента вектора r.X = {0} не соответствует требуемому значению {1}", r.X, x);
            Assert.AreEqual(y, r.Y, delta, "\r\nКомпонента вектора r.Y = {0} не соответствует требуемому значению {1}", r.Y, y);
            Assert.AreEqual(z, r.Z, delta, "\r\nКомпонента вектора r.Z = {0} не соответствует требуемому значению {1}", r.Z, z);
        }

        /// <summary>Тест общего конструктора 3-х-мерного вектора</summary>
        [TestMethod, Priority(1), Description("Тест общего конструктора 3-х-мерного вектора")]
        public void Vector3DConstructor_Simple_Test()
        {
            TestVectorsComponents(0, 0, 0, new Vector3D(0, 0, 0));
            TestVectorsComponents(1, 0, 0, new Vector3D(1, 0, 0));
            TestVectorsComponents(0, 1, 0, new Vector3D(0, 1, 0));
            TestVectorsComponents(0, 0, 1, new Vector3D(0, 0, 1));
            var x = GetRNDDouble();
            var y = GetRNDDouble();
            var z = GetRNDDouble();
            TestVectorsComponents(x, y, z, new Vector3D(x, y, z));
        }

        /// <summary>Тест конструктора с одним параметром X</summary>
        [TestMethod, Priority(1), Description("Тест конструктора с одним параметром X")]
        public void Vector3DConstructor_X_Test()
        {
            TestVectorsComponents(0, 0, 0, new Vector3D(0));
            TestVectorsComponents(1, 0, 0, new Vector3D(1));
            var x = GetRNDDouble();
            TestVectorsComponents(x, 0, 0, new Vector3D(x));
        }

        /// <summary>Тест конструктора с двумя параметрами X, Y</summary>
        [TestMethod, Priority(1), Description("Тест конструктора с двумя параметрами X, Y")]
        public void Vector3DConstructor_XY_Test()
        {
            TestVectorsComponents(0, 0, 0, new Vector3D(0, 0));
            TestVectorsComponents(1, 0, 0, new Vector3D(1, 0));
            TestVectorsComponents(0, 1, 0, new Vector3D(0, 1));
            TestVectorsComponents(1, 1, 0, new Vector3D(1, 1));
            var x = GetRNDDouble();
            var y = GetRNDDouble();
            TestVectorsComponents(x, y, 0, new Vector3D(x, y));
        }

        /// <summary>Тестирование конструктора по радиусу и пространственному углу</summary>
        [TestMethod, Priority(1), Description("Тестирование конструктора по радиусу и пространственному углу")]
        public void Vector3DConstructor_R_Angle_Test()
        {
            const double pi = Consts.pi;
            const double pi05 = Consts.pi05;
            //const double pi2 = Consts.pi2;
            const double pi3_2 = 3 * pi * .5;

            //Action<double, SpaceAngle> testRA =
            //(r, a) =>
            //{
            //    var v = new Vector3D(r, a);
            //    Assert.AreEqual(r, v.R, 1e-15, "{0}.R = {1} != {2} = r", v, v.R, r);
            //    Assert.AreEqual(a, v.Angle, "{0}.a = {1}[π] != {2}[π] = r", v, v.Angle / pi, a / pi);
            //};

            static void TestXYZ(double r, SpaceAngle a, double x, double y, double z)
            {
                //testRA(r, a);
                var v = new Vector3D(r, a);
                Assert.AreEqual(x, v.X, 2e-16, "\r\n{0}.X = {1} != {2} = x", v, v.X, x);
                Assert.AreEqual(y, v.Y, 2e-16, "\r\n{0}.Y = {1} != {2} = x", v, v.Y, y);
                Assert.AreEqual(z, v.Z, 2e-16, "\r\n{0}.Z = {1} != {2} = x", v, v.Z, z);
            }

            var R = 0.0;
            var A = new SpaceAngle();
            TestXYZ(R, A, 0, 0, 0);

            R = 1;
            TestXYZ(R, A, 0, 0, 1);

            TestXYZ(1, new SpaceAngle(0.0, 0.0), 0, 0, 1);
            TestXYZ(1, new SpaceAngle(pi05, pi05), 0, 1, 0);
            TestXYZ(1, new SpaceAngle(pi05, 0.0), 1, 0, 0);

            TestXYZ(1, new SpaceAngle(pi, 0.0), 0, 0, -1);
            TestXYZ(1, new SpaceAngle(pi05, pi3_2), 0, -1, 0);
            TestXYZ(1, new SpaceAngle(pi05, pi), -1, 0, 0);


            double Theta;
            double Phi;

            void TestThetaPhi() => TestXYZ(R, new SpaceAngle(Theta, Phi), R * sin(Theta) * cos(Phi), R * sin(Theta) * sin(Phi), R * cos(Theta));

            for (Phi = -2 * pi; Phi <= 2 * pi; Phi += 0.1 * pi)
                for (Theta = -2 * pi; Theta <= 2 * pi; Theta += 0.1 * pi)
                {
                    R = GetRNDDouble();
                    TestThetaPhi();
                }
        }

        /// <summary>Тестирование конструктора по пространственному углу</summary>
        [TestMethod, Priority(1), Description("Тестирование конструктора по пространственному углу")]
        public void Vector3DConstructor_Angle_Test()
        {
            const double pi = Consts.pi;
            const double pi05 = Consts.pi05;
            //const double pi2 = Consts.pi2;
            const double pi3_2 = 3 * pi * .5;

            //Action<double, SpaceAngle> testRA =
            //(r, a) =>
            //{
            //    var v = new Vector3D(r, a);
            //    Assert.AreEqual(r, v.R, 1e-15, "{0}.R = {1} != {2} = r", v, v.R, r);
            //    Assert.AreEqual(a, v.Angle, "{0}.a = {1}[π] != {2}[π] = r", v, v.Angle / pi, a / pi);
            //};

            void TestXYZ(SpaceAngle a, double x, double y, double z)
            {
                //testRA(r, a);
                var v = a.DirectionalVector;
                Assert.AreEqual(x, v.X, 2e-16, "\r\n{0}.X = {1} != {2} = x", v, v.X, x);
                Assert.AreEqual(y, v.Y, 2e-16, "\r\n{0}.Y = {1} != {2} = x", v, v.Y, y);
                Assert.AreEqual(z, v.Z, 2e-16, "\r\n{0}.Z = {1} != {2} = x", v, v.Z, z);
            }

            var A = new SpaceAngle();
            TestXYZ(A, 0, 0, 1);

            TestXYZ(new SpaceAngle(0.0, 0.0), 0, 0, 1);
            TestXYZ(new SpaceAngle(pi05, pi05), 0, 1, 0);
            TestXYZ(new SpaceAngle(pi05, 0.0), 1, 0, 0);

            TestXYZ(new SpaceAngle(pi, 0.0), 0, 0, -1);
            TestXYZ(new SpaceAngle(pi05, pi3_2), 0, -1, 0);
            TestXYZ(new SpaceAngle(pi05, pi), -1, 0, 0);

            double Theta;
            double Phi;

            void TestThetaPhi() => TestXYZ(new SpaceAngle(Theta, Phi), sin(Theta) * cos(Phi), sin(Theta) * sin(Phi), cos(Theta));

            for (Phi = -2 * pi; Phi <= 2 * pi; Phi += 0.1 * pi)
                for (Theta = -2 * pi; Theta <= 2 * pi; Theta += 0.1 * pi)
                    TestThetaPhi();
        }

        /// <summary>Тестирование метода клонирования</summary>
        [TestMethod, Priority(2), Description("Тестирование метода клонирования")]
        public void Clone_Test()
        {
            var v = Vector3D.Random();
            Assert.AreEqual(v, v.Clone());
        }

        /// <summary>Тестирование метода определения эквивалентности между вектором и объектом</summary>
        [TestMethod, Priority(2), Description("Тестирование метода определения эквивалентности между вектором и объектом")]
        public void EqualsToObject_Test()
        {
            var v = Vector3D.Random();
            Assert.IsTrue(v.Equals((object)v));
            Assert.IsTrue(v.Equals((object)v.Clone()));
            Assert.IsFalse(v.Equals(null));
            Assert.IsFalse(v.Equals("123"));
            Assert.IsFalse(v.Equals(123));
            Assert.IsFalse(v.Equals(new object()));
        }

        /// <summary>Тестирование метода определения эквивалентности между векторами</summary>
        [TestMethod, Priority(2), Description("Тестирование метода определения эквивалентности между векторами")]
        public void EqualsToVector3D_Test()
        {
            const double eps = double.Epsilon;
            var V = Vector3D.Random();
            Assert.IsTrue(V.Equals(V));
            var v = V.Clone();
            Assert.IsTrue(v.Equals(v));
            Assert.IsTrue(new Vector3D(1, 0, 0).Equals(new Vector3D(1, 0, 0)));
            Assert.IsFalse(new Vector3D(1, 0, 0).Equals(new Vector3D(0, 1, 0)));
            Assert.IsTrue(new Vector3D(1, 0, 0).Equals(Vector3D.i));
            Assert.IsFalse(new Vector3D(1, 0, 0).Equals(Vector3D.j));
            do
            {
                V = Vector3D.Random();
                v = Vector3D.Random();
            }
            while (Math.Abs(V.X - v.X) < eps || Math.Abs(V.Y - v.Y) < eps || Math.Abs(V.Z - v.Z) < eps);
            Assert.IsFalse(V.Equals(v));
            Assert.IsFalse(v.Equals(V));
        }

        /// <summary>Тестирование метода определения угла между векторами</summary>
        [TestMethod, Priority(1), Description("Тестирование метода определения угла между векторами")]
        public void GetAngle_Test()
        {
            var x = new Vector3D(5, 5, 7);
            var y = new Vector3D(-10, 15, -3);

            var phi_x = Math.Atan2(x.Y, x.X);
            var phi_y = Math.Atan2(y.Y, y.X);
            var theta_x = Math.Atan2(Math.Sqrt(x.X * x.X + x.Y * x.Y), x.Z);
            var theta_y = Math.Atan2(Math.Sqrt(y.X * y.X + y.Y * y.Y), y.Z);

            var angle_x = new SpaceAngle(theta_x, phi_x);
            var angle_y = new SpaceAngle(theta_y, phi_y);
            var delta_theta = theta_x - theta_y;
            var delta_phi = phi_x - phi_y;
            var angle = new SpaceAngle(delta_theta, delta_phi);

            Assert.AreEqual(angle_x - angle_y, x.GetAngle(y));
            Assert.AreEqual(angle, x.GetAngle(y));
        }

        /// <summary>Тестирование метода получения обратного вектора</summary>
        [TestMethod, Priority(4), Description("Тестирование метода получения обратного вектора")]
        public void GetInverse_Test()
        {
            const double eps = double.Epsilon;
            Vector3D V;
            do { V = Vector3D.Random(); } while (Math.Abs(V.X) < eps || Math.Abs(V.Y) < eps || Math.Abs(V.Z) < eps);
            var inv = V.GetInverse();
            Assert.AreEqual(inv.X, 1 / V.X);
            Assert.AreEqual(inv.Y, 1 / V.Y);
            Assert.AreEqual(inv.Z, 1 / V.Z);
            Assert.AreEqual(inv, 1 / V);
        }

        /// <summary>Тестирование метода определения проекциии вектора на угловое направление</summary>
        [TestMethod, Priority(3), Description("Тестирование метода определения проекциии вектора на угловое направление")]
        public void GetProjectionToAngleDirection_Test()
        {
            GetProjectionToVector3D_Test();
            var Direction = new SpaceAngle();

            void Test(Vector3D v)
            {
                var p = v.GetProjectionTo(new Vector3D(Direction));
                var P = v.GetProjectionTo(Direction);
                Assert.That.Value(P).IsEqual(p, 6.0e-14);
            }

            Test(new Vector3D());

            Test(Vector3D.i);
            Test(Vector3D.j);
            Test(Vector3D.k);

            for (var i = 0; i < 1000; i++)
            {
                Direction = SpaceAngle.Random();
                Test(Vector3D.Random());
            }
        }

        /// <summary>Тестирование метода определения проекциии вектора на вектор</summary>
        [TestMethod, Priority(3), Description("Тестирование метода определения проекциии вектора на вектор")]
        public void GetProjectionToVector3D_Test()
        {
            Assert.AreEqual(0, Vector3D.i.GetProjectionTo(Vector3D.j));
            Assert.AreEqual(1, Vector3D.i.GetProjectionTo(Vector3D.i));

            Vector3D X;
            do { X = Vector3D.Random(); } while (Math.Abs(X.R) < double.Epsilon);

            Assert.AreEqual(X.X, X.GetProjectionTo(Vector3D.i));
            Assert.AreEqual(X.Y, X.GetProjectionTo(Vector3D.j));
            Assert.AreEqual(X.Z, X.GetProjectionTo(Vector3D.k));

            Vector3D Y;
            do { Y = Vector3D.Random(); } while (Math.Abs(Y.R) < double.Epsilon);

            X = new Vector3D(3, 5, 7);
            Y = new Vector3D(-5, 7, -10);

            var x_r = Math.Sqrt(X.X * X.X + X.Y * X.Y + X.Z * X.Z);
            var y_r = Math.Sqrt(Y.X * Y.X + Y.Y * Y.Y + Y.Z * Y.Z);
            var p = (X.X * Y.X + X.Y * Y.Y + X.Z * Y.Z);
            Assert.AreEqual(p / y_r, X.GetProjectionTo(Y));
            Assert.AreEqual(p / x_r, Y.GetProjectionTo(X));

            do { X = Vector3D.Random(); } while (Math.Abs(X.R) < double.Epsilon);
            do { Y = Vector3D.Random(); } while (Math.Abs(Y.R) < double.Epsilon);

            Assert.AreEqual((X * Y) / Y.R, X.GetProjectionTo(Y));
            Assert.AreEqual((X * Y) / X.R, Y.GetProjectionTo(X));
        }

        /// <summary>Тестирование метода преобразования вектора в базисе</summary>
        [TestMethod, Priority(4), Description("Тестирование метода преобразования вектора в базисе")]
        public void InBasis_Test()
        {
            var x = Vector3D.Random();
            var b = new Basis3D(1, 0, 0,
                                0, 1, 0,
                                0, 0, 1);

            void Test(Vector3D v) => Assert.AreEqual(v, x.InBasis(b));
            Test(x);

            b = new Basis3D(0, 1, 0,
                            1, 0, 0,
                            0, 0, 1);
            Test(new Vector3D(x.Y, x.X, x.Z));

            const double eps = 3e-16;
            const double pi = Consts.pi;
            const double angle = Consts.pi / 3;
            b = Basis3D.RotateOZ(angle);
            var y = x.InBasis(b);

            Assert.AreEqual(angle, (y.AngleXOY - x.AngleXOY).GetAbsMod(2 * pi), eps,
                "\r\n|(y.AngleXOY - x.AngleXOY) - angle| = {0:E}" +
                "\r\nangle = {1}·π" +
                "\r\nx.AngleXOY = {2}·π - {3}" +
                "\r\nx.AngleXOY = {4}·π - {5}" +
                "\r\ny.AngleXOY - x.AngleXOY = {6}·π",
                Math.Abs((y.AngleXOY - x.AngleXOY) - angle),
                angle / Consts.pi,
                x.AngleXOY / pi, x,
                y.AngleXOY / pi, y,
                (y.AngleXOY - x.AngleXOY) / pi);
        }

        /// <summary>Тестирование скалярного произведения двух векторов</summary>
        [TestMethod, Priority(2), Description("Тестирование скалярного произведения двух векторов")]
        public void Product_Scalar_VectorToVector_Test()
        {
            var a = new Vector3D();
            var b = new Vector3D();

            void Test()
            {
                Assert.AreEqual(a.X * b.X + a.Y * b.Y + a.Z * b.Z, a.Product_Scalar(b));
                Assert.AreEqual(a.X * b.X + a.Y * b.Y + a.Z * b.Z, b.Product_Scalar(a));
            }

            Test();

            a = Vector3D.Random();
            Test();
            b = Vector3D.Random();
            Test();
        }

        /// <summary>Тестирование метода смешанного произведения векторов </summary>
        [TestMethod, Priority(4), Description("Тестирование метода смешанного произведения векторов")]
        public void Product_Scalar_VectorToVectorToVector_Test()
        {
            var A = Vector3D.Random();
            var B = Vector3D.Random();
            var C = Vector3D.Random();

            Assert.AreEqual(A * B.Product_Vector(C), Vector3D.Product_Mixed(A, B, C));
        }

        /// <summary>Тестирование векторного произведения</summary>
        [TestMethod, Priority(2), Description("Тестирование векторного произведения")]
        public void Product_Vector_VectorToVector_Test()
        {
            var a = new Vector3D();
            var b = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X), a.Product_Vector(b));
            Test();
            a = Vector3D.Random();
            b = Vector3D.Random();
            Test();
        }

        /// <summary>Тест статического метода создания вектора по координатам</summary>
        [TestMethod, Priority(0), Description("Тест статического метода создания вектора по координатам")]
        public void Static_XYZ_Test()
        {
            var x = GetRNDDouble();
            var y = GetRNDDouble();
            var z = GetRNDDouble();

            Assert.AreEqual(new Vector3D(x, y, z), Vector3D.XYZ(x, y, z));
        }

        #region Операторы сложения

        /// <summary>Тест оператора сложения векторов</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения векторов")]
        public void op_Addition_VectorToVector_Test()
        {
            var a = new Vector3D();
            var b = new Vector3D();

            void Check()
            {
                var expected = new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
                Assert.AreEqual(expected, a + b);
                Assert.AreEqual(expected, b + a);
            }

            Check();
            a = Vector3D.Random();
            Check();
            b = Vector3D.Random();
            Check();
        }

        /// <summary>Тест оператора сложения скаляра с вектором</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения скаляра с вектором")]
        public void op_Addition_ScalarToVector_Test()
        {
            var x = 0.0;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), x + v);
            Test();
            v = Vector3D.Random();
            Test();
            x = GetRNDDouble();
            Test();
        }

        /// <summary>Тест оператора сложения вектором со скаляром</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения вектором со скаляром")]
        public void op_Addition_VectorToScalar_Test()
        {
            var x = 0.0;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), v + x);
            Test();
            v = Vector3D.Random();
            Test();
            x = GetRNDDouble();
            Test();
        }

        /// <summary>Тест оператора сложения скаляра (одинарной точности) с вектором</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения скаляра (одинарной точности) с вектором")]
        public void op_Addition_FloatScalarToVector_Test()
        {
            var x = 0.0f;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), x + v);
            Test();
            v = Vector3D.Random();
            Test();
            x = (float)GetRNDDouble();
            Test();
        }

        /// <summary>Тест оператора сложения вектором со скаляром (одинарной точности)</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения вектором со скаляром (одинарной точности)")]
        public void op_Addition_VectorToFloatScalar_Test()
        {
            var x = 0.0f;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), v + x);
            Test();
            v = Vector3D.Random();
            Test();
            x = (float)GetRNDDouble();
            Test();
        }

        /// <summary>Тест оператора сложения скаляра (целочисленного) с вектором</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения скаляра (целочисленного) с вектором")]
        public void op_Addition_IntScalarToVector_Test()
        {
            var x = 0;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), x + v);
            Test();
            v = Vector3D.Random();
            Test();
            x = (int)GetRNDDouble();
            Test();
        }

        /// <summary>Тест оператора сложения вектором со скаляром (целочисленного)</summary>
        [TestMethod, Priority(2), Description("Тест оператора сложения вектором со скаляром (целочисленного)")]
        public void op_Addition_IntToFloatScalar_Test()
        {
            var x = 0;
            var v = new Vector3D();

            void Test() => Assert.AreEqual(new Vector3D(v.X + x, v.Y + x, v.Z + x), v + x);
            Test();
            v = Vector3D.Random();
            Test();
            x = (int)GetRNDDouble();
            Test();
        }

        #endregion

        #region Оператор вычитания

        /// <summary>Тестирование оператора вычитания целого числа из вектора</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания целого числа из вектора")]
        public void op_Subtraction_VectorToInt_Test()
        {
            var v = Vector3D.Random();
            var x = GetRNDInt(-100, 100);
            TestVectorsComponents(v.X - x, v.Y - x, v.Z - x, v - x);
        }

        /// <summary>Тестирование оператора вычитания вектора из вектора</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вектора из вектора")]
        public void op_Subtraction_VectorToVector_Test()
        {
            var x = Vector3D.Random();
            var y = Vector3D.Random();
            TestVectorsComponents(x.X - y.X, x.Y - y.Y, x.Z - y.Z, x - y);
        }

        /// <summary>Тестирование оператора вычитания вектора из целого числа</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вектора из целого числа")]
        public void op_Subtraction_IntToVector_Test()
        {
            var v = Vector3D.Random();
            var x = GetRNDInt(-100, 100);
            TestVectorsComponents(x - v.X, x - v.Y, x - v.Z, x - v);
        }

        /// <summary>Тестирование оператора вычитания вектора из вещественного числа двойной точности</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вектора из вещественного числа двойной точности")]
        public void op_Subtraction_DoubleToVector_Test()
        {
            var v = Vector3D.Random();
            var x = GetRNDDouble();
            TestVectorsComponents(x - v.X, x - v.Y, x - v.Z, x - v);
        }

        /// <summary>Тестирование оператора вычитания вектора из вещественного числа одной точности</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вектора из вещественного числа одной точности")]
        public void op_Subtraction_FloatToVector_Test()
        {
            var v = Vector3D.Random();
            var x = (float)GetRNDDouble();
            TestVectorsComponents(x - v.X, x - v.Y, x - v.Z, x - v);
        }

        /// <summary>Тестирование оператора вычитания вещественного числа одной точности из вектора</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вещественного числа одной точности из вектора")]
        public void op_Subtraction_VectorToFloat_Test()
        {
            var v = Vector3D.Random();
            var x = (float)GetRNDDouble();
            TestVectorsComponents(v.X - x, v.Y - x, v.Z - x, v - x);
        }

        /// <summary>Тестирование оператора вычитания вещественного числа двойной точности из вектора</summary>
        [TestMethod, Priority(3), Description("Тестирование оператора вычитания вещественного числа двойной точности из вектора")]
        public void op_Subtraction_VectorToDouble_Test()
        {
            var v = Vector3D.Random();
            var x = GetRNDDouble();
            TestVectorsComponents(v.X - x, v.Y - x, v.Z - x, v - x);
        }

        #endregion

        #region Оператор умножения

        /// <summary>Тестирование оператора скалярного умножения векторов</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения векторов")]
        public void op_Multiply_VectorToVector_Test()
        {
            var a = Vector3D.Random();
            var b = Vector3D.Random();
            Assert.AreEqual(a.X * b.X + a.Y * b.Y + a.Z * b.Z, a * b);
        }

        /// <summary>Тестирование оператора скалярного умножения вектора на вещественное число двойной точности</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения вектора на вещественное число двойной точности")]
        public void op_Multiply_VectorToDouble_Test()
        {
            var a = Vector3D.Random();
            var b = GetRNDDouble();
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), a * b);
        }

        /// <summary>Тестирование оператора скалярного умножения вещественного число двойной точности на вектор</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения вещественного число двойной точности на вектор")]
        public void op_Multiply_DoubleToVector_Test()
        {
            var a = Vector3D.Random();
            var b = GetRNDDouble();
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), b * a);
        }

        /// <summary>Тестирование оператора скалярного умножения вектора на вещественное число одинарной точности</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения вектора на вещественное число одинарной точности")]
        public void op_Multiply_VectorToFloat_Test()
        {
            var a = Vector3D.Random();
            var b = (float)GetRNDDouble();
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), a * b);
        }

        /// <summary>Тестирование оператора скалярного умножения вещественного число одинарной точности на вектор</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения вещественного число одинарной точности на вектор")]
        public void op_Multiply_FloatToVector_Test()
        {
            var a = Vector3D.Random();
            var b = (float)GetRNDDouble();
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), b * a);
        }

        /// <summary>Тестирование оператора скалярного умножения целого числа на вектор</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения целого числа на вектор")]
        public void op_Multiply_VectorToInt_Test()
        {
            var a = Vector3D.Random();
            var b = GetRNDInt(-100, 100);
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), a * b);
        }

        /// <summary>Тестирование оператора скалярного умножения вектора на целое число</summary>
        [TestMethod, Priority(1), Description("Тестирование оператора скалярного умножения вектора на целое число")]
        public void op_Multiply_IntToVector_Test()
        {
            var a = Vector3D.Random();
            var b = GetRNDInt(-100, 100);
            Assert.AreEqual(new Vector3D(a.X * b, a.Y * b, a.Z * b), b * a);
        }

        #endregion

        #region Операторы деления (!)

        //[TestMethod, Ignore]
        //public void op_Division_ScalarToVector_Test()
        //{
        //    float x = 0F; 
        //    var V = new Vector3D(); 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (x/V);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Division_VectorToFloatScalar_Test()
        //{
        //    var V = new Vector3D(); 
        //    float x = 0F; 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (V/x);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Division_VectorToIntScalar_Test()
        //{
        //    var V = new Vector3D(); 
        //    int x = 0; 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (V/x);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Division_VectorToScalar_Test()
        //{
        //    var V = new Vector3D(); 
        //    double x = 0F; 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (V/x);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Division_ScalarToVector_Test()
        //{
        //    double x = 0F; 
        //    var V = new Vector3D(); 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (x/V);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Division_IntScalarToVector_Test()
        //{
        //    int x = 0; 
        //    var V = new Vector3D(); 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = (x/V);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //} 

        #endregion

        ///// <summary>Тест оператора определения ортогональности векторов</summary>
        //[TestMethod, Priority(3), Description("Тест оператора определения ортогональности векторов")]
        //public void op_Bitwise_And_Test()
        //{
        //    var A = new Vector3D();
        //    var B = new Vector3D();
        //    bool expected = false;
        //    bool actual;
        //    actual = (A & B);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");
        //}

        ///// <summary>Тест оператора определения параллельности векторов</summary>
        //[TestMethod, Priority(3), Description("Тест оператора определения параллельности векторов")]
        //public void op_Bitwise_Or_Test()
        //{
        //    var A = new Vector3D();
        //    var B = new Vector3D();
        //    bool expected = false;
        //    bool actual;
        //    actual = (A | B);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");
        //}

        ///// <summary>Тестирование оператор определения угла между векторами</summary>
        //[TestMethod, Priority(2), Description("Тестирование оператор определения угла между векторами")]
        //public void op_ExclusiveOr_Test()
        //{
        //    var A = new Vector3D();
        //    var B = new Vector3D();
        //    var expected = new SpaceAngle();
        //    SpaceAngle actual;
        //    actual = (A ^ B);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");
        //}

        #region Операторы преобразования (!)

        //[TestMethod, Ignore]
        //public void op_Explicit_Test()
        //{
        //    var Angle = new SpaceAngle(); 
        //    var expected = new Vector3D(); 
        //    Vector3D actual;
        //    actual = Angle;
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Implicit_ToStaceAngle_Test()
        //{
        //    var V = new Vector3D(); 
        //    var expected = new SpaceAngle(); 
        //    SpaceAngle actual;
        //    actual = V;
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");       
        //}

        //[TestMethod, Ignore]
        //public void op_Implicit_ToScalar_Test()
        //{
        //    var V = new Vector3D();
        //    double expected = 0F;
        //    double actual;
        //    actual = V;
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");
        //} 

        #endregion

        #region Операторы проекциии и т.п. (!)

        //[TestMethod, Ignore]
        //public void op_Modulus_VectorToVector_Test()
        //{
        //    var A = new Vector3D(); 
        //    var B = new Vector3D(); 
        //    double expected = 0F; 
        //    double actual;
        //    actual = (A%B);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");    
        //}

        //[TestMethod, Ignore]
        //public void op_Modulus_VectorToScalar_Test()
        //{
        //    var Vector = new Vector3D(); 
        //    var b = new Basis3D(); 
        //    double expected = 0F; 
        //    double actual;
        //    actual = (Vector%b);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //}

        //[TestMethod, Ignore]
        //public void op_Modulus_VectorToSpaceAngleDirection_Test()
        //{
        //    var Vector = new Vector3D(); 
        //    var Direction = new SpaceAngle(); 
        //    double expected = 0F; 
        //    double actual;
        //    actual = (Vector%Direction);
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //} 

        #endregion


        //[TestMethod, Ignore]
        //public void Angle_Test()
        //{
        //    var target = new Vector3D(); 
        //    var expected = new SpaceAngle(); 
        //    SpaceAngle actual;
        //    target.Angle = expected;
        //    actual = target.Angle;
        //    Assert.AreEqual(expected, actual);
        //    throw new NotImplementedException("Тестовый метод не реализован");     
        //}

        #region Углы проекциий в плоскостях (!)

        //[TestMethod, Ignore]
        //public void AngleXOY_Test()
        //{
        //    var target = new Vector3D(); 
        //    double actual;
        //    actual = target.AngleXOY;
        //    throw new NotImplementedException("Тестовый метод не реализован");      
        //}

        //[TestMethod, Ignore]
        //public void AngleXOZ_Test()
        //{
        //    var target = new Vector3D(); 
        //    double actual;
        //    actual = target.AngleXOZ;
        //    throw new NotImplementedException("Тестовый метод не реализован");     
        //}

        //[TestMethod, Ignore]
        //public void AngleYOZ_Test()
        //{
        //    var target = new Vector3D(); 
        //    double actual;
        //    actual = target.AngleYOZ;
        //    throw new NotImplementedException("Тестовый метод не реализован");     
        //} 

        #endregion

        #region Базисные вектора

        /// <summary>Тест статического свойства - базисный вектор</summary>
        [TestMethod, Priority(2), Description("Тест статического свойства - базисный вектор")]
        public void BasysUnitVector_Test() => TestVectorsComponents(1, 1, 1, Vector3D.BasysUnitVector);

        /// <summary>Тест статического свойства - базисный вектор i</summary>
        [TestMethod, Priority(2), Description("Тест статического свойства - базисный вектор i")]
        public void BasysVector_i_Test() => TestVectorsComponents(1, 0, 0, Vector3D.i);

        /// <summary>Тест статического свойства - базисный вектор j</summary>
        [TestMethod, Priority(2), Description("Тест статического свойства - базисный вектор j")]
        public void BasysVector_j_Test() => TestVectorsComponents(0, 1, 0, Vector3D.j);

        /// <summary>Тест статического свойства - базисный вектор k</summary>
        [TestMethod, Priority(2), Description("Тест статического свойства - базисный вектор k")]
        public void BasysVector_k_Test() => TestVectorsComponents(0, 0, 1, Vector3D.k);

        #endregion

        #region Основные углы

        /// <summary>Тест свойства азимутального угла</summary>
        [TestMethod, Priority(2), Description("Тест свойства азимутального угла")]
        public void Phi_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test() => Assert.AreEqual(Math.Atan2(y, x), new Vector3D(x, y, z).Phi);
            Test();

            z = GetRNDDouble();
            Test();

            x = GetRNDDouble();
            Test();

            y = GetRNDDouble();
            Test();
        }

        [TestMethod, Priority(2), Description("Тестирование свойства - угол места")]
        public void Theta_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test()
            {
                var v = new Vector3D(x, y, z);
                Assert.AreEqual(Math.Atan2(Math.Sqrt(x * x + y * y), z), v.Theta, "{0}", v);
            }

            Test();

            z = GetRNDDouble();
            Test();

            x = GetRNDDouble();
            Test();

            y = GetRNDDouble();
            Test();
        }

        #endregion

        #region Длины векторов

        /// <summary>Тест свойства вектора - Длина</summary>
        [TestMethod, Priority(1), Description("Тест свойства вектора - Длина")]
        public void R_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test()
            {
                var R = Math.Sqrt(x * x + y * y + z * z);

                var r = new Vector3D(x, y, z);
                var rR = r.R;
                Assert.AreEqual(R, rR, double.Epsilon, "Длина вектора {0} = {1} не соответствует ожидаемой {2}", r, rR, R);
            }

            Test();
            x = GetRNDDouble();
            Test();
            y = GetRNDDouble();
            Test();
            z = GetRNDDouble();
            Test();
        }

        #region Длины векторов в плоскостях

        /// <summary>Тестирование свойства - длина проекциии вектора в плоскости XOY</summary>
        [TestMethod, Priority(2), Description("Тестирование свойства - длина проекциии вектора в плоскости XOY")]
        public void R_XOY_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test()
            {
                var R = Math.Sqrt(x * x + y * y);

                var r = new Vector3D(x, y, z);
                var rR = r.R_XOY;
                Assert.AreEqual(R, rR, double.Epsilon, "Длина вектора {0} = {1} не соответствует ожидаемой {2}", r, rR, R);
            }

            Test();
            x = GetRNDDouble();
            Test();
            y = GetRNDDouble();
            Test();
            z = GetRNDDouble();
            Test();
        }

        /// <summary>Тестирование свойства - длина проекциии вектора в плоскости XOZ</summary>
        [TestMethod, Priority(2), Description("Тестирование свойства - длина проекциии вектора в плоскости XOZ")]
        public void R_XOZ_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test()
            {
                var R = Math.Sqrt(x * x + z * z);

                var r = new Vector3D(x, y, z);
                var rR = r.R_XOZ;
                Assert.AreEqual(R, rR, double.Epsilon, "Длина вектора {0} = {1} не соответствует ожидаемой {2}", r, rR, R);
            }

            Test();
            x = GetRNDDouble();
            Test();
            y = GetRNDDouble();
            Test();
            z = GetRNDDouble();
            Test();
        }

        /// <summary>Тестирование свойства - длина проекциии вектора в плоскости YOZ</summary>
        [TestMethod, Priority(2), Description("Тестирование свойства - длина проекциии вектора в плоскости YOZ")]
        public void R_YOZ_Test()
        {
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;

            void Test()
            {
                var R = Math.Sqrt(y * y + z * z);

                var r = new Vector3D(x, y, z);
                var rR = r.R_YOZ;
                Assert.AreEqual(R, rR, double.Epsilon, "Длина вектора {0} = {1} не соответствует ожидаемой {2}", r, rR, R);
            }

            Test();
            x = GetRNDDouble();
            Test();
            y = GetRNDDouble();
            Test();
            z = GetRNDDouble();
            Test();
        }

        #endregion

        #endregion

        #region Проекции вектора в плоскости

        //[TestMethod, Ignore]
        //public void VectorXOY_Test()
        //{
        //    var target = new Vector3D(); 
        //    Vector2D actual;
        //    actual = target.VectorXOY;
        //    throw new NotImplementedException("Тестовый метод не реализован");  
        //}

        //[TestMethod, Ignore]
        //public void VectorXOZ_Test()
        //{
        //    var target = new Vector3D(); 
        //    Vector2D actual;
        //    actual = target.VectorXOZ;
        //    throw new NotImplementedException("Тестовый метод не реализован");  
        //}

        //[TestMethod, Ignore]
        //public void VectorYOZ_Test()
        //{
        //    var target = new Vector3D(); 
        //    Vector2D actual;
        //    actual = target.VectorYOZ;
        //    throw new NotImplementedException("Тестовый метод не реализован");        
        //} 

        #endregion

        #region Базовые свойства

        /// <summary>Тест свойства - X</summary>
        [TestMethod, Priority(1), Description("Тест свойства - X")]
        public void X_Test()
        {
            var x = GetRNDDouble();
            var r = new Vector3D(x, 0, 0);
            Assert.AreEqual(x, r.X, "Свойство вектора r.X = {0} не соответствует ожидаемому x = {1}", r.X, x);
        }

        /// <summary>Тест свойства - Y</summary>
        [TestMethod, Priority(1), Description("Тест свойства - Y")]
        public void Y_Test()
        {
            var y = GetRNDDouble();
            var r = new Vector3D(0, y, 0);
            Assert.AreEqual(y, r.Y, "Свойство вектора r.Y = {0} не соответствует ожидаемому y = {1}", r.Y, y);
        }

        /// <summary>Тест свойства - Z</summary>
        [TestMethod, Priority(1), Description("Тест свойства - Z")]
        public void Z_Test()
        {
            var z = GetRNDDouble();
            var r = new Vector3D(0, 0, z);
            Assert.AreEqual(z, r.Z, "Свойство вектора r.Z = {0} не соответствует ожидаемому z = {1}", r.Z, z);
        }

        #endregion
    }
}