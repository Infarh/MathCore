using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathCore.Tests
{
    [TestClass]
    public class SpecialFunctionsTests
    {
        [TestMethod]
        public void Fibonachi()
        {
            var expected_value_last = 1;
            var expected_value = 1;

            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi(0)).IsEqual(0);
            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi(1)).IsEqual(1);
            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi(2)).IsEqual(1);

            for (var i = 0; i < 10; i++)
            {
                var tmp = expected_value_last;
                expected_value_last = expected_value;
                expected_value = expected_value_last + tmp;

                var actual_value = MathCore.SpecialFunctions.Fibonachi(i + 3);
                Assert.That.Value(actual_value).IsEqual(expected_value);
            }
        }

        [TestMethod]
        public void Fibonachi2()
        {
            var expected_value_last = 1;
            var expected_value = 1;

            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi2(0)).IsEqual(0);
            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi2(1)).IsEqual(1);
            Assert.That.Value(MathCore.SpecialFunctions.Fibonachi2(2)).IsEqual(1);

            for (var i = 0; i < 10; i++)
            {
                var tmp = expected_value_last;
                expected_value_last = expected_value;
                expected_value = expected_value_last + tmp;

                var actual_value = MathCore.SpecialFunctions.Fibonachi2(i + 3);
                Assert.That.Value(actual_value).IsEqual(expected_value);
            }
        }
    }
}
