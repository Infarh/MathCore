using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using MathCore;
using MathCore.Annotations;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.ComponentModel
{
    /// <summary>Класс методов-расширений интерфейса <see cref="INotifyPropertyChanged"/></summary>
    // ReSharper disable once InconsistentNaming
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>Подписка на событие изменения указанного свйоства</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Name">Имя свйоства</param>
        public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, string Name)
            => obj.PropertyChanged += (s, e) => { if(string.Equals(e.PropertyName, Name)) handler(s, e); };

        /// <summary>Подписка на событие изменения указанного свйоства</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Name">Имя свйоства</param>
        /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
        public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, string Name)
        {
            void Handler(object s, PropertyChangedEventArgs e)
            {
                if (string.Equals(e.PropertyName, Name)) handler(s, e);
            }

            obj.PropertyChanged += Handler;
            return new LambdaDisposable(() => obj.PropertyChanged -= Handler);
        }

        /// <summary>Подписка на событие изменения указанных свйоств</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Names">Имена свйоств</param>
        public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, params string[] Names)
            => obj.PropertyChanged += (s, e) => { if(Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e); };


        /// <summary>Подписка на событие изменения указанных свйоств</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Names">Имена свйоств</param>
        /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
        public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, params string[] Names)
        {
            void Handler(object s, PropertyChangedEventArgs e)
            {
                if (Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e);
            }

            obj.PropertyChanged += Handler;
            return new LambdaDisposable(() => obj.PropertyChanged -= Handler);
        }

        /// <summary>Подписка на событие изменения указанных свйоств</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Names">Имена свйоств</param>
        public static void RegisterPropertyChangedHandler(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, IEnumerable<string> Names)
            => obj.PropertyChanged += (s, e) => { if(Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e); };

        /// <summary>Подписка на событие изменения указанных свйоств</summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="handler">Обработчик события <see cref="INotifyPropertyChanged.PropertyChanged"/> типа <see cref="PropertyChangedEventHandler"/></param>
        /// <param name="Names">Имена свйоств</param>
        /// <returns>Объект <see cref="IDisposable"/>, вызывающий отписку от события в случае своего уничтожения</returns>
        public static IDisposable RegisterPropertyChangedHandler_Disposable(this INotifyPropertyChanged obj, PropertyChangedEventHandler handler, IEnumerable<string> Names)
        {
            void Handler(object s, PropertyChangedEventArgs e)
            {
                if (Names.Any(name => string.Equals(name, e.PropertyName))) handler(s, e);
            }

            obj.PropertyChanged += Handler;
            return new LambdaDisposable(() => obj.PropertyChanged -= Handler);
        }

        #region Связи свйоств по атрибутам AffectsTheAttribute и DependencyOnAttribute

        /// <summary>Аргумент события изменения зависимого свойства</summary>
        public sealed class DependentPropertyChangedEventArgs : PropertyChangedEventArgs
        {
            /// <summary>Перечень свойств, породивших изменение</summary>
            public string[] FromProperties { get; private set; }

            /// <summary>Инициализация нового аргумента события изменения зависимого свойства</summary>
            /// <param name="PropertyName">Имя изменившегося свойства</param>
            /// <param name="FromProperties">Список свойств, породивших изменение</param>
            public DependentPropertyChangedEventArgs([NotNull] string PropertyName, [NotNull] string[] FromProperties) : base(PropertyName)
            {
                Contract.Requires(!string.IsNullOrEmpty(PropertyName));
                Contract.Requires(FromProperties != null);
                Contract.Requires(FromProperties.Length > 0);
                this.FromProperties = FromProperties;
            }
        }

        /// <summary>Перечень слабых ссылок на отслеживаемые объекты</summary>
        private static readonly HashSet<WeakReference> __ObjectsSet = new HashSet<WeakReference>();
        /// <summary>Словарь описаний связей между свойствами типов</summary>
        private static readonly Dictionary<Type, RegistratorInfo> __RegistrationPool = new Dictionary<Type, RegistratorInfo>();
        /// <summary>Информация о связях между свойствами типов</summary>
        private sealed class RegistratorInfo
        {
            /// <summary>Словарь связей имён свойств типа</summary>
            private readonly Dictionary<string, string[]> _Dependences;
            /// <summary>Обработчик события изменения свойства объекта</summary>
            private PropertyChangedEventHandler _Handler;

            /// <summary>Инициализация нового экземпляра информации и связях между свойствами типа</summary>
            /// <param name="Dependences">Словарь имён свойст зависимостей</param>
            public RegistratorInfo(Dictionary<string, string[]> Dependences)
            {
                Contract.Requires(Dependences != null);
                _Dependences = Dependences;
            }

            /// <summary>Метод установки обработчика событий обновления свойства объекта, генерирующего вторичные события обновления зависимых свойств</summary>
            /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
            /// <param name="OnPropertyChanged">Метод генерации события <see cref="INotifyPropertyChanged.PropertyChanged"/> в объекте <paramref name="obj"/></param>
            public void Subscribe(INotifyPropertyChanged obj, [NotNull] Action<PropertyChangedEventArgs> OnPropertyChanged)
            {
                if(_Handler is null)
                    _Handler = (s, e) =>
                    {
                        if(!_Dependences.ContainsKey(e.PropertyName)) return;
                        if(e is DependentPropertyChangedEventArgs args)
                        {
                            var p_stack = args.FromProperties ?? new string[0];
                            if(p_stack.Contains(str => e.PropertyName.Equals(str))) return;
                            foreach(var property in _Dependences[args.PropertyName])
                            {
                                var new_p_stack = new string[p_stack.Length + 1];
                                new_p_stack[0] = args.PropertyName;
                                Array.Copy(p_stack, 0, new_p_stack, 1, new_p_stack.Length);
                                OnPropertyChanged(new DependentPropertyChangedEventArgs(property, new_p_stack));
                            }
                        }
                        else
                            foreach(var property in _Dependences[e.PropertyName])
                                OnPropertyChanged(new DependentPropertyChangedEventArgs(property, new[] { e.PropertyName }));
                    };
                obj.PropertyChanged += _Handler;
            }

            /// <summary>Отписка от события обновления свойств объекта</summary>
            /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
            public void UnSubscrige(INotifyPropertyChanged obj) => obj.PropertyChanged -= _Handler;
        }

        /// <summary>Обработка событий слорки мусора в системе</summary>
        /// <param name="Sender">Источник события - не используется</param>
        /// <param name="e">Аргумент события - не используется</param>
        private static void OnGarbageCollected(object Sender, EventArgs e)
        {
            lock (__ObjectsSet)
            {
                __ObjectsSet.RemoveWhere(wr => !wr.IsAlive);
                __RegistrationPool.RemoveWhere(v => !__ObjectsSet.Any(w => v.Key.IsInstanceOfType(w.Target)));
            }
        }

        /// <summary>
        /// Создание связей между свойствами объекта на основе атрибутов <see cref="AffectsTheAttribute"/> и <see cref="DependencyOnAttribute"/>
        /// </summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="OnPropertyChanged">Метод генерации события обновления свойства</param>
        /// <typeparam name="T">Тип объекта</typeparam>
        public static void PropertyDependences_Register<T>([NotNull] this T obj, [NotNull] Action<PropertyChangedEventArgs> OnPropertyChanged)
            where T : class, INotifyPropertyChanged
        {
            Contract.Requires(obj != null);
            Contract.Requires(OnPropertyChanged != null);
            lock (__ObjectsSet)
            {
                if(__ObjectsSet.Any(wr => obj.Equals(wr.Target))) return;
                __ObjectsSet.Add(new WeakReference(obj));
                typeof(T).GetRegistrator().Subscribe(obj, OnPropertyChanged);
                if(__ObjectsSet.Count == 1)
                    GCWacher.Complite += OnGarbageCollected;
            }
        }

        /// <summary>
        /// Создание связей между свойствами объекта на основе атрибутов <see cref="AffectsTheAttribute"/> и <see cref="DependencyOnAttribute"/>
        /// </summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <param name="OnPropertyChanged">Метод генерации события обновления свойства</param>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <returns></returns>
        public static IDisposable PropertyDependences_Register_Disposable<T>([NotNull] this T obj, [NotNull] Action<PropertyChangedEventArgs> OnPropertyChanged)
            where T : class, INotifyPropertyChanged
        {
            Contract.Requires(obj != null);
            Contract.Requires(OnPropertyChanged != null);
            lock (__ObjectsSet)
            {
                if(__ObjectsSet.Any(wr => obj.Equals(wr.Target))) return new LambdaDisposable();
                __ObjectsSet.Add(new WeakReference(obj));
                typeof(T).GetRegistrator().Subscribe(obj, OnPropertyChanged);
                if(__ObjectsSet.Count == 1)
                    GCWacher.Complite += OnGarbageCollected;
            }
            return new LambdaDisposable(obj.PropertyDependences_Unregister);
        }

        /// <summary>
        /// Разрушение связей между свойствами, созданными методом <see cref="PropertyDependences_Register{T}"/>
        /// </summary>
        /// <param name="obj">Объект, реализующий интерфейс <see cref="INotifyPropertyChanged"/></param>
        /// <typeparam name="T">Тип объекта</typeparam>
        public static void PropertyDependences_Unregister<T>([NotNull] this T obj) where T : class, INotifyPropertyChanged
        {
            Contract.Requires(obj != null);
            lock (__ObjectsSet)
            {
                var w_ref = __ObjectsSet.FirstOrDefault(wr => obj.Equals(wr.Target));
                if(w_ref is null) return;
                var type = typeof(T);
                type.GetRegistrator().UnSubscrige(obj);
                __ObjectsSet.Remove(w_ref);
                if(!__ObjectsSet.Any(w => w.Target is T))
                    __RegistrationPool.Remove(type);
                if(__ObjectsSet.Count == 0)
                    GCWacher.Complite -= OnGarbageCollected;
            }
        }

        /// <summary>Метод получения информации о связях между свойствами объекта класса</summary>
        /// <param name="type">Тип рассматриваемого объекта</param>
        /// <returns>Информация о связях между свойствами объекта</returns>
        private static RegistratorInfo GetRegistrator([NotNull] this Type type)
        {
            Contract.Requires(type != null);
            lock (__ObjectsSet)
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                if (__RegistrationPool.TryGetValue(type, out RegistratorInfo registration))
                    return registration;

                var dep = new Dictionary<string, List<string>>(properties.Length);
                foreach (var property in properties)
                {
                    foreach(AffectsTheAttribute affect_on in property.GetCustomAttributes(typeof(AffectsTheAttribute), true))
                        dep.GetValueOrAddNew(property.Name, () => new List<string>()).Add(affect_on.Name);

                    foreach(DependencyOnAttribute dependence_on in property.GetCustomAttributes(typeof(DependencyOnAttribute), true))
                        dep.GetValueOrAddNew(dependence_on.Name, () => new List<string>()).Add(property.Name);
                }

                var dependences = dep.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());
                __RegistrationPool.Add(type, registration = new RegistratorInfo(dependences));
                return registration;
            }
        }

        #endregion

        public abstract class Subscriber
        {
            private event PropertyChangedEventHandler OnPropertyChangedEventHandlers;

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

            private event Action<string> OnPropertyChangedHandlers;
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
            protected readonly INotifyPropertyChanged _NotifyPropertyChangedObject;

            [NotNull]
            private readonly string _PropertyName;

            public virtual bool IsEmpty => OnPropertyChangedEventHandlers is null && OnPropertyChangedHandlers is null && ValueChangeEventHandlers is null;

            [NotNull]
            public INotifyPropertyChanged NotifyPropertyChangedObject => _NotifyPropertyChangedObject;

            protected Subscriber([NotNull] INotifyPropertyChanged Obj, [NotNull] string PropertyName)
            {
                _PropertyName = PropertyName;
                _NotifyPropertyChangedObject = Obj;
            }

            private void OnPropertyChangedHandler([CanBeNull] object Sender, [NotNull] PropertyChangedEventArgs E)
            {
                if (E.PropertyName != _PropertyName) return;
                OnObjectPropertyChanged(Sender, E);
            }

            protected virtual void OnObjectPropertyChanged([CanBeNull] object Sender, [NotNull]  PropertyChangedEventArgs E)
            {
                OnPropertyChangedEventHandlers?.Invoke(Sender, E);
                OnPropertyChangedHandlers?.Invoke(E.PropertyName);
                ValueChangeEventHandlers?.Invoke();
            }

            protected void Subscribe() => _NotifyPropertyChangedObject.PropertyChanged += OnPropertyChangedHandler;
            protected void Unsubscribe() => _NotifyPropertyChangedObject.PropertyChanged -= OnPropertyChangedHandler;

            internal virtual void ClearHandlers()
            {
                OnPropertyChangedEventHandlers = null;
                OnPropertyChangedHandlers = null;
                ValueChangeEventHandlers = null;
            }
        }
        public sealed class Subscriber<T> : Subscriber where T : INotifyPropertyChanged
        {
            private event Action<T> OnObjectValueChangedHandlers;
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

            internal Subscriber([NotNull] INotifyPropertyChanged Obj, [NotNull] string PropertyName) : base(Obj, PropertyName) { }

            protected override void OnObjectPropertyChanged(object Sender, [NotNull] PropertyChangedEventArgs E)
            {
                base.OnObjectPropertyChanged(Sender, E);
                OnObjectValueChangedHandlers?.Invoke((T)Sender);
            }

            internal override void ClearHandlers()
            {
                base.ClearHandlers();
                OnObjectValueChangedHandlers = null;
            }
        }

        [NotNull]
        private static readonly Dictionary<INotifyPropertyChanged, Dictionary<string, Subscriber>> __Subscribers =
            new Dictionary<INotifyPropertyChanged, Dictionary<string, Subscriber>>();


        [NotNull]
        public static IDisposable UsingSubscribeToProperty(
            [NotNull] this INotifyPropertyChanged obj,
            [NotNull] string EventName,
            [NotNull] PropertyChangedEventHandler Handler)
        {
            obj.SubscribeTo(EventName, Handler);
            return new LambdaDisposable(() => obj.UnsubscribeFromProperty(EventName, Handler));
        }

        /// <summary>Подписаться на событие изменения свойства</summary>
        /// <typeparam name="T">Тип объекта, генерирующего событие</typeparam>
        /// <param name="obj">Объект, на событие изменения свойств которого производится подписка</param>
        /// <param name="PropertyName">Имя отслеживаемого свойства</param>
        /// <param name="Handler">ОБработчик события</param>
        public static void SubscribeTo<T>(
            [CanBeNull] this T obj,
            [NotNull] string PropertyName,
            [NotNull] PropertyChangedEventHandler Handler)
            where T : class, INotifyPropertyChanged
        {
            if (obj is null) return;
            lock (__Subscribers)
            {
                var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<string, Subscriber>()) ?? throw new InvalidOperationException();
                var object_subscriber = object_subscribers.GetValueOrAddNew(PropertyName, () => new Subscriber<T>(obj, PropertyName)) ?? throw new InvalidOperationException();
                object_subscriber.OnPropertyChangedEvent += Handler;
            }
        }

        [NotNull]
        public static Subscriber<T> SubscribeTo<T>([NotNull] this T obj, [NotNull] string PropertyName)
            where T : INotifyPropertyChanged
        {
            lock (__Subscribers)
            {
                var object_subscribers = __Subscribers.GetValueOrAddNew(obj, () => new Dictionary<string, Subscriber>()) ?? throw new InvalidOperationException();
                return (Subscriber<T>)object_subscribers.GetValueOrAddNew(PropertyName, () => new Subscriber<T>(obj, PropertyName)) ?? throw new InvalidOperationException();
            }
        }

        public static void UnsubscribeFromProperty(
            [CanBeNull] this INotifyPropertyChanged obj,
            [NotNull] string EventName,
            [NotNull] PropertyChangedEventHandler Handler)
        {
            if (obj is null) return;
            lock (__Subscribers)
            {
                var object_subscribers = __Subscribers.GetValue(obj);
                var object_subscriber = object_subscribers?.GetValue(EventName);
                if (object_subscriber is null) return;

                object_subscriber.OnPropertyChangedEvent -= Handler;

                if (object_subscriber.IsEmpty) object_subscribers.Remove(EventName);
                if (object_subscribers.Count == 0) __Subscribers.Remove(obj);
            }
        }

        public static void ClearPropertyEventHandlers([NotNull] this INotifyPropertyChanged obj, [CanBeNull] string EventName = null)
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
}