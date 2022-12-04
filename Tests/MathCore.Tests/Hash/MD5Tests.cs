using System.Diagnostics;
using System.Text;

namespace MathCore.Tests.Hash;

[TestClass]
public class MD5Tests
{
    [TestMethod]
    public void Hash_of_Hello_World_UTF8()
    {
        const string str = "Hello World";

        var bytes = Encoding.UTF8.GetBytes(str);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var md5_result_bytes = MathCore.Hash.MD5.Compute(bytes);

        var actual = md5_result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }

    [TestMethod]
    public void Hash_Stream_of_Hello_World_UTF8()
    {
        const string str = "Hello World";

        var bytes = Encoding.UTF8.GetBytes(str);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var md5_result_bytes = MathCore.Hash.MD5.Compute(new MemoryStream(bytes));

        var actual = md5_result_bytes.ToStringHex();
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
    [DataRow(70, DisplayName = "Data byte length = 70")]
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

        var md5_result_bytes = MathCore.Hash.MD5.Compute(bytes);

        var actual   = md5_result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }


    [TestMethod]
    //[DataRow(10)]
    //[DataRow(30)]
    [DataRow(55)]
    [DataRow(56)]
    //[DataRow(57)]
    //[DataRow(58)]
    //[DataRow(59)]
    //[DataRow(60)]
    //[DataRow(61)]
    //[DataRow(62)]
    [DataRow(63)]
    [DataRow(64)]
    //[DataRow(65)]
    //[DataRow(66)]
    //[DataRow(67)]
    //[DataRow(68)]
    //[DataRow(69)]
    //[DataRow(70)]
    public void Hash_Stream(int DataLength)
    {
        //var rnd_seed = Random.Shared.Next();
        var rnd_seed = 5;
        Debug.WriteLine("Random seed " + rnd_seed);

        var rnd = new Random(rnd_seed);

        var bytes = new byte[DataLength];
        rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.MD5.HashData(bytes);

        var md5_result_bytes = MathCore.Hash.MD5.Compute(new MemoryStream(bytes));

        var actual   = md5_result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }
}
