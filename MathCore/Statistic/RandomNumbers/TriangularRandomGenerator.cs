
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MathCore.Statistic.RandomNumbers;

public class TriangularRandomGenerator : RandomGenerator
{
    private double _A;
    private double _B;

    public double a { get => _A; set { if(Math.Abs(value - _A) > double.Epsilon) SetAB(value, b); } }
    public double b { get => _B; set { if(Math.Abs(value - _B) > double.Epsilon) SetAB(a, value); } }

    public TriangularRandomGenerator(double a, double b)
        : base(mu: (a + b) / 2, sigma: (b - a) / Math.Sqrt(24))
    {
        _A = a;
        _B = b;
    }

    public void SetAB(double a, double b)
    {
        _A    = a;
        _B    = b;
        Sigma = (b - a) / Math.Sqrt(24);
        Mu    = (a + b) / 2;
    }

    public override double Distribution(double x) => Distributions.Triangular(x, _A, _B);

    protected override double GetNextValue() => throw new NotImplementedException();
}