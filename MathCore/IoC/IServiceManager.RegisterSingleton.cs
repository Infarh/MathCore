using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public partial interface IServiceManager
    {
        [NotNull] ServiceRegistration RegisterSingleton(Type ServiceType);
        [NotNull] ServiceRegistration RegisterSingleton(Type InterfaceType, Type ServiceType);
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>() where TService : class;
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>(TService ServiceInstance) where TService : class;
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TServiceInterface, TService>() where TService : class, TServiceInterface;
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;
    }
}