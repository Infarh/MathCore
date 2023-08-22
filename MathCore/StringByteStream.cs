#nullable enable
using System.Text;

namespace MathCore;

public class StringByteStream(string Str, Encoding? encoding = null) : Stream
{
    private readonly Encoding _Encoding = encoding ?? Encoding.UTF8;
    private readonly int _StringLength = Str.Length;
    private int _StringOffset;
    private long _ByteOffset;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length { get; } = encoding.GetByteCount(Str);

    public override long Position
    {
        get => _ByteOffset;
        set => throw new NotSupportedException();
    }

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_StringOffset >= _StringLength) return 0;

        var str_tail_length = _StringLength - _StringOffset;
        var buffer_chars_count = encoding.GetCharCount(buffer, offset, count);
        var char_count = Math.Min(str_tail_length, buffer_chars_count);

        var byte_count = _Encoding.GetBytes(Str, _StringOffset, char_count, buffer, offset);

        _StringOffset += char_count;
        _ByteOffset += byte_count;
        return byte_count;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}