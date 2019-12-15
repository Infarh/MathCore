using System;
using MathCore.Annotations;

namespace MathCore.Monades.WorkFlow
{
    public readonly struct WorkResult : IWorkResult
    {
        [CanBeNull] public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;

        public WorkResult([CanBeNull] Exception Error = null) => this.Error = Error;
    }

    public readonly struct WorkResult<T> : IWorkResult<T>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public T Result { get; }

        public WorkResult(T Result, Exception Error = null)
        {
            this.Error = Error;
            this.Result = Result;
        }
    }
}