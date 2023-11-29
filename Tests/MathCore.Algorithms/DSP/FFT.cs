namespace MathCore.Algorithms.DSP;

public static class FFT
{
    public static Complex[] TransformRecursive(double[] input)
    {
        var n = input.Length;
        var n05 = n / 2;

        // Base case
        if (n == 1) return [input[0]];

        // Splitting the even and odd indexed elements
        var even = new double[n05];
        var odd = new double[n05];
        for (var i = 0; i < n05; i++)
            (even[i], odd[i]) = (input[2 * i], input[2 * i + 1]);

        // Recursively computing the FFT
        var even_fft = TransformRecursive(even);
        var odd_fft = TransformRecursive(odd);

        // Computing the FFT result
        var result = new Complex[n];
        var w0 = -Consts.pi2 / n;
        for (var k = 0; k < n05; k++)
        {
            var w = Complex.Exp(w0 * k);

            result[k] = even_fft[k] + w * odd_fft[k];
            result[n05 + k] = even_fft[k] - w * odd_fft[k];
        }
        return result;
    }

    public static void Test()
    {
        double[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        var fft = TransformRecursive(input);

        Console.WriteLine("FFT result:");
        foreach (var c in fft)
            Console.WriteLine(c);
    }
}
