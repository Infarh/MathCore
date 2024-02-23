// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Statistic.RandomNumbers;

public class TriangularRandomGenerator(double A, double B)
    : RandomGenerator(mu: (A + B) / 2, sigma: (B - A) / Math.Sqrt(24))
{
    public double a { get => A; set { if(Math.Abs(value - A) > double.Epsilon) SetAB(value, b); } }
    public double b { get => B; set { if(Math.Abs(value - B) > double.Epsilon) SetAB(a, value); } }

    public void SetAB(double a, double b)
    {
        A    = a;
        B    = b;
        Sigma = (b - a) / Math.Sqrt(24);
        Mu    = (a + b) / 2;
    }

    public override double Distribution(double x) => Distributions.Triangular(x, A, B);

    protected override double GetNextValue() => throw new NotImplementedException();
}