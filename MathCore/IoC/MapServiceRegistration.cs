using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathCore.IoC
{
    public class MapServiceRegistration : ServiceRegistration
    {
        private readonly ServiceRegistration _BaseServiceRegistration;
        public MapServiceRegistration(IServiceManager Manager, Type MapServiceType, ServiceRegistration BaseServiceRegistration) : base(Manager, MapServiceType) => _BaseServiceRegistration = BaseServiceRegistration;

        protected override object CreateServiceInstance() => _BaseServiceRegistration.CreateNewService();

        public override object GetService() => _BaseServiceRegistration.GetService();

        internal override ServiceRegistration CloneFor(IServiceManager manager) => new MapServiceRegistration(manager, ServiceType, manager.ServiceRegistrations[_BaseServiceRegistration.ServiceType]);
    }
}
