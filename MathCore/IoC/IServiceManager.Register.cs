using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.IoC
{
    public partial interface IServiceManager
    {
        [NotNull] ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TServiceType : class;

        [NotNull] ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TService : class, TServiceInterface;

        [NotNull] ServiceRegistration<TService> Register<TService>(
            [NotNull] Func<TService> FactoryMethod, in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) 
            where TService : class;

        /// <summary>Зарегистрировать тип сервиса</summary>
        /// <param name="ServiceType">Тип регистрируемого сервиса</param>
        /// <param name="mode">Режим регистрации</param>
        /// <returns>Объект регистрации</returns>
        [NotNull]
        ServiceRegistration RegisterType(
            [NotNull] Type ServiceType,
            ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton);

        /// <summary>Зарегистрировать интерфейс сервиса</summary>
        /// <param name="InterfaceType">Тип интерфейса сервиса</param>
        /// <param name="ServiceType">Тип реализации интерфейса сервиса</param>
        /// <param name="mode">Режим регистрации</param>
        /// <returns>Объект регистрации</returns>
        [NotNull]
        ServiceRegistration RegisterType(
            [CanBeNull] Type InterfaceType,
            [NotNull] Type ServiceType,
            ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton);

        [NotNull] MapServiceRegistration Map<TService, TMapService>() where TService : class where TMapService : class;

        bool Unregister<TService>();
        bool Unregister(Type ServiceType);

        bool ServiceRegistered<TService>();
        bool ServiceRegistered(Type ServiceType);
    }
}