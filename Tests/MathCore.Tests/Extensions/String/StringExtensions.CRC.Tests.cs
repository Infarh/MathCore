using System.Text;

namespace MathCore.Tests.Extensions.Strings;

[TestClass]
public class StringExtensionsCRCTests
{
    [TestMethod]
    public void ComputeCRC32_Standard_ZIP()
    {
        const string data = "123456789";
        const uint expected = 0xCBF43926;

        var actual = data.ComputeCRC32(
            Polynomial: 0xEDB88320,
            InitialValue: 0xFFFFFFFF,
            XOROut: 0xFFFFFFFF,
            RefIn: true,
            RefOut: false);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ComputeCRC8_FromByteSequence()
    {
        var bytes = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        var text = Encoding.Latin1.GetString(bytes);
        const byte expected = 0x18;

        var actual = text.ComputeCRC8(encoding: Encoding.Latin1);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ComputeCRC8_WithCustomParameters()
    {
        var bytes = new byte[] { 0x3F, 0xA2, 0x13, 0x21, 0x03 };
        var text = Encoding.Latin1.GetString(bytes);
        const byte expected = 0xDE;

        var actual = text.ComputeCRC8(
            encoding: Encoding.Latin1,
            Polynomial: 0x07,
            InitialValue: 0xFF,
            XOROut: 0xFF);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ComputeCRC_AllVariants_Different()
    {
        const string text = "Test Data For CRC";

        var crc8  = text.ComputeCRC8();
        var crc16 = text.ComputeCRC16();
        var crc32 = text.ComputeCRC32(Polynomial: 0xEDB88320, InitialValue: 0xFFFFFFFF, XOROut: 0xFFFFFFFF, RefIn: true, RefOut: false);
        var crc64 = text.ComputeCRC64();

        Assert.AreNotEqual((ulong)crc8, (ulong)crc16);
        Assert.AreNotEqual((ulong)crc16, (ulong)crc32);
        Assert.AreNotEqual(crc32, (uint)crc64);
    }

    [TestMethod]
    public void ComputeCRC_Consistency()
    {
        const string text = "Consistency check string";

        var a1 = text.ComputeCRC32(Polynomial: 0xEDB88320, InitialValue: 0xFFFFFFFF, XOROut: 0xFFFFFFFF, RefIn: true, RefOut: false);
        var a2 = text.ComputeCRC32(Polynomial: 0xEDB88320, InitialValue: 0xFFFFFFFF, XOROut: 0xFFFFFFFF, RefIn: true, RefOut: false);

        Assert.AreEqual(a1, a2);
    }

    [TestMethod]
    public void ComputeCRC32_NonEmpty()
    {
        const string text = "Some random test text";
        var crc = text.ComputeCRC32(Polynomial: 0xEDB88320, InitialValue: 0xFFFFFFFF, XOROut: 0xFFFFFFFF, RefIn: true, RefOut: false);
        Assert.IsTrue(crc >= 0);
    }
}
