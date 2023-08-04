using System.IO.Compression;

namespace MathCore.Extensions.IO;

public static class ZipArchiveExtensions
{
    public static IEnumerable<string> EnumLines(this ZipArchive zip, Func<ZipArchiveEntry, bool> EntrySelector)
    {
        var entry = zip.Entries.First(EntrySelector);
        using var stream = entry.Open();
        foreach (var line in stream.GetStringLines())
            yield return line;
    }
}
