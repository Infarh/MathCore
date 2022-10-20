using System.Linq.Expressions;

namespace MathCore.Expressions.Complex;

public sealed class ComplexDivideExpression : ComplexLambdaBinaryExpression
{
    private static Expression GetNorma(ComplexExpression R) => Add(GetPower(R.Re, Constant(2d)), GetPower(R.Im, Constant(2d)));

    internal ComplexDivideExpression(ComplexExpression Left, ComplexExpression Right)
        : base(Left, Right,
            (L, R) => Divide(Add(Multiply(L.Re, R.Re), Multiply(Left.Im, Right.Im)), GetNorma(R)),
            (L, R) => Divide(Subtract(Multiply(L.Im, R.Re), Multiply(L.Re, R.Im)), GetNorma(R))) { }

    //protected override Expression GetRe() { return Divide(Add(Multiply(Left.Re, Right.Re), Multiply(Left.Im, Right.Im)), Norma); }
    //protected override Expression GetIm() { return Divide(Subtract(Multiply(Left.Im, Right.Re), Multiply(Left.Re, Right.Im)), Norma); }
}