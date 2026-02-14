using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public partial interface IServiceManager
{
    ServiceRegistration RegisterSingleTask(Type ServiceType);

    ServiceRegistration RegisterSingleTask(Type InterfaceType, Type ServiceType);

    SingleTaskServiceRegistration<TService> RegisterSingleTask<TService>() where TService : class;

    SingleTaskServiceRegistration<TService> RegisterSingleTask<TServiceInterface, TService>() where TService : class, TServiceInterface;

    SingleTaskServiceRegistration<TService> RegisterSingleTask<TService>(Func<TService> FactoryMethod) where TService : class;
}
