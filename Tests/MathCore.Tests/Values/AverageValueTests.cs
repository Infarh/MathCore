using MathCore.Values;

namespace MathCore.Tests.Values;

[TestClass]
public class AverageValueTests
{
    private static readonly Random __RND = new();

    private static IEnumerable<double> GetValues(int count = 100, double min = 0, double max = 100)
    {
        var d = (Math.Max(max, min) - Math.Min(max, min)) / 2;
        var m = (min + max) / 2;
        for (var i = 0; i < count; i++)
            yield return __RND.NextDouble() * d + m;
    }

    [TestMethod]
    public void Ctor_Length_Test()
    {
        var length = __RND.Next(5, 15);

        var average = new AverageValue(length);

        Assert.AreEqual(length, average.Length);
        Assert.AreEqual(0, average.ValuesCount);
        Assert.AreEqual(0, average.Value);
        Assert.IsTrue(double.IsNaN(average.StartValue));
    }

    [TestMethod]
    public void Ctor_StartValue_Test()
    {
        var start_value = __RND.NextDouble() * 100;

        var average = new AverageValue(start_value);

        Assert.AreEqual(start_value, average.Value);
        Assert.AreEqual(start_value, average.StartValue);
        Assert.AreEqual(1, average.ValuesCount);
        Assert.AreEqual(-1, average.Length);
    }

    [TestMethod]
    public void Ctor_StartValue_Length_Test()
    {
        var start_value = __RND.NextDouble() * 100;
        var length      = __RND.Next(5, 15);

        var average = new AverageValue(start_value, length);

        Assert.AreEqual(start_value, average.Value);
        Assert.AreEqual(start_value, average.StartValue);
        Assert.AreEqual(1, average.ValuesCount);
        Assert.AreEqual(length, average.Length);
    }

    [TestMethod]
    public void AddValue_Test()
    {
        const int count   = 10;
        var       data    = Enumerable.Range(0, count).Select(n => (double)n).ToArray();
        var       average = data.Average();

        var average_value = new AverageValue();

        foreach (var v in data) average_value.AddValue(v);

        Assert.AreEqual(average, average_value.Value);
    }

    [TestMethod]
    public void AddValue_Start0_Test()
    {
        const int count = 10;
        var data = Enumerable
           .Range(0, count)
           .Select(n => (double)n)
           .ToArray();
        var average = data.Average();

        var average_value = new AverageValue(0d);

        foreach (var v in data.Skip(1))
            average_value.AddValue(v);

        Assert.AreEqual(average, average_value.Value);
    }

    [TestMethod]
    public void RollingAverage_Test()
    {
        const int count = 10;
        var       data  = Enumerable.Range(0, count).Select(n => (double)n).ToArray();

        var average_value = new AverageValue();

        var expected = new double[data.Length];
        var actual   = new double[data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            actual[i]   = average_value.AddValue(data[i]);
            expected[i] = data.Take(i + 1).Average();
        }

        CollectionAssert.AreEqual(expected, actual);
    }
}