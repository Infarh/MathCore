// ReSharper disable UnusedAutoPropertyAccessor.Global // Отключение предупреждений для неиспользуемых автосвойств
// ReSharper disable UnusedType.Global // Отключение предупреждений для неиспользуемых типов
// ReSharper disable UnusedMember.Global // Отключение предупреждений для неиспользуемых членов

namespace MathCore.IoC.ServiceRegistrations;

/// <summary>Регистрация сервиса с единым экземпляром в рамках цепочки async/await</summary>
/// <remarks>Экземпляр сервиса изолируется по асинхронному контексту выполнения через AsyncLocal</remarks>
/// <example>
/// <code>
/// var service_manager = new ServiceManager();
/// service_manager.RegisterSingleTask&lt;MyService&gt;();
/// var service_instance = service_manager.Get(typeof(MyService));
/// </code>
/// </example>
public class SingleTaskServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
{
    private sealed class InstanceHolder
    {
        public bool IsCreated { get; set; }

        public object? Value { get; set; }
    }

    private volatile AsyncLocal<InstanceHolder?> _Initializer = null!;

    private volatile AsyncLocal<Exception?> _Exceptions = null!;

    /// <summary>Признак того, что экземпляр сервиса создан в текущем асинхронном контексте</summary>
    public bool IsInstanceCreated => _Initializer.Value is { IsCreated: true };

    /// <summary>Требуется ли освобождать экземпляр при сбросе</summary>
    public bool NeedDisposeInstance { get; set; }

    /// <summary>Текущий экземпляр сервиса в рамках цепочки async/await</summary>
    /// <returns>Экземпляр сервиса или null если он ещё не создан</returns>
    public object? CurrentInstance => IsInstanceCreated ? Instance : null;

    /// <summary>Получить экземпляр сервиса для текущего асинхронного контекста</summary>
    /// <returns>Экземпляр сервиса</returns>
    public object? Instance => GetService();

    /// <summary>Последнее исключение создания сервиса</summary>
    /// <returns>Последнее исключение или null если ошибок не было</returns>
    public override Exception? LastException { get => _Exceptions.Value; set => _Exceptions.Value = value; }

    /// <summary>Инициализировать регистрацию сервиса</summary>
    /// <param name="Manager">Менеджер сервисов</param>
    /// <param name="ServiceType">Тип сервиса</param>
    public SingleTaskServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) => ResetAll();

    /// <summary>Инициализировать регистрацию сервиса с фабричным методом</summary>
    /// <param name="Manager">Менеджер сервисов</param>
    /// <param name="ServiceType">Тип сервиса</param>
    /// <param name="FactoryMethod">Метод создания экземпляра сервиса</param>
    public SingleTaskServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) => ResetAll();

    /// <summary>Получить или создать экземпляр сервиса для текущей цепочки async/await</summary>
    /// <param name="parameters">Параметры для создания сервиса</param>
    /// <returns>Экземпляр сервиса</returns>
    /// <remarks>Создаёт экземпляр только один раз на асинхронный контекст</remarks>
    /// <example>
    /// <code>
    /// var service_manager = new ServiceManager();
    /// service_manager.RegisterSingleTask&lt;MyService&gt;();
    /// var service_instance = service_manager.Get(typeof(MyService));
    /// </code>
    /// </example>
    public override object? GetService(params object[] parameters)
    {
        var holder = _Initializer.Value;
        if (holder is null)
        {
            holder = new InstanceHolder();
            _Initializer.Value = holder;
        }

        if (!holder.IsCreated)
        {
            holder.Value = CreateNewService(parameters);
            holder.IsCreated = true;
        }

        return holder.Value;
    }

    /// <summary>Сбросить состояние экземпляров для всех асинхронных контекстов</summary>
    /// <remarks>Сбрасывает хранение экземпляров и исключений для новых цепочек async/await</remarks>
    public void ResetAll()
    {
        _Initializer = new();
        _Exceptions = new();
    }

    /// <summary>Сбросить экземпляр сервиса в текущем асинхронном контексте</summary>
    /// <remarks>При включённом NeedDisposeInstance освобождает экземпляр если он реализует IDisposable</remarks>
    public void Reset()
    {
        var holder = _Initializer.Value;
        if (NeedDisposeInstance)
            (holder?.Value as IDisposable)?.Dispose();
        if (holder is null) return;
        holder.Value = null;
        holder.IsCreated = false;
    }

    internal override ServiceRegistration CloneFor(IServiceManager manager) => _FactoryMethod is null
        ? new SingleTaskServiceRegistration<TService>(manager, ServiceType)
        : new SingleTaskServiceRegistration<TService>(manager, ServiceType, _FactoryMethod);
}