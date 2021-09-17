using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    /// <summary>Менеджер сервисов</summary>
    public sealed partial class ServiceManager : IServiceManager, IServiceRegistrations
    {
        private static readonly object __DefaultManagerSyncRoot = new();
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

        private readonly Dictionary<Type, ServiceRegistration> _Services = new();
        private readonly object _SyncRoot = new();

        [NotNull] public IServiceRegistrations ServiceRegistrations => this;

        public object this[Type ServiceType] => Get(ServiceType);

        [CanBeNull] ServiceRegistration IServiceRegistrations.this[[NotNull] Type ServiceType] => _Services.TryGetValue(ServiceType, out var registration) ? registration : null;

        public ServiceManager() => RegisterSingleton<IServiceManager>(this);

        public bool ServiceRegistered<TService>() => ServiceRegistered(typeof(TService));
        public bool ServiceRegistered([NotNull] Type ServiceType) =>
            _Services.ContainsKey(ServiceType)
            || _Services.Values.Any(s => s.AllowInheritance && ServiceType.IsAssignableFrom(s.ServiceType));

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

        public TObject Create<TObject>(params object[] parameters) where TObject : class
        {
            if (!ServiceRegistered<TObject>())
                return (TObject)new SingleCallServiceRegistration<TObject>(this, typeof(TObject)).GetService(parameters);
            return Get<TObject>() ?? throw new InvalidOperationException("Менеджер сервисов вернул пустую ссылку на зарегистрированный сервис");
        }

        public object Create(Type ObjectType, params object[] parameters)
        {
            if (ServiceRegistered(ObjectType))
                return Get(ObjectType) ?? throw new InvalidOperationException("Менеджер сервисов вернул пустую ссылку на зарегистрированный сервис");
            return ((ServiceRegistration)Activator.CreateInstance(typeof(SingleCallServiceRegistration<>).MakeGenericType(ObjectType), (object)this, (object)ObjectType)).GetService(parameters);
        }

        [NotNull] public ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class => new(this);

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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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