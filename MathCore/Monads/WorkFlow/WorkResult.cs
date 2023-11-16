#nullable enable
namespace MathCore.Monads.WorkFlow;

/// <summary>Результат выполнения работы</summary>
public readonly struct WorkResult : IWorkResult, IEquatable<WorkResult>
{
    /// <inheritdoc />
    public Exception? Error { get; }

    /// <inheritdoc />
    public bool Success => Error is null;

    /// <inheritdoc />
    public bool Failure => !Success;

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
    public WorkResult(Exception? PrevError = null, Exception? CurrentError = null) =>
        Error = PrevError is null
            ? CurrentError
            : CurrentError is null 
                ? PrevError 
                : PrevError is AggregateException aggregate_exception
                    ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                    : new AggregateException(PrevError, CurrentError);

    public bool Equals(WorkResult other) => Equals(Error, other.Error);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WorkResult other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Error != null ? Error.GetHashCode() : 0;

    public static bool operator ==(WorkResult left, WorkResult right) => left.Equals(right);
    public static bool operator !=(WorkResult left, WorkResult right) => !left.Equals(right);
}

/// <summary>Результат выполнения работы со значением</summary>
/// <typeparam name="T">Тип результата, доступного после выполнения работы</typeparam>
public readonly struct WorkResult<T> : IWorkResult<T>, IEquatable<WorkResult<T>>
{
    /// <inheritdoc />
    public Exception Error { get; }

    /// <inheritdoc />
    public bool Success => Error is null;

    /// <inheritdoc />
    public bool Failure => !Success;

    /// <inheritdoc />
    public T Result { get; }

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="Result">Результат выполнения работы</param>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    public WorkResult(T Result, Exception? PrevError = null)
    {
        Error       = PrevError;
        this.Result = Result;
    }

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
    public WorkResult(Exception? PrevError = null, Exception? CurrentError = null)
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

    /// <inheritdoc />
    public bool Equals(WorkResult<T> other) => Equals(Error, other.Error) && EqualityComparer<T>.Default.Equals(Result, other.Result);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WorkResult<T> result && Equals(result);

    /// <inheritdoc />
    public override int GetHashCode() => unchecked((Error != null ? Error.GetHashCode() : 0) * 397 ^ EqualityComparer<T>.Default.GetHashCode(Result));

    public static bool operator ==(WorkResult<T> left, WorkResult<T> right) => left.Equals(right);

    public static bool operator !=(WorkResult<T> left, WorkResult<T> right) => !(left == right);
}

/// <summary>Результат выполнения параметрической работы</summary>
/// <typeparam name="TParameter">Тип параметра работы</typeparam>
/// <typeparam name="T">Тип значения, доступного по завершении работы</typeparam>
public readonly struct WorkResult<TParameter, T> : IWorkResult<TParameter, T>, IEquatable<WorkResult<TParameter, T>>
{
    /// <inheritdoc />
    public Exception Error { get; }

    /// <inheritdoc />
    public bool Success => Error is null;

    /// <inheritdoc />
    public bool Failure => !Success;

    /// <inheritdoc />
    public TParameter Parameter { get; }

    /// <inheritdoc />
    public T Result { get; }

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    public WorkResult(Exception PrevError)
    {
        Error     = PrevError;
        Parameter = default;
        Result    = default;
    }

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="Parameter">Параметр текущего процесса выполнения работы</param>
    /// <param name="Result">Результат выполнения работы</param>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    public WorkResult(TParameter Parameter, T Result, Exception? PrevError = null)
    {
        Error          = PrevError;
        this.Parameter = Parameter;
        this.Result    = Result;
    }

    /// <summary>Инициализация нового результата выполнения работы</summary>
    /// <param name="Parameter">Параметр текущего процесса выполнения работы</param>
    /// <param name="PrevError">Ошибка предыдущего процесса выполнения работы</param>
    /// <param name="CurrentError">Ошибка текущего процесса выполнения работы</param>
    public WorkResult(TParameter Parameter, Exception? PrevError = null, Exception? CurrentError = null)
    {
        this.Parameter = Parameter;
        Result         = default;
        Error = PrevError is null
            ? CurrentError
            : CurrentError is null
                ? PrevError
                : PrevError is AggregateException aggregate_exception
                    ? new AggregateException(aggregate_exception.InnerExceptions.AppendLast(CurrentError))
                    : new AggregateException(PrevError, CurrentError);
    }

    /// <inheritdoc />
    public bool Equals(WorkResult<TParameter, T> other) => Equals(Error, other.Error) && EqualityComparer<TParameter>.Default.Equals(Parameter, other.Parameter) && EqualityComparer<T>.Default.Equals(Result, other.Result);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WorkResult<TParameter, T> other && Equals(other);

    /// <inheritdoc />
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