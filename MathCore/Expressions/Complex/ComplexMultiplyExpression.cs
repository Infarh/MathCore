namespace MathCore.Expressions.Complex
{
    public sealed class ComplexMultiplyExpression : ComplexLambdaBinaryExpression
    {

        internal ComplexMultiplyExpression(ComplexExpression Left, ComplexExpression Right)
                    : base(Left, Right,
                                (L, R) => Subtract(Multiply(L.Re, R.Re), Multiply(L.Im, R.Im)),
                                (L, R) => Add(Multiply(L.Re, R.Im), Multiply(L.Im, R.Re))) { }

        //protected override Expression GetRe() { return Subtract(Multiply(Left.Re, Right.Re), Multiply(Left.Im, Right.Im)); }
        //protected override Expression GetIm() { return Add(Multiply(Left.Re, Right.Im), Multiply(Left.Im, Right.Re)); }
    }
}