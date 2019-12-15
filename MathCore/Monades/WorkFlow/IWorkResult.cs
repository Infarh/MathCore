using System;

namespace MathCore.Monades.WorkFlow
{
    public interface IWorkResult
    {
        Exception Error { get; }
        bool Success { get; }
        bool Failure { get; }
    }

    public interface IWorkResult<out T> : IWorkResult
    {
        T Result { get; }
    }
}