namespace MathCore.Expressions.Complex
{
    public sealed class ComplexAddExpression : ComplexLambdaBinaryExpression
    {

        internal ComplexAddExpression(ComplexExpression Left, ComplexExpression Right)
                    : base(Left, Right, (L, R) => Add(L.Re, R.Re), (L, R) => Add(L.Im, R.Im)) { }

        //protected override Expression GetRe() { return Add(Left.Re, Right.Re); }
        //protected override Expression GetIm() { return Add(Left.Im, Right.Im); }

    }
}