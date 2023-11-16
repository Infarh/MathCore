#nullable enable
using System.Linq.Expressions;

namespace MathCore.MathParser;

[NotImplemented]
public class DifferentialTransformationVisitor : ExpressionVisitorEx
{
    private readonly object _Differentiate_LockObject = new();
    private ParameterExpression _Differential_Parameter;

    public DifferentialTransformationVisitor() => throw new NotImplementedException();

    public Expression Differentiate<TDelegate>(Expression<TDelegate> expression, string ParameterName)
    {
        var parameter = (from p in expression.Parameters where p.Name == ParameterName select p).FirstOrDefault();
        if(parameter is null)
            throw new ArgumentException(@"Не задан параметр дифференцирования", nameof(ParameterName));

        lock(_Differentiate_LockObject)
        {
            _Differential_Parameter = parameter;
            return Visit(expression);
        }
    }

    protected override Expression VisitConstant(ConstantExpression c) => Expression.Constant(0);

    //protected override Expression VisitParameter(ParameterExpression p) => base.VisitParameter(p);

    protected override Expression VisitBinary(BinaryExpression b)
    {
        switch(b.NodeType)
        {
            case ExpressionType.Add:
                break;
            case ExpressionType.Subtract:
                break;
            case ExpressionType.Multiply:
                break;
            case ExpressionType.Divide:
                break;
        }

        return base.VisitBinary(b);
    }
}