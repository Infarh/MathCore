using System;
using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public class ComplexLambdaUnaryExpression : ComplexUnaryExpression
    {
        private readonly Func<ComplexExpression, Expression> _GetRe;
        private readonly Func<ComplexExpression, Expression> _GetIm;

        public ComplexLambdaUnaryExpression(ComplexExpression Value,
                    Func<ComplexExpression, Expression> GetRe, Func<ComplexExpression, Expression> GetIm)
                    : base(Value)
        {
            _GetRe = GetRe;
            _GetIm = GetIm;
        }

        protected override Expression GetRe() => _GetRe(Value);

        protected override Expression GetIm() => _GetIm(Value);
    }
}