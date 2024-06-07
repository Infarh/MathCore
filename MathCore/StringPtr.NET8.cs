#if NET8_0_OR_GREATER

namespace MathCore;

public readonly ref partial struct StringPtr
{
    public ReadOnlySpan<char> Span => Source.AsSpan(Pos, Length);

    public ReadOnlyMemory<char> Memory => Source.AsMemory(Pos, Length);

    public StringPtr this[Range range]
    {
        get
        {
            var start_index = range.Start;
            var end_index = range.End;


            var start = start_index.IsFromEnd ? Length - start_index.Value : start_index.Value;
            var end = end_index.IsFromEnd ? Length - end_index.Value : end_index.Value;

            var length = end - start;
            return this[start, length];
        }
    }

    public char this[Index index] => this[index.Value];
}

#endif