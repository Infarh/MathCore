namespace Benchmarks;

public class CSVSplit
{
    public static int Parse(string Line, char Separator = ',')
    {
        var items = Line.Split(Separator);
        var result = int.Parse(items[0]);
        for (var i = 1; i < items.Length; i++)
            result += items[i].Length;
        return result;
    }
}