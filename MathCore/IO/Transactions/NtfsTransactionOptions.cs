namespace MathCore.IO.Transactions;

/// <summary>Параметры транзакции NTFS</summary>
public sealed class NtfsTransactionOptions
{
    /// <summary>Описание транзакции для системного журнала KTM</summary>
    public string? Description { get; init; }

    /// <summary>Таймаут транзакции в миллисекундах</summary>
    public int TimeoutMilliseconds { get; init; }
}
