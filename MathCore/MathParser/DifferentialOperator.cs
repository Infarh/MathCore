using System;
using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser
{
    public class DifferentialOperator : Functional
    {
        /// <inheritdoc />
        public DifferentialOperator() : base("Diff") => throw new NotImplementedException();

        public DifferentialOperator([NotNull] string Name) : base(Name) => throw new NotImplementedException();

        /// <inheritdoc />
        public override double GetValue(MathExpression ParametersExpression, MathExpression Function) => throw new NotImplementedException();

        /// <inheritdoc />
        public override Expression Compile(MathExpression ParametersExpression, MathExpression Function) => throw new NotImplementedException();

        /// <inheritdoc />
        public override Expression Compile(MathExpression ParametersExpression, MathExpression Function, ParameterExpression[] Parameters) => throw new NotImplementedException();
    }
}