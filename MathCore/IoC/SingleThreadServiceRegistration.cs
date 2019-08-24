using System;
using System.Threading;

namespace MathCore.IoC
{
    public class SingleThreadServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
    {
        private volatile ThreadLocal<object> _Initializer;
        private volatile ThreadLocal<Exception> _Exceptions;

        public bool IsInstanceCreated => _Initializer.IsValueCreated;
        public bool NeedDisposeInstance { get; set; }

        public object CurrentInstance => IsInstanceCreated ? Instance : null;
        public object Instance => GetService();

        public override Exception LastException { get => _Exceptions.Value; set => _Exceptions.Value = value; }

        public SingleThreadServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) => ResetAll();

        public SingleThreadServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) => ResetAll();

        public override object GetService() => _Initializer.Value;

        public void ResetAll()
        {
            var last_initializer = _Initializer;
            _Initializer = new ThreadLocal<object>(CreateNewService);
            _Exceptions = new ThreadLocal<Exception>();
            last_initializer?.Dispose();
        }

        public void Reset()
        {
            if (NeedDisposeInstance)
                (_Initializer.Value as IDisposable)?.Dispose();
            _Initializer.Value = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !Disposed) _Initializer.Dispose();
            base.Dispose(disposing);
        }

        internal override ServiceRegistration CloneFor(IServiceManager manager) => _FactoryMethod is null
            ? new SingleThreadServiceRegistration<TService>(manager, ServiceType)
            : new SingleThreadServiceRegistration<TService>(manager, ServiceType, _FactoryMethod);
    }
}