using System;
using System.ComponentModel;
using System.IO;

namespace MathCore.IO
{
    public class LimitedStream : Stream
    {
        private readonly Stream _Stream;
        private readonly long _Offset;
        private long _LimitedLength;

        public LimitedStream(Stream Stream, long Offset, long LimitedLength)
        {
            _Stream = Stream;
            _Offset = Offset;
            _LimitedLength = LimitedLength;
            _Stream.Seek(Offset, SeekOrigin.Begin);
        }

        public override long Position
        {
            get => Math.Max(0, _Stream.Position - _Offset);
            set => _Stream.Position = value + _Offset;
        }
        public override long Length => Math.Min(_Stream.Length - _Offset, _LimitedLength);

        public override bool CanSeek => _Stream.CanSeek;
        public override bool CanRead => _Stream.CanRead;
        public override bool CanWrite => _Stream.CanWrite;

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!_Stream.CanSeek)
                throw new NotSupportedException();

            long limited_offset, seek_result;
            switch (origin)
            {
                default:
                    throw new InvalidEnumArgumentException(nameof(origin), (int)origin, typeof(SeekOrigin));

                case SeekOrigin.Begin:
                    if (offset < 0)
                        throw new IOException("An attempt was made to move the position before the beginning of the stream.")
                        {
                            Data = { { "offset", offset } }
                        };

                    limited_offset = offset + _Offset;
                    seek_result = _Stream.Seek(limited_offset, SeekOrigin.Begin) - _Offset;
                    return seek_result;

                case SeekOrigin.End:
                    if (-offset > _LimitedLength)
                        throw new IOException("An attempt was made to move the position before the beginning of the stream.")
                        {
                            Data = { { "offset", offset } }
                        };

                    limited_offset = _Offset + _LimitedLength + offset - _Stream.Length;
                    seek_result = _Stream.Seek(limited_offset, SeekOrigin.End) - _Offset;
                    return seek_result;

                case SeekOrigin.Current:
                    return _Stream.Seek(offset, SeekOrigin.Current) - _Offset;
            }
        }

        public override void SetLength(long value)
        {
            _LimitedLength = value;
            var delta = _Stream.Length - _LimitedLength;
            if (delta > 0)
                _Stream.SetLength(delta);
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            Math.Min(Length - Position, count) is var read_length and > 0
                ? _Stream.Read(buffer, offset, (int)read_length)
                : 0;

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Math.Min(Length - Position, count) is var write_length and > 0)
                _Stream.Write(buffer, offset, (int)write_length);
        }

        public override void Flush() => _Stream.Flush();
    }
}
