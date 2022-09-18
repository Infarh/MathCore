using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace MathCore.Tests.IO
{
    public class HashStream : Stream
    {
        protected readonly Stream _Source;
        protected readonly HashAlgorithm _Hasher;

        public byte[] Hash => _Hasher.Hash;

        public override bool CanRead => _Source.CanRead;

        public override bool CanSeek => _Source.CanSeek;

        public override bool CanWrite => _Source.CanWrite;

        public override long Length => _Source.Length;

        public override long Position
        {
            get => _Source.Position;
            set => _Source.Position = value;
        }

        public HashStream(Stream Source, HashAlgorithm Hasher)
        {
            _Source = Source;
            _Hasher = Hasher;
        }

        public override long Seek(long offset, SeekOrigin origin) => _Source.Seek(offset, origin);

        public override void SetLength(long value) => _Source.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count)
        {
            var ret = _Source.Read(buffer, offset, count);
            _Hasher.TransformBlock(buffer, offset, ret, buffer, offset);
            return ret;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _Source.Write(buffer, offset, count);
            _Hasher.TransformBlock(buffer, offset, count, buffer, offset);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken Cancel)
        {
            var readed = await _Source.ReadAsync(buffer, offset, count, Cancel);
            if(readed > 0)
                _Hasher.TransformBlock(buffer, offset, readed, buffer, offset);
            return readed;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken Cancel)
        {
            Cancel.ThrowIfCancellationRequested();
            var write_task = _Source.WriteAsync(buffer, offset, count, Cancel);
            _Hasher.TransformBlock(buffer, offset, count, buffer, offset);
            await write_task.ConfigureAwait(false);
        }

        public override void Flush() => _Source.Flush();
    }
}
