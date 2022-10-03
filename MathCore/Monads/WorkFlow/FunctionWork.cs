#nullable enable
using System;

namespace MathCore.Monads.WorkFlow;

/// <summary>Работа, выполняющая указанную функцию</summary>
/// <typeparam name="T">Тип значения функции</typeparam>
public class FunctionWork<T> : Work<T>
{
    /// <summary>Функция, выполняемая в рамках работы</summary>
    private readonly Func<T> _WorkFunction;

    /// <summary>Инициализация новой работы, выполняющей указанную функцию</summary>
    /// <param name="WorkFunction">Функция, выполняемая работой</param>
    /// <param name="BaseWork">Базовая работа</param>
    internal FunctionWork(Func<T> WorkFunction, Work? BaseWork = null) : base(BaseWork) => _WorkFunction = WorkFunction ?? throw new ArgumentNullException(nameof(WorkFunction));

    /// <inheritdoc />
    protected override IWorkResult Execute(IWorkResult? BaseResult)
    {
        try
        {
            var result = _WorkFunction();
            return new WorkResult<T>(result, BaseResult?.Error);
        }
        catch (Exception error)
        {
            return new WorkResult<T>(BaseResult?.Error, error);
        }
    }
}