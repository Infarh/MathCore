using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Monades
{
    public interface IWorkExecutionResult
    {
        Exception Error { get; }
        bool Success { get; }
        bool Failure { get; }
    }

    public interface IWorkExecutionResult<out T> : IWorkExecutionResult
    {
        T Result { get; }
    }

    public readonly struct WorkExecutionResult : IWorkExecutionResult
    {
        [CanBeNull] public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;

        public WorkExecutionResult([CanBeNull] Exception Error = null) => this.Error = Error;
    }

    public readonly struct WorkExecutionResult<T> : IWorkExecutionResult<T>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public T Result { get; }

        public WorkExecutionResult(T Result, Exception Error = null)
        {
            this.Error = Error;
            this.Result = Result;
        }
    }

    public abstract class Work
    {
        #region Поля

        [CanBeNull] protected readonly Work _BaseWork;

        #endregion

        #region Свойства

        [NotNull, ItemNotNull]
        public IEnumerable<Work> SubWorks
        {
            get
            {
                var work = _BaseWork;
                while (work != null)
                {
                    yield return work;
                    work = work._BaseWork;
                }
            }
        }

        public bool Executed { get; private set; }

        [CanBeNull] public Exception CurrentError { get; private set; }

        [CanBeNull] public Exception Error => CurrentError ?? SubWorks.FirstOrDefault(w => w.CurrentError != null)?.CurrentError;

        public virtual bool Success => CurrentError is null && SubWorks.All(work => work.Success);

        public bool Failure => !Success; 

        #endregion

       
    }

    public abstract class Work<T> : Work
    {
       
    }

    public class Work<T, TResult> : Work<TResult>
    {
       
    }
}