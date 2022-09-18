namespace Benchmarks;

public class CSVSpan
{
    public static int Parse(string Line, char Separator = ',')
    {
        var i = 0;
        var result = 0;

        void Process(ReadOnlySpan<char> item) =>
            result += i++ switch
            {
                0 => int.Parse(item),
                _ => item.Length
            };

        ParseLine(Line.AsSpan(), Process, Separator);

        return result;
    }

    public delegate void ProcessReadOnlySpan(ReadOnlySpan<char> str);

    public static void ParseLine(ReadOnlySpan<char> Str, ProcessReadOnlySpan Processor, char Separator = ',')
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
}