using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks;

public class ParseDoubleTest
{
    [Params("3.1415926535897932384626", "12345678901234567890", "9.8765432101234e-207")]
    public string StrValue { get; set; }

    [Benchmark(Baseline = true)]
    public double Parse() => double.Parse(StrValue, CultureInfo.InvariantCulture);

    [Benchmark]
    public double TryParse() => double.TryParse(StrValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : double.NaN;

    [Benchmark]
    public double Convert() => System.Convert.ToDouble(StrValue, CultureInfo.InvariantCulture);

    [Benchmark]
    public double StringPtr() => StrValue.AsStringPtr().ParseDouble();
}

public class ParseIntTest
{
    [Params("1234567890")]
    public string StrValue { get; set; }

    [Benchmark]
    public int Parse() => int.Parse(StrValue);

    [Benchmark(Baseline = true)]
    public int Multiply()
    {
        var str   = StrValue;
        var resut = 0;
        foreach (var c in str)
            resut = resut * 10 + (c - '0');
        return resut;
    }

    [Benchmark]
    public int Offset()
    {
        var str   = StrValue;
        var resut = 0;
        foreach (var c in str)
            resut = (resut << 3) + (resut << 1) + (c - '0');
        return resut;
    }
}
