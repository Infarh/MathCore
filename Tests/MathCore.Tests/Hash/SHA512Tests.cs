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

        //bytes = new byte[160];
        //var rnd = new Random(5);
        //rnd.NextBytes(bytes);

        var expected_result = System.Security.Cryptography.SHA512.HashData(bytes);

        var result_bytes = MathCore.Hash.SHA512.Compute(bytes);

        var actual   = result_bytes.ToStringHex();
        var expected = expected_result.ToStringHex();

        Debug.WriteLine("");
        Debug.WriteLine("Expected {0}", (object)expected);
        Debug.WriteLine("Actual   {0}", (object)actual);

        actual.AssertEquals(expected);
    }
}