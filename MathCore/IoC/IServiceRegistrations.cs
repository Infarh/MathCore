using System;

namespace MathCore.IoC
{
    public interface IServiceRegistrations
    {
        ServiceRegistration this[Type ServiceType] { get; }
    }
}