using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

// ReSharper disable once CheckNamespace
namespace MathCore.ViewModels
{
    /// <summary>Наблюдаемая модель</summary>
    public class ViewModel : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
    {
        #region INotifyPropertyChanging

        public event PropertyChangingEventHandler PropertyChanging;

        public class PropertyChangingEventArgs<T> : PropertyChangingEventArgs
        {
            public T OldValue { get; }

            public T NewValue { get; set; }

            /// <inheritdoc />
            public PropertyChangingEventArgs(T OldValue, T NewValue, string PropertyName) : base(PropertyName)
            {
                this.OldValue = OldValue;
                this.NewValue = NewValue;
            }
        }

        protected virtual bool OnPropertyChanging<T>(T OldValue, ref T NewValue, [CallerMemberName] string PropertyName = null)
        {
            PropertyChangingEventArgs<T> args = null;
            PropertyChanging?.Invoke(this, args = new PropertyChangingEventArgs<T>(OldValue, NewValue, PropertyName));
            return !(args is null) && Equals(OldValue, NewValue = args.NewValue);
        }

        #endregion

        #region INotifyPropertyChanged 

        private event PropertyChangedEventHandler PropertyChangedEvent;

        /// <summary>Событие возникает когда изменяется значение свойства объекта</summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_PropertiesDependencesSyncRoot)
                    PropertyChanged_AddHandler(value);
            }
            remove
            {
                lock (_PropertiesDependencesSyncRoot)
                    PropertyChanged_RemoveHanddler(value);
            }
        }

        /// <summary>Присоединить обработчик события <see cref="PropertyChanged"/></summary>
        /// <param name="handler">Присоединяемый обработчик события <see cref="PropertyChanged"/></param>
        protected virtual void PropertyChanged_AddHandler(PropertyChangedEventHandler handler) => PropertyChangedEvent += handler;

        /// <summary>Словарь обработчиков событий изменений свойств</summary>
        private Dictionary<string, Action> _PropertyChangedHandlers;

        /// <summary>Добавление собработчика события изменения свойства</summary>
        /// <param name="PropertyName">Имя отслеживаемого события</param>
        /// <param name="handler">Устанавливаемый обработчик</param>
        protected void PropertyChanged_AddHandler(string PropertyName, Action handler)
        {
            lock (_PropertiesDependencesSyncRoot)
            {
                var handlers = _PropertyChangedHandlers ??= new Dictionary<string, Action>();
                if (handlers.TryGetValue(PropertyName, out var h)) handlers[PropertyName] = h + handler;
                else handlers.Add(PropertyName, handler);
            }
        }

        /// <summary>Извлечение обработчика события изменния указанного свойства</summary>
        /// <param name="PropertyName">Имя отслеживаемого свойства</param>
        /// <param name="handler">Извлекаемый обработчик события</param>
        /// <returns>Истина, если обработчик события удалён успешно</returns>
        protected bool PropertyChanged_RemoveHandler(string PropertyName, Action handler)
        {
            lock (_PropertiesDependencesSyncRoot)
            {
                if (_PropertyChangedHandlers == null || _PropertyChangedHandlers.Count == 0 || !_PropertyChangedHandlers.TryGetValue(PropertyName, out var h)) return false;
                // ReSharper disable once DelegateSubtraction
                h -= handler;
                if (h == null)
                    _PropertyChangedHandlers.Remove(PropertyName);
                else
                    _PropertyChangedHandlers[PropertyName] = h;
                return true;
            }
        }

        /// <summary>Очистка обработчиков изменений свойства</summary>
        /// <returns>Истина, если очистка произведена успешно</returns>
        protected bool PropertyChanged_ClearHandlers(string PropertyName)
        {
            lock (_PropertiesDependencesSyncRoot)
                return _PropertyChangedHandlers != null && _PropertyChangedHandlers.Count > 0 &&
                       _PropertyChangedHandlers.Remove(PropertyName);
        }

        /// <summary>Очистка обработчиков изменений всех свойств</summary>
        /// <returns>Истина, если очистка произведена успешно</returns>
        protected virtual bool PropertyChanged_ClearHandlers()
        {
            lock (_PropertiesDependencesSyncRoot)
            {
                if (_PropertyChangedHandlers == null || _PropertyChangedHandlers.Count == 0) return false;
                _PropertyChangedHandlers.Clear();
                _PropertyChangedHandlers = null;
                return true;
            }
        }

        /// <summary>Отсоединить обработчик события <see cref="PropertyChanged"/></summary>
        /// <param name="handler">Отсоединяемый обработчик события <see cref="PropertyChanged"/></param>
        protected virtual void PropertyChanged_RemoveHanddler(PropertyChangedEventHandler handler) => PropertyChangedEvent -= handler;

        /// <summary>Получить перечисление всех объектов, подписанных на событие <see cref="PropertyChanged"/></summary>
        /// <typeparam name="T">Тип интересующих объектов</typeparam>
        /// <returns></returns>
        protected IEnumerable<T> GetPropertyChangedObservers<T>() => PropertyChangedEvent?.GetInvocationList().Select(i => i.Target).OfType<T>() ?? Enumerable.Empty<T>();

        /// <summary>Получить перечисление всех методов, подписанных на событие <see cref="PropertyChanged"/></summary>
        /// <returns>Перечисление всех методов-подписчиков события <see cref="PropertyChanged"/></returns>
        protected IEnumerable<PropertyChangedEventHandler> GetPropertyChangedObserversMethods() =>
            PropertyChangedEvent?.GetInvocationList().Cast<PropertyChangedEventHandler>() ?? Enumerable.Empty<PropertyChangedEventHandler>();

        /// <summary>Получить перечисление всех методов, подписанных на событие <see cref="PropertyChanged"/></summary>
        /// <typeparam name="T">Тип интересующих объектов</typeparam>
        /// <returns>Перечисление всех методов-подписчиков события <see cref="PropertyChanged"/> для объекта типа <typeparamref name="T"/></returns>
        protected IEnumerable<PropertyChangedEventHandler> GetPropertyChangedObserversMethods<T>() =>
            PropertyChangedEvent?.GetInvocationList().Where(i => i.Target is T).Cast<PropertyChangedEventHandler>() ?? Enumerable.Empty<PropertyChangedEventHandler>();


        [NotNull] private readonly object _PropertiesDependencesSyncRoot = new object();
        /// <summary>Словарь графа зависимости изменений свйоств</summary>
        [CanBeNull] private Dictionary<string, List<string>> _PropertiesDependencesDictionary;

        /// <summary>Добавить зависимости между свйоствами</summary>
        /// <param name="PropertyName">Имя исходного свйоства</param>
        /// <param name="Dependences">Перечисление свйоств, на которые исходное свойство имеет влияние</param>
        protected void PropertyDependence_Add(string PropertyName, params string[] Dependences)
        {
            // Если не указано имя свойства, то это ошибка
            if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

            // Блокируем критическую секцию для многопоточных операций
            lock (_PropertiesDependencesSyncRoot)
            {
                // Если словарь зависимостей не существует, то создаём новый
                Dictionary<string, List<string>> dependences_dictionary;
                if (_PropertiesDependencesDictionary is null)
                {
                    dependences_dictionary = new Dictionary<string, List<string>>();
                    _PropertiesDependencesDictionary = dependences_dictionary;
                }
                else dependences_dictionary = _PropertiesDependencesDictionary;

                // Извлекаем из словаря зависимостей список зависящих от указанного свойства свойств (если он не существует, то создаём новый
                var dependences = dependences_dictionary.GetValueOrAddNew(PropertyName, () => new List<string>());

                // Перебираем все зависимые свойства среди указанных исключая исходное свойство
                foreach (var dependence_property in Dependences.Where(name => name != PropertyName))
                {
                    // Если список зависимостей уже содержит зависящее свойство, то пропускаем его
                    if (dependences.Contains(dependence_property)) continue;
                    // Проверяем возможные циклы зависимостей
                    var invoke_queue = IsLoopDependency(PropertyName, dependence_property);
                    if (invoke_queue != null) // Если цикл найден, то это ошибка
                        throw new InvalidOperationException($"Попытка добавить зависимость между свойством {PropertyName} и (->) {dependence_property} вызывающую петлю зависимости [{string.Join(">", invoke_queue)}]");

                    // Добавляем свойство в список зависимостей
                    dependences.Add(dependence_property);

                    foreach (var other_property in dependences_dictionary.Keys.Where(name => name != PropertyName))
                    {
                        var d = dependences_dictionary[other_property];
                        if (!d.Contains(PropertyName)) continue;
                        invoke_queue = IsLoopDependency(other_property, dependence_property);
                        if (invoke_queue != null) // Если цикл найден, то это ошибка
                            throw new InvalidOperationException($"Попытка добавить зависимость между свойством {other_property} и (->) {dependence_property} вызывающую петлю зависимости [{string.Join(">", invoke_queue)}]");

                        d.Add(dependence_property);
                    }
                }
            }
        }

        /// <summary>Проверка модели на циклические зависимости между свойствами</summary>
        /// <param name="property">Проверяемое свойство</param>
        /// <param name="next_property">Следующее свойство в цепочке зависимости</param>
        /// <returns>Истина, если найден цикл</returns>
        private Queue<string> IsLoopDependency(string property, string dependence, string next_property = null, Stack<string> invoke_stack = null)
        {
            if (invoke_stack is null) invoke_stack = new Stack<string> { property };
            if (string.Equals(property, next_property)) return invoke_stack.ToQueueReverse().AddValue(property);
            var check_property = next_property ?? dependence;
            if (!_PropertiesDependencesDictionary.TryGetValue(check_property, out var dependence_properties)) return null;
            foreach (var dependence_property in dependence_properties)
            {
                var invoke_queue = IsLoopDependency(property, dependence, dependence_property, invoke_stack.AddValue(check_property));
                if (invoke_queue != null) return invoke_queue;
            }
            invoke_stack.Pop();
            return null;
        }

        /// <summary>Удаление зависимости между свйоствами</summary>
        /// <param name="PropertyName">Исходное свойство</param>
        /// <param name="Dependence">Свйоство, связь с которым надо разорвать</param>
        /// <returns>Истина, если связь успено удалена, ложь - если связь отсутствовала</returns>
        protected bool PropertyDependences_Remove(string PropertyName, string Dependence)
        {
            lock (_PropertiesDependencesSyncRoot)
            {
                if (_PropertiesDependencesDictionary?.ContainsKey(PropertyName) != true) return false;
                var dependences = _PropertiesDependencesDictionary[PropertyName];
                var result = dependences.Remove(Dependence);
                if (dependences.Count == 0)
                    _PropertiesDependencesDictionary.Remove(PropertyName);
                return result;
            }
        }

        /// <summary>Очистить граф зависимостей между свйоствами для указанного свйоства</summary>
        /// <param name="PropertyName">Название свйоства, связи которого нао удалить</param>
        protected void PropertyDependences_Clear(string PropertyName)
        {
            lock (_PropertiesDependencesSyncRoot)
                _PropertiesDependencesDictionary?.Remove(PropertyName);
        }

        /// <summary>Метод генерации события изменения значения свойства</summary>
        /// <param name="PropertyName">Имя изменившегося свойства</param>
        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual void OnPropertyChanged([NotNull] [CallerMemberName] string PropertyName = null)
        {
            var handlers = PropertyChangedEvent;
            handlers.Start(this, PropertyName);
            if (PropertyName is null) return;
            string[] dependences = null;
            var properties_dependences_dictionary = _PropertiesDependencesDictionary;
            if (properties_dependences_dictionary != null)
                lock (properties_dependences_dictionary)
                    if (properties_dependences_dictionary.ContainsKey(PropertyName))
                        dependences = properties_dependences_dictionary[PropertyName].Where(name => name != PropertyName).ToArray();
            var dependency_handlers = _PropertyChangedHandlers;
            if (dependency_handlers != null && dependency_handlers.TryGetValue(PropertyName, out var handler)) handler?.Invoke();
            if (dependences == null) return;
            handlers.Start(this, dependences);
            if (dependency_handlers != null)
                foreach (var dependence in dependences)
                    if (dependency_handlers.TryGetValue(dependence, out handler)) handler?.Invoke();
        }

        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual void OnPropertyChanged_Simple([NotNull] [CallerMemberName] string PropertyName = null) => 
            PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        /// <summary>Словарь, хранящий время последней генерации события изменения указанного свйоства в асинхронном режиме</summary>
        [NotNull] private readonly Dictionary<string, DateTime> _PropertyAsyncInvokeTime = new Dictionary<string, DateTime>();

        /// <summary>Асинхронная генерация события изменения свойства с возможностью указания таймаута ожидания повторных изменений</summary>
        /// <param name="PropertyName">Имя свйоства</param>
        /// <param name="Timeout">Таймаут ожидания повторных изменений, прежде чем событие будет сгенерировано</param>
        /// <param name="OnChanging">Метод, выполняемый до генерации события</param>
        /// <param name="OnChanged">Метод, выполняемый после генерации события</param>
        protected async void OnPropertyChangedAsync([NotNull] string PropertyName, int Timeout = 0, [CanBeNull] Action OnChanging = null, [CanBeNull] Action OnChanged = null)
        {
            if (Timeout == 0)
            {
                OnPropertyChanged(PropertyName);
                return;
            }
            var now = DateTime.Now;
            if (_PropertyAsyncInvokeTime.TryGetValue(PropertyName, out var last_call_time) && (now - last_call_time).TotalMilliseconds < Timeout)
            {
                _PropertyAsyncInvokeTime[PropertyName] = now;
                return;
            }

            _PropertyAsyncInvokeTime[PropertyName] = now;
            var delta = Timeout - (DateTime.Now - _PropertyAsyncInvokeTime[PropertyName]).TotalMilliseconds;
            while (delta > 0)
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                await Task.Delay(TimeSpan.FromMilliseconds(delta));
                delta = Timeout - (DateTime.Now - _PropertyAsyncInvokeTime[PropertyName]).TotalMilliseconds;
            }
            OnChanging?.Invoke();
            OnPropertyChanged(PropertyName);
            OnChanged?.Invoke();
        }

        #endregion

        /// <summary>Инициализация новой view-модели</summary>
        /// <param name="check_dependences">Создавать карту зависимостей на основе атрибутов</param>
        protected ViewModel(bool check_dependences = true)
        {
            if (!check_dependences) return;
            var type = GetType();
            foreach (var property in type.GetProperties())
            {
                foreach (var depends_on_attribute in property.GetCustomAttributes(typeof(DependencyOnAttribute), true).OfType<DependencyOnAttribute>())
                    PropertyDependence_Add(depends_on_attribute.Name, property.Name);
                foreach (var affects_the_attribute in property.GetCustomAttributes(typeof(AffectsTheAttribute), true).OfType<AffectsTheAttribute>())
                    PropertyDependence_Add(property.Name, affects_the_attribute.Name);
                foreach (var changed_handler_attribute in property.GetCustomAttributes(typeof(ChangedHandlerAttribute), true).OfType<ChangedHandlerAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.MethodName)))
                {
                    var handler = type.GetMethod(changed_handler_attribute.MethodName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
                    if (handler == null) throw new InvalidOperationException(
                        $"Для свойства {property.Name} определён аттрибут {typeof(ChangedHandlerAttribute).Name}, но в классе {type.Name} отсутствует " +
                        $"указанный в аттрибуте метод реакции на изменеие значения свйоства {changed_handler_attribute.MethodName}");
                    PropertyChanged_AddHandler(property.Name, (Action)Delegate.CreateDelegate(typeof(Action), this, handler));
                }
            }
        }

        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || OnPropertyChanging(field, ref value, PropertyName)) return false;
            field = value;
            if (!string.IsNullOrWhiteSpace(PropertyName)) OnPropertyChanged(PropertyName);
            return true;
        }

        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="value_check">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_check, [NotNull] [CallerMemberName] string PropertyName = null)
            => value_check(value) && Set(ref field, value, PropertyName);

        /// <summary>Установить значение поля модели, в котором хранится значение изменяющегося свойства</summary>
        /// <typeparam name="T">Тип значения поля</typeparam>
        /// <param name="field">Ссылка на поле модели</param>
        /// <param name="value">Значение, устанавливаемое для поля</param>
        /// <param name="value_check">Метод определения области допустимых значений (должен вернуть истину для корректного значения)</param>
        /// <param name="ErrorMessage">Сообщение, записываемое в генерируемое исключение <see cref="ArgumentOutOfRangeException"/> в случае если проверка <paramref name="value_check"/> не пройдена</param>
        /// <param name="PropertyName">Имя метода, вызывавшего обновление. По умолчанию должно быть равно пустоте</param>
        /// <returns>Истина, если метод изменил значение поля и вызвал событие <see cref="PropertyChanged"/></returns>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_check, [NotNull] string ErrorMessage, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (!value_check(value)) throw new ArgumentOutOfRangeException(nameof(value), ErrorMessage);
            return Set(ref field, value, PropertyName);
        }

        /// <summary>Метод установки значения свйоства, осуществляющий генерацию события изменения свйоства</summary>
        /// <typeparam name="T">Тип знначения свйоства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свйоства</param>
        /// <param name="value">Значение свйоства, которое надо установить</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свйоства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CanBeNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            if (!string.IsNullOrWhiteSpace(PropertyName))
                OnPropertyChanged?.Invoke(PropertyName);
            return true;
        }

        /// <summary>Метод установки значения свйоства, осуществляющий генерацию события изменения свйоства</summary>
        /// <typeparam name="T">Тип знначения свйоства</typeparam>
        /// <param name="field">Ссылка на поле, хранящее значение свйоства</param>
        /// <param name="value">Значение свйоства, которое надо установить</param>
        /// <param name="PropertyName">Имя свойства</param>
        /// <returns>Истина, если значение свйоства установлено успешно</returns>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static bool Set<T>([CanBeNull] ref T field, [CanBeNull] T value, [CanBeNull] Action<string> OnPropertyChanged, [NotNull] Func<T, bool> value_check, [NotNull] [CallerMemberName] string PropertyName = null)
            => value_check(value) && Set(ref field, value, OnPropertyChanged, PropertyName);


        [NotNull]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return new SetStaticValueResult<T>(false, field, field, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        [NotNull]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static SetStaticValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, Func<T, bool> value_checker, [NotNull] Action<string> OnPropertyChanged, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || !value_checker(value)) return new SetStaticValueResult<T>(false, field, value, OnPropertyChanged);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetStaticValueResult<T>(true, old_value, value, OnPropertyChanged);
        }

        [NotNull]
        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return new SetValueResult<T>(false, field, field, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        [NotifyPropertyChangedInvocator]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        protected virtual SetValueResult<T> SetValue<T>([CanBeNull] ref T field, [CanBeNull] T value, [NotNull] Func<T, bool> value_checker, [NotNull] [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value) || !value_checker(value)) return new SetValueResult<T>(false, field, value, this);
            var old_value = field;
            field = value;
            OnPropertyChanged(PropertyName);
            return new SetValueResult<T>(true, old_value, value, this);
        }

        public class SetValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly ViewModel _Model;

            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] ViewModel model) : this(Result, OldValue, OldValue, model) { }
            internal SetValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] ViewModel model)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _Model = model;
            }

            public bool Then([NotNull] Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            [NotNull]
            public SetValueResult<T> Update([NotNull] string PropertyName)
            {
                _Model.OnPropertyChanged(PropertyName);
                return this;
            }

            [NotNull]
            public SetValueResult<T> Update([NotNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _Model.OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] Action execute)
            {
                execute();
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }
        }

        public class SetStaticValueResult<T>
        {
            private readonly bool _Result;
            [CanBeNull] private readonly T _OldValue;
            [CanBeNull] private readonly T _NewValue;
            [NotNull] private readonly Action<string> _OnPropertyChanged;

            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [NotNull] Action<string> OnPropertyChanged) : this(Result, OldValue, OldValue, OnPropertyChanged) { }
            internal SetStaticValueResult(bool Result, [CanBeNull] T OldValue, [CanBeNull] T NewValue, [NotNull] Action<string> OnPropertyChanged)
            {
                _Result = Result;
                _OldValue = OldValue;
                _NewValue = NewValue;
                _OnPropertyChanged = OnPropertyChanged;
            }

            public bool Then([NotNull] Action execute)
            {
                if (_Result) execute();
                return _Result;
            }

            public bool Then([NotNull] Action<T> execute)
            {
                if (_Result) execute(_NewValue);
                return _Result;
            }

            public bool Then([NotNull] Action<T, T> execute)
            {
                if (_Result) execute(_OldValue, _NewValue);
                return _Result;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull] string PropertyName)
            {
                _OnPropertyChanged(PropertyName);
                return this;
            }

            [NotNull]
            public SetStaticValueResult<T> Update([NotNull, ItemCanBeNull] params string[] PropertyName)
            {
                foreach (var name in PropertyName) _OnPropertyChanged(name);
                return this;
            }

            public bool AnywayThen([NotNull] Action execute)
            {
                execute();
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<bool> execute)
            {
                execute(_Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T> execute)
            {
                execute(_NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, bool> execute)
            {
                execute(_NewValue, _Result);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T> execute)
            {
                execute(_OldValue, _NewValue);
                return _Result;
            }
            public bool AnywayThen([NotNull] Action<T, T, bool> execute)
            {
                execute(_OldValue, _NewValue, _Result);
                return _Result;
            }
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Признак того, что объект уже уничтожен</summary>
        private bool _Disposed;

        /// <summary>Освобождение ресурсов</summary>
        /// <param name="disposing">Если истина, то требуется освободить управляемые объекты. Освободить неуправляемыые объекты в любом случае</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed) return;
            if (disposing) DisposeManagedObject();
            DisposeUnmanagedObject();
            _Disposed = true;
        }

        /// <summary>Освободить управляемые объекты</summary>
        protected virtual void DisposeManagedObject() { }
        /// <summary>Освободить неуправляемые объекты</summary>
        protected virtual void DisposeUnmanagedObject() { }

        #endregion
    }
}
