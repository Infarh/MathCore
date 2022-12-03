using System.Diagnostics;
using System.Text;

using MathCore.Hash;

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

        var md5_result_bytes = MD5.Compute(bytes);

        var expected = expected_result.Select(b => b.ToString("x2")).Sum();

        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)md5_result_bytes);

        md5_result_bytes.AssertEquals(expected);
    }
}
