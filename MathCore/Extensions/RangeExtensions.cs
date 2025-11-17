#if NET8_0_OR_GREATER

namespace MathCore.Extensions;

public static class RangeExtensions
{
    public static void Deconstruct(this Range range, out Index start, out Index end) => (start, end) = (range.Start, range.End);

    public static (int Start, int End) ToIndexes(this Range range, int length)
    {
        var (start, end) = range;
        return (start.ToIndex(length), end.ToIndex(length));
    }
}

public static class IndexExtensions
{
    public static int ToIndex(this Index index, int length) => index.IsFromEnd ? length - index.Value : index.Value;
}

#endif

