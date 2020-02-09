using System;
using System.Linq;
using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.MathParser
{
    [NotImplemented]
    public class DifferentialTransformationVisitor : ExpressionVisitorEx
    {
        private readonly object _Differentiate_LockObject = new object();
        private ParameterExpression _Differential_Parameter;

        public DifferentialTransformationVisitor() => throw new NotImplementedException();

        public Expression Differentiate<TDelegate>([NotNull] Expression<TDelegate> expression, string ParameterName)
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

        [NotNull] protected override Expression VisitConstant(ConstantExpression c) => Expression.Constant(0);

        //protected override Expression VisitParameter(ParameterExpression p) => base.VisitParameter(p);

        protected override Expression VisitBinary([NotNull] BinaryExpression b)
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
}