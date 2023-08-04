#nullable enable

using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public partial interface IServiceManager
{
    ServiceRegistration RegisterSingleThread(Type ServiceType);
       
    ServiceRegistration RegisterSingleThread(Type InterfaceType, Type ServiceType);
       
    SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>() where TService : class;
       
    SingleThreadServiceRegistration<TService> RegisterSingleThread<TServiceInterface, TService>() where TService : class, TServiceInterface;
       
    SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>(Func<TService> FactoryMethod) where TService : class;
}