using System.Diagnostics;
using System.Text;

using MathCore.IO;

namespace MathCore.Tests.IO;

[TestClass]
public class Base64EncoderTests
{
    [TestMethod]
    public void Encode_7B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6];
        var expected_result = Convert.ToBase64String(input_data);

        using (var writer = new StringWriter(buffer))
        {
            using var encoder = new Base64Encoder(writer);
            encoder.Write(input_data);
        }

        var actual_result = buffer.ToString();


        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug(  "  actual");
    }

    [TestMethod]
    public void Encode_8B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6, 7];
        var expected_result = Convert.ToBase64String(input_data);

        using (var writer = new StringWriter(buffer))
        {
            using var encoder = new Base64Encoder(writer);
            encoder.Write(input_data);
        }

        var actual_result = buffer.ToString();


        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug(  "  actual");
    }

    [TestMethod]
    public void Encode_9B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        var expected_result = Convert.ToBase64String(input_data);

        using (var writer = new StringWriter(buffer))
        {
            using var encoder = new Base64Encoder(writer);
            encoder.Write(input_data);
        }

        var actual_result = buffer.ToString();


        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug(  "  actual");
    }

    [TestMethod]
    public async Task EncodeAsync_7B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6];
        var expected_result = Convert.ToBase64String(input_data);

        await using (var writer = new StringWriter(buffer))
        {
            await using var encoder = new Base64Encoder(writer);
            await encoder.WriteAsync(input_data);
        }

        var actual_result = buffer.ToString();

        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug("  actual");
    }

    [TestMethod]
    public async Task EncodeAsync_8B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6, 7];
        var expected_result = Convert.ToBase64String(input_data);

        await using (var writer = new StringWriter(buffer))
        {
            await using var encoder = new Base64Encoder(writer);
            await encoder.WriteAsync(input_data);
        }

        var actual_result = buffer.ToString();

        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug("  actual");
    }

    [TestMethod]
    public async Task EncodeAsync_9B()
    {
        var buffer = new StringBuilder();

        byte[] input_data = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        var expected_result = Convert.ToBase64String(input_data);

        await using (var writer = new StringWriter(buffer))
        {
            await using var encoder = new Base64Encoder(writer);
            await encoder.WriteAsync(input_data);
        }

        var actual_result = buffer.ToString();

        actual_result.AssertEquals(expected_result);
        expected_result.ToDebug("expected");
        actual_result.ToDebug("  actual");
    }
}
