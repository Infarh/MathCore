using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore.Functions.Differentiable
{
    public abstract class OperatorUnary : Function
    {
        protected readonly Function f1;
        protected OperatorUnary(Function f1) { this.f1 = f1; }
    }

    public abstract class OperatorUnaryConst : OperatorUnary
    {
        protected readonly Constant c;
        protected OperatorUnaryConst(Function f1, Constant c) : base(f1) { this.c = c; }
    }

    public class OperatorPowerOf : OperatorUnaryConst
    {
        public OperatorPowerOf(Function f1, Constant c) : base(f1, c) { }
        public override double Value(double x) => Math.Pow(f1.Value(x), c);
        public override Function Derivative() => c * f1 ^ (c - 1);
    }
}
