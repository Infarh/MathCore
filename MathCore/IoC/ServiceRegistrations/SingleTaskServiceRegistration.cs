using System;
using System.Threading;
using MathCore.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace MathCore.IoC.ServiceRegistrations
{
    [NotImplemented]
    public class SingleTaskServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
    {
        private volatile AsyncLocal<object> _Initializer;
        private volatile AsyncLocal<Exception> _Exceptions;

        public bool NeedDisposeInstance { get; set; }

        public object CurrentInstance => Instance;
        public object Instance => GetService();

        public override Exception LastException { get => _Exceptions.Value; set => _Exceptions.Value = value; }

        public SingleTaskServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) => ResetAll();

        public SingleTaskServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) => ResetAll();

        public override object GetService() => throw new NotImplementedException();

        public void ResetAll()
        {
            var last_initializer = _Initializer;
            _Initializer = new AsyncLocal<object>(e => CreateNewService());
            _Exceptions = new AsyncLocal<Exception>();
        }

        public void Reset()
        {
            if (NeedDisposeInstance)
                (_Initializer.Value as IDisposable)?.Dispose();
            _Initializer.Value = null;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    //if (disposing && !Disposed) _Initializer.Dispose();
        //    base.Dispose(disposing);
        //}

        [NotNull]
        internal override ServiceRegistration CloneFor(IServiceManager manager) => _FactoryMethod is null
            ? new SingleThreadServiceRegistration<TService>(manager, ServiceType)
            : new SingleThreadServiceRegistration<TService>(manager, ServiceType, _FactoryMethod);
    }
}