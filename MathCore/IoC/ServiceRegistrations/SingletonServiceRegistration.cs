#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable UnusedMember.Global

namespace MathCore.IoC.ServiceRegistrations;

/// <summary>Регистрация сервиса в режиме единственного экземпляра</summary>
/// <typeparam name="TService">Тип реализации сервиса</typeparam>
public class SingletonServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
{
    /// <summary>Признак того, что экземпляр сервиса был создан</summary>
    private bool _Created;

    /// <summary>Регистрация сервиса выполнена на основе существующего экземпляра</summary>
    private readonly bool _InstanceService;
    
    private readonly object _SyncRoot = new();

    public TimeSpan? InstanceActualityTime
    {
        get => _InstanceActualityTime;
        set
        {
            if (_InstanceActualityTime  == value) return;
            _InstanceActualityCheckCancel?.Cancel();
            _InstanceActualityTime = value;
        }
    }

    public bool IsInstanceCreated => _Created || _InstanceService;

    public TService? CurrentInstance { get; private set; }

    public TService? Instance => (TService?)GetService();

    public bool NeedDisposeInstance { get; set; }

    public SingletonServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) { }

    public SingletonServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) { }

    public SingletonServiceRegistration(IServiceManager Manager, Type ServiceType, TService ServiceInstance) : base(Manager, ServiceType)
    {
        CurrentInstance = ServiceInstance;
        _Created = true;
        _InstanceService = true;
    }

    public override object? GetService(params object[] parameters)
    {
        if (!_Created)
            lock (_SyncRoot)
            {
                if (_Created) return CurrentInstance;
                _Created = true;
                CurrentInstance = (TService?)CreateNewService(parameters);
            }

        CheckInstanceActualityTimeAsync();
        return LastException is { } last_error
            ? throw last_error 
            : CurrentInstance;
    }

    private DateTime _InstanceLastAccessTime;
    
    private TimeSpan? _InstanceActualityTime;
    
    private CancellationTokenSource? _InstanceActualityCheckCancel = new();

    private async void CheckInstanceActualityTimeAsync()
    {
        _InstanceLastAccessTime = DateTime.Now;
        if (_InstanceActualityTime is not { } instance_time || _InstanceActualityCheckCancel != null) return;
        lock (_SyncRoot)
        {
            if (_InstanceActualityCheckCancel != null) return;
            _InstanceActualityCheckCancel = new CancellationTokenSource();
        }
        var cancel = _InstanceActualityCheckCancel.Token;
        
        do
            await Task.Delay(instance_time, cancel).ConfigureAwait(false);
        while (DateTime.Now - _InstanceLastAccessTime >= instance_time || !cancel.IsCancellationRequested);

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

    public SingletonServiceRegistration<TService> With(Action<SingletonServiceRegistration<TService>>? Initializer)
    {
        Initializer?.Invoke(this);
        return this;
    }

    internal override ServiceRegistration CloneFor(IServiceManager manager) => _InstanceService
        ? new SingletonServiceRegistration<TService>(manager, ServiceType, CurrentInstance ?? throw new InvalidOperationException("Не указан экземпляр сервиса для регистрации"))
        : new SingletonServiceRegistration<TService>(manager, ServiceType);
}