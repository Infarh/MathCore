using MathCore.Annotations;

namespace MathCore.Functions.Differentiable;

public abstract class OperatorUnary : Function
{
    protected readonly Function _F1;
    protected OperatorUnary(Function f1) => _F1 = f1;
}

public abstract class OperatorUnaryConst : OperatorUnary
{
    protected readonly Constant _C;
    protected OperatorUnaryConst(Function f1, Constant c) : base(f1) => _C = c;
}

public class OperatorPowerOf : OperatorUnaryConst
{
    public OperatorPowerOf(Function f1, Constant c) : base(f1, c) { }
    public override double Value(double x) => Math.Pow(_F1.Value(x), _C);
    [NotNull] public override Function Derivative() => _C * _F1 ^ (_C - 1);
}