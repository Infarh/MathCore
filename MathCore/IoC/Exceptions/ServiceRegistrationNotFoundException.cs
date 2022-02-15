#nullable enable
using System;
using System.Runtime.Serialization;

namespace MathCore.IoC.Exceptions;

[Serializable]
public class ServiceRegistrationNotFoundException : ServiceRegistrationException
{
    public ServiceRegistrationNotFoundException(Type ServiceType)
        : base(ServiceType)
    {
    }

    public ServiceRegistrationNotFoundException(Type ServiceType, string message)
        : base(ServiceType, message)
    {
    }

    public ServiceRegistrationNotFoundException(Type ServiceType, string message, Exception inner)
        : base(ServiceType, message, inner)
    {
    }

    protected ServiceRegistrationNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}