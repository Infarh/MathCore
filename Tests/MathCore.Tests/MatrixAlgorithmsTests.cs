#nullable enable
using System.Diagnostics;

namespace MathCore.Tests;

[TestClass]
public class MatrixAlgorithmsTests
{
    [TestMethod]
    public void TridiagonalAlgorithm()
    {
        // https://en.wikibooks.org/wiki/Algorithm_Implementation/Linear_Algebra/Tridiagonal_matrix_algorithm#C++

        double[] a = [-1, -1, -1];
        double[] b = [+4, +4, +4, +4];
        double[] c = [-1, -1, -1];
        double[] d = [+5, +5, 10, 23];

        // | 4 -1       | |x0 = 2| = |  5 |  4 * 2 -1 * 3        =  8  -3    =   5
        // |-1  4 -1    | |x1 = 3| = |  5 | -1 * 2 +4 * 3 -1 * 5 = -2 +12 -5 =   5
        // |   -1  4 -1 | |x2 = 5| = | 10 | -1 * 3 +4 * 5 -1 * 7 = -3 +20 -7  = 10
        // |      -1  4 | |x3 = 7| = | 23 | -1 * 5 +4 * 7        = -5 +28     = 23


        double[] expected = [+2, +3, +5, +7];

        var actual = Matrix.TridiagonalAlgorithm(a, b, c, d);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void TridiagonalAlgorithm1()
    {
        double[] a = [-1, +2, -1];
        double[] b = [+8, +6, 10, +6];
        double[] c = [-2, -2, -4];
        double[] d = [+6, +3, +8, +5];


        double[] expected = [+1, +1, +1, +1];

        var actual = Matrix.TridiagonalAlgorithm(a, b, c, d);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void CrateZTransformMatrixArray()
    {
        // http://we.easyelectronics.ru/Theory/cifrovye-rekursivnye-filtry-chast-2.html
        //double[,] z_matrix = 
        //{
        //    //           1,     2,    3,    4,   5,   6,   7,   8,   9,  10,  11,  12,   13,   14,    15,   16
        //    //       -----------------------------------------------------------------------------------------
        //    /* 1*/ {     1,     1,    1,    1,   1,   1,   1,   1,   1,   1,   1,   1,    1,    1,     1,    1 },
        //    /* 2*/ {   -15,   -13,  -11,   -9,  -7,  -5,  -3,  -1,   1,   3,   5,   7,    9,   11,    13,   15 },
        //    /* 3*/ {   105,    77,   53,   33,  17,   5,  -3,  -7,  -7,  -3,   5,  17,   33,   53,    77,  105 },
        //    /* 4*/ {  -455,  -273, -143,  -57,  -7,  15,  17,   7,  -7, -17, -15,   7,   57,  143,   273,  455 },
        //    /* 5*/ {  1365,   637,  221,   21, -43, -35,  -3,  21,  21,  -3, -35, -43,   21,  221,   637, 1365 },
        //    /* 6*/ { -3003, -1001, -143,   99,  77,  -1, -39, -21,  21,  39,   1, -77,  -99,  143,  1001, 3003 },
        //    /* 7*/ {  5005,  1001, -143, -187, -11,  65,  25, -35, -35,  25,  65, -11, -187, -143,  1001, 5005 },
        //    /* 8*/ { -6435,  -429,  429,   99, -99, -45,  45,  35, -35, -45,  45,  99,  -99, -429,   429, 6435 },
        //    /* 9*/ {  6435,  -429, -429,   99,  99, -45, -45,  35,  35, -45, -45,  99,   99, -429,  -429, 6435 },
        //    /*10*/ { -5005,  1001,  143, -187,  11,  65, -25, -35,  35,  25, -65, -11,  187, -143, -1001, 5005 },
        //    /*11*/ {  3003, -1001,  143,   99, -77,  -1,  39, -21, -21,  39,  -1, -77,   99,  143, -1001, 3003 },
        //    /*12*/ { -1365,   637, -221,   21,  43, -35,   3,  21, -21,  -3,  35, -43,  -21,  221,  -637, 1365 },
        //    /*13*/ {   455,  -273,  143,  -57,   7,  15, -17,   7,   7, -17,  15,   7,  -57,  143,  -273,  455 },
        //    /*14*/ {  -105,    77,  -53,   33, -17,   5,   3,  -7,   7,  -3,  -5,  17,  -33,   53,   -77,  105 },
        //    /*15*/ {    15,   -13,   11,   -9,   7,  -5,   3,  -1,  -1,   3,  -5,   7,   -9,   11,   -13,   15 },
        //    /*16*/ {    -1,     1,   -1,    1,  -1,   1,  -1,   1,  -1,   1,  -1,   1,   -1,    1,    -1,    1 },
        //};
        static int[,] CreateExpectedMatrix(int MatrixOrder)
        {
            var matrix = new int[MatrixOrder, MatrixOrder];
            matrix[0, 0] = 1;

            for (var j = 1; j < MatrixOrder; j++)
            {
                matrix[0, j] = 1;

                for (var i = 1; i < MatrixOrder; i++)
                    matrix[i, j] = matrix[i, j - 1] + matrix[i - 1, j - 1];

                for (var k = 0; k < j; k++)
                    for (var (i, last) = (1, matrix[0, k]); i < MatrixOrder; i++)
                    {
                        var tmp = matrix[i, k];
                        matrix[i, k] -= last;
                        last = tmp;
                    }
            }

            return matrix;
        }

        const int order = 16;
        var expected_matrix = CreateExpectedMatrix(order);

        var actual_matrix = Matrix.GetZTransformMatrix(order);
        var actual_matrix_array = actual_matrix.GetData();

        try
        {
            for (var i = 0; i < order; i++)
                for (var j = 0; j < order; j++)
                    actual_matrix_array[i, j].AssertEquals(expected_matrix[i, j], $"Ячейка матрицы [{i},{j}] не совпадает с требуемым значением");
        }
        catch (AssertFailedException)
        {
            Debug.WriteLine("Expected matrix");
            Debug.WriteLine(Matrix.Create(expected_matrix).View().ToString());
            Debug.WriteLine("---------------");
            Debug.WriteLine("Actual matrix");
            Debug.WriteLine(actual_matrix.View().ToString());

            throw;
        }
    }
}
