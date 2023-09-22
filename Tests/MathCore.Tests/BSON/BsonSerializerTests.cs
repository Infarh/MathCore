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
            { "short", (short)3.14 },
            { "decimal", (decimal)3.14 },
            { "bool", true },
            { "char", 'q' },
            { "bytes", new byte[] { 1, 2, 3, 4, 5 } },
        };

        using var actual_stream = new MemoryStream();
        using var expect_stream = new MemoryStream();

        BsonSerializer.Serialize(value, actual_stream);

        var writer = new BsonDataWriter(expect_stream);

        var serializer = new JsonSerializer();
        serializer.Serialize(writer, value);

        var actual_hex = actual_stream.ToArray().ToStringHexBytes();
        var expect_hex = expect_stream.ToArray().ToStringHexBytes();
    }
}
