namespace MathCore.Exceptions;

[Serializable]
public class ConnectionException : Exception
{
    public ConnectionException() { }
    public ConnectionException(string message) : base(message) { }
    public ConnectionException(string message, Exception inner) : base(message, inner) { }

#if !NET8_0_OR_GREATER
    protected ConnectionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
: base(info, context) { } 
#endif
}