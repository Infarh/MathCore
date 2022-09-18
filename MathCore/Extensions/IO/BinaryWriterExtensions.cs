#nullable enable
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class BinaryWriterExtensions
{
    public static async Task WriteAsync(this BinaryWriter Writer, double Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }

    public static async Task WriteAsync(this BinaryWriter Writer, float Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }

    public static async Task WriteAsync(this BinaryWriter Writer, long Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }

    public static async Task WriteAsync(this BinaryWriter Writer, int Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }

    public static async Task WriteAsync(this BinaryWriter Writer, short Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }

    public static async Task WriteAsync(this BinaryWriter Writer, bool Value, CancellationToken Cancel = default)
    {
        var buffer = BitConverter.GetBytes(Value);
        await Writer.BaseStream.WriteAsync(buffer, 0, buffer.Length, Cancel).ConfigureAwait(false);
    }
}