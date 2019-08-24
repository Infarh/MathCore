using System;
using System.Diagnostics.CodeAnalysis;
using MathCore.Annotations;
// ReSharper disable UnusedMemberInSuper.Global

namespace MathCore.IoC
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public interface IServiceManager : IDisposable, ICloneable<IServiceManager>
    {
        [CanBeNull] object this[[NotNull] Type ServiceType] { get; }

        IServiceRegistrations ServiceRegistrations { get; }

        bool ServiceRegistered<TService>();
        bool ServiceRegistered(Type ServiceType);

        [NotNull] ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) where TServiceType : class;

        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>() where TService : class;
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>(TService ServiceInstance) where TService : class;
        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TService>() where TService : class;
        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>() where TService : class;

        [NotNull] ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) where TService : class, TServiceInterface;

        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TServiceInterface, TService>() where TService : class, TServiceInterface;

        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TServiceInterface, TService>() where TService : class, TServiceInterface;

        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TServiceInterface, TService>() where TService : class, TServiceInterface;

        [NotNull] ServiceRegistration<TService> Register<TService>([NotNull] Func<TService> FactoryMethod, in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) where TService : class;
        [NotNull] SingletonServiceRegistration<TService> RegisterSingleton<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;
        [NotNull] SingleCallServiceRegistration<TService> RegisterSingleCall<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;
        [NotNull] SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>([NotNull] Func<TService> FactoryMethod) where TService : class;

        [NotNull] MapServiceRegistration Map<TService, TMapService>() where TService : class where TMapService : class;

        bool Unregister<TService>();
        bool Unregister(Type ServiceType);
        TServiceInterface Get<TServiceInterface>() where TServiceInterface : class;
        object Get(Type ServiceType);
        ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class;
    }
}