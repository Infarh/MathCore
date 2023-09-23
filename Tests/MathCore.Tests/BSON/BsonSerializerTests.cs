using MathCore.BSON;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MathCore.Tests.BSON;

[TestClass]
public class BsonSerializerTests
{
    [TestMethod]
    public void SerializeDictionaryOfStringsHelloWorld()
    {
        Dictionary<string, string> value = new()
        {
            { "hello", "world" },
        };

        using var actual_stream = new MemoryStream(22);
        using var expect_stream = new MemoryStream(22);

        BsonSerializer.Serialize(value, actual_stream);

        var writer = new BsonDataWriter(expect_stream);

        var serializer = new JsonSerializer();
        serializer.Serialize(writer, value);

        var actual_bytes = actual_stream.ToArray();
        var expect_bytes = expect_stream.ToArray();

        //actual_bytes.SequenceEqual(expect_bytes).AssertTrue();

        var actual_hex = actual_bytes.ToStringHexBytes();
        var expect_hex = expect_bytes.ToStringHexBytes();

        actual_hex.AssertEquals(expect_hex);
    }

    [TestMethod]
    public void SerializeDictionaryOfObjects()
    {
        Dictionary<string, object> value = new()
        {
            { "str", "qwe" },
            { "int", 42 },
            { "uint", 13u },
            { "short", (short)15 },
            { "ushort", (ushort)32 },
            { "long", (long)17 },
            { "ulong", (ulong)22 },
            { "byte", (byte)11 },
            { "sbyte", (byte)18 },
            { "double", 3.14 },
            { "float", (float)3.14 },
            { "decimal", (decimal)3.14 },
            { "bool", true },
            { "char", 'q' },
            { "bytes", new byte[] { 5, 4, 3, 2, 1 } },
        };

        using var actual_stream = new MemoryStream();
        using var expect_stream = new MemoryStream();

        //BsonSerializer.Serialize(value, actual_stream);

        var writer = new BsonDataWriter(expect_stream);

        var serializer = new JsonSerializer();
        serializer.Serialize(writer, value);

        var actual_hex = actual_stream.ToArray().ToStringHexBytes();
        var expect_hex = expect_stream.ToArray().ToStringHexBytes();

        // C2.00.00.00
        // 02 73.74.72.00 04.00.00.00 71.77.65.00
        // 10 69.6E.74.00 2A.00.00.00
        // 10 75.69.6E.74.00 0D.00.00.00
        // 10 73.68.6F.72.74.00 0F.00.00.00
        // 10 75.73.68.6F.72.74.00 20.00.00.00
        // 12 6C.6F.6E.67.00 11.00.00.00.00.00.00.00
        // 12 75.6C.6F.6E.67.00 16.00.00.00.00.00.00.00
        // 10 62.79.74.65.00 0B.00.00.00
        // 10 73.62.79.74.65.00 12.00.00.00
        // 01 64.6F.75.62.6C.65.00    1F.85.EB.51.B8.1E.09.40
        // 01 66.6C.6F.61.74.00       00.00.00.60.B8.1E.09.40
        // 01 64.65.63.69.6D.61.6C.00 1F.85.EB.51.B8.1E.09.40
        // 08 62.6F.6F.6C.00 01
        // 02 63.68.61.72.00 02.00.00.00 71.00
        // 05 62.79.74.65.73.00 05.00.00.00 00 05.04.03.02.01
        // 00
    }
}
