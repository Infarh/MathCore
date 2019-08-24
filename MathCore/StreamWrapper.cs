using System.IO;

namespace System
{
    public class StreamWrapper : Stream
    {
        public class SeekArgs : EventArgs
        {
            public long Offset { get; }
            public SeekOrigin Origin { get; }
            public long Result { get; }

            public SeekArgs(long Offset, SeekOrigin Origin, long Result)
            {
                this.Offset = Offset;
                this.Origin = Origin;
                this.Result = Result;
            }
        }

        public class SetLengthArgs : EventArgs
        {
            public long Length { get; private set; }
            public SetLengthArgs(long Length) { this.Length = Length; }
        }

        public class ReadArgs : EventArgs
        {
            public byte[] Buffer { get; }
            public int Offset { get; }
            public int Count { get; }
            public int Result { get; }

            public ReadArgs(byte[] Buffer, int Offset, int Count, int Result)
            {
                this.Buffer = Buffer;
                this.Offset = Offset;
                this.Count = Count;
                this.Result = Result;
            }
        }

        public class WriteArgs : EventArgs
        {
            public byte[] Buffer { get; }
            public int Offset { get; }
            public int Count { get; }

            public WriteArgs(byte[] Buffer, int Offset, int Count)
            {
                this.Buffer = Buffer;
                this.Offset = Offset;
                this.Count = Count;
            }
        }

        public class PositionChangedArgs : EventArgs
        {
            public long OldPosition { get; }
            public long NewPosition { get; }

            public PositionChangedArgs(long OldPosition, long NewPosition)
            {
                this.OldPosition = OldPosition;
                this.NewPosition = NewPosition;
            }
        }

        public event EventHandler OnFlush;
        public event EventHandler<SeekArgs> OnSeek;
        public event EventHandler<SetLengthArgs> OnSetLength;
        public event EventHandler<ReadArgs> OnRead;
        public event EventHandler<WriteArgs> OnWrite;
        public event EventHandler<PositionChangedArgs> OnPositionChanged;
        public event EventHandler OnEndOfStream;

        private readonly Stream _DataStream;

        public StreamWrapper(Stream Source) { _DataStream = Source; }


        public override void Flush()
        {
            _DataStream.Flush();
            OnFlush?.Invoke(this, EventArgs.Empty);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var result = _DataStream.Seek(offset, origin);
            OnSeek?.Invoke(this, new SeekArgs(offset, origin, result));
            return result;
        }

        public override void SetLength(long value)
        {
            _DataStream.SetLength(value);
            OnSetLength?.Invoke(this, new SetLengthArgs(value));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = _DataStream.Read(buffer, offset, count);
            OnRead?.Invoke(this, new ReadArgs(buffer, offset, count, result));
            if(EndOfStream) OnEndOfStream?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _DataStream.Write(buffer, offset, count);
            OnWrite?.Invoke(this, new WriteArgs(buffer, offset, count));
        }

        public override bool CanRead => _DataStream.CanRead;

        public override bool CanSeek => _DataStream.CanSeek;

        public override bool CanWrite => _DataStream.CanWrite;

        public override long Length => _DataStream.Length;

        public override long Position
        {
            get { return _DataStream.Position; }
            set
            {
                var old = _DataStream.Position;
                _DataStream.Position = value;
                OnPositionChanged?.Invoke(this, new PositionChangedArgs(old, value));
                if(EndOfStream) OnEndOfStream?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool EndOfStream => _DataStream.Position == _DataStream.Length;

        public double Progress => (double)_DataStream.Position / _DataStream.Length;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
                _DataStream.Dispose();
        }
    }
}