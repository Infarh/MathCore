#nullable enable
namespace System.IO;

public static class TextWriterExtensions
{
    public static TextWriter WriteLineValues(this TextWriter writer, char Separator, params string[] values)
    {
        if (values.Length == 0) return writer;

        writer.Write(values[0]);
        for(var i = 1; i < values.Length; i++)
        {
            writer.Write(Separator);
            writer.Write(values[i]);
        }

        writer.WriteLine();
        return writer;
    }

    public static async Task<TextWriter> WriteLineValuesAsync(this TextWriter writer, char Separator, params string[] values)
    {
        if (values.Length == 0) return writer;

        await writer.WriteAsync(values[0]).ConfigureAwait(false);
        for(var i = 1; i < values.Length; i++)
        {
            await writer.WriteAsync(Separator).ConfigureAwait(false);
            await writer.WriteAsync(values[i]).ConfigureAwait(false);
        }

        await writer.WriteLineAsync().ConfigureAwait(false);
        return writer;
    }
}
