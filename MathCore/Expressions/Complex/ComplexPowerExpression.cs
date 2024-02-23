#nullable enable
using System.Linq.Expressions;

namespace MathCore.Expressions.Complex;

public sealed class ComplexPowerExpression(ComplexExpression Left, ComplexExpression Right)
    : ComplexBinaryExpression(Left, Right)
{
    private Expression? _Mod;
    private Expression? _Arg;

    private Expression abs => _Mod ??= Multiply(GetPower(Left.Power, Divide(Right.Re, 2)), GetPower(Math.E, Divide(Left.Arg, Right.Im)));
    private Expression arg => _Arg ??= Add(Multiply(Left.Arg, Right.Re), Multiply(GetLog(Left.Abs), Right.Im));

    protected override Expression GetRe() => Multiply(abs, GetCos(arg));

    protected override Expression GetIm() => Multiply(abs, GetSin(arg));
}