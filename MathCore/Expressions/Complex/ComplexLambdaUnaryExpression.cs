using System.Linq.Expressions;

namespace MathCore.Expressions.Complex;

public class ComplexLambdaUnaryExpression(
    ComplexExpression Value,
    Func<ComplexExpression, Expression> GetReExpr,
    Func<ComplexExpression, Expression> GetImExpr)
    : ComplexUnaryExpression(Value)
{
    protected override Expression GetRe() => GetReExpr(Value);

    protected override Expression GetIm() => GetImExpr(Value);
}