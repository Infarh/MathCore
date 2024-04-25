using System.Diagnostics;

using MathCore.Hash.CRC;

namespace MathCore.Tests.Hash.CRC;

[TestClass]
public class CRC16Tests
{
    [TestMethod]
    public void Poly_1021_initial_0000_data_3FA2132103_crc_718E()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const ushort expected_crc = 0x718E;

        var crc = new CRC16(CRC16.Mode.XMODEM);

        var actual_crc = crc.Compute(data);

        Debug.WriteLine("Actual   0x{0:X4}", actual_crc);
        Debug.WriteLine("Expected 0x{0:X4}", expected_crc);
        Assert.That.Value($"0x{actual_crc:X4}").IsEqual($"0x{expected_crc:X4}");
    }

    [TestMethod]
    public void StaticHash()
    {
        var data = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        const ushort expected_crc = 0x718E;

        var actual_crc = CRC16.Hash(data, CRC16.Mode.XMODEM, 0, 0);

        var expected_hash = $"0x{expected_crc:X4}";
        var actual_hash = $"0x{actual_crc:X4}";

        expected_hash.ToDebug();
        actual_hash.ToDebug();

        actual_hash.AssertEquals(expected_hash);
    }
}
