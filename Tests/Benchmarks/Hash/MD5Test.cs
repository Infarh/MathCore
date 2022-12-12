using StandardMD5 = System.Security.Cryptography.MD5;
using CustomMD5 = MathCore.Hash.MD5;

namespace Benchmarks.Hash;

[MemoryDiagnoser, MarkdownExporter, HtmlExporter]
public class MD5Test
{
    private byte[] _Data;
    private readonly byte[] _Hash = new byte[16];
    private Stream _DataStream;

    [Params(32, 128, 1024 * 1024 / 2, 1024 * 1024, 1024 * 1024 * 10)]
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
    public byte[] Standard()
    {
        var hash = StandardMD5.HashData(_DataStream);
        return hash;
    }

    [Benchmark]
    public void StandardDest() => StandardMD5.HashData(_DataStream, _Hash);

    [Benchmark(Baseline = true)]
    public async Task<byte[]> StandardAsync()
    {
        var hash = await StandardMD5.HashDataAsync(_DataStream);
        return hash;
    }

    [Benchmark]
    public async Task StandardDestAsync() => await StandardMD5.HashDataAsync(_DataStream, _Hash);

    [Benchmark]
    public byte[] CustomStream()
    {
        var hash = CustomMD5.Compute(_DataStream);
        return hash;
    }

    [Benchmark]
    public async Task<byte[]> CustomStreamAsync()
    {
        var hash = await CustomMD5.ComputeAsync(_DataStream);
        return hash;
    }
}
