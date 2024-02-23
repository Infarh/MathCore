#nullable enable

using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC;

public class MapServiceRegistration(
    IServiceManager Manager,
    Type MapServiceType,
    ServiceRegistration BaseServiceRegistration)
    : ServiceRegistration(Manager, MapServiceType)
{
    protected override object? CreateServiceInstance(params object[] parameters) => 
        BaseServiceRegistration.CreateNewService(parameters);

    public override object? GetService(params object[] parameters) => 
        BaseServiceRegistration.GetService(parameters);

    internal override ServiceRegistration CloneFor(IServiceManager manager) => 
        new MapServiceRegistration(
            manager, 
            ServiceType, 
            manager.ServiceRegistrations[BaseServiceRegistration.ServiceType] 
            ?? throw new InvalidOperationException($"Не удалось получит сервис типа {BaseServiceRegistration.ServiceType}"));
}