using StandardSHA256 = System.Security.Cryptography.SHA256;
using CustomSHA256 = MathCore.Hash.SHA256;

namespace Benchmarks.Hash;

[MemoryDiagnoser, MarkdownExporter, HtmlExporter]
public class SHA256Test
{
    private byte[] _Data;
    private Stream _DataStream;

    [Params(11, 50, 1024 * 1024 / 2, 1024 * 1024 * 10)]
    public int DataLength { get; set; }

    [GlobalSetup]
    public void Initialize()
    {
        var data = new byte[DataLength];
        Random.Shared.NextBytes(data);

        _Data = data;
        _DataStream = new MemoryStream(data);
    }

    [Benchmark]
    public byte[] Standard() => StandardSHA256.HashData(_DataStream);

    [Benchmark]
    public byte[] StandardStream() => StandardSHA256.HashData(_DataStream);

    [Benchmark(Baseline = true)]
    public byte[] Custom() => CustomSHA256.Compute(_Data);

    [Benchmark]
    public byte[] CustomStream() => CustomSHA256.Compute(_DataStream);
}
