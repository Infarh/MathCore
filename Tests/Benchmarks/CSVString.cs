using System.Collections.Generic;

namespace Benchmarks;

public class CSVString
{
    public static int Parse(string Line, char Separator = ',')
    {
        var i = 0;
        var result = 0;
        foreach (var item in ParseLine(Line, Separator))
            result += i++ switch
            {
                0 => int.Parse(item),
                _ => item.Length
            };

        return result;
    }

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
}