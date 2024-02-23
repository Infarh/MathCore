#nullable enable
namespace MathCore.Vectors;

public class VectorND_double : Vector<double>
{
    public double Length => Math.Sqrt(this.Sum());

    public VectorND_double(int Dimension) : base(Dimension) { }

    public VectorND_double(double[] Elements) : base(Elements) { }

    public VectorND_double(IEnumerable<double> Elements) : base(Elements.ToArray()) { }


    public VectorND_double GetProduction(VectorND_double b)
    {
        if (Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[Dimension].Initialize(this, b, (i, v, bb) => v[i] * bb[i]));
    }

    public double GetScalarProduction(VectorND_double b)
    {
        if (Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return GetProduction(b).Sum();
    }

    public static VectorND_double GetVectorProduction(VectorND_double[] Vectors)
    {
        if (Vectors is null || Vectors.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(Vectors),
                "Векторное произведение не может быть рассчитано для пустого множества векторов");

        var N = Vectors[0].Dimension;
        for (var i = 1; i < Vectors.Length; i++)
            if (Vectors[i].Dimension != N)
                throw new ArgumentOutOfRangeException(nameof(Vectors),
                    $"Размерность вектора {i}:{Vectors[i].Dimension} не соответствует размерности первого вектора {N}");

        if (N - 1 != Vectors.Length)
            throw new ArgumentOutOfRangeException(nameof(Vectors),
                "Количество векторов для векторного произведения не соответствует размерности векторного пространства");


        var M = new Matrix(N, (i, j) => i == 0 ? 0 : Vectors[i - 1][j]);
        var k = -1;
        // ReSharper disable once HeapView.CanAvoidClosure
        return new(new double[N].Initialize(i => M.GetMinor(i, 0).GetDeterminant() * (k *= -1)));
    }

    public VectorND_double GetInversed() => new(new double[Dimension].Initialize(this, (i, v) => 1 / v[i]));

    public static VectorND_double operator +(VectorND_double a, VectorND_double b)
    {
        if (a.Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] + bb[i]));
    }

    public static VectorND_double operator +(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] + bb));

    public static VectorND_double operator +(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa + bb[i]));

    public static VectorND_double operator -(VectorND_double a, VectorND_double b)
    {
        if (a.Dimension != b.Dimension) throw new ArgumentException("Размерности векторов не совпадают");
        return new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] - bb[i]));
    }

    public static VectorND_double operator -(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] - bb));

    public static VectorND_double operator -(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa - bb[i]));

    public static double operator *(VectorND_double a, VectorND_double b) => a.GetScalarProduction(b);

    public static VectorND_double operator *(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] * bb));

    public static VectorND_double operator *(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa * bb[i]));

    public static double operator /(VectorND_double a, VectorND_double b) => a * b.GetInversed();

    public static VectorND_double operator /(VectorND_double a, double b) => new(new double[a.Dimension].Initialize(a, b, (i, aa, bb) => aa[i] / bb));

    public static VectorND_double operator /(double a, VectorND_double b) => new(new double[b.Dimension].Initialize(a, b, (i, aa, bb) => aa / bb[i]));
}