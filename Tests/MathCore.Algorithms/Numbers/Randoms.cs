namespace MathCore.Algorithms.Numbers;

public static class Randoms
{
    public static double[] Uniform(int Count, double s0, double z0)
    {
        // http://blog.kislenko.net/show.php?id=1337

        var result = new double[Count];
        var zz = z0;
        result[0] = s0;

        for (var i = 1; i < Count - 1; i++)
        {
            result[i] = result[i] / zz + Math.PI;
            result[i + 1] = result[i] - (int)result[i];
            zz += 1e-8;
        }

        return result;
    }

    public static IEnumerable<double> EnumUniform(int Count, double s0, double z0)
    {
        if(Count == 0)
            yield break;

        var r = s0;
        var z = z0;

        var count = Count;
        while (Count < 0 || count >= 0)
        {
            if (Count > 0)
                count--;

            var result = r / z + Math.PI;
            yield return result;
            r = result - (int)result;
            z += 1e-8;
        }
    }
}
