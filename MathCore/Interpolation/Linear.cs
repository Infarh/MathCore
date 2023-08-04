namespace MathCore.Interpolation;

public class Linear(double[] X, double[] Y) : Interpolator, IInterpolator
{
    public static double Interpolate(double x, double x1, double y1, double x2, double y2) => y1 + (x - x1) * (y2 - y1) / (x2 - x1);

    public double this[double x] => Value(x);

    public double Value(double x)
    {
        var i1 = 0;
        var i2 = X.Length - 1;
        if (x < X[i1]) i2     = 1;
        else if(x > X[i2]) i1 = i2 - 1;
        else
            do
            {
                if(X[i1] == x) return Y[i1];
                if(X[i2] == x) return Y[i2];

                var i  = (i1 + i2) / 2;
                var xi = X[i];
                if(xi == x) return Y[i];

                if(x > xi)
                    i1 = i;
                else
                    i2 = i;
            } while(i2 - i1 > 1);

        return Mapping.GetValue(x, X[i1], X[i2], Y[i1], Y[i2]);
    }

    public Func<double, double> GetFunction() => Value;
}