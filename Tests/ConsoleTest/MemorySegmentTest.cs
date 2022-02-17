using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;

namespace ConsoleTest;

public class MemorySegmentTest
{
    public static void Run()
    {
        string[] lines =
        {
            "123.",
            "..\"Hello,.World!,.QWE\".",
            ".Value.",
            "\"123,23",
        };

        var line = string.Join(',', lines);
        var line_array = line.ToCharArray().AsMemory();
        var values = CSVParseTest.ParseLine(line_array).ToList();

        var segment_start = new MemorySegment<char>(values[1]);
        var segment_end = segment_start.Append(values[3]);

        var pp = new Pipe();

        Writer(pp.Writer);

        static void Writer(PipeWriter writer)
        {

        }

        var seq = new ReadOnlySequence<char>(segment_start, 0, segment_end, segment_end.Memory.Length);

        var sss = seq.ToString();

        Debug.WriteLine(sss);
    }
}