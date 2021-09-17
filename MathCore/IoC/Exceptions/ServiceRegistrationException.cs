using System;
using System.Runtime.Serialization;

namespace MathCore.IoC.Exceptions
{
    [Serializable]
    public class ServiceRegistrationException : ApplicationException
    {
        public Type ServiceType { get; }

        public ServiceRegistrationException(Type ServiceType) => this.ServiceType = ServiceType;

        public ServiceRegistrationException(Type ServiceType, string message)
            : base(message) =>
            this.ServiceType = ServiceType;

        public ServiceRegistrationException(Type ServiceType, string message, Exception inner)
            : base(message, inner) =>
            this.ServiceType = ServiceType;

        protected ServiceRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}