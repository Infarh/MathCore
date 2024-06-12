#nullable enable

using System.Text;

namespace MathCore;

public static class IniFile
{
    public record struct Value(string Category, string Name, string Data);

    public static IEnumerable<Value> Read(FileInfo file, Encoding? encoding = null)
    {
        var category = string.Empty;
        foreach (var line in file.GetStringLines(encoding ?? Encoding.UTF8).Where(l => l is { Length: > 2 } and not [';', ..]))
        {
            if (line.AsStringPtr().Trim().IsInBracket('[', ']'))
            {
                category = line.AsStringPtr(1, -1);
                continue;
            }

            var index = line.IndexOf('=');
            if (index < 0) continue;

            var name = line.AsStringPtr(0, index).Trim().ToString();
            var value = line.AsStringPtr(index + 1).Trim().ToString();

            yield return new(category, name, value);
        }
    }
}
