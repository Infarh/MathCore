using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MathCore.IoC
{
    [Serializable]
    public class ServiceConstructorNotFoundException : Exception
    {
        public Type ServiceType { get; }

        public ServiceConstructorNotFoundException(Type ServiceType) => this.ServiceType = ServiceType;
        public ServiceConstructorNotFoundException(Type ServiceType, string message) : base(message) => this.ServiceType = ServiceType;
        public ServiceConstructorNotFoundException(Type ServiceType, string message, Exception inner) : base(message, inner) => this.ServiceType = ServiceType;

        protected ServiceConstructorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
