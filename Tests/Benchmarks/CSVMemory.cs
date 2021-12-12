using System.Collections.Generic;
using System;

namespace Benchmarks;

public class CSVMemory
{
    public static int Parse(string Line, char Separator = ',')
    {
        var i = 0;
        var result = 0;
        foreach (var item in ParseLine(Line.AsMemory(), Separator))
            result += i++ switch
            {
                0 => int.Parse(item.Span),
                _ => item.Length
            };

        return result;
    }

    public static IEnumerable<ReadOnlyMemory<char>> ParseLine(ReadOnlyMemory<char> Str, char Separator = ',')
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