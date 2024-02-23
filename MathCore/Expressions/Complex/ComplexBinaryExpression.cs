namespace MathCore.Expressions.Complex;

public abstract class ComplexBinaryExpression(ComplexExpression Left, ComplexExpression Right) : ComplexExpression
{
    public ComplexExpression Left { get; private set; } = Left;
    public ComplexExpression Right { get; private set; } = Right;
}