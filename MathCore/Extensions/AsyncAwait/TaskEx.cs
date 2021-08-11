#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MathCore.Annotations;
using MathCore.Extensions.AsyncAwait;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public static class TaskEx
    {
        public static PerformActionAwaitable ConfigureAwait(this Task task, bool LockContext, Action BeforeAction) => new(BeforeAction, task, LockContext);

        public static PerformActionAwaitable<T> ConfigureAwait<T>(this Task<T> task, bool LockContext, Action BeforeAction) => new(BeforeAction, task, LockContext);

        public static TaskSchedulerAwaitable ConfigureAwait(this Task task, TaskScheduler ContinuationScheduler) => new(ContinuationScheduler, task);

        public static SynchronizationContextAwaitable ConfigureAwait(this Task task, SynchronizationContext Context) => new(Context, task);

        public static TaskSchedulerAwaitable<T> ConfigureAwait<T>(this Task<T> task, TaskScheduler ContinuationScheduler) => new(ContinuationScheduler, task);

        public static SynchronizationContextAwaitable<T> ConfigureAwait<T>(this Task<T> task, SynchronizationContext Context) => new(Context, task);

        /// <summary>Переход в асинхронную область - в новый поток из пула потоков</summary>
        public static YieldAsyncAwaitable YieldAsync() => new();

        public static Task<T> ToTask<T>(this T value) => Task.FromResult(value);

        public static async Task<TResult> Bind<T, TResult>(this Task<T> task, Func<T, Task<TResult>> Selector) => await Selector(await task);

        //public static Task<TResult> SelectMany<T, TValue, TResult>(
        //    this Task<T> task,
        //    Func<T, Task<TValue>> Selector,
        //    Func<T, TValue, TResult> Projection)
        //{
        //    return task.Bind(outer => Selector(outer).Bind(inner => Projection(outer, inner).ToTask()));
        //}

        /// <summary>Переключиться в контекст планировщика потоков</summary>
        /// <param name="scheduler">Планировщик потоков, распределяющий процессы выполнения задач</param>
        public static TaskSchedulerAwaitable SwitchContext(this TaskScheduler scheduler) => new(scheduler);

        public static SynchronizationContextAwaitable SwitchContext(this SynchronizationContext context) => new(context);

        public static Task<Task> WhenAny(this IEnumerable<Task> tasks) => Task.WhenAny(tasks);
        public static Task<Task<T>> WhenAny<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAny(tasks);

        public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);

        public static void AndForget<T>(this Task<T> task, Action<AggregateException>? OnException = null) => task.ContinueWith(t => OnException?.Invoke(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
        public static void AndForget(this Task task, Action<AggregateException>? OnException = null) => task.ContinueWith(t => OnException?.Invoke(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancel) =>
            task.IsCompleted || !cancel.CanBeCanceled
                ? task
                : cancel.IsCancellationRequested
                    ? Task.FromCanceled<T>(cancel)
                    : task.CreateCancellation(cancel);

        private static readonly Action<object> __CancellationRegistration =
            s => ((TaskCompletionSource<bool>)s).TrySetResult(default);

        private static async Task<T> CreateCancellation<T>(this Task<T> task, CancellationToken cancel)
        {
            var tcs = new TaskCompletionSource<object?>();
            using (cancel.Register(__CancellationRegistration, tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException();
            return await task;
        }

        public static Task Catch<T>(this Task<T> task, Action<AggregateException> ProcessExceptions) => task.ContinueWith(t => ProcessExceptions(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
        public static Task Catch(this Task task, Action<AggregateException> ProcessExceptions) => task.ContinueWith(t => ProcessExceptions(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);

        public static Task Finally<T>(this Task<T> task, Action OnTaskCompleted) => task.ContinueWith(t => OnTaskCompleted(), TaskContinuationOptions.None);
        public static Task Finally(this Task task, Action OnTaskCompleted) => task.ContinueWith(t => OnTaskCompleted(), TaskContinuationOptions.None);

        // ReSharper disable once InconsistentNaming
        const TaskContinuationOptions not_on_cancel = TaskContinuationOptions.NotOnCanceled;

        /// https://blogs.msdn.microsoft.com/pfxteam/2010/04/04/a-tour-of-parallelextensionsextras/
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return source.ContinueWith(t => selector(t.Result), not_on_cancel)!;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return source.ContinueWith(t => selector(t.Result), not_on_cancel).Unwrap();
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> SelectMany<TSource, TCollection, TResult>
        (
            this Task<TSource> source,
            Func<TSource, Task<TCollection>> GetCollection,
            Func<TSource, TCollection, TResult> GetResult)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (GetCollection is null) throw new ArgumentNullException(nameof(GetCollection));
            if (GetResult is null) throw new ArgumentNullException(nameof(GetResult));

            return source
               .ContinueWith(
                    t => GetCollection(t.Result).ContinueWith(c => GetResult(t.Result, c.Result), not_on_cancel),
                    not_on_cancel)
               .Unwrap()!;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> Where<TSource>(this Task<TSource> source, Func<TSource, bool> predicate)
        {
            // Validate arguments
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            // Create a continuation to run the predicate and return the source's result.
            // If the predicate returns false, cancel the returned Task.
            var cts = new CancellationTokenSource();
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                if (!predicate(result)) cts.CancelAndThrow();
                return result;
            }, cts.Token, not_on_cancel, TaskScheduler.Default)!;
        }

        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>
        (
            this Task<TOuter> Outer,
            Task<TInner> Inner,
            Func<TOuter, TKey> OuterKeySelector,
            Func<TInner, TKey> InnerKeySelector,
            Func<TOuter, TInner, TResult> ResultSelector
        ) => Join(
            Outer: Outer,
            Inner: Inner,
            OuterKeySelector: OuterKeySelector,
            InnerKeySelector: InnerKeySelector,
            ResultSelector: ResultSelector,
            Comparer: EqualityComparer<TKey>.Default);

        [NotNull, ItemCanBeNull]
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>
        (
            this Task<TOuter> Outer,
            Task<TInner> Inner,
            Func<TOuter, TKey> OuterKeySelector,
            Func<TInner, TKey> InnerKeySelector,
            Func<TOuter, TInner, TResult> ResultSelector,
            IEqualityComparer<TKey> Comparer
        )
        {
            // Validate arguments
            if (Outer is null) throw new ArgumentNullException(nameof(Outer));
            if (Inner is null) throw new ArgumentNullException(nameof(Inner));
            if (OuterKeySelector is null) throw new ArgumentNullException(nameof(OuterKeySelector));
            if (InnerKeySelector is null) throw new ArgumentNullException(nameof(InnerKeySelector));
            if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));
            if (Comparer is null) throw new ArgumentNullException(nameof(Comparer));

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return Outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return Inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(Outer, Inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (Comparer.Equals(OuterKeySelector(Outer.Result), InnerKeySelector(Inner.Result)))
                        return ResultSelector(Outer.Result, Inner.Result);
                    cts.CancelAndThrow();
                    return default!; // won't be reached
                }, cts.Token, not_on_cancel, TaskScheduler.Default);
            }, not_on_cancel).Unwrap()!;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>
        (
            this Task<TOuter> outer,
            Task<TInner> inner,
            Func<TOuter, TKey> OuterKeySelector,
            Func<TInner, TKey> InnerKeySelector,
            Func<TOuter, Task<TInner>, TResult> ResultSelector
        ) => GroupJoin(
            outer: outer,
            inner: inner,
            OuterKeySelector: OuterKeySelector,
            InnerKeySelector: InnerKeySelector,
            ResultSelector: ResultSelector,
            Comparer: EqualityComparer<TKey>.Default);

        [NotNull, ItemCanBeNull]
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>
        (
            this Task<TOuter> outer,
            Task<TInner> inner,
            Func<TOuter, TKey> OuterKeySelector,
            Func<TInner, TKey> InnerKeySelector,
            Func<TOuter, Task<TInner>, TResult> ResultSelector,
            IEqualityComparer<TKey> Comparer
            )
        {
            // Validate arguments
            if (outer is null) throw new ArgumentNullException(nameof(outer));
            if (inner is null) throw new ArgumentNullException(nameof(inner));
            if (OuterKeySelector is null) throw new ArgumentNullException(nameof(OuterKeySelector));
            if (InnerKeySelector is null) throw new ArgumentNullException(nameof(InnerKeySelector));
            if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));
            if (Comparer is null) throw new ArgumentNullException(nameof(Comparer));

            // First continue off of the outer and then off of the inner.  Two separate
            // continuations are used so that each may be canceled easily using the NotOnCanceled option.
            return outer.ContinueWith(delegate
            {
                var cts = new CancellationTokenSource();
                return inner.ContinueWith(delegate
                {
                    // Propagate all exceptions
                    Task.WaitAll(outer, inner);

                    // Both completed successfully, so if their keys are equal, return the result
                    if (Comparer.Equals(OuterKeySelector(outer.Result), InnerKeySelector(inner.Result)))
                        return ResultSelector(outer.Result, inner);
                    cts.CancelAndThrow();
                    return default!; // won't be reached
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, not_on_cancel).Unwrap()!;
        }

        public static Task<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>
        (
            this Task<TSource> source,
            Func<TSource, TKey> KeySelector,
            Func<TSource, TElement> ElementSelector
        )
        {
            // Validate arguments
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (KeySelector is null) throw new ArgumentNullException(nameof(KeySelector));
            if (ElementSelector is null) throw new ArgumentNullException(nameof(ElementSelector));

            // When the source completes, return a grouping of just the one element
            return source.ContinueWith(t =>
            {
                var result = t.Result;
                var key = KeySelector(result);
                var element = ElementSelector(result);
                return (IGrouping<TKey, TElement>)new OneElementGrouping<TKey, TElement> { Key = key, Element = element };
            }, not_on_cancel);
        }

        /// <summary>Represents a grouping of one element.</summary>
        /// <typeparam name="TKey">The type of the key for the element.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        private class OneElementGrouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            public TKey Key { get; internal set; } = default!;
            internal TElement Element { get; set; } = default!;
            public IEnumerator<TElement> GetEnumerator() { yield return Element; }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static Task<TSource> OrderBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        public static Task<TSource> OrderByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey>? KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        public static Task<TSource> ThenBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey>? KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        public static Task<TSource> ThenByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey>? KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, in TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(_ => result.TrySetCanceled(), null, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            });
            return result.Task;
        }

        public static Task WithAsyncCallback(this Task task, AsyncCallback? callback, object? state)
        {
            var tcs = new TaskCompletionSource<object?>(state);
            task.ContinueWith(_ =>
            {
                tcs.TrySetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        public static Task<TResult> WithAsyncCallback<TResult>(this Task<TResult> task, AsyncCallback? callback, object? state)
        {
            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(_ =>
            {
                tcs.TrySetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        #region ContinueWith accepting TaskFactory
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith(this Task task, Action<Task> ContinuationAction, TaskFactory factory) =>
            task.ContinueWith(ContinuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, TResult> ContinuationFunction, TaskFactory factory) =>
            task.ContinueWith(ContinuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        #endregion

        #region ContinueWith accepting TaskFactory<TResult>
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>> ContinuationAction, TaskFactory<TResult> factory) =>
            task.ContinueWith(ContinuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        public static Task<TNewResult> ContinueWith<TResult, TNewResult>(this Task<TResult> task, Func<Task<TResult>, TNewResult> ContinuationFunction, TaskFactory<TResult> factory) =>
            task.ContinueWith(ContinuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        #endregion

        #region ToAsync(AsyncCallback, object)
        /// <summary>
        /// Creates a Task that represents the completion of another Task, and 
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task ToAsync(this Task task, AsyncCallback? callback, object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task that represents the completion of another Task, and 
        /// that schedules an AsyncCallback to run upon completion.
        /// </summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="callback">The AsyncCallback to run.</param>
        /// <param name="state">The object state to use with the AsyncCallback.</param>
        /// <returns>The new task.</returns>
        public static Task<TResult> ToAsync<TResult>(this Task<TResult> task, AsyncCallback? callback, object? state)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(_ =>
            {
                tcs.SetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }
        #endregion

        #region Exception Handling
        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task IgnoreExceptions(this Task task)
        {
            task.ContinueWith(t => (_ = t.Exception)!,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        public static Task TraceExceptions(this Task task, string? Message = null)
        {
            task.ContinueWith(t => Trace.TraceWarning("{0}: {1}", string.IsNullOrWhiteSpace(Message) ? $"Exception in task id{t.Id}" : $"{Message}", t.Exception),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Suppresses default exception handling of a Task that would otherwise reraise the exception on the finalizer thread.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> IgnoreExceptions<T>(this Task<T> task) => (Task<T>)((Task)task).IgnoreExceptions();

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task FailFastOnException(this Task task)
        {
            task.ContinueWith(t => Environment.FailFast("A task faulted.", t.Exception),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        public static Task<T> FailFastOnException<T>(this Task<T> task) => (Task<T>)((Task)task).FailFastOnException();

        /// <summary>Propagates any exceptions that occurred on the specified task.</summary>
        /// <param name="task">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task task)
        {
            if (!task.IsCompleted) throw new InvalidOperationException("The task has not completed.");
            if (task.IsFaulted) task.Wait();
        }

        /// <summary>Propagates any exceptions that occurred on the specified tasks.</summary>
        /// <param name="tasks">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions(this Task[] tasks)
        {
            if (tasks is null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Any(t => t is null)) throw new ArgumentException(nameof(tasks));
            if (tasks.Any(t => !t.IsCompleted)) throw new InvalidOperationException("A task has not completed.");
            Task.WaitAll(tasks);
        }
        #endregion

        #region Observables
        /// <summary>Creates an IObservable that represents the completion of a Task.</summary>
        /// <typeparam name="TResult">Specifies the type of data returned by the Task.</typeparam>
        /// <param name="task">The Task to be represented as an IObservable.</param>
        /// <returns>An IObservable that represents the completion of the Task.</returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task) =>
            task is null
                ? throw new ArgumentNullException(nameof(task))
                : new TaskObservable<TResult>(task);

        /// <summary>An implementation of IObservable that wraps a Task.</summary>
        /// <typeparam name="TResult">The type of data returned by the task.</typeparam>
        private class TaskObservable<TResult> : IObservable<TResult>
        {
            private readonly Task<TResult> _ObservedTask;

            public TaskObservable(Task<TResult> ObservedTask) => _ObservedTask = ObservedTask;

            public IDisposable Subscribe(IObserver<TResult> observer)
            {
                // Validate arguments
                if (observer is null) throw new ArgumentNullException(nameof(observer));

                // Support cancelling the continuation if the observer is unsubscribed
                var cts = new CancellationTokenSource();

                // Create a continuation to pass data along to the observer
                _ObservedTask.ContinueWith(t =>
                {
                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            observer.OnNext(_ObservedTask.Result);
                            observer.OnCompleted();
                            break;

                        case TaskStatus.Faulted:
                            observer.OnError(_ObservedTask.Exception ?? throw new InvalidOperationException());
                            break;

                        case TaskStatus.Canceled:
                            observer.OnError(new TaskCanceledException(t));
                            break;
                    }
                }, cts.Token);

                // Support unsubscribe simply by canceling the continuation if it hasn't yet run
                return new CancelOnDispose(cts);
            }
        }

        /// <summary>Translate a call to IDisposable.Dispose to a CancellationTokenSource.Cancel.</summary>
        private class CancelOnDispose : IDisposable
        {
            private readonly CancellationTokenSource _Source;

            public CancelOnDispose(CancellationTokenSource source) => _Source = source;

            void IDisposable.Dispose() => _Source.Cancel();
        }
        #endregion

        #region Timeouts
        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task WithTimeout(this Task task, in TimeSpan timeout)
        {
            var result = new TaskCompletionSource<object>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<object>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }

        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<TResult>)state).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }
        #endregion

        #region Children
        /// <summary>
        /// Ensures that a parent task can't transition into a completed state
        /// until the specified task has also completed, even if it's not
        /// already a child task.
        /// </summary>
        /// <param name="task">The task to attach to the current task as a child.</param>
        public static void AttachToParent(this Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            task.ContinueWith(t => t.Wait(), CancellationToken.None, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion

        #region Waiting
        ///// <summary>Waits for the task to complete execution, pumping in the meantime.</summary>
        ///// <param name="task">The task for which to wait.</param>
        ///// <remarks>This method is intended for usage with Windows Presentation Foundation.</remarks>
        //public static void WaitWithPumping(this Task task)
        //{
        //    if (task is null) throw new ArgumentNullException(nameof(task));

        //    var nested_frame = new DispatcherFrame();
        //    task.ContinueWith(_ => nested_frame.Continue = false);
        //    Dispatcher.PushFrame(nested_frame);
        //    task.Wait();
        //}

        /// <summary>Waits for the task to complete execution, returning the task's final status.</summary>
        /// <param name="task">The task for which to wait.</param>
        /// <returns>The completion status of the task.</returns>
        /// <remarks>Unlike Wait, this method will not throw an exception if the task ends in the Faulted or Canceled state.</remarks>
        public static TaskStatus WaitForCompletionStatus(this Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            return task.Status;
        }
        #endregion

        #region Then
        /// <summary>Creates a task that represents the completion of a follow-up action when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The action to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the action.</returns>
        public static Task Then(this Task task, Action next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<object?>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    try
                    {
                        next();
                        result.TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        public static Task<TResult> Then<TResult>(this Task task, Func<TResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<TResult>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted) result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    try
                    {
                        result.TrySetResult(next());
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up action when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The action to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the action.</returns>
        public static Task Then<TResult>(this Task<TResult> task, Action<TResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<object?>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    try
                    {
                        next(task.Result);
                        result.TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        public static Task<TResult> Then<T, TResult>(this Task<T> task, Func<T, TResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<TResult>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    try
                    {
                        result.TrySetResult(next(task.Result));
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task Then(this Task task, Func<Task> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted)
                    result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try
                    {
                        next().ContinueWith(t => result.TrySetFromTask(t), TaskScheduler.Default);
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task Then<T>(this Task<T> task, Func<T, Task> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) result.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try
                    {
                        next(task.Result).ContinueWith(t => result.TrySetFromTask(t), TaskScheduler.Default);
                    }
                    catch (Exception e)
                    {
                        result.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task<TResult> Then<TResult>(this Task task, Func<Task<TResult>> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TResult>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception!.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try
                    {
                        next().ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        public static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task, Func<TResult, Task<TNewResult>> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var result = new TaskCompletionSource<TNewResult>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) result.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) result.TrySetCanceled();
                else
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try
                    {
                        next(task.Result).ContinueWith(t => result.TrySetFromTask(t), TaskScheduler.Default);
                    }
                    catch (Exception exc)
                    {
                        result.TrySetException(exc);
                    }
            }, TaskScheduler.Default);
            return result.Task;
        }
        #endregion
    }
}