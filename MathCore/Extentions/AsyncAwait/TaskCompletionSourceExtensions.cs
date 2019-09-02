using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    /// <summary>Extension methods for TaskCompletionSource.</summary>
    public static class TaskCompletionSourceExtensions
    {
        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        public static void SetFromTask<TResult>([NotNull] this TaskCompletionSource<TResult> ResultSetter, [NotNull] Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: ResultSetter.SetResult(task is Task<TResult> t ? t.Result : default); break;
                case TaskStatus.Faulted: ResultSetter.SetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException()); break;
                case TaskStatus.Canceled: ResultSetter.SetCanceled(); break;
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        public static void SetFromTask<TResult>([NotNull] this TaskCompletionSource<TResult> ResultSetter, [NotNull] Task<TResult> task) => SetFromTask(ResultSetter, (Task)task);

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>([NotNull] this TaskCompletionSource<TResult> ResultSetter, [NotNull] Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: return ResultSetter.TrySetResult(task is Task<TResult> t ? t.Result : default);
                case TaskStatus.Faulted: return ResultSetter.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                case TaskStatus.Canceled: return ResultSetter.TrySetCanceled();
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>([NotNull] this TaskCompletionSource<TResult> ResultSetter, [NotNull, ItemCanBeNull] Task<TResult> task) => TrySetFromTask(ResultSetter, (Task)task);
    }
}