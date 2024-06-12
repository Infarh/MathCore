namespace MathCore.Extensions;

public static class ExceptionEx
{
    public static TException WithData<TException>(this TException exception, object Key, object Value)
       where TException : Exception
    {
        exception.Data[Key] = Value;
        return exception;
    }
}
