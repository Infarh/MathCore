namespace MathCore.Expressions.Complex;

public abstract class ComplexUnaryExpression(ComplexExpression Value) : ComplexExpression
{
    public ComplexExpression Value { get; private set; } = Value;
}