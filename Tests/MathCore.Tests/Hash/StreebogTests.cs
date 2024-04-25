using MathCore.Hash;

namespace MathCore.Tests.Hash;

[TestClass]
public class StreebogTests
{
    [TestMethod]
    public void HashTest()
    {
        //byte[] message =
        //[
        //    0x32,0x31,0x30,0x39,0x38,0x37,0x36,0x35,0x34,0x33,0x32,0x31,0x30,0x39,0x38,0x37,
        //    0x36,0x35,0x34,0x33,0x32,0x31,0x30,0x39,0x38,0x37,0x36,0x35,0x34,0x33,0x32,0x31,
        //    0x30,0x39,0x38,0x37,0x36,0x35,0x34,0x33,0x32,0x31,0x30,0x39,0x38,0x37,0x36,0x35,
        //    0x34,0x33,0x32,0x31,0x30,0x39,0x38,0x37,0x36,0x35,0x34,0x33,0x32,0x31,0x30
        //];

        var message = "210987654321098765432109876543210987654321098765432109876543210"u8.ToArray();

        const string expected_hash256 = "00557BE5E584FD52A449B16B0251D05D27F94AB76CBAA6DA890B59D8EF1E159D";
        const string expected_hash512 = "486F64C1917879417FEF082B3381A4E211C324F074654C38823A7B76F830AD00" +
                                        "FA1FBAE42B1285C0352F227524BC9AB16254288DD6863DCCD5B9F54A1AD0541B";

        var gost256 = new Streebog(256);
        var gost512 = new Streebog(512);

        var hash256 = gost256.Compute(message);
        var hash512 = gost512.Compute(message);

        var h256_str = Convert.ToHexString(hash256);
        var h512_str = Convert.ToHexString(hash512);

        h256_str.AssertEquals(expected_hash256);
        h512_str.AssertEquals(expected_hash512);
    }
}
