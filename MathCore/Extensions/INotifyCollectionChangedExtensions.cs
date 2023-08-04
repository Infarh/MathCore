#nullable enable
using System.Collections.Specialized;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.ComponentModel;

// ReSharper disable once InconsistentNaming
public static class INotifyCollectionChangedExtensions
{
    public abstract class CollectionChangesSubscriber
    {
        private event NotifyCollectionChangedEventHandler? OnCollectionChangedEventHandlers;

        public event NotifyCollectionChangedEventHandler? OnCollectionChangedEvent
        {
            add
            {
                if (IsEmpty) Subscribe();
                OnCollectionChangedEventHandlers += value;
            }
            remove
            {
                OnCollectionChangedEventHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }
        
        private event Action<NotifyCollectionChangedAction>? CollectionChangedHandlers;
        public event Action<NotifyCollectionChangedAction>? CollectionChanged //todo: разобраться с событиями!
        {
            add
            {
                if (IsEmpty) Subscribe();
                CollectionChangedHandlers += value;
            }
            remove
            {
                CollectionChangedHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        private event Action? ValueChangeEventHandlers;
        public event Action? ValueChangeEvent
        {
            add
            {
                if (IsEmpty) Subscribe();
                ValueChangeEventHandlers += value;
            }
            remove
            {
                ValueChangeEventHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        protected readonly WeakReference<INotifyCollectionChanged> _Collection;

        private readonly NotifyCollectionChangedAction _ChangeType;

        public virtual bool IsEmpty => OnCollectionChangedEventHandlers is null && CollectionChangedHandlers is null && ValueChangeEventHandlers is null;

        public INotifyCollectionChanged Collection => 
            _Collection.TryGetTarget(out var collection)
                ? collection
                : throw new InvalidOperationException("Попытка доступа к объекту, который был удалён из памяти");

        protected CollectionChangesSubscriber(INotifyCollectionChanged Obj, NotifyCollectionChangedAction ChangeType)
        {
            _ChangeType = ChangeType;
            _Collection = new(Obj);
        }

        private void OnCollectionChangedHandler(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            if (E.Action != _ChangeType) return;
            OnCollectionChanged(Sender, E);
        }

        protected virtual void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            OnCollectionChangedEventHandlers?.Invoke(Sender, E);
            CollectionChangedHandlers?.Invoke(E.Action);
            ValueChangeEventHandlers?.Invoke();
        }

        protected void Subscribe() => Collection.CollectionChanged += OnCollectionChangedHandler;
        protected void Unsubscribe() => Collection.CollectionChanged -= OnCollectionChangedHandler;

        internal virtual void ClearHandlers()
        {
            OnCollectionChangedEventHandlers = null;
            CollectionChangedHandlers        = null;
            ValueChangeEventHandlers  = null;
        }
    }

    public sealed class CollectionChangesSubscriber<TCollection, TItem> : CollectionChangesSubscriber
        where TCollection : ICollection<TItem>, INotifyCollectionChanged
    {
        private event Action<ICollection<TItem>>? OnCollectionChangedEventHandlers;
        public new event Action<ICollection<TItem>>? OnCollectionChangedEvent //todo: разобраться с событиями!
        {
            add
            {
                if (IsEmpty) Subscribe();
                OnCollectionChangedEventHandlers += value;
            }
            remove
            {
                OnCollectionChangedEventHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        public override bool IsEmpty => base.IsEmpty && OnCollectionChangedEventHandlers is null;

        internal CollectionChangesSubscriber(TCollection Obj, NotifyCollectionChangedAction ChangeType) : base(Obj, ChangeType) { }

        protected override void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            base.OnCollectionChanged(Sender, E);
            var handlers = OnCollectionChangedEventHandlers;
            if (handlers is null) return;
            var collection = E.Action switch
            {
                NotifyCollectionChangedAction.Add     => E.NewItems.Cast<TItem>().ToArray(),
                NotifyCollectionChangedAction.Remove  => E.OldItems.Cast<TItem>().ToArray(),
                NotifyCollectionChangedAction.Replace => (ICollection<TItem>?) Sender,
                NotifyCollectionChangedAction.Move    => (ICollection<TItem>?) Sender,
                NotifyCollectionChangedAction.Reset   => (ICollection<TItem>?) Sender,
                _                                     => throw new ArgumentOutOfRangeException()
            };
            handlers.Invoke(collection!);
        }

        internal override void ClearHandlers()
        {
            base.ClearHandlers();
            OnCollectionChangedEventHandlers = null;
        }
    }

    private static readonly Dictionary<INotifyCollectionChanged, Dictionary<NotifyCollectionChangedAction, CollectionChangesSubscriber>> __Subscribers = new();

    public static IDisposable UsingSubscribeToProperty<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
        where T : ICollection<TItem>, INotifyCollectionChanged
    {
        obj.OnCollectionChanged<T, TItem>(ChangeType, Handler);
        return new LambdaDisposable(() => obj.UnsubscribeFrom(ChangeType, Handler));
    }

    public static void OnCollectionChanged<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
        where T : INotifyCollectionChanged, ICollection<TItem>
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<NotifyCollectionChangedAction, CollectionChangesSubscriber>()) ?? throw new InvalidOperationException();
            var object_subscriber = object_subscribers.GetValueOrAddNew(ChangeType, () => new CollectionChangesSubscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
            object_subscriber.OnCollectionChangedEvent += Handler;
        }
    }

    public static void UnsubscribeFrom(
        this INotifyCollectionChanged obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValue(obj);
            var object_subscriber  = object_subscribers?.GetValue(ChangeType);
            if (object_subscriber is null) return;

            object_subscriber.OnCollectionChangedEvent -= Handler;

            if (object_subscriber.IsEmpty) object_subscribers.Remove(ChangeType);
            if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
        }
    }

    public static CollectionChangesSubscriber<T, TItem> SubscribeCollectionTo<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType)
        where T : ICollection<TItem>, INotifyCollectionChanged
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<NotifyCollectionChangedAction, CollectionChangesSubscriber>()) ?? throw new InvalidOperationException();
            return (CollectionChangesSubscriber<T, TItem>)object_subscribers.GetValueOrAddNew(ChangeType, () => new CollectionChangesSubscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
        }
    }

    public static void ClearEventHandlers(
        this INotifyCollectionChanged obj,
        NotifyCollectionChangedAction? ChangeType = null)
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValue(obj);
            if (object_subscribers is null) return;
            if (ChangeType.HasValue)
            {
                var object_subscriber = object_subscribers.GetValue(ChangeType.Value);
                if (object_subscriber is null) return;

                object_subscriber.ClearHandlers();

                object_subscribers.Remove(ChangeType.Value);
                if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
            }
            else
                foreach (var (key, value) in object_subscribers.ToArray())
                {
                    value.ClearHandlers();
                    object_subscribers.Remove(key);
                    if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
                }
        }
    }

    public static IDisposable DeferChanges(this INotifyCollectionChanged collection, NotifyCollectionChangedEventHandler EventHandler) => new CollectionEventDeferer(collection, EventHandler);

    private class CollectionEventDeferer : IDisposable
    {
        private readonly INotifyCollectionChanged _Collection;
        private readonly NotifyCollectionChangedEventHandler _EventHandler;
        private readonly List<NotifyCollectionChangedEventArgs> _Events = new(1000);

        public CollectionEventDeferer(INotifyCollectionChanged collection, NotifyCollectionChangedEventHandler EventHandler)
        {
            _Collection                   =  collection ?? throw new ArgumentNullException(nameof(collection));
            _EventHandler                 =  EventHandler ?? throw new ArgumentNullException(nameof(EventHandler));
            _Collection.CollectionChanged -= EventHandler;
            _Collection.CollectionChanged += OnCollectionChanged;
        }

        public void Dispose()
        {
            _Collection.CollectionChanged -= OnCollectionChanged;
            foreach (var @event in _Events)
                _EventHandler(_Collection, @event);
            _Collection.CollectionChanged += _EventHandler;
        }

        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs e) => _Events.Add(e);
    }

    class CollectionItemPropertyChangedSubscriber<TCollection, TItem> : IDisposable
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        where TItem : INotifyPropertyChanged
    {
        private readonly TCollection _Collection;
        private readonly string _PropertyName;
        private readonly EventHandler _OnPropertyChanged;

        public CollectionItemPropertyChangedSubscriber(TCollection Collection, string PropertyName, EventHandler OnPropertyChanged)
        {
            _Collection                  =  Collection;
            _PropertyName                =  PropertyName;
            _OnPropertyChanged           =  OnPropertyChanged;
            Collection.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (INotifyPropertyChanged item in E.NewItems) 
                        item.PropertyChanged += OnItemPropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (INotifyPropertyChanged item in E.OldItems) 
                        item.PropertyChanged -= OnItemPropertyChanged;
                    break;
            }
        }

        private void OnItemPropertyChanged(object Sender, PropertyChangedEventArgs E)
        {
            if (E.PropertyName != _PropertyName) return;
            _OnPropertyChanged(Sender, EventArgs.Empty);
        }

        public void Dispose()
        {
            OnCollectionChanged(_Collection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _Collection));
            _Collection.CollectionChanged -= OnCollectionChanged;
        }
    }

    public static IDisposable SubscribeToItemPropertyChanges<TCollection, TItem>(
        this TCollection collection, 
        string PropertyName,
        EventHandler OnPropertyChanged)
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem> 
        where TItem : INotifyPropertyChanged =>
        new CollectionItemPropertyChangedSubscriber<TCollection, TItem>(collection, PropertyName, OnPropertyChanged);

    //public static void OnItemPropertyChanged<TCollection, TItem>(
    //    [NotNull] this TCollection collection,
    //    string PropertyName,
    //    [NotNull] EventHandler OnPropertyChanged)
    //    where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    //    where TItem : INotifyPropertyChanged
    //{
    //    foreach (var item in collection)
    //        item.PropertyChanged += OnPropertyChanged;

    //}
}