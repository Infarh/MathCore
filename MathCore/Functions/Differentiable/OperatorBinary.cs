using MathCore.Annotations;

namespace MathCore.Functions.Differentiable;

public abstract class OperatorBinary : OperatorUnary
{
    protected readonly Function _F2;
    protected OperatorBinary(Function f1, Function f2) : base(f1) => this._F2 = f2;
}

public class Addition : OperatorBinary
{
    public Addition(Function f1, Function f2) : base(f1, f2) { }
    public override double Value(double x) => _F1.Value(x) + _F2.Value(x);
    [NotNull] public override Function Derivative() => _F1.Derivative() + _F2.Derivative();
}

public class Subtraction : OperatorBinary
{
    public Subtraction(Function f1, Function f2) : base(f1, f2) { }
    public override double Value(double x) => _F1.Value(x) - _F2.Value(x);
    [NotNull] public override Function Derivative() => _F1.Derivative() - _F2.Derivative();
}

public class Multiplication : OperatorBinary
{
    public Multiplication(Function f1, Function f2) : base(f1, f2) { }
    public override double Value(double x) => _F1.Value(x) * _F2.Value(x);
    [NotNull] public override Function Derivative() => _F1.Derivative() * _F2 + _F1 * _F2.Derivative();
}

public class Division : OperatorBinary
{
    public Division(Function f1, Function f2) : base(f1, f2) { }
    public override double Value(double x) => _F1.Value(x) / _F2.Value(x);
    [NotNull] public override Function Derivative() => (_F1.Derivative() * _F2 - _F1 * _F2.Derivative()) / (_F2 * _F2);
}