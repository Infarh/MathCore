#nullable enable
using System;
using System.Collections.Generic;

namespace ConsoleTest;

public static class CSVParseTest
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

    public delegate void ProcessSpan(Span<char> str);

    public static void ParseLine(Span<char> Str, ProcessSpan Processor, char Separator = ',')
    {
        const char quote = '"';

        int end;
        for (var str = Str; str.Length > 0; str = str[(end + 1)..])
        {
            end = str.IndexOf(Separator);
            if (end < 0)
            {
                Processor(str);
                return;
            }

            if (str[..end].IndexOf(quote) is >= 0 and var q_index && q_index < str.Length - 1)
            {
                var other_str = str[(end + 1)..];
                if (other_str.IndexOf(quote) is >= 0 and var next_q_index &&
                    other_str[(next_q_index + 1)..].IndexOf(Separator) is >= 0 and var comma_index)
                    end += next_q_index + comma_index + 2;
                else
                {
                    Processor(str);
                    return;
                }
            }

            Processor(str[..end]);
        }
    }

    public static IEnumerable<Memory<char>> ParseLine(Memory<char> Str, char Separator = ',')
    {
        const char quote = '"';

        int end;
        for (var str = Str; str.Length > 0; str = str[(end + 1)..])
        {
            end = str.Span.IndexOf(Separator);
            if (end < 0)
            {
                yield return str;
                break;
            }

            if (str[..end].Span.IndexOf(quote) is >= 0 and var q_index && q_index < str.Length - 1)
            {
                var other_str = str[(end + 1)..];
                if (other_str.Span.IndexOf(quote) is >= 0 and var next_q_index &&
                    other_str[(next_q_index + 1)..].Span.IndexOf(Separator) is >= 0 and var comma_index)
                    end += next_q_index + comma_index + 2;
                else
                {
                    yield return str;
                    break;
                }
            }

            yield return str[..end];
        }
    }
}