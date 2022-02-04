using System;
using System.Linq;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public sealed partial class ServiceManager
    {
        #region Регистрация сервисов

        public ServiceRegistration RegisterSingleton(Type ServiceType)
        {
            lock (_SyncRoot)
            {
                var registration_type = typeof(SingletonServiceRegistration<>).MakeGenericType(ServiceType);
                var registration = (ServiceRegistration)registration_type.CreateObject(this, ServiceType);

                _Services[ServiceType] = registration;
                return registration;
            }
        }

        public ServiceRegistration RegisterSingleton(Type InterfaceType, Type ServiceType)
        {
            lock (_SyncRoot)
            {
                var registration_type = typeof(SingletonServiceRegistration<>).MakeGenericType(ServiceType);
                var registration = _Services.Values.FirstOrDefault(r => r.GetType() == registration_type)
                    ?? (ServiceRegistration)registration_type.CreateObject(this, ServiceType);

                _Services[InterfaceType] = registration;
                return registration;
            }
        }

        public SingletonServiceRegistration<TService> RegisterSingleton<TService>() where TService : class
        {
            var service_type = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingletonServiceRegistration<TService>(this, typeof(TService));
                _Services[service_type] = registration;
                return registration;
            }
        }

        public SingletonServiceRegistration<TService> RegisterSingleton<TService>(TService ServiceInstance) where TService : class
        {
            var service_type = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingletonServiceRegistration<TService>(this, typeof(TService), ServiceInstance);
                _Services[service_type] = registration;
                return registration;
            }
        }

        public SingletonServiceRegistration<TService> RegisterSingleton<TServiceInterface, TService>()
            where TService : class, TServiceInterface
        {
            var service_interface = typeof(TServiceInterface);
            lock (_SyncRoot)
            {
                var registration = _Services.Values.OfType<SingletonServiceRegistration<TService>>().FirstOrDefault()
                                   ?? new SingletonServiceRegistration<TService>(this, typeof(TService));
                _Services[service_interface] = registration;
                return registration;
            }
        }

        public SingletonServiceRegistration<TService> RegisterSingleton<TService>(Func<TService> FactoryMethod) where TService : class
        {
            var service_interface = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingletonServiceRegistration<TService>(this, service_interface, FactoryMethod);
                _Services[service_interface] = registration;
                return registration;
            }
        }

        #endregion
    }
}