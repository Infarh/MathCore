using System.Globalization;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringPtrParseDoubleTest
{
    private static readonly string __TestStringValue = "-123.456E-051";
    private static readonly CultureInfo __InvariantCulture = CultureInfo.InvariantCulture;

    [Benchmark(Baseline = true)]
    public double DoubleTryParse() => double.TryParse(__TestStringValue, NumberStyles.Any, __InvariantCulture, out var v) ? v : double.NaN;

    [Benchmark]
    public double StrPtrTryParse() => __TestStringValue.AsStringPtr().TryParseDouble(__InvariantCulture, out var v) ? v : double.NaN;
}
