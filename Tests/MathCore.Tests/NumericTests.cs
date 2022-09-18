namespace MathCore.Tests
{
    [TestClass]
    public class NumericTests
    {
        [TestMethod]
        public void HiBit_of_bx0111_1011_return_bx0100_0000()
        {
            const int value = 123;                  // bx0111_1011
            const int expected_hi_bit_value = 64;   // bx0100_0000

            var actual_hi_bit_index = Numeric.HiBit(value);

            Assert.That.Value(actual_hi_bit_index).IsEqual(expected_hi_bit_value);
        }

        [TestMethod]
        public void HiBit_of_0x0020_1234_return_0x0020_0000()
        {
            const int value = 0x0020_1234;
            const int expected_hi_bit_value = 0x0020_0000;

            var actual_hi_bit_index = Numeric.HiBit(value);

            Assert.That.Value(actual_hi_bit_index).IsEqual(expected_hi_bit_value);
        }

        [TestMethod]
        public void SignedBitCount_of_bx0111_1011_return_6()
        {
            const int value = 123;            // bx0111_1011
            const int expected_bit_count = 6;

            var actual_bit_count = Numeric.SignedBitCount(value);

            Assert.That.Value(actual_bit_count).IsEqual(expected_bit_count);
        }

        [TestMethod]
        public void Log2_of_bx0111_1011_return_6()
        {
            const int value = 123;            // bx0111_1011
            const int expected_log2 = 6;

            var actual_log2 = Numeric.Log2(value);

            Assert.That.Value(actual_log2).IsEqual(expected_log2);
        }
    }
}
