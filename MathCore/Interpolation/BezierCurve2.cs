using System;
using MathCore.Annotations;

namespace MathCore.Interpolation;

internal class BezierCurve2
{
    private double[] _FactorialLookup;

    public BezierCurve2() => CreateFactorialTable();

    // just check if n is appropriate, then return the result
    private double FastFactorial(int n)
    {
        if(n < 0) throw new Exception("n is less than 0");
        if(n > 32) return n.Factorial();

        return _FactorialLookup[n];
    }

    // create lookup table for fast Factorial calculation
    private void CreateFactorialTable() => _FactorialLookup = new[]
    {
        1d,
        1d,
        2d,
        6d,
        24d,
        120d,
        720d,
        5040d,
        40320d,
        362880d,
        3628800d,
        39916800d,
        479001600d,
        6227020800d,
        87178291200d,
        1307674368000d,
        20922789888000d,
        355687428096000d,
        6402373705728000d,
        121645100408832000d,
        2432902008176640000d,
        51090942171709440000d,
        1124000727777607680000d,
        25852016738884976640000d,
        620448401733239439360000d,
        15511210043330985984000000d,
        403291461126605635584000000d,
        10888869450418352160768000000d,
        304888344611713860501504000000d,
        8841761993739701954543616000000d,
        265252859812191058636308480000000d,
        8222838654177922817725562880000000d,
        263130836933693530167218012160000000d
    };

    private double Ni(int n, int i) => FastFactorial(n) / (FastFactorial(i) * FastFactorial(n - i));

    /// <summary>Calculate Bernstein basis</summary>
    private double Bernstein(int n, int i, double t)
    {
        /* Prevent problems with pow */
        var ti  = t.Equals(0d) && i == 0 ? 1.0 : Math.Pow(t, i);         // t^i
        var tni = n == i && t.Equals(1d) ? 1.0 : Math.Pow(1 - t, n - i); // (1 - t)^i
        return Ni(n, i) * ti * tni;
    }

    public void Bezier2D([NotNull]double[] b, int CPts, [NotNull]double[] p)
    {
        var n_pts = b.Length / 2;

        // Calculate points on curve

        var i_count = 0;
        var t       = 0d;
        var step    = 1d / (CPts - 1);

        for(var i1 = 0; i1 != CPts; i1++)
        {
            if(1.0 - t < 5e-6)
                t = 1.0;

            var j_count = 0;
            p[i_count]     = 0.0;
            p[i_count + 1] = 0.0;
            for(var i = 0; i != n_pts; i++)
            {
                var basis = Bernstein(n_pts - 1, i, t);
                p[i_count]     += basis * b[j_count];
                p[i_count + 1] += basis * b[j_count + 1];
                j_count        += 2;
            }

            i_count += 2;
            t       += step;
        }
    }
}