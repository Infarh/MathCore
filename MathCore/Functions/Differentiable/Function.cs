using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore.Functions.Differentiable
{
    [Copyright("http://habrahabr.ru/post/149630/")]
    public abstract class Function
    {
        public abstract double Value(double x);
        public abstract Function Derivative();
        public static Function operator +(Function f1, Function f2) => new Addition(f1, f2);
        public static Function operator -(Function f1, Function f2) => new Substraction(f1, f2);
        public static Function operator *(Function f1, Function f2) => new Multipycation(f1, f2);
        public static Function operator /(Function f1, Function f2) => new Division(f1, f2);
        public static Function operator ^(Function f1, Constant c) => new OperatorPowerOf(f1, c);
    }

    public class Constant : Function
    {
        public readonly double C;
        public Constant(double c) { C = c; }
        public override double Value(double x) => C;
        public override Function Derivative() => new Zero();
        public static implicit operator Constant(double c) => new Constant(c);
        public static implicit operator double(Constant c) => c.C;
    }

    public class Zero : Constant { public Zero() : base(0) { } }
    public class One : Constant { public One() : base(1) { } }

    public class Identity : Function
    {
        public override double Value(double x) => x;
        public override Function Derivative() => new One();
    }
}
