using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.IoC
{
    /// <summary>Менеджер сервисов</summary>
    public sealed class ServiceManager : IServiceManager, IServiceRegistrations
    {
        private static readonly object __DefaultManagerSyncRoot = new object();
        private static volatile ServiceManager __Default;

        [NotNull]
        public static ServiceManager Default
        {
            get
            {
                if (__Default is null)
                    lock (__DefaultManagerSyncRoot)
                        if (__Default is null)
                            __Default = new ServiceManager();
                return __Default;
            }
        }

        private readonly Dictionary<Type, ServiceRegistration> _Services = new Dictionary<Type, ServiceRegistration>();
        private readonly object _SyncRoot = new object();

        [NotNull] public IServiceRegistrations ServiceRegistrations => this;

        public object this[Type ServiceType] => Get(ServiceType);

        [CanBeNull] ServiceRegistration IServiceRegistrations.this[[NotNull] Type ServiceType] => _Services.TryGetValue(ServiceType, out var registration) ? registration : null;

        public ServiceManager() => RegisterSingleton<IServiceManager>(this);

        public bool ServiceRegistered<TService>() => ServiceRegistered(typeof(TService));
        public bool ServiceRegistered([NotNull] Type ServiceType) =>
            _Services.ContainsKey(ServiceType)
            || _Services.Values.Any(s => s.AllowInheritance && ServiceType.IsAssignableFrom(s.ServiceType));

        public ServiceRegistration<TServiceType> Register<TServiceType>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TServiceType : class
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton =>
                    (ServiceRegistration<TServiceType>) RegisterSingleton<TServiceType>(),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceType>(),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceType>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        }

        #region Регистрация сервисов

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

        public SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>() where TService : class
        {
            var service_type = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingleThreadServiceRegistration<TService>(this, typeof(TService));
                _Services[service_type] = registration;
                return registration;
            }
        }

        public ServiceRegistration<TService> Register<TServiceInterface, TService>(in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton)
            where TService : class, TServiceInterface
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton => (ServiceRegistration<TService>)
                    RegisterSingleton<TServiceInterface, TService>(),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall<TServiceInterface, TService>(),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread<TServiceInterface, TService>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
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

        public SingleThreadServiceRegistration<TService> RegisterSingleThread<TServiceInterface, TService>()
            where TService : class, TServiceInterface
        {
            var service_interface = typeof(TServiceInterface);
            lock (_SyncRoot)
            {
                var registration = _Services.Values.OfType<SingleThreadServiceRegistration<TService>>().FirstOrDefault()
                                    ?? new SingleThreadServiceRegistration<TService>(this, typeof(TService));
                _Services[service_interface] = registration;
                return registration;
            }
        }

        public ServiceRegistration<TService> Register<TService>(Func<TService> FactoryMethod, in ServiceRegistrationMode mode = ServiceRegistrationMode.Singleton) where TService : class
        {
            lock (_SyncRoot)
                return mode switch
                {
                    ServiceRegistrationMode.Singleton => (ServiceRegistration<TService>) RegisterSingleton(
                        FactoryMethod),
                    ServiceRegistrationMode.SingleCall => RegisterSingleCall(FactoryMethod),
                    ServiceRegistrationMode.SingleThread => RegisterSingleThread(FactoryMethod),
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
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

        public SingleThreadServiceRegistration<TService> RegisterSingleThread<TService>(Func<TService> FactoryMethod) where TService : class
        {
            var service_interface = typeof(TService);
            lock (_SyncRoot)
            {
                var registration = new SingleThreadServiceRegistration<TService>(this, service_interface, FactoryMethod);
                _Services[service_interface] = registration;
                return registration;
            }
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

        [CanBeNull] public TServiceInterface Get<TServiceInterface>() where TServiceInterface : class => Get(typeof(TServiceInterface)) as TServiceInterface;

        public object Get([NotNull] Type ServiceType)
        {
            lock (_SyncRoot)
            {
                if (_Services.TryGetValue(ServiceType, out var info))
                    return info.GetService();

                var base_registration = _Services.Values.Where(reg => reg.AllowInheritance)
                    .FirstOrDefault(services_value => ServiceType.IsAssignableFrom(services_value.ServiceType));
                if (base_registration is null) return null;
                _Services.Add(ServiceType, new MapServiceRegistration(this, ServiceType, base_registration));
                return base_registration.GetService();
            }
        }

        [NotNull] public ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class => new ServiceManagerAccessor<TService>(this);

        private bool _Disposed;
        private void Dispose(bool disposing)
        {
            if (!disposing || _Disposed) return;
            lock (_SyncRoot)
            {
                if (_Disposed) return;
                foreach (IDisposable service in _Services.Values)
                    service.Dispose();
                _Disposed = true;
            }
        }

        //~ServiceManager() => Dispose(false);

        void IDisposable.Dispose() => Dispose(true);//GC.SuppressFinalize(this);

        #region ICloneable

        [NotNull]
        public IServiceManager Clone()
        {
            var new_manager = new ServiceManager();
            foreach (var (key, value) in _Services.Where(s => s.Key != typeof(IServiceManager)))
                new_manager._Services.Add(key, value.CloneFor(new_manager));

            return new_manager;
        }

        object ICloneable.Clone() => Clone();

        #endregion
    }
}
