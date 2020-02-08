using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public class ComplexConstantExpression : ComplexExpression
    {
        private readonly Expression _Re;
        private readonly Expression _Im;

        internal ComplexConstantExpression(double Re, double Im) : this(Constant(Re), Constant(Im)) { }

        internal ComplexConstantExpression(Expression Re, Expression Im) { _Re = Re; _Im = Im; }

        protected override Expression GetRe() => _Re;
        protected override Expression GetIm() => _Im;
    }
}