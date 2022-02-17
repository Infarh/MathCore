using System.Numerics;

namespace ConsoleTest.Recursion;

public static class TailRecursion
{
    public static RecursionResult<BigInteger> Factorial(int n, BigInteger product) =>
        n < 2
            ? Return(product)
            : Next(() => Factorial(n - 1, n * product));

    public static T Execute<T>(Func<RecursionResult<T>> func)
    {
        while (true)
        {
            var recursion_result = func();
            if (recursion_result.IsFinalResult)
                return recursion_result.Result;
            func = recursion_result.NextStep;
        }
    }

    public static RecursionResult<T> Return<T>(T result) => new(true, result, null);

    public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> NextStep) => new(false, default, NextStep);
}

public class RecursionResult<T>
{
    private readonly bool _IsFinalResult;
    
    private readonly T _Result;
    
    private readonly Func<RecursionResult<T>> _NextStep;
    
    internal RecursionResult(bool IsFinalResult, T result, Func<RecursionResult<T>> NextStep)
    {
        _IsFinalResult = IsFinalResult;
        _Result = result;
        _NextStep = NextStep;
    }

    public bool IsFinalResult => _IsFinalResult;

    public T Result => _Result;
    
    public Func<RecursionResult<T>> NextStep => _NextStep;
}
