using System;
using System.Linq.Expressions;
using MathCore.Annotations;

namespace MathCore.Expressions.Complex
{
    public sealed class ComplexPowerExpression : ComplexBinaryExpression
    {
        [CanBeNull] private Expression _Mod;
        [CanBeNull] private Expression _Arg;

        [NotNull] private Expression abs => _Mod ??= Multiply(GetPower(Left.Power, Divide(Right.Re, 2)), GetPower(Math.E, Divide(Left.Arg, Right.Im)));
        [NotNull] private Expression arg => _Arg ??= Add(Multiply(Left.Arg, Right.Re), Multiply(GetLog(Left.Abs), Right.Im));
        public ComplexPowerExpression(ComplexExpression Left, ComplexExpression Right) : base(Left, Right) { }

        protected override Expression GetRe() => Multiply(abs, GetCos(arg));

        protected override Expression GetIm() => Multiply(abs, GetSin(arg));
    }
}