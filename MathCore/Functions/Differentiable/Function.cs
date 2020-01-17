using System;
using MathCore.Annotations;

// ReSharper disable MemberCanBeProtected.Global

namespace MathCore.Functions.Differentiable
{
    [Copyright("http://habrahabr.ru/post/149630/")]
    public abstract class Function
    {
        public abstract double Value(double x);
        public abstract Function Derivative();
        [NotNull] public static Function operator +(Function f1, Function f2) => new Addition(f1, f2);
        [NotNull] public static Function operator -(Function f1, Function f2) => new Subtraction(f1, f2);
        [NotNull] public static Function operator *(Function f1, Function f2) => new Multiplication(f1, f2);
        [NotNull] public static Function operator /(Function f1, Function f2) => new Division(f1, f2);
        [NotNull] public static Function operator ^(Function f1, Constant c) => new OperatorPowerOf(f1, c);
    }

    public class Constant : Function
    {
        private readonly double _C;
        // ReSharper disable once UnusedMember.Global
        public double C => _C;
        public Constant(double c) => _C = c;
        public override double Value(double x) => _C;
        [NotNull] public override Function Derivative() => new Zero();
        [NotNull] public static implicit operator Constant(double c) => new Constant(c);
        public static implicit operator double([NotNull] Constant c) => c._C;
    }

    public class Zero : Constant { public Zero() : base(0) { } }
    public class One : Constant { public One() : base(1) { } }

    public class Identity : Function
    {
        public override double Value(double x) => x;
        [NotNull] public override Function Derivative() => new One();
    }
}