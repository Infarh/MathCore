#nullable enable
using System.IO;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Объект с интерфейсом потока байт данных, обеспечивающий слежение за процессами чтения/записи данных</summary>
public class StreamWrapper : Stream
{
    /// <summary>Аргумент события <see cref="StreamWrapper.OnSeek"/></summary>
    public class SeekArgs : EventArgs
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
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnSetLength"/></summary>
    public class SetLengthArgs : EventArgs
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
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnRead"/></summary>
    public class ReadArgs : EventArgs
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
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnWrite"/></summary>
    public class WriteArgs : EventArgs
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
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnPositionChanged"/></summary>
    public class PositionChangedArgs : EventArgs
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
    }

    /// <summary>Событие возникает при вызове метода <see cref="Flush"/></summary>
    public event EventHandler? OnFlush;

    /// <summary>Событие возникает при изменении положения считывания/записи</summary>
    public event EventHandler<SeekArgs>? OnSeek;

    /// <summary>Событие возникает при изменении длины потока</summary>
    public event EventHandler<SetLengthArgs>? OnSetLength;

    /// <summary>Событие возникает при чтении данных из потока</summary>
    public event EventHandler<ReadArgs>? OnRead;

    /// <summary>Событие возникает при записи данных в поток</summary>
    public event EventHandler<WriteArgs>? OnWrite;

    /// <summary>Событие возникает в момент, когда положение чтение/записи в потоке смещается</summary>
    public event EventHandler<PositionChangedArgs>? OnPositionChanged;

    /// <summary>Событие, возникающее когда положение чтение/записи выходит на конец потока</summary>
    public event EventHandler? OnEndOfStream;

    private readonly Stream _DataStream;

    /// <summary>Инициализация нового экземпляра <see cref="StreamWrapper"/></summary>
    /// <param name="Source">Поток, доступ к которому требуется контролировать</param>
    public StreamWrapper(Stream Source) => _DataStream = Source;

    /// <inheritdoc />
    public override void Flush()
    {
        _DataStream.Flush();
        OnFlush?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        var result = _DataStream.Seek(offset, origin);
        OnSeek?.Invoke(this, new SeekArgs(offset, origin, result));
        return result;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        var old_length = _DataStream.Length;
        _DataStream.SetLength(value);
        OnSetLength?.Invoke(this, new SetLengthArgs(old_length, value));
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        var result = _DataStream.Read(buffer, offset, count);
        OnRead?.Invoke(this, new ReadArgs(buffer, offset, count, result));
        if(EndOfStream) OnEndOfStream?.Invoke(this, EventArgs.Empty);
        return result;
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        _DataStream.Write(buffer, offset, count);
        OnWrite?.Invoke(this, new WriteArgs(buffer, offset, count));
    }

    /// <summary>Можно ли осуществлять чтение данных из потока (определяется внутренним потоком)</summary>
    public override bool CanRead => _DataStream.CanRead;

    /// <summary>Можно ли осуществлять перемещение позиции чтения/записи в потоке (определяется внутренним потоком)</summary>
    public override bool CanSeek => _DataStream.CanSeek;

    /// <summary>Можно ли осуществлять запись данных в поток (определяется внутренним потоком)</summary>
    public override bool CanWrite => _DataStream.CanWrite;

    /// <summary>Длина потока</summary>
    public override long Length => _DataStream.Length;

    /// <summary>Текущее положение в потоке (при установке значения осуществляется проверка на предмет конца потока)</summary>
    public override long Position
    {
        get => _DataStream.Position;
        set
        {
            var old = _DataStream.Position;
            _DataStream.Position = value;
            OnPositionChanged?.Invoke(this, new PositionChangedArgs(old, value));
            if(EndOfStream) OnEndOfStream?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Признак конца потока</summary>
    public bool EndOfStream => _DataStream.Position == _DataStream.Length;

    /// <summary>Прогресс</summary>
    public double Progress => (double)_DataStream.Position / _DataStream.Length;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if(disposing)
            _DataStream.Dispose();
    }
}