namespace MathCore.Algorithms.Polynoms;

public static class NewtonPolynom
{
    public static void Test()
    {
        double[] X =
        {
            -3/2d,
            -3/4d,
            0,
            3/4d,
            3/2d
        };

        double[] Y =
        {
            -14.1014,
            -0.931596,
            0,
            0.931596,
            14.1014
        };

        var length = Y.Length;
        var len = length - 1;

        var delta = new double[length];
        for (var i = 0; i < length; i++)
            delta[len - i] = Y[i];

        for (var i = 1; i < length; i++)
            for (var j = len; j >= i; j--)
                delta[len - j] = (delta[len - j] - delta[length - j]) / (X[j] - X[j - i]);

        for (var i = length - 2; i >= 0; i--)
        {
            var x0 = -X[i];
            var d = delta[len - i];

            var j = len - i;
            delta[j] = delta[--j];

            while(j > 0)
                delta[j] = delta[j] * x0 + delta[--j];

            delta[0] = delta[0] * x0 + d;
        }
    }
}