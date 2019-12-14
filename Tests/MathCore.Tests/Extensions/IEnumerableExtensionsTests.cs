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

            CollectionAssert.That.Collection(last).IsEqualTo(Enumerable.Range(total_count - count, count).ToArray());
        }
    }
}