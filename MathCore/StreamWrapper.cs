#nullable enable
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>Объект с интерфейсом потока байт данных, обеспечивающий слежение за процессами чтения/записи данных</summary>
/// <remarks>Инициализация нового экземпляра <see cref="StreamWrapper"/></remarks>
/// <param name="Source">Поток, доступ к которому требуется контролировать</param>
/// <param name="ExplicitLength">Если поток не поддерживает поиск, то тут указать его длину (если она определена)</param>
public class StreamWrapper(Stream Source, long ExplicitLength = 0) : Stream
{
    /// <summary>Аргумент события <see cref="StreamWrapper.OnSeek"/></summary>
    /// <remarks>Инициализация нового экземпляра <see cref="SeekArgs"/></remarks>
    /// <param name="offset">Величина заданного смещения в потоке</param>
    /// <param name="origin">Направление смещения в потоке</param>
    /// <param name="result">Результат выполнения смещения в потоке</param>
    public readonly struct SeekArgs(long offset, SeekOrigin origin, long result)
    {
        /// <summary>Величина заданного смещения в потоке</summary>
        public long Offset => offset;

        /// <summary>Направление смещения в потоке</summary>
        public SeekOrigin Origin => origin;

        /// <summary>Результат выполнения смещения в потоке</summary>
        public long Result => result;

        public void Deconstruct(out long Offset, out SeekOrigin Origin, out long Result)
        {
            Offset = offset;
            Origin = origin;
            Result = result;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnSetLength"/></summary>
    /// <remarks>Инициализация нового экземпляра <see cref="SetLengthArgs"/></remarks>
    /// <param name="old">Предыдущая длина потока</param>
    /// <param name="length">Установленная длина потока</param>
    public readonly struct SetLengthArgs(long old, long length)
    {
        /// <summary>Предыдущая длина потока</summary>
        public long OldLength => old;

        /// <summary>Установленная длина потока</summary>
        public long Length => length;

        public void Deconstruct(out long OldLength, out long Length)
        {
            OldLength = old;
            Length    = length;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnRead"/></summary>
    /// <remarks>Инициализация нового экземпляра <see cref="ReadArgs"/></remarks>
    /// <param name="buffer">Прочитанный массив байт</param>
    /// <param name="offset">Смещение в массиве</param>
    /// <param name="count">Число читаемых байт</param>
    /// <param name="result">Число прочитанных байт</param>
    public readonly struct ReadArgs(byte[] buffer, int offset, int count, int result)
    {
        /// <summary>Прочитанный массив байт</summary>
        public byte[] Buffer => buffer;

        /// <summary>Смещение в массиве</summary>
        public int Offset => offset;

        /// <summary>Число читаемых байт</summary>
        public int Count => count;

        /// <summary>Число прочитанных байт</summary>
        public int Result => result;

        public void Deconstruct(out byte[] Buffer, out int Offset, out int Count, out int Result)
        {
            Buffer = buffer;
            Offset = offset;
            Count  = count;
            Result = result;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnWrite"/></summary>
    /// <remarks>Инициализация нового экземпляра <see cref="WriteArgs"/></remarks>
    /// <param name="buffer">Записываемый массив байт</param>
    /// <param name="offset">Смещение в массиве</param>
    /// <param name="count">Число записываемых байт</param>
    public readonly struct WriteArgs(byte[] buffer, int offset, int count)
    {
        /// <summary>Записываемый массив байт</summary>
        public byte[] Buffer => buffer;

        /// <summary>Смещение в массиве</summary>
        public int Offset => offset;

        /// <summary>Число записываемых байт</summary>
        public int Count => count;

        public void Deconstruct(out byte[] Buffer, out int Offset, out int Count)
        {
            Buffer = buffer;
            Offset = offset;
            Count  = count;
        }
    }

    /// <summary>Аргумент события <see cref="StreamWrapper.OnPositionChanged"/></summary>
    /// <remarks>Инициализация нового экземпляра <see cref="PositionChangedArgs"/></remarks>
    /// <param name="Old">Предыдущее положение в потоке данных</param>
    /// <param name="New">Новое положение в потоке данных</param>
    public readonly struct PositionChangedArgs(long Old, long New)
    {
        /// <summary>Предыдущее положение в потоке данных</summary>
        public long OldPosition => Old;

        /// <summary>Новое положение в потоке данных</summary>
        public long NewPosition => New;

        public void Deconstruct(out long OldPosition, out long NewPosition)
        {
            OldPosition = Old;
            NewPosition = New;
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

    /// <summary>Положение в потоке (используется если поток не поддерживает поиск)</summary>
    private long _Position;

    /* --------------------------------------------------------------------------------------- */

    /// <inheritdoc />
    public override void Flush()
    {
        Source.Flush();
        OnFlush?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        var position = Source.Seek(offset, origin);
        _Position = position;
        OnSeek?.Invoke(this, new(offset, origin, position));
        return position;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        var old_length = Source.Length;
        Source.SetLength(value);
        OnSetLength?.Invoke(this, new(old_length, value));
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        var stream = Source;
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
        Source.Write(buffer, offset, count);
        _Position += count;
        OnWrite?.Invoke(this, new(buffer, offset, count));
    }

    /// <summary>Можно ли осуществлять чтение данных из потока (определяется внутренним потоком)</summary>
    public override bool CanRead => Source.CanRead;

    /// <summary>Можно ли осуществлять перемещение позиции чтения/записи в потоке (определяется внутренним потоком)</summary>
    public override bool CanSeek => Source.CanSeek;

    /// <summary>Можно ли осуществлять запись данных в поток (определяется внутренним потоком)</summary>
    public override bool CanWrite => Source.CanWrite;

    /// <summary>Длина потока</summary>
    public override long Length => Source is { CanSeek: true, Length: var length } ? length : ExplicitLength;

    /// <summary>Текущее положение в потоке (при установке значения осуществляется проверка на предмет конца потока)</summary>
    public override long Position
    {
        get => Source is { CanSeek: true, Position: var position } ? position : _Position;
        set
        {
            var stream = Source;
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
        get => Source.Position == Source.Length;
    }

    /// <summary>Прогресс</summary>
    public double Progress => Source is { CanSeek: true, Position: var pos, Length: var len }
        ? (double)pos / len
        : (double)_Position / ExplicitLength;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            Source.Dispose();
    }
}