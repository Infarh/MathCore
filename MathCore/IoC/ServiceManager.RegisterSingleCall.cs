using System;
using System.Linq;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public sealed partial class ServiceManager
    {
        #region Регистрация сервисов

        public SingleCallServiceRegistration<TService> RegisterSingleCall<TService>() where TService : class
        {
            var service_type = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingleCallServiceRegistration<TService>(this, typeof(TService));
                _Services[service_type] = registration;
                return registration;
            }
        }

        public SingleCallServiceRegistration<TService> RegisterSingleCall<TServiceInterface, TService>()
            where TService : class, TServiceInterface
        {
            var service_interface = typeof(TServiceInterface);
            lock (_SyncRoot)
            {
                var registration = _Services.Values.OfType<SingleCallServiceRegistration<TService>>().FirstOrDefault()
                                   ?? new SingleCallServiceRegistration<TService>(this, typeof(TService));
                _Services[service_interface] = registration;
                return registration;
            }
        }

        public SingleCallServiceRegistration<TService> RegisterSingleCall<TService>(Func<TService> FactoryMethod) where TService : class
        {
            var service_interface = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingleCallServiceRegistration<TService>(this, service_interface, FactoryMethod);
                _Services[service_interface] = registration;
                return registration;
            }
        }

        #endregion
    }
}