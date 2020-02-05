using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public partial interface IServiceManager
    {
        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TService>() where TService : class;
        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TServiceInterface, TService>() where TService : class, TServiceInterface;
        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;
    }
}