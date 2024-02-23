#nullable enable
namespace MathCore.Functions.Differentiable;

public abstract class OperatorUnary(Function f1) : Function
{
    protected readonly Function _F1 = f1;
}

public abstract class OperatorUnaryConst(Function f1, Constant c) : OperatorUnary(f1)
{
    protected readonly Constant _C = c;
}

public class OperatorPowerOf(Function f1, Constant c) : OperatorUnaryConst(f1, c)
{
    public override double Value(double x) => Math.Pow(_F1.Value(x), _C);
    public override Function Derivative() => _C * _F1 ^ (_C - 1);
}