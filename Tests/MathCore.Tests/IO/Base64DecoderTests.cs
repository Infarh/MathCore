using System.Diagnostics;

using MathCore.IO;

namespace MathCore.Tests.IO;

[TestClass]
public class Base64DecoderTests
{
    [TestMethod]
    public void Decode_6B()
    {
        byte[] expected_data = [0, 1, 2, 3, 4, 5];
        var base64_str = Convert.ToBase64String(expected_data);
        base64_str.ToDebug("base64");

        using var data_stream = new MemoryStream();
        using var writer = new BinaryWriter(data_stream);
        using (var decoder = new Base64Decoder(writer))
            decoder.Write(base64_str);

        var actual_data = data_stream.ToArray();

        actual_data.AssertEquals(expected_data);
        actual_data.ToDebugEnum();
    }

    [TestMethod]
    public void Decode_7B()
    {
        byte[] expected_data = [0, 1, 2, 3, 4, 5, 6];
        var base64_str = Convert.ToBase64String(expected_data);
        base64_str.ToDebug("base64");

        using var data_stream = new MemoryStream();
        using var writer = new BinaryWriter(data_stream);
        using (var decoder = new Base64Decoder(writer))
            decoder.Write(base64_str);

        var actual_data = data_stream.ToArray();

        actual_data.AssertEquals(expected_data);
        actual_data.ToDebugEnum();
    }

    [TestMethod]
    public void Decode_8B()
    {
        byte[] expected_data = [0, 1, 2, 3, 4, 5, 6, 7];
        var base64_str = Convert.ToBase64String(expected_data);
        base64_str.ToDebug("base64");

        using var data_stream = new MemoryStream();
        using var writer = new BinaryWriter(data_stream);
        using (var decoder = new Base64Decoder(writer))
            decoder.Write(base64_str);

        var actual_data = data_stream.ToArray();

        actual_data.AssertEquals(expected_data);
        actual_data.ToDebugEnum();
    }

    [TestMethod]
    public void Decode_9B()
    {
        byte[] expected_data = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        var base64_str = Convert.ToBase64String(expected_data);
        base64_str.ToDebug("base64");

        using var data_stream = new MemoryStream();
        using var writer = new BinaryWriter(data_stream);
        using (var decoder = new Base64Decoder(writer))
            decoder.Write(base64_str);

        var actual_data = data_stream.ToArray();

        actual_data.AssertEquals(expected_data);
        actual_data.ToDebugEnum();
    }
}
