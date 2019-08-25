using System;
using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public class ComplexLambdaBinaryExpression : ComplexBinaryExpression
    {
        private readonly Func<ComplexExpression, ComplexExpression, Expression> _GetRe;
        private readonly Func<ComplexExpression, ComplexExpression, Expression> _GetIm;

        public ComplexLambdaBinaryExpression(ComplexExpression Left, ComplexExpression Right,
                    Func<ComplexExpression, ComplexExpression, Expression> GetRe, Func<ComplexExpression, ComplexExpression, Expression> GetIm)
                    : base(Left, Right)
        {
            _GetRe = GetRe;
            _GetIm = GetIm;
        }

        protected override Expression GetRe() => _GetRe(Left, Right);

        protected override Expression GetIm() => _GetIm(Left, Right);
    }
}