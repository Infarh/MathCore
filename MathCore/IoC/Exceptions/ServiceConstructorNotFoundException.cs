#nullable enable
using System.Runtime.Serialization;

namespace MathCore.IoC.Exceptions;

[Serializable]
public class ServiceConstructorNotFoundException : Exception
{
    public Type ServiceType { get; } = null!;

    public ServiceConstructorNotFoundException() { }
    
    public ServiceConstructorNotFoundException(string Message) : base(Message) { }
    
    public ServiceConstructorNotFoundException(string Message, Exception InnerException) : base(Message, InnerException) { }
    
    public ServiceConstructorNotFoundException(Type ServiceType) => this.ServiceType = ServiceType;
    
    public ServiceConstructorNotFoundException(Type ServiceType, string message) : base(message) => this.ServiceType = ServiceType;
    
    public ServiceConstructorNotFoundException(Type ServiceType, string message, Exception inner) : base(message, inner) => this.ServiceType = ServiceType;

    protected ServiceConstructorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}