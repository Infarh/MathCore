using MathCore.Annotations;

namespace MathCore.IoC
{
    public class ServiceManagerAccessor<TService> where TService : class
    {
        private readonly ServiceManager _ServiceManager;

        [CanBeNull] public TService Service => _ServiceManager.Get<TService>();

        [NotNull] public TService ServiceRequired => _ServiceManager.GetRequired<TService>();

        public ServiceManagerAccessor(ServiceManager ServiceManager) => _ServiceManager = ServiceManager;
    }
}