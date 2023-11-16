#nullable enable
namespace MathCore.MathParser;

/// <summary>Оператор</summary>
public class ExpressionOperator : ExpressionItem
{
    /// <summary>Получить значение</summary>
    /// <returns>Значение оператора</returns>
    public override double GetValue() => throw new NotImplementedException();
}