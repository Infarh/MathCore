using System.Collections.Generic;
using System.Threading;

namespace System.Collections.Concurrent
{
    public class ConcurrentList<T> : IList<T>, IDisposable
    {
        #region Fields

        private readonly List<T> _List;
        private readonly ReaderWriterLockSlim _Lock;

        #endregion

        #region Properties

        public T this[int index]
        {
            get
            {
                try
                {
                    _Lock.EnterReadLock();
                    return _List[index];
                }
                finally
                {
                    _Lock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _Lock.EnterWriteLock();
                    _List[index] = value;
                }
                finally
                {
                    _Lock.ExitWriteLock();
                }
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    _Lock.EnterReadLock();
                    return _List.Count;
                }
                finally
                {
                    _Lock.ExitReadLock();
                }
            }
        }

        public bool IsReadOnly => false;

        #endregion

        #region Constructors

        public ConcurrentList()
        {
            _Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _List = new List<T>();
        }

        public ConcurrentList(int capacity)
        {
            _Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _List = new List<T>(capacity);
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            _Lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _List = new List<T>(items);
        }

        #endregion

        #region Methods

        public void Add(T item)
        {
            try
            {
                _Lock.EnterWriteLock();
                _List.Add(item);
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }

        public void Insert(int index, T item)
        {
            try
            {
                _Lock.EnterWriteLock();
                _List.Insert(index, item);
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }

        public bool Remove(T item)
        {
            try
            {
                _Lock.EnterWriteLock();
                return _List.Remove(item);
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            try
            {
                _Lock.EnterWriteLock();
                _List.RemoveAt(index);
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }

        public int IndexOf(T item)
        {
            try
            {
                _Lock.EnterReadLock();
                return _List.IndexOf(item);
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        }

        public void Clear()
        {
            try
            {
                _Lock.EnterWriteLock();
                _List.Clear();
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }

        public bool Contains(T item)
        {
            try
            {
                _Lock.EnterReadLock();
                return _List.Contains(item);
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int ArrayIndex)
        {
            try
            {
                _Lock.EnterReadLock();
                _List.CopyTo(array, ArrayIndex);
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        }

        public IEnumerator<T> GetEnumerator() => new ConcurrentEnumerator(_List, _Lock);

        IEnumerator IEnumerable.GetEnumerator() => new ConcurrentEnumerator(_List, _Lock);

        private readonly struct ConcurrentEnumerator : IEnumerator<T>
        {
            #region Fields

            private readonly IEnumerator<T> _Inner;
            private readonly ReaderWriterLockSlim _Lock;

            #endregion

            #region Properties

            public T Current => _Inner.Current;

            object IEnumerator.Current => _Inner.Current;

            #endregion

            #region Constructor

            public ConcurrentEnumerator(IEnumerable<T> inner, ReaderWriterLockSlim @lock)
            {
                _Lock = @lock;
                _Lock.EnterReadLock();
                _Inner = inner.GetEnumerator();
            }

            #endregion

            #region Methods

            public bool MoveNext() => _Inner.MoveNext();

            public void Reset() => _Inner.Reset();

            public void Dispose() => _Lock.ExitReadLock();

            #endregion
        }

        ~ConcurrentList() => Dispose(false);

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (disposing)
                GC.SuppressFinalize(this);

            _Lock.Dispose();
        }
        #endregion
    }
}
