namespace MathCore.Exceptions;

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

#if !NET8_0_OR_GREATER
    protected CalculationsException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    } 
#endif
}