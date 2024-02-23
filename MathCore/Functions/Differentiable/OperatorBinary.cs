#nullable enable
namespace MathCore.Functions.Differentiable;

public abstract class OperatorBinary(Function f1, Function f2) : OperatorUnary(f1)
{
    protected readonly Function _F2 = f2;
}

public class Addition(Function f1, Function f2) : OperatorBinary(f1, f2)
{
    public override double Value(double x) => _F1.Value(x) + _F2.Value(x);
    public override Function Derivative() => _F1.Derivative() + _F2.Derivative();
}

public class Subtraction(Function f1, Function f2) : OperatorBinary(f1, f2)
{
    public override double Value(double x) => _F1.Value(x) - _F2.Value(x);
    public override Function Derivative() => _F1.Derivative() - _F2.Derivative();
}

public class Multiplication(Function f1, Function f2) : OperatorBinary(f1, f2)
{
    public override double Value(double x) => _F1.Value(x) * _F2.Value(x);
    public override Function Derivative() => _F1.Derivative() * _F2 + _F1 * _F2.Derivative();
}

public class Division(Function f1, Function f2) : OperatorBinary(f1, f2)
{
    public override double Value(double x) => _F1.Value(x) / _F2.Value(x);
    public override Function Derivative() => (_F1.Derivative() * _F2 - _F1 * _F2.Derivative()) / (_F2 * _F2);
}