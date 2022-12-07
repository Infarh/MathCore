using System.Diagnostics;
using System.Text;

namespace MathCore.Tests.Hash;

[TestClass]
public class SHA512Tests
{
    [TestMethod]
    public void Hash_Hello_World()
    {
        const string str = "Hello World";

        var bytes = Encoding.UTF8.GetBytes(str);

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = MathCore.Hash.SHA512.Compute(bytes);

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

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = MathCore.Hash.SHA512.Compute(new MemoryStream(bytes));

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(011, DisplayName = "Data length =  11")]
    [DataRow(111, DisplayName = "Data length = 111")]
    [DataRow(112, DisplayName = "Data length = 112")]
    [DataRow(127, DisplayName = "Data length = 127")]
    [DataRow(128, DisplayName = "Data length = 128")]
    [DataRow(160, DisplayName = "Data length = 160")]
    [DataRow(271, DisplayName = "Data length = 271")]
    public void Hash(int DataLength)
    {
        Debug.WriteLine(DateTime.Now);
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = MathCore.Hash.SHA512.Compute(bytes);

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(011, DisplayName = "Data stream length =  11")]
    [DataRow(111, DisplayName = "Data stream length = 111")]
    [DataRow(112, DisplayName = "Data stream length = 112")]
    [DataRow(113, DisplayName = "Data stream length = 113")]
    [DataRow(119, DisplayName = "Data stream length = 119")]
    [DataRow(120, DisplayName = "Data stream length = 120")]
    [DataRow(127, DisplayName = "Data stream length = 127")]
    [DataRow(128, DisplayName = "Data stream length = 128")]
    [DataRow(160, DisplayName = "Data stream length = 160")]
    [DataRow(271, DisplayName = "Data stream length = 271")]
    public void Hash_Stream(int DataLength)
    {
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = MathCore.Hash.SHA512.Compute(new MemoryStream(bytes));

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    [DataRow(011, DisplayName = "Data stream length =  11")]
    [DataRow(111, DisplayName = "Data stream length = 111")]
    [DataRow(112, DisplayName = "Data stream length = 112")]
    [DataRow(113, DisplayName = "Data stream length = 113")]
    [DataRow(119, DisplayName = "Data stream length = 119")]
    [DataRow(120, DisplayName = "Data stream length = 120")]
    [DataRow(127, DisplayName = "Data stream length = 127")]
    [DataRow(128, DisplayName = "Data stream length = 128")]
    [DataRow(160, DisplayName = "Data stream length = 160")]
    [DataRow(271, DisplayName = "Data stream length = 271")]
    public async Task HashAsync_Stream(int DataLength)
    {
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = await MathCore.Hash.SHA512.ComputeAsync(new MemoryStream(bytes));

        var actual = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }
}