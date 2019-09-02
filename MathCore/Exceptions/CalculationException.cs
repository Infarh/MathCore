using System;
using System.Runtime.Serialization;

namespace MathCore.Exceptions
{
    [Serializable]
    public class CalculationsException : Exception
    {
        public CalculationsException()
        {
        }

        public CalculationsException(string message) : base(message)
        {
        }

        public CalculationsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CalculationsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}