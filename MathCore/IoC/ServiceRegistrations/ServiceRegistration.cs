using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;
using MathCore.IoC.Exceptions;
// ReSharper disable VirtualMemberNeverOverridden.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.IoC.ServiceRegistrations
{
    public abstract partial class ServiceRegistration : IDisposable
    {
        public event ExceptionEventHandler<Exception> ExceptionThrown;

        protected virtual void OnExceptionThrown([NotNull] Exception e) => ExceptionThrown.ThrowIfUnhandled(this, e);

        protected readonly IServiceManager _Manager;

        public virtual Exception LastException { get; set; }

        public Type ServiceType { get; }

        public bool AllowInheritance { get; set; }

        protected ServiceRegistration(IServiceManager Manager, Type ServiceType)
        {
            _Manager = Manager;
            this.ServiceType = ServiceType;
        }

        protected abstract object CreateServiceInstance(params object[] parameters);

        public virtual object CreateNewService(params object[] parameters)
        {
            try
            {
                var last_exception = LastException;
                if (last_exception != null)
                    throw last_exception;
                return CreateServiceInstance(parameters);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                LastException = e;
                OnExceptionThrown(e);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return null;
        }

        public abstract object GetService(params object[] parameters);

        #region IDisposable

        protected bool Disposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) return;
            Disposed = true;
        }

        //~ServiceRegistration() => Dispose(false);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal abstract ServiceRegistration CloneFor(IServiceManager manager);
    }

    public abstract class ServiceRegistration<TService> : ServiceRegistration where TService : class
    {
        protected readonly ServiceConstructorInfo[] _Constructors;
        protected readonly Func<TService> _FactoryMethod;

        #region Конструкторы

        protected ServiceRegistration(IServiceManager Manager, [NotNull] Type ServiceType) : base(Manager, ServiceType) => _Constructors = ServiceType.GetConstructors().Select(c => new ServiceConstructorInfo(c)).OrderByDescending(c => c.Parameters.Count).ToArray();

        protected ServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType) => _FactoryMethod = FactoryMethod;

        #endregion

        protected override object CreateServiceInstance(params object[] parameters) => _FactoryMethod is null ? CreateInstanceByReflection(parameters) : _FactoryMethod();

        private object CreateInstanceByReflection(params object[] parameters)
        {
            var service_manager = _Manager;
            return (_Constructors.FirstOrDefault(ApplicableConstructor) 
                ?? throw new ServiceConstructorNotFoundException(typeof(TService)))
               .CreateInstance(new Func<Type, object>(service_manager.Get), parameters);

            bool ApplicableConstructor(ServiceConstructorInfo ctor)
            {
                foreach (Type parameterType in ctor.ParameterTypes)
                {
                    Type type = parameterType;
                    if (!service_manager.ServiceRegistered(type) && !parameters.Where(p => p != null).Any(p => type.IsInstanceOfType(p)))
                        return false;
                }
                return true;
            }
        }

        [NotNull]
        public ServiceRegistration<TService> With([CanBeNull] Action<ServiceRegistration<TService>> Initializer)
        {
            Initializer?.Invoke(this);
            return this;
        }

        [NotNull] public MapServiceRegistration MapTo<TMapService>() where TMapService : class => _Manager.Map<TService, TMapService>();
    }
}