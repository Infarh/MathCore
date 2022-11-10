#nullable enable
using System.IO;
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Объект с интерфейсом потока байт данных, обеспечивающий слежение за процессами чтения/записи данных</summary>
public class StreamWrapper : Stream
{
    /// <summary>Аргумент события <see cref="StreamWrapper.OnSeek"/></summary>
    public readonly struct SeekArgs
    {
        /// <summary>Величина заданного смещения в потоке</summary>
        public long Offset { get; }

        /// <summary>Направление смещения в потоке</summary>
        public SeekOrigin Origin { get; }

        /// <summary>Результат выполнения смещения в потоке</summary>
        public long Result { get; }

        /// <summary>Инициализация нового экземпляра <see cref="SeekArgs"/></summary>
        /// <param name="Offset">Величина заданного смещения в потоке</param>
        /// <param name="Origin">Направление смещения в потоке</param>
        /// <param name="Result">Результат выполнения смещения в потоке</param>
        public SeekArgs(long Offset, SeekOrigin Origin, long Result)
        {
            this.Offset = Offset;
            this.Origin = Origin;
            this.Result = Result;
        }

        public void Deconstruct(out long Offset, out SeekOrigin Origin, out long Result)
        {
            Offset = this.Offset;
            Origin = this.Origin;
            Result = this.Result;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnSetLength"/></summary>
    public readonly struct SetLengthArgs
    {
        /// <summary>Предыдущая длина потока</summary>
        public long OldLength { get; }

        /// <summary>Установленная длина потока</summary>
        public long Length { get; }

        /// <summary>Инициализация нового экземпляра <see cref="SetLengthArgs"/></summary>
        /// <param name="OldLength">Предыдущая длина потока</param>
        /// <param name="Length">Установленная длина потока</param>
        public SetLengthArgs(long OldLength, long Length)
        {
            this.OldLength = OldLength;
            this.Length    = Length;
        }

        public void Deconstruct(out long OldLength, out long Length)
        {
            OldLength = this.OldLength;
            Length    = this.Length;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnRead"/></summary>
    public readonly struct ReadArgs
    {
        /// <summary>Прочитанный массив байт</summary>
        public byte[] Buffer { get; }

        /// <summary>Смещение в массиве</summary>
        public int Offset { get; }

        /// <summary>Число читаемых байт</summary>
        public int Count { get; }

        /// <summary>Число прочитанных байт</summary>
        public int Result { get; }

        /// <summary>Инициализация нового экземпляра <see cref="ReadArgs"/></summary>
        /// <param name="Buffer">Прочитанный массив байт</param>
        /// <param name="Offset">Смещение в массиве</param>
        /// <param name="Count">Число читаемых байт</param>
        /// <param name="Result">Число прочитанных байт</param>
        public ReadArgs(byte[] Buffer, int Offset, int Count, int Result)
        {
            this.Buffer = Buffer;
            this.Offset = Offset;
            this.Count  = Count;
            this.Result = Result;
        }

        public void Deconstruct(out byte[] Buffer, out int Offset, out int Count, out int Result)
        {
            Buffer = this.Buffer;
            Offset = this.Offset;
            Count  = this.Count;
            Result = this.Result;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnWrite"/></summary>
    public readonly struct WriteArgs
    {
        /// <summary>Записываемый массив байт</summary>
        public byte[] Buffer { get; }

        /// <summary>Смещение в массиве</summary>
        public int Offset { get; }

        /// <summary>Число записываемых байт</summary>
        public int Count { get; }

        /// <summary>Инициализация нового экземпляра <see cref="WriteArgs"/></summary>
        /// <param name="Buffer">Записываемый массив байт</param>
        /// <param name="Offset">Смещение в массиве</param>
        /// <param name="Count">Число записываемых байт</param>
        public WriteArgs(byte[] Buffer, int Offset, int Count)
        {
            this.Buffer = Buffer;
            this.Offset = Offset;
            this.Count  = Count;
        }

        public void Deconstruct(out byte[] Buffer, out int Offset, out int Count)
        {
            Buffer = this.Buffer;
            Offset = this.Offset;
            Count  = this.Count;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnPositionChanged"/></summary>
    public readonly struct PositionChangedArgs
    {
        /// <summary>Предыдущее положение в потоке данных</summary>
        public long OldPosition { get; }

        /// <summary>Новое положение в потоке данных</summary>
        public long NewPosition { get; }

        /// <summary>Инициализация нового экземпляра <see cref="PositionChangedArgs"/></summary>
        /// <param name="OldPosition">Предыдущее положение в потоке данных</param>
        /// <param name="NewPosition">Новое положение в потоке данных</param>
        public PositionChangedArgs(long OldPosition, long NewPosition)
        {
            this.OldPosition = OldPosition;
            this.NewPosition = NewPosition;
        }

        public void Deconstruct(out long OldPosition, out long NewPosition)
        {
            OldPosition = this.OldPosition;
            NewPosition = this.NewPosition;
        }
    }

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Событие возникает при вызове метода <see cref="Flush"/></summary>
    public event EventHandler? OnFlush;

    /// <summary>Событие возникает при изменении положения считывания/записи</summary>
    public event EventHandlerRef<SeekArgs>? OnSeek;

    /// <summary>Событие возникает при изменении длины потока</summary>
    public event EventHandlerRef<SetLengthArgs>? OnSetLength;

    /// <summary>Событие возникает при чтении данных из потока</summary>
    public event EventHandlerRef<ReadArgs>? OnRead;

    /// <summary>Событие возникает при записи данных в поток</summary>
    public event EventHandlerRef<WriteArgs>? OnWrite;

    /// <summary>Событие возникает в момент, когда положение чтение/записи в потоке смещается</summary>
    public event EventHandlerRef<PositionChangedArgs>? OnPositionChanged;

    /// <summary>Событие, возникающее когда положение чтение/записи выходит на конец потока</summary>
    public event EventHandler? OnEndOfStream;

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Внутренний поток</summary>
    private readonly Stream _DataStream;

    /// <summary>Ожидаемая длани потока в случае если поток не поддерживает поиск</summary>
    private readonly long _ExplicitLength;

    /// <summary>Положение в потоке (используется если поток не поддерживает поиск)</summary>
    private long _Position;

    /* --------------------------------------------------------------------------------------- */

    /// <summary>Инициализация нового экземпляра <see cref="StreamWrapper"/></summary>
    /// <param name="Source">Поток, доступ к которому требуется контролировать</param>
    /// <param name="ExplicitLength">Если поток не поддерживает поиск, то тут указать его длину (если она определена)</param>
    public StreamWrapper(Stream Source, long ExplicitLength = 0)
    {
        _DataStream     = Source;
        _ExplicitLength = ExplicitLength;
    }

    /* --------------------------------------------------------------------------------------- */

    /// <inheritdoc />
    public override void Flush()
    {
        _DataStream.Flush();
        OnFlush?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        var position = _DataStream.Seek(offset, origin);
        _Position = position;
        OnSeek?.Invoke(this, new(offset, origin, position));
        return position;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        var old_length = _DataStream.Length;
        _DataStream.SetLength(value);
        OnSetLength?.Invoke(this, new(old_length, value));
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        var stream = _DataStream;
        var readed = stream.Read(buffer, offset, count);
        _Position += readed;
        OnRead?.Invoke(this, new(buffer, offset, count, readed));
        if (stream.CanSeek && EndOfStream)
            OnEndOfStream?.Invoke(this, EventArgs.Empty);
        return readed;
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        _DataStream.Write(buffer, offset, count);
        _Position += count;
        OnWrite?.Invoke(this, new(buffer, offset, count));
    }

    /// <summary>Можно ли осуществлять чтение данных из потока (определяется внутренним потоком)</summary>
    public override bool CanRead => _DataStream.CanRead;

    /// <summary>Можно ли осуществлять перемещение позиции чтения/записи в потоке (определяется внутренним потоком)</summary>
    public override bool CanSeek => _DataStream.CanSeek;

    /// <summary>Можно ли осуществлять запись данных в поток (определяется внутренним потоком)</summary>
    public override bool CanWrite => _DataStream.CanWrite;

    /// <summary>Длина потока</summary>
    public override long Length => _DataStream is { CanSeek: true, Length: var length } ? length : _ExplicitLength;

    /// <summary>Текущее положение в потоке (при установке значения осуществляется проверка на предмет конца потока)</summary>
    public override long Position
    {
        get => _DataStream is { CanSeek: true, Position: var position } ? position : _Position;
        set
        {
            var stream = _DataStream;
            var old    = stream.Position;
            stream.Position = value;
            OnPositionChanged?.Invoke(this, new(old, value));
            if (stream.CanSeek && EndOfStream) 
                OnEndOfStream?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Признак конца потока</summary>
    
    public bool EndOfStream
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _DataStream.Position == _DataStream.Length;
    }

    /// <summary>Прогресс</summary>
    public double Progress => _DataStream is { CanSeek: true, Position: var pos, Length: var len }
        ? (double)pos / len
        : (double)_Position / _ExplicitLength;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _DataStream.Dispose();
    }
}