using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public sealed partial class ServiceManager
    {
        public ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TServiceType : class
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton =>
                    RegisterSingleton<TServiceType>(),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceType>(),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceType>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        }

        #region Регистрация сервисов

        public ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TService : class, TServiceInterface
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton => RegisterSingleton<TServiceInterface, TService>(),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceInterface, TService>(),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceInterface, TService>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        }

        public ServiceRegistration<TService> Register<TService>(
            Func<TService> FactoryMethod, in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) where TService : class
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton => RegisterSingleton(FactoryMethod),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall(FactoryMethod),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread(FactoryMethod),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
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

        #endregion

        public bool Unregister<TService>() => Unregister(typeof(TService));

        public bool Unregister([NotNull] Type ServiceType)
        {
            lock (_SyncRoot) return _Services.Remove(ServiceType);
        }
    }
}