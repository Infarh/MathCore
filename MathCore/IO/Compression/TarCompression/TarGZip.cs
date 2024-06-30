﻿using System.Collections;
using System.IO.Compression;

namespace MathCore.IO.Compression.TarCompression;

/// <summary>Класс для работы с архивами Tar со сжатием GZIP</summary>
public partial class TarGZip(string Name) : IEnumerable<TarGZip.Entry>
{
    public static TarGZip Open(string FileName) => new(FileName);

    private static IEnumerable<Entry> EnumerateEntries(string FileName)
    {
        if (!File.Exists(FileName)) throw new FileNotFoundException("Файл архива не найден", FileName);
        using var file_stream = File.OpenRead(FileName);
        using var reader = new BinaryReader(new GZipStream(file_stream, CompressionMode.Decompress));
        while (file_stream.Position < file_stream.Length)
        {
            var entry = new Entry(reader);
            entry.ReadData();
            yield return entry;
        }
    }

    public string FileName => Name;

    /// <inheritdoc />
    public IEnumerator<Entry> GetEnumerator() =>
        // ReSharper disable once NotDisposedResourceIsReturned
        !File.Exists(Name)
            ? throw new FileNotFoundException("Файл архива не найден", Name)
            : EnumerateEntries(Name).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}