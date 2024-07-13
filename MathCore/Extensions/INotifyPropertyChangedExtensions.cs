#nullable enable
using System.Reflection;
using MathCore;
// ReSharper disable EventNeverSubscribedTo.Global

// ReSharper disable UnusedType.Global

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.ComponentModel;

/// <summary>Класс методов-расширений интерфейса <see cref="INotifyPropertyChanged"/></summary>
// ReSharper disable once InconsistentNaming
public static class INotifyPropertyChangedExtensions
{
    /// <summary>Подписка на событие изменения указанного свойства</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Name">Имя свойства</param>
    public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, string Name)
        => obj.PropertyChanged += (s, e) => { if (string.Equals(e.PropertyName, Name)) handler(s, e); };

    /// <summary>Подписка на событие изменения указанного свойства</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Name">Имя свойства</param>
    /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
    public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, string Name)
    {
        obj.PropertyChanged += Handler;
        return new LambdaDisposable(() => obj.PropertyChanged -= Handler);

        void Handler(object? s, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, Name)) handler(s, e);
        }
    }

    /// <summary>Подписка на событие изменения указанных свойств</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Names">Имена свойств</param>
    public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, params string[] Names)
        => obj.PropertyChanged += (s, e) => { if (Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e); };

    /// <summary>Подписка на событие изменения указанных свойств</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Names">Имена свойств</param>
    /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
    public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, params string[] Names)
    {
        obj.PropertyChanged += Handler;
        return new LambdaDisposable(() => obj.PropertyChanged -= Handler);

        void Handler(object? s, PropertyChangedEventArgs e)
        {
            if (Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e);
        }
    }

    /// <summary>Подписка на событие изменения указанных свойств</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Names">Имена свойств</param>
    public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, IEnumerable<string> Names)
        => obj.PropertyChanged += (s, e) => { if (Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e); };

    /// <summary>Подписка на событие изменения указанных свойств</summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
    /// <param name="Names">Имена свойств</param>
    /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
    public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, IEnumerable<string> Names)
    {
        var names = Names as string[] ?? Names.ToArray();

        obj.PropertyChanged += Handler;
        return new LambdaDisposable(() => obj.PropertyChanged -= Handler);

        void Handler(object? s, PropertyChangedEventArgs e)
        {
            if (names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e);
        }
    }

    #region Связи свойств по атрибутам AffectsTheAttribute и DependencyOnAttribute

    /// <summary>Аргумент события изменения зависимого свойства</summary>
    public sealed class DependentPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>Перечень свойств, породивших изменение</summary>
        public string[]? FromProperties { get; }

        /// <summary>Инициализация нового аргумента события изменения зависимого свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        /// <param name="FromProperties">Список свойств, породивших изменение</param>
        public DependentPropertyChangedEventArgs(string PropertyName, string[] FromProperties) : base(PropertyName) => this.FromProperties = FromProperties;
    }

    /// <summary>Перечень слабых ссылок на отслеживаемые объекты</summary>
    private static readonly HashSet<WeakReference> __ObjectsSet = [];
    /// <summary>Словарь описаний связей между свойствами типов</summary>
    private static readonly Dictionary<Type, RegistratorInfo> __RegistrationPool = [];
    /// <summary>Информация о связях между свойствами типов</summary>
    private sealed class RegistratorInfo
    {
        /// <summary>Словарь связей имён свойств типа</summary>
        private readonly Dictionary<string, string[]> _Dependencies;
        /// <summary>Обработчик события изменения свойства объекта</summary>
        private PropertyChangedEventHandler? _Handler;

        /// <summary>Инициализация нового экземпляра информации и связях между свойствами типа</summary>
        /// <param name="Dependencies">Словарь имён свойств зависимостей</param>
        public RegistratorInfo(Dictionary<string, string[]> Dependencies) => _Dependencies = Dependencies;

        /// <summary>Метод установки обработчика событий обновления свойства объекта, генерирующего вторичные события обновления зависимых свойств</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="OnPropertyChanged">Метод генерации события <see cref="INotifyPropertyChanged.PropertyChanged"/> в объекте <paramref name="obj"/></param>
        public void Subscribe(INotifyPropertyChanged obj, Action<PropertyChangedEventArgs> OnPropertyChanged)
        {
            _Handler ??= (_, e) =>
            {
                if (!_Dependencies.ContainsKey(e.PropertyName)) return;
                if (e is DependentPropertyChangedEventArgs args)
                {
                    var p_stack = args.FromProperties ?? [];
                    if (p_stack.Contains(str => e.PropertyName.Equals(str))) return;
                    foreach (var property in _Dependencies[args.PropertyName])
                    {
                        var new_p_stack = new string[p_stack.Length + 1];
                        new_p_stack[0] = args.PropertyName;
                        Array.Copy(p_stack, 0, new_p_stack, 1, new_p_stack.Length);
                        OnPropertyChanged(new DependentPropertyChangedEventArgs(property, new_p_stack));
                    }
                }
                else
                    foreach (var property in _Dependencies[e.PropertyName])
                        OnPropertyChanged(new DependentPropertyChangedEventArgs(property, [e.PropertyName]));
            };
            obj.PropertyChanged += _Handler;
        }

        /// <summary>Отписка от события обновления свойств объекта</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        public void UnSubscribe(INotifyPropertyChanged obj) => obj.PropertyChanged -= _Handler;
    }

    /// <summary>Обработка событий сборки мусора в системе</summary>
    /// <param name="Sender">Источник события - не используется</param>
    /// <param name="e">Аргумент события - не используется</param>
    private static void OnGarbageCollected(object? Sender, EventArgs e)
    {
        lock (__ObjectsSet)
        {
            __ObjectsSet.RemoveWhere(wr => !wr.IsAlive);
            __RegistrationPool.RemoveWhere(v => !__ObjectsSet.Any(w => v.Key.IsInstanceOfType(w.Target)));
        }
    }

    /// <summary>Создание связей между свойствами объекта на основе атрибутов <see cref="AffectsTheAttribute"/> и <see cref="DependencyOnAttribute"/></summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="OnPropertyChanged">Метод генерации события обновления свойства</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    public static void PropertyDependencies_Register<T>(this T obj, Action<PropertyChangedEventArgs> OnPropertyChanged)
        where T : class, INotifyPropertyChanged
    {
        lock (__ObjectsSet)
        {
            if (__ObjectsSet.Any(wr => obj.Equals(wr.Target))) return;
            __ObjectsSet.Add(new(obj));
            typeof(T).GetRegistrator().Subscribe(obj, OnPropertyChanged);
            if (__ObjectsSet.Count == 1)
                GCWatcher.Complete += OnGarbageCollected;
        }
    }

    /// <summary>Создание связей между свойствами объекта на основе атрибутов <see cref="AffectsTheAttribute"/> и <see cref="DependencyOnAttribute"/></summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <param name="OnPropertyChanged">Метод генерации события обновления свойства</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns></returns>
    public static IDisposable PropertyDependencies_Register_Disposable<T>(this T obj, Action<PropertyChangedEventArgs> OnPropertyChanged)
        where T : class, INotifyPropertyChanged
    {
        lock (__ObjectsSet)
        {
            if (__ObjectsSet.Any(wr => obj.Equals(wr.Target))) return new LambdaDisposable();
            __ObjectsSet.Add(new(obj));
            typeof(T).GetRegistrator().Subscribe(obj, OnPropertyChanged);
            if (__ObjectsSet.Count == 1)
                GCWatcher.Complete += OnGarbageCollected;
        }
        return new LambdaDisposable(obj.PropertyDependencies_Unregister);
    }

    /// <summary>Разрушение связей между свойствами, созданными методом <see cref="PropertyDependencies_Register{T}"/></summary>
    /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
    /// <typeparam name="T">Тип объекта</typeparam>
    public static void PropertyDependencies_Unregister<T>(this T obj) where T : class, INotifyPropertyChanged
    {
        lock (__ObjectsSet)
        {
            var w_ref = __ObjectsSet.FirstOrDefault(wr => obj.Equals(wr.Target));
            if (w_ref is null) return;
            var type = typeof(T);
            type.GetRegistrator().UnSubscribe(obj);
            __ObjectsSet.Remove(w_ref);
            if (!__ObjectsSet.Any(w => w.Target is T))
                __RegistrationPool.Remove(type);
            if (__ObjectsSet.Count == 0)
                GCWatcher.Complete -= OnGarbageCollected;
        }
    }

    /// <summary>Метод получения информации о связях между свойствами объекта класса</summary>
    /// <param name="type">Тип рассматриваемого объекта</param>
    /// <returns>Информация о связях между свойствами объекта</returns>
    private static RegistratorInfo GetRegistrator(this Type type)
    {
        lock (__ObjectsSet)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (__RegistrationPool.TryGetValue(type, out var registration))
                return registration;

            var dep = new Dictionary<string, List<string>>(properties.Length);
            foreach (var property in properties)
            {
                foreach (AffectsTheAttribute affect_on in property.GetCustomAttributes(typeof(AffectsTheAttribute), true))
                    dep.GetValueOrAddNew(property.Name, () => []).Add(affect_on.Name);

                foreach (DependencyOnAttribute dependence_on in property.GetCustomAttributes(typeof(DependencyOnAttribute), true))
                    dep.GetValueOrAddNew(dependence_on.Name, () => []).Add(property.Name);
            }

            var dependencies = dep.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
            __RegistrationPool.Add(type, registration = new(dependencies));
            return registration;
        }
    }

    #endregion

    public abstract class Subscriber(INotifyPropertyChanged Obj, string PropertyName)
    {
        private event PropertyChangedEventHandler? OnPropertyChangedEventHandlers;

        public event PropertyChangedEventHandler OnPropertyChangedEvent
        {
            add
            {
                if (IsEmpty) Subscribe();
                OnPropertyChangedEventHandlers += value;
            }
            remove
            {
                OnPropertyChangedEventHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        private event Action<string>? OnPropertyChangedHandlers;
        public event Action<string> OnPropertyChanged
        {
            add
            {
                if (IsEmpty) Subscribe();
                OnPropertyChangedHandlers += value;
            }
            remove
            {
                OnPropertyChangedHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        private event Action? ValueChangeEventHandlers;
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

        protected readonly WeakReference<INotifyPropertyChanged> _NotifyPropertyChangedObject = new(Obj);

        public virtual bool IsEmpty => OnPropertyChangedEventHandlers is null && OnPropertyChangedHandlers is null && ValueChangeEventHandlers is null;

        public INotifyPropertyChanged NotifyPropertyChangedObject => 
            _NotifyPropertyChangedObject.TryGetTarget(out var obj) 
                ? obj 
                : throw new InvalidOperationException("Объект, для которого производится попытка доступа, удалён");

        private void OnPropertyChangedHandler(object? Sender, PropertyChangedEventArgs E)
        {
            if (E.PropertyName != PropertyName) return;
            OnObjectPropertyChanged(Sender, E);
        }

        protected virtual void OnObjectPropertyChanged(object? Sender, PropertyChangedEventArgs E)
        {
            OnPropertyChangedEventHandlers?.Invoke(Sender, E);
            OnPropertyChangedHandlers?.Invoke(E.PropertyName);
            ValueChangeEventHandlers?.Invoke();
        }

        protected void Subscribe() => NotifyPropertyChangedObject.PropertyChanged += OnPropertyChangedHandler;
        protected void Unsubscribe() => NotifyPropertyChangedObject.PropertyChanged -= OnPropertyChangedHandler;

        internal virtual void ClearHandlers()
        {
            OnPropertyChangedEventHandlers = null;
            OnPropertyChangedHandlers      = null;
            ValueChangeEventHandlers       = null;
        }
    }
    public sealed class Subscriber<T> : Subscriber where T : INotifyPropertyChanged
    {
        private event Action<T>? OnObjectValueChangedHandlers;
        public event Action<T> OnObjectValueChanged
        {
            add
            {
                if (IsEmpty) Subscribe();
                OnObjectValueChangedHandlers += value;
            }
            remove
            {
                OnObjectValueChangedHandlers -= value;
                if (IsEmpty) Unsubscribe();
            }
        }

        public override bool IsEmpty => base.IsEmpty && OnObjectValueChangedHandlers is null;

        internal Subscriber(INotifyPropertyChanged Obj, string PropertyName) : base(Obj, PropertyName) { }

        protected override void OnObjectPropertyChanged(object? Sender, PropertyChangedEventArgs E)
        {
            base.OnObjectPropertyChanged(Sender, E);
            OnObjectValueChangedHandlers?.Invoke((T?)Sender);
        }

        internal override void ClearHandlers()
        {
            base.ClearHandlers();
            OnObjectValueChangedHandlers = null;
        }
    }

    private static readonly Dictionary<INotifyPropertyChanged, Dictionary<string, Subscriber>> __Subscribers = [];

    public static IDisposable UsingSubscribeToProperty(
        this INotifyPropertyChanged obj,
        string EventName,
        PropertyChangedEventHandler Handler)
    {
        obj.SubscribeTo(EventName, Handler);
        return new LambdaDisposable(() => obj.UnsubscribeFromProperty(EventName, Handler));
    }

    /// <summary>Подписаться на событие изменения свойства</summary>
    /// <typeparam name="T">Тип объекта, генерирующего событие</typeparam>
    /// <param name="obj">Объект, на событие изменения свойств которого производится подписка</param>
    /// <param name="PropertyName">Имя отслеживаемого свойства</param>
    /// <param name="Handler">Обработчик события</param>
    public static void SubscribeTo<T>(
        this T? obj,
        string PropertyName,
        PropertyChangedEventHandler Handler)
        where T : class, INotifyPropertyChanged
    {
        if (obj is null) return;
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => []) ?? throw new InvalidOperationException();
            var object_subscriber = object_subscribers.GetValueOrAddNew(PropertyName, () => new Subscriber<T>(obj, PropertyName)) ?? throw new InvalidOperationException();
            object_subscriber.OnPropertyChangedEvent += Handler;
        }
    }

    public static Subscriber<T> SubscribeTo<T>(this T obj, string PropertyName)
        where T : INotifyPropertyChanged
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => []) ?? throw new InvalidOperationException();
            return (Subscriber<T>)object_subscribers.GetValueOrAddNew(PropertyName, () => new Subscriber<T>(obj, PropertyName)) ?? throw new InvalidOperationException();
        }
    }

    public static void UnsubscribeFromProperty(
        this INotifyPropertyChanged? obj,
        string EventName,
        PropertyChangedEventHandler Handler)
    {
        if (obj is null) return;
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValue(obj);
            var object_subscriber  = object_subscribers?.GetValue(EventName);
            if (object_subscriber is null) return;

            object_subscriber.OnPropertyChangedEvent -= Handler;

            if (object_subscriber.IsEmpty) object_subscribers.Remove(EventName);
            if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
        }
    }

    public static void ClearPropertyEventHandlers(this INotifyPropertyChanged obj, string? EventName = null)
    {
        lock (__Subscribers)
        {
            var object_subscribers = __Subscribers.GetValue(obj);
            if (object_subscribers is null) return;
            if (EventName != null)
            {
                var object_subscriber = object_subscribers.GetValue(EventName);
                if (object_subscriber is null) return;

                object_subscriber.ClearHandlers();

                object_subscribers.Remove(EventName);
                if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
            }
            else foreach (var (key, value) in object_subscribers.ToArray())
            {
                value.ClearHandlers();
                object_subscribers.Remove(key);
                if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
            }
        }
    }
}