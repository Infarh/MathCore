using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

public class SubstitutionVisitor([NotNull] LambdaExpression Substitution) : ExpressionVisitorEx
{
    private readonly LambdaExpression _Substitution = Substitution;
    private readonly ParameterExpression _Parameter = Substitution.Parameters.First();

    protected override Expression VisitParameter(ParameterExpression p)
        => p.Name == _Parameter.Name && p.Type == _Parameter.Type
            ? _Substitution.Body
            : base.VisitParameter(p);
}