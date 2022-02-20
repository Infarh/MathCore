#nullable enable
namespace MathCore.IoC;

/// <summary>Объект доступа к экземплярам сервиса <typeparamref name="TService"/></summary>
/// <typeparam name="TService">Тип сервиса</typeparam>
public class ServiceManagerAccessor<TService> where TService : class
{
    /// <summary>Экземпляр менеджера сервисов</summary>
    private readonly ServiceManager _ServiceManager;

    /// <summary>Экземпляр сервиса <typeparamref name="TService"/></summary>
    public TService? Service => _ServiceManager.Get<TService>();

    public TService ServiceRequired => _ServiceManager.GetRequired<TService>();

    /// <summary>Инициализация нового объекта доступа к экземплярам сервиса <typeparamref name="TService"/></summary>
    /// <param name="ServiceManager">Менеджер сервисов</param>
    public ServiceManagerAccessor(ServiceManager ServiceManager) => _ServiceManager = ServiceManager;
}