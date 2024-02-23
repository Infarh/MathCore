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

public class Tangent() : Division(new Sinus(), new Cousins());
public class Cotangent() : Division(new Cousins(), new Sinus());

public class Exponent : Function
{
    public override double Value(double x) => Math.Exp(x);
    public override Function Derivative() => new Exponent();
}

public class HyperbolicSinus() : Division(new Exponent() - new One() / new Exponent(), (Constant)2);

public class HyperbolicCosines() : Division(new Exponent() + new One() / new Exponent(), (Constant)2);

public class HyperbolicTangent() : Division(new HyperbolicSinus(), new HyperbolicCosines());

public class HyperbolicCotangent() : Division(new HyperbolicCosines(), new HyperbolicSinus());