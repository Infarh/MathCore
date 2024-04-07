using System.Globalization;
using System.Runtime.CompilerServices;

namespace Benchmarks;

[MemoryDiagnoser]
public class ToHexStringTest
{
    private const int __Size = 10_000;

    private static readonly byte[] __Data = Enumerable.Range(0, __Size).ToArray(i => (byte)i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char ToChar(int b) => b switch
    {
        0 => '0',
        1 => '1',
        2 => '2',
        3 => '3',
        4 => '4',
        5 => '5',
        6 => '6',
        7 => '7',
        8 => '8',
        9 => '9',
        10 => 'a',
        11 => 'b',
        12 => 'c',
        13 => 'd',
        14 => 'e',
        15 => 'f',
        _ => ' '
    };

    [Benchmark]
    public string Custom()
    {
        Span<char> bb = stackalloc char[__Size * 2];

        for (var i = 0; i < __Size; i++)
        {
            bb[i * 2] = ToChar(__Data[i] >> 4);
            bb[i * 2 + 1] = ToChar(__Data[i] & 0xf);
        }

        return new(bb);
    }

    [Benchmark(Baseline = true)]
    public string ConvertToHexStr()
    {
        var hex = Convert.ToHexString(__Data);
        return hex;
    }
}
