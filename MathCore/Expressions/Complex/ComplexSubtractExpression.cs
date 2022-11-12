namespace MathCore.Expressions.Complex;

public sealed class ComplexSubtractExpression : ComplexLambdaBinaryExpression
{

    internal ComplexSubtractExpression(ComplexExpression Left, ComplexExpression Right)
        : base(Left, Right, (L, R) => Subtract(L.Re, R.Re), (L, R) => Subtract(L.Im, R.Im)) { }

    //protected override Expression GetRe() { return Subtract(Left.Re, Right.Re); }
    //protected override Expression GetIm() { return Subtract(Left.Im, Right.Im); }
}