using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MathCore.Annotations;

namespace System.ComponentModel
{
    public static class INotifyCollectionChangedExtensions
    {
        public abstract class Subscriber
        {
            private event NotifyCollectionChangedEventHandler OnCollectionChangedEventHandlers;

            public event NotifyCollectionChangedEventHandler OnCollectionChangedEvent
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

            private event Action<NotifyCollectionChangedAction> OnCollectionChangedHandlers;
            public event Action<NotifyCollectionChangedAction> OnPCollectionChanged
            {
                add
                {
                    if (IsEmpty) Subscribe();
                    OnCollectionChangedHandlers += value;
                }
                remove
                {
                    OnCollectionChangedHandlers -= value;
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

            public virtual bool IsEmpty => OnCollectionChangedEventHandlers is null && OnCollectionChangedHandlers is null && ValueChangeEventHandlers is null;

            [NotNull]
            public INotifyCollectionChanged Collection => _Collection;

            protected Subscriber([NotNull] INotifyCollectionChanged Obj, NotifyCollectionChangedAction ChangeType)
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
                OnCollectionChangedEventHandlers?.Invoke(Sender, E);
                OnCollectionChangedHandlers?.Invoke(E.Action);
                ValueChangeEventHandlers?.Invoke();
            }

            protected void Subscribe() => _Collection.CollectionChanged += OnCollectionChangedHandler;
            protected void Unsubscribe() => _Collection.CollectionChanged -= OnCollectionChangedHandler;

            internal virtual void ClearHandlers()
            {
                OnCollectionChangedEventHandlers = null;
                OnCollectionChangedHandlers = null;
                ValueChangeEventHandlers = null;
            }
        }

        public sealed class Subscriber<TCollection, TItem> : Subscriber
            where TCollection : ICollection<TItem>, INotifyCollectionChanged
        {
            private event Action<ICollection<TItem>> OnClooectionChangedEventHandlers;
            public event Action<ICollection<TItem>> OnClooectionChangedEvent
            {
                add
                {
                    if (IsEmpty) Subscribe();
                    OnClooectionChangedEventHandlers += value;
                }
                remove
                {
                    OnClooectionChangedEventHandlers -= value;
                    if (IsEmpty) Unsubscribe();
                }
            }

            public override bool IsEmpty => base.IsEmpty && OnClooectionChangedEventHandlers is null;

            internal Subscriber([NotNull] TCollection Obj, NotifyCollectionChangedAction ChangeType) : base(Obj, ChangeType) { }

            protected override void OnCollectionChanged(object Sender, NotifyCollectionChangedEventArgs E)
            {
                base.OnCollectionChanged(Sender, E);
                var handlers = OnClooectionChangedEventHandlers;
                if (handlers is null) return;
                ICollection<TItem> collection;
                switch (E.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        collection = E.NewItems.Cast<TItem>().ToArray();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        collection = E.OldItems.Cast<TItem>().ToArray();
                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Reset:
                        collection = (ICollection<TItem>)Sender;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                handlers.Invoke(collection);
            }

            internal override void ClearHandlers()
            {
                base.ClearHandlers();
                OnClooectionChangedEventHandlers = null;
            }
        }

        [NotNull]
        private static readonly Dictionary<INotifyCollectionChanged, Dictionary<NotifyCollectionChangedAction, Subscriber>> __Subscribers = new Dictionary<INotifyCollectionChanged, Dictionary<NotifyCollectionChangedAction, Subscriber>>();

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
                var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<NotifyCollectionChangedAction, Subscriber>()) ?? throw new InvalidOperationException();
                var object_subscriber = object_subscribers.GetValueOrAddNew(ChangeType, () => new Subscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
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
        public static Subscriber<T, TItem> SubscribeCollectionTo<T, TItem>(
            [NotNull] this T obj,
            NotifyCollectionChangedAction ChangeType)
            where T : ICollection<TItem>, INotifyCollectionChanged
        {
            lock (__Subscribers)
            {
                var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<NotifyCollectionChangedAction, Subscriber>()) ?? throw new InvalidOperationException();
                return (Subscriber<T, TItem>)object_subscribers.GetValueOrAddNew(ChangeType, () => new Subscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
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
                    foreach (var k in object_subscribers.ToArray())
                    {
                        k.Value.ClearHandlers();

                        object_subscribers.Remove(k.Key);
                        if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
                    }
            }
        }

        [NotNull] public static IDisposable DeferChanges([NotNull] this INotifyCollectionChanged collection, [NotNull] NotifyCollectionChangedEventHandler EventHandler) => new CollectionEventDeferer(collection, EventHandler);

        private class CollectionEventDeferer : IDisposable
        {
            [NotNull] private readonly INotifyCollectionChanged _Collection;
            [NotNull] private readonly NotifyCollectionChangedEventHandler _EventHandler;
            [NotNull, ItemNotNull] private readonly List<NotifyCollectionChangedEventArgs> _Events = new List<NotifyCollectionChangedEventArgs>(1000);

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
    }
}