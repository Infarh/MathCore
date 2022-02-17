namespace MathCore.Algorithms.Numbers.Fibonachi;

public static class FibonaachiNumbers
{
    public static void RunTest()
    {
        //var fff = SpecialFunctions.Fibonacci(7);

        //var ff = EnumFib().ElementAt(8);

        //var fib = new int[10];
        //for (var m = 0; m < fib.Length; m++)
        //{
        //    fib[m] = Fib(m);
        //}

        //Array.Clear(fib, 0, fib.Length);
        //Fib(fib);
    }

    public static int Fib(int n) => n switch
    {
        0 => 1,
        1 => 1,
        _ => Fib(n - 1) + Fib(n - 2)
    };

    public static void Fib(int[] F)
    {
        for (var i = 0; i < F.Length; i++)
            F[i] = i switch
            {
                0 => 1,
                1 => 1,
                _ => F[i - 1] + F[i - 2]
            };
    }

    public static IEnumerable<int> EnumFib()
    {
        yield return 1;
        yield return 1;
        var last1 = 1;
        var last2 = 1;
        while (true)
        {
            var fib = last1 + last2;
            yield return fib;
            last1 = last2;
            last2 = fib;
        }
    }
}