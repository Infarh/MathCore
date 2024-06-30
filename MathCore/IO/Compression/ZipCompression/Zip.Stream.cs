namespace MathCore.IO.Compression.ZipCompression;

public sealed partial class Zip
{
    private sealed class ArchiveStream(Stream stream, long FLength) : Stream
    {
        private long _Position;

        #region override Stream

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => FLength;

        /// <inheritdoc />
        public override long Position
        {
            get => _Position;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Flush() => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc />
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            var data_left_count = FLength - _Position;
            _Position += count = stream.Read(buffer, offset, data_left_count >= count ? count : (int)data_left_count);
            return count;
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public event EventHandler Disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing) Disposed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}