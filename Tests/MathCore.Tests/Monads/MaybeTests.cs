using MathCore.Monads;

namespace MathCore.Tests.Monads
{
    [TestClass]
    public class MaybeTests
    {
        [TestMethod]
        public void Test()
        {
            var one = Maybe.Return(1);
            var nothing = Maybe.Nothing<int>();
            var nothing2 =
                from x in one
                from y in nothing
                select x + y;

            var two = one.Where(z => z > 0).Select(z => z + 1);

            Assert.That.Value(one()).IsEqual(1);
            Assert.That.Value(two()).IsEqual(2);
            Assert.ThrowsException<InvalidOperationException>(() => nothing2());
        }
    }
}
