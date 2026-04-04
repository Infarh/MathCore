namespace MathCore.IO.Transactions;

/// <summary>Исключение операций транзакций NTFS</summary>
[Serializable]
public class NtfsTransactionException : InvalidOperationException
{
    /// <summary>Создаёт исключение операций транзакций NTFS</summary>
    public NtfsTransactionException() { }

    /// <summary>Создаёт исключение операций транзакций NTFS</summary>
    /// <param name="Message">Сообщение ошибки</param>
    public NtfsTransactionException(string Message) : base(Message) { }

    /// <summary>Создаёт исключение операций транзакций NTFS</summary>
    /// <param name="Message">Сообщение ошибки</param>
    /// <param name="InnerException">Вложенное исключение</param>
    public NtfsTransactionException(string Message, Exception InnerException) : base(Message, InnerException) { }

#if !NET8_0_OR_GREATER
    protected NtfsTransactionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
#endif
}
