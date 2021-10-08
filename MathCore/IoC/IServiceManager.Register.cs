using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.IoC
{
    public partial interface IServiceManager
    {
        /// <summary>Зарегистрировать тип сервиса</summary>
        /// <typeparam name="TServiceType">Регистрируемый тип реализации</typeparam>
        /// <param name="mode">Режим регистрации</param>
        /// <returns>Регистрация тпа</returns>
        [NotNull] ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TServiceType : class;

        /// <summary>Зарегистрировать интерфейс сервиса с указанием его реализации</summary>
        /// <typeparam name="TServiceInterface">Тип интерфейса сервиса</typeparam>
        /// <typeparam name="TService">Тип реализации сервиса</typeparam>
        /// <param name="mode">Режим регистрации</param>
        /// <returns>Регистрация сервиса</returns>
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

        /// <summary>Отменить регистрацию типа сервиса</summary>
        /// <typeparam name="TService">Тип реализации сервиса, регистрацию которого надо отменить</typeparam>
        /// <returns>Истина, если регистрация сервиса отменена успешно</returns>
        bool Unregister<TService>();

        /// <summary>Отменить регистрацию типа сервиса</summary>
        /// <param name="ServiceType">Тип реализации сервиса, регистрацию которого надо отменить</param>
        /// <returns>Истина, если регистрация сервиса отменена успешно</returns>
        bool Unregister(Type ServiceType);

        /// <summary>Зарегистрирован ли сервис</summary>
        /// <typeparam name="TService">Тип проверяемого сервиса</typeparam>
        /// <returns>Истина, если сервис зарегистрирован у менеджера</returns>
        bool ServiceRegistered<TService>();

        /// <summary>Зарегистрирован ли сервис</summary>
        /// <param name="ServiceType">Тип проверяемого сервиса</param>
        /// <returns>Истина, если сервис зарегистрирован у менеджера</returns>
        bool ServiceRegistered(Type ServiceType);
    }
}