using System;
using System.Collections.Generic;
using System.Text;

namespace MathCore.Functions.Differentiable
{
    public class Sinus : Function
    {
        public override double Value(double x) => Math.Sin(x);
        public override Function Derivative() => new Cosinus();
    }

    public class Cosinus : Function
    {
        public override double Value(double x) => Math.Cos(x);
        public override Function Derivative() => new Sinus();
    }

    public class Tangens : Division { public Tangens() : base(new Sinus(), new Cosinus()) { } }
    public class Cotangens : Division { public Cotangens() : base(new Cosinus(), new Sinus()) { } }

    public class Exponenta : Function
    {
        public override double Value(double x) => Math.Exp(x);
        public override Function Derivative() => new Exponenta();
    }

    public class HiperbolicSinus : Division
    {
        public HiperbolicSinus()
            : base(new Exponenta() - new One() / new Exponenta(), (Constant)2)
        { }
    }

    public class HiperbolicCosinus : Division
    {
        public HiperbolicCosinus()
            : base(new Exponenta() + new One() / new Exponenta(), (Constant)2)
        { }
    }

    public class HiperbolicTangens : Division { public HiperbolicTangens() : base(new HiperbolicSinus(), new HiperbolicCosinus()) { } }
    public class HiperbolicCotangens : Division { public HiperbolicCotangens() : base(new HiperbolicCosinus(), new HiperbolicSinus()) { } }
}
