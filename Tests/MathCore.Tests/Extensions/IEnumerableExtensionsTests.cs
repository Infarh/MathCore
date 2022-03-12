namespace MathCore.Tests.Extensions;

[TestClass]
// ReSharper disable once InconsistentNaming
public class IEnumerableExtensionsTests
{
    [TestMethod]
    public void AverageMedian_WindowLength_Odd()
    {
        double[] x = { 2, 80, 6, 3, 0, 2 };
        double[] expected_y = { 2, 41, 6, 6, 3, 2 };

        var buffer = x.AverageMedian(3).ToArray();

        Assert.That.Collection(buffer).ValuesAreEqual(expected_y);
    }

    [TestMethod]
    public void AverageMedian_WindowLength_Even()
    {
        //2, 80, 6, 3, 0, 2, 7, 10, 1, 4
        //2                               : [2]             = 2
        //^                                  ^
        //2, 80                           : [2, 80]         = 41
        //   ^                                  ^
        //2, 80, 6                        : [2, 6, 80]      = 6
        //       ^                              ^  >
        //2, 80, 6, 3                     : [2, 3, 6, 80]   = 4.5
        //*         ^                           ^  >
        //-----------------------------------------------
        //   80, 6, 3, 0                  : [0, 3, 6, 80]   = 4.5
        //   *         ^                     ^        *
        //       6, 3, 0, 2               : [0, 2, 3, 6]    = 2.5
        //       *        ^                     ^  >
        //          3, 0, 2, 7            : [0, 2, 3, 7]    = 2.5
        //          *        ^                        ^
        //             0, 2, 7, 10        : [0, 2, 7, 10]   = 4.5
        //             *        ^                  <  ^
        //                2, 7, 10, 1     : [1, 2, 7, 10]   = 4.5
        //                *         ^        ^
        //                   7, 10, 1, 4  : [1, 4, 7, 10]   = 5.5
        //                             ^        ^

        double[] x = { 2, 80, 6, 3, 0, 2, 7, 10, 1, 4 };
        double[] expected_y = { 2, 41, 6, 4.5, 4.5, 2.5, 2.5, 4.5, 4.5, 5.5 };

        var buffer = x.AverageMedian(4).ToArray();

        Assert.That.Collection(buffer).ValuesAreEqual(expected_y);
    }

    [TestMethod]
    public void TakeLast_Test()
    {
        const int total_count = 100;
        const int count = 7;

        var items = Enumerable.Range(0, total_count);
        var last = items.TakeLast(count).ToArray();

        CollectionAssert.That.Collection(last)
           .IsEqualTo(Enumerable.Range(total_count - count, count).ToArray());
    }

    [TestMethod]
    public void AsBlockEnumerable()
    {
        const int items_count = 95;
        const int block_size = 10;
        var expected_blocks_count = (int)Math.Ceiling((double)items_count / block_size);

        var items = Enumerable.Range(1, items_count).ToList();

        var blocks = items.AsBlockEnumerable(block_size).ToArray();

        Assert.That.Value(blocks).Where(b => b.Length).IsEqual(expected_blocks_count);

        for (var i = 0; i < expected_blocks_count - 1; i++)
        {
            var block = blocks[i];
            var expected_collection = Enumerable.Range(i * block_size + 1, block_size);
            Assert.That.Collection(block).IsEqualTo(expected_collection);
        }
    }
}