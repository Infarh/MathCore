using System;

namespace MathCore.Functions.Differentiable;

public class Sinus : Function
{
    public override double Value(double x) => Math.Sin(x);
    public override Function Derivative() => new Cousins();
}

public class Cousins : Function
{
    public override double Value(double x) => Math.Cos(x);
    public override Function Derivative() => new Sinus();
}

public class Tangent : Division { public Tangent() : base(new Sinus(), new Cousins()) { } }
public class Cotangent : Division { public Cotangent() : base(new Cousins(), new Sinus()) { } }

public class Exponent : Function
{
    public override double Value(double x) => Math.Exp(x);
    public override Function Derivative() => new Exponent();
}

public class HyperbolicSinus : Division
{
    public HyperbolicSinus()
        : base(new Exponent() - new One() / new Exponent(), (Constant)2)
    { }
}

public class HyperbolicCosines : Division
{
    public HyperbolicCosines()
        : base(new Exponent() + new One() / new Exponent(), (Constant)2)
    { }
}

public class HyperbolicTangent : Division { public HyperbolicTangent() : base(new HyperbolicSinus(), new HyperbolicCosines()) { } }

public class HyperbolicCotangent : Division { public HyperbolicCotangent() : base(new HyperbolicCosines(), new HyperbolicSinus()) { } }