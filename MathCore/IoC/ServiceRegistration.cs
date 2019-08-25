using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MathCore.IoC
{
    public abstract class ServiceRegistration : IDisposable
    {
        public event ExceptionEventHandler<Exception> ExceptionThrown;

        protected virtual void OnExceptionThrown(Exception e) => ExceptionThrown.ThrowIfUnhandled(this, e);

        protected readonly IServiceManager _Manager;

        public virtual Exception LastException { get; set; }

        public Type ServiceType { get; }

        public bool AllowInheritance { get; set; }

        protected ServiceRegistration(IServiceManager Manager, Type ServiceType)
        {
            _Manager = Manager;
            this.ServiceType = ServiceType;
        }

        protected abstract object CreateServiceInstance();

        public virtual object CreateNewService()
        {
            try
            {
                var last_exception = LastException;
                if (last_exception != null)
                    throw last_exception;
                return CreateServiceInstance();
            }
            catch (Exception e)
            {
                LastException = e;
                OnExceptionThrown(e);
            }

            return null;
        }

        public abstract object GetService();

        #region IDisposable

        protected bool Disposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) return;
            Disposed = true;
        }

        //~ServiceRegistration() => Dispose(false);

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal abstract ServiceRegistration CloneFor(IServiceManager manager);
    }

    public abstract class ServiceRegistration<TService> : ServiceRegistration where TService : class
    {
        protected class ServiceConstructorInfo
        {
            private readonly ConstructorInfo _Constructor;
            private ParameterInfo[] _Parameters;

            public ParameterInfo[] Parameters => _Parameters ?? (_Parameters = _Constructor.GetParameters());
            public IEnumerable<Type> ParameterTypes => Parameters.Select(p => p.ParameterType);

            public bool IsDefault => Parameters.Length == 0;

            public ServiceConstructorInfo(ConstructorInfo constructor) => _Constructor = constructor;

            public object[] GetParametersValues(Func<Type, object> ParameterSelector) => ParameterTypes.ToArray(ParameterSelector);

            public object CreateInstance(object[] parameters) => _Constructor.Invoke(parameters);

            public object CreateInstance(Func<Type, object> ParameterSelector) => CreateInstance(GetParametersValues(ParameterSelector));
        }

        protected readonly ServiceConstructorInfo[] _Constructors;
        protected readonly Func<TService> _FactoryMethod;

        #region Конструкторы

        protected ServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) => _Constructors = ServiceType.GetConstructors().Select(c => new ServiceConstructorInfo(c)).OrderByDescending(c => c.Parameters.Length).ToArray();

        protected ServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType) => _FactoryMethod = FactoryMethod;

        #endregion

        protected override object CreateServiceInstance() => _FactoryMethod is null ? CreateInstanceByReflection() : _FactoryMethod();

        private object CreateInstanceByReflection()
        {
            var service_manager = _Manager;
            var constructors = _Constructors;
            var constructor = constructors.FirstOrDefault(ctor => ctor.ParameterTypes.All(service_manager.ServiceRegistered))
                           ?? throw new ServiceConstructorNotFoundException(typeof(TService));

            return constructor.CreateInstance(service_manager.Get);
        }

        public ServiceRegistration<TService> With(Action<ServiceRegistration<TService>> Initializer)
        {
            Initializer?.Invoke(this);
            return this;
        }

        public MapServiceRegistration MapTo<TMapService>() where TMapService : class => _Manager.Map<TService, TMapService>();
    }
}
