using System;
using System.Runtime.Serialization;

namespace MathCore.Exceptions
{
    [Serializable]
    public class ConnectionException : Exception
    {
        public ConnectionException() { }
        public ConnectionException(string message) : base(message) { }
        public ConnectionException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
