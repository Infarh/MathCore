namespace ConsoleTest.Extensions;

public static class StrEx
{
    public static ReadOnlySpan<char> SplitSpan(this string s, char Separator, out int EndIndex, int StartIndex = 0)
    {
        if (StartIndex >= s.Length)
        {
            EndIndex = StartIndex;
            return [];
        }

        var index = s.IndexOf(Separator, StartIndex);
        if (index < 0)
        {
            EndIndex = s.Length;
            return s.AsSpan()[StartIndex..];
        }

        EndIndex = index;
        var length = EndIndex - StartIndex;
        return length > 1
            ? s.AsSpan().Slice(StartIndex, length - 1)
            : [];
    }

    public static ReadOnlyMemory<char> SplitMemory(this string s, char Separator, out int EndIndex, int StartIndex = 0)
    {
        if (StartIndex >= s.Length)
        {
            EndIndex = StartIndex;
            return ReadOnlyMemory<char>.Empty;
        }

        var index = s.IndexOf(Separator, StartIndex);
        if (index < 0)
        {
            EndIndex = s.Length;
            return s.AsMemory()[StartIndex..];
        }

        EndIndex = index + 1;
        var length = EndIndex - StartIndex;
        return length > 1
            ? s.AsMemory().Slice(StartIndex, length - 1)
            : ReadOnlyMemory<char>.Empty;
    }

    public static IEnumerable<ReadOnlyMemory<char>> EnumFragments(this string s, char Separator)
    {
        if(s.Length == 0)
            yield break;
        var index = 0;
        while (index < s.Length) 
            yield return s.SplitMemory(Separator, out index, index);
    }
}