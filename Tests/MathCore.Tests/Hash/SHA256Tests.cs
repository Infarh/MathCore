﻿using System.Text;

using MathCore.Hash;

namespace MathCore.Tests.Hash;

[TestClass]
public class SHA256Tests
{
    [TestMethod]
    public void Hash_123()
    {
        const string str      = "123";
        const string expected = "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3";

        var input_bytes = Encoding.UTF8.GetBytes(str);

        var sha256 = new Sha256Digest();

        var digest = sha256.hash(input_bytes);
        var result = digest.ToByteArray();

        var hash_str = result.ToStringHex(false);

        hash_str.AssertEquals(expected);
    }
}
