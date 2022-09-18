using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using MathCore.Annotations;

namespace MathCore.Tests;

[TestClass]
public class MatrixArrayTests
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

    #region DebugPrint

    [CanBeNull]
    public static string ToArrayFormat(double[,] array, [MathCore.Annotations.NotNull] string format = "g") =>
        array.ToStringFormatView(format, ", ")
          ?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
           .Select(s => $"\t\t\t\t{{ {s} }}")
           .JoinStrings(",\r\n")
           .ToFormattedString("{{\r\n{0}\r\n\t\t\t}}");

    //public static void ToDebug(double[,] u, double[] w, double[,] v)
    //{
    //    Debug.WriteLine("u0 = new[,]");
    //    Debug.WriteLine($"{ToArrayFormat(u)};");
    //    Debug.WriteLine(string.Empty);
    //    Debug.WriteLine($"w0 = new[] {ToArrayFormat(w)};");
    //    Debug.WriteLine(string.Empty);
    //    Debug.WriteLine("v0 = new[,]");
    //    Debug.WriteLine($"{ToArrayFormat(v)};");
    //}

    //[NotNull]
    //public static string ToArrayFormat([NotNull] double[] array, string format = "g") =>
    //    $"{{ {string.Join(", ", array.Select(v => v.ToString(format, CultureInfo.InvariantCulture)).ToArray())} }}";

    #endregion

    //[DebuggerStepThrough, NotNull]
    //private static double[,] GetRandom(int N, int M, double d = 1, double m = 0) =>
    //    new double[N, M].Initialize(new Tuple<double, double, Random>(m, d, RndGenerator), (i, j, v) => v.Item1 + v.Item2 * v.Item3.NextDouble());

    //[DebuggerStepThrough, NotNull]
    //private static double[,] GetRandom(int N, double d = 1, double m = 0) => GetRandom(N, N, d, m);

    //private static double[,] GetRandom_NonSingular(int N, double d = 1, double m = 0)
    //{
    //    var matrix = GetRandom(N, d, m);

    //    while (Matrix.Array.IsMatrixSingular(matrix))
    //        matrix[RndGenerator.Next(0, N - 1), RndGenerator.Next(0, N - 1)] = m + d * RndGenerator.NextDouble();

    //    return matrix;
    //}

    //[DebuggerStepThrough, NotNull]
    //private static double[,] GetRandom_Singular(int N, double d = 1, double m = 0)
    //{
    //    var matrix = GetRandom(N, d, m);
    //    var i1 = RndGenerator.Next(0, N - 1);
    //    var i2 = RndGenerator.Next(0, N - 1);
    //    if (i1 == i2) i2 = (i2 + 1) % N;
    //    var k = RndGenerator.NextDouble();
    //    for (var j = 0; j < N; j++)
    //        matrix[i2, j] = matrix[i1, j] * k;

    //    return matrix;
    //}

    [DebuggerStepThrough, MathCore.Annotations.NotNull]
    private static double[,] GetUnitaryArrayMatrix(int n) => Matrix.Array.GetUnitaryArrayMatrix(n);

    /* ------------------------------------------------------------------------------------------ */

    /// <summary>Тест процедуры приведения матрицы к треугольному виду</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду")]
    public void Triangulate_Test()
    {
        double[,] m =
        {
            { 1, 2 },
            { 3, 4 }
        };

        double[,] t0 =
        {
            { 1, 2 },
            { 0, -2 }
        };

        var t = m.CloneObject();
        var rank = Matrix.Array.Triangulate(t, out var d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        t0 = new double[,]
        {
            { 1, 2, 3 },
            { 0, -3, -6 },
            { 0, 0, 0 }
        };

        t = m.CloneObject();
        rank = Matrix.Array.Triangulate(t, out d);
        Assert.AreEqual(0, d);
        Assert.AreEqual(m.GetLength(0) - 1, rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 0d, 1, 2 },
            { 3d, 4, 5 },
            { 6d, 7, 0 }
        };

        t0 = new[,]
        {
            { 6, 7, 0 },
            { 0, 0.5, 5 },
            { 0, 0, -8 }
        };

        t = m.CloneObject();
        rank = Matrix.Array.Triangulate(t, out d);
        Assert.AreEqual(24, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 10, 9d, 8, 7, 6 },
            { 11, 12, 4, 2, 7 }
        };

        t0 = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 0d, -11, -22, -33, -44 },
            { 0, 0, -9, -12, -8 }
        };

        t = m.CloneObject();
        rank = Matrix.Array.Triangulate(t, out d);
        Assert.AreEqual(99, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 2d, 3, 4 },
            { 3d, 5, 1 },
            { 7d, 8, 4 },
            { 0d, 3, 4 },
            { 6d, 2, 1 }
        };

        t0 = new[,]
        {
            { 2d, 3, 4 },
            { 0d, 0.5, -5 },
            { 0d, 0, -35 },
            { 0d, 0, 0 },
            { 0d, 0, 0 }
        };

        t = m.CloneObject();
        rank = Matrix.Array.Triangulate(t, out d);
        Assert.AreEqual(-35, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 10, 9d, 8, 7, 6 },
            { 2d, 4d, 6, 8, 10 },
            { 8d, 16d, 24, 32, 40 }
        };

        t0 = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 0d, -11, -22, -33, -44 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t = m.CloneObject();
        rank = Matrix.Array.Triangulate(t, out d);
        Assert.AreEqual(0, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду")]
    public void GetTriangle_Test()
    {
        double[,] m =
        {
            { 1, 2 },
            { 3, 4 }
        };

        double[,] t0 =
        {
            { 1, 2 },
            { 0, -2 }
        };

        var t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out var rank, out var d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        t0 = new double[,]
        {
            { 1, 2, 3 },
            { 0, -3, -6 },
            { 0, 0, 0 }
        };

        t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out rank, out d);
        Assert.AreEqual(0, d);
        Assert.AreEqual(m.GetLength(0) - 1, rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 0d, 1, 2 },
            { 3d, 4, 5 },
            { 6d, 7, 0 }
        };

        t0 = new[,]
        {
            { 6, 7, 0 },
            { 0, 0.5, 5 },
            { 0, 0, -8 }
        };

        t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out rank, out d);
        Assert.AreEqual(24, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 10, 9d, 8, 7, 6 },
            { 11, 12, 4, 2, 7 }
        };

        t0 = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 0d, -11, -22, -33, -44 },
            { 0, 0, -9, -12, -8 }
        };

        t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out rank, out d);
        Assert.AreEqual(99, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 2d, 3, 4 },
            { 3d, 5, 1 },
            { 7d, 8, 4 },
            { 0d, 3, 4 },
            { 6d, 2, 1 }
        };

        t0 = new[,]
        {
            { 2d, 3, 4 },
            { 0d, 0.5, -5 },
            { 0d, 0, -35 },
            { 0d, 0, 0 },
            { 0d, 0, 0 }
        };

        t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out rank, out d);
        Assert.AreEqual(-35, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);

        m = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 10, 9d, 8, 7, 6 },
            { 2d, 4d, 6, 8, 10 },
            { 8d, 16d, 24, 32, 40 }
        };

        t0 = new[,]
        {
            { 1d, 2d, 3, 4, 5 },
            { 0d, -11, -22, -33, -44 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out rank, out d);
        Assert.AreEqual(0, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ")]
    public void TriangulateWithRightPart_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };

        var t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 }
        };

        var b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 }
        };

        var t = m.CloneObject();
        var b1 = b.CloneObject();
        var rank = Matrix.Array.Triangulate(t, b1, out var d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);

        m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 },
            { 3d, 6 },
            { 9d, 12 },
        };

        b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 },
            { 0, 0 },
            { 0, 0 }
        };

        b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t = m.CloneObject();
        b1 = b.CloneObject();
        rank = Matrix.Array.Triangulate(t, b1, out d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ")]
    public void GetTriangleWithRightPart_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };

        var t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 }
        };

        var b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 }
        };

        var t = m.CloneObject();
        var b1 = b.CloneObject();
        t = Matrix.Array.GetTriangle(t, b1, out var rank, out var d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);

        m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 },
            { 3d, 6 },
            { 9d, 12 },
        };

        b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 },
            { 0, 0 },
            { 0, 0 }
        };

        b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t = m.CloneObject();
        b1 = b.CloneObject();
        t = Matrix.Array.GetTriangle(t, b1, out rank, out d);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с присоединённой матрицей правых частей СЛАУ")]
    public void GetTriangleWithRightPartAndPermutation_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };

        var t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 }
        };

        var b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 }
        };

        var p0 = new[,]
        {
            { 1d, 0 },
            { 0d, 1 }
        };

        var t = m.CloneObject();
        var b1 = b.CloneObject();
        var b2 = b1;
        t = Matrix.Array.GetTriangle(t, ref b2, out var p, out var rank, out var d, true);
        Assert.AreNotEqual(b1, b2);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b2);
        CollectionAssert.AreEqual(p0, p);
        t = m.CloneObject();
        b1 = b.CloneObject();
        b2 = b1;
        t = Matrix.Array.GetTriangle(t, ref b1, out p, out rank, out d, false);
        Assert.AreEqual(b1, b2);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b2);
        CollectionAssert.AreEqual(p0, p);

        m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 },
            { 3d, 6 },
            { 9d, 12 },
        };

        b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 },
            { 0, 0 },
            { 0, 0 }
        };

        b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        p0 = new[,]
        {
            { 1d, 0, 0 , 0},
            { 0d, 1, 0, 0 },
            { 0d, 0, 1, 0 },
            { 0d, 0, 0, 1 }
        };

        t = m.CloneObject();
        b1 = b.CloneObject();
        b2 = b1;
        t = Matrix.Array.GetTriangle(t, ref b2, out p, out rank, out d, true);
        Assert.AreNotEqual(b1, b2);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b2);
        CollectionAssert.AreEqual(p0, p);
        t = m.CloneObject();
        b1 = b.CloneObject();
        b2 = b1;
        t = Matrix.Array.GetTriangle(t, ref b2, out p, out rank, out d, false);
        Assert.AreEqual(b1, b2);
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b2);
        CollectionAssert.AreEqual(p0, p);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с перестановкой строк</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с перестановкой строк")]
    public void Triangulate_WithPermutations_Test()
    {
        var m = new[,]
        {
            { 1d, 3, 5 },
            { 2d, 6, 0 },
            { 7, 12, -3 }
        };

        var t0 = new[,]
        {
            { 1d, 3, 5 },
            { 0, -9, -38 },
            { 0, 0, -10 }
        };

        var p0 = new double[,]
        {
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 0, 1, 0 }
        };

        var t = m.CloneObject();
        var rank = Matrix.Array.Triangulate(t, out var p, out var d);
        Assert.AreEqual(m.GetLength(0), rank);
        Assert.AreEqual(-90, d);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(p0, p);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с перестановкой строк</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с перестановкой строк")]
    public void GetTriangle_WithPermutations_Test()
    {
        var m = new[,]
        {
            { 1d, 3, 5 },
            { 2d, 6, 0 },
            { 7, 12, -3 }
        };

        var t0 = new[,]
        {
            { 1d, 3, 5 },
            { 0, -9, -38 },
            { 0, 0, -10 }
        };

        var p0 = new double[,]
        {
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 0, 1, 0 }
        };

        var t = m.CloneObject();
        t = Matrix.Array.GetTriangle(t, out var p, out var rank, out var d);
        Assert.AreEqual(m.GetLength(0), rank);
        Assert.AreEqual(-90, d);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(p0, p);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с клонированием исходной матрицы</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с клонированием исходной матрицы")]
    public void Triangulate_WithCloningMatrix_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };

        var t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 }
        };

        var b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 }
        };

        var t = m;
        var b1 = b.CloneObject();
        var rank = Matrix.Array.Triangulate(ref t, b1, out var d);
        Assert.IsFalse(ReferenceEquals(t, m));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);

        t = m;
        b1 = b.CloneObject();
        rank = Matrix.Array.Triangulate(ref t, b1, out d, false);
        Assert.IsTrue(ReferenceEquals(t, m));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);
    }

    /// <summary>Тест процедуры приведения матрицы к треугольному виду с клонированием исходной матрицы и матрицы правой части</summary>
    [TestMethod, Priority(2), Description("Тест процедуры приведения матрицы к треугольному виду с клонированием исходной матрицы и матрицы правой части")]
    public void Triangulate_WithCloningMatrixAndRightPart_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };

        var t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 }
        };

        var b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 }
        };

        var p0 = new[,]
        {
            { 1d, 0 },
            { 0d, 1 }
        };

        var t = m;
        var b1 = b;
        var rank = Matrix.Array.Triangulate(ref t, ref b1, out var p, out var d, true, true);
        Assert.IsFalse(ReferenceEquals(t, m));
        Assert.IsFalse(ReferenceEquals(b1, b));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(p0, p);
        CollectionAssert.AreEqual(b0, b1);

        t = m;
        b1 = b;
        rank = Matrix.Array.Triangulate(ref t, ref b1, out p, out d, false, false);
        Assert.IsTrue(ReferenceEquals(t, m));
        Assert.IsTrue(ReferenceEquals(b1, b));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0), rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(p0, p);
        CollectionAssert.AreEqual(b0, b1);

        m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 },
            { 3d, 6 },
            { 9d, 12 },
        };

        b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        t0 = new[,]
        {
            { 1d, 2 },
            { 0, -2 },
            { 0, 0 },
            { 0, 0 }
        };

        b0 = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10-3*3, 5-7*3, 3-8*3, -2-0*3, 0-12*3 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };

        //            p0 = new[,]
        //            {
        //                { 1d, 0 },
        //                { 0d, 1 }
        //            };

        t = m;
        b1 = b;
        rank = Matrix.Array.Triangulate(ref t, ref b1, out _, out _);
        Assert.IsFalse(ReferenceEquals(t, m));
        Assert.IsFalse(ReferenceEquals(b1, b));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);

        t = m;
        b1 = b;
        rank = Matrix.Array.Triangulate(ref t, ref b1, out _, out d, false, false);
        Assert.IsTrue(ReferenceEquals(t, m));
        Assert.IsTrue(ReferenceEquals(b1, b));
        Assert.AreEqual(-2, d);
        Assert.AreEqual(m.GetLength(0) - 2, rank);
        CollectionAssert.AreEqual(t0, t);
        CollectionAssert.AreEqual(b0, b1);
    }

    [TestMethod]
    public void TrySolve_Exceptions()
    {
        double[,] m = null;
        double[,] b = null;
        double[,] p;
        Exception error;
        try
        {
            Matrix.Array.TrySolve(m, ref b, out p);
            error = null;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            error = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(error);
        Assert.IsInstanceOfType(error, typeof(ArgumentNullException));
        Assert.AreEqual("matrix", ((ArgumentNullException)error).ParamName);

        m = new double[5, 7];
        b = null;
        try
        {
            Matrix.Array.TrySolve(m, ref b, out p);
            error = null;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            error = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(error);
        Assert.IsInstanceOfType(error, typeof(ArgumentNullException));
        Assert.AreEqual("b", ((ArgumentNullException)error).ParamName);

        m = new double[5, 8];
        b = new double[5, 8];
        try
        {
            Matrix.Array.TrySolve(m, ref b, out p);
            error = null;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            error = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(error);
        Assert.IsInstanceOfType(error, typeof(ArgumentException));
        Assert.AreEqual("matrix", ((ArgumentException)error).ParamName);

        m = new double[5, 5];
        b = new double[6, 8];
        try
        {
            Matrix.Array.TrySolve(m, ref b, out p);
            error = null;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            error = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(error);
        Assert.IsInstanceOfType(error, typeof(ArgumentException));
        Assert.AreEqual("b", ((ArgumentException)error).ParamName);
    }

    /// <summary>Тест вычисления СЛАУ методом Гаусса</summary>
    [TestMethod, Priority(2), Description("Тест вычисления СЛАУ методом Гаусса")]
    public void TrySolve_Test()
    {
        // Единичная матрица 2х2
        var I = GetUnitaryArrayMatrix(2);

        // Матрица системы
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        // Матрица правой части (единичная матрица)
        var b = I.CloneObject();
        // Матрица правой части для последующего сравнения результата решения системы
        var b1 = b.CloneObject();
        // Результат решения задачи
        var b0 = new[,]
        {
            { -2d, 1 },
            { 1.5, -.5 },
        };
        // Истина, если решение найдено
        Assert.IsTrue(Matrix.Array.TrySolve(m, ref b, out var p), "В результате решения СЛАУ метод определил, что матрица вырождена");
        // Сравниваем полученный (b) результат с требуемым (b0)
        CollectionAssert.AreEqual(b0, b);
        // Матрица перестановок должна быть (точно) равна единичной матрице
        Assert.IsTrue(Matrix.Array.AreEquals(I, p), "Матрица перестановок не равна единичной матрице");

        // Подстановка решения в систему. Должны получить матрицу правой части системы
        var x = Matrix.Array.Operator.Multiply(m, b);
        // Проверяем соответствие результата подстановки решения в СЛАУ с исходной правой частью
        CollectionAssert.AreEqual(b1, x);

        m = new[,]
        {
            { 1d, 3 },
            { 2d, 1 }
        };

        b = new[,]
        {
            { 5d },
            { 12 }
        };
        b1 = b.CloneObject();

        b0 = new[,]
        {
            { 6.2 },
            { -.4 },
        };

        Assert.IsTrue(Matrix.Array.TrySolve(m, ref b, out p));

        CollectionAssert.AreEqual(b0, b);
        CollectionAssert.AreEqual(I, p);

        x = Matrix.Array.Operator.Multiply(m, b);
        CollectionAssert.AreEqual(b1, x);

        b = new[,]
        {
            { 3d, 7, 8, 0, 12 },
            { 10, 5, 3, -2, 0 }
        };
        b1 = b.CloneObject();

        b0 = new[,]
        {
            {  5.4, 1.6, 0.2, -1.2, -2.4 },
            { -0.8, 1.8, 2.6,  0.4,  4.8 }
        };

        var comparer = MatrixTest.GetComparer();
        Assert.IsTrue(Matrix.Array.TrySolve(m, ref b, out p));
        CollectionAssert.AreEqual(b0, b, comparer);
        CollectionAssert.AreEqual(I, p);

        x = Matrix.Array.Operator.Multiply(m, b);
        CollectionAssert.AreEqual(b1, x, comparer);

        m = new[,]
        {
            { 7d, 8, 9 },
            { 6d, 7, 8 },
            { 2d, 0, 3 }
        };

        b = GetUnitaryArrayMatrix(3);

        b0 = new[,]
        {
            {  4.2, -4.8,  0.2},
            { -0.4,  0.6, -0.4},
            { -2.8, 3.2,  0.2}
        };

        I = GetUnitaryArrayMatrix(3);
        Assert.IsTrue(Matrix.Array.TrySolve(m, ref b, out p));
        CollectionAssert.AreEqual(I, p);
        CollectionAssert.AreEqual(b0, b, MatrixTest.GetComparer(1e-13));

    }

    /// <summary>Тест вычисления СЛАУ методом Гаусса с генерацией исключения в случае если матрица системы вырождена</summary>
    [TestMethod, Priority(2), Description("Тест вычисления СЛАУ методом Гаусса с генерацией исключения в случае если матрица системы вырождена")]
    public void Solve_Test()
    {
        var I = GetUnitaryArrayMatrix(2);

        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = I.CloneObject();
        var b1 = b.CloneObject();

        var b0 = new[,]
        {
            { -2d, 1 },
            { 1.5, -.5 },
        };

        Matrix.Array.Solve(m, ref b, out var p);

        CollectionAssert.AreEqual(b, b0);
        CollectionAssert.AreEqual(I, p);

        var x = Matrix.Array.Operator.Multiply(m, b);
        CollectionAssert.AreEqual(x, b1);

        b = I;
        b1 = b.CloneObject();
        var b2 = b1;

        Matrix.Array.Solve(m, ref b2, out _);
        Assert.AreEqual(b1, b2);
        CollectionAssert.AreEqual(b2, b0);

        b1 = b.CloneObject();
        b2 = b1;

        Matrix.Array.Solve(m, ref b2, out _, true);
        Assert.AreNotEqual(b1, b2);
        CollectionAssert.AreEqual(b0, b2);

        m = new[,]
        {
            { 1d, 2, 3 },
            { 4d, 5, 6 },
            { 7d, 8, 9 },
        };

        b = GetUnitaryArrayMatrix(3);
        b1 = b.CloneObject();
        b2 = b1;
        try
        {
            Matrix.Array.Solve(m, ref b2, out p);
            Assert.Fail();

        }
        // ReSharper disable once CatchAllClause
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.AreEqual(b1, b2);
        CollectionAssert.AreEqual(b, b2);
    }

    /// <summary>Тест вычисления СЛАУ методом Гаусса</summary>
    [TestMethod, Priority(2), Description("Тест вычисления СЛАУ методом Гаусса")]
    public void GetSolve_Test()
    {
        var m = new[,]
        {
            { 1d, 2 },
            { 3d, 4 }
        };

        var b = GetUnitaryArrayMatrix(2);

        var x0 = new[,]
        {
            { -2d, 1 },
            { 1.5, -.5 },
        };

        var b1 = b.CloneObject();
        var x = Matrix.Array.GetSolve(m, b1, out var p);
        CollectionAssert.AreEqual(GetUnitaryArrayMatrix(2), p);
        CollectionAssert.AreEqual(b, b1);
        CollectionAssert.AreEqual(x0, x);

        m = new[,]
        {
            { 1d, 2, 3 },
            { 4d, 5, 6 },
            { 7d, 8, 9 },
        };

        b = GetUnitaryArrayMatrix(3);
        b1 = b.CloneObject();
        try
        {
            x = Matrix.Array.GetSolve(m, b1, out p);
            Assert.Fail();

        }
        // ReSharper disable once CatchAllClause
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            Assert.IsInstanceOfType(e, typeof(InvalidOperationException));
        }
#pragma warning restore CA1031 // Do not catch general exception types
        CollectionAssert.AreEqual(b, b1);
        CollectionAssert.AreEqual(x0, x);
    }

    /// <summary>Тест вычисления определителя матрицы</summary>
    [TestMethod, Priority(3), Description("Тест вычисления определителя матрицы")]
    public void GetDeterminant_Test()
    {
        double[,] a =
        {
            { 7, 8 },
            { 6, 7 }
        };

        var d = Matrix.Array.GetDeterminant(a);

        Assert.AreEqual(1, d);

        double[,] m =
        {
            { 7, 8, 9 },
            { 6, 7, 8 },
            { 2, 0, 3 }
        };

        d = Matrix.Array.GetDeterminant(m);


        Assert.AreEqual(5, d);

        double[,] c =
        {
            { 3, 4, 7, 5 },
            { 4, 6, 1, 7 },
            { 2, 6, 0, 2 },
            { 4, 5, 1, 3 }
        };

        d = Matrix.Array.GetDeterminant(c);

        Assert.AreEqual(-334d, d, 1e-12);

        m = new double[0, 0];

        d = Matrix.Array.GetDeterminant(m);

        Assert.IsTrue(double.IsNaN(d));

        m = new double[,] { { 5 } };

        d = Matrix.Array.GetDeterminant(m);

        Assert.AreEqual(5, d);

        Exception exception = null;
        try
        {
            Matrix.Array.GetDeterminant(new double[5, 7]);
        }
        // ReSharper disable once CatchAllClause
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception as ArgumentException);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetDeterminant_ArgumentNullException_Test() => Matrix.Array.GetDeterminant(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetDeterminant_RectangularMatrix_ArgumentException_Test() => Matrix.Array.GetDeterminant(new double[3, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Inverse_ArgumentNullException_Test() => Matrix.Array.Inverse(null, out _);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Inverse_ArgumentException_Test() => Matrix.Array.Inverse(new double[5, 7], out _);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Inverse_ArgumentOutOfRangeException_Test1() => Matrix.Array.Inverse(new double[0, 5], out _);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Inverse_ArgumentOutOfRangeException_Test2() => Matrix.Array.Inverse(new double[5, 0], out _);
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void Inverse_InvalidOperationException_Test() => Matrix.Array.Inverse(new[,] { { 1d, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, out _);

    /// <summary>Тест вычисления обратной матрицы</summary>
    [TestMethod, Priority(1), Description("Тест вычисления обратной матрицы")]
    public void Inverse_Test()
    {
        double[,] a =
        {
            { 7, 8, 9 },
            { 6, 7, 8 },
            { 2, 0, 3 }
        };

        var inv = Matrix.Array.Inverse(a, out var p);

        var i0 = GetUnitaryArrayMatrix(3);
        var i = Matrix.Array.Operator.Multiply(a, inv);
        CollectionAssert.AreEqual(i0, i, MatrixTest.GetComparer(1e-14));
        CollectionAssert.AreEqual(GetUnitaryArrayMatrix(3), p);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Inverse_MatrixResult_Matrix_ArgumentNullException_Test() => Matrix.Array.Inverse(null, new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Inverse_MatrixResult_Result_ArgumentNullException_Test() => Matrix.Array.Inverse(new double[5, 5], null);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Inverse_MatrixResult_Matrix_ArgumentException_Test() => Matrix.Array.Inverse(new double[5, 7], new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Inverse_MatrixResult_Result_ArgumentException_Test1() => Matrix.Array.Inverse(new double[5, 5], new double[5, 7]);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Inverse_MatrixResult_Result_ArgumentException_Test2() => Matrix.Array.Inverse(new double[5, 5], new double[7, 5]);
    [TestMethod, ExpectedException(typeof(InvalidOperationException))]
    public void Inverse_MatrixResult_SingularMatrix_InvalidOperationException_Test() => Matrix.Array.Inverse(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, new double[3, 3]);
    [TestMethod]
    public void Inverse_MatrixResult_Test()
    {
        double[,] a =
        {
            { 7, 8, 9 },
            { 6, 7, 8 },
            { 2, 0, 3 }
        };
        var inv = new double[3, 3];

        Matrix.Array.Inverse(a, inv);
        var i = Matrix.Array.Operator.Multiply(a, inv);

        var i0 = GetUnitaryArrayMatrix(3);
        CollectionAssert.AreEqual(i0, i, MatrixTest.GetComparer(1e-14));
    }

    /// <summary>Тест вычисления обратной матрицы</summary>
    [TestMethod, Priority(1), Description("Тест вычисления обратной матрицы")]
    public void Inverse_WithoutPermutations_Test()
    {
        double[,] a =
        {
            { 0, 8, 9 },
            { 6, 7, 8 },
            { 2, 0, 3 }
        };
        double[,] p0 =
        {
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 0, 0, 1 }
        };

        var b = Matrix.Array.Inverse(a);

        var i = Matrix.Array.Operator.Multiply(b, a);

        CollectionAssert.AreEqual(p0, i, MatrixTest.GetComparer(2.2205e-16));
    }

    /// <summary>Тест LU-разложения матрицы 2x2</summary>
    [TestMethod, Priority(1), Description("Тест LU-разложения матрицы 2x2")]
    public void LU2_Decomposition_Test()
    {
        double[,] a = { { 4, 3 }, { 6, 3 } };
        double[,] L = { { 1, 0 }, { 1.5, 1 } };
        double[,] U = { { 4, 3 }, { 0, -1.5 } };


        var result = Matrix.Array.GetLUDecomposition(a, out var l, out var u, out var d);
        Assert.IsTrue(result);
        Assert.AreEqual(4 * 3 - 3 * 6, d);
        CollectionAssert.AreEqual(l, L);
        CollectionAssert.AreEqual(u, U);
    }

    /// <summary>Тест LU-разложения матрицы 2x2</summary>
    [TestMethod, Priority(1), Description("Тест LU-разложения матрицы 2x2")]
    public void LU2_C_Decomposition_Test()
    {
        double[,] a = { { 4, 3 }, { 6, 3 } };
        double[,] C = { { 4, 3 }, { 1.5, -1.5 } };


        var result = Matrix.Array.GetLUDecomposition(a, out var c);
        Assert.IsTrue(result);
        CollectionAssert.AreEqual(C, c);
    }

    /// <summary>Тест LU-разложения матрицы 3x3</summary>
    [TestMethod, Priority(1), Description("Тест LU-разложения матрицы 3x3")]
    public void LU3_Decomposition_Test()
    {
        double[,] a = { { 5, 3, 2 }, { 1, 2, 0 }, { 3, 0, 4 } };
        double[,] L = { { 1, 0, 0 }, { 0.2, 1, 0 }, { 0.6, -1.28571428571429, 1 } };
        double[,] U = { { 5, 3, 2 }, { 0, 1.4, -0.4 }, { 0, 0, 2.28571428571429 } };

        var result = Matrix.Array.GetLUDecomposition(a, out var l, out var u, out var d);
        Assert.IsTrue(result);
        Assert.AreEqual(5 * (2 * 4 - 0 * 0) - 3 * (1 * 4 - 0 * 3) + 2 * (1 * 0 - 2 * 3), d);
        var eps = MatrixTest.GetComparer(1e-14);
        CollectionAssert.AreEqual(L, l, eps);
        CollectionAssert.AreEqual(U, u, eps);
    }

    /// <summary>Тест LU-разложения матрицы 3x3</summary>
    [TestMethod, Priority(1), Description("Тест LU-разложения матрицы 3x3")]
    public void LU3_C_Decomposition_Test()
    {
        double[,] a = { { 5, 3, 2 }, { 1, 2, 0 }, { 3, 0, 4 } };
        double[,] C = { { 5, 3, 2 }, { 0.2, 1.4, -0.4 }, { 0.6, -1.28571428571429, 2.28571428571429 } };

        var result = Matrix.Array.GetLUDecomposition(a, out var c);
        Assert.IsTrue(result);
        CollectionAssert.AreEqual(C, c, MatrixTest.GetComparer());
    }

    /// <summary>Тест QR-разложения матрицы</summary>
    [TestMethod, Priority(1), Description("Тест QR-разложения матрицы")]
    public void QR_Decomposition_Test()
    {
        double[,] a =
        {
            { 12, -51, 4 },
            { 6, 167, -68 },
            { -4, 24, -41 }
        };

        double[,] q0 =
        {
            { 6d/7, -69d/175, -58d/175 },
            { 3d/7, 158d/175, 6d/175 },
            { -2d/7, 6d/35, -33d/35 }
        };

        double[,] r0 =
        {
            { 14, 21, -14 },
            { 0, 175, -70 },
            { 0, 0, 35 }
        };

        Matrix.Array.QRDecomposition(a, out var q, out var r);

        var a0 = Matrix.Array.Operator.Multiply(q0, r0);
        var a1 = Matrix.Array.Operator.Multiply(q, r);

        var eps = MatrixTest.GetComparer(1.5e-14);
        CollectionAssert.AreEqual(a, a0, eps);
        CollectionAssert.AreEqual(a0, a1, eps);
        CollectionAssert.AreEqual(a, a1, eps);
        CollectionAssert.AreEqual(q0, q, eps);
        CollectionAssert.AreEqual(r0, r, eps);
    }

    /// <summary>Тест SVD-разложения матрицы</summary>
    [TestMethod, Priority(1), Description("Тест SVD-разложения матрицы")]
    public void SVD_Decomposition_Test()
    {
        static void Check(double[,] M, double[,] U0, double[] W0, double[,] V0, double eps = double.Epsilon)
        {
            Matrix.Array.SVD(M, out var U, out var W, out var V);

            var cmp = MatrixTest.GetComparer(eps);

            Assert.AreEqual(U0.GetLength(0), U.GetLength(0));
            Assert.AreEqual(U0.GetLength(1), U.GetLength(1));
            try
            {
                CollectionAssert.AreEqual(U0, U, cmp, "U0 - U = {0}", Matrix.Array.Operator.Subtract(U0, U).ToStringFormatView("g", ", ", CultureInfo.InvariantCulture));
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException($"Разница в элементах матрицы (U0 - U) составила {Matrix.Array.Operator.Subtract(U0, U).EnumerateElementsByRows().Select(Math.Abs).Max()}", e);
            }

            Assert.AreEqual(W0.Length, W.Length);
            try
            {
                CollectionAssert.AreEqual(W0, W, cmp, W0.Zip(W, (x, y) => (x - y).ToString(CultureInfo.InvariantCulture)).JoinStrings(", "));
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException($"Разница в элементах столбца собственных чисел (W0 - W) составила {W0.Zip(W, (x, y) => (x - y).Abs()).Max()}", e);

            }

            Assert.AreEqual(V0.GetLength(0), V.GetLength(0));
            Assert.AreEqual(V0.GetLength(1), V.GetLength(1));
            try
            {
                CollectionAssert.AreEqual(V0, V, cmp, "V0 - V = {0}", Matrix.Array.Operator.Subtract(V0, V).ToStringFormatView("g", ", ", CultureInfo.InvariantCulture));

            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException($"Разница в элементах матрицы (V0 - V) составила {Matrix.Array.Operator.Subtract(V0, V).EnumerateElementsByRows().Select(Math.Abs).Max()}", e);
            }

            var M1 = Matrix.Array.Operator.Multiply(U, Matrix.Array.CreateDiagonal(W));
            M1 = Matrix.Array.Operator.Multiply(M1, Matrix.Array.Transpose(V));

            try
            {
                CollectionAssert.AreEqual(M, M1, cmp, "M - M1 = {0}", Matrix.Array.Operator.Subtract(M, M1).ToStringFormatView("g", ", ", CultureInfo.InvariantCulture));
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException($"Разница в элементах матрицы (M - M1) составила {Matrix.Array.Operator.Subtract(M, M1).EnumerateElementsByRows().Select(Math.Abs).Max()}", e);
            }
        }

        double[,] m =
        {
            { 1, 2, 4 },
            { 2, 9, 8 },
            { 4, 8, 2 }
        };

        var u0 = new[,]
        {
            { -0.265466507027381, 0.442200214734329, 0.856730123046689 },
            { -0.788756632776462, 0.411395689780541, -0.456745619224893 },
            { -0.554428090826772, -0.797002231188161, 0.239576575614717 }
        };

        var w0 = new[] { 15.2964386406325, 4.3487480701758, 1.05230942954324 };

        var v0 = new[,]
        {
            { -0.265466507027382, -0.442200214734329, 0.856730123046689 },
            { -0.788756632776462, -0.411395689780541, -0.456745619224893 },
            { -0.554428090826772, 0.797002231188161, 0.239576575614717 }
        };

        Check(m, u0, w0, v0, 4.8e-14);
        Check(Matrix.Array.Transpose(m), u0, w0, v0, 4.8e-14);

        m = new double[,]
        {
            { 1, 2, 4 },
            { 2, 9, 8 },
            { 4, 8, 2 },
            { 0, 1, 5 }
        };

        u0 = new[,]
        {
            { -0.265542354957568, 0.312848988366203, 0.596853835835679 },
            { -0.771978637895685, 0.182449721133546, -0.593959321163358 },
            { -0.525340452429551, -0.704802504430481, 0.412259238451132 },
            { -0.239903416810653, 0.609992655271404, 0.347879496037195 }
        };

        w0 = new[] { 15.7303317555405, 5.58045576061558, 1.18961185453396 };

        v0 = new[,]
        {
            { -0.248619132847575, -0.383743134781594, 0.889342303778537 },
            { -0.757868281005861, -0.494705456267893, -0.425325969327052 },
            { -0.603178410942683, 0.779748496662078, 0.16783350835244 }
        };

        Check(m, u0, w0, v0, 4.62e-14);
        Check(Matrix.Array.Transpose(m), v0, w0, u0, 4.62e-14);

        m = new double[,]
        {
            { 1, 2, 4, 1 },
            { 2, 9, 8, 2 },
            { 4, 8, 2, 3 },
            { 5, 6, 7, 4 }
        };

        u0 = new[,]
        {
            { -0.225398796369434, -0.409736168851733, 0.111505259267744, -0.876857018952091 },
            { -0.634490655956711, -0.343318127078299, -0.649222859742303, 0.24096462294515 },
            { -0.46568383498979, 0.834079910520865, -0.0904542565066422, -0.281544483473819 },
            { -0.574243463983732, -0.136233763875403, 0.746923210928333, 0.306252383874501 }
        };

        w0 = new[] { 19.0497217931626, 4.52034500612088, 3.26714933515809, 0.0177721838955954 };

        v0 = new[,]
        {
            { -0.326951376812032, 0.34483615688347, 0.669041523408804, 0.571466763741674 },
            { -0.699860349926749, 0.43047626162858, -0.569943340388595, 0.00708995919464687 },
            { -0.573687766659127, -0.81209868121762, 0.0917595407689226, 0.0543899349893172 },
            { -0.272361220049387, 0.190457200033821, 0.468111327975933, -0.818790086279197 }
        };

        Check(m, u0, w0, v0, 1.422e-14);

        m = new double[,]
        {
            { 1, 2, 4, 1 },
            { 2, 9, 8, 2 },
            { 4, 8, 2, 3 },
            { 2, 4, 8, 2 }
        };

        u0 = new[,]
        {
            { -0.246116465481026, -0.27138430161176, -0.256470751271611, -0.894427190999916 },
            { -0.68634880624888, -0.0623324475356906, 0.724596427085415, -8.77688032196619e-19 },
            { -0.475456352630157, 0.792380887059814, -0.382196005429061, -1.71452648676632e-17 },
            { -0.492232930962052, -0.54276860322352, -0.512941502543221, 0.447213595499958 }
        };

        w0 = new[] { 17.9018858414946, 5.64711417858407, 1.90593409438226, 0 };

        v0 = new[,]
        {
            { -0.251655350185575, 0.298902606132943, -0.714581332018668, 0.580258853185659 },
            { -0.69500804788132, 0.542615565337834, 0.471733146634089, -1.05438237757742e-16 },
            { -0.634795270481017, -0.768814601777661, -0.0509123688447716, 0.058025885318566 },
            { -0.225096340881198, 0.158586532825126, -0.514051834930818, -0.812362394459923 }
        };

        Check(m, u0, w0, v0, 2.132e-14);
    }

    [TestMethod]
    public void ToColsArray_Test()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };

        var cols = Matrix.Array.MatrixToColsArray(m);

        Assert.AreEqual(3, cols.Length);
        Assert.AreEqual(3, cols[0].Length);
        Assert.AreEqual(3, cols[1].Length);
        Assert.AreEqual(3, cols[2].Length);
        CollectionAssert.AreEqual(new[] { 1d, 4, 7 }, cols[0]);
        CollectionAssert.AreEqual(new[] { 2d, 5, 8 }, cols[1]);
        CollectionAssert.AreEqual(new[] { 3d, 6, 9 }, cols[2]);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ToColsArray_ArgumentNullException_Test() => Matrix.Array.MatrixToColsArray(null);

    [TestMethod]
    public void ToRowsArray_Test()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };

        var cols = Matrix.Array.MatrixToRowsArray(m);

        Assert.AreEqual(3, cols.Length);
        Assert.AreEqual(3, cols[0].Length);
        Assert.AreEqual(3, cols[1].Length);
        Assert.AreEqual(3, cols[2].Length);
        CollectionAssert.AreEqual(new[] { 1d, 2, 3 }, cols[0]);
        CollectionAssert.AreEqual(new[] { 4d, 5, 6 }, cols[1]);
        CollectionAssert.AreEqual(new[] { 7d, 8, 9 }, cols[2]);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ToRowsArray_ArgumentNullException_Test() => Matrix.Array.MatrixToRowsArray(null);

    [TestMethod]
    public void ColsToMatrix_Test()
    {
        double[][] cols =
        {
            new [] { 1d,4,7 },
            new [] { 2d,5,8 },
            new [] { 3d,6,9 }
        };

        var m = Matrix.Array.ColsArrayToMatrix(cols);

        CollectionAssert.AreEqual(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, m);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void ColsToMatrix_ArgumentNullException_Test() => Matrix.Array.ColsArrayToMatrix(null);

    [TestMethod]
    public void RowsToMatrix_Test()
    {
        double[][] cols =
        {
            new [] { 1d,2,3 },
            new [] { 4d,5,6 },
            new [] { 7d,8,9 }
        };

        var m = Matrix.Array.RowsArrayToMatrix(cols);

        CollectionAssert.AreEqual(new double[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } }, m);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void RowsToMatrix_ArgumentNullException_Test() => Matrix.Array.RowsArrayToMatrix(null);

    [TestMethod]
    public void IsMatrixSingular_Test()
    {
        double[,] singular =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };
        double[,] not_singular =
        {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 0}
        };

        var is_singular_matrix_singular = Matrix.Array.IsMatrixSingular(singular);
        var is_not_singular_matrix_singular = Matrix.Array.IsMatrixSingular(not_singular);

        Assert.IsTrue(is_singular_matrix_singular);
        Assert.IsFalse(is_not_singular_matrix_singular);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void IsMatrixSingular_ArgumentNullException_Test() => Matrix.Array.IsMatrixSingular(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void IsMatrixSingular_EmptyMatrix_ArgumentException_Test() => Matrix.Array.IsMatrixSingular(new double[0, 0]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void IsMatrixSingular_RectangularMatrix_ArgumentException_Test() => Matrix.Array.IsMatrixSingular(new double[3, 5]);

    [TestMethod]
    public void Rank_Test()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };

        var n = Matrix.Array.Rank(m);

        Assert.AreEqual(2, n);

        m = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        n = Matrix.Array.Rank(m);

        Assert.AreEqual(3, n);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    // ReSharper disable once AssignNullToNotNullAttribute                                                            B
    public void Rank_ArgumentNullException_Test() => Matrix.Array.Rank(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Rank_ArgumentException_Test() => Matrix.Array.Rank(new double[0, 0]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Rank_ArgumentException_Test1() => Matrix.Array.Rank(new double[0, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Rank_ArgumentException_Test2() => Matrix.Array.Rank(new double[5, 0]);

    [TestMethod]
    public void CreateDiagonal_Test()
    {
        double[] shadow = { 1, 2, 3 };

        var m = Matrix.Array.CreateDiagonal(shadow);

        Assert.IsNotNull(m);
        Assert.AreEqual(3, m.GetLength(0));
        Assert.AreEqual(3, m.GetLength(1));
        CollectionAssert.AreEqual(new[,] { { shadow[0], 0, 0 }, { 0, shadow[1], 0 }, { 0, 0, shadow[2] } }, m);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void CreateDiagonal_ArgumentNullException_Test() => Matrix.Array.CreateDiagonal(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void CreateDiagonal_ArgumentException_Test() => Matrix.Array.CreateDiagonal(Array.Empty<double>());

    [TestMethod]
    public void GetMatrixShadow_Test()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };

        var shadow = Matrix.Array.GetMatrixShadow(m);

        CollectionAssert.AreEqual(new[] { 1d, 5, 9 }, shadow);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetMatrixShadow_ArgumentNullException_Test() => Matrix.Array.GetMatrixShadow(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetMatrixShadow_ArgumentException_Test1() => Matrix.Array.GetMatrixShadow(new double[5, 0]);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetMatrixShadow_ArgumentException_Test2() => Matrix.Array.GetMatrixShadow(new double[0, 5]);

    [TestMethod]
    public void EnumerateMatrixShadow_Test()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };

        var shadow = Matrix.Array.EnumerateMatrixShadow(m);

        CollectionAssert.AreEqual(new[] { 1d, 5, 9 }, shadow.ToArray());
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void EnumerateMatrixShadow_ArgumentNullException_Test() => Matrix.Array.EnumerateMatrixShadow(null).ToArray();

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void EnumerateMatrixShadow_ArgumentException_Test1() => Matrix.Array.EnumerateMatrixShadow(new double[5, 0]).ToArray();

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void EnumerateMatrixShadow_ArgumentException_Test2() => Matrix.Array.EnumerateMatrixShadow(new double[0, 5]).ToArray();

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void EnumerateMatrixShadow_ArgumentException_Test3() => Matrix.Array.EnumerateMatrixShadow(new double[0, 0]).ToArray();

    [TestMethod]
    public void CreateColArray_Test()
    {
        double[] col_items = { 1, 2, 3 };

        var col = Matrix.Array.CreateColArray(col_items);

        CollectionAssert.AreEqual(new double[,] { { 1 }, { 2 }, { 3 } }, col);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void CreateColArray_ArgumentNullException_Test() => Matrix.Array.CreateColArray(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    // ReSharper disable once RedundantExplicitParamsArrayCreation
    public void CreateColArray_ArgumentException_Test() => Matrix.Array.CreateColArray(Array.Empty<double>());

    [TestMethod]
    public void CreateRowArray_Test()
    {
        double[] col_items = { 1, 2, 3 };

        var row = Matrix.Array.CreateRowArray(col_items);

        CollectionAssert.AreEqual(new double[,] { { 1, 2, 3 } }, row);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void CreateRowArray_ArgumentNullException_Test() => Matrix.Array.CreateRowArray(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    // ReSharper disable once RedundantExplicitParamsArrayCreation
    public void CreateRowArray_ArgumentException_Test() => Matrix.Array.CreateRowArray(Array.Empty<double>());

    [TestMethod]
    public void GetUnitaryArrayMatrix_Test()
    {
        var i = Matrix.Array.GetUnitaryArrayMatrix(3);

        CollectionAssert.AreEqual(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }, i);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetUnitaryArrayMatrix_ArgumentOutOfRangeException_Test() => Matrix.Array.GetUnitaryArrayMatrix(0);

    [TestMethod]
    public void InitializeUnitaryArrayMatrix_Test()
    {
        var i = new double[3, 3];

        Matrix.Array.InitializeUnitaryMatrix(i);

        CollectionAssert.AreEqual(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }, i);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void InitializeUnitaryArrayMatrix_ArgumentNullException_Test() => Matrix.Array.InitializeUnitaryMatrix(null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void InitializeUnitaryArrayMatrix_ArgumentException_Test() => Matrix.Array.InitializeUnitaryMatrix(new double[5, 7]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetTransvection_ArgumentNullException_Test() => Matrix.Array.GetTransvection(null, 0);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetTransvection_ArgumentException_Test1() => Matrix.Array.GetTransvection(new double[5, 7], 0);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetTransvection_ArgumentException_Test2() => Matrix.Array.GetTransvection(new double[5, 5], -1);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetTransvection_ArgumentException_Test3() => Matrix.Array.GetTransvection(new double[5, 5], 5);

    [TestMethod]
    public void GetTransvection_Test()
    {
        double[,] m =
        {
            { 1,  2,  3,  4,  5 },
            { 6,  8,  7,  9,  10 },
            { 11, 12, 13, 14, 15 },
            { 16, 18, 17, 19, 20 },
            { 21, 22, 23, 24, 25 }
        };

        double[,] t0 =
        {
            {1, -2/8d,  0, 0, 0},
            {0,  1/8d, 0, 0, 0},
            {0, -12/8d,   1, 0, 0},
            {0, -18/8d,  0, 1, 0},
            {0, -22/8d,  0, 0, 1}
        };

        var t = Matrix.Array.GetTransvection(m, 1);

        CollectionAssert.AreEqual(t0, t);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Transvection_ResultArgumentNullException_Test() => Matrix.Array.Transvection(new double[5, 5], 0, null);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Transvection_MatrixArgumentNullException_Test() => Matrix.Array.Transvection(null, 0, new double[5, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Transvection_MatrixNonRectangularArgumentException_Test() => Matrix.Array.Transvection(new double[5, 7], 0, new double[5, 7]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Transvection_ResultNonRectangularArgumentException_Test1() => Matrix.Array.Transvection(new double[5, 5], 0, new double[5, 7]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Transvection_ResultNonRectangularArgumentException_Test2() => Matrix.Array.Transvection(new double[5, 5], 0, new double[7, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Transvection_ArgumentException_Test1() => Matrix.Array.Transvection(new double[5, 5], -1, new double[5, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Transvection_ArgumentException_Test2() => Matrix.Array.Transvection(new double[5, 5], 5, new double[5, 5]);

    [TestMethod]
    public void Transvection_Test()
    {
        double[,] m =
        {
            { 1,  2,  3,  4,  5 },
            { 6,  8,  7,  9,  10 },
            { 11, 12, 13, 14, 15 },
            { 16, 18, 17, 19, 20 },
            { 21, 22, 23, 24, 25 }
        };

        double[,] t0 =
        {
            {1, -2/8d,  0, 0, 0},
            {0,  1/8d, 0, 0, 0},
            {0, -12/8d,   1, 0, 0},
            {0, -18/8d,  0, 1, 0},
            {0, -22/8d,  0, 0, 1}
        };

        var t = new double[m.GetLength(0), m.GetLength(1)];
        Matrix.Array.Transvection(m, 1, t);

        CollectionAssert.AreEqual(t0, t);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetCol_ArgumentNullException_Test() => Matrix.Array.GetCol(null, 0);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_ArgumentOutOfRangeException_Test1() => Matrix.Array.GetCol(new double[5, 5], -1);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_ArgumentOutOfRangeException_Test2() => Matrix.Array.GetCol(new double[5, 5], 5);
    [TestMethod]
    public void GetCol_Test()
    {
        double[,] m = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        var col = Matrix.Array.GetCol(m, 1);
        CollectionAssert.AreEqual(new[,] { { 2d }, { 5 }, { 8 } }, col);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetCol_Array_ArgumentNullException_Test1() => Matrix.Array.GetCol_Array(null, 0);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetCol_Array_ArgumentNullException_Test2() => Matrix.Array.GetCol_Array(null, 0, new double[3]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetCol_Array_ArgumentNullException_Test3() => Matrix.Array.GetCol_Array(new double[3, 3], 0, null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetCol_Array_ArgumentException_Test1() => Matrix.Array.GetCol_Array(new double[3, 3], 1, new double[2]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetCol_Array_ArgumentException_Test2() => Matrix.Array.GetCol_Array(new double[3, 3], 1, new double[5]);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_Array_Argument_A_OutOfRangeException_Test11() => Matrix.Array.GetCol_Array(new double[3, 3], -1);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_Array_Argument_A_OutOfRangeException_Test12() => Matrix.Array.GetCol_Array(new double[3, 3], 3);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_Array_Argument_A_OutOfRangeException_Test21() => Matrix.Array.GetCol_Array(new double[3, 3], -1, new double[3]);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetCol_Array_Argument_A_OutOfRangeException_Test22() => Matrix.Array.GetCol_Array(new double[3, 3], 3, new double[3]);

    [TestMethod]
    public void GetCol_Array_Test1()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };
        var col = Matrix.Array.GetCol_Array(m, 1);
        CollectionAssert.AreEqual(new[] { 2d, 5, 8 }, col);
    }

    [TestMethod]
    public void GetCol_Array_Test2()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };
        var col = new double[3];
        Matrix.Array.GetCol_Array(m, 1, col);
        CollectionAssert.AreEqual(new[] { 2d, 5, 8 }, col);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetRow_ArgumentNullException_Test() => Matrix.Array.GetRow(null, 0);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_ArgumentOutOfRangeException_Test1() => Matrix.Array.GetRow(new double[5, 5], -1);
    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_ArgumentOutOfRangeException_Test2() => Matrix.Array.GetRow(new double[5, 5], 5);
    [TestMethod]
    public void GetRow_Test()
    {
        double[,] m = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        var row = Matrix.Array.GetRow(m, 1);
        CollectionAssert.AreEqual(new[,] { { 4d, 5, 6 } }, row);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetRow_Array_ArgumentNullException_Test1() => Matrix.Array.GetRow_Array(null, 0);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetRow_Array_ArgumentNullException_Test2() => Matrix.Array.GetRow_Array(null, 0, new double[3]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetRow_Array_ArgumentNullException_Test3() => Matrix.Array.GetRow_Array(new double[3, 3], 0, null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetRow_Array_ArgumentException_Test1() => Matrix.Array.GetRow_Array(new double[3, 3], 1, new double[2]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void GetRow_Array_ArgumentException_Test2() => Matrix.Array.GetRow_Array(new double[3, 3], 1, new double[5]);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_Array_Argument_A_OutOfRangeException_Test11() => Matrix.Array.GetRow_Array(new double[3, 3], -1);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_Array_Argument_A_OutOfRangeException_Test12() => Matrix.Array.GetRow_Array(new double[3, 3], -3);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_Array_Argument_A_OutOfRangeException_Test21() => Matrix.Array.GetRow_Array(new double[3, 3], -1, new double[3]);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetRow_Array_Argument_A_OutOfRangeException_Test22() => Matrix.Array.GetRow_Array(new double[3, 3], -3, new double[3]);

    [TestMethod]
    public void GetRow_Array_Test1()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };
        var row = Matrix.Array.GetRow_Array(m, 1);
        CollectionAssert.AreEqual(new[] { 4d, 5, 6 }, row);
    }

    [TestMethod]
    public void GetRow_Array_Test2()
    {
        double[,] m =
        {
            { 1,2,3 },
            { 4,5,6 },
            { 7,8,9 }
        };
        var row = new double[3];
        Matrix.Array.GetRow_Array(m, 1, row);
        CollectionAssert.AreEqual(new[] { 4d, 5, 6 }, row);
    }

    /// <summary>Тест метода перестановки строк</summary>
    [TestMethod, Priority(0), Description("Тест метода перестановки строк")]
    public void Permutation_Left_Test()
    {
        double[,] m =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        double[,] p =
        {
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 0, 0, 1 },
        };

        double[,] m0 =
        {
            { 4, 5, 6 },
            { 1, 2, 3 },
            { 7, 8, 9 }
        };

        Matrix.Array.Permutation_Left(m, p);
        CollectionAssert.AreEqual(m0, m);

        m = new double[,] { { 1 } };

        p = new double[,] { { 1 } };

        Matrix.Array.Permutation_Left(m, p);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Permutation_Left_ArgumentNullException_Test1() => Matrix.Array.Permutation_Left(null, new double[5, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Permutation_Left_ArgumentNullException_Test2() => Matrix.Array.Permutation_Left(new double[5, 5], null);

    /// <summary>Матрица перестановок не квадратная</summary>
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Permutation_Left_ArgumentException_Test1() => Matrix.Array.Permutation_Left(new double[5, 5], new double[5, 7]);

    /// <summary>Число строк матрицы не равно числу столбцов матрицы перестановок</summary>
    [TestMethod, ExpectedException(typeof(ArgumentException)), Description("Число строк матрицы не равно числу столбцов матрицы перестановок")]
    public void Permutation_Left_ArgumentException_Test2() => Matrix.Array.Permutation_Left(new double[5, 10], new double[7, 7]);

    [TestMethod, ExpectedException(typeof(InvalidOperationException)), Description("Несимметричная матрица перестановок")]
    public void Permutation_Left_InvalidOperationException_Test() => Matrix.Array.Permutation_Left(new double[3, 3], new double[,]
    {
        { 0, 2, 0 },
        { 1, 0, 0 },
        { 0, 0, 1 },
    });

    /// <summary>Тест метода перестановки столбцов</summary>
    [TestMethod, Priority(0), Description("Тест метода перестановки столбцов")]
    public void Permutation_Right_Test()
    {
        double[,] m =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        double[,] p =
        {
            { 0, 1, 0 },
            { 1, 0, 0 },
            { 0, 0, 1 },
        };

        double[,] m0 =
        {
            { 2, 1, 3 },
            { 5, 4, 6 },
            { 8, 7, 9 }
        };

        Matrix.Array.Permutation_Right(m, p);

        CollectionAssert.AreEqual(m0, m);

        m = new double[,] { { 1 } };

        p = new double[,] { { 1 } };

        Matrix.Array.Permutation_Right(m, p);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Permutation_Right_ArgumentNullException_Test1() => Matrix.Array.Permutation_Right(null, new double[5, 5]);

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Permutation_Right_ArgumentNullException_Test2() => Matrix.Array.Permutation_Right(new double[5, 5], null);

    /// <summary>Матрица перестановок не квадратная</summary>
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Permutation_Right_ArgumentException_Test1() => Matrix.Array.Permutation_Right(new double[5, 5], new double[5, 7]);

    /// <summary>Число строк матрицы не равно числу столбцов матрицы перестановок</summary>
    [TestMethod, ExpectedException(typeof(ArgumentException)), Description("Число строк матрицы не равно числу столбцов матрицы перестановок")]
    public void Permutation_Right_ArgumentException_Test2() => Matrix.Array.Permutation_Right(new double[5, 10], new double[7, 7]);

    [TestMethod, ExpectedException(typeof(InvalidOperationException)), Description("Несимметричная матрица перестановок")]
    public void Permutation_Right_InvalidOperationException_Test() => Matrix.Array.Permutation_Right(new double[3, 3], new double[,]
    {
        { 0, 2, 0 },
        { 1, 0, 0 },
        { 0, 0, 1 },
    });

    [TestMethod]
    public void GetLength_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };

        Matrix.Array.GetLength(a, out var N, out var M);

        Assert.AreEqual(a.GetLength(0), N);
        Assert.AreEqual(a.GetLength(1), M);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetLength_ArgumentNullException_Test() => Matrix.Array.GetLength(null, out _, out _);

    [TestMethod]
    public void GetRowsCount_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };

        Matrix.Array.GetRowsCount(a, out var N);

        Assert.AreEqual(a.GetLength(0), N);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetRowsCount_ArgumentNullException_Test() => Matrix.Array.GetRowsCount(null, out _);

    [TestMethod]
    public void GetColsCount_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };

        Matrix.Array.GetColsCount(a, out var M);

        Assert.AreEqual(a.GetLength(1), M);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetColsCount_ArgumentNullException_Test() => Matrix.Array.GetColsCount(null, out _);

    [TestMethod]
    public void Transpose_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };

        double[,] expected =
        {
            { 1, 4, 7, 10 },
            { 2, 5, 8, 11 },
            { 3, 6, 9, 12 }
        };

        var actual = Matrix.Array.Transpose(a);

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Transpose_ArgumentNullException_Test() => Matrix.Array.Transpose(null);

    [TestMethod]
    public void TransposeOut_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };

        double[,] expected =
        {
            { 1, 4, 7, 10 },
            { 2, 5, 8, 11 },
            { 3, 6, 9, 12 }
        };

        var actual = new double[a.GetLength(1), a.GetLength(0)];
        Matrix.Array.Transpose(a, actual);

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TransposeOut_Input_ArgumentNullException_Test()
    {
        double[,] a = null;
        var actual = new double[3, 4];
        Matrix.Array.Transpose(a, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void TransposeOut_Output_ArgumentNullException_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };
        double[,] actual = null;
        Matrix.Array.Transpose(a, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void TransposeOut_Incorrect_N_Output_ArgumentException_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };
        var actual = new double[a.GetLength(1), 1];
        Matrix.Array.Transpose(a, actual);
    }

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void TransposeOut_Incorrect_M_Output_ArgumentException_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 10,11,12 }
        };
        var actual = new double[1, a.GetLength(0)];
        Matrix.Array.Transpose(a, actual);
    }

    [TestMethod]
    public void GetAdjunct_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        var expected = 4 * 8 - 5 * 7;

        var actual = Matrix.Array.GetAdjunct(a, 0, 2);
        Assert.That.Value(actual).IsEqual(expected);

        expected = -(1 * 6 - 3 * 4);
        actual = Matrix.Array.GetAdjunct(a, 2, 1);
        Assert.That.Value(actual).IsEqual(expected);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetAdjunct_ArgumentNullException_Test() => Matrix.Array.GetAdjunct(null, 0, 2);

    [TestMethod]
    public void GetAdjunct_ArgumentOutOfRangeException_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Exception exception = null;
        try
        {
            Matrix.Array.GetAdjunct(a, -1, 2);
        }
        catch (ArgumentOutOfRangeException e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.That.Value(exception).Is<ArgumentOutOfRangeException>();
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ParamName).IsEqual("n");
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ActualValue).IsEqual(-1);

        exception = null;
        try
        {
            Matrix.Array.GetAdjunct(a, 3, 2);
        }
        catch (ArgumentOutOfRangeException e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.That.Value(exception).Is<ArgumentOutOfRangeException>();
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ParamName).IsEqual("n");
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ActualValue).IsEqual(3);

        exception = null;
        try
        {
            Matrix.Array.GetAdjunct(a, 0, -1);
        }
        catch (ArgumentOutOfRangeException e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.That.Value(exception).Is<ArgumentOutOfRangeException>();
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ParamName).IsEqual("m");
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ActualValue).IsEqual(-1);

        exception = null;
        try
        {
            Matrix.Array.GetAdjunct(a, 0, 3);
        }
        catch (ArgumentOutOfRangeException e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.That.Value(exception).Is<ArgumentOutOfRangeException>();
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ParamName).IsEqual("m");
        Assert.That.Value(((ArgumentOutOfRangeException)exception).ActualValue).IsEqual(3);
    }

    [TestMethod]
    public void GetMinor_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        double[,] expected_minor =
        {
            { 4, 5 },
            { 7, 8 }
        };

        var actual_minor = Matrix.Array.GetMinor(a, 0, 2);

        CollectionAssert.AreEqual(expected_minor, actual_minor);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void GetMinor_ArgumentNullException_Test() => Matrix.Array.GetMinor(null, 0, 2);

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetMinor_ArgumentOutOfRangeException_N_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Matrix.Array.GetMinor(a, 3, 2);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetMinor_ArgumentOutOfRangeException_N2_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Matrix.Array.GetMinor(a, -1, 2);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetMinor_ArgumentOutOfRangeException_M_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Matrix.Array.GetMinor(a, 0, 3);
    }

    [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GetMinor_ArgumentOutOfRangeException_M2_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        Matrix.Array.GetMinor(a, 0, -1);
    }

    [TestMethod]
    public void GetMinor_ResultMatrix_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        double[,] expected_minor =
        {
            { 4, 5 },
            { 7, 8 }
        };

        var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
        Matrix.Array.GetMinor(a, 0, 2, actual_minor);

        CollectionAssert.AreEqual(expected_minor, actual_minor);
    }

    [TestMethod]
    public void GetMinor_ResultMatrix_Exceptions_Test()
    {
        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 }
        };

        //double[,] expected_minor =
        //{
        //    { 4, 5 },
        //    { 7, 8 }
        //};

        Exception exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            // ReSharper disable once AssignNullToNotNullAttribute
            Matrix.Array.GetMinor(null, 0, 2, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        Assert.AreEqual("matrix", ((ArgumentNullException)exception).ParamName);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            // ReSharper disable once AssignNullToNotNullAttribute
            Matrix.Array.GetMinor(a, 0, 2, null);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentNullException));
        Assert.AreEqual("result", ((ArgumentNullException)exception).ParamName);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            Matrix.Array.GetMinor(a, -1, 2, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        Assert.AreEqual("n", ((ArgumentOutOfRangeException)exception).ParamName);
        Assert.AreEqual(-1, ((ArgumentOutOfRangeException)exception).ActualValue);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            Matrix.Array.GetMinor(a, 3, 2, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        Assert.AreEqual("n", ((ArgumentOutOfRangeException)exception).ParamName);
        Assert.AreEqual(3, ((ArgumentOutOfRangeException)exception).ActualValue);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            Matrix.Array.GetMinor(a, 0, -1, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        Assert.AreEqual("m", ((ArgumentOutOfRangeException)exception).ParamName);
        Assert.AreEqual(-1, ((ArgumentOutOfRangeException)exception).ActualValue);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            Matrix.Array.GetMinor(a, 0, 3, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentOutOfRangeException));
        Assert.AreEqual("m", ((ArgumentOutOfRangeException)exception).ParamName);
        Assert.AreEqual(3, ((ArgumentOutOfRangeException)exception).ActualValue);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0), a.GetLength(1) - 1];
            Matrix.Array.GetMinor(a, 0, 2, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        Assert.AreEqual("result", ((ArgumentException)exception).ParamName);

        exception = null;
        try
        {
            var actual_minor = new double[a.GetLength(0) - 1, a.GetLength(1)];
            Matrix.Array.GetMinor(a, 0, 2, actual_minor);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception e)
        {
            exception = e;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentException));
        Assert.AreEqual("result", ((ArgumentException)exception).ParamName);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Matrix_ArgumentNullException_x_Test() => Matrix.Array.Operator.BiliniarMultiply(null, new double[5, 5], new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Matrix_ArgumentNullException_a_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[5, 5], null, new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Matrix_ArgumentNullException_y_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[5, 5], new double[5, 5], null);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiply_Matrix_ArgumentException_x_Length_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[4, 6], new double[5, 3], new double[3, 7]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiply_Matrix_ArgumentException_y_Length_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[4, 3], new double[3, 3], new double[2, 7]);

    [TestMethod]
    public void Operator_BiliniarMultiply_Matrix_Test()
    {
        double[,] x = { { 1, 2, 3, 4 } };

        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 0, 1, 2 }
        };

        double[,] y = { { 1 }, { 6 }, { 1 } };

        var b11 = Matrix.Array.Operator.Multiply(x, a);
        var b21 = Matrix.Array.Operator.Multiply(a, y);
        var b1 = Matrix.Array.Operator.Multiply(b11, y);
        var b2 = Matrix.Array.Operator.Multiply(x, b21);
        CollectionAssert.AreEqual(b1, b2);

        var b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);

        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(y.GetLength(1), b.GetLength(1));

        CollectionAssert.AreEqual(b1, b);
        CollectionAssert.AreEqual(b2, b);

        x = new double[,]
        {
            { 1,2,3,4 },
            { 5,6,7,8 },
            { 9,0,1,2 }
        };

        y = new double[,]
        {
            { 1,2,3,4,5},
            { 6,7,8,9,0 },
            { 1,2,3,4,5 }
        };

        double[,] b0 =
        {
            { 320,  440,  560,  680,  400 },
            { 832, 1144, 1456, 1768, 1040 },
            { 224,  308,  392,  476,  280 }
        };

        b11 = Matrix.Array.Operator.Multiply(x, a);
        b21 = Matrix.Array.Operator.Multiply(a, y);
        b1 = Matrix.Array.Operator.Multiply(b11, y);
        b2 = Matrix.Array.Operator.Multiply(x, b21);
        CollectionAssert.AreEqual(b1, b2);
        CollectionAssert.AreEqual(b0, b2);
        CollectionAssert.AreEqual(b0, b2);

        b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);

        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(y.GetLength(1), b.GetLength(1));
        Assert.AreEqual(b0.GetLength(0), b.GetLength(0));
        Assert.AreEqual(b0.GetLength(1), b.GetLength(1));

        CollectionAssert.AreEqual(b1, b);
        CollectionAssert.AreEqual(b2, b);
        CollectionAssert.AreEqual(b0, b);

        x = new double[0, 4];
        a = new double[4, 3];
        y = new double[3, 5];

        b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);
        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(y.GetLength(1), b.GetLength(1));

        x = new double[3, 4];
        a = new double[4, 3];
        y = new double[3, 0];

        b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);
        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(y.GetLength(1), b.GetLength(1));
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Vector_ArgumentNullException_x_Test() => Matrix.Array.Operator.BiliniarMultiply(null, new double[5, 5], new double[5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Vector_ArgumentNullException_a_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[5], null, new double[5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiply_Vector_ArgumentNullException_y_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[5], new double[5, 5], null);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiply_Vector_ArgumentException_x_Length_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[5], new double[5, 3], new double[5]);

    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiply_Vector_ArgumentException_y_Length_Test() => Matrix.Array.Operator.BiliniarMultiply(new double[3], new double[5, 3], new double[7]);
    [TestMethod]
    public void Operator_BiliniarMultiply_Vector_Test()
    {
        double[] x = { 1, 2, 3, 4 };

        double[,] a =
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 },
            { 0, 1, 2 }
        };

        double[] y = { 1, 2, 3 };

        var b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);
        var b0 = x[0] * (a[0, 0] * y[0] + a[0, 1] * y[1] + a[0, 2] * y[2]) +
            x[1] * (a[1, 0] * y[0] + a[1, 1] * y[1] + a[1, 2] * y[2]) +
            x[2] * (a[2, 0] * y[0] + a[2, 1] * y[1] + a[2, 2] * y[2]) +
            x[3] * (a[3, 0] * y[0] + a[3, 1] * y[1] + a[3, 2] * y[2]);

        Assert.AreEqual(b0, b);

        x = Array.Empty<double>();
        a = new double[0, 3];
        b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);
        Assert.IsTrue(double.IsNaN(b));

        x = new double[] { 1, 2, 3, 4 };
        y = Array.Empty<double>();
        a = new double[4, 0];
        b = Matrix.Array.Operator.BiliniarMultiply(x, a, y);
        Assert.IsTrue(double.IsNaN(b));
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiplyAuto_Vector_ArgumentNullException_x_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto((double[])null, new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiplyAuto_Vector_ArgumentNullException_a_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[5], null);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiplyAuto_Vector_ArgumentException_a_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[5], new double[3, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiplyAuto_Vector_ArgumentException_x_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[7], new double[5, 5]);

    [TestMethod]
    public void Operator_BiliniarMultiplyAuto_Vector_Test()
    {
        double[] x = { 1, 2, 3, 4, 5 };

        double[,] a =
        {
            { 1,2,3,4,5 },
            { 6,7,8,9,0 },
            { 1,2,3,4,5 },
            { 6,7,8,9,0 },
            { 1,2,3,4,5 }
        };

        var b0 = 975d;

        var b = Matrix.Array.Operator.BiliniarMultiplyAuto(x, a);

        Assert.AreEqual(b0, b);

        x = Array.Empty<double>();
        a = new double[0, 0];
        b = Matrix.Array.Operator.BiliniarMultiplyAuto(x, a);
        Assert.IsTrue(double.IsNaN(b));
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiplyAuto_ArgumentNullException_x_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto((double[,])null, new double[5, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public void Operator_BiliniarMultiplyAuto_ArgumentNullException_a_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[3, 5], null);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiplyAuto_ArgumentException_a_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[3, 5], new double[3, 5]);
    [TestMethod, ExpectedException(typeof(ArgumentException))]
    public void Operator_BiliniarMultiplyAuto_ArgumentException_x_Test() => Matrix.Array.Operator.BiliniarMultiplyAuto(new double[3, 7], new double[5, 5]);

    [TestMethod]
    public void Operator_BiliniarMultiplyAuto_Test()
    {
        double[,] x =
        {
            { 1,2,3,4,5 },
        };

        double[,] a =
        {
            { 1,2,3,4,5 },
            { 6,7,8,9,0 },
            { 1,2,3,4,5 },
            { 6,7,8,9,0 },
            { 1,2,3,4,5 }
        };

        double[,] b0 =
        {
            { 975 }
        };

        var b = Matrix.Array.Operator.BiliniarMultiplyAuto(x, a);

        Assert.AreEqual(b.GetLength(0), b.GetLength(0));
        Assert.AreEqual(b.GetLength(1), b.GetLength(1));
        Assert.AreEqual(b0.GetLength(0), b.GetLength(0));
        Assert.AreEqual(b0.GetLength(1), b.GetLength(1));
        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(x.GetLength(0), b.GetLength(1));
        CollectionAssert.AreEqual(b0, b);

        x = new double[,]
        {
            { 1,2,3,4,5 },
            { 6,7,8,9,0 },
            { 1,2,3,4,5 }
        };

        b0 = new double[,]
        {
            { 975,  2100,  975 },
            { 2050, 4800, 2050 },
            {  975, 2100,  975 }
        };

        b = Matrix.Array.Operator.BiliniarMultiplyAuto(x, a);

        Assert.AreEqual(b.GetLength(0), b.GetLength(0));
        Assert.AreEqual(b.GetLength(1), b.GetLength(1));
        Assert.AreEqual(b0.GetLength(0), b.GetLength(0));
        Assert.AreEqual(b0.GetLength(1), b.GetLength(1));
        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(x.GetLength(0), b.GetLength(1));
        CollectionAssert.AreEqual(b0, b);

        x = new double[0, 0];
        a = new double[0, 0];
        b = Matrix.Array.Operator.BiliniarMultiplyAuto(x, a);
        Assert.AreEqual(x.GetLength(0), b.GetLength(0));
        Assert.AreEqual(x.GetLength(0), b.GetLength(1));
        CollectionAssert.AreEqual(new double[0, 0], b);
    }

    [TestMethod, SuppressMessage("ReSharper", "InconsistentNaming")]
    public void AXAt_Test()
    {
        double[,] A =
        {
            { 1, 2, 3 },
            { 3, 2, 1 },
        };

        double[,] X =
        {
            { 3, 2, 1 },
            { 4, 5, 6 },
            { 9, 8, 7 },
        };

        double[,] expected_Y0 =
        {
            { 212, 220 },
            { 140, 148 }
        };

        var At = Matrix.Array.Transpose(A);

        var actual_Y = Matrix.Array.Operator.AXAt(A, X);
        var expected_Y = Matrix.Array.Operator.Multiply(A, Matrix.Array.Operator.Multiply(X, At));

        Assert.That.Value(actual_Y)
           .Where(y => y.GetLength(0)).CheckEquals(expected_Y.GetLength(0))
           .Where(y => y.GetLength(1)).CheckEquals(expected_Y.GetLength(1));

        CollectionAssert.That.Collection(actual_Y)
           .IsEqualTo(expected_Y)
           .IsEqualTo(expected_Y0);
    }

    [TestMethod]
    public void XtAY_Test()
    {
        double[] x = { 3, 2, 1 };
        double[] y = { 1, 2, 3, 4, 5 };
        double[,] A =
        {
            { 5, 4, 3, 2, 1 },
            { 1, 2, 3, 4, 5 },
            { 1, 2, 3, 2, 1 }
        };

        //                                [ 1 ]
        //              [ 5, 4, 3, 2, 1 ] [ 2 ]
        // [ 3, 2, 1] * [ 1, 2, 3, 4, 5 ] [ 3 ] = 242
        //              [ 1, 2, 3, 2, 1 ] [ 4 ]
        //                                [ 5 ]

        var result = Matrix.Array.Operator.XtAY(x, A, y);

        result.AssertEquals(242);
    }

    [TestMethod]
    public void MultiplyAtB_Test()
    {
        double[,] A =
        {
            { 1, 2, 3, 4, 5 },
            { 5, 4, 3, 2, 1 },
            { 1, 2, 3, 2, 1 }
        };
        double[,] B =
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 },
        };

        var expected_C = new double[,]
        {
            { 21, 28 },
            { 24, 32 },
            { 27, 36 },
            { 20, 28 },
            { 13, 20 },
        };

        var C = Matrix.Array.Operator.MultiplyAtB(A, B);

        Assert.That.Collection(C).IsEqualTo(expected_C);
    }

    [TestMethod]
    public void MultiplyABt_Test()
    {
        double[,] A =
        {
            { 1, 2, 3, 4, 5 },
            { 5, 4, 3, 2, 1 },
            { 1, 2, 3, 2, 1 }
        };
        double[,] B =
        {
            { 1, 2, 3, 4, 5 },
            { 5, 4, 3, 2, 1 },
        };

        var C = Matrix.Array.Operator.MultiplyABt(A, B);

        Assert.That.Collection(C).IsEqualTo(new double[,]
        {
            { 55, 35 },
            { 35, 55 },
            { 27, 27 },
        });
    }

    [TestMethod]
    public void MultiplyAtb_Test()
    {
        double[,] A =
        {
            { 1, 2, 3, 4, 5 },
            { 5, 4, 3, 2, 1 },
            { 1, 2, 3, 2, 1 }
        };
        double[] x = { 1, 2, 3 };

        var y = Matrix.Array.Operator.MultiplyAtb(A, x);

        y.AssertEquals(14, 16, 18, 14, 10);
    }

    [TestMethod]
    public void MultiplyRowToColMatrix_Test()
    {
        double[] x = { 1, 2, 3 };
        double[] y = { 1, 2, 3, 4, 5 };

        var M = Matrix.Array.Operator.MultiplyRowToColMatrix(x, y);
        Assert.That.Collection(M).IsEqualTo(new double[,]
        {
            { 1, 02, 03 },
            { 2, 04, 06 },
            { 3, 06, 09 },
            { 4, 08, 12 },
            { 5, 10, 15 },
        });
    }
}