using System.Runtime.Versioning;

namespace MathCore.IO.Transactions;

/// <summary>Фабрика транзакций NTFS</summary>
[SupportedOSPlatform("windows")]
public static class NtfsTransactions
{
    /// <summary>Открывает новую транзакцию NTFS</summary>
    /// <param name="Options">Параметры новой транзакции</param>
    /// <returns>Созданная транзакция NTFS</returns>
    public static NtfsTransaction Begin(NtfsTransactionOptions? Options = null) => new(Options);
}
