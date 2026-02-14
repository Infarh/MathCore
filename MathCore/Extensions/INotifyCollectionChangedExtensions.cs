using System.Collections.Specialized;
// ReSharper disable EventNeverSubscribedTo.Global

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.ComponentModel;

// ReSharper disable once InconsistentNaming
public static class INotifyCollectionChangedExtensions
{
    /// <summary>Абстрактный подписчик на изменения коллекции</summary>
    public abstract class CollectionChangesSubscriber(
        INotifyCollectionChanged Obj,
        NotifyCollectionChangedAction ChangeType)
    {
        /// <summary>Событие, возникающее при изменении коллекции</summary>
        private event NotifyCollectionChangedEventHandler? OnCollectionChangedEventHandlers;

        /// <summary>Событие, возникающее при изменении коллекции</summary>
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

        /// <summary>Событие, возникающее при изменении типа действия коллекции</summary>
        private event Action<NotifyCollectionChangedAction>? CollectionChangedHandlers;
        /// <summary>Событие, возникающее при изменении типа действия коллекции</summary>
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

        /// <summary>Событие, возникающее при изменении значения коллекции</summary>
        private event Action? ValueChangeEventHandlers;
        /// <summary>Событие, возникающее при изменении значения коллекции</summary>
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

        /// <summary>Слабая ссылка на коллекцию</summary>
        protected readonly WeakReference<INotifyCollectionChanged> _Collection = new(Obj);

        /// <summary>Проверяет, есть ли подписчики на события</summary>
        public virtual bool IsEmpty => OnCollectionChangedEventHandlers is null && CollectionChangedHandlers is null && ValueChangeEventHandlers is null;

        /// <summary>Коллекция, на которую осуществляется подписка</summary>
        public INotifyCollectionChanged Collection =>
            _Collection.TryGetTarget(out var collection)
                ? collection
                : throw new InvalidOperationException("Попытка доступа к объекту, который был удалён из памяти");

        /// <summary>Обработчик события изменения коллекции</summary>
        private void OnCollectionChangedHandler(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            if (E.Action != ChangeType) return;
            OnCollectionChanged(Sender, E);
        }

        /// <summary>Виртуальный метод обработки изменения коллекции</summary>
        protected virtual void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            OnCollectionChangedEventHandlers?.Invoke(Sender, E);
            CollectionChangedHandlers?.Invoke(E.Action);
            ValueChangeEventHandlers?.Invoke();
        }

        /// <summary>Подписка на событие CollectionChanged</summary>
        protected void Subscribe() => Collection.CollectionChanged += OnCollectionChangedHandler;
        /// <summary>Отписка от события CollectionChanged</summary>
        protected void Unsubscribe() => Collection.CollectionChanged -= OnCollectionChangedHandler;

        /// <summary>Очистка всех обработчиков событий</summary>
        internal virtual void ClearHandlers()
        {
            OnCollectionChangedEventHandlers = null;
            CollectionChangedHandlers = null;
            ValueChangeEventHandlers = null;
        }
    }

    /// <summary>Подписчик на изменения коллекции определённого типа</summary>
    public sealed class CollectionChangesSubscriber<TCollection, TItem> : CollectionChangesSubscriber
        where TCollection : ICollection<TItem>, INotifyCollectionChanged
    {
        /// <summary>Событие, возникающее при изменении коллекции определённого типа</summary>
        private event Action<ICollection<TItem>>? OnCollectionChangedEventHandlers;
        /// <summary>Событие, возникающее при изменении коллекции определённого типа</summary>
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

        /// <summary>Проверяет, есть ли подписчики на события</summary>
        public override bool IsEmpty => base.IsEmpty && OnCollectionChangedEventHandlers is null;

        /// <summary>Конструктор подписчика на изменения коллекции</summary>
        internal CollectionChangesSubscriber(TCollection Obj, NotifyCollectionChangedAction ChangeType) : base(Obj, ChangeType) { }

        /// <summary>Обработка изменения коллекции</summary>
        protected override void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            base.OnCollectionChanged(Sender, E);
            var handlers = OnCollectionChangedEventHandlers;
            if (handlers is null) return;
            var collection = E.Action switch
            {
                NotifyCollectionChangedAction.Add => [.. E.NewItems!.Cast<TItem>()],
                NotifyCollectionChangedAction.Remove => [.. E.OldItems!.Cast<TItem>()],
                NotifyCollectionChangedAction.Replace => (ICollection<TItem>?)Sender,
                NotifyCollectionChangedAction.Move => (ICollection<TItem>?)Sender,
                NotifyCollectionChangedAction.Reset => (ICollection<TItem>?)Sender,
                _ => throw new InvalidEnumArgumentException(nameof(E.Action), (int)E.Action, typeof(NotifyCollectionChangedAction))
            };
            handlers.Invoke(collection!);
        }

        /// <summary>Очистка всех обработчиков событий</summary>
        internal override void ClearHandlers()
        {
            base.ClearHandlers();
            OnCollectionChangedEventHandlers = null;
        }
    }

    /// <summary>Словарь подписчиков на коллекции</summary>
    private static readonly Dictionary<INotifyCollectionChanged, Dictionary<NotifyCollectionChangedAction, CollectionChangesSubscriber>> __Subscribers = [];

    /// <summary>Подписка на событие изменения коллекции с автоматической отпиской</summary>
    public static IDisposable UsingSubscribeToProperty<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
        where T : ICollection<TItem>, INotifyCollectionChanged
    {
        obj.OnCollectionChanged<T, TItem>(ChangeType, Handler);
        return new LambdaDisposable(() => obj.UnsubscribeFrom(ChangeType, Handler));
    }

    /// <summary>Подписка на событие изменения коллекции</summary>
    public static void OnCollectionChanged<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
        where T : INotifyCollectionChanged, ICollection<TItem>
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => []) ?? throw new InvalidOperationException();
            var object_subscriber = object_subscribers.GetValueOrAddNew(ChangeType, () => new CollectionChangesSubscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
            object_subscriber.OnCollectionChangedEvent += Handler;
        }
    }

    /// <summary>Отписка от события изменения коллекции</summary>
    public static void UnsubscribeFrom(
        this INotifyCollectionChanged obj,
        NotifyCollectionChangedAction ChangeType,
        NotifyCollectionChangedEventHandler Handler)
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValue(obj);
            var object_subscriber = object_subscribers?.GetValue(ChangeType);
            if (object_subscriber is null) return;

            object_subscriber.OnCollectionChangedEvent -= Handler;

            if (object_subscriber.IsEmpty && object_subscribers is not null) object_subscribers.Remove(ChangeType);
            if (object_subscribers?.Count == 0) __Subscribers.Remove(obj);
        }
    }

    /// <summary>Получение подписчика на изменения коллекции</summary>
    public static CollectionChangesSubscriber<T, TItem> SubscribeCollectionTo<T, TItem>(
        this T obj,
        NotifyCollectionChangedAction ChangeType)
        where T : ICollection<TItem>, INotifyCollectionChanged
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => []) ?? throw new InvalidOperationException();
            return (CollectionChangesSubscriber<T, TItem>)object_subscribers.GetValueOrAddNew(ChangeType, () => new CollectionChangesSubscriber<T, TItem>(obj, ChangeType)) ?? throw new InvalidOperationException();
        }
    }

    /// <summary>Очистка обработчиков событий коллекции</summary>
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

    /// <summary>Откладывает обработку событий изменения коллекции</summary>
    public static IDisposable DeferChanges(this INotifyCollectionChanged collection, NotifyCollectionChangedEventHandler EventHandler) => new CollectionEventDeferer(collection, EventHandler);

    /// <summary>Класс, реализующий отложенную обработку событий изменения коллекции</summary>
    private class CollectionEventDeferer : IDisposable
    {
        private readonly INotifyCollectionChanged _Collection;
        private readonly NotifyCollectionChangedEventHandler _EventHandler;
        private readonly List<NotifyCollectionChangedEventArgs> _Events = new(1000);

        /// <summary>Конструктор класса CollectionEventDeferer</summary>
        public CollectionEventDeferer(INotifyCollectionChanged collection, NotifyCollectionChangedEventHandler EventHandler)
        {
            _Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _EventHandler = EventHandler ?? throw new ArgumentNullException(nameof(EventHandler));
            _Collection.CollectionChanged -= EventHandler;
            _Collection.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>Выполняет обработку накопленных событий и отписывается от коллекции</summary>
        public void Dispose()
        {
            _Collection.CollectionChanged -= OnCollectionChanged;
            foreach (var @event in _Events)
                _EventHandler(_Collection, @event);
            _Collection.CollectionChanged += _EventHandler;
        }

        /// <summary>Обработчик события CollectionChanged, добавляющий событие в список</summary>
        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs e) => _Events.Add(e);
    }

    /// <summary>Подписчик на изменения свойства элементов коллекции</summary>
    private class CollectionItemPropertyChangedSubscriber<TCollection, TItem> : IDisposable
        where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
        where TItem : INotifyPropertyChanged
    {
        private readonly TCollection _Collection;
        private readonly string _PropertyName;
        private readonly EventHandler _OnPropertyChanged;

        /// <summary>Конструктор подписчика на изменения свойства элементов коллекции</summary>
        public CollectionItemPropertyChangedSubscriber(TCollection Collection, string PropertyName, EventHandler OnPropertyChanged)
        {
            _Collection = Collection;
            _PropertyName = PropertyName;
            _OnPropertyChanged = OnPropertyChanged;
            Collection.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>Обработчик события изменения коллекции</summary>
        private void OnCollectionChanged(object? Sender, NotifyCollectionChangedEventArgs E)
        {
            switch (E.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (INotifyPropertyChanged item in E.NewItems!)
                        item.PropertyChanged += OnItemPropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (INotifyPropertyChanged item in E.OldItems!)
                        item.PropertyChanged -= OnItemPropertyChanged;
                    break;
            }
        }

        /// <summary>Обработчик события изменения свойства элемента</summary>
        private void OnItemPropertyChanged(object? Sender, PropertyChangedEventArgs E)
        {
            if (E.PropertyName != _PropertyName) return;
            _OnPropertyChanged(Sender, EventArgs.Empty);
        }

        /// <summary>Отписка от событий и очистка обработчиков</summary>
        public void Dispose()
        {
            OnCollectionChanged(_Collection, new(NotifyCollectionChangedAction.Remove, _Collection));
            _Collection.CollectionChanged -= OnCollectionChanged;
        }
    }

    /// <summary>Подписка на изменения свойства элементов коллекции</summary>
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