using System;
using MathCore.Annotations;

namespace MathCore.IoC
{
    public class SingleCallServiceRegistration<TService> : ServiceRegistration<TService> where TService : class
    {
        public SingleCallServiceRegistration(IServiceManager Manager, Type ServiceType) : base(Manager, ServiceType) { }

        public SingleCallServiceRegistration(IServiceManager Manager, Type ServiceType, Func<TService> FactoryMethod) : base(Manager, ServiceType, FactoryMethod) { }

        public override object GetService() => CreateNewService();

        public override object CreateNewService()
        {
            try
            {
                return CreateServiceInstance();
            }
            catch (Exception e)
            {
                LastException = e;
                OnExceptionThrown(e);
            }

            return null;
        }

        [NotNull] internal override ServiceRegistration CloneFor([NotNull] IServiceManager manager) =>
            new SingleCallServiceRegistration<TService>(manager, ServiceType, _FactoryMethod);
    }
}