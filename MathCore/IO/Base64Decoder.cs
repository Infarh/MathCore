using System.Runtime.InteropServices;
using System.Text;

namespace MathCore.IO;

public class Base64Decoder(BinaryWriter Writer) : TextWriter
{
    private static readonly byte[] __Decoder =
    [
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0062, 0xff, 0xff, 0xff, 0063,
        0052, 0053, 0054, 0055, 0056, 0057, 0058, 0059, 0060, 0061, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0000, 0001, 0002, 0003, 0004, 0005, 0006, 0007, 0008, 0009, 0010, 0011, 0012, 0013, 0014,
        0015, 0016, 0017, 0018, 0019, 0020, 0021, 0022, 0023, 0024, 0025, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0026, 0027, 0028, 0029, 0030, 0031, 0032, 0033, 0034, 0035, 0036, 0037, 0038, 0039, 0040,
        0041, 0042, 0043, 0044, 0045, 0046, 0047, 0048, 0049, 0050, 0051, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
        0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
    ];

    //                                               111111111122222222223333333333444444444455555555556666
    //                                     0123456789012345678901234567890123456789012345678901234567890123
    //private const string __DecoderStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    //private static readonly byte[] __Decoder = new byte[256];
    //static Base64Decoder()
    //{
    //    Array.Fill(__Decoder, (byte)0xFF);
    //    for (var (i, len) = ((byte)0, __DecoderStr.Length); i < len; i++)
    //        __Decoder[__DecoderStr[i]] = i;

    //    var s = $@"[{string.Join(", ", __Decoder.Select(v => v == 0xff ? "0xff" : v.ToString("0000")))}]";
    //}

    private int _Buffer;
    private int _Bits;
    private const int __WriteBufferLength = 3;
    private readonly byte[] _WriteBuffer = new byte[__WriteBufferLength];

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char[] str)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        foreach (var c in str)
        {
            if (c == '=')
            {
                Complete();
                break;
            }

            var index = (byte)c;
            var b = __Decoder[index];
            if (b == 0xFF) throw new InvalidOperationException();

            _Buffer = _Buffer << 6 | b;
            _Bits += 6;

            if (_Bits == 24)
                FlushBuffer();
        }
    }

#if NET8_0_OR_GREATER
    public override void Write(ReadOnlySpan<char> str)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        foreach (var c in str)
        {
            if (c == '=')
            {
                Complete();
                break;
            }

            var index = (byte)c;
            var b = __Decoder[index];
            if (b == 0xFF) throw new InvalidOperationException();

            _Buffer = _Buffer << 6 | b;
            _Bits += 6;

            if (_Bits == 24)
                FlushBuffer();
        }
    }

    public override async Task WriteAsync(ReadOnlyMemory<char> str, CancellationToken Cancel = default)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        for (var i = 0; i < str.Length; i++)
        {
            var c = str.Span[i];

            if (c == '=')
            {
                await CompleteAsync(Cancel).ConfigureAwait(false);
                break;
            }

            var index = (byte)c;
            var b = __Decoder[index];
            if (b == 0xFF) throw new InvalidOperationException();

            _Buffer = _Buffer << 6 | b;
            _Bits += 6;

            if (_Bits == 24)
                await FlushBufferAsync(Cancel).ConfigureAwait(false);
        }
    }
#endif

    public override async Task WriteAsync(char[] str, int Offset, int Count)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        for (var i = 0; i < Count; i++)
        {
            var c = str[Offset + i];

            if (c == '=')
            {
                await CompleteAsync().ConfigureAwait(false);
                break;
            }

            var index = (byte)c;
            var b = __Decoder[index];
            if (b == 0xFF) throw new InvalidOperationException();

            _Buffer = _Buffer << 6 | b;
            _Bits += 6;

            if (_Bits == 24)
                await FlushBufferAsync(default).ConfigureAwait(false);
        }
    }

    // 0 0 0 0 0 0 0 1|0 0 0 0 0 0 1 0|0 0 0 0 0 0 1 1 = 66_051
    // ^ ^ ^ ^ ^ ^|^ ^ ^ ^ ^ ^|^ ^ ^ ^ ^ ^|^ ^ ^ ^ ^ ^
    //      0     |    16     |     8     |     3     
    //      A     |     Q     |     I     |     D     

    private void CopyBufferTo(byte[] bytes)
    {
#if NET8_0_OR_GREATER
        var data = MemoryMarshal.Cast<int, byte>(MemoryMarshal.CreateSpan(ref _Buffer, 1))[..3];
        for (var i = 0; i < 3; i++)
            bytes[i] = data[2 - i];
#else
        bytes[3] = (byte)(_Buffer >> 16 & 0xFF);
        bytes[2] = (byte)(_Buffer >> 08 & 0xFF);
        bytes[1] = (byte)(_Buffer >> 00 & 0xFF);
#endif
    }

    private void FlushBuffer()
    {
        CopyBufferTo(_WriteBuffer);

        Writer.Write(_WriteBuffer);
        Writer.Flush();

        _Buffer = 0;
        _Bits = 0;
    }

    private async Task FlushBufferAsync(CancellationToken Cancel)
    {
        CopyBufferTo(_WriteBuffer);

#if NET8_0_OR_GREATER
        await Writer.BaseStream.WriteAsync(_WriteBuffer, Cancel).ConfigureAwait(false);
#else
        await Writer.BaseStream.WriteAsync(_WriteBuffer, 0, __WriteBufferLength, Cancel).ConfigureAwait(false);
#endif

        _Buffer = 0;
        _Bits = 0;
    }

    public void Complete()
    {
        switch (_Bits)
        {
            default: throw new InvalidOperationException();

            case <= 0: return;

            case 12:
                {
                    if ((_Buffer & 0b1111) != 0) throw new FormatException();

                    var b0 = (byte)(_Buffer >> 4 & 0xFF);
                    Writer.Write(b0);
                }
                break;

            case 18:
                {
                    if ((_Buffer & 0b11) != 0) throw new FormatException();

                    _WriteBuffer[0] = (byte)(_Buffer >> 10 & 0xFF);
                    _WriteBuffer[1] = (byte)(_Buffer >> 02 & 0xFF);

                    Writer.BaseStream.Write(_WriteBuffer, 0, 2);
                }
                break;

            case 24:
                FlushBuffer();
                break;
        }

        _Bits = -1;
    }

    public async Task CompleteAsync(CancellationToken Cancel = default)
    {
        switch (_Bits)
        {
            default: throw new InvalidOperationException();

            case <= 0: return;

            case 12:
                {
                    if ((_Buffer & 0b1111) != 0) throw new FormatException();

                    _WriteBuffer[0] = (byte)(_Buffer >> 4 & 0xFF);
#if NET8_0_OR_GREATER
                    await Writer.BaseStream.WriteAsync(_WriteBuffer.AsMemory(0, 1), Cancel).ConfigureAwait(false);
#else
                    await Writer.BaseStream.WriteAsync(_WriteBuffer, 0, 1, Cancel).ConfigureAwait(false);
#endif
                }
                break;

            case 18:
                {
                    if ((_Buffer & 0b11) != 0) throw new FormatException();

                    _WriteBuffer[0] = (byte)(_Buffer >> 10 & 0xFF);
                    _WriteBuffer[1] = (byte)(_Buffer >> 02 & 0xFF);

#if NET8_0_OR_GREATER
                    await Writer.BaseStream.WriteAsync(_WriteBuffer.AsMemory(0, 2), Cancel).ConfigureAwait(false);
#else
                    await Writer.BaseStream.WriteAsync(_WriteBuffer, 0, 2, Cancel).ConfigureAwait(false);
#endif
                }
                break;

            case 24:
                await FlushBufferAsync(Cancel).ConfigureAwait(false);
                break;
        }

        _Bits = -1;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            Complete();
    }

#if NET8_0_OR_GREATER
    public override async ValueTask DisposeAsync() => await CompleteAsync();
#endif
}
