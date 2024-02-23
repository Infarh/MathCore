#nullable enable

// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.Functions.Differentiable;

[Copyright("http://habrahabr.ru/post/149630/")]
public abstract class Function
{
    public abstract double Value(double x);
    public abstract Function Derivative();
    public static Function operator +(Function f1, Function f2) => new Addition(f1, f2);
    public static Function operator -(Function f1, Function f2) => new Subtraction(f1, f2);
    public static Function operator *(Function f1, Function f2) => new Multiplication(f1, f2);
    public static Function operator /(Function f1, Function f2) => new Division(f1, f2);
    public static Function operator ^(Function f1, Constant c) => new OperatorPowerOf(f1, c);
}

public class Constant(double c) : Function
{
    private readonly double _C = c;
    // ReSharper disable once UnusedMember.Global
    public double C => _C;
    public override double Value(double x) => _C;
    public override Function Derivative() => new Zero();
    public static implicit operator Constant(double c) => new(c);
    public static implicit operator double(Constant c) => c._C;
}

public class Zero() : Constant(0);
public class One() : Constant(1);

public class Identity : Function
{
    public override double Value(double x) => x;
    public override Function Derivative() => new One();
}