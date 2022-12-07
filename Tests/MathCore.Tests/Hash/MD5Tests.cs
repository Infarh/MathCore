using System.Diagnostics;
using System.Text;

namespace MathCore.Tests.Hash;

[TestClass]
public class MD5Tests
{
    [TestMethod]
    public void Hash_Hello_World()
    {
        const string str = "Hello World";

        var bytes = Encoding.UTF8.GetBytes(str);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var result_bytes = MathCore.Hash.MD5.Compute(bytes);

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void Hash_Stream_Hello_World()
    {
        const string str = "Hello World";

        var bytes = Encoding.UTF8.GetBytes(str);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var result_bytes = MathCore.Hash.MD5.Compute(new MemoryStream(bytes));

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(11, DisplayName = "Data byte length = 11")]
    [DataRow(55, DisplayName = "Data byte length = 55")]
    [DataRow(56, DisplayName = "Data byte length = 56")]
    [DataRow(63, DisplayName = "Data byte length = 63")]
    [DataRow(160, DisplayName = "Data byte length = 160")]
    public void Hash(int DataLength)
    {
        Debug.WriteLine(DateTime.Now);
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var result_bytes = MathCore.Hash.MD5.Compute(bytes);

        var actual   = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(11, DisplayName = "Data stream length = 11")]
    [DataRow(55, DisplayName = "Data stream length = 55")]
    [DataRow(56, DisplayName = "Data stream length = 56")]
    [DataRow(63, DisplayName = "Data stream length = 63")]
    [DataRow(160, DisplayName = "Data stream length = 160")]
    public void Hash_Stream(int DataLength)
    {
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var result_bytes = MathCore.Hash.MD5.Compute(new MemoryStream(bytes));

        var actual   = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(11, DisplayName = "Data stream length = 11")]
    [DataRow(55, DisplayName = "Data stream length = 55")]
    [DataRow(56, DisplayName = "Data stream length = 56")]
    [DataRow(63, DisplayName = "Data stream length = 63")]
    [DataRow(160, DisplayName = "Data stream length = 160")]
    public async Task HashAsync_Stream(int DataLength)
    {
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var result_bytes = await MathCore.Hash.MD5.ComputeAsync(new MemoryStream(bytes));

        var actual   = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }
}
