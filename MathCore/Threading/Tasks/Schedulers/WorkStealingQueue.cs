using System.Collections.Generic;
using System.Threading;

namespace MathCore.Threading.Tasks.Schedulers
{
    /// <summary>A work-stealing queue.</summary>
    /// <typeparam name="T">Specifies the type of data stored in the queue.</typeparam>
    internal class WorkStealingQueue<T> where T : class
    {
        private const int __InitialSize = 32;
        private T[] _Array = new T[__InitialSize];
        private int _Mask = __InitialSize - 1;
        private volatile int _HeadIndex;
        private volatile int _TailIndex;

        private readonly object _ForeignLock = new object();

        internal void LocalPush(T obj)
        {
            var tail = _TailIndex;

            // When there are at least 2 elements' worth of space, we can take the fast path.
            if (tail < _HeadIndex + _Mask)
            {
                _Array[tail & _Mask] = obj;
                _TailIndex = tail + 1;
            }
            else
                // We need to contend with foreign pops, so we lock.
                lock (_ForeignLock)
                {
                    var head = _HeadIndex;
                    var count = _TailIndex - _HeadIndex;

                    // If there is still space (one left), just add the element.
                    if (count >= _Mask)
                    {
                        // We're full; expand the queue by doubling its size.
                        var new_array = new T[_Array.Length << 1];
                        for (var i = 0; i < _Array.Length; i++)
                            new_array[i] = _Array[(i + head) & _Mask];

                        // Reset the field values, incl. the mask.
                        _Array = new_array;
                        _HeadIndex = 0;
                        _TailIndex = tail = count;
                        _Mask = (_Mask << 1) | 1;
                    }

                    _Array[tail & _Mask] = obj;
                    _TailIndex = tail + 1;
                }
        }

        internal bool LocalPop(ref T obj)
        {
            while (true)
            {
                // Decrement the tail using a fence to ensure subsequent read doesn't come before.
                var tail = _TailIndex;
                if (_HeadIndex >= tail)
                {
                    obj = null;
                    return false;
                }

                tail -= 1;
#pragma warning disable 0420
                Interlocked.Exchange(ref _TailIndex, tail);
#pragma warning restore 0420

                // If there is no interaction with a take, we can head down the fast path.
                if (_HeadIndex <= tail)
                {
                    var idx = tail & _Mask;
                    obj = _Array[idx];

                    // Check for nulls in the array.
                    if (obj == null) continue;

                    _Array[idx] = null;
                    return true;
                }

                lock (_ForeignLock)
                    if (_HeadIndex <= tail)
                    {
                        // Element still available. Take it.
                        var idx = tail & _Mask;
                        obj = _Array[idx];

                        // Check for nulls in the array.
                        if (obj == null) continue;

                        _Array[idx] = null;
                        return true;
                    }
                    else
                    {
                        // We lost the race, element was stolen, restore the tail.
                        _TailIndex = tail + 1;
                        obj = null;
                        return false;
                    }
            }
        }

        internal bool TrySteal(ref T obj)
        {
            obj = null;

            while (true)
            {
                if (_HeadIndex >= _TailIndex)
                    return false;

                lock (_ForeignLock)
                {
                    // Increment head, and ensure read of tail doesn't move before it (fence).
                    var head = _HeadIndex;
#pragma warning disable 0420
                    Interlocked.Exchange(ref _HeadIndex, head + 1);
#pragma warning restore 0420

                    if (head < _TailIndex)
                    {
                        var idx = head & _Mask;
                        obj = _Array[idx];

                        // Check for nulls in the array.
                        if (obj == null) continue;

                        _Array[idx] = null;
                        return true;
                    }

                    // Failed, restore head.
                    _HeadIndex = head;
                    obj = null;
                }

                return false;
            }
        }

        internal bool TryFindAndPop(T obj)
        {
            // We do an O(N) search for the work item. The theory of work stealing and our
            // inlining logic is that most waits will happen on recently queued work.  And
            // since recently queued work will be close to the tail end (which is where we
            // begin our search), we will likely find it quickly.  In the worst case, we
            // will traverse the whole local queue; this is typically not going to be a
            // problem (although degenerate cases are clearly an issue) because local work
            // queues tend to be somewhat shallow in length, and because if we fail to find
            // the work item, we are about to block anyway (which is very expensive).

            for (var i = _TailIndex - 1; i >= _HeadIndex; i--)
                if (_Array[i & _Mask] == obj)
                    // If we found the element, block out steals to avoid interference.
                    lock (_ForeignLock)
                    {
                        // If we lost the race, bail.
                        if (_Array[i & _Mask] == null) return false;

                        // Otherwise, null out the element.
                        _Array[i & _Mask] = null;

                        // And then check to see if we can fix up the indexes (if we're at
                        // the edge).  If we can't, we just leave nulls in the array and they'll
                        // get filtered out eventually (but may lead to superflous resizing).
                        if (i == _TailIndex)
                            Interlocked.Decrement(ref _TailIndex);
                        else if (i == _HeadIndex)
                            Interlocked.Increment(ref _HeadIndex);

                        return true;
                    }

            return false;
        }

        internal T[] ToArray()
        {
            var list = new List<T>();
            for (var i = _TailIndex - 1; i >= _HeadIndex; i--)
            {
                var obj = _Array[i & _Mask];
                if (obj != null) list.Add(obj);
            }
            return list.ToArray();
        }
    }
}