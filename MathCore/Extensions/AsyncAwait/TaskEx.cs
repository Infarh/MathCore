using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathCore.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
    public static class TaskEx
    {
        /// <summary>Переход в асинхронную область - в новый поток из пула потоков</summary>
        public static YieldAsyncAwaitable YieldAsync() => new YieldAsyncAwaitable();

        /// <summary>Переключиться в контекст планировщика потоков</summary>
        /// <param name="scheduler">Планировщик потоков, распределяющий процессы выполнения задач</param>
        [NotNull] public static TaskSchedulerAwaiter SwitchContext(this TaskScheduler scheduler) => new TaskSchedulerAwaiter(scheduler);

        [NotNull, ItemNotNull] public static Task<Task> WhenAny([NotNull, ItemNotNull] this IEnumerable<Task> tasks) => Task.WhenAny(tasks);
        [NotNull, ItemNotNull] public static Task<Task<T>> WhenAny<T>([NotNull, ItemNotNull] this IEnumerable<Task<T>> tasks) => Task.WhenAny(tasks);


        [NotNull] public static Task WhenAll([NotNull, ItemNotNull] this IEnumerable<Task> tasks) => Task.WhenAll(tasks);
        [NotNull, ItemNotNull] public static Task<T[]> WhenAll<T>([NotNull, ItemNotNull] this IEnumerable<Task<T>> tasks) => Task.WhenAll(tasks);

        public static void AndForget<T>([NotNull, ItemCanBeNull] this Task<T> task, [CanBeNull] Action<AggregateException> OnException = null) => task.ContinueWith(t => OnException?.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        public static void AndForget([NotNull] this Task task, [CanBeNull] Action<AggregateException> OnException = null) => task.ContinueWith(t => OnException?.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

        [NotNull, ItemCanBeNull]
        public static async Task<T> WithCancellation<T>([NotNull, ItemCanBeNull] this Task<T> task, CancellationToken cancel)
        {
            var tcs = new TaskCompletionSource<object>();
            using (cancel.Register(t => ((TaskCompletionSource<object>)t).TrySetResult(default), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException();
            return await task;
        }

        [NotNull] public static Task Catch<T>([NotNull, ItemCanBeNull] this Task<T> task, [NotNull] Action<AggregateException> ProcessExceptions) => task.ContinueWith(t => ProcessExceptions(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        [NotNull] public static Task Catch([NotNull] this Task task, [NotNull] Action<AggregateException> ProcessExceptions) => task.ContinueWith(t => ProcessExceptions(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

        [NotNull] public static Task Finally<T>([NotNull, ItemNotNull] this Task<T> task, [NotNull] Action OnTaskCompleted) => task.ContinueWith(t => OnTaskCompleted(), TaskContinuationOptions.None);
        [NotNull] public static Task Finally([NotNull] this Task task, [NotNull] Action OnTaskCompleted) => task.ContinueWith(t => OnTaskCompleted(), TaskContinuationOptions.None);

        /// https://blogs.msdn.microsoft.com/pfxteam/2010/04/04/a-tour-of-parallelextensionsextras/
        [NotNull, ItemCanBeNull]
        public static Task<TResult> Select<TSource, TResult>([NotNull, ItemCanBeNull] this Task<TSource> source, [NotNull] Func<TSource, TResult> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled);
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> SelectMany<TSource, TResult>([NotNull, ItemCanBeNull] this Task<TSource> source, [NotNull] Func<TSource, Task<TResult>> selector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (selector is null) throw new ArgumentNullException(nameof(selector));

            return source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> SelectMany<TSource, TCollection, TResult>
        (
            [NotNull, ItemNotNull] this Task<TSource> source,
            [NotNull] Func<TSource, Task<TCollection>> CollectionSelector,
            [NotNull] Func<TSource, TCollection, TResult> ResultSelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (CollectionSelector is null) throw new ArgumentNullException(nameof(CollectionSelector));
            if (ResultSelector is null) throw new ArgumentNullException(nameof(ResultSelector));

            return source.ContinueWith(t => CollectionSelector(t.Result).ContinueWith(c => ResultSelector(t.Result, c.Result), TaskContinuationOptions.NotOnCanceled), TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> Where<TSource>([NotNull, ItemCanBeNull] this Task<TSource> source, [NotNull] Func<TSource, bool> predicate)
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
            }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>
        (
            [NotNull, ItemCanBeNull] this Task<TOuter> Outer,
            [NotNull, ItemCanBeNull] Task<TInner> Inner,
            [NotNull] Func<TOuter, TKey> OuterKeySelector,
            [NotNull] Func<TInner, TKey> InnerKeySelector,
            [NotNull] Func<TOuter, TInner, TResult> ResultSelector
        ) =>
            Join(Outer, Inner, OuterKeySelector, InnerKeySelector, ResultSelector, EqualityComparer<TKey>.Default);

        [NotNull, ItemCanBeNull]
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>
        (
            [NotNull, ItemCanBeNull] this Task<TOuter> Outer,
            [NotNull, ItemCanBeNull] Task<TInner> Inner,
            [NotNull] Func<TOuter, TKey> OuterKeySelector,
            [NotNull] Func<TInner, TKey> InnerKeySelector,
            [NotNull] Func<TOuter, TInner, TResult> ResultSelector,
            [NotNull] IEqualityComparer<TKey> Comparer
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
                    return default; // won't be reached
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>
        (
            [NotNull, ItemCanBeNull] this Task<TOuter> outer,
            [NotNull, ItemCanBeNull] Task<TInner> inner,
            [NotNull] Func<TOuter, TKey> OuterKeySelector,
            [NotNull] Func<TInner, TKey> InnerKeySelector,
            [NotNull] Func<TOuter, Task<TInner>, TResult> ResultSelector
        ) =>
            GroupJoin(outer, inner, OuterKeySelector, InnerKeySelector, ResultSelector, EqualityComparer<TKey>.Default);

        [NotNull, ItemCanBeNull]
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>
        (
            [NotNull, ItemCanBeNull] this Task<TOuter> outer,
            [NotNull, ItemCanBeNull] Task<TInner> inner,
            [NotNull] Func<TOuter, TKey> OuterKeySelector,
            [NotNull] Func<TInner, TKey> InnerKeySelector,
            [NotNull] Func<TOuter, Task<TInner>, TResult> ResultSelector,
            [NotNull] IEqualityComparer<TKey> Comparer
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
                    return default; // won't be reached
                }, cts.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);
            }, TaskContinuationOptions.NotOnCanceled).Unwrap();
        }

        [NotNull, ItemCanBeNull]
        public static Task<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>
        (
            [NotNull, ItemCanBeNull] this Task<TSource> source,
            [NotNull] Func<TSource, TKey> KeySelector,
            [NotNull] Func<TSource, TElement> ElementSelector
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
            }, TaskContinuationOptions.NotOnCanceled);
        }

        /// <summary>Represents a grouping of one element.</summary>
        /// <typeparam name="TKey">The type of the key for the element.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        private class OneElementGrouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            [CanBeNull] public TKey Key { get; internal set; }
            [CanBeNull] internal TElement Element { get; set; }
            public IEnumerator<TElement> GetEnumerator() { yield return Element; }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> OrderBy<TSource, TKey>([NotNull, ItemCanBeNull] this Task<TSource> source, Func<TSource, TKey> KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> OrderByDescending<TSource, TKey>([NotNull, ItemCanBeNull] this Task<TSource> source, [CanBeNull] Func<TSource, TKey> KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> ThenBy<TSource, TKey>([NotNull, ItemCanBeNull] this Task<TSource> source, [CanBeNull] Func<TSource, TKey> KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TSource> ThenByDescending<TSource, TKey>([NotNull, ItemCanBeNull] this Task<TSource> source, [CanBeNull] Func<TSource, TKey> KeySelector)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            // A single item is already in sorted order, no matter what the key selector is, so just
            // return the original.
            return source;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> WithTimeout<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task, in TimeSpan timeout)
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


        [NotNull]
        public static Task WithAsyncCallback([NotNull] this Task task, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
        {
            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(_ =>
            {
                tcs.TrySetFromTask(task);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        [NotNull, ItemCanBeNull]
        public static Task<TResult> WithAsyncCallback<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
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
        [NotNull]
        public static Task ContinueWith([NotNull] this Task task, [NotNull] Action<Task> ContinuationAction, [NotNull] TaskFactory factory) =>
            task.ContinueWith(ContinuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TResult> ContinueWith<TResult>([NotNull] this Task task, [NotNull] Func<Task, TResult> ContinuationFunction, [NotNull] TaskFactory factory) =>
            task.ContinueWith(ContinuationFunction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);
        #endregion

        #region ContinueWith accepting TaskFactory<TResult>
        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationAction">The continuation action.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        [NotNull]
        public static Task ContinueWith<TResult>([NotNull] this Task<TResult> task, [NotNull] Action<Task<TResult>> ContinuationAction, [NotNull] TaskFactory<TResult> factory) =>
            task.ContinueWith(ContinuationAction, factory.CancellationToken, factory.ContinuationOptions, factory.Scheduler);

        /// <summary>Creates a continuation task using the specified TaskFactory.</summary>
        /// <param name="task">The antecedent Task.</param>
        /// <param name="ContinuationFunction">The continuation function.</param>
        /// <param name="factory">The TaskFactory.</param>
        /// <returns>A continuation task.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TNewResult> ContinueWith<TResult, TNewResult>([NotNull] this Task<TResult> task, [NotNull] Func<Task<TResult>, TNewResult> ContinuationFunction, [NotNull] TaskFactory<TResult> factory) =>
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
        [NotNull]
        public static Task ToAsync([NotNull] this Task task, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
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
        [NotNull, ItemCanBeNull]
        public static Task<TResult> ToAsync<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task, [CanBeNull] AsyncCallback callback, [CanBeNull] object state)
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
        [NotNull]
        public static Task IgnoreExceptions([NotNull] this Task task)
        {
            task.ContinueWith(t => _ = t.Exception,
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default);
            return task;
        }

        [NotNull]
        public static Task TraceExceptions([NotNull] this Task task, [CanBeNull] string Message = null)
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
        [NotNull, ItemCanBeNull]
        public static Task<T> IgnoreExceptions<T>([NotNull, ItemCanBeNull] this Task<T> task) => (Task<T>)((Task)task).IgnoreExceptions();

        /// <summary>Fails immediately when an exception is encountered.</summary>
        /// <param name="task">The Task to be monitored.</param>
        /// <returns>The original Task.</returns>
        [NotNull]
        public static Task FailFastOnException([NotNull] this Task task)
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
        [NotNull, ItemCanBeNull]
        public static Task<T> FailFastOnException<T>([NotNull, ItemCanBeNull] this Task<T> task) => (Task<T>)((Task)task).FailFastOnException();

        /// <summary>Propagates any exceptions that occurred on the specified task.</summary>
        /// <param name="task">The Task whose exceptions are to be propagated.</param>
        public static void PropagateExceptions([NotNull] this Task task)
        {
            if (!task.IsCompleted) throw new InvalidOperationException("The task has not completed.");
            if (task.IsFaulted) task.Wait();
        }

        /// <summary>Propagates any exceptions that occurred on the specified tasks.</summary>
        /// <param name="tasks">The Tassk whose exceptions are to be propagated.</param>
        public static void PropagateExceptions([NotNull, ItemCanBeNull] this Task[] tasks)
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
        [NotNull]
        public static IObservable<TResult> ToObservable<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            return new TaskObservable<TResult>(task);
        }

        /// <summary>An implementation of IObservable that wraps a Task.</summary>
        /// <typeparam name="TResult">The type of data returned by the task.</typeparam>
        private class TaskObservable<TResult> : IObservable<TResult>
        {
            [NotNull, ItemCanBeNull] private readonly Task<TResult> f_ObservedTask;

            public TaskObservable([NotNull, ItemCanBeNull] Task<TResult> ObservedTask) => f_ObservedTask = ObservedTask;

            public IDisposable Subscribe(IObserver<TResult> observer)
            {
                // Validate arguments
                if (observer is null) throw new ArgumentNullException(nameof(observer));

                // Support cancelling the continuation if the observer is unsubscribed
                var cts = new CancellationTokenSource();

                // Create a continuation to pass data along to the observer
                f_ObservedTask.ContinueWith(t =>
                {
                    switch (t.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            observer.OnNext(f_ObservedTask.Result);
                            observer.OnCompleted();
                            break;

                        case TaskStatus.Faulted:
                            observer.OnError(f_ObservedTask.Exception ?? throw new InvalidOperationException());
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
            [NotNull] private readonly CancellationTokenSource f_Source;

            public CancelOnDispose([NotNull] CancellationTokenSource source) => f_Source = source;

            void IDisposable.Dispose() => f_Source.Cancel();
        }
        #endregion

        #region Timeouts
        /// <summary>Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.</summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        [NotNull]
        public static Task WithTimeout([NotNull] this Task task, in TimeSpan timeout)
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
        [NotNull, ItemCanBeNull]
        public static Task<TResult> WithTimeout<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task, TimeSpan timeout)
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
        public static void AttachToParent([NotNull] this Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            task.ContinueWith(t => t.Wait(), CancellationToken.None, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        #endregion

        #region Waiting
        ///// <summary>Waits for the task to complete execution, pumping in the meantime.</summary>
        ///// <param name="task">The task for which to wait.</param>
        ///// <remarks>This method is intended for usage with Windows Presentation Foundation.</remarks>
        //public static void WaitWithPumping([NotNull] this Task task)
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
        public static TaskStatus WaitForCompletionStatus([NotNull] this Task task)
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
        [NotNull]
        public static Task Then([NotNull] this Task task, [NotNull] Action next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    try
                    {
                        next();
                        tcs.TrySetResult(null);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TResult> Then<TResult>([NotNull] this Task task, [NotNull] Func<TResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TResult>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    try
                    {
                        var result = next();
                        tcs.TrySetResult(result);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up action when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The action to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the action.</returns>
        [NotNull]
        public static Task Then<TResult>([NotNull, ItemCanBeNull] this Task<TResult> task, [NotNull] Action<TResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    try
                    {
                        next(task.Result);
                        tcs.TrySetResult(null);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a follow-up function when a task completes.</summary>
        /// <param name="task">The task.</param>
        /// <param name="next">The function to run when the task completes.</param>
        /// <returns>The task that represents the completion of both the task and the function.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TNewResult> Then<TResult, TNewResult>([NotNull, ItemCanBeNull] this Task<TResult> task, [NotNull] Func<TResult, TNewResult> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TNewResult>();
            task.ContinueWith(_ =>
            {
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    try
                    {
                        var result = next(task.Result);
                        tcs.TrySetResult(result);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        [NotNull]
        public static Task Then([NotNull] this Task task, [NotNull] Func<Task> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                    // Otherwise, get the next task.  If it's null, bail.  If not,
                    // when it's done we'll have our result.
                    try
                    {
                        next().ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        [NotNull, ItemCanBeNull]
        public static Task Then<T>([NotNull, ItemCanBeNull] this Task<T> task, [NotNull] Func<T, Task> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                // Otherwise, get the next task.  If it's null, bail.  If not,
                // when it's done we'll have our result.
                    try
                    {
                        next(task.Result).ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TResult> Then<TResult>([NotNull] this Task task, [NotNull] Func<Task<TResult>> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TResult>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                // Otherwise, get the next task.  If it's null, bail.  If not,
                // when it's done we'll have our result.
                    try
                    {
                        next().ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>Creates a task that represents the completion of a second task when a first task completes.</summary>
        /// <param name="task">The first task.</param>
        /// <param name="next">The function that produces the second task based on the result of the first.</param>
        /// <returns>The task that represents the completion of both the first and second task.</returns>
        [NotNull, ItemCanBeNull]
        public static Task<TNewResult> Then<TResult, TNewResult>([NotNull, ItemCanBeNull] this Task<TResult> task, [NotNull] Func<TResult, Task<TNewResult>> next)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            if (next is null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<TNewResult>();
            task.ContinueWith(_ =>
            {
                // When the first task completes, if it faulted or was canceled, bail
                if (task.IsFaulted) tcs.TrySetException(task.Exception?.InnerExceptions ?? throw new InvalidOperationException());
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else
                // Otherwise, get the next task.  If it's null, bail.  If not,
                // when it's done we'll have our result.
                    try
                    {
                        next(task.Result).ContinueWith(t => tcs.TrySetFromTask(t), TaskScheduler.Default);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception exc)
                    {
                        tcs.TrySetException(exc);
                    }
#pragma warning restore CA1031 // Do not catch general exception types
            }, TaskScheduler.Default);
            return tcs.Task;
        }
        #endregion
    }
}