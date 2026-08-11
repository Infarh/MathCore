using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MathCore.Extensions.AsyncAwait;

/// <summary>Ожидаемый объект, выполняющий действие перед началом задачи</summary>
/// <param name="Method">Действие, выполняемое перед задачей</param>
/// <param name="Task">Задача</param>
/// <param name="LockContext">Использовать ли исходный контекст синхронизации</param>
public readonly ref struct PerformActionAwaitable(Action Method, Task Task, bool LockContext = true)
{
    private readonly Task _Task = Task;

    /// <summary>Возвращает объект ожидания</summary>
    /// <returns>Объект ожидания</returns>
    public Awaiter GetAwaiter() => new(Method, _Task, LockContext);

    /// <summary>Объект ожидания выполнения задачи с предварительным действием</summary>
    public readonly struct Awaiter(Action Method, Task Task, bool LockContext) : ICriticalNotifyCompletion
    {
        private readonly Action _Method = Method;
        private readonly Task _Task = Task;
        private readonly bool _LockContext = LockContext;
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        /// <summary>Завершена ли задача</summary>
        public bool IsCompleted => _Task.IsCompleted;

        /// <summary>Получение результата выполнения задачи</summary>
        public void GetResult() => _Task.Wait();

        private static void RunAction(object? State) => ((Action)State!).Invoke();

        /// <summary>Планирует продолжение при завершении задачи</summary>
        /// <param name="Continuation">Продолжение</param>
        public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, true, _LockContext);

        /// <summary>Планирует продолжение при завершении задачи без потока контекста</summary>
        /// <param name="Continuation">Продолжение</param>
        public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, false, _LockContext);

        private static void QueueContinuation(Action Continuation, Action Method, bool FlowContext, bool LockContext)
        {
            if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

            var context = SynchronizationContext.Current;
            if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
            {
                context.Post(__SendOrPostCallbackRunAction, Method);
                context.Post(__SendOrPostCallbackRunAction, Continuation);
            }
            else
            {
                var scheduler = TaskScheduler.Current;
                if (scheduler == TaskScheduler.Default)
                {
                    if (FlowContext)
                    {
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                }
                else
                {
                    Task.Factory.StartNew(Method, default, TaskCreationOptions.PreferFairness, scheduler);
                    Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }
        }

        /// <summary>Проверка равенства объектов ожидания</summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Истина, если объекты равны</returns>
        public override bool Equals(object? obj) => obj is Awaiter awaiter && Equals(awaiter);

        /// <summary>Хэш-код объекта ожидания</summary>
        /// <returns>Хэш-код</returns>
        public override int GetHashCode() => _LockContext.GetHashCode();

        /// <summary>Оператор равенства объектов ожидания</summary>
        public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

        /// <summary>Оператор неравенства объектов ожидания</summary>
        public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

        /// <summary>Проверка равенства объектов ожидания</summary>
        /// <param name="other">Сравниваемый объект ожидания</param>
        /// <returns>Истина, если объекты равны</returns>
        public bool Equals(Awaiter other) => other._LockContext == _LockContext && other._Method == _Method;
    }
}

/// <summary>Ожидаемый объект, выполняющий действие перед началом задачи с результатом</summary>
/// <param name="Method">Действие, выполняемое перед задачей</param>
/// <param name="task">Задача с результатом</param>
/// <param name="LockContext">Использовать ли исходный контекст синхронизации</param>
/// <typeparam name="T">Тип результата задачи</typeparam>
public readonly ref struct PerformActionAwaitable<T>(Action Method, Task<T> task, bool LockContext = true)
{
    /// <summary>Возвращает объект ожидания</summary>
    /// <returns>Объект ожидания</returns>
    public Awaiter GetAwaiter() => new(Method, task, LockContext);

    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    /// <summary>Объект ожидания выполнения задачи с результатом и предварительным действием</summary>
    public readonly struct Awaiter(Action Method, Task<T> task, bool LockContext) : ICriticalNotifyCompletion
    {
        private readonly Action _Method = Method;
        private readonly bool _LockContext = LockContext;
        private static readonly WaitCallback __WaitCallbackRunAction = RunAction;
        private static readonly SendOrPostCallback __SendOrPostCallbackRunAction = RunAction;

        /// <summary>Завершена ли задача</summary>
        public bool IsCompleted => task.IsCompleted;

        /// <summary>Получение результата выполнения задачи</summary>
        /// <returns>Результат задачи</returns>
        public T GetResult() => task.Result;

        private static void RunAction(object? State) => ((Action)State!).Invoke();

        /// <summary>Планирует продолжение при завершении задачи</summary>
        /// <param name="Continuation">Продолжение</param>
        public void OnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, true, _LockContext);

        /// <summary>Планирует продолжение при завершении задачи без потока контекста</summary>
        /// <param name="Continuation">Продолжение</param>
        public void UnsafeOnCompleted(Action Continuation) => QueueContinuation(Continuation, _Method, false, _LockContext);

        private static void QueueContinuation(Action Continuation, Action Method, bool FlowContext, bool LockContext)
        {
            if (Continuation is null) throw new ArgumentNullException(nameof(Continuation));

            var context = SynchronizationContext.Current;
            if (LockContext && context != null && context.GetType() != typeof(SynchronizationContext))
            {
                context.Post(__SendOrPostCallbackRunAction, Method);
                context.Post(__SendOrPostCallbackRunAction, Continuation);
            }
            else
            {
                var scheduler = TaskScheduler.Current;
                if (scheduler == TaskScheduler.Default)
                {
                    if (FlowContext)
                    {
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.QueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                    else
                    {
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Method);
                        ThreadPool.UnsafeQueueUserWorkItem(__WaitCallbackRunAction, Continuation);
                    }
                }
                else
                {
                    Task.Factory.StartNew(Method, default, TaskCreationOptions.PreferFairness, scheduler);
                    Task.Factory.StartNew(Continuation, default, TaskCreationOptions.PreferFairness, scheduler);
                }
            }
        }

        /// <summary>Проверка равенства объектов ожидания</summary>
        /// <param name="obj">Сравниваемый объект</param>
        /// <returns>Истина, если объекты равны</returns>
        public override bool Equals(object? obj) => obj is Awaiter awaiter && Equals(awaiter);

        /// <summary>Хэш-код объекта ожидания</summary>
        /// <returns>Хэш-код</returns>
        public override int GetHashCode() => _LockContext.GetHashCode();

        /// <summary>Оператор равенства объектов ожидания</summary>
        public static bool operator ==(Awaiter left, Awaiter right) => left.Equals(right);

        /// <summary>Оператор неравенства объектов ожидания</summary>
        public static bool operator !=(Awaiter left, Awaiter right) => !(left == right);

        /// <summary>Проверка равенства объектов ожидания</summary>
        /// <param name="other">Сравниваемый объект ожидания</param>
        /// <returns>Истина, если объекты равны</returns>
        public bool Equals(Awaiter other) => other._LockContext == _LockContext && other._Method == _Method;
    }
}