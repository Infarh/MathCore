#nullable enable
using System.Linq.Expressions;

namespace MathCore.MathParser;

public class DifferentialOperator : Functional
{
    /// <inheritdoc />
    public DifferentialOperator() : base("Diff") => throw new NotImplementedException();

    public DifferentialOperator(string Name) : base(Name) => throw new NotImplementedException();

    /// <inheritdoc />
    public override double GetValue(MathExpression ParametersExpression, MathExpression Function) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Expression Compile(MathExpression ParametersExpression, MathExpression Function) => throw new NotImplementedException();

    /// <inheritdoc />
    public override Expression Compile(MathExpression ParametersExpression, MathExpression Function, ParameterExpression[] Parameters) => throw new NotImplementedException();
}