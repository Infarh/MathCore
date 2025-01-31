
using MathCore.Algorithms.HashSums;

const string test_str = "123456789";

var test_bytes = System.Text.Encoding.UTF8.GetBytes(test_str);

var hex = test_bytes.ToStringHex();

await using (var stream = new MemoryStream(test_bytes))
{
    var crc32 = await stream.GetCRC32Async();
    var result = crc32.ToString("X8");
}

Console.WriteLine("End.");
return;

