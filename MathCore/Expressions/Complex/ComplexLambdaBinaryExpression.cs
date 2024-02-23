using System.Linq.Expressions;

namespace MathCore.Expressions.Complex;

public class ComplexLambdaBinaryExpression(
    ComplexExpression Left,
    ComplexExpression Right,
    Func<ComplexExpression, ComplexExpression, Expression> GetReExpr,
    Func<ComplexExpression, ComplexExpression, Expression> GetImExpr)
    : ComplexBinaryExpression(Left, Right)
{
    protected override Expression GetRe() => GetReExpr(Left, Right);

    protected override Expression GetIm() => GetImExpr(Left, Right);
}