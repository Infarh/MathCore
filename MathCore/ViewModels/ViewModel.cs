#nullable enable
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

using MathCore.Annotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedType.Global
// ReSharper disable AssignNullToNotNullAttribute

// ReSharper disable once CheckNamespace
namespace MathCore.ViewModels;

/// <summary>Наблюдаемая модель</summary>
public partial class ViewModel : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
{
    #region INotifyPropertyChanging

    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>Генерация события находящегося в процессе изменения значения свойства</summary>
    /// <param name="OldValue">Предыдущее значение свойства</param>
    /// <param name="NewValue">Новое значение свойства</param>
    /// <param name="PropertyName">Имя изменившегося свойства (если не указано, то берётся имя вызывающего метода)</param>
    /// <typeparam name="T">Тип значения свойства</typeparam>
    /// <returns>Истина, если событие было обработано и новое значение свойства не равно старому</returns>
    protected virtual bool OnPropertyChanging<T>(
        T OldValue, 
        ref T? NewValue, 
        [CallerMemberName] in string PropertyName = null!)
    {
        if (PropertyChanging is not { } handlers) return false;
        var args = new PropertyChangingEventArgs<T>(OldValue, NewValue, PropertyName);
        handlers(this, args);
        NewValue = args.NewValue;
        return !Equals(OldValue, NewValue);
    }

    #endregion

    #region INotifyPropertyChanged 

    private event PropertyChangedEventHandler? PropertyChangedEvent;

    /// <summary>Событие возникает когда изменяется значение свойства объекта</summary>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            lock (_PropertiesDependenciesSyncRoot)
                PropertyChanged_AddHandler(value);
        }
        remove
        {
            lock (_PropertiesDependenciesSyncRoot)
                PropertyChanged_RemoveHandler(value);
        }
    }

    /// <summary>Присоединить обработчик события <see cref="PropertyChanged"/></summary>
    /// <param name="handler">Присоединяемый обработчик события <see cref="PropertyChanged"/></param>
    protected virtual void PropertyChanged_AddHandler(in PropertyChangedEventHandler handler) => PropertyChangedEvent += handler;

    /// <summary>Словарь обработчиков событий изменений свойств</summary>
    private Dictionary<string, Action>? _PropertyChangedHandlers;

    /// <summary>Добавление обработчика события изменения свойства</summary>
    /// <param name="PropertyName">Имя отслеживаемого события</param>
    /// <param name="handler">Устанавливаемый обработчик</param>
    protected void PropertyChanged_AddHandler(in string PropertyName, in Action handler)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            var handlers = _PropertyChangedHandlers ??= [];
            if (handlers.TryGetValue(PropertyName, out var h)) handlers[PropertyName] = h + handler;
            else handlers.Add(PropertyName, handler);
        }
    }

    /// <summary>Извлечение обработчика события изменения указанного свойства</summary>
    /// <param name="PropertyName">Имя отслеживаемого свойства</param>
    /// <param name="handler">Извлекаемый обработчик события</param>
    /// <returns>Истина, если обработчик события удалён успешно</returns>
    protected bool PropertyChanged_RemoveHandler(in string PropertyName, in Action handler)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertyChangedHandlers is not { Count: > 0 } || !_PropertyChangedHandlers.TryGetValue(PropertyName, out var h)) 
                return false;
            // ReSharper disable once DelegateSubtraction
            h -= handler;
            if (h is null)
                _PropertyChangedHandlers.Remove(PropertyName);
            else
                _PropertyChangedHandlers[PropertyName] = h;
            return true;
        }
    }

    /// <summary>Очистка обработчиков изменений свойства</summary>
    /// <returns>Истина, если очистка произведена успешно</returns>
    protected bool PropertyChanged_ClearHandlers(in string PropertyName)
    {
        lock (_PropertiesDependenciesSyncRoot)
            return _PropertyChangedHandlers is { Count: > 0 } &&
                _PropertyChangedHandlers.Remove(PropertyName);
    }

    /// <summary>Очистка обработчиков изменений всех свойств</summary>
    /// <returns>Истина, если очистка произведена успешно</returns>
    protected virtual bool PropertyChanged_ClearHandlers()
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertyChangedHandlers is not { Count: > 0 }) return false;
            _PropertyChangedHandlers.Clear();
            _PropertyChangedHandlers = null;
            return true;
        }
    }

    /// <summary>Отсоединить обработчик события <see cref="PropertyChanged"/></summary>
    /// <param name="handler">Отсоединяемый обработчик события <see cref="PropertyChanged"/></param>
    protected virtual void PropertyChanged_RemoveHandler(in PropertyChangedEventHandler handler) => PropertyChangedEvent -= handler;

    /// <summary>Получить перечисление всех объектов, подписанных на событие <see cref="PropertyChanged"/></summary>
    /// <typeparam name="T">Тип интересующих объектов</typeparam>
    /// <returns>Перечисление объектов-подписчиков события <see cref="PropertyChanged"/></returns>
    protected IEnumerable<T> GetPropertyChangedObservers<T>() => 
        PropertyChangedEvent?
           .GetInvocationList()
           .Select(i => i.Target)
           .OfType<T>() 
        ?? [];

    /// <summary>Получить перечисление всех методов, подписанных на событие <see cref="PropertyChanged"/></summary>
    /// <returns>Перечисление всех методов-подписчиков события <see cref="PropertyChanged"/></returns>
    protected IEnumerable<PropertyChangedEventHandler> GetPropertyChangedObserversMethods() =>
        PropertyChangedEvent?
           .GetInvocationList()
           .Cast<PropertyChangedEventHandler>() 
        ?? [];

    /// <summary>Получить перечисление всех методов, подписанных на событие <see cref="PropertyChanged"/></summary>
    /// <typeparam name="T">Тип интересующих объектов</typeparam>
    /// <returns>Перечисление всех методов-подписчиков события <see cref="PropertyChanged"/> для объекта типа <typeparamref name="T"/></returns>
    protected IEnumerable<PropertyChangedEventHandler> GetPropertyChangedObserversMethods<T>() =>
        PropertyChangedEvent?
           .GetInvocationList()
           .Where(i => i.Target is T)
           .Cast<PropertyChangedEventHandler>() 
        ?? [];


    private readonly object _PropertiesDependenciesSyncRoot = new();
    /// <summary>Словарь графа зависимости изменений свойств</summary>
    private Dictionary<string, List<string>>? _PropertiesDependenciesDictionary;

    /// <summary>Добавить зависимости между свойствами</summary>
    /// <param name="PropertyName">Имя исходного свойства</param>
    /// <param name="Dependencies">Перечисление свойств, на которые исходное свойство имеет влияние</param>
    protected void PropertyDependence_Add(string PropertyName, params string[] Dependencies)
    {
        // Если не указано имя свойства, то это ошибка
        if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName));

        // Блокируем критическую секцию для многопоточных операций
        lock (_PropertiesDependenciesSyncRoot)
        {
            // Если словарь зависимостей не существует, то создаём новый
            Dictionary<string, List<string>> dependencies_dictionary;
            if (_PropertiesDependenciesDictionary is null)
            {
                dependencies_dictionary           = [];
                _PropertiesDependenciesDictionary = dependencies_dictionary;
            }
            else dependencies_dictionary = _PropertiesDependenciesDictionary;

            // Извлекаем из словаря зависимостей список зависящих от указанного свойства свойств (если он не существует, то создаём новый
            var dependencies = dependencies_dictionary.GetValueOrAddNew(PropertyName, () => []);

            // Перебираем все зависимые свойства среди указанных исключая исходное свойство
            foreach (var dependence_property in Dependencies.Where(name => name != PropertyName))
            {
                // Если список зависимостей уже содержит зависящее свойство, то пропускаем его
                if (dependencies.Contains(dependence_property)) continue;
                // Проверяем возможные циклы зависимостей
                var invoke_queue = IsLoopDependency(PropertyName, dependence_property);
                if (invoke_queue != null) // Если цикл найден, то это ошибка
                    throw new InvalidOperationException($"Попытка добавить зависимость между свойством {PropertyName} и (->) {dependence_property} вызывающую петлю зависимости [{string.Join(">", invoke_queue)}]");

                // Добавляем свойство в список зависимостей
                dependencies.Add(dependence_property);

                foreach (var other_property in dependencies_dictionary.Keys.Where(name => name != PropertyName))
                {
                    var d = dependencies_dictionary[other_property];
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
    /// <param name="dependency">Зависимость</param>
    /// <param name="next_property">Следующее свойство в цепочке зависимости</param>
    /// <param name="invoke_stack">Стек вызова свойств</param>
    /// <returns>Истина, если найден цикл</returns>
    private Queue<string>? IsLoopDependency(
        in string property, 
        in string dependency, 
        in string? next_property = null, 
        Stack<string>? invoke_stack = null)
    {
        if(_PropertiesDependenciesDictionary is null) throw new InvalidOperationException("Отсутствует словарь свойств-зависимости");
        invoke_stack ??= [property];

        if (string.Equals(property, next_property)) 
            return invoke_stack.ToQueueReverse().AddValue(property);

        var check_property = next_property ?? dependency;
        if(_PropertiesDependenciesDictionary is null) 
            throw new InvalidOperationException("Отсутствует словарь свойств-зависимости");

        if (!_PropertiesDependenciesDictionary.TryGetValue(check_property, out var dependence_properties)) 
            return null;

        foreach (var dependence_property in dependence_properties)
            if (IsLoopDependency(property, dependency, dependence_property, invoke_stack.AddValue(check_property)) is { } invoke_queue) 
                return invoke_queue;

        invoke_stack.Pop();
        return null;
    }

    /// <summary>Удаление зависимости между свойствами</summary>
    /// <param name="PropertyName">Исходное свойство</param>
    /// <param name="Dependence">Свойство, связь с которым надо разорвать</param>
    /// <returns>Истина, если связь успешно удалена, ложь - если связь отсутствовала</returns>
    protected bool PropertyDependencies_Remove(in string PropertyName, in string Dependence)
    {
        lock (_PropertiesDependenciesSyncRoot)
        {
            if (_PropertiesDependenciesDictionary?.ContainsKey(PropertyName) != true) 
                return false;

            var dependencies = _PropertiesDependenciesDictionary[PropertyName];
            var result       = dependencies.Remove(Dependence);
            if (dependencies.Count == 0)
                _PropertiesDependenciesDictionary.Remove(PropertyName);
            return result;
        }
    }

    /// <summary>Очистить граф зависимостей между свойствами для указанного свойства</summary>
    /// <param name="PropertyName">Название свойства, связи которого нао удалить</param>
    protected void PropertyDependencies_Clear(in string PropertyName)
    {
        lock (_PropertiesDependenciesSyncRoot)
            _PropertiesDependenciesDictionary?.Remove(PropertyName);
    }

    /// <summary>Метод генерации события изменения значения свойства</summary>
    /// <param name="PropertyName">Имя изменившегося свойства</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null!)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (PropertyName is null) return; // Если имя свойства не указано, то выход
        if (_PropertyChangedEventsSuppressor != null)
        {
            _PropertyChangedEventsSuppressor.RegisterEvent(PropertyName);
            return;
        }

        var handlers = PropertyChangedEvent;
        handlers.Start(this, PropertyName);
        string[]? dependencies                      = null;
        var      properties_dependencies_dictionary = _PropertiesDependenciesDictionary;
        if (properties_dependencies_dictionary != null)
            lock (properties_dependencies_dictionary)
                if (properties_dependencies_dictionary.ContainsKey(PropertyName))
                    dependencies = properties_dependencies_dictionary[PropertyName].Where(name => name != PropertyName).ToArray();
        var dependency_handlers = _PropertyChangedHandlers;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (dependency_handlers is not null && dependency_handlers.TryGetValue(PropertyName, out var handler)) handler?.Invoke();
        if (dependencies is null) return;
        handlers.Start(this, dependencies);
        if (dependency_handlers != null)
            foreach (var dependence in dependencies)
                if (dependency_handlers.TryGetValue(dependence, out handler)) handler?.Invoke();
    }

    /// <summary>Словарь, хранящий время последней генерации события изменения указанного свойства в асинхронном режиме</summary>
    private readonly Dictionary<string, DateTime> _PropertyAsyncInvokeTime = [];

    /// <summary>Асинхронная генерация события изменения свойства с возможностью указания таймаута ожидания повторных изменений</summary>
    /// <param name="PropertyName">Имя свойства</param>
    /// <param name="Timeout">Таймаут ожидания повторных изменений, прежде чем событие будет сгенерировано</param>
    /// <param name="OnChanging">Метод, выполняемый до генерации события</param>
    /// <param name="OnChanged">Метод, выполняемый после генерации события</param>
    protected async void OnPropertyChangedAsync(
        string PropertyName, 
        int Timeout = 0,
        Action? OnChanging = null, 
        Action? OnChanged = null)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (PropertyName is null) return; // Если имя свойства не указано, то выход
        if (Timeout == 0)
        {
            OnPropertyChanged(PropertyName);
            return;
        }

        if (_PropertyChangedEventsSuppressor != null)
        {
            _PropertyChangedEventsSuppressor.RegisterEvent(PropertyName);
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
            await Task.Delay(TimeSpan.FromMilliseconds(delta)).ConfigureAwait(false);
            delta = Timeout - (DateTime.Now - _PropertyAsyncInvokeTime[PropertyName]).TotalMilliseconds;
        }
        OnChanging?.Invoke();
        OnPropertyChanged(PropertyName);
        OnChanged?.Invoke();
    }

    #endregion

    /// <summary>Инициализация новой view-модели</summary>
    /// <param name="CheckDependencies">Создавать карту зависимостей на основе атрибутов</param>
    protected ViewModel(bool CheckDependencies = true)
    {
        if (!CheckDependencies) return;
        var type = GetType();
        foreach (var property in type.GetProperties())
        {
            foreach (var depends_on_attribute in property.GetCustomAttributes(typeof(DependencyOnAttribute), true).OfType<DependencyOnAttribute>())
                PropertyDependence_Add(depends_on_attribute.Name, property.Name);
            foreach (var affects_the_attribute in property.GetCustomAttributes(typeof(AffectsTheAttribute), true).OfType<AffectsTheAttribute>())
                PropertyDependence_Add(property.Name, affects_the_attribute.Name);
            foreach (var changed_handler_attribute in property.GetCustomAttributes(typeof(ChangedHandlerAttribute), true).OfType<ChangedHandlerAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.MethodName)))
            {
                var handler = type.GetMethod(changed_handler_attribute.MethodName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic) 
                    ?? throw new InvalidOperationException(
                    $"Для свойства {property.Name} определён атрибут {nameof(ChangedHandlerAttribute)}, но в классе {type.Name} отсутствует " +
                    $"указанный в атрибуте метод реакции на изменение значения свойства {changed_handler_attribute.MethodName}");
                PropertyChanged_AddHandler(property.Name, (Action)Delegate.CreateDelegate(typeof(Action), this, handler));
            }
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
    /// <param name="disposing">Если истина, то требуется освободить управляемые объекты. Освободить неуправляемые объекты в любом случае</param>
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