using System.Collections;
using System.Collections.Frozen;

using MathCore.Values;

namespace MathCore.Tests.Values;

[TestClass]
public class RollingMaxTests
{
    [TestMethod]
    public void MaxTest()
    {
        double[] values = [50, 49, 51, 48, 52, 47, 53, 46, 54, 45, 55, 44, 56, 43, 57, 42, 58, 41, 59, 40, 60];
        var expected_max = values.OrderByDescending(v => v).Take(5).ToArray();

        var max = RollingMax.New(5, values);

        for (var i = 0; i < expected_max.Length; i++)
            max[i].AssertEquals(expected_max[i]);
    }

    [TestMethod]
    public void MaxWithRepeatValues()
    {
        double[] values = [1, 2, 2, 3, 4, 4, 5];
        var expected_max = values.OrderByDescending(v => v).Take(5).ToArray();

        var max = RollingMax.New(5, values);

        for (var i = 0; i < expected_max.Length; i++)
            max[i].AssertEquals(expected_max[i]);
    }

    [TestMethod]
    public void MaxWithRepeatValuesDiff()
    {
        int[] values = [7, 1, 3, 2, 4, 5, 6, 7];
        var expected_max = values.OrderDescending().Take(5);

        var max = new RollingMax<int>(5);

        // []          << 7
        // [7]         << 1
        // [7,1]       << 3
        // [7,3,1]     << 2
        // [7,3,2,1]   << 4
        // [7,4,3,2,1] << 5
        // [7,5,4,3,2] << 6
        // [7,6,5,4,3] << 7
        // [7,7,6,5,4]
        foreach (var value in values) 
            max.Add(value);
    }

    [TestMethod]
    public void Min()
    {
        double[] values = [50, 49, 51, 48, 52, 47, 53, 46, 54, 45, 55, 44, 56, 43, 57, 42, 58, 41, 59, 40, 60];
        var expected_max = values.OrderBy(v => v).Take(5).ToArray();

        var max = new RollingMax<double>(5, Inverted: true);

        foreach (var value in values)
            max.Add(value);

        for (var i = 0; i < expected_max.Length; i++)
            max[i].AssertEquals(expected_max[i]);
    }

    [TestMethod, Ignore]
    public void Domain()
    {
        var rnd = new Random(5);
        var persons = Enumerable
            .Range(1, 1000)
            .ToArray(i => new
            {
                Id = i,
                Name = $"Person {i}",
                Rating = rnd.Next(10001)
            });

        HashSet<int> vv = [9998, 9991, 9983, 9983, 9980, 9998, 9983, 9977, 9910, 9797];
        var vvv = vv.ToFrozenSet();

        var ratings = persons.Select(p => p.Rating).Where(vvv.Contains).ToArray();
        var max_ratings = ratings.OrderByDescending(v => v).Take(5).ToArray();

        var max_r = RollingMax.New(5, ratings);

        var max_persons = persons.OrderByDescending(p => p.Rating).Take(5).ToArray();

        var max = RollingMax.Build(5, persons).New((v1, v2) => Comparer<int>.Default.Compare(v1.Rating, v2.Rating));

        var actual_max = max.ToArray();

        for (var i = 0; i < max.Count; i++)
            max[i].AssertEquals(max_persons[i]);
    }
}
