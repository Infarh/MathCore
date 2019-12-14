using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MathCore.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DST = System.Diagnostics.DebuggerStepThroughAttribute;

namespace MathCore.Tests
{
    [TestClass]
    public class MatrixTest
    {
        /* ------------------------------------------------------------------------------------------ */

        private static Random RndGenerator;

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
        public void MyTestInitialize() => RndGenerator = new Random();

        //Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup() => RndGenerator = null;

        #endregion

        [DST, NotNull]
        public static IComparer GetComparer(double tolerance = 1e-14) => new LambdaComparer<double>((x1, x2) =>
        {
            var delta = x2 - x1;
            if (Math.Abs(delta) < tolerance) delta = 0;
            return Math.Sign(delta);
        });

        /* ------------------------------------------------------------------------------------------ */

        /// <summary>Тест оператора сложения двух матриц</summary>
        [TestMethod, Priority(0), Description("Тест оператора сложения двух матриц")]
        public void Creation_Test()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            var A = new Matrix(a);

            Assert.AreEqual(a[0, 0], A[0, 0]);
            Assert.AreEqual(a[0, 1], A[0, 1]);
            Assert.AreEqual(a[0, 2], A[0, 2]);

            Assert.AreEqual(a[1, 0], A[1, 0]);
            Assert.AreEqual(a[1, 1], A[1, 1]);
            Assert.AreEqual(a[1, 2], A[1, 2]);

            Assert.AreEqual(a[2, 0], A[2, 0]);
            Assert.AreEqual(a[2, 1], A[2, 1]);
            Assert.AreEqual(a[2, 2], A[2, 2]);

            a = new double[5, 7];
            A = (Matrix)a;
            Assert.AreEqual(a.GetLength(0), A.N);
            Assert.AreEqual(a.GetLength(1), A.M);
            Assert.AreEqual(0, A[3, 5]);
            Assert.AreEqual(0, a[3, 5]);
            A[3, 5] = 7;
            Assert.AreEqual(7, A[3, 5]);
            Assert.AreEqual(7, a[3, 5]);
        }

        /// <summary>Тест вычисления обратной матрицы</summary>
        [TestMethod, Priority(0), Description("Тест вычисления обратной матрицы")]
        public void Equals_Test()
        {
            double[,] a =
            {
                { 3, 4, 7, 5 },
                { 4, 6, 1, 7 },
                { 2, 6, 0, 2 },
                { 4, 5, 1, 3 }
            };

            var A = new Matrix(a);
            var B = new Matrix(a.CloneObject());

            Assert.AreEqual(A, B);
            Assert.IsTrue(A == B);
            Assert.AreEqual(A, a);
            Assert.IsTrue(A == a);

            double[,] c =
            {
                { 3, 4, 7, 0 },
                { 4, 6, 1, 7 },
                { 2, 6, 0, 2 },
                { 4, 5, 1, 3 }
            };

            var C = new Matrix(c);

            Assert.AreNotEqual(A, C);
            Assert.IsFalse(A == C);
            Assert.IsFalse(A == c);
        }

        /// <summary>Тест оператора сложения двух матриц</summary>
        [TestMethod, Priority(1), Description("Тест оператора сложения двух матриц")]
        public void OperatorAdd_Matrix_Matrix_Test()
        {
            double[,] a =
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            double[,] b =
            {
                { 7, 8, 9 },
                { 6, 1, 7 },
                { -3, -2, -1 }
            };

            var A = new Matrix(a);
            var B = new Matrix(b);

            var C = A + B;

            Assert.AreEqual(8, C[0, 0]);
            Assert.AreEqual(10, C[0, 1]);
            Assert.AreEqual(12, C[0, 2]);

            Assert.AreEqual(10, C[1, 0]);
            Assert.AreEqual(6, C[1, 1]);
            Assert.AreEqual(13, C[1, 2]);

            Assert.AreEqual(4, C[2, 0]);
            Assert.AreEqual(6, C[2, 1]);
            Assert.AreEqual(8, C[2, 2]);
        }

        /// <summary>Тест оператора сложения двух матриц</summary>
        [TestMethod, Priority(1), Description("Тест оператора сложения двух матриц")]
        public void OperatorSubstract_Matrix_Matrix_Test()
        {
            double[,] a =
            {
                { 5, 7, 9 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            double[,] b =
            {
                { 7, 8, 9 },
                { 6, 1, 7 },
                { -3, -2, -1 }
            };

            var A = new Matrix(a);
            var B = new Matrix(b);

            var C = A - B;

            Assert.AreEqual(-2, C[0, 0]);
            Assert.AreEqual(-1, C[0, 1]);
            Assert.AreEqual(0, C[0, 2]);

            Assert.AreEqual(-2, C[1, 0]);
            Assert.AreEqual(4, C[1, 1]);
            Assert.AreEqual(-1, C[1, 2]);

            Assert.AreEqual(10, C[2, 0]);
            Assert.AreEqual(10, C[2, 1]);
            Assert.AreEqual(10, C[2, 2]);
        }

        /// <summary>Тест оператора произведения двух матриц</summary>
        [TestMethod, Priority(1), Description("Тест оператора произведения двух матриц")]
        public void OperatorMultiply_Matrix_Matrix_Test()
        {
            double[,] a =
            {
                { 5, 7, 9 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            double[,] b =
            {
                { 7, 8, 9 },
                { 6, 1, 7 },
                { 2, 1, 3 }
            };

            var A = new Matrix(a);
            var B = new Matrix(b);

            var C = A * B;

            double[,] c =
            {
                { 95, 56, 121 },
                { 70, 43, 89 },
                { 115, 73, 146 }
            };

            Assert.AreEqual(95, C[0, 0]);
            Assert.AreEqual(70, C[1, 0]);
            Assert.AreEqual(115, C[2, 0]);

            Assert.AreEqual(56, C[0, 1]);
            Assert.AreEqual(43, C[1, 1]);
            Assert.AreEqual(73, C[2, 1]);

            Assert.AreEqual(121, C[0, 2]);
            Assert.AreEqual(89, C[1, 2]);
            Assert.AreEqual(146, C[2, 2]);

            Assert.AreEqual(C, c);
        }

        /* ------------------------------------------------------------------------------------------ */
    }
}
