namespace MathCore.IO;

public class Base64Encoder(TextWriter Writer) :
    IDisposable
#if NET8_0_OR_GREATER
    , IAsyncDisposable
#endif
{
    private static readonly char[] __Encoder =
    [
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
        'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
        'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
        'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
        'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
        'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z', '0', '1', '2', '3',
        '4', '5', '6', '7', '8', '9', '+', '/'
    ];

    private int _Buffer;
    private sbyte _Bits;
    private readonly char[] _WriteBuffer = new char[4];

#if  NET8_0_OR_GREATER
    public void Write(ReadOnlySpan<byte> bytes)
#else
    public void Write(byte[] bytes)
#endif
    {
        if (_Bits < 0) throw new InvalidOperationException();

        foreach (var b in bytes)
        {
            _Buffer = _Buffer << 8 | b;
            _Bits += 8;

            if (_Bits == 24)
                FlushBuffer();
        }
    }

#if NET8_0_OR_GREATER
    public async Task WriteAsync(ReadOnlyMemory<byte> bytes, CancellationToken Cancel = default)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        for (var i = 0; i < bytes.Span.Length; i++)
        {
            _Buffer = _Buffer << 8 | bytes.Span[i];
            _Bits += 8;

            if (_Bits == 24)
                await FlushBufferAsync(Cancel);
        }
    }
#else
    public async Task WriteAsync(byte[] bytes, CancellationToken Cancel = default)
    {
        if (_Bits < 0) throw new InvalidOperationException();

        foreach (var b in bytes)
        {
            _Buffer = _Buffer << 8 | b;
            _Bits += 8;

            if (_Bits == 24)
                await FlushBufferAsync(Cancel);
        }
    }
#endif

    private void FlushBuffer()
    {
        for (var i = 0; i < 4; i++)
        {
            var ch_index = (_Buffer >> 18) & 0b11_1111;
            _Buffer <<= 6;

            _WriteBuffer[i] = __Encoder[ch_index];
        }

        Writer.Write(_WriteBuffer);
        _Buffer = 0;
        _Bits = 0;
    }

    private async Task FlushBufferAsync(CancellationToken Cancel)
    {
        for (var i = 0; i < 4; i++)
        {
            var ch_index = (_Buffer >> 18) & 0b11_1111;
            _Buffer <<= 6;

            _WriteBuffer[i] = __Encoder[ch_index];
        }

#if NET8_0_OR_GREATER
        await Writer.WriteAsync(_WriteBuffer, Cancel).ConfigureAwait(false); 
#else
        Cancel.ThrowIfCancellationRequested();
        await Writer.WriteAsync(_WriteBuffer).ConfigureAwait(false);
#endif
        _Buffer = 0;
        _Bits = 0;
    }

    public void Complete()
    {
        switch (_Bits)
        {
            default: throw new InvalidOperationException();

            case 0:
                break;

            case 8:

                _WriteBuffer[0] = __Encoder[_Buffer >> 2 & 0b1111_11];
                _WriteBuffer[1] = __Encoder[(_Buffer & 0b11) << 4];
                _WriteBuffer[2] = '=';
                _WriteBuffer[3] = '=';

                Writer.Write(_WriteBuffer);
                Writer.Flush();

                break;

            case 16:

                _WriteBuffer[0] = __Encoder[_Buffer >> 10 & 0b111111];
                _WriteBuffer[1] = __Encoder[_Buffer >> 04 & 0b111111];
                _WriteBuffer[2] = __Encoder[_Buffer << 02 & 0b111111];
                _WriteBuffer[3] = '=';

                Writer.Write(_WriteBuffer);
                Writer.Flush();

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

            case 0:
                break;

            case 8:

                _WriteBuffer[0] = __Encoder[_Buffer >> 2 & 0b1111_11];
                _WriteBuffer[1] = __Encoder[(_Buffer & 0b11) << 4];
                _WriteBuffer[2] = '=';
                _WriteBuffer[3] = '=';

#if NET8_0_OR_GREATER
                await Writer.WriteAsync(_WriteBuffer, Cancel).ConfigureAwait(false);
                await Writer.FlushAsync(Cancel).ConfigureAwait(false);
#else
                Cancel.ThrowIfCancellationRequested();
                await Writer.WriteAsync(_WriteBuffer).ConfigureAwait(false);
                Cancel.ThrowIfCancellationRequested();
                await Writer.FlushAsync().ConfigureAwait(false);
#endif

                break;

            case 16:

                _WriteBuffer[0] = __Encoder[_Buffer >> 10 & 0b111111];
                _WriteBuffer[1] = __Encoder[_Buffer >> 04 & 0b111111];
                _WriteBuffer[2] = __Encoder[_Buffer << 02 & 0b111111];
                _WriteBuffer[3] = '=';

#if NET8_0_OR_GREATER
                await Writer.WriteAsync(_WriteBuffer, Cancel).ConfigureAwait(false);
                await Writer.FlushAsync(Cancel).ConfigureAwait(false);
#else
                Cancel.ThrowIfCancellationRequested();
                await Writer.WriteAsync(_WriteBuffer).ConfigureAwait(false);
                Cancel.ThrowIfCancellationRequested();
                await Writer.FlushAsync().ConfigureAwait(false);
#endif

                break;

            case 24:
                await FlushBufferAsync(Cancel).ConfigureAwait(false);
                break;
        }

        _Bits = -1;
    }

    public void Dispose() => Complete();

#if NET8_0_OR_GREATER
    public async ValueTask DisposeAsync() => await CompleteAsync().ConfigureAwait(false); 
#endif
}
