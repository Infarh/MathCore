#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MathCore.IoC.Exceptions;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

/// <summary>Менеджер сервисов</summary>
public sealed partial class ServiceManager : IServiceManager, IServiceRegistrations
{
    private static readonly object __DefaultManagerSyncRoot = new();

    private static volatile ServiceManager? __Default;

    public static ServiceManager Default
    {
        get
        {
            if (__Default is not null) return __Default;
            lock (__DefaultManagerSyncRoot)
                if (__Default is null)
                    __Default = new ServiceManager();
            return __Default;
        }
    }

    public class RegistrationNotFoundEventArgs : EventArgs
    {
        public Type ServiceType { get; }
        
        public IReadOnlyCollection<object> Parameters { get; }
        
        public object? ServiceInstance { get; set; }
        
        public RegistrationNotFoundEventArgs(Type ServiceType, object[] Parameters)
        {
            this.ServiceType = ServiceType;
            this.Parameters = Parameters;
        }
    }

    public event EventHandler<RegistrationNotFoundEventArgs>? RegistrationNotFound;

    private bool InvokeRegistrationNotFound(Type ServiceType, object[] parameters, out object? ServiceInstance)
    {
        if (RegistrationNotFound is not { } handlers)
        {
            ServiceInstance = null;
            return false;
        }

        var args = new RegistrationNotFoundEventArgs(ServiceType, parameters);
        handlers(this, args);
        ServiceInstance = args.ServiceInstance;
        return ServiceInstance is not null;
    }

    private readonly Dictionary<Type, ServiceRegistration> _Services = new();

    private readonly object _SyncRoot = new();

    public IServiceRegistrations ServiceRegistrations => this;

    public bool ThrowIfNotFound { get; set; } = true;

    private List<IServiceManager>? _MergedServiceManagers;

    public ICollection<IServiceManager> MergedServiceManagers => _MergedServiceManagers ??= new List<IServiceManager>();

    public object? this[Type ServiceType] => Get(ServiceType);

    ServiceRegistration? IServiceRegistrations.this[Type ServiceType] => _Services.TryGetValue(ServiceType, out var registration) ? registration : null;

    public ServiceManager()
    {
        RegisterSingleton<IServiceManager>(this);
        Map<IServiceManager, ServiceManager>();
    }

    public bool ServiceRegistered<TService>() => ServiceRegistered(typeof(TService));

    public bool ServiceRegistered(Type ServiceType) =>
        _Services.ContainsKey(ServiceType)
        || _Services.Values.Any(s => s.AllowInheritance && ServiceType.IsAssignableFrom(s.ServiceType));

    public TServiceInterface? Get<TServiceInterface>() where TServiceInterface : class => Get(typeof(TServiceInterface)) as TServiceInterface;

    public TServiceInterface Get<TServiceInterface>(params object[] parameters) where TServiceInterface : class => throw new NotImplementedException();

    public object? Get(Type ServiceType)
    {
        lock (_SyncRoot)
        {
            if (_Services.TryGetValue(ServiceType, out var info))
                return info.GetService();

            var base_registration = _Services.Values.Where(reg => reg.AllowInheritance)
               .FirstOrDefault(services_value => ServiceType.IsAssignableFrom(services_value.ServiceType));

            if (base_registration is null)
            {
                if (_MergedServiceManagers is not { Count: > 0 } managers) 
                    return null;

                foreach (var manager in managers)
                    if (manager.Get(ServiceType) is { } obj)
                        return obj;

                return null;
            }

            _Services.Add(ServiceType, new MapServiceRegistration(this, ServiceType, base_registration));
            return base_registration.GetService();
        }
    }

    public object? Get(Type ServiceType, params object[] parameters)
    {
        lock (_SyncRoot)
        {
            if (_Services.TryGetValue(ServiceType, out var service_registration))
                return service_registration.GetService(parameters);

            ServiceRegistration? base_registration = null;
            foreach (var services_value in _Services.Values.Where(reg => reg.AllowInheritance))
            {
                if (!ServiceType.IsAssignableFrom(services_value.ServiceType)) continue;
                base_registration = services_value;
                break;
            }
            if (base_registration is null)
                if (InvokeRegistrationNotFound(ServiceType, parameters, out var instance))
                    return instance;
                else
                {
                    instance = CheckMergedManagers(ServiceType, parameters);
                    return instance is null && ThrowIfNotFound
                        ? throw new ServiceRegistrationNotFoundException(ServiceType)
                        : instance;
                }

            _Services.Add(ServiceType, new MapServiceRegistration(this, ServiceType, base_registration));
            return base_registration.GetService();
        }
    }

    public TObject Create<TObject>(params object[] parameters) where TObject : class =>
        ServiceRegistered<TObject>()
            ? Get<TObject>()
            ?? throw new InvalidOperationException("Менеджер сервисов вернул пустую ссылку на зарегистрированный сервис")
            : (TObject)new SingleCallServiceRegistration<TObject>(this, typeof(TObject)).GetService(parameters);

    public object? Create(Type ObjectType, params object[] parameters) =>
        ServiceRegistered(ObjectType)
            ? Get(ObjectType)
            ?? throw new InvalidOperationException("Менеджер сервисов вернул пустую ссылку на зарегистрированный сервис")
            : ((ServiceRegistration)Activator.CreateInstance(typeof(SingleCallServiceRegistration<>).MakeGenericType(ObjectType), this, ObjectType))
           .GetService(parameters);

    private object? CheckMergedManagers(Type ServiceType, object[] parameters)
    {
        foreach (var manager in MergedServiceManagers)
            if (manager != this && manager.ServiceRegistered(ServiceType))
                return manager.Get(ServiceType, parameters);
        return null;
    }

    public ServiceManagerAccessor<TService> ServiceAccessor<TService>() where TService : class => new(this);

    public object Run(object Instance, string MethodName)
    {
        if (Instance is null) throw new ArgumentNullException(nameof(Instance));
        if (MethodName is not { Length: > 0 }) throw new InvalidOperationException("Не указан метод для вызова");

        var instance_type = Instance.GetType();
        var methods = instance_type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
           .Where(method => method.Name == MethodName)
           .OrderByDescending(method => method.GetParameters().Length);

        MethodInfo? call_method = null;
        ParameterInfo[]? parameter_infos = null;
        foreach (var method in methods)
        {
            var method_parameters = method.GetParameters();
            if (method_parameters.Length == 0 || method_parameters.All(p => ServiceRegistered(p.ParameterType)))
            {
                call_method = method;
                parameter_infos = method_parameters;
                break;
            }
        }

        if (call_method is null)
            throw new InvalidOperationException("В указанном типе объекта указанный метод для запуска, содержащий полный набор параметров, известных менеджеру сервисов, не найден");

        var parameters = parameter_infos!.Select(p => Get(p.ParameterType)).ToArray();
        var result = call_method.Invoke(Instance, parameters);
        return result;
    }

    public object Run<T>(string StaticMethodName)
    {
        if (StaticMethodName is not { Length: > 0 }) throw new InvalidOperationException("Не указан метод для вызова");

        var instance_type = typeof(T);
        var methods = instance_type.GetMethods(BindingFlags.Static | BindingFlags.Public)
           .Where(method => method.Name == StaticMethodName)
           .OrderByDescending(method => method.GetParameters().Length);

        MethodInfo? call_method = null;
        ParameterInfo[]? parameter_infos = null;
        foreach (var method in methods)
        {
            var method_parameters = method.GetParameters();
            if (method_parameters.Length == 0 || method_parameters.All(p => ServiceRegistered(p.ParameterType)))
            {
                call_method = method;
                parameter_infos = method_parameters;
                break;
            }
        }

        if (call_method is null)
            throw new InvalidOperationException("В указанном типе объекта указанный метод для запуска, содержащий полный набор параметров, известных менеджеру сервисов, не найден");

        var parameters = parameter_infos!.Select(p => Get(p.ParameterType)).ToArray();
        var result = call_method.Invoke(null, parameters);
        return result;
    }

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
    public void Dispose() => Dispose(true);

    #region ICloneable

    public IServiceManager Clone()
    {
        var new_manager = new ServiceManager();
        foreach (var (key, value) in _Services.Where(s => s.Key != typeof(IServiceManager)))
            new_manager._Services.Add(key, value.CloneFor(new_manager));

        return new_manager;
    }

    object ICloneable.Clone() => Clone();

    #endregion

    object? IServiceProvider.GetService(Type ServiceType) => Get(ServiceType);
}