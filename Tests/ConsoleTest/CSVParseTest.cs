#nullable enable
using System.Collections.Generic;

namespace ConsoleTest
{
    public static class CSVParseTest
    {
        public const string SourceStr = "123 ,  \"Hello, World!, QWE\" , Value";

        public static IEnumerable<string> ParseCSVLine(string Str, char Separator = ',')
        {
            const char quote = '"';
            for(int start = 0, end; start < Str.Length; start = end + 1)
            {
                end = Str.IndexOf(Separator, start + 1);
                if (end < 0)
                    end = Str.Length;

                if (Str.IndexOf(quote, start, end - start) is > 0 and var start_quote_index)
                {
                    var close_quote_index = Str.IndexOf(quote, start_quote_index + 1);
                    if(close_quote_index < 0) yield break;
                    end = Str.IndexOf(Separator, close_quote_index + 1);
                }

                var s = Str[start..end];
                yield return s;
            }
        }
    }
}
