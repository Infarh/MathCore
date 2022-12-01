using System.Globalization;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringPtrParseDoubleTest
{
    private static readonly CultureInfo __InvariantCulture = CultureInfo.InvariantCulture;

    [Params("-123.456E-051")]
    private string TestStringValue { get; set; } = "-123.456E-051";

    [Benchmark(Baseline = true)]
    public double DoubleTryParse() => double.TryParse(TestStringValue, NumberStyles.Any, __InvariantCulture, out var v) ? v : double.NaN;

    [Benchmark]
    public double StrPtrTryParse() => TestStringValue.AsStringPtr().TryParseDouble(__InvariantCulture, out var v) ? v : double.NaN;
}
