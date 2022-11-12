namespace MathCore.Expressions.Complex;

public sealed class ComplexInverseExpression : ComplexLambdaUnaryExpression
{
    public ComplexInverseExpression(ComplexExpression Value) : base(Value, z => Negate(z.Re), z => Negate(z.Im)) { }
}