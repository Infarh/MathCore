using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

/// <summary>Методы расширения для CancellationToken</summary>
public static class CancellationTokenExtensions
{
    /// <summary>Отменяет CancellationTokenSource и выбрасывает соответствующее исключение OperationCanceledException</summary>
    /// <param name="source">Источник токена отмены для отмены</param>
    /// <example>
    /// <code>
    /// var cts = new CancellationTokenSource();
    /// // ... выполнение работы ...
    /// cts.CancelAndThrow(); // Отменяет и немедленно выбрасывает исключение
    /// </code>
    /// </example>
    public static void CancelAndThrow(this CancellationTokenSource source)
    {
        source.Cancel();
        source.Token.ThrowIfCancellationRequested();
    }

    /// <summary>Создаёт CancellationTokenSource, который будет отменён при запросе отмены указанного токена</summary>
    /// <param name="token">Токен отмены</param>
    /// <returns>Созданный CancellationTokenSource, связанный с указанным токеном</returns>
    /// <example>
    /// <code>
    /// var original_token = new CancellationToken();
    /// var linked_source = original_token.CreateLinkedSource();
    /// </code>
    /// </example>
    public static CancellationTokenSource CreateLinkedSource(this CancellationToken token) => CancellationTokenSource.CreateLinkedTokenSource(token, new());

    /// <summary>Возвращает awaiter, позволяющий асинхронно ожидать отмены токена</summary>
    /// <param name="cancel">Токен отмены для ожидания</param>
    /// <returns>TaskAwaiter для асинхронного ожидания отмены</returns>
    /// <remarks>Позволяет использовать CancellationToken в конструкции await для ожидания сигнала отмены</remarks>
    /// <example>
    /// <code>
    /// var cts = new CancellationTokenSource();
    /// await cts.Token; // Ожидает отмены токена
    /// Console.WriteLine("Токен был отменён");
    /// </code>
    /// </example>
    public static TaskAwaiter GetAwaiter(this CancellationToken cancel)
    {
        var result = new TaskCompletionSource<bool>();
        Task task = result.Task;
        if (cancel.IsCancellationRequested) result.SetResult(true);
        else cancel.Register(s => ((TaskCompletionSource<bool>?)s).SetResult(true), result);
        return task.GetAwaiter();
    }

    /// <summary>Создаёт связанный CancellationTokenSource из двух токенов отмены</summary>
    /// <param name="Source">Первый токен отмены</param>
    /// <param name="Cancel">Второй токен отмены</param>
    /// <returns>CancellationTokenSource, который будет отменён при отмене любого из указанных токенов</returns>
    /// <example>
    /// <code>
    /// var token1 = new CancellationToken();
    /// var token2 = new CancellationToken();
    /// var linked_source = token1.LinkWith(token2);
    /// </code>
    /// </example>
    public static CancellationTokenSource LinkWith(this CancellationToken Source, CancellationToken Cancel) =>
        CancellationTokenSource.CreateLinkedTokenSource(Source, Cancel);
}