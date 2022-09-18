using MathCore.Interpolation;

namespace MathCore.Tests.Interpolation;

[TestClass]
public class NewtonTests
{
    [TestMethod]
    public void GetPolynomCoefficients()
    {
        double[] xx =
        {
            -3/2d,
            -3/4d,
            0,
            3/4d,
            3/2d
        };

        double[] yy =
        {
            -14.1014,
            -0.931596,
            0,
            0.931596,
            14.1014
        };

        double[] expected_coefficients =
        {
            -1.7763568394002505E-15,
            -1.4774737777777762,
            0,
            4.834847604938271,
            0
        };

        var actual_coefficients = Newton.GetPolynomCoefficients(xx, yy);

        Assert.That.Collection(actual_coefficients).IsEqualTo(expected_coefficients);

        // Сравнение с первоисточником в англо.википедии https://en.wikipedia.org/wiki/Newton_polynomial
        double[] expected_wikipedia_coefficients =
        {
            -0.00005,
            -1.4775,
            0.00001,
            4.8348,
            0
        };

        const double accuracy = 5.1e-5;
        Assert.That.Collection(actual_coefficients)
           .IsEqualTo(expected_wikipedia_coefficients, accuracy);
    }

    [TestMethod]
    public void GetPolynomCoefficients2()
    {
        double[] xx = { 1, 2, 3, 4 };
        double[] yy = { 6, 9, 2, 5, };

        double[] d = { 6, 3, -5, 10 / 3d };

        var p = new Polynom(d[0]);

        var p1 = new Polynom(-xx[0], 1);
        var pp1 = d[1] * p1;
        p += pp1;

        var p2 = new Polynom(-xx[1], 1);
        p += d[2] * p1 * p2;

        var p3 = new Polynom(-xx[2], 1);
        p += d[3] * p1 * p2 * p3;

        var expected_coefficients = p.Coefficients;

        var actual_coefficients = Newton.GetPolynomCoefficients(xx, yy);

        var actual_coefficients2 = Lagrange.GetPolynomCoefficients(xx, yy);

        const double accuracy = 7.106e-15;
        Assert.That.Collection(actual_coefficients).IsEqualTo(expected_coefficients, accuracy);
    }
}