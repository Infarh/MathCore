namespace MathCore.Expressions.Complex
{
    public sealed class ComplexConjugateExpression : ComplexLambdaUnaryExpression
    {
        public ComplexConjugateExpression(ComplexExpression Value) : base(Value, z => z.Re, z => Negate(z.Im)) { }
    }
}