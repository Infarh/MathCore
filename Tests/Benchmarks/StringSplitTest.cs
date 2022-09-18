using System.Globalization;
using System.Text;

using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringSplitTest
{
    private static char[] __Separators = { ':', '|', '$', ';' };
    private static readonly CultureInfo __Culture = CultureInfo.CurrentCulture;

    private static readonly string __DataString = Enumerable
       .Range(1, 50)
       .Aggregate(
            new StringBuilder(),
            (s, _) => s.AppendFormat("Value{0}={0};", Math.PI * 1000),
            s =>
            {
                s.Length--;
                return s.ToString();
            });

    [Benchmark(Baseline = true)]
    public double Split()
    {
        var result = 0d;
        foreach (var s in __DataString.Split(';'))
            result += double.Parse(s.Split('=')[1]);
        return result;
    }

    [Benchmark]
    public double StringRefSplit()
    {
        var result = 0d;
        var culture = __Culture;
        foreach (var s in __DataString.AsStringPtr().Split(';')) 
            result += s.GetValueDouble(__Culture);

        return result;
    }

    [Benchmark]
    public double StringRefSplitMany()
    {
        var result = 0d;
        var separators = __Separators;
        var culture = __Culture;
        foreach (var s in __DataString.AsStringPtr().Split(separators)) 
            result += s.GetValueDouble(culture);
        return result;
    }
}