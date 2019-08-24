using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public class CopmlexConstantExpression : ComplexExpression
    {
        private readonly Expression _Re;
        private readonly Expression _Im;

        internal CopmlexConstantExpression(double Re, double Im) : this(Constant(Re), Constant(Im)) { }

        internal CopmlexConstantExpression(Expression Re, Expression Im) { _Re = Re; _Im = Im; }

        protected override Expression GetRe() { return _Re; }
        protected override Expression GetIm() { return _Im; }
    }
}