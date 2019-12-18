using System;
using System.Collections.Generic;
using System.Linq;
using MathCore.Annotations;

namespace MathCore.Monades.WorkFlow
{
    /// <summary>Результат выполнения работы</summary>
    public readonly struct WorkResult : IWorkResult, IEquatable<WorkResult>
    {
        [CanBeNull] public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;

        /// <summary>Инициализация нового результата выполнения работы</summary>
        /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
        /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
        public WorkResult([CanBeNull] Exception PrevError = null, [CanBeNull] Exception CurrentError = null) =>
            Error = PrevError is null
                ? CurrentError
                : CurrentError is null 
                    ? PrevError 
                    : PrevError is AggregateException aggregate_exception
                        ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                        : new AggregateException(PrevError, CurrentError);

        public bool Equals(WorkResult other) => Equals(Error, other.Error);

        public override bool Equals(object obj) => obj is WorkResult other && Equals(other);

        public override int GetHashCode() => Error != null ? Error.GetHashCode() : 0;

        public static bool operator ==(WorkResult left, WorkResult right) => left.Equals(right);
        public static bool operator !=(WorkResult left, WorkResult right) => !left.Equals(right);
    }

    /// <summary>Результат выполнения работы со значением</summary>
    /// <typeparam name="T">Тип результата, доступного после выполнения работы</typeparam>
    public readonly struct WorkResult<T> : IWorkResult<T>, IEquatable<WorkResult<T>>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public T Result { get; }

        /// <summary>Инициализация нового результата выполнения работы</summary>
        /// <param name="Result">Результат выполнения работы</param>
        /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
        public WorkResult(T Result, [CanBeNull] Exception PrevError = null)
        {
            Error = PrevError;
            this.Result = Result;
        }

        /// <summary>Инициализация нового результата выполнения работы</summary>
        /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
        /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
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

        public bool Equals(WorkResult<T> other) => Equals(Error, other.Error) && EqualityComparer<T>.Default.Equals(Result, other.Result);

        public override bool Equals(object obj) => obj is WorkResult<T> result && Equals(result);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Error != null ? Error.GetHashCode() : 0) * 397 ^ EqualityComparer<T>.Default.GetHashCode(Result);
            }
        }

        public static bool operator ==(WorkResult<T> left, WorkResult<T> right) => left.Equals(right);

        public static bool operator !=(WorkResult<T> left, WorkResult<T> right) => !(left == right);
    }

    /// <summary>Результат выполнения параметрической работы</summary>
    /// <typeparam name="TParameter">Тип параметра работы</typeparam>
    /// <typeparam name="T">Тип значения, доступного по завершении работы</typeparam>
    public readonly struct WorkResult<TParameter, T> : IWorkResult<TParameter, T>, IEquatable<WorkResult<TParameter, T>>
    {
        public Exception Error { get; }
        public bool Success => Error is null;
        public bool Failure => !Success;
        public TParameter Parameter { get; }
        public T Result { get; }

        /// <summary>Инициализация нового результата выполнения работы</summary>
        /// <param name="Parameter">Параметр текущего процесса выполнения работы</param>
        /// <param name="Result">Результат выполнения работы</param>
        /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
        public WorkResult(TParameter Parameter, T Result, [CanBeNull] Exception PrevError = null)
        {
            Error = PrevError;
            this.Parameter = Parameter;
            this.Result = Result;
        }

        /// <summary>Инициализация нового результата выполнения работы</summary>
        /// <param name="Parameter">Параметр текущего процесса выполнения работы</param>
        /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
        /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
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

        public bool Equals(WorkResult<TParameter, T> other) => Equals(Error, other.Error) && EqualityComparer<TParameter>.Default.Equals(Parameter, other.Parameter) && EqualityComparer<T>.Default.Equals(Result, other.Result);

        public override bool Equals(object obj) => obj is WorkResult<TParameter, T> other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash_code = Error != null ? Error.GetHashCode() : 0;
                hash_code = hash_code * 397 ^ EqualityComparer<TParameter>.Default.GetHashCode(Parameter);
                hash_code = hash_code * 397 ^ EqualityComparer<T>.Default.GetHashCode(Result);
                return hash_code;
            }
        }

        public static bool operator ==(WorkResult<TParameter, T> left, WorkResult<TParameter, T> right) => left.Equals(right);
        public static bool operator !=(WorkResult<TParameter, T> left, WorkResult<TParameter, T> right) => !left.Equals(right);
    }
}