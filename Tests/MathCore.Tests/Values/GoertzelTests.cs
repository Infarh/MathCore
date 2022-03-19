using MathCore.Values;

namespace MathCore.Tests.Values;

[TestClass]
public class GoertzelTests
{
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

        Assert.That.Value(actual_y7).IsEqual(expected_y7);
    }
}
