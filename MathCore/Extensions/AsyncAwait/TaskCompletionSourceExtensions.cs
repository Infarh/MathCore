#nullable enable

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    /// <summary>Extension methods for TaskCompletionSource.</summary>
    public static class TaskCompletionSourceExtensions
    {
        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transferred.</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> ResultSetter, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: ResultSetter.SetResult((task is Task<TResult> t ? t.Result : default)!); break;
                case TaskStatus.Faulted: ResultSetter.SetException(task.Exception!.InnerExceptions); break;
                case TaskStatus.Canceled: ResultSetter.SetCanceled(); break;
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        /// <summary>Transfers the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transferred.</param>
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> ResultSetter, Task<TResult> task) => SetFromTask(ResultSetter, (Task)task);

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transferred.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> ResultSetter, Task task) =>
            task.Status switch
            {
                TaskStatus.RanToCompletion => ResultSetter.TrySetResult((task is Task<TResult> t ? t.Result : default)!),
                TaskStatus.Faulted => ResultSetter.TrySetException(task.Exception!.InnerExceptions),
                TaskStatus.Canceled => ResultSetter.TrySetCanceled(),
                _ => throw new InvalidOperationException("The task was not completed.")
            };

        /// <summary>Attempts to transfer the result of a Task to the TaskCompletionSource.</summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="ResultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transferred.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> ResultSetter, Task<TResult> task) => TrySetFromTask(ResultSetter, (Task)task);
    }
}