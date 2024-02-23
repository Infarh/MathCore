#nullable enable

// ReSharper disable UnusedType.Global

namespace MathCore.Extensions;

public static class ReaderWriterLockSlimExtensions
{
    public readonly ref struct WriterLock
    {
        private readonly ReaderWriterLockSlim _Lock;

        public WriterLock(ReaderWriterLockSlim Lock)
        {
            _Lock = Lock;
            _Lock.EnterWriteLock();
        }

        public void Dispose()
        {
            if(_Lock.IsWriteLockHeld)
                _Lock.ExitWriteLock();
        }
    }

    public readonly ref struct ReaderLock
    {
        private readonly ReaderWriterLockSlim _Lock;

        public ReaderLock(ReaderWriterLockSlim Lock)
        {
            _Lock = Lock;
            _Lock.EnterReadLock();
        }

        public void Dispose()
        {
            if(_Lock.IsReadLockHeld)
                _Lock.ExitReadLock();
        }
    }

    public static ReaderLock LockRead(this ReaderWriterLockSlim Lock) => new(Lock);

    public static WriterLock LockWrite(this ReaderWriterLockSlim Lock) => new(Lock);

    public static LockObject<T> Obj<T>(this ReaderWriterLockSlim Lock, T obj) => new(Lock, obj);

    public static LockObject<T> Lock<T>(this ReaderWriterLockSlim Lock, T obj) => new(Lock, obj);

    public readonly ref struct LockObject<T>(ReaderWriterLockSlim Lock, T obj)
    {
        public TResult Read<TResult>(Func<T, TResult> Reader)
        {
            using (Lock.LockRead())
                return Reader(obj);
        }

        public TResult Read<TResult, TP>(TP Parameter, Func<T, TP, TResult> Reader)
        {
            using (Lock.LockRead())
                return Reader(obj, Parameter);
        }

        public void Write(Action<T> Writer)
        {
            using(Lock.LockWrite())
                Writer(obj);
        }

        public TResult Write<TResult>(Func<T, TResult> Writer)
        {
            using(Lock.LockWrite())
                return Writer(obj);
        }

        public void Write<TP>(TP Parameter, Action<T, TP> Writer)
        {
            using(Lock.LockWrite())
                Writer(obj, Parameter);
        }

        public TResult Write<TResult, TP>(TP Parameter, Func<T, TP, TResult> Writer)
        {
            using(Lock.LockWrite())
                return Writer(obj, Parameter);
        }
    }
}
