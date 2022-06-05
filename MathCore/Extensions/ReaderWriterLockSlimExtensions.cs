#nullable enable
using System;
using System.Threading;
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

    public readonly ref struct LockObject<T>
    {
        private readonly ReaderWriterLockSlim _Lock;

        private readonly T _Obj;

        public LockObject(ReaderWriterLockSlim Lock, T obj)
        {
            _Lock = Lock;
            _Obj = obj;
        }

        public TResult Read<TResult>(Func<T, TResult> Reader)
        {
            using (_Lock.LockRead())
                return Reader(_Obj);
        }

        public TResult Read<TResult, TP>(TP Parameter, Func<T, TP, TResult> Reader)
        {
            using (_Lock.LockRead())
                return Reader(_Obj, Parameter);
        }

        public void Write(Action<T> Writer)
        {
            using(_Lock.LockWrite())
                Writer(_Obj);
        }

        public TResult Write<TResult>(Func<T, TResult> Writer)
        {
            using(_Lock.LockWrite())
                return Writer(_Obj);
        }

        public void Write<TP>(TP Parameter, Action<T, TP> Writer)
        {
            using(_Lock.LockWrite())
                Writer(_Obj, Parameter);
        }

        public TResult Write<TResult, TP>(TP Parameter, Func<T, TP, TResult> Writer)
        {
            using(_Lock.LockWrite())
                return Writer(_Obj, Parameter);
        }
    }
}
