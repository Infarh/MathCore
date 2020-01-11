using MathCore.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    public class SubstitutionVisitor : ExpressionVisitorEx
    {
        private readonly LambdaExpression _Substitution;
        private readonly ParameterExpression _Parameter;

        public SubstitutionVisitor([NotNull] LambdaExpression Substitution)
        {
            _Substitution = Substitution;
            _Parameter = Substitution.Parameters.First();
        }

        protected override Expression VisitParameter(ParameterExpression p)
            => p.Name == _Parameter.Name && p.Type == _Parameter.Type
                ? _Substitution.Body
                : base.VisitParameter(p);
    }
}