using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore.Functions.Differentiable
{
    public abstract class OperatorBinary : OperatorUnary
    {
        protected readonly Function f2;
        protected OperatorBinary(Function f1, Function f2) : base(f1) { this.f2 = f2; }
    }

    public class Addition : OperatorBinary
    {
        public Addition(Function f1, Function f2) : base(f1, f2) { }
        public override double Value(double x) => f1.Value(x) + f2.Value(x);
        public override Function Derivative() => f1.Derivative() + f2.Derivative();
    }

    public class Substraction : OperatorBinary
    {
        public Substraction(Function f1, Function f2) : base(f1, f2) { }
        public override double Value(double x) => f1.Value(x) - f2.Value(x);
        public override Function Derivative() => f1.Derivative() - f2.Derivative();
    }

    public class Multipycation : OperatorBinary
    {
        public Multipycation(Function f1, Function f2) : base(f1, f2) { }
        public override double Value(double x) => f1.Value(x) * f2.Value(x);
        public override Function Derivative() => f1.Derivative() * f2 + f1 * f2.Derivative();
    }

    public class Division : OperatorBinary
    {
        public Division(Function f1, Function f2) : base(f1, f2) { }
        public override double Value(double x) => f1.Value(x) / f2.Value(x);
        public override Function Derivative() => (f1.Derivative() * f2 - f1 * f2.Derivative()) / (f2 * f2);
    }
}
