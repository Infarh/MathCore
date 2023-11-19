#nullable enable
namespace MathCore.IoC.Exceptions;

[Serializable]
public class ServiceRegistrationNotFoundException : ServiceRegistrationException
{
    public ServiceRegistrationNotFoundException(Type ServiceType) : base(ServiceType) { }

    public ServiceRegistrationNotFoundException(Type ServiceType, string message) : base(ServiceType, message) { }

    public ServiceRegistrationNotFoundException(Type ServiceType, string message, Exception inner) : base(ServiceType, message, inner) { }

#if !NET8_0_OR_GREATER
    protected ServiceRegistrationNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { } 
#endif
}