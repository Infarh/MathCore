namespace MathCore.IO.Transactions;

/// <summary>Состояние транзакции NTFS</summary>
public enum NtfsTransactionState
{
    /// <summary>Транзакция активна</summary>
    Active,

    /// <summary>Транзакция зафиксирована</summary>
    Committed,

    /// <summary>Транзакция отменена</summary>
    RolledBack,

    /// <summary>Транзакция освобождена</summary>
    Disposed,
}
