namespace MathCore.Values;

public class Goertzel : IResettable
{
    private readonly Complex _W;
    
    private double _s1;
    
    private double _s2;

    public double State1 => _s1;

    public double State2 => _s2;

    public Complex State { get; private set; }

    public Goertzel(double f0) => _W = Complex.Exp(Consts.pi2 * f0);

    public void Reset()
    {
        _s1 = 0;
        _s2 = 0;
    }

    public Complex Add(double x)
    {
        var s = x + 2 * _W.Re * _s1 - _s2;
        var y = _W * s - _s1;
        State = y;

        _s2 = _s1;
        _s1 = s;

        return y;
    }
}
