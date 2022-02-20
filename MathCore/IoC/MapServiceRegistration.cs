#nullable enable
using System;

using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public class MapServiceRegistration : ServiceRegistration
{
    private readonly ServiceRegistration _BaseServiceRegistration;

    public MapServiceRegistration(IServiceManager Manager, Type MapServiceType, ServiceRegistration BaseServiceRegistration) 
        : base(Manager, MapServiceType) => 
        _BaseServiceRegistration = BaseServiceRegistration;

    protected override object? CreateServiceInstance(params object[] parameters) => 
        _BaseServiceRegistration.CreateNewService(parameters);

    public override object? GetService(params object[] parameters) => 
        _BaseServiceRegistration.GetService(parameters);

    internal override ServiceRegistration CloneFor(IServiceManager manager) => 
        new MapServiceRegistration(
            manager, 
            ServiceType, 
            manager.ServiceRegistrations[_BaseServiceRegistration.ServiceType] 
            ?? throw new InvalidOperationException($"Не удалось получит сервис типа {_BaseServiceRegistration.ServiceType}"));
}