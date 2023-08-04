#nullable enable
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public sealed partial class ServiceManager
{
    #region Регистрация сервисов

    public ServiceRegistration RegisterSingleThread(Type ServiceType)
    {
        lock (_SyncRoot)
        {
            var registration_type = typeof(SingleThreadServiceRegistration<>).MakeGenericType(ServiceType);
            var registration = (ServiceRegistration)registration_type.CreateObject(this, ServiceType);

            _Services[ServiceType] = registration;
            return registration;
        }
    }

    public ServiceRegistration RegisterSingleThread(Type InterfaceType, Type ServiceType)
    {
        lock (_SyncRoot)
        {
            var registration_type = typeof(SingleThreadServiceRegistration<>).MakeGenericType(ServiceType);
            var registration = _Services.Values.FirstOrDefault(r => r.GetType() == registration_type)
                ?? (ServiceRegistration)registration_type.CreateObject(this, ServiceType);

            _Services[InterfaceType] = registration;
            return registration;
        }
    }

    public SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>() where TService : class
    {
        var service_type = typeof(TService);
        lock (_SyncRoot)
        {
            var registration = new SingleThreadServiceRegistration<TService>(this, typeof(TService));
            _Services[service_type] = registration;
            return registration;
        }
    }

    public SingleThreadServiceRegistration<TService> RegisterSingleThread<TServiceInterface, TService>()
        where TService : class, TServiceInterface
    {
        var service_interface = typeof(TServiceInterface);
        lock (_SyncRoot)
        {
            var registration = _Services.Values.OfType<SingleThreadServiceRegistration<TService>>().FirstOrDefault()
                ?? new SingleThreadServiceRegistration<TService>(this, typeof(TService));
            _Services[service_interface] = registration;
            return registration;
        }
    }

    public SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>(Func<TService> FactoryMethod) where TService : class
    {
        var service_interface = typeof(TService);
        lock (_SyncRoot)
        {
            var registration = new SingleThreadServiceRegistration<TService>(this, service_interface, FactoryMethod);
            _Services[service_interface] = registration;
            return registration;
        }
    }

    #endregion
}