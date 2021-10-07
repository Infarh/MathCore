using System;
using System.ComponentModel;
using System.IO;

namespace MathCore.IO
{
    public class LimitedStream : Stream
    {
        /// <summary>Поток-источник данных</summary>
        private readonly Stream _BaseStream;

        /// <summary>Смещение потока относительного исходного</summary>
        private long _DataOffset;

        /// <summary>Количество байт данных в потоке</summary>
        private long _DataLength;

        /// <summary>Возможность растягивать исходный поток</summary>
        private bool _CanExpand;
        private bool? _CanRead;
        private bool? _CanWrite;

        public bool? StreamCanRead { get => _CanRead; set => _CanRead = value; }
        public bool? StreamCanWrite { get => _CanWrite; set => _CanWrite = value; }

        /// <summary>Поток-источник данных</summary>
        public Stream BaseStream => _BaseStream;

        /// <summary>Смещение потока относительного исходного</summary>
        /// <exception cref="ArgumentOutOfRangeException" accessor="set">Если передано значение меньше нуля.</exception>
        public long DataOffset
        {
            get => _DataOffset;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Смещение не может быть меньше нуля");
                _DataOffset = value;
            }
        }

        public Stream AsReadOnly => new LimitedStream(this) { _CanWrite = false };

        public LimitedStream(Stream BaseStream) : this(BaseStream, 0, BaseStream.Length) { }
        public LimitedStream(Stream BaseStream, long Offset) : this(BaseStream, Offset, BaseStream.Length - Offset) { }
        public LimitedStream(Stream BaseStream, long Offset, long DataLength)
        {
            _BaseStream = BaseStream ?? throw new ArgumentNullException(nameof(BaseStream));
            _DataOffset = Offset;
            _DataLength = DataLength;
        }

        #region Stream inherits


        /// <inheritdoc />
        public override bool CanRead => _CanRead ?? _BaseStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _BaseStream.CanSeek;


        /// <inheritdoc />
        public override bool CanWrite => _CanWrite ?? _BaseStream.CanWrite;

        /// <summary>Возможность растягивать исходный поток</summary>
        public bool CanExpand { get => _CanExpand; set => _CanExpand = value; }

        /// <inheritdoc />
        public override long Length => _DataLength >= 0 ? _DataLength : _BaseStream.Length - _DataOffset;

        /// <inheritdoc />
        public override long Position
        {
            get => Math.Max(0, _BaseStream.Position - _DataOffset);
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Ожидается положительное значениее");
                _BaseStream.Position = _DataOffset + value;
            }
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_DataOffset == 0) return _BaseStream.Seek(offset, origin);

            const SeekOrigin begin = SeekOrigin.Begin;
            const SeekOrigin current = SeekOrigin.Current;
            const SeekOrigin end = SeekOrigin.End;

            switch (origin)
            {
                case begin:
                    if (offset < 0)
                        throw new ArgumentOutOfRangeException(nameof(offset), "Выполнена попытка позиционирования до начала потока");
                    return _BaseStream.Seek(_DataOffset + offset, begin) - _DataOffset;

                case current:
                    if (_BaseStream.Position < _DataOffset)
                        return _BaseStream.Seek(_DataOffset + offset, begin) - _DataOffset;
                    if (_BaseStream.Position + offset < _DataOffset)
                        throw new ArgumentOutOfRangeException(nameof(offset), "Выполнена попытка позиционирования до начала потока");
                    return _BaseStream.Seek(offset, current) - _DataOffset;

                case end:
                    return _BaseStream.Seek(_DataOffset + Length + offset, begin) - _DataOffset;

                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }

        }

        /// <inheritdoc />
        public override void SetLength(long value) => _DataLength = value;

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_CanRead.GetValueOrDefault(true)) throw new NotSupportedException();
            if (_BaseStream.Position < _DataOffset) _BaseStream.Seek(_DataOffset, SeekOrigin.Begin);
            var data_length = Length;
            if (_BaseStream.Position + count > _DataOffset + data_length)
                count -= (int)(_BaseStream.Position + count - (_DataOffset + data_length));
            return count <= 0 ? 0 : _BaseStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!_CanWrite.GetValueOrDefault(true)) throw new NotSupportedException();
            var pos = _BaseStream.Position;
            if (pos < _DataOffset)
                pos = _BaseStream.Seek(_DataOffset, SeekOrigin.Begin);
            if (_DataLength >= 0 && pos + count > _DataOffset + _DataLength)
                _DataLength += _CanExpand
                    ? pos + count - (_DataOffset + _DataLength)
                    : throw new InvalidOperationException("Поток не подлежит расширению");
            if (count > 0)
                _BaseStream.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        public override void Flush() => _BaseStream.Flush();

        /// <inheritdoc />
        public override int ReadByte()
        {
            if (!_CanRead.GetValueOrDefault(true)) throw new NotSupportedException();
            if (_BaseStream.Position < _DataOffset)
                _BaseStream.Seek(_DataOffset, SeekOrigin.Begin);
            else if (_BaseStream.Position >= _DataOffset + _DataLength)
                return -1;
            return _BaseStream.ReadByte();
        }

        /// <inheritdoc />
        public override void WriteByte(byte value)
        {
            if (!_CanWrite.GetValueOrDefault(true)) throw new NotSupportedException();
            var base_pos = _BaseStream.Position;
            if (base_pos < _DataOffset)
                base_pos = _BaseStream.Seek(_DataOffset, SeekOrigin.Begin);
            if (_DataLength >= 0 && ++base_pos > _DataOffset + _DataLength)
                _DataLength += _CanExpand
                    ? base_pos - (_DataOffset + _DataLength)
                    : throw new InvalidOperationException("Поток не подлежит расширению");
            base.WriteByte(value);
        }

        public event EventHandler Disposing;
        public event EventHandler Disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) Disposing?.Invoke(this, EventArgs.Empty);
            base.Dispose(disposing);
            if (disposing) Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
