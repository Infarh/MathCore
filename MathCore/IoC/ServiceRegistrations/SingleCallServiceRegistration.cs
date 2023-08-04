#nullable enable
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
        catch (Exception e)
        {
            LastException = e;
            OnExceptionThrown(e);
        }

        return null;
    }

    internal override ServiceRegistration CloneFor(IServiceManager manager) =>
        new SingleCallServiceRegistration<TService>(
            manager, 
            ServiceType, 
            _FactoryMethod ?? throw new InvalidOperationException("Не указан метод-фабрика объектов"));
}