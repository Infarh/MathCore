namespace MathCore.Tests.Extensions.Numerics;

[TestClass]
public class IntExtensionsTests
{
    [TestMethod]
    public void GetNumberOfDigits_of_123_return_3()
    {
        const int value                = 123;
        const int expected_digit_count = 3;

        var actual_digit_count = value.GetNumberOfDigits();

        Assert.That.Value(actual_digit_count)
           .IsEqual(expected_digit_count);
    }

    [TestMethod]
    public void BitsCount_of_123_return_5()
    {
        const int value               = 123;
        const int expected_bits_count = 7;

        var actual_bits_count = value.BitCount();

        Assert.That.Value(actual_bits_count).IsEqual(expected_bits_count);
    }

    [TestMethod]
    public void FromComplementBinary()
    {
        const int x = 0b0111_1111__1111_1111__1111_1111__1111_1111; // 65535
        const int bit_count = 17;

        var y = x.FromComplementBinary(bit_count);
    }
}