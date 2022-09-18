using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ConsoleTest;

//https://habr.com/ru/company/clrium/blog/420051/
public ref struct ValueStringBuilder
{
    private char[]? _ArrayToReturnToPool;
    private Span<char> _Chars;
    private int _Pos;

    public ValueStringBuilder(Span<char> InitialBuffer)
    {
        _ArrayToReturnToPool = null;
        _Chars               = InitialBuffer;
        _Pos                 = 0;
    }

    public int Length
    {
        get => _Pos;
        set
        {
            var delta = value - _Pos;
            if (delta > 0)
                Append('\0', delta);
            else
                _Pos = value;
        }
    }

    public override string ToString()
    {
        var s = new string(_Chars[.._Pos]);
        Clear();
        return s;
    }

    public bool TryCopyTo(Span<char> destination, out int CharsWritten)
    {
        if (_Chars[.._Pos].TryCopyTo(destination))
        {
            CharsWritten = _Pos;
            Clear();
            return true;
        }

        CharsWritten = 0;
        Clear();
        return false;
    }

    public void Insert(int index, char value, int count)
    {
        if (_Pos > _Chars.Length - count) Grow(count);

        var remaining = _Pos - index;
        _Chars.Slice(index, remaining).CopyTo(_Chars[(index + count)..]);
        _Chars.Slice(index, count).Fill(value);
        _Pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _Pos;
        if (pos < _Chars.Length)
        {
            _Chars[pos] = c;
            _Pos        = pos + 1;
        }
        else
            GrowAndAppend(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string s)
    {
        var pos = _Pos;
        // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        if (s.Length == 1 && pos < _Chars.Length)
        {
            _Chars[pos] = s[0];
            _Pos        = pos + 1;
        }
        else
            AppendSlow(s);
    }

    private void AppendSlow(string s)
    {
        var pos = _Pos;
        if (pos > _Chars.Length - s.Length) Grow(s.Length);

        s.AsSpan().CopyTo(_Chars[pos..]);
        _Pos += s.Length;
    }

    public void Append(char c, int count)
    {
        if (_Pos > _Chars.Length - count) Grow(count);

        var dst                                     = _Chars.Slice(_Pos, count);
        for (var i = 0; i < dst.Length; i++) dst[i] = c;
        _Pos += count;
    }

    public unsafe void Append(char* value, int length)
    {
        var pos = _Pos;
        if (pos > _Chars.Length - length) Grow(length);

        var dst                                     = _Chars.Slice(_Pos, length);
        for (var i = 0; i < dst.Length; i++) dst[i] = *value++;
        _Pos += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var orig_pos = _Pos;
        if (orig_pos > _Chars.Length - length) Grow(length);

        _Pos = orig_pos + length;
        return _Chars.Slice(orig_pos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int RequiredAdditionalCapacity)
    {
        Debug.Assert(RequiredAdditionalCapacity > _Chars.Length - _Pos);

        var pool_array = ArrayPool<char>.Shared.Rent(Math.Max(_Pos + RequiredAdditionalCapacity, _Chars.Length * 2));

        _Chars.CopyTo(pool_array);

        var to_return                 = _ArrayToReturnToPool;
        _Chars = _ArrayToReturnToPool = pool_array;
        if (to_return is { }) ArrayPool<char>.Shared.Return(to_return);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Clear()
    {
        var to_return = _ArrayToReturnToPool;
        // for safety, to avoid using pooled array if this instance is erroneously appended to again
        this = default; 
        if (to_return is { }) ArrayPool<char>.Shared.Return(to_return);
    }
}

public sealed class NativeMemoryManager : MemoryManager<byte>
{
    //https://habr.com/ru/company/clrium/blog/420051/

    private readonly int _Length;
    private IntPtr _Pointer;
    private int _RetainedCount;
    private bool _Disposed;

    public NativeMemoryManager(int length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

        _Length  = length;
        _Pointer = Marshal.AllocHGlobal(length);
    }

    // The code that's violating the rule is on this line.
    /// <inheritdoc />
#pragma warning disable CA2015
    ~NativeMemoryManager()
#pragma warning restore CA2015
    {
        Debug.WriteLine($"{nameof(NativeMemoryManager)} being finalized");
        Dispose(false);
    }

    public bool IsDisposed
    {
        get
        {
            lock (this) return _Disposed && _RetainedCount == 0;
        }
    }

    public override Memory<byte> Memory => CreateMemory(_Length);

    public bool IsRetained
    {
        get
        {
            lock (this) return _RetainedCount > 0;
        }
    }

    public override unsafe Span<byte> GetSpan() => new((void*)_Pointer, _Length);

    public override unsafe MemoryHandle Pin(int ElementIndex = 0)
    {
        // Note that this intentionally allows elementIndex == _length to
        // support pinning zero-length instances.
        if ((uint)ElementIndex > (uint)_Length) throw new ArgumentOutOfRangeException(nameof(ElementIndex));

        lock (this)
        {
            if (_RetainedCount == 0 && _Disposed) throw new Exception();
            _RetainedCount++;
        }

        var pointer = (void*)((byte*)_Pointer + ElementIndex); // T = byte
        return new MemoryHandle(pointer, default, this);
    }

    public override void Unpin()
    {
        lock (this)
            if (_RetainedCount > 0)
            {
                _RetainedCount--;
                if (_RetainedCount != 0 || !_Disposed) return;
                Marshal.FreeHGlobal(_Pointer);
                _Pointer = IntPtr.Zero;
            }
    }

    protected override void Dispose(bool disposing)
    {
        lock (this)
        {
            _Disposed = true;
            if (_RetainedCount != 0) return;
            Marshal.FreeHGlobal(_Pointer);
            _Pointer = IntPtr.Zero;
        }
    }
}