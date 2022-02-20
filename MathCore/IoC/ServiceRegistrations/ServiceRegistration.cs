#nullable enable
using System;
using System.Linq;

using MathCore.IoC.Exceptions;
// ReSharper disable VirtualMemberNeverOverridden.Global

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.IoC.ServiceRegistrations;

public abstract partial class ServiceRegistration : IDisposable
{
    public event ExceptionEventHandler<Exception> ExceptionThrown = null!;

    protected virtual void OnExceptionThrown(Exception e) => ExceptionThrown.ThrowIfUnhandled(this, e);

    protected readonly IServiceManager _Manager;

    public virtual Exception? LastException { get; set; }

    public Type ServiceType { get; }

    public bool AllowInheritance { get; set; }

    protected ServiceRegistration(IServiceManager Manager, Type ServiceType)
    {
        _Manager = Manager;
        this.ServiceType = ServiceType;
    }

    protected abstract object? CreateServiceInstance(params object[] parameters);

    public virtual object? CreateNewService(params object[] parameters)
    {
        try
        {
            return LastException is { } last_exception
                ? throw last_exception 
                : CreateServiceInstance(parameters);
        }
        catch (Exception e)
        {
            LastException = e;
            OnExceptionThrown(e);
        }

        return null;
    }

    public abstract object? GetService(params object[] parameters);

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
    protected readonly ServiceConstructorInfo[] _Constructors = null!;

    protected readonly Func<TService>? _FactoryMethod;

    #region Конструкторы

    protected ServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) => 
        _Constructors = (ServiceType ?? throw new ArgumentNullException(nameof(ServiceType)))
           .GetConstructors()
           .Select(c => new ServiceConstructorInfo(c))
           .OrderByDescending(c => c.Parameters.Count)
           .ToArray();

    protected ServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType) => _FactoryMethod = FactoryMethod;

    #endregion

    protected override object CreateServiceInstance(params object[] parameters) => _FactoryMethod is null ? CreateInstanceByReflection(parameters) : _FactoryMethod();

    private object CreateInstanceByReflection(params object[] parameters)
    {
        var service_manager = _Manager;
        bool ApplicableConstructor(ServiceConstructorInfo ctor)
        {
            foreach (var type in ctor.ParameterTypes)
                if (!service_manager.ServiceRegistered(type) && !parameters.Where(p => p != null).Any(p => type.IsInstanceOfType(p)))
                    return false;
            return true;
        }

        if (_Constructors.FirstOrDefault(ctor => ApplicableConstructor(ctor)) is not { } constructor)
            throw new ServiceConstructorNotFoundException(typeof(TService));

        return constructor.CreateInstance(service_manager.Get, parameters);
    }

    public ServiceRegistration<TService> With(Action<ServiceRegistration<TService>>? Initializer)
    {
        Initializer?.Invoke(this);
        return this;
    }

    public MapServiceRegistration MapTo<TMapService>() where TMapService : class => _Manager.Map<TService, TMapService>();
}