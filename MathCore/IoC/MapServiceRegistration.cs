using System;
using MathCore.Annotations;
using MathCore.IoC.ServiceRegistrations;

namespace MathCore.IoC
{
    public class MapServiceRegistration : ServiceRegistration
    {
        private readonly ServiceRegistration _BaseServiceRegistration;
        public MapServiceRegistration(IServiceManager Manager, Type MapServiceType, ServiceRegistration BaseServiceRegistration) : base(Manager, MapServiceType) => _BaseServiceRegistration = BaseServiceRegistration;

        protected override object CreateServiceInstance() => _BaseServiceRegistration.CreateNewService();

        public override object GetService() => _BaseServiceRegistration.GetService();

        [NotNull] internal override ServiceRegistration CloneFor([NotNull] IServiceManager manager) => new MapServiceRegistration(manager, ServiceType, manager.ServiceRegistrations[_BaseServiceRegistration.ServiceType]);
    }
}