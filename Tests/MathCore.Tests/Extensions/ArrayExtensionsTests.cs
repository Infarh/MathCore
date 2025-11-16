namespace MathCore.Tests.Extensions;

[TestClass]
public class ArrayExtensionsTests
{
    [TestMethod]
    public void AsRandomEnumerableTest()
    {
        var array = Enumerable.Range(0, 100).ToArray();

        var random_array_1 = array.AsRandomEnumerable().ToArray();
        var random_array_2 = array.AsRandomEnumerable().ToArray();

        Assert.That.Value(random_array_1.Length).IsEqual(array.Length);
        Assert.That.Value(random_array_2.Length).IsEqual(array.Length);

        var numbers1 = random_array_1.Distinct().ToArray();
        var numbers2 = random_array_2.Distinct().ToArray();

        Assert.That.Value(numbers1.Length).IsEqual(array.Length);
        Assert.That.Value(numbers2.Length).IsEqual(array.Length);

        CollectionAssert.That.Collection(random_array_1.OrderBy(v => v).ToArray()).IsEqualTo(array);
        CollectionAssert.That.Collection(random_array_2.OrderBy(v => v).ToArray()).IsEqualTo(array);
    }
}