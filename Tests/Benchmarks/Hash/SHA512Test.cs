using StandardSHA512 = System.Security.Cryptography.SHA512;
using CustomSHA512 = MathCore.Hash.SHA512;

namespace Benchmarks.Hash;

[MemoryDiagnoser, MarkdownExporter, HtmlExporter]
public class SHA512Test
{
    private byte[] _Data;
    private Stream _DataStream;

    [Params(11, 150, 1024 * 1024 / 2, 1024 * 1024 * 10)]
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
    public byte[] Standard() => StandardSHA512.HashData(_Data);

    [Benchmark]
    public byte[] StandardStream() => StandardSHA512.HashData(_DataStream);
    
    [Benchmark(Baseline = true)]
    public byte[] Custom() => CustomSHA512.Compute(_DataStream);
    
    [Benchmark]
    public byte[] CustomStream() => CustomSHA512.Compute(_DataStream);
}
