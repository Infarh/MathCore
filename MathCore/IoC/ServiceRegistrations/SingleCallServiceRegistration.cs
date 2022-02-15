#nullable enable
using System;

using MathCore.Annotations;

namespace MathCore.IoC.ServiceRegistrations;

public class SingleCallServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
{
    public SingleCallServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) { }

    public SingleCallServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) { }

    public override object? GetService(params object[] parameters) => CreateNewService();

    public override object? CreateNewService(params object[] parameters)
    {
        try
        {
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

    internal override ServiceRegistration CloneFor(IServiceManager manager) =>
        new SingleCallServiceRegistration<TService>(
            manager, 
            ServiceType, 
            _FactoryMethod ?? throw new InvalidOperationException("Не указан метод-фабрика объектов"));
}