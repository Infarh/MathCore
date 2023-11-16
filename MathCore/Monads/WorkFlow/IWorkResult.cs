#nullable enable
namespace MathCore.Monads.WorkFlow;

/// <summary>Результат выполнения работы</summary>
public interface IWorkResult
{
    /// <summary>Ошибка, которая произошла в ходе выполнения работы</summary>
    Exception Error { get; }

    /// <summary>Признак того, что работа завершилась успешно</summary>
    bool Success { get; }

    /// <summary>Признак того, что работа завершилась провалом</summary>
    bool Failure { get; }
}

/// <summary>Результат выполнения работы со значением</summary>
/// <typeparam name="T">Тип результата, доступного после выполнения работы</typeparam>
public interface IWorkResult<out T> : IWorkResult
{
    /// <summary>Результат выполнения работы</summary>
    T Result { get; }
}

/// <summary>Результат выполнения параметрической работы</summary>
/// <typeparam name="TParameter">Тип параметра работы</typeparam>
/// <typeparam name="T">Тип значения, доступного по завершении работы</typeparam>
public interface IWorkResult<out TParameter, out T> : IWorkResult<T>
{
    /// <summary>Параметр, с которым работа была выполнена</summary>
    TParameter Parameter { get; }
}