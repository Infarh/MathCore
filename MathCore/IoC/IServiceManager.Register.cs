#nullable enable
using System;

using MathCore.IoC.ServiceRegistrations;

// ReSharper disable UnusedMethodReturnValue.Global

namespace MathCore.IoC;

public partial interface IServiceManager
{
    /// <summary>Зарегистрировать тип сервиса</summary>
    /// <typeparam name="TServiceType">Регистрируемый тип реализации</typeparam>
    /// <param name="Mode">Режим регистрации</param>
    /// <returns>Регистрация тпа</returns>
    ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton)
        where TServiceType : class;

    /// <summary>Зарегистрировать интерфейс сервиса с указанием его реализации</summary>
    /// <typeparam name="TServiceInterface">Тип интерфейса сервиса</typeparam>
    /// <typeparam name="TService">Тип реализации сервиса</typeparam>
    /// <param name="Mode">Режим регистрации</param>
    /// <returns>Регистрация сервиса</returns>
    ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton)
        where TService : class, TServiceInterface;

    ServiceRegistration<TService> Register<TService>(
        Func<TService> FactoryMethod, in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton) 
        where TService : class;

    /// <summary>Зарегистрировать тип сервиса</summary>
    /// <param name="ServiceType">Тип регистрируемого сервиса</param>
    /// <param name="Mode">Режим регистрации</param>
    /// <returns>Объект регистрации</returns>
    ServiceRegistration RegisterType(Type ServiceType, ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton);

    /// <summary>Зарегистрировать интерфейс сервиса</summary>
    /// <param name="InterfaceType">Тип интерфейса сервиса</param>
    /// <param name="ServiceType">Тип реализации интерфейса сервиса</param>
    /// <param name="Mode">Режим регистрации</param>
    /// <returns>Объект регистрации</returns>
    ServiceRegistration RegisterType(Type InterfaceType, Type ServiceType, ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton);

    MapServiceRegistration Map<TService, TMapService>() where TService : class where TMapService : class;

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