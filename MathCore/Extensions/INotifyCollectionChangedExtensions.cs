using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.ComponentModel
{
    public static class INotifyCollectionChangedExtensions
    {
        public abstract class CollectionChangesSubscriber
        {
            private event NotifyCollectionChangedEventHandler _OnCollectionChangedEvent;

            public event NotifyCollectionChangedEventHandler OnCollectionChangedEvent
            {
                add
                {
                    if (IsEmpty) Subscribe();
                    _OnCollectionChangedEvent += value;
                }
                remove
                {
                    _OnCollectionChangedEvent -= value;
                    if (IsEmpty) Unsubscribe();
                }
            }

            private event Action<NotifyCollectionChangedAction> _CollectionChanged;
            public event Action<NotifyCollectionChangedAction> CollectionChanged   //todo: разобраться с событиями!
            {
                add
                {
                    if (IsEmpty) Subscribe();
                    _CollectionChanged += value;
                }
                remove
                {
                    _CollectionChanged -= value;
                    if (IsEmpty) Unsubscribe();
                }
            }

            private event Action ValueChangeEventHandlers;
            public event Action ValueChangeEvent
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

            [NotNull]
            protected readonly INotifyCollectionChanged _Collection;

            private readonly NotifyCollectionChangedAction _ChangeType;

            public virtual bool IsEmpty => _OnCollectionChangedEvent is null && _CollectionChanged is null && ValueChangeEventHandlers is null;

            [NotNull]
            public INotifyCollectionChanged Collection => _Collection;

            protected CollectionChangesSubscriber([NotNull] INotifyCollectionChanged Obj, NotifyCollectionChangedAction ChangeType)
            {
                _ChangeType = ChangeType;
                _Collection = Obj;
            }

            private void OnCollectionChangedHandler([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs E)
            {
                if (E.Action != _ChangeType) return;
                OnCollectionChanged(Sender, E);
            }

            protected virtual void OnCollectionChanged([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs E)
            {
                _OnCollectionChangedEvent?.Invoke(Sender, E);
                _CollectionChanged?.Invoke(E.Action);
                ValueChangeEventHandlers?.Invoke();
            }

            protected void Subscribe() => _Collection.CollectionChanged += OnCollectionChangedHandler;
            protected void Unsubscribe() => _Collection.CollectionChanged -= OnCollectionChangedHandler;

            internal virtual void ClearHandlers()
            {
                _OnCollectionChangedEvent = null;
                _CollectionChanged = null;
                ValueChangeEventHandlers = null;
            }
        }

        public sealed class CollectionChangesSubscriber<TCollection, TItem> : CollectionChangesSubscriber
            where TCollection : ICollection<TItem>, INotifyCollectionChanged
        {
            private event Action<ICollection<TItem>> OnCollectionChangedEventHandlers;
            public new event Action<ICollection<TItem>> OnCollectionChangedEvent   //todo: разобраться с событиями!
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

            internal CollectionChangesSubscriber([NotNull] TCollection Obj, NotifyCollectionChangedAction ChangeType) : base(Obj, ChangeType) { }

            protected override void OnCollectionChanged(object Sender, NotifyCollectionChangedEventArgs E)
            {
                base.OnCollectionChanged(Sender, E);
                var handlers = OnCollectionChangedEventHandlers;
                if (handlers is null) return;
                var collection = E.Action switch
                {
                    NotifyCollectionChangedAction.Add => E.NewItems.Cast<TItem>().ToArray(),
                    NotifyCollectionChangedAction.Remove => E.OldItems.Cast<TItem>().ToArray(),
                    NotifyCollectionChangedAction.Replace => (ICollection<TItem>) Sender,
                    NotifyCollectionChangedAction.Move => (ICollection<TItem>) Sender,
                    NotifyCollectionChangedAction.Reset => (ICollection<TItem>) Sender,
                    _ => throw new ArgumentOutOfRangeException()
                };
                handlers.Invoke(collection);
            }

            internal override void ClearHandlers()
            {
                base.ClearHandlers();
                OnCollectionChangedEventHandlers = null;
            }
        }

        [NotNull]
        private static readonly Dictionary<INotifyCollectionChanged, Dictionary<NotifyCollectionChangedAction, CollectionChangesSubscriber>> __Subscribers = new();

        [NotNull]
        public static IDisposable UsingSubscribeToProperty<T, TItem>(
            [NotNull] this T obj,
            NotifyCollectionChangedAction ChangeType,
            [NotNull] NotifyCollectionChangedEventHandler Handler)
            where T : ICollection<TItem>, INotifyCollectionChanged
        {
            obj.OnCollectionChanged<T, TItem>(ChangeType, Handler);
            return new LambdaDisposable(() => obj.UnsubscribeFrom(ChangeType, Handler));
        }

        public static void OnCollectionChanged<T, TItem>(
            [NotNull] this T obj,
            NotifyCollectionChangedAction ChangeType,
            [NotNull] NotifyCollectionChangedEventHandler Handler)
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
            [NotNull] this INotifyCollectionChanged obj,
            NotifyCollectionChangedAction ChangeType,
            [NotNull] NotifyCollectionChangedEventHandler Handler)
        {
            lock (__Subscribers)
            {
                var object_subscribers = __Subscribers.GetValue(obj);
                var object_subscriber = object_subscribers?.GetValue(ChangeType);
                if (object_subscriber is null) return;

                object_subscriber.OnCollectionChangedEvent -= Handler;

                if (object_subscriber.IsEmpty) object_subscribers.Remove(ChangeType);
                if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
            }
        }

        [NotNull]
        public static CollectionChangesSubscriber<T, TItem> SubscribeCollectionTo<T, TItem>(
            [NotNull] this T obj,
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
            [NotNull] this INotifyCollectionChanged obj,
            [CanBeNull] NotifyCollectionChangedAction? ChangeType = null)
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

        [NotNull] public static IDisposable DeferChanges([NotNull] this INotifyCollectionChanged collection, [NotNull] NotifyCollectionChangedEventHandler EventHandler) => new CollectionEventDeferer(collection, EventHandler);

        private class CollectionEventDeferer : IDisposable
        {
            [NotNull] private readonly INotifyCollectionChanged _Collection;
            [NotNull] private readonly NotifyCollectionChangedEventHandler _EventHandler;
            [NotNull, ItemNotNull] private readonly List<NotifyCollectionChangedEventArgs> _Events = new(1000);

            public CollectionEventDeferer([NotNull] INotifyCollectionChanged collection, [NotNull] NotifyCollectionChangedEventHandler EventHandler)
            {
                _Collection = collection ?? throw new ArgumentNullException(nameof(collection));
                _EventHandler = EventHandler ?? throw new ArgumentNullException(nameof(EventHandler));
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

            private void OnCollectionChanged([CanBeNull] object Sender, [NotNull] NotifyCollectionChangedEventArgs e) => _Events.Add(e);
        }

        class CollectionItemPropertyChangedSubscriber<TCollection, TItem> : IDisposable
            where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
            where TItem : INotifyPropertyChanged
        {
            [NotNull] private readonly TCollection _Collection;
            private readonly string _PropertyName;
            [NotNull] private readonly EventHandler _OnPropertyChanged;

            public CollectionItemPropertyChangedSubscriber([NotNull] TCollection Collection, string PropertyName, [NotNull] EventHandler OnPropertyChanged)
            {
                _Collection = Collection;
                _PropertyName = PropertyName;
                _OnPropertyChanged = OnPropertyChanged;
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

        [NotNull]
        public static IDisposable SubscribeToItemPropertyChanges<TCollection, TItem>(
            [NotNull] this TCollection collection, 
            string PropertyName,
            [NotNull] EventHandler OnPropertyChanged)
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
}