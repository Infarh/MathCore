using System.Diagnostics;
using System.Text;

using MathCore.Hash;

namespace MathCore.Tests.Hash;

[TestClass]
public class SHA256Tests
{
    [TestMethod]
    public void Hash_Hello_World()
    {
        Debug.WriteLine(DateTime.Now);

        const string str = "Hello World";

        var input_bytes       = Encoding.UTF8.GetBytes(str);
        var expected_hash     = System.Security.Cryptography.SHA256.HashData(input_bytes);
        var expected_hash_str = expected_hash.ToStringHex(false);

        var result = SHA256.Compute(input_bytes);

        var hash_str = result.ToStringHex(false);

        hash_str.AssertEquals(expected_hash_str);
    }

    [TestMethod]
    public void Hash_LongData()
    {
        Debug.WriteLine(DateTime.Now);

        //var seed = Random.Shared.Next();
        var seed = 5;
        var rnd  = new Random(seed);

        var data_length = rnd.Next(1024 * 1024 / 2, 5 * 1024 * 1024 + 1);
        Debug.WriteLine("Data length = {0}", data_length);

        var data = new byte[data_length];
        rnd.NextBytes(data);

        var expected_hash = System.Security.Cryptography.SHA256.HashData(data);

        var actual_hash = SHA256.Compute(data);

        var expected_hash_str = expected_hash.ToStringHex(false);
        var actual_hash_str   = actual_hash.ToStringHex(false);

        actual_hash_str.AssertEquals(expected_hash_str);
    }
}