using System;
using System.Threading;
using System.Threading.Tasks;
using MathCore.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.IoC.ServiceRegistrations
{
    public class SingletonServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
    {
        private bool _Created;
        private readonly bool _InstanceService;
        private readonly object _SyncRoot = new object();

        public TimeSpan? InstanceActualityTime
        {
            get => _InstanceActualityTime;
            set
            {
                if (_InstanceActualityTime.Equals(value)) return;
                _InstanceActualityCheckCancel?.Cancel();
                _InstanceActualityTime = value;
            }
        }

        public bool IsInstanceCreated => _Created || _InstanceService;

        public TService CurrentInstance { get; private set; }

        public TService Instance => (TService)GetService();

        public bool NeedDisposeInstance { get; set; }

        public SingletonServiceRegistration(IServiceManager Manager, [NotNull] Type ServiceType) : base(Manager, ServiceType) { }

        public SingletonServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) { }

        public SingletonServiceRegistration(IServiceManager Manager, [NotNull] Type ServiceType, TService ServiceInstance) : base(Manager, ServiceType)
        {
            CurrentInstance = ServiceInstance;
            _Created = true;
            _InstanceService = true;
        }

        public override object GetService()
        {
            if (!_Created)
                lock (_SyncRoot)
                {
                    if (_Created) return CurrentInstance;
                    _Created = true;
                    CurrentInstance = (TService)CreateNewService();
                }

            CheckInstanceActualityTimeAsync();
            var last_error = LastException;
            if (last_error != null) throw last_error;
            return CurrentInstance;
        }

        private DateTime _InstanceLastAccessTime;
        private TimeSpan? _InstanceActualityTime;
        private CancellationTokenSource _InstanceActualityCheckCancel = new CancellationTokenSource();

        private async void CheckInstanceActualityTimeAsync()
        {
            _InstanceLastAccessTime = DateTime.Now;
            if (InstanceActualityTime is null || _InstanceActualityCheckCancel != null) return;
            lock (_SyncRoot)
            {
                if (_InstanceActualityCheckCancel != null) return;
                _InstanceActualityCheckCancel = new CancellationTokenSource();
            }
            var cancel = _InstanceActualityCheckCancel.Token;
            do
                await Task.Delay((TimeSpan)InstanceActualityTime, cancel).ConfigureAwait(false);
            while (DateTime.Now - _InstanceLastAccessTime >= _InstanceActualityTime || !cancel.IsCancellationRequested);
            _InstanceActualityCheckCancel = null;
            Reset();
        }

        public void Reset()
        {
            if (_InstanceService || !_Created) return;
            lock (_SyncRoot)
            {
                if (!_Created) return;
                _Created = false;
                LastException = null;
                CurrentInstance = null;
                _InstanceActualityCheckCancel?.Cancel();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                if (!NeedDisposeInstance) return;
                lock (_SyncRoot)
                    if (_Created)
                    {
                        (CurrentInstance as IDisposable)?.Dispose();
                        _Created = false;
                        CurrentInstance = null;
                        _InstanceActualityCheckCancel?.Cancel();
                        _InstanceActualityCheckCancel?.Dispose();
                    }
            }
            base.Dispose(disposing);
        }

        [NotNull]
        public SingletonServiceRegistration<TService> With([CanBeNull] Action<SingletonServiceRegistration<TService>> Initializer)
        {
            Initializer?.Invoke(this);
            return this;
        }

        [NotNull]
        internal override ServiceRegistration CloneFor(IServiceManager manager) => _InstanceService
            ? new SingletonServiceRegistration<TService>(manager, ServiceType, CurrentInstance)
            : new SingletonServiceRegistration<TService>(manager, ServiceType);
    }
}