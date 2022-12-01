using System.IO;

namespace Benchmarks;

[MemoryDiagnoser]
public class CSVParseTest
{
    private static IEnumerable<string> Lines => ReadLines("data.csv");

    private static IEnumerable<string> ReadLines(string FileName)
    {
        if (!File.Exists(FileName)) throw new FileNotFoundException("Файл данных не найден", FileName);
        using var reader = File.OpenText(FileName);
        while (!reader.EndOfStream)
            if (reader.ReadLine() is { Length: > 0 } line)
                yield return line;
    }

    [Benchmark(Baseline = true)]
    public int ParseSplit()
    {
        var result = 0;
        foreach (var line in Lines)
            result += CSVSplit.Parse(line);
        return result;
    }

    [Benchmark]
    public int ParseRegex()
    {
        var result = 0;
        var parser = new CSVRegex(',');
        foreach (var line in Lines)
            result += parser.Parse(line);
        return result;
    }

    [Benchmark]
    public int ParseMemory()
    {
        var result = 0;
        foreach (var line in Lines)
            result += CSVMemory.Parse(line);
        return result;
    }

    [Benchmark]
    public int ParseSpan()
    {
        var result = 0;
        foreach (var line in Lines)
            result += CSVSpan.Parse(line);
        return result;
    }

    [Benchmark]
    public int ParseString()
    {
        var result = 0;
        foreach (var line in Lines)
            result += CSVString.Parse(line);
        return result;
    }
}