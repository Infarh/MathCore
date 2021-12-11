using System;
using System.Collections.Generic;

namespace MathCore.Algorithms.CSV;

public static class Parser
{
    public static IEnumerable<string> ParseLine(string Str, char Separator = ',')
    {
        const char quote = '"';
        for (int start = 0, end; start < Str.Length; start = end + 1)
        {
            end = Str.IndexOf(Separator, start + 1);
            if (end < 0)
                end = Str.Length;

            if (Str.IndexOf(quote, start, end - start) is > 0 and var start_quote_index)
            {
                var close_quote_index = Str.IndexOf(quote, start_quote_index + 1);
                if (close_quote_index < 0) yield break;
                end = Str.IndexOf(Separator, close_quote_index + 1);
            }

            yield return Str[start..end];
        }
    }

    //public static IEnumerable<string> ParseLine(ReadOnlySpan<char> Str, char Separator = ',')
    //{
    //    const char quote = '"';
    //    for (int start = 0, end; start < Str.Length; start = end + 1)
    //    {
    //        end = Str.IndexOf(Separator);
    //        if (end < 0)
    //            end = Str.Length;

    //        var segment = Str[start..end];
    //        if (segment.IndexOf(quote) is > 0 and var start_quote_index)
    //        {
    //            var close_quote_index = Str[(start_quote_index + 1)..].IndexOf(quote);
    //            if (close_quote_index < 0) yield break;
    //            end = Str[(close_quote_index + 1)..].IndexOf(Separator);
    //            segment = Str[start..end];
    //        }

    //        yield return new string(segment);
    //    }
    //}

    //public static IEnumerable<Memory<char>> ParseLine(Memory<char> Str, char Separator = ',')
    //{
    //    const char quote = '"';
    //    var str = Str.Span;
    //    for (int start = 0, end; start < Str.Length; start = end + 1)
    //    {
    //        end = str.IndexOf(Separator);
    //        if (end < 0)
    //            end = Str.Length;

    //        if (str[start..end].IndexOf(quote) is > 0 and var start_quote_index)
    //        {
    //            var close_quote_index = str[(start_quote_index + 1)..].IndexOf(quote);
    //            if (close_quote_index < 0) yield break;
    //            end = str[(close_quote_index + 1)..].IndexOf(Separator);
    //        }

    //        yield return Str[start..end];
    //    }
    //}

    //public static IEnumerable<ReadOnlyMemory<char>> ParseLine(ReadOnlyMemory<char> Str, char Separator = ',')
    //{
    //    const char quote = '"';
    //    var str = Str.Span;
    //    for (int start = 0, end; start < Str.Length; start = end + 1)
    //    {
    //        end = str.IndexOf(Separator);
    //        if (end < 0)
    //            end = Str.Length;

    //        if (str[start..end].IndexOf(quote) is > 0 and var start_quote_index)
    //        {
    //            var close_quote_index = str[(start_quote_index + 1)..].IndexOf(quote);
    //            if (close_quote_index < 0) yield break;
    //            end = str[(close_quote_index + 1)..].IndexOf(Separator);
    //        }

    //        yield return Str[start..end];
    //    }
    //}
}