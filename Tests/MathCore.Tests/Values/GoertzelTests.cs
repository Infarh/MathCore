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
}
