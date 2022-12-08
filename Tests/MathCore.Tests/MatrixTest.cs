#nullable enable
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

using DST = System.Diagnostics.DebuggerStepThroughAttribute;
// ReSharper disable InconsistentNaming

namespace MathCore.Tests;

[TestClass]
public class MatrixTest
{
    /* ------------------------------------------------------------------------------------------ */

    //private static Random RndGenerator;

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
    //[TestInitialize]
    //public void MyTestInitialize() => RndGenerator = new Random();

    //Use TestCleanup to run code after each test has run
    //[TestCleanup]
    //public void MyTestCleanup() => RndGenerator = null;

    #endregion

    [DST]
    public static IComparer GetComparer(double tolerance = 1e-14) => new LambdaComparer<double>((x1, x2) =>
    {
        var delta = x2 - x1;
        if (Math.Abs(delta) < tolerance) delta = 0;
        return Math.Sign(delta);
    });

    private static string MatrixArrayToString(double[,] matrix, string? Format = null, [CallerArgumentExpression(nameof(matrix))] string MatrixName = null!)
    {
        matrix.NotNull();
        var (n, m) = (matrix.GetLength(0), matrix.GetLength(1));

        var s = new StringBuilder("double[,] ")
           .Append(MatrixName)
           .AppendLine(" =")
           .AppendLine("{");

        var ss = new string[n, m];
        var ll = new int[m];

        var provider = CultureInfo.InvariantCulture;
        if (Format is not { Length: > 0 })
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                {
                    var s0 = matrix[i, j].ToString(Format, provider);
                    ss[i, j] = s0;
                    ll[j]    = Math.Max(ll[j], s0.Length);
                }
        else
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                {
                    var s0 = matrix[i, j].ToString(provider);
                    ss[i, j] = s0;
                    ll[j]    = Math.Max(ll[j], s0.Length);
                }

        for (var i = 0; i < n; i++)
        {
            s.Append(" { ");
            for (var j = 0; j < m; j++) 
                s.Append(ss[i, j].PadLeft(ll[j])).Append(", ");

            s.Length -= 2;
            s.AppendLine(" },");
        }

        s.Append("};");
        return s.ToString();
    }

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Тест оператора сложения двух матриц</summary>
    [TestMethod, Priority(0), Description("Тест оператора сложения двух матриц")]
    public void Creation()
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
    public void Equals()
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
        Assert.AreEqual<object>(A, a);
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
    public void OperatorAdd_Matrix_Matrix()
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
    public void OperatorSubtract_Matrix_Matrix()
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
    public void OperatorMultiply_Matrix_Matrix()
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

        Assert.AreEqual<object>(C, c);
    }

    [TestMethod]
    public void Triangulate()
    {
        static void CombineRows(double[,] matrix, int DestI, int SrcI1, double SrcK1, int SrcI2, double SrcK2)
        {
            var (n, m) = (matrix.GetLength(0), matrix.GetLength(1));
            if (DestI >= n) throw new ArgumentOutOfRangeException(nameof(DestI), DestI, "Номер строки назначения вышел за пределы массива");
            if (SrcI1 >= n) throw new ArgumentOutOfRangeException(nameof(SrcI1), SrcI1, "Номер строки источника 1 вышел за пределы массива");
            if (SrcI2 >= n) throw new ArgumentOutOfRangeException(nameof(SrcI2), SrcI2, "Номер строки источника 2 вышел за пределы массива");

            for (var j = 0; j < m; j++)
                matrix[DestI, j] = matrix[SrcI1, j] * SrcK1 + matrix[SrcI2, j] * SrcK2;
        }

        //static void SwapRows(double[,] matrix, int I1, int I2)
        //{
        //    var (n, m) = (matrix.GetLength(0), matrix.GetLength(1));
        //    if (I1 >= n) throw new ArgumentOutOfRangeException(nameof(I1), I1, "Номер строки I1 вышел за пределы массива");
        //    if (I2 >= n) throw new ArgumentOutOfRangeException(nameof(I2), I2, "Номер строки I2 вышел за пределы массива");

        //    if (I1 == I2) return;

        //    for (var j = 0; j < m; j++)
        //        (matrix[I1, j], matrix[I2, j]) = (matrix[I2, j], matrix[I1, j]);
        //}

        double[,] a =
        {
            { 1, 2, 3,  4 },
            { 0, 5, 6,  7 },
            { 0, 0, 8,  9 },
            { 0, 0, 0, 10 }
        };

        //CombineRows(a, 1, 0, 3, 1, 2);
        CombineRows(a, 2, 0, 5, 2, 2);
        CombineRows(a, 3, 0, -9, 3, 3);

        var rank = Matrix.Array.Triangulate(a, out var d);

        Debug.WriteLine(MatrixArrayToString(a));
    }

    /* ------------------------------------------------------------------------------------------ */
}