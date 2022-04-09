using MathCore.Values;

namespace MathCore.Tests.Values;

[TestClass]
public class GoertzelTests : UnitTest
{
    private class ComplexToleranceComparer : IEqualityComparer<Complex>
    {
        private double _Tolerance;

        /// <summary>Точность сравнения</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">Если значение точности меньше нуля</exception>
        public double Tolerance
        {
            get => this._Tolerance;
            set => this._Tolerance = value >= 0.0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Значение точности должно быть больше, либо равно 0");
        }

        public ComplexToleranceComparer(double Tolerance = 1e-14) => _Tolerance = Tolerance;

        public bool Equals(Complex x, Complex y) =>
            Math.Abs(x.Re - y.Re) <= _Tolerance &&
            Math.Abs(x.Im - y.Im) <= _Tolerance;

        public int GetHashCode(Complex obj)
        {
            var (re, im) = obj;
            var z = new Complex(Math.Abs(re / _Tolerance) * _Tolerance, Math.Abs(im / _Tolerance) * _Tolerance);
            return z.GetHashCode();
        }
    }

    [TestMethod]
    public void SpectrumSample()
    {
        double[] s = { 3, 2, 1, -1, 1, -2, -3, -2 };

        var goertzel = new Goertzel(1 / 8d);

        var y = new Complex[s.Length];
        var s1 = new double[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            y[i] = goertzel.Add(s[i]);
            s1[i] = goertzel.State1;
        }

        var expected_y7 = new Complex(4.1213203435596384, -7.5355339059327431);
        var actual_y7 = goertzel.State;

        var comparer = new ComplexToleranceComparer();

        Assert.That.Value(actual_y7).IsEqual(expected_y7, comparer);
    }

    [TestMethod]
    public void Harmonic()
    {
        const double fd = 100;
        const double dt = 1 / fd;
        const double f0 = 10;
        const double T0 = 1 / f0;
        const int periods_count = 100;
        const double Tmax = T0 * periods_count;
        const int samples_count = (int)(Tmax / dt);
        const double f_rel = f0 / fd;

        double[] ff =
        {
            0.0 * f0,
            0.1 * f0,
            0.9 * f0,
            1.0 * f0,
            1.1 * f0,
            0.9 * fd / 2,
        };

        var results = new (double f, double Y)[ff.Length];

        for (var i = 0; i < results.Length; i++)
        {
            var ff0 = ff[i];
            var values = Enumerable.Range(0, samples_count)
               .Select(j => j * dt)
               .Select(t => Math.Sin(Consts.pi2 * ff0 * t));

            var goertzel = new Goertzel(f0 / fd);

            var y = values.ToArray(goertzel.Add);

            var result = y[^1];
            results[i] = (ff[i], result.Abs);
        }
    }
}
