using System.Runtime.CompilerServices;
using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    /// <summary>Extension methods for CancellationToken.</summary>
    public static class CancellationTokenExtensions
    {
        /// <summary>Cancels a CancellationTokenSource and throws a corresponding OperationCanceledException.</summary>
        /// <param name="source">The source to be canceled.</param>
        public static void CancelAndThrow([NotNull] this CancellationTokenSource source)
        {
            source.Cancel();
            source.Token.ThrowIfCancellationRequested();
        }

        /// <summary>Creates a CancellationTokenSource that will be canceled when the specified token has cancellation requested.</summary>
        /// <param name="token">The token.</param>
        /// <returns>The created CancellationTokenSource.</returns>
        [NotNull]
        public static CancellationTokenSource CreateLinkedSource(this CancellationToken token) => CancellationTokenSource.CreateLinkedTokenSource(token, new CancellationToken());

        public static TaskAwaiter GetAwaiter(this CancellationToken cancel)
        {
            var result = new TaskCompletionSource<bool>();
            Task task = result.Task;
            if (cancel.IsCancellationRequested) result.SetResult(true);
            else cancel.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), result);
            return task.GetAwaiter();
        }

        public static CancellationTokenSource LinkWith(this in CancellationToken Source, in CancellationToken Cancel) =>
            CancellationTokenSource.CreateLinkedTokenSource(Source, Cancel);
    }
}