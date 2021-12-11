using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests.Extensions
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class IEnumerableExtensionsTests
    {
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
}