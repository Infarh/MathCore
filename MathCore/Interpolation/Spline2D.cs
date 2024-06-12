namespace MathCore.Interpolation;

[Copyright("Scott W Harden", url = "https://swharden.com/blog/2022-01-22-spline-interpolation/")]
public class Spline2D
{
    /// <summary>Generate a smooth (interpolated) curve that follows the path of the given X/Y points</summary>
    public static (double[] xs, double[] ys) InterpolateXY(double[] xs, double[] ys, int count)
    {
        if (xs is null || ys is null || xs.Length != ys.Length)
            throw new ArgumentException($"{nameof(xs)} and {nameof(ys)} must have same length");

        var input_point_count = xs.Length;
        var input_distances = new double[input_point_count];

        for (var i = 1; i < input_point_count; i++)
        {
            var dx = xs[i] - xs[i - 1];
            var dy = ys[i] - ys[i - 1];

            var distance = Math.Sqrt(dx * dx + dy * dy);

            input_distances[i] = input_distances[i - 1] + distance;
        }

        var mean_distance = input_distances[input_point_count - 1] / (count - 1);
        //var even_distances = Enumerable.Range(0, count).Select(x => x * mean_distance).ToArray();
        var even_distances = new double[count].Initialize(mean_distance, (i, m) => i * m);


        var xs_out = Interpolate(input_distances, xs, even_distances);
        var ys_out = Interpolate(input_distances, ys, even_distances);
        return (xs_out, ys_out);
    }

    private static double[] Interpolate(double[] XOrig, double[] YOrig, double[] XInterp)
    {
        var (a, b) = FitMatrix(XOrig, YOrig);

        var y_interp = new double[XInterp.Length];

        for (var i = 0; i < y_interp.Length; i++)
        {
            int j;
            for (j = 0; j < XOrig.Length - 2; j++)
                if (XInterp[i] <= XOrig[j + 1])
                    break;

            var dx = XOrig[j + 1] - XOrig[j];
            var t = (XInterp[i] - XOrig[j]) / dx;
            var y = (1 - t) * YOrig[j] + t * YOrig[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
            y_interp[i] = y;
        }

        return y_interp;
    }

    private static (double[] a, double[] b) FitMatrix(double[] x, double[] y)
    {
        var n = x.Length;
        var a = new double[n - 1];
        var b = new double[n - 1];
        var r = new double[n];
        var A = new double[n];
        var B = new double[n];
        var C = new double[n];

        var dx1 = x[1] - x[0];
        C[0] = 1 / dx1;
        B[0] = 2 * C[0];
        r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);

        double dy1;
        for (var i = 1; i < n - 1; i++)
        {
            dx1 = x[i] - x[i - 1];
            var dx2 = x[i + 1] - x[i];

            A[i] = 1.0f / dx1;
            C[i] = 1.0f / dx2;
            B[i] = 2.0f * (A[i] + C[i]);

            dy1 = y[i] - y[i - 1];
            var dy2 = y[i + 1] - y[i];

            r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
        }

        dx1 = x[n - 1] - x[n - 2];
        dy1 = y[n - 1] - y[n - 2];

        A[n - 1] = 1 / dx1;
        B[n - 1] = 2 * A[n - 1];
        r[n - 1] = 3 * (dy1 / (dx1 * dx1));

        var c_prime = new double[n];
        c_prime[0] = C[0] / B[0];
        for (var i = 1; i < n; i++)
            c_prime[i] = C[i] / (B[i] - c_prime[i - 1] * A[i]);

        var d_prime = new double[n];
        d_prime[0] = r[0] / B[0];
        for (var i = 1; i < n; i++)
            d_prime[i] = (r[i] - d_prime[i - 1] * A[i]) / (B[i] - c_prime[i - 1] * A[i]);

        var k = new double[n];
        k[n - 1] = d_prime[n - 1];
        for (var i = n - 2; i >= 0; i--)
            k[i] = d_prime[i] - c_prime[i] * k[i + 1];

        for (var i = 1; i < n; i++)
        {
            dx1 = x[i] - x[i - 1];
            dy1 = y[i] - y[i - 1];

            a[i - 1] = k[i - 1] * dx1 - dy1;
            b[i - 1] = -k[i] * dx1 + dy1;
        }

        return (a, b);
    }
}
