using System.Globalization;
using System.IO.Compression;

namespace MathCore.Interpolation;
public class InterpolatorNDLinear
{
    public static InterpolatorNDLinear LoadCSV(FileInfo file, bool Header = true, char Separator = ';', bool SkipWrongLines = true)
    {
        if (string.Equals(file.Extension, ".zip", StringComparison.OrdinalIgnoreCase))
        {
            using var zip = ZipFile.OpenRead(file.FullName);
            var entry = zip.Entries.Count switch
            {
                0 => throw new InvalidOperationException("Пустой архив"),
                1 => zip.Entries[0],
                _ => zip.Entries.FirstOrDefault(e => string.Equals(Path.GetExtension(e.Name), ".csv", StringComparison.OrdinalIgnoreCase))
                  ?? zip.Entries.FirstOrDefault(e => string.Equals(Path.GetExtension(e.Name), ".txt", StringComparison.OrdinalIgnoreCase))
                  ?? zip.Entries[0]
            };
            using var reader = entry.Open().GetStreamReader();
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
        }

        if (string.Equals(file.Extension, ".gzip", StringComparison.OrdinalIgnoreCase))
        {
            using var file_stream = file.OpenRead();
            using var gzip_stream = new GZipStream(file_stream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip_stream);
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
        }

        using (var reader = file.OpenText())
            return LoadCSV(reader, Header, Separator, SkipWrongLines);
    }

    public static InterpolatorNDLinear LoadCSV(TextReader reader, bool Header = true, char Separator = ';', bool SkipWrongLines = true)
    {
        if (Header)
            if (reader.ReadLine() is null)
                throw new InvalidOperationException("Отсутствуют данные для загрузки");

        var arguments_list = new List<double[]>(1000);
        var values_list = new List<double>(1000);

        string line;

        var line_index = Header ? 1 : 0;
        var arguments_count = 0;
        do
        {
            line = reader.ReadLine() ?? throw new InvalidOperationException("Отсутствуют данные для загрузки");
            line_index++;

            arguments_count = line.CountChar(Separator);
            if (!SkipWrongLines && arguments_count == 0)
                throw new InvalidOperationException("Отсутствуют данные для загрузки");
        }
        while (line.Length == 0 && arguments_count == 0);

        var min = new double[arguments_count];
        var max = new double[arguments_count];

        do
        {
            var line_ptr = line.AsStringPtr();

            var args = new double[arguments_count];
            var value = 0d;

            var error_line = false;
            var i = 0;
            foreach (var s in line_ptr.Split(Separator))
            {
                if (!s.TryParseDouble(CultureInfo.CurrentCulture, out var v) && !s.TryParseDouble(CultureInfo.InvariantCulture, out v))
                {
                    if (!SkipWrongLines)
                        throw new InvalidOperationException($"Ошибка формата файла в строке {line_index}: невозможно прочитать вещественное число из значения {i} ({s.ToString()}");

                    error_line = true;
                    break;
                }

                if (i < arguments_count)
                {
                    args[i] = v;

                    min[i] = Math.Min(min[i], v);
                    max[i] = Math.Max(max[i], v);

                    i++;
                }
                else
                {
                    value = v;
                    break;
                }
            }

            if (error_line)
            {
                line_index++;
                continue;
            }

            arguments_list.Add(args);
            values_list.Add(value);

            line_index++;
        }
        while ((line = reader.ReadLine()) is not null);

        for (var i = 0; i < arguments_count; i++)
            max[i] -= min[i];



        return new();
    }

    private InterpolatorNDLinear()
    {
        
    }
}
