using System;
using System.Linq.Expressions;

namespace MathCore.Expressions.Complex
{
    public sealed class ComplexPowerExpression : ComplexBinaryExpression
    {
        private Expression _r;
        private Expression _arg;

        private Expression r => _r ?? (_r = Multiply(GetPower(Left.Power, Divide(Right.Re, 2)), GetPower(Math.E, Divide(Left.Arg, Right.Im))));
        private Expression arg => _arg ?? (_arg = Add(Multiply(Left.Arg, Right.Re), Multiply(GetLog(Left.Abs), Right.Im)));
        public ComplexPowerExpression(ComplexExpression Left, ComplexExpression Right) : base(Left, Right) { }

        protected override Expression GetRe() => Multiply(r, GetCos(arg));

        protected override Expression GetIm() => Multiply(r, GetSin(arg));
    }
}