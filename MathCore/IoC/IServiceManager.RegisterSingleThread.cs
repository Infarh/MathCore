using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public partial interface IServiceManager
    {
        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>() where TService : class;
        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TServiceInterface, TService>() where TService : class, TServiceInterface;
        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;
    }
}