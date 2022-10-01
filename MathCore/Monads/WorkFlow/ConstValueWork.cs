#nullable enable
namespace MathCore.Monads.WorkFlow;

/// <summary>Работа, возвращающая указанное константное значение</summary>
/// <typeparam name="T">Тип возвращаемого работой значения</typeparam>
public class ConstValueWork<T> : Work<T>
{
    /// <summary>Значение, возвращаемое работой</summary>
    public T Value { get; set; }

    /// <summary>Инициализация новой работы, возвращающей константное значение</summary>
    /// <param name="Value">Значение, которое будет возвращать работа</param>
    /// <param name="BaseWork">Базовая работа</param>
    internal ConstValueWork(T Value, Work? BaseWork = null) : base(BaseWork) => this.Value = Value;

    /// <inheritdoc />
    protected override IWorkResult Execute(IWorkResult? BaseResult) => new WorkResult<T>(Value, BaseResult?.Error);
}