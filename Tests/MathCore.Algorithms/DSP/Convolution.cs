namespace MathCore.Algorithms.DSP;

public static class Convolution
{
    public static double[] GetConvolution(this double[] x, params double[] h)
    {
        var y = new double[x.Length + h.Length - 1];

        for (var n = 0; n < y.Length; n++)
        {
            var yn = 0d;

            for (var k = 0; k < n && n - k >= 0; k++) 
                yn += h[k] * x[n - k];

            y[n] = yn;
        }

        x.EnumConvolution(y).ToArray();

        return y;
    }

    public static IEnumerable<double> EnumConvolution(this double[] x, params double[] h)
    {
        var x_length = x.Length;
        var h_length = h.Length;
        var length = x_length + h_length - 1;

        for (var n = 0; n < length; n++)
        {
            var yn = 0d;
            for (var k = 0; k < n && n - k >= 0; k++) 
                yn += h[k] * x[n - k];

            yield return yn;
        }
    }
}