#nullable enable

using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public partial interface IServiceManager
{
    ServiceRegistration RegisterSingleton(Type ServiceType);
        
    ServiceRegistration RegisterSingleton(Type InterfaceType, Type ServiceType);
        
    SingletonServiceRegistration<TService> RegisterSingleton<TService>() where TService : class;
        
    SingletonServiceRegistration<TService> RegisterSingleton<TService>(TService ServiceInstance) where TService : class;
        
    SingletonServiceRegistration<TService> RegisterSingleton<TServiceInterface, TService>() where TService : class, TServiceInterface;
        
    SingletonServiceRegistration<TService> RegisterSingleton<TService>(Func<TService> FactoryMethod) where TService : class;
}