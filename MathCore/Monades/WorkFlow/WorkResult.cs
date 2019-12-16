using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Monades.WorkFlow
{
    public readonly struct WorkResult : IWorkResult
    {
        [CanBeNull] public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;

        public WorkResult([CanBeNull] Exception PrevError = null, [CanBeNull] Exception CurrentError = null) =>
            Error = PrevError is null
                ? CurrentError
                : CurrentError is null 
                    ? PrevError 
                    : PrevError is AggregateException aggregate_exception
                        ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                        : new AggregateException(PrevError, CurrentError);
    }

    public readonly struct WorkResult<T> : IWorkResult<T>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public T Result { get; }

        public WorkResult(T Result, [CanBeNull] Exception PrevError = null)
        {
            Error = PrevError;
            this.Result = Result;
        }

        public WorkResult([CanBeNull] Exception PrevError = null, [CanBeNull] Exception CurrentError = null)
        {
            Result = default;
            Error = PrevError is null
                ? CurrentError
                : CurrentError is null
                    ? PrevError
                    : PrevError is AggregateException aggregate_exception
                        ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                        : new AggregateException(PrevError, CurrentError);
        }
    }

    public readonly struct WorkResult<TParameter, T> : IWorkResult<TParameter, T>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public TParameter Parameter { get; }
        public T Result { get; }

        public WorkResult(TParameter Parameter, T Result, [CanBeNull] Exception PrevError = null)
        {
            Error = PrevError;
            this.Parameter = Parameter;
            this.Result = Result;
        }

        public WorkResult(TParameter Parameter, [CanBeNull] Exception PrevError = null, [CanBeNull] Exception CurrentError = null)
        {
            this.Parameter = Parameter;
            Result = default;
            Error = PrevError is null
                ? CurrentError
                : CurrentError is null
                    ? PrevError
                    : PrevError is AggregateException aggregate_exception
                        ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                        : new AggregateException(PrevError, CurrentError);
        }
    }
}