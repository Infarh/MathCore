namespace MathCore.Expressions.Complex;

public sealed class ComplexInverseExpression(ComplexExpression Value) : ComplexLambdaUnaryExpression(Value, z => Negate(z.Re), z => Negate(z.Im));