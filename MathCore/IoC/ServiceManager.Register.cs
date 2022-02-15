#nullable enable
using System;

using System.Linq;

using MathCore.Annotations;
using MathCore.IoC.Exceptions;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public sealed partial class ServiceManager
{
    public ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton)
        where TServiceType : class
    {
        lock (_SyncRoot)
            return Mode switch
            {
                ServiceRegistrationMode.Singleton =>
                    RegisterSingleton<TServiceType>(),
                ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceType>(),
                ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceType>(),
                _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null)
            };
    }

    #region Регистрация сервисов

    public ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton)
        where TService : class, TServiceInterface
    {
        lock (_SyncRoot)
            return Mode switch
            {
                ServiceRegistrationMode.Singleton => RegisterSingleton<TServiceInterface, TService>(),
                ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceInterface, TService>(),
                ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceInterface, TService>(),
                _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null)
            };
    }

    public ServiceRegistration<TService> Register<TService>(
        Func<TService> FactoryMethod, in ServiceRegistrationMode Mode = ServiceRegistrationMode.Singleton) where TService : class
    {
        lock (_SyncRoot)
            return Mode switch
            {
                ServiceRegistrationMode.Singleton => RegisterSingleton(FactoryMethod),
                ServiceRegistrationMode.SingleCall => RegisterSingleCall(FactoryMethod),
                ServiceRegistrationMode.SingleThread => RegisterSingleThread(FactoryMethod),
                _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null)
            };
    }

    public MapServiceRegistration Map<TService, TMapService>() where TMapService : class where TService : class
    {
        var service_type = typeof(TService);
        var map_service_type = typeof(TMapService);
        lock (_SyncRoot)
        {
            var base_registration = _Services[service_type];
            var registration = new MapServiceRegistration(this, map_service_type, base_registration);
            _Services[map_service_type] = registration;
            return registration;
        }
    }

    public ServiceRegistration RegisterType(Type ServiceType, ServiceRegistrationMode Mode) =>
        Mode switch
        {
            ServiceRegistrationMode.Singleton => RegisterTypeSingleton(ServiceType),
            ServiceRegistrationMode.SingleCall => RegisterTypeSingleCall(ServiceType),
            ServiceRegistrationMode.SingleThread => RegisterTypeSingleThread(ServiceType),
            _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null)
        };

    public ServiceRegistration RegisterType(Type InterfaceType, Type ServiceType, ServiceRegistrationMode Mode) =>
        Mode switch
        {
            ServiceRegistrationMode.Singleton => RegisterTypeSingleton(InterfaceType, ServiceType),
            ServiceRegistrationMode.SingleCall => RegisterTypeSingleCall(InterfaceType, ServiceType),
            ServiceRegistrationMode.SingleThread => RegisterTypeSingleThread(InterfaceType, ServiceType),
            _ => throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null)
        };

    private ServiceRegistration Register(Type ServiceType, Type RegistrationBaseType)
    {
        if (ServiceType is null)
            throw new ArgumentNullException(nameof(ServiceType));
        if (!ServiceType.IsClass)
            throw new ServiceRegistrationNotFoundException(ServiceType, "Регистрируемый тип сервиса не является классом");

        var type = !ServiceType.IsAbstract 
            ? RegistrationBaseType.MakeGenericType(ServiceType) 
            : throw new ServiceRegistrationNotFoundException(ServiceType, "Регистрируемый тип сервиса не может являться абстрактным классом");

        lock (_SyncRoot)
        {
            var instance = (ServiceRegistration)Activator.CreateInstance(type, this, ServiceType);
            _Services[ServiceType] = instance;
            return instance;
        }
    }

    private ServiceRegistration Register(Type? InterfaceType, Type ServiceType, Type RegistrationBaseType)
    {
        if (InterfaceType is null)
            return Register(ServiceType, RegistrationBaseType);
        if (ServiceType is null)
            throw new ArgumentNullException(nameof(ServiceType));
        if (!ServiceType.IsClass)
            throw new ServiceRegistrationNotFoundException(ServiceType, "Регистрируемый тип сервиса не является классом");

        var type = !ServiceType.IsAbstract 
            ? RegistrationBaseType.MakeGenericType(ServiceType) 
            : throw new ServiceRegistrationNotFoundException(ServiceType, "Регистрируемый тип сервиса не может являться абстрактным классом");

        lock (_SyncRoot)
        {
            var registration = _Services.Values.FirstOrDefault(r => r.ServiceType == ServiceType)
                ?? (ServiceRegistration)Activator.CreateInstance(type, this, ServiceType);
            _Services[InterfaceType] = registration;
            return registration;
        }
    }

    public ServiceRegistration RegisterTypeSingleton(Type ServiceType) => Register(ServiceType, typeof(SingletonServiceRegistration<>));

    public ServiceRegistration RegisterTypeSingleCall(Type ServiceType) => Register(ServiceType, typeof(SingleCallServiceRegistration<>));

    public ServiceRegistration RegisterTypeSingleThread(Type ServiceType) => Register(ServiceType, typeof(SingleThreadServiceRegistration<>));

    public ServiceRegistration RegisterTypeSingleton(Type InterfaceType, Type ServiceType) =>
        Register(InterfaceType, ServiceType, typeof(SingletonServiceRegistration<>));

    public ServiceRegistration RegisterTypeSingleCall(Type InterfaceType, Type ServiceType) =>
        Register(InterfaceType, ServiceType, typeof(SingleCallServiceRegistration<>));

    public ServiceRegistration RegisterTypeSingleThread(Type InterfaceType, Type ServiceType) =>
        Register(InterfaceType, ServiceType, typeof(SingleThreadServiceRegistration<>));

    #endregion

    public bool Unregister<TService>() => Unregister(typeof(TService));

    public bool Unregister(Type ServiceType)
    {
        lock (_SyncRoot) return _Services.Remove(ServiceType);
    }
}