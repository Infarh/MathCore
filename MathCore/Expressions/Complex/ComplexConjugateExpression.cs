namespace MathCore.Expressions.Complex;

public sealed class ComplexConjugateExpression(ComplexExpression Value) : ComplexLambdaUnaryExpression(Value, z => z.Re, z => Negate(z.Im));