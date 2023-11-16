#nullable enable
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace MathCore;

/// <summary>
/// Provides a base implementation for producer-consumer collections that wrap other
/// producer-consumer collections.
/// </summary>
/// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
[Serializable]
public abstract class ProducerConsumerCollectionBase<T> : IProducerConsumerCollection<T>
{
    private readonly IProducerConsumerCollection<T> _Contained;

    /// <summary>Initializes the ProducerConsumerCollectionBase instance.</summary>
    /// <param name="contained">The collection to be wrapped by this instance.</param>
    protected ProducerConsumerCollectionBase(IProducerConsumerCollection<T> contained) => _Contained = contained.NotNull();

    /// <summary>Gets the contained collection.</summary>
    protected IProducerConsumerCollection<T> ContainedCollection => _Contained;

    /// <summary>Attempts to add the specified value to the end of the deque.</summary>
    /// <param name="item">The item to add.</param>
    /// <returns>true if the item could be added; otherwise, false.</returns>
    protected virtual bool TryAdd(T item) => _Contained.TryAdd(item);

    /// <summary>Attempts to remove and return an item from the collection.</summary>
    /// <param name="item">
    /// When this method returns, if the operation was successful, item contains the item removed. If
    /// no item was available to be removed, the value is unspecified.
    /// </param>
    /// <returns>
    /// true if an element was removed and returned from the collection; otherwise, false.
    /// </returns>
    protected virtual bool TryTake(out T item) => _Contained.TryTake(out item);

    /// <summary>Attempts to add the specified value to the end of the deque.</summary>
    /// <param name="item">The item to add.</param>
    /// <returns>true if the item could be added; otherwise, false.</returns>
    bool IProducerConsumerCollection<T>.TryAdd(T item) => TryAdd(item);

    /// <summary>Attempts to remove and return an item from the collection.</summary>
    /// <param name="item">
    /// When this method returns, if the operation was successful, item contains the item removed. If
    /// no item was available to be removed, the value is unspecified.
    /// </param>
    /// <returns>
    /// true if an element was removed and returned from the collection; otherwise, false.
    /// </returns>
    bool IProducerConsumerCollection<T>.TryTake(out T item) => TryTake(out item);

    /// <summary>Gets the number of elements contained in the collection.</summary>
    public int Count => _Contained.Count;

    /// <summary>Creates an array containing the contents of the collection.</summary>
    /// <returns>The array.</returns>
    public T[] ToArray() => _Contained.ToArray();

    /// <summary>Copies the contents of the collection to an array.</summary>
    /// <param name="array">The array to which the data should be copied.</param>
    /// <param name="index">The starting index at which data should be copied.</param>
    public void CopyTo(T[] array, int index) => _Contained.CopyTo(array, index);

    /// <summary>Copies the contents of the collection to an array.</summary>
    /// <param name="array">The array to which the data should be copied.</param>
    /// <param name="index">The starting index at which data should be copied.</param>
    void ICollection.CopyTo(Array array, int index) => _Contained.CopyTo(array, index);

    /// <summary>Gets an enumerator for the collection.</summary>
    /// <returns>An enumerator.</returns>
    public IEnumerator<T> GetEnumerator() => _Contained.GetEnumerator();

    /// <summary>Gets an enumerator for the collection.</summary>
    /// <returns>An enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Gets whether the collection is synchronized.</summary>
    bool ICollection.IsSynchronized => _Contained.IsSynchronized;

    /// <summary>Gets the synchronization root object for the collection.</summary>
    object ICollection.SyncRoot => _Contained.SyncRoot;
}

/// <summary>Debug view for the IProducerConsumerCollection.</summary>
/// <typeparam name="T">Specifies the type of the data being aggregated.</typeparam>
internal sealed class IProducerConsumerCollection_DebugView<T>
{
    private readonly IProducerConsumerCollection<T> _Collection;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Values => _Collection.ToArray();

    public IProducerConsumerCollection_DebugView(IProducerConsumerCollection<T> collection) => _Collection = collection;
}

/// <summary>Provides a thread-safe object pool.</summary>
/// <typeparam name="T">Specifies the type of the elements stored in the pool.</typeparam>
[DebuggerDisplay("Count={" + nameof(Count) + "}")]
[DebuggerTypeProxy(typeof(IProducerConsumerCollection_DebugView<>))]
public sealed class ObjectPool<T> : ProducerConsumerCollectionBase<T>
{
    private readonly Func<T> _Factory;

    /// <summary>Initializes an instance of the ObjectPool class.</summary>
    /// <param name="Factory">The function used to create items when no items exist in the pool.</param>
    public ObjectPool(Func<T> Factory) : this(Factory, new ConcurrentQueue<T>()) { }

    /// <summary>Initializes an instance of the ObjectPool class.</summary>
    /// <param name="Factory">The function used to create items when no items exist in the pool.</param>
    /// <param name="collection">The collection used to store the elements of the pool.</param>
    public ObjectPool(Func<T> Factory, IProducerConsumerCollection<T> collection)
        : base(collection) => 
        _Factory = Factory.NotNull();

    /// <summary>Adds the provided item into the pool.</summary>
    /// <param name="item">The item to be added.</param>
    public void PutObject(T item) => base.TryAdd(item);

    /// <summary>Gets an item from the pool.</summary>
    /// <returns>The removed or created item.</returns>
    /// <remarks>If the pool is empty, a new item will be created and returned.</remarks>
    public T GetObject() => base.TryTake(out var value) ? value : _Factory();

    /// <summary>Clears the object pool, returning all of the data that was in the pool.</summary>
    /// <returns>An array containing all of the elements in the pool.</returns>
    public T[] ToArrayAndClear()
    {
        var items = new List<T>();
        while (base.TryTake(out var value)) 
            items.Add(value);
        return items.ToArray();
    }

    protected override bool TryAdd(T item)
    {
        PutObject(item);
        return true;
    }

    protected override bool TryTake(out T item)
    {
        item = GetObject();
        return true;
    }
}