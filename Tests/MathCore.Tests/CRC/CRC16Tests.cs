using MathCore.CRC;

namespace MathCore.Tests.CRC;

[TestClass]
public class CRC16Tests
{
    [TestMethod]
    public void Poly_1021_initial_0000_data_3FA2132103_crc_718E()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const ushort expected_crc = 0x718E;

        var crc = new CRC16();

        var actual_crc = crc.Compute(data);

        Assert.That.Value(actual_crc).IsEqual(expected_crc);
    }
}
