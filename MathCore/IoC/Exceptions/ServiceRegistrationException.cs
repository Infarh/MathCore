#nullable enable
namespace MathCore.IoC.Exceptions;

[Serializable]
public class ServiceRegistrationException : ApplicationException
{
    public Type ServiceType { get; } = null!;

    public ServiceRegistrationException(Type ServiceType) => this.ServiceType = ServiceType;

    public ServiceRegistrationException(Type ServiceType, string message)
        : base(message) =>
        this.ServiceType = ServiceType;

    public ServiceRegistrationException(Type ServiceType, string message, Exception inner)
        : base(message, inner) =>
        this.ServiceType = ServiceType;

#if !NET8_0_OR_GREATER
    protected ServiceRegistrationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
   : base(info, context)
    {
    } 
#endif
}